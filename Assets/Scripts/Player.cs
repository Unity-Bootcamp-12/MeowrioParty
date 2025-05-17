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

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 5f;

    public bool IsMoving { get { return _isMoving; } }

    public int RollDice()
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
    {
        if (!_isMoving)
        {
            StartCoroutine(MoveToSequenceCo(nextTile));
        }
    }

    private IEnumerator MoveToSequenceCo(Tile nextTile)
    {
        _isMoving = true;
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

        _isMoving = false;
    }

 
}
