using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

#region BoardGameService

public class BoardGameService
{
    public class Tile
    {

    }

    public class Board
    {
        Tile[] _tile;

        public Board(int tileNumber)
        {
            _tile = new Tile[tileNumber];
        }

        public Tile GetTile(int index)
        {
            return _tile[index];
        }
    }

    public class Player
    {
        private int _idNumber;
        private int _currentTileIndex;
        public int IdNumber => _idNumber;
        public int CurrentTileIndex => _currentTileIndex;

        public Player(int idNumber)
        {
            _idNumber = idNumber;
        }

        public void MoveTo(int tileIndex)
        {
            _currentTileIndex = tileIndex;
        }
    }

    private int _maxRound;
    private int _currentRound;
    private Board _board;
    private List<Player> _playerList;
    private Dictionary<Player, int> _diceNumberForSetPlayerOrderDic;
    private int _currentTurn;

    public Dictionary<Player, int> DiceNumberForSetPlayerOrderDic => _diceNumberForSetPlayerOrderDic;

    public void InitializeBoardGame(int maxRound, int tileCount)
    {
        _maxRound = maxRound;
        _currentRound = 1;
        _board = new Board(tileCount);
        _playerList = new List<Player>();
        _diceNumberForSetPlayerOrderDic = new Dictionary<Player, int>();
        _currentTurn = 0;
    }

    public void AddPlayer(int clientId)
    {
        _playerList.Add(new Player(clientId));
    }

    public Player GetPlayer(ulong clientId)
    {
        for (int i = 0; i< _playerList.Count; ++i)
        {
            if (_playerList[i].IdNumber == (int)clientId)
                return _playerList[i];
        }
        return null;
    }

    public void RollDiceForOrder(Player player)
    {
        _diceNumberForSetPlayerOrderDic[player] = RollDice(1, 6);
        Debug.Log("player : " + player.IdNumber + ", diceValue : " + _diceNumberForSetPlayerOrderDic[player]);

        if (_diceNumberForSetPlayerOrderDic.Count == _playerList.Count)
        {
            SetTurnOrder();
        }
    }

    public void BoardGameStart()
    {
        foreach (Player player in _playerList)
        {
            player.MoveTo(0);
        }
    }

    public bool IsSetOrderDone()
    {
        if (_diceNumberForSetPlayerOrderDic.Count == _playerList.Count)
            return true;
        else
            return false;
    }

    public int GetCurrentPlayerId()
    {
        return _playerList[_currentTurn].IdNumber;
    }

    private int RollDice(int minValue, int maxValue)
    {
        return UnityEngine.Random.Range(minValue, maxValue + 1);
    }

    private void SetTurnOrder()
    {
        var sortedDic = _diceNumberForSetPlayerOrderDic.OrderByDescending(p => p.Value).ToList();

        _playerList.Clear();
        foreach (var playerDiceNumPair in sortedDic)
        {
            _playerList.Add(playerDiceNumPair.Key);
        }
    }
}
#endregion

public enum GameState
{
    GameIntro,
    GameReady,
    GamePlay,
    GameEnd,
}

public class BoardManager : NetSingleton<BoardManager>
{
    public InputManagerSO inputManager;

    [SerializeField] private GameState _currentState;
    [SerializeField] private int _maxRound;
    [SerializeField] private int _maxPlayerNumber;
    [SerializeField] private int _maxTileCount;

    public GameState CurrentState => _currentState;

    [SerializeField] private Board _board;

    public Board Board => _board;

    [SerializeField] private List<GameObject> _spawnPointList;

    public List<GameObject> characterPrefabList;
    private BoardGameService _gameService;

    public event Action OnGameStart;
    public event Action OnSetOrderStart;
    public event Action OnBoardGameStart;
    public event Action<int> OnNextTurnStart;

    public override void Awake()
    {
        base.Awake();
        _gameService = new BoardGameService();
        _maxRound = 2;
        _maxPlayerNumber = 2;
        _maxTileCount = 8;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        _gameService.InitializeBoardGame(_maxRound, _maxTileCount);
    }

    public void StartSetOrderState()
    {
        _currentState = GameState.GameReady;
        OnSetOrderStart?.Invoke();
    }

    public void ProcessPlayerInput(ulong clientId)
    {
        if (_currentState == GameState.GameReady)
        {
            _gameService.RollDiceForOrder(_gameService.GetPlayer(clientId));
            if (_gameService.IsSetOrderDone())
            {
                _currentState = GameState.GamePlay;
                _gameService.BoardGameStart();
                OnBoardGameStart?.Invoke();
            }
        }
    }

    public int GetDiceNumberForOrder(ulong clientId)
    {
        BoardGameService.Player player = _gameService.GetPlayer(clientId);
        return _gameService.DiceNumberForSetPlayerOrderDic[player];
    }

    public int GetCurrentTileIndex(ulong clientId)
    {
        return _gameService.GetPlayer(clientId).CurrentTileIndex;
    }

    public void NextTurnStart()
    {
        int clientId = _gameService.GetCurrentPlayerId();
        OnNextTurnStart?.Invoke(clientId);
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        _gameService.AddPlayer((int)clientId);

        if (IsServer)
        {
            CreatePlayerObjectRpc(clientId);
        }

        if (NetworkManager.Singleton.ConnectedClientsList.Count == _maxPlayerNumber)
        {
            GameStart();
        }
    }

    [Rpc(SendTo.Server)]
    private void CreatePlayerObjectRpc(ulong clientId)
    {
        int prefabIndex = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
        Vector3 spawnPos = _spawnPointList[prefabIndex].transform.position;
        GameObject playerObj = Instantiate(characterPrefabList[prefabIndex], spawnPos, Quaternion.identity);
        playerObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    private void GameStart()
    {
        _currentState = GameState.GameIntro;
        OnGameStart?.Invoke();
        CameraManager.Instance.ChangeCamera(0);
    }
}
