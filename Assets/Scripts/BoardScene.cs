using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BoardScene : MonoBehaviour
{
    [SerializeField] GameObject _cameraManagerPrefab;
    [SerializeField] GameObject _boardManagerPrefab;
    [SerializeField] BoardManager _boardManager;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, loadSceneMode) =>
        {
            if (sceneName == "BoardTest")
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    StartCoroutine(LoadManagersCoroutine());
                }
            }
        };
    }

    private IEnumerator LoadManagersCoroutine()
    {
        CameraManager cameraManager = null;
        
        if (_cameraManagerPrefab != null)
        {
            GameObject cameraManagerObj = Instantiate(_cameraManagerPrefab);
            if (cameraManagerObj.TryGetComponent<CameraManager>(out CameraManager cameraManagerComponent))
            {
                cameraManager = cameraManagerComponent;
            }
        }

        while (cameraManager == null)
        {
            yield return null;
        }

        if (NetworkManager.Singleton.IsServer)
        {
            if (_boardManagerPrefab != null)
            {
                GameObject boardManagerObj = Instantiate(_boardManagerPrefab);
                if (boardManagerObj.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                {
                    networkObject.Spawn();
                }
            }
        }
        LeaderBoardManager.Instance.InitializeLeaderBoard(NetworkManager.Singleton.ConnectedClientsList.Count);
    }
}
