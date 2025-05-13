using System;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public event Action<int> OnRollDice;
    
    [SerializeField]
    private int minValue = 1;
    [SerializeField]
    private int maxValue = 6;
    // �ֻ����� ���� ���� ��ȯ
    public int RollDiceFunction()
    {
        int value = UnityEngine.Random.Range(minValue, maxValue+1);
        OnRollDice.Invoke(value);
        Debug.Log(value);
        return value;
    }
}
