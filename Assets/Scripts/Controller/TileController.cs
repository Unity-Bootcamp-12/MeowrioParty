using Unity.Netcode;
using UnityEngine;
public enum ETileType
{
    None,
    CoinPlusTile,
    CoinMinusTile,
    StarTile,
    WarpTile
}

public class TileController : MonoBehaviour
{
    public ETileType tileType;
    public int tileIndex;

    [Header("Effect Parameters")]
    public int eventParam;
    public TileController MoveTo;

    public void TileEventAtServer(PlayerData playerData, PlayerController playerController)
    {
        switch (tileType)
        {
            case ETileType.CoinPlusTile:
                playerData.UpdateCoinCnt(eventParam);
                Debug.Log("Coinplus");
                break;
            case ETileType.CoinMinusTile:
                playerData.UpdateCoinCnt(-eventParam);
                Debug.Log("CoinMinusTile");
                break;
            case ETileType.StarTile:
                Debug.Log("StarTile");
                break;
            case ETileType.WarpTile:
                WarpTo(playerData,playerController, MoveTo);
                Debug.Log("WarpTile");
                break;
            default:
                break;
        }
    }
    public ETileType TileEventLeaderBoard(ulong _clientId)
    {
        switch (tileType)
        {
            case ETileType.CoinPlusTile:
                LeaderBoardManager.Instance.UpdateCoin(_clientId, eventParam);
                Debug.Log("Coinplus");
                PlaySFXClinetRpc(SFXType.Coin);
                break;
            case ETileType.CoinMinusTile:
                LeaderBoardManager.Instance.UpdateCoin(_clientId, -eventParam);
                Debug.Log("CoinMinusTile");
                break;
            case ETileType.StarTile:

                //LeaderBoardManager.Instance.UpdateStar(_clientId, eventParam);
                Debug.Log("StarTile");
                PlaySFXClinetRpc(SFXType.Star);
                break;
            default:
                Debug.Log("Coinplus");
                break;
        }
        return tileType;
    }

    private void WarpTo(PlayerData playerData, PlayerController playerController, TileController targetTile)
    {
        playerData.MoveTo(targetTile);
        playerController.TransportPlayer(targetTile);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySFXClinetRpc(SFXType sfxType)
    {
        SoundManager.Instance.PlaySFX(sfxType);
    }
}
