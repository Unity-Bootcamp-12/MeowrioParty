using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// COMMENT : �÷��̾� �̵��� ���õ� ������ ó���ϸ� ��
// COMMENT : BoardManager�� ������ Tile�� �˷���

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: ���� ����� ���õ� �κ��̹Ƿ� BoardManager�� ���� �־�� ��.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: �Ʒ� ������ ���ʿ�

    private Queue<Tile> _moveQueue = new Queue<Tile>();
    private bool isMoving = false;
    [SerializeField] float moveSpeed = 5f;

    private void Update()
    {
        if (_moveQueue.Count > 0 && !isMoving) //�����̰� ���� �ʰ� ť�� ����
        {
            isMoving = true;
            StartCoroutine(MoveTileQueue());
        }
    }
    public int RollDice()
    {
        return _dice.Roll();
    }

    public void GetMoveQueue(Queue<Tile> tileQueue)
    {
        _moveQueue.Clear();
        _moveQueue = tileQueue;//Ÿ�� ť�� �޾ƿͼ�
    }

    private IEnumerator MoveTileQueue()
    {
        //int _diceValueUI = _dice.DiceValue-1;
        //_diceNumberObjects[_diceValueUI].SetActive(true);
        for (int i = 0; i < _moveQueue.Count; i++)
        {
            Tile nextTile = _moveQueue.Dequeue();

/*            int current = nextTile.tileIndex - i;
            int next = nextTile.tileIndex - i - 1;
            if (current < _diceNumberObjects.Count && next >= 0)
            {
                if (_diceNumberObjects[current] != null)
                    _diceNumberObjects[current].SetActive(false);
                if (_diceNumberObjects[next] != null)
                    _diceNumberObjects[next].SetActive(true);
            }
*/
            Vector3 startPos = transform.position;
            Vector3 endPos = nextTile.transform.position;
            float distance = Vector3.Distance(startPos, endPos);
            float duration = distance / moveSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos; // ���� ��ġ ����
            currentTile = nextTile;
            yield return new WaitForSeconds(0.1f);

        }
        isMoving = false;
        _dice.gameObject.SetActive(false);
    }
        

    // comment: �Ʒ� ������ ���� �������� ��.
    private IEnumerator MoveCoroutine(Tile nextTile)
    {
        transform.position = nextTile.transform.position;
        currentTile = nextTile;
        yield return new WaitForSeconds(1f);
    }
}
