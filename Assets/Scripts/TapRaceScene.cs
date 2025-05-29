using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapRaceScene : MonoBehaviour
{
    [SerializeField] GameObject _miniGameManagerPrefab;

    private GameObject _miniGameManagerObject;
    private Coroutine _loadManagerCoroutine;

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }

    void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "TapRaceScene")
        {
            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                _loadManagerCoroutine = StartCoroutine(LoadManager());
            }
        }
    }

    IEnumerator LoadManager()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _miniGameManagerObject = Instantiate(_miniGameManagerPrefab);
            if (_miniGameManagerObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                networkObject.Spawn();
            }
        }
        yield return null;
    }

    IEnumerator UnLoadManager()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (_miniGameManagerObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                networkObject.Despawn();
            }
        }
        yield return null;
    }
}
