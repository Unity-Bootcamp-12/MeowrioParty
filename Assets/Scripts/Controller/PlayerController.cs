using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private InputManagerSO _inputManager;

    [SerializeField] private DiceController _diceObj;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>();

    private Animator _animator;

    public bool IsMoving { get; private set; }
    [SerializeField] private float moveSpeed = 3f;

    private void Awake()
    {
        _inputManager.OnConfirmButtonPerformed += _inputManager_OnConfirmButtonPerformed;
        _animator = GetComponent<Animator>();
    }

    private void _inputManager_OnConfirmButtonPerformed(object sender, bool e)
    {
        BoardManager.Instance.ProcessPlayerInputServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    void Start()
    {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(200, 50);
    }

    public void MoveTo(TileController nextTile)
    {
        if (!IsMoving)
        {
            IsMoving = true;
            _animator.SetBool("isMoving", true);
            Vector3 endPos = nextTile.transform.position;
            transform.DOLookAt(endPos, 0.2f);

            transform.DOMove(endPos, moveSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        IsMoving = false;
                        _animator.SetBool("isMoving", false);
                    });
        }
    }

    [Rpc(SendTo.Everyone)]
    public void RollDiceSequenceRpc(int diceValue)
    {
        StartCoroutine(RollDiceSequenceCoroutine(diceValue));
    }

    IEnumerator RollDiceSequenceCoroutine(int diceValue)
    {
        ToggleDice(false);
        PlaySFXClinetRpc(SFXType.StopDice);
        TurnOnDiceNumber(diceValue);
        yield return null;
    }

    public void ToggleDice(bool isOn)
    {
        _diceObj.gameObject.SetActive(isOn);
        PlaySFXClinetRpc(SFXType.RollingDice);
    }

    public void TurnOnDiceNumber(int index)
    {
        if (_diceNumberObjects[index - 1] != null)
        {
            _diceNumberObjects[index - 1].SetActive(true);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void TurnOffDiceNumberRpc()
    {
        foreach (var diceNumberObject in _diceNumberObjects)
        {
            diceNumberObject.SetActive(false);
        }

    }

    public void TransportPlayer(TileController tile)
    {
        gameObject.transform.position = tile.gameObject.transform.position;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySFXClinetRpc(SFXType sfxType)
    {
        SoundManager.Instance.PlaySFX(sfxType);
    }
}
