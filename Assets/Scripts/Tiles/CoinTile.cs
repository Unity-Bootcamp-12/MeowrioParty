using System.Collections;
using UnityEngine;

public class CoinTile : Tile
{
    [SerializeField] private int _coinChangeAmount; // 양수: 지급, 음수: 차감
    [SerializeField] private GameObject _coinEffectPrefab; // 코인 효과 (선택)

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
        Debug.Log($"[CoinTile] {player.name}의 코인이 {Mathf.Max(_coinChangeAmount, 0)}만큼 변경되었습니다. 현재 코인 : {player.Coin}");
    }
}