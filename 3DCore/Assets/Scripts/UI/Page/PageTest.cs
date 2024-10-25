using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageTest : UIDialog
{
    [SerializeField]
    Button _ButtonStart;

    [SerializeField]
    TextMeshProUGUI _txtStartButtonCaption;

    private void Awake()
    {
        if (null == _MenuMgr) _MenuMgr = MenuManager.Singleton;
        if (null == _GameMgr) _GameMgr = GameManager.Singleton;

        _txtStartButtonCaption.text = "Start";
    }

    public void OnClickStartButton()
    {
        PopupTwoButton popup = _MenuMgr.OpenPopup<PopupTwoButton>(EUIPopup.PopupTwoButton);

        popup.SetTitle("�����ұ�?");
        popup.SetMessage("�� ������ �غ���!");
        popup.SetButtonText("����", "���", NextScene);

    }

    void NextScene()
    { 
        _MenuMgr.SceneNext(ESceneType.TestScene);
    }
}
