using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileIndex;

    public virtual void TileEvent(Player player)
    {
        Debug.Log($"{player.gameObject.name}이(가) 타일{tileIndex}에 도착했습니다.");
    }
}