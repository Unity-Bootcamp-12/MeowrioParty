using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: ���� ����� ���õ� �κ��̹Ƿ� BoardManager�� ���� �־�� ��.

    [SerializeField] private Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: �Ʒ� ������ ���ʿ�

    // COMMENT : �÷��̾� �̵��� ���õ� ������ ó���ϸ� ��
    // COMMENT : BoardManager�� ������ Tile�� �˷���
    public void MoveToNextTile(Tile nextTile)
    {
        StartCoroutine(MoveCoroutine(nextTile));
    }

    public int RollDice()
    {
        return _dice.Roll();
    }

    // comment: �Ʒ� ������ ���� �������� ��.
    private IEnumerator MoveCoroutine(Tile nextTile)
    {
        transform.position = nextTile.transform.position;
        currentTile = nextTile;
        yield return new WaitForSeconds(1f);
    }
}
