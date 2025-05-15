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


    public int RollDiceForOrder()
    {
        return RollDice();
    }

    //�ֻ����� ������ �ִϸ��̼��� ���� �� �÷��̾� �̵�
    public void RollDiceForMove()
    {
        RollDice(() =>
        {
            Move(_dice.DiceValue);
        });
    }

    private int RollDice(Action callback = null)
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll(callback);
    }

    private void Move(int diceValue)
    {
        StartCoroutine(MoveCoroutine(diceValue));
    }

    // comment: �Ʒ� ������ ���� �������� ��.
    private IEnumerator MoveCoroutine(int diceValue)
    {
        int index = currentTile.tileIndex;
        int nextIndex = 0;
        int leftIndex = diceValue-1;
        for (int i = 0; i < diceValue; i++)
        {
            _diceNumberObjects[leftIndex].SetActive(true);

            nextIndex = (++index) % BoardManager.Instance.Board.tiles.Length;
            Transform destination = BoardManager.Instance.Board.tiles[nextIndex].transform;
            transform.position = destination.position;
            yield return new WaitForSeconds(1f);
            _diceNumberObjects[leftIndex].SetActive(false);
            leftIndex--;
        }
        currentTile = BoardManager.Instance.Board.tiles[nextIndex];
    }
}
