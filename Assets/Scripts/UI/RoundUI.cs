using System;
using TMPro;
using UnityEngine;

public class RoundUIData : BaseUIData
{
    public string currentRound;
    public string maxRound;
}

public class RoundUI : BaseUI
{
    public TextMeshProUGUI roundTxt = null;

    private RoundUIData m_RoundUIData = null;

    private Action m_OnChangeRound = null;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_RoundUIData = uiData as RoundUIData;

        roundTxt.text = m_RoundUIData.currentRound + "/" + m_RoundUIData.maxRound;
    }

    public void OnChangeRound()
    {
        m_OnChangeRound?.Invoke();
    }
}
