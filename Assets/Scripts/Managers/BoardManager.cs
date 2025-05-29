using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    GameReady,
    GamePlay,
    GameEnd,
    MiniGame
}

public class BoardManager : NetSingleton<BoardManager>
{
    [Header("Variables")]
    [SerializeField] private NetworkVariable<GameState> _currentState;
    [SerializeField] private int _maxRound;
    [SerializeField] private NetworkVariable<int> _currentRound;
    [SerializeField] private NetworkList<ulong> _playerTurnOrder;
    [SerializeField] private ulong _currentPlayerId;

    private int _currentPlayerTurnIndex;
    private Dictionary<ulong, int> _playerDiceNumberList = new(); //순서 Dictionary
    public bool _canInput;
    private bool _isWaiting;

    //public bool CanInput => _canInput;

    [SerializeField] private Board _board;
    [SerializeField] private List<GameObject> _spawnPointList;
    [SerializeField] private List<GameObject> characterPrefabList;
    [SerializeField] private DataSO dataSO;

    private Dictionary<ulong, PlayerData> _playerDataMap = new();
    public Dictionary<ulong, PlayerController> _playerCtrlMap = new();

    private Action OnInitializeDone;
    private Action<ulong> OnSetOrderDone;

    public override void Awake()
    {
        base.Awake();

        //_maxRound = 2;
        _isWaiting = false;
        _playerTurnOrder = new NetworkList<ulong>();

        OnInitializeDone += OnInitializeDone_StartOpeningSequenceRpc;
        OnSetOrderDone += OnSetOrderDone_StartBoardGameSequenceServerRpc;
        _currentRound.OnValueChanged += ChangeRoundUIRpc;
    }

    public override void OnNetworkSpawn()
    {
        _board = FindFirstObjectByType<Board>();
        // 네트워크상에 스폰 시 초기값 설정 및 InitializePlayer에 개별 ClientID 전달
        if (IsServer)
        {

            _currentPlayerTurnIndex = 0;
            _currentState.Value = GameState.GameReady;
            _currentRound.Value = 0;

            StartCoroutine(CreatePlayerControllersCoroutine(NetworkManager.Singleton.ConnectedClientsList));
        }
    }

