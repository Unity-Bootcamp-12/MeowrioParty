using UnityEngine;

public class UITest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //예시
        // var confirmUIData = new ConfirmUIData();
        // confirmUIData.ConfirmType = ConfirmType.OK;
        // confirmUIData.TitleTxt = "UI Test";
        // confirmUIData.DescTxt = "This is Test Popup";
        // confirmUIData.OKBtnTxt = "OK";
        // UIManager.Instance.OpenUI<ConfirmUI>(confirmUIData);

        var roundUIData = new RoundUIData();
        //roundUIData.currentRound = BoardManager.Instance.currentRound;
        roundUIData.currentRound = "3";
        roundUIData.maxRound = "10";
        UIManager.Instance.OpenUI<RoundUI>(roundUIData);


        var noticeUIData = new NoticeUIData();
        noticeUIData.currentNoticeTxt = "게임 시작!!";
        UIManager.Instance.OpenUI<NoticeUI>(noticeUIData);

        var buttonUIData = new ButtonUIData();
        buttonUIData.OnClickRollDiceBtn += () => { Debug.Log("뿡뿡"); };
        UIManager.Instance.OpenUI<ButtonUI>(buttonUIData);

        // var leaderBoardUIData = new LeaderBoardUIData();
        // leaderBoardUIData.FirstPlayerNameTxt = "dfd";
        // //...
        // UIManager.Instance.OpenUI<LeaderBoardUI>(leaderBoardUIData);

    }

}
