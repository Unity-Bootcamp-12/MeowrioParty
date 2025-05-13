using System;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private IntEventChannelSO OnRollDice;

    [SerializeField]
    private int minValue = 1;
    [SerializeField]
    private int maxValue = 6;

    // �ֻ����� ���� ���� ��ȯ
    public int RollDice()
    {
        int value = UnityEngine.Random.Range(minValue, maxValue + 1);
        OnRollDice.RaiseEvent(value);
        Debug.Log(value);
        return value;
    }
}
