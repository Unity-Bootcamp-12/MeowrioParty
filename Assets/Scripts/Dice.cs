using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayDiceAnimationRpc()
    {
        _animator.SetTrigger("RollTrigger");
    }
}
