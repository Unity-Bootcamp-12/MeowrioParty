using System.Collections;
using UnityEngine;

public class StarTile : Tile
{
    [SerializeField] private int _coinChangeAmount = -10;
    [SerializeField] private int _starChangeAmount = 1;
    [SerializeField] private GameObject _starEffectPrefab;

    private void Awake()
    {
        _starEffectPrefab.SetActive(false);
    }

    public override void TileEvent(Player player)
    {
        if (player.Coin >= 10)
        {
            BoardManager.Instance.StartCoroutine(HandleStarEffect(player));
        }
        Debug.Log(player.gameObject.name + "이 거지라서 스타를 못 샀어요");
    }

    private IEnumerator HandleStarEffect(Player player)
    {
        if (_starEffectPrefab != null)
        {
            GameObject effect = Instantiate(_starEffectPrefab, player.transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, 1f);
        }

        yield return new WaitForSeconds(1f);

        player.AddCoins(_coinChangeAmount);
        player.AddStars(_starChangeAmount);

        Debug.Log($"[StarTile] {player.name}의 스타가 {_starChangeAmount}개 증가하고 코인이 {_coinChangeAmount} 감소했습니다.");
    }
}