    IEnumerator CreatePlayerControllersCoroutine(IReadOnlyList<NetworkClient> clientList)
    {
        foreach (NetworkClient client in clientList)
        {
            ulong clientId = client.ClientId;
            Debug.Log("InitializePlayerRpc : " + clientId);

            int prefabIndex = (int)clientId;
            Vector3 spawnPos = _spawnPointList[prefabIndex].transform.position;

            GameObject playerObj = Instantiate(characterPrefabList[prefabIndex], spawnPos, Quaternion.identity);
            playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            while (client.PlayerObject == null)
            {
                yield return null;
            }

            PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();
            playerCtrl.ToggleDiceRpc(true);

            _playerCtrlMap[clientId] = playerCtrl;
            _playerDataMap[clientId] = new PlayerData(clientId);
            _playerDataMap[clientId].currentTile = _board.tileControllers[0];

            _playerDiceNumberList[clientId] = -1;
        }

        OnInitializeDone?.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    private void OnInitializeDone_StartOpeningSequenceRpc()
    {
        CameraManager.Instance.ChangeCamera(CameraType.Board);
        SoundManager.Instance.PlayBGM(BGMType.GameBGM);
        //UIManager.Instance.OpenNoticeUISec("파티 시작!", 3f);
        StartCoroutine(UIManager.Instance.OpenNoticeUIEveryoneSecCo("파티 시작!", 3f));
        StartCoroutine(OpeningCo());
    }

    private IEnumerator OpeningCo()
    {
        yield return new WaitForSeconds(3f);
        CameraManager.Instance.ChangeCamera(CameraType.Stage);
        StartCoroutine(UIManager.Instance.OpenNoticeUIEveryoneSecCo("순서를 정해보죠!", 3f));
        dataSO.UpdateLeaderBoardClient();
        //UIManager.Instance.OpenNoticeUISec("순서를 정해보죠!", 3f);
        _canInput = true;
    }

    [Rpc(SendTo.Server)]
    public void ProcessPlayerInputServerRpc(ulong clientId)
    {
        if (_canInput)
        {
            if (_currentState.Value == GameState.GameReady)
            {
                RollDiceForTurnOrderServerRpc(clientId);
                return;
            }
            else if (_currentState.Value == GameState.GamePlay)
            {
                RollDiceForBoardGameServerRpc(clientId);
                return;
            }
        }
    }

    [Rpc(SendTo.Server)] //클 -> 서 주사위 굴려줘
    public void RollDiceForTurnOrderServerRpc(ulong clientId)
    {
        if (_playerDiceNumberList[clientId] != -1) return;
        int diceValue = UnityEngine.Random.Range(1, 7);
        _playerDiceNumberList[clientId] = diceValue;

        Debug.Log("cliendId : " + clientId + ", diceValue : " + diceValue);

        RollDiceSequenceRpc(clientId, diceValue);

        //if all of players rolled dice for order
        if (_playerDiceNumberList.All(p => p.Value != -1))
        {
            StartCoroutine(SetTurnOrderEndingCoroutine());
        }
    }

    IEnumerator SetTurnOrderEndingCoroutine()
    {
        float waitTime = 3f;
        SetTurnOrderEndingSequenceRpc(waitTime);

        yield return new WaitForSeconds(waitTime);

        SetTurnOrder();

        _currentState.Value = GameState.GamePlay;
        _currentPlayerId = _playerTurnOrder[0];
        _currentRound.Value = 1;

        NoticeEveryoneSecRpc(_currentRound.Value + "라운드 시작~!", waitTime);

        yield return new WaitForSeconds(waitTime);

        //Player들 시작 타일로 이동
        foreach (var connectedClient in NetworkManager.Singleton.ConnectedClients)
        {
            _playerCtrlMap[connectedClient.Key].TransportPlayer(_board.tileControllers[0]);
            _playerCtrlMap[connectedClient.Key].TurnOffDiceNumberRpc();
        }
        _playerCtrlMap[_currentPlayerId].ToggleDiceRpc(true);

        OnSetOrderDone?.Invoke(_currentPlayerId);
    }

    [Rpc(SendTo.Everyone)]
    private void SetTurnOrderEndingSequenceRpc(float waitTime)
    {
        StartCoroutine(UIManager.Instance.OpenNoticeUIEveryoneSecCo("순서가 정해졌어요!", waitTime));
        //UIManager.Instance.OpenNoticeUISec("순서가 정해졌어요!", waitTime);
    }

    private void SetTurnOrder()
    {
        var sorted = _playerDiceNumberList.OrderByDescending(p => p.Value).ToList();
        _playerTurnOrder.Clear();
        foreach (var tempDic in sorted)
        {
            _playerTurnOrder.Add(tempDic.Key);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void OnSetOrderDone_StartBoardGameSequenceServerRpc(ulong clientId)
    {
        ChangeCameraSequenceRpc(CameraType.Focus, clientId);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeCameraSequenceRpc(CameraType type, ulong clientId)
    {
        CameraManager.Instance.ChangeCamera(type);
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId == clientId)
            {
                CameraManager.Instance.SetTarget(client.PlayerObject.transform);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RollDiceForBoardGameServerRpc(ulong clientId)
    {
        if (!IsPlayersTurn(clientId)) return;

        int diceValue = UnityEngine.Random.Range(1, 7);
        _playerCtrlMap[clientId].RollDiceSequenceRpc(diceValue);

        //CloseEveryoneNoticeUIRpc();

        StartCoroutine(SendTileCo(clientId, diceValue));
        _canInput = false;
    }

    private bool IsPlayersTurn(ulong clientId)
    {
        return clientId == _currentPlayerId;
    }

    private IEnumerator SendTileCo(ulong playerId, int diceValue)
    {
        PlayerData data = _playerDataMap[playerId];
        PlayerController controller = _playerCtrlMap[playerId];

        int tileIndex = data.currentTile.tileIndex;

        for (int i = 0; i < diceValue; i++) //한 타일씩 -> 나중에 갈림길 고려
        {
            int nextIndex = (tileIndex + 1) % _board.tileControllers.Length;
            TileController nextTileObj = _board.tileControllers[nextIndex];

            data.MoveTo(nextTileObj);
            controller.MoveTo(nextTileObj);
            controller.TurnOnDiceNumberRpc(diceValue - i);
            tileIndex = nextIndex;

            while (controller.IsMoving)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            controller.TurnOffDiceNumberRpc();
        }

        if (_board.tileControllers[tileIndex].tileType == ETileType.StarTile &&
    LeaderBoardManager.Instance.IsAffordStar(playerId))
        {
            _isWaiting = true;
            OpenExchangeStarUIRpc(playerId);

            WaitRpc();

            while (_isWaiting)
            {
                yield return null;
            }
        }

        _board.tileControllers[tileIndex].TileEventAtServer(data, controller);
        TileEffectRpc(tileIndex, playerId);

        yield return new WaitForSeconds(2f);

        NextTurn();
    }

    [Rpc(SendTo.Everyone)]
    private void WaitRpc()
    {
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        while (_isWaiting)
        {
            yield return null;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void BreakWaitRpc()
    {
        _isWaiting = false;
    }

    [Rpc(SendTo.Everyone)]
    private void TileEffectRpc(int tileIndex, ulong id)
    {
        ETileType currentTile = _board.tileControllers[tileIndex].TileEventLeaderBoard(id);
        if (!IsServer) return;
        if (currentTile == ETileType.CoinPlusTile)
        {
            _playerCtrlMap[id].TurnOnCoinPlusRpc();
        }
        else if (currentTile == ETileType.CoinMinusTile)
        {
            _playerCtrlMap[id].TurnOnCoinMinusRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void OpenExchangeStarUIRpc(ulong id)
    {
        UIManager.Instance.OpenExchangerStar(id);
    }

    private void NextTurn()
    {
        _currentPlayerTurnIndex++;

        if (_currentPlayerTurnIndex == _playerTurnOrder.Count)
        {
            _currentPlayerTurnIndex = 0;
            //_currentRound.Value++;

            StartMiniGame();
        }
        else
        {
            _currentPlayerId = _playerTurnOrder[_currentPlayerTurnIndex];
            _playerCtrlMap[_currentPlayerId].ToggleDiceRpc(true);
            ChangeCameraSequenceRpc(CameraType.Focus, _currentPlayerId);
            NextTurnSequenceRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void NextTurnSequenceRpc()
    {
        StartCoroutine(NextTurnSequenceCoroutine());
    }

    IEnumerator NextTurnSequenceCoroutine()
    {
        NoticeEveryoneSecRpc("주사위를 굴리세요", 3f);
        yield return new WaitForSeconds(3f);
        _canInput = true;
    }

    private void StartMiniGame()
    {
        //CloseEveryoneNoticeUIRpc();

        NoticeEveryoneSecRpc("미니게임 시작!" + "\n" + "SPACE를 빠르게 눌러 먼저 도착하세요!^0^", 5f);
        _currentState.Value = GameState.MiniGame;
        NetworkManager.Singleton.SceneManager.LoadScene("TapRaceScene", LoadSceneMode.Additive);
    }

    public void OnMiniGamePlayerFinished(ulong clientId)
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject = _playerCtrlMap[client.ClientId].GetComponent<NetworkObject>();
        }

        MiniGameFinishedSequenceRpc(clientId);
    }

    [Rpc(SendTo.Everyone)]
    private void MiniGameFinishedSequenceRpc(ulong clientId)
    {
        StartCoroutine(MiniGameFinishedSequenceCoroutine(clientId));
    }

    IEnumerator MiniGameFinishedSequenceCoroutine(ulong clientId)
    {
        CameraManager.Instance.ChangeCamera(CameraType.Board);
        string winnerName;
        switch ((int)clientId)
        {
            case 0:
                winnerName = "Mario";
                break;
            case 1:
                winnerName = "Luigi";
                break;
            case 2:
                winnerName = "Waluigi";
                break;
            case 3:
                winnerName = "Birdo";
                break;

            default:
                winnerName = "bug";
                break;
        }
        NoticeEveryoneSecRpc("우승자는 " + winnerName + "~!", 3f);
        yield return new WaitForSeconds(3f);
        StopMiniGame();
    }

    public void StopMiniGame()
    {
        if (!IsServer) return;

        _currentRound.Value++;

        if (_currentRound.Value > _maxRound)
        {
            int winnerClientId = LeaderBoardManager.Instance.GetHighestScoreClientId();

            _playerCtrlMap[(ulong)winnerClientId].transform.position = _spawnPointList[winnerClientId].transform.position;

            ChangeCameraSequenceRpc(CameraType.Focus, (ulong)winnerClientId);
            NoticeEveryoneRpc("우승!!");

            _playerCtrlMap[(ulong)winnerClientId].PlayAnimationRpc("Victory");
            SoundManager.Instance.PlaySFX(SFXType.Clear);

            _currentState.Value = GameState.GameEnd;
            return;
        }
        else
        {
            _currentState.Value = GameState.GamePlay;
            
            Scene scene = SceneManager.GetSceneByName("TapRaceScene");
            NetworkManager.Singleton.SceneManager?.UnloadScene(scene);

            NoticeEveryoneSecRpc(_currentRound.Value + "라운드 시작~!", 3f);

            _currentPlayerId = _playerTurnOrder[_currentPlayerTurnIndex];
            _playerCtrlMap[_currentPlayerId].ToggleDiceRpc(true);
            ChangeCameraSequenceRpc(CameraType.Focus, _currentPlayerId);
            NextTurnSequenceRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void CloseEveryoneNoticeUIRpc()
    {
        UIManager.Instance.CloseTargetUI<NoticeUI>();
    }

    [Rpc(SendTo.Everyone)]
    private void NoticeEveryoneRpc(string message)
    {
        UIManager.Instance.OpenNoticeUI(message);
    }

    [Rpc(SendTo.Server)]
    private void RollDiceSequenceRpc(ulong clientId, int diceValue)
    {
        _playerCtrlMap[clientId].RollDiceSequenceRpc(diceValue);
    }

    [Rpc(SendTo.Everyone)]
    private void NoticeEveryoneSecRpc(string message, float timer)
    {
        //UIManager.Instance.OpenNoticeUISec(message, timer);
        StartCoroutine(UIManager.Instance.OpenNoticeUIEveryoneSecCo(message, timer));
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeRoundUIRpc(int previous, int current)
    {
        if (current >= _maxRound) current = _maxRound;
        //일단 ui 숫자 안 넘어가게 여기서 처리함 나중에 엔딩 씬으로 넘기기 추가되면 괜찮을 듯 
        UIManager.Instance.CloseTargetUI<RoundUI>();
        RoundUIData roundUIData = new RoundUIData();
        roundUIData.currentRound = current.ToString();
        roundUIData.maxRound = _maxRound.ToString();

        UIManager.Instance.NoticeRoundUI(roundUIData);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateDataSORpc(ulong clientId)
    {
        LeaderBoardManager.Instance.UpdateCoin(clientId, -20);
        LeaderBoardManager.Instance.UpdateStar(clientId, 1);
    }

}
