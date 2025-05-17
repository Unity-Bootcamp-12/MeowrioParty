using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// COMMENT : �÷��̾� �̵��� ���õ� ������ ó���ϸ� ��
// COMMENT : BoardManager�� ������ Tile�� �˷���

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: ���� ����� ���õ� �κ��̹Ƿ� BoardManager�� ���� �־�� ��.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: �Ʒ� ������ ���ʿ�

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 3f;

    public bool IsMoving { get { return _isMoving; } }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }
    public int RollDice()
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
    {
        if (!_isMoving)
        {
            _isMoving = true;
            Vector3 endPos = nextTile.transform.position;
            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        _isMoving = false;
                        currentTile = nextTile;
                    });
        }
    } 
}
