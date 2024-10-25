using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageMenu : UIDialog
{
     private void Awake()
    {
        if (null == _MenuMgr) _MenuMgr = MenuManager.Singleton;
        if (null == _GameMgr) _GameMgr = GameManager.Singleton;
    }

    public void OnClickStartButton()
    {
        PopupTwoButton popup = _MenuMgr.OpenPopup<PopupTwoButton>(EUIPopup.PopupTwoButton);

        popup.SetTitle("Start Now?");
        popup.SetMessage("Let's Start Game");
        popup.SetButtonText("Start", "Cancel", NextScene);

    }

    void NextScene()
    { 
        _MenuMgr.SceneNext(ESceneType.TestScene);
    }
}
