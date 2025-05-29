using System;
using UnityEngine;
using DG.Tweening;

public class BaseUIData
{
    //동일한 UI여도 상황에 따라 다른 걸 띄워야 할 때가 있기 때문에
    public Action OnShow;
    public Action OnClose;

    public bool isAnimPlay = false;
}

public class BaseUI : MonoBehaviour
{
    // public Animation m_UIOpenAnim;
    public bool m_isAnimPlay;

    private Action m_OnShow;
    private Action m_OnClose;

    public virtual void Init(Transform anchor)
    {
        m_OnShow = null;
        m_OnClose = null;

        transform.SetParent(anchor);

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.offsetMin = Vector3.zero;
        rectTransform.offsetMax = Vector3.zero;

    }
    public virtual void SetInfo(BaseUIData uiData)
    {
        m_isAnimPlay = uiData.isAnimPlay;

        m_OnShow = uiData.OnShow;
        m_OnClose = uiData.OnClose;
    }

    public virtual void ShowUI()
    {
        // if(m_UIOpenAnim != null)
        // {
        //     m_UIOpenAnim.Play();
        // }

        if (m_isAnimPlay)
        {
            var rectTransform = GetComponent<RectTransform>();
            var seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScale(1.1f, 0.2f));
            seq.Append(rectTransform.DOScale(1f, 0.1f));
            seq.Play();
        }
        

        m_OnShow?.Invoke();
        m_OnShow = null;
    }

    public virtual void CloseUI(bool isCloseAll = false) 
    {
        if(!isCloseAll) //씬전환등 열려있는 모든 화면을 닫아야 할 때 true로 넘겨서 필요한 처리 다 무시하고 화면만 닫아주기 위해
        {
            m_OnClose?.Invoke();
        }
        m_OnClose = null;
        UIManager.Instance.CloseUI(this);
    }

    //닫기버튼 눌렀을 때 함수 
    public virtual void OnClickCloseButton()
    {
        //SoundManager.Instance.PlaySFX(SFX.ui_button_click);
        CloseUI();
    }
}
