using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Transform UICanvasTrs; //UI화면을 랜더링할 컨버스
    public Transform CloseUITrs; //닫을 때 비활성화 할 

    private BaseUI m_FrontUI; //UI가 열렸을 때 가장 상단에 열려있는 UI

    //열려있는, 닫혀있는 ui pool 나눠서 관리리
    private Dictionary<Type, GameObject> m_OpenUIPool = new Dictionary<Type, GameObject>();
    private Dictionary<Type, GameObject> m_ClosedUIPool = new Dictionary<Type, GameObject>();
    

    private BaseUI GetUI<T>(out bool isAlreadyOpen)
    {
        Type uiType = typeof(T);
        BaseUI ui = null;
        isAlreadyOpen = false;

        if(m_OpenUIPool.ContainsKey(uiType) == true)
        {
            ui = m_OpenUIPool[uiType].GetComponent<BaseUI>();
            isAlreadyOpen = true;
        }
        else if(m_ClosedUIPool.ContainsKey(uiType) == true)
        {
            ui = m_ClosedUIPool[uiType].GetComponent<BaseUI>();
            m_ClosedUIPool.Remove(uiType);
        }
        else
        {
            //모든 UI들이 미리 씬에 세팅되어 있지 않고, 동적으로 처리
            //프리팹의 이름이 uiClass 이름과 동일해야 함 
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            ui = uiObj.GetComponent<BaseUI>();
        }

        return ui;
    }

    //실제로 여는 함수
    public void OpenUI<T>(BaseUIData uiData)
    {
        Type uiType = typeof(T);
        bool isAlreadyOpen = false;
        var ui = GetUI<T>(out isAlreadyOpen);

        if (ui == null)
        {
            //Logger.LogError($"{uiType} prefab doesn't exist in Resources");
            return;
        }

        if (isAlreadyOpen == true)
        {
            return;
        }

        var siblingIndex = UICanvasTrs.childCount;
        ui.Init(UICanvasTrs);
        ui.transform.SetSiblingIndex(siblingIndex);
        ui.gameObject.SetActive(true);
        ui.SetInfo(uiData);
        ui.ShowUI();

        m_FrontUI = ui;
        m_OpenUIPool.Add(uiType, ui.gameObject);
    }

    public void CloseUI(BaseUI ui)
    {
        Type uiType = ui.GetType();

        ui.gameObject.SetActive(false);
        m_OpenUIPool.Remove(uiType);
        m_ClosedUIPool[uiType] = ui.gameObject;
        ui.transform.SetParent(CloseUITrs);

        m_FrontUI = null;
        var lastChild = UICanvasTrs.GetChild(UICanvasTrs.childCount - 1);
        if (lastChild != null)
        {
            m_FrontUI = lastChild.gameObject.GetComponent<BaseUI>();
        }
    }

    //열려있는 UI 체크해서 가져오는 메소드
    public BaseUI GetActiveUI<T>()
    {
        var uiType = typeof(T);
        return m_OpenUIPool.ContainsKey(uiType) ? m_OpenUIPool[uiType].GetComponent<BaseUI>() : null;
    }

    public bool ExistOpenUI() // 열린 게 하나라도 있는지
    {
        return m_FrontUI != null;
    }

    public BaseUI GetCurrentFrontUI() //가장 최상단 ui instance 반환
    {
        return m_FrontUI;
    }

    public void CloseCurrentFrontUI() 
    {
        m_FrontUI.CloseUI();
    }

    public void CloseAllOpenUI() //열린 거 다 닫기
    {
        while (m_FrontUI != null)
        {
            m_FrontUI.CloseUI(true);
        }
    }
}