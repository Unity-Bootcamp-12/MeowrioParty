using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUIData : BaseUIData
{
    // public string RollDiceBtnTxt;
    // public string ItemBtnTxt;
    public Action OnClickRollDiceBtn;
    public Action OnClickItemBtn;
}

public class ButtonUI : BaseUI
{
    public Button RollDiceBtn = null;
    public Button ItemBtn = null;

    private ButtonUIData m_ButtonUIData = null;
    private Action m_OnClickRollDiceBtn = null;
    private Action m_OnClickItemBtn = null;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_ButtonUIData = uiData as ButtonUIData;

        m_OnClickRollDiceBtn = m_ButtonUIData.OnClickRollDiceBtn;
        m_OnClickItemBtn = m_ButtonUIData.OnClickItemBtn;

    }

    public void OnClickRollDiceBtn()
    {
        m_OnClickRollDiceBtn?.Invoke();
    }
}
