using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileIndex;

    public virtual void Event(Player player)
    {
        Debug.Log($"{player.gameObject.name}��(��) Ÿ��{tileIndex}�� �����߽��ϴ�.");
    }
}