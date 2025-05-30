using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class NoticeUIData : BaseUIData
{
    public string currentNoticeTxt;
}

public class NoticeUI : BaseUI
{
    public TextMeshProUGUI noticeTxt = null;

    private NoticeUIData m_NoticeUIData = null;

    private Action m_OnNoticePop;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_NoticeUIData = uiData as NoticeUIData;

        noticeTxt.text = m_NoticeUIData.currentNoticeTxt;
    }

    public override void ShowUI()
    {
        base.ShowUI();
       
            var rectTransform = GetComponent<RectTransform>();
            var seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScale(1f, 0.3f));
            seq.Play();
        
    }

    public void OnNoticePop()
    {
        m_OnNoticePop?.Invoke();
    }
}
