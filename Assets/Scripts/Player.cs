using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System.Collections;
using System;
// COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
// COMMENT : BoardManager가 가야할 Tile을 알려줌

public class Player : NetworkBehaviour
{
    [SerializeField] private InputManagerSO _inputManager;

    [SerializeField] private Dice _dice;

    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsMoving { get { return _isMoving; } }

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _inputManager.OnConfirmButtonPerformed += OnConfirmButton;
    }
    private void OnDisable()
    {
        _inputManager.OnConfirmButtonPerformed -= OnConfirmButton;
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    public override void OnNetworkSpawn()
    {
        BoardManager.Instance.OnGameStart += () =>
        {
            GameStartSequenceRpc();
        };

        BoardManager.Instance.OnSetOrderStart += () =>
        {
            SetOrderStartSequenceRpc();
        };

        BoardManager.Instance.OnBoardGameStart += () =>
        {
            BoardGameStartSequenceRpc();
        };

        BoardManager.Instance.OnNextTurnStart += (clientId) =>
        {
            TurnStartSequenceRpc(clientId);
        };
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void GameStartSequenceRpc()
    {
        StartCoroutine(StartPlayCo());
    }

    IEnumerator StartPlayCo()
    {
        float showTime = 2f;
        _animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(showTime);
        _animator.SetBool("isMoving", false);
        BoardManager.Instance.StartSetOrderState();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetOrderStartSequenceRpc()
    {
        _dice.PlayDiceAnimationRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PopDiceNumberSequenceRpc(int diceNumber)
    {
        StartCoroutine(TurnOnDiceNumberCo(diceNumber));
    }

    IEnumerator TurnOnDiceNumberCo(int diceNumber)
    {
        TurnOnDiceNumber(diceNumber);
        TurnOffDice();
        yield return null;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BoardGameStartSequenceRpc()
    {
        TurnOffDiceNumber();
        int tileIndex = BoardManager.Instance.GetCurrentTileIndex(NetworkManager.Singleton.LocalClientId);
        MoveTo(BoardManager.Instance.Board.tiles[tileIndex], () =>
        {
            BoardManager.Instance.NextTurnStart();
        });
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TurnStartSequenceRpc(int clientId)
    {
        if ((int)NetworkManager.Singleton.LocalClientId == clientId)
        {
            CameraManager.Instance.ChangeCamera(1);
            CameraManager.Instance.SetTarget(transform);
            TurnOnDice();
            _dice.PlayDiceAnimationRpc();
        }
    }

    public void MoveTo(Tile nextTile, Action callback = null)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            _animator.SetBool("isMoving", true);
            Vector3 endPos = nextTile.transform.position;
            Vector3 direction = (endPos - transform.position).normalized;
            direction.y = 0f;
            transform.DOLookAt(endPos, 0.2f);

            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _isMoving = false;
                        callback?.Invoke();
                        _animator.SetBool("isMoving", false);
                    });
        }
    }

    public void TurnOnDiceNumber(int index)
    {
        TurnOffDiceNumber();

        if (_diceNumberObjects[index - 1] != null)
        {
            _diceNumberObjects[index - 1].SetActive(true);
        }

    }
    public void TurnOffDiceNumber()
    {
        foreach (var diceNumberObject in _diceNumberObjects)
        {
            diceNumberObject.SetActive(false);
        }

    }
    public void TurnOnDice()
    {
        _dice.gameObject.SetActive(true);
    }

    public void TurnOffDice()
    {
        _dice.gameObject.SetActive(false);
    }

    private void OnConfirmButton(object sender, bool isPressed)
    {
        if (!isPressed) { return; }

        if (IsOwner)
        {
            if (BoardManager.Instance.CurrentState == GameState.GameReady)
            {
                BoardManager.Instance.ProcessPlayerInput(NetworkManager.Singleton.LocalClientId);

                PopDiceNumberSequenceRpc(BoardManager.Instance.GetDiceNumberForOrder(NetworkManager.Singleton.LocalClientId));
            }
            else if (BoardManager.Instance.CurrentState == GameState.GamePlay)
            {
            }
        }
    }
}
