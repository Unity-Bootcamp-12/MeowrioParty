using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTile : Tile
{
    [SerializeField] private Tile _targetTile;

    public override void TileEvent(Player player)
    {
        BoardManager.Instance.StartCoroutine(WaitAndTeleport(player));
    }

    private IEnumerator WaitAndTeleport(Player player)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(player.gameObject.name + "이/가 " + _targetTile.name + "으로 워프");
        player.transform.position = _targetTile.transform.position;
        player.currentTile = _targetTile;
        _targetTile.TileEvent(player);
    }
}