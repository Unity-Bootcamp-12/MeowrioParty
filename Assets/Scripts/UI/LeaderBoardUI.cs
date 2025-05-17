using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUIData : BaseUIData
{
    public string FirstPlayerNameTxt = null;
    public string SeceondPlayerNameTxt = null;
    public string ThirdPlayerNameTxt = null;
    public string FourthPlayerNameTxt = null;

    public Sprite FirstPlayerSpr = null;
    public Sprite SecondPlayerSpr  = null;
    public Sprite ThirdPlayerISpr = null;
    public Sprite FourthPlayerSpr  = null; 
}

public class LeaderBoardUI : BaseUI
{
    // 닫기 버튼? 도 있어야 할 것 같습니다.
    public TextMeshProUGUI FirstPlayerNameTxt = null;
    public TextMeshProUGUI SeceondPlayerNameTxt = null;
    public TextMeshProUGUI ThirdPlayerNameTxt = null;
    public TextMeshProUGUI FourthPlayerNameTxt = null;

    public Image FirstPlayerImg = null;
    public Image SecondPlayerImg = null;
    public Image ThirdPlayerImg = null;
    public Image FourthPlayerImg = null;

    private LeaderBoardUIData m_leaderBoardUIData = null;
    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_leaderBoardUIData = uiData as LeaderBoardUIData;

        FirstPlayerNameTxt.text = m_leaderBoardUIData.FirstPlayerNameTxt;
        SeceondPlayerNameTxt.text = m_leaderBoardUIData.SeceondPlayerNameTxt;
        ThirdPlayerNameTxt.text = m_leaderBoardUIData.ThirdPlayerNameTxt;
        FourthPlayerNameTxt.text = m_leaderBoardUIData.FourthPlayerNameTxt;

        FirstPlayerImg.sprite = m_leaderBoardUIData.FirstPlayerSpr;
        SecondPlayerImg.sprite = m_leaderBoardUIData.SecondPlayerSpr;
        ThirdPlayerImg.sprite = m_leaderBoardUIData.ThirdPlayerISpr;
        FourthPlayerImg.sprite = m_leaderBoardUIData.FourthPlayerSpr;
    }
}
