using System.Collections;
using UnityEngine;

public class CoinTile : Tile
{
    [SerializeField] private int _coinChangeAmount; // ���: ����, ����: ����
    [SerializeField] private GameObject _coinEffectPrefab; // ���� ȿ�� (����)

    private void Awake()
    {
        _coinEffectPrefab.SetActive(false);
    }

    public override void Event(Player player)
    {
        BoardManager.Instance.StartCoroutine(HandleCoinEffect(player));
    }

    private IEnumerator HandleCoinEffect(Player player)
    {
        if (_coinEffectPrefab != null)
        {
            GameObject effect = Instantiate(_coinEffectPrefab, player.transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, 1f);
        }

        yield return new WaitForSeconds(1f);

        player.AddCoins(_coinChangeAmount);
        Debug.Log($"[CoinTile] {player.name}�� ������ {Mathf.Max(_coinChangeAmount, 0)}��ŭ ����Ǿ����ϴ�. ���� ���� : {player.Coin}");
    }
}