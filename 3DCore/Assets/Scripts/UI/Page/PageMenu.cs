using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageMenu : UIDialog
{
    [SerializeField]
    GameObject _goCreate, _goDelete;

    [SerializeField]
    TextMeshProUGUI _txtAuid;

     private void Awake()
    {
        if (null == _MenuMgr) _MenuMgr = MenuManager.Singleton;
        if (null == _GameMgr) _GameMgr = GameManager.Singleton;

        InitializePage();
    }

    public void InitializePage()
    {
        if (PlayerPrefs.GetInt(ComType.STORAGE_UID) == 0)
        {
            _goCreate.SetActive(true);
            _goDelete.SetActive(false);
            _txtAuid.gameObject.SetActive(false);
        }
        else
        {
            _goCreate.SetActive(false);
            _goDelete.SetActive(true);
            _txtAuid.gameObject.SetActive(true);
            _txtAuid.text = PlayerPrefs.GetInt(ComType.STORAGE_UID).ToString();
        }
    } 

    public void OnClickStartButton()
    {
        if ( PlayerPrefs.GetInt(ComType.STORAGE_UID) == 0)
        {
            PopupOneButton pop = _MenuMgr.OpenPopup<PopupOneButton>(EUIPopup.PopupOneButton);

            pop.SetTitle("Infomation");
            pop.SetMessage("First, create an account.");
            pop.SetButtonText("OK");
        }
        else
        {
            PopupTwoButton popup = _MenuMgr.OpenPopup<PopupTwoButton>(EUIPopup.PopupTwoButton);

            popup.SetTitle("Start Now?");
            popup.SetMessage("Let's Start Game");
            popup.SetButtonText("Start", "Cancel", NextScene);
        }
    }

    void NextScene()
    { 
        _MenuMgr.SceneNext(ESceneType.TestScene);
    }

    public void OnClickCreateAccount()
    {
        PopupWait4Response wait = _MenuMgr.OpenPopup<PopupWait4Response>(EUIPopup.PopupWait4Response);

        StartCoroutine(GameDataManager.Singleton.CreateAccount(wait, this));
    }

    public void OnClickDeleteAccount()
    {
        GameDataManager.Singleton.DeleteAccount();
        InitializePage();
    }
}
