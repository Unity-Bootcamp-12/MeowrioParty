using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
// COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
// COMMENT : BoardManager가 가야할 Tile을 알려줌

public class Player : MonoBehaviour
{
    public int playerID; //일단 public으로 선언
    [SerializeField] private InputManagerSO _inputManager;

    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsMoving { get { return _isMoving; } }

    private Animator _animator;

    [SerializeField] private int _coin;
    [SerializeField] private int _star;

    public int Coin { get { return _coin; } }
    public int Star { get { return _star; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _inputManager.OnDiceButtonPerformed += OnDiceInputReceived;
    }
    private void OnDisable()
    {
        _inputManager.OnDiceButtonPerformed -= OnDiceInputReceived;
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    public int RollDice()
    {
        TurnOffDice();
        Debug.Log(gameObject + "Rolled Dice");
        StartCoroutine(RollDiceWithJump());
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
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
                        currentTile = nextTile;

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

    private void OnDiceInputReceived(int receivedID)
    {
        if (receivedID != playerID)
        {
            return;
        }
        BoardManager.Instance.OnPlayersInput(this);
    }
    public IEnumerator RollDiceWithJump()
    {

        //_animator.SetTrigger("Jump");

        //Vector3 jumpTarget = transform.position; // 제자리 점프
        //float jumpPower = 2f;
        //float duration = 0.5f;
        //transform.DOJump(jumpTarget, jumpPower, 1, duration);



        float animTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);
        // 문제점 1. 플레이어의 반복적인 움직임 명령을 보드매니저에서 통제하므로
        //           플레이어의 이동 명령 또한 보드매니저에서 반복문 이전에 통제해야 한다.
        //           현재는 턴 설정 페이즈에서만 점프하며, 라운드 진행 중에는 점프와 동시에 강제로 이동한다.
    }

    public void AddCoins(int amount)
    {
        _coin += amount;
        _coin = Mathf.Max(_coin, 0);
    }
    public void AddStars(int amount)
    {
        _star++;
        AddCoins(amount);
    }
}