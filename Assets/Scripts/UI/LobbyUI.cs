using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LobbyUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    [Header("Display")]
    [SerializeField] private TMP_Text playerListText;

    [Header("Connection Settings")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;

    private void Start()
    {
        // ��ư ����� �ڵ�� ���
        hostButton.onClick.AddListener(OnHostClicked);
        clientButton.onClick.AddListener(OnClientClicked);
        readyButton.onClick.AddListener(OnReadyClicked);
        startButton.onClick.AddListener(OnStartClicked);

        readyButton.interactable = false;
        startButton.interactable = false;
    }
    private void OnEnable()
    {
        StartCoroutine(WaitAndRegister());
    }

    private IEnumerator WaitAndRegister()
    {
        // LobbyManager�� �ʱ�ȭ�� ������ ���
        while (LobbyManager.Instance == null)
        {
            yield return null;
        }

        LobbyManager.Instance.OnPlayerListChanged += UpdatePlayerListUI;
    }

    public void OnHostClicked()
    {
        //ApplyConnectionSettings();
        NetworkManager.Singleton.StartHost();
        readyButton.gameObject.SetActive(false);
        startButton.interactable = true;
    }

    public void OnClientClicked()
    {
        //ApplyConnectionSettings();
        NetworkManager.Singleton.StartClient();
        readyButton.interactable = true;
        startButton.gameObject.SetActive(false);
    }

    private void ApplyConnectionSettings()
    {
        string ip = ipInputField.text;
        ushort port = 7777; // default port

        if (ushort.TryParse(portInputField.text, out ushort parsedPort))
            port = parsedPort;

        var unityTransport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

        if (unityTransport != null)
        {
            unityTransport.SetConnectionData(ip, port);
        }
    }

    public void OnReadyClicked()
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        LobbyManager.Instance.SetReadyServerRpc(myClientId);
        readyButton.interactable = false;
    }

    public void OnStartClicked()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Board", UnityEngine.SceneManagement.LoadSceneMode.Single);

        //if (NetworkManager.Singleton.IsHost)
        //{
        //    ulong myClientId = NetworkManager.Singleton.LocalClientId;

        //    // ȣ��Ʈ�� Ready ���¸� ���� ����
        //    LobbyManager.Instance.SetReadyServerRpc(myClientId);

        //    // ��� �÷��̾ �غ�Ǿ����� Ȯ��
        //    if (LobbyManager.Instance.IsAllPlayersReady())
        //    {
        //        NetworkManager.Singleton.SceneManager.LoadScene("Board", UnityEngine.SceneManagement.LoadSceneMode.Single);
        //    }
        //    else
        //    {
        //        Debug.Log("Not all players are ready yet.");
        //    }
        //}
    }

    public void UpdatePlayerListUI()
    {
        playerListText.text = "";
        foreach (var player in LobbyManager.Instance.playerStates)
        {
            playerListText.text += $"Player {player.ClientId} - {(player.IsReady ? "Ready" : "Not Ready")}\n";
        }
    }
}
