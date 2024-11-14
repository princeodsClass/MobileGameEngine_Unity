using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : SingletonMono<MenuManager>
{
    private Dictionary<int, UIDialog> _dicUIPage = new Dictionary<int, UIDialog>();
    private Dictionary<int, UIDialog> _dicUIPopup = new Dictionary<int, UIDialog>();
    private Dictionary<int, GameObject> _dicUICompo = new Dictionary<int, GameObject>();

    private Transform _tUIRoot = null;
    private Transform _tRootPage = null;
    private Transform _tRootAbove = null;
    private Transform _tRootPopup = null;

    private GameResourceManager _ResMgr = null;
    private GameAudioManager _AudioMgr = null;
    private UIRootBase _UIRoot = null;

    private EUIPage _eCurPage = EUIPage.End;
    private EUIPopup _eCurPopup = EUIPopup.End;

    public List<EUIPopup> _liCurPopupType = new List<EUIPopup>();
    public List<UIDialog> _liCurPopup = new List<UIDialog>();

    public bool IsLoadingComplete { get; private set; }
    public bool IsErrorWithLogout { get; private set; }

    public ESceneType CurScene { get; set; }

     private void Awake()
    {
        if (null == _ResMgr) _ResMgr = GameResourceManager.Singleton;
        if (null == _AudioMgr) _AudioMgr = GameAudioManager.Singleton;

        CurScene = ESceneType.MemuScene;
        IsLoadingComplete = true;
        IsErrorWithLogout = false;
    }

    public void Clear()
    {
        _eCurPage = EUIPage.End;

        _liCurPopupType.Clear();
        _liCurPopup.Clear();

        IsErrorWithLogout = false;

        _dicUIPage.Clear();
        _dicUIPopup.Clear();
        _dicUICompo.Clear();

        if (null != _UIRoot)
            _UIRoot.Clear();
    }

    public void Initialize()
    {
        Clear();

        GameObject goRoot = GameObject.Find(ComType.UI_ROOT_NAME);
        if (null == goRoot)
        {
            string strPath = $"{ComType.UI_PATH}/{ComType.UI_ROOT_NAME}";
            GameObject goPref = Resources.Load(strPath) as GameObject;
            if (null != goPref)
                goRoot = Instantiate(goPref) as GameObject;
        }

        if (null != goRoot)
        {
            _tUIRoot = goRoot.transform;
            _tRootPage = _tUIRoot.Find(ComType.UI_ROOT_PAGE);
            _tRootAbove = _tUIRoot.Find(ComType.UI_ROOT_ABOVE);
            _tRootPopup = _tUIRoot.Find(ComType.UI_ROOT_POPUP);
        }
    }

    public void SetUIRoot(UIRootBase uiRoot)
    {
        _UIRoot = uiRoot;
    }

    public void ShowBackground(bool bShow)
    {
        _UIRoot.SetActiveBackground(bShow);
    }

    public void ShowPopupDimmed(bool bShow)
    {
        _UIRoot.SetPopupDimmed(bShow);
    }

    public void SetCameraOption()
    {
        if (null != _UIRoot) _UIRoot.SetCameraOption();
    }

    public void SceneNext(ESceneType eScene)
    {
        if (CurScene == eScene) return;

        if (!IsLoadingComplete) return;

        IsLoadingComplete = false;
        _AudioMgr.ChangeAudioMixSnapShot(eScene.ToString());
        // 기타 신 변경 대응해야 할 것들 처리.

        SceneManager.LoadScene(eScene.ToString());
        CurScene = eScene;
    }

    public void SceneEnd()
    {
        IsLoadingComplete = true;
    }

    public void SetLogoutFlag(bool bSet)
    {
        IsErrorWithLogout = bSet;
    }

    public Transform GetUIRoot() { return _tUIRoot; }
    public Canvas GetUICanvas() { return _UIRoot.GetCanvas(); }
    public Vector2 GetUIRootSize() { return _UIRoot.GetRectSize(); }
    public UIRootBase GetCurUIBase() { return _UIRoot; }

    private UIDialog LoadPage(EUIPage ePage)
    {
        GameObject goPage = _ResMgr.CreateObject(EResourceType.UIPage, ePage.ToString(), _tRootPage);
        if (null == goPage) return null;

        int nKey = (int)ePage;
        UIDialog uiDlg = goPage.GetComponent<UIDialog>();
        if (_dicUIPage.ContainsKey(nKey))
            _dicUIPage.Remove(nKey);

        _dicUIPage.Add(nKey, uiDlg);
        return uiDlg;
    }

    private UIDialog LoadAbove(EUIPage ePage)
    {
        GameObject goPage = _ResMgr.CreateObject(EResourceType.UIPage, ePage.ToString(), _tRootAbove);
        if (null == goPage) return null;

        int nKey = (int)ePage;
        UIDialog uiDlg = goPage.GetComponent<UIDialog>();
        if (_dicUIPage.ContainsKey(nKey))
            _dicUIPage.Remove(nKey);

        _dicUIPage.Add(nKey, uiDlg);
        return uiDlg;
    }

    private UIDialog LoadPopup(EUIPopup ePopup)
    {
        GameObject goPopup = _ResMgr.CreateObject(EResourceType.UIPopup, ePopup.ToString(), _tRootPopup);
        if (null == goPopup) return null;

        int nKey = (int)ePopup;
        UIDialog uiDlg = goPopup.GetComponent<UIDialog>();
        if (_dicUIPopup.ContainsKey(nKey))
            _dicUIPopup.Remove(nKey);

        _dicUIPopup.Add(nKey, uiDlg);
        return uiDlg;
    }

    public GameObject LoadComponent(Transform tRoot, EUIComponent eCompo)
    {
        GameObject goCompo = _ResMgr.CreateObject(EResourceType.UIComponent, eCompo.ToString(), tRoot);
        return goCompo;
    }

    public T LoadComponent<T>(Transform tRoot, EUIComponent eCompo)
    {
        GameObject goCompo = LoadComponent(tRoot, eCompo);
        if (null != goCompo)
            return goCompo.GetComponent<T>();
        return default;
    }

    public void OpenPage(EUIPage ePage)
    {
        if (ePage != _eCurPage) HidePage(_eCurPage);
        _eCurPage = ePage;

        UIDialog uiDlg = GetPage(ePage);
        if (null != uiDlg)
            uiDlg.Open();
    }

    public T OpenPage<T>(EUIPage ePage) where T : UIDialog
    {
        if (ePage != _eCurPage) HidePage(_eCurPage);
        _eCurPage = ePage;

        UIDialog uiDlg = GetPage(ePage);
        if (null != uiDlg)
            uiDlg.Open();

        return uiDlg as T;
    }

    public T GetPage<T>(EUIPage ePage) where T : UIDialog
    {
        if (_dicUIPage.TryGetValue((int)ePage, out UIDialog uiDlg))
            return uiDlg as T;
        return LoadPage(ePage) as T;
    }

    public UIDialog GetPage(EUIPage ePage)
    {
        if (_dicUIPage.TryGetValue((int)ePage, out UIDialog uiDlg))
            return uiDlg;
        return LoadPage(ePage);
    }

    public UIDialog GetAbove(EUIPage ePage)
    {
        if (_dicUIPage.TryGetValue((int)ePage, out UIDialog uiDlg))
            return uiDlg;
        return LoadAbove(ePage);
    }

    public T OpenAbove<T>(EUIPage ePage) where T : UIDialog
    {
        int nPage = (int)ePage;
        if (!_dicUIPage.ContainsKey(nPage))
            LoadAbove(ePage);

        _dicUIPage[nPage].Open();
        return _dicUIPage[nPage] as T;
    }

    public T OpenPopup<T>(EUIPopup ePopup, bool overlap = false) where T : UIDialog
    {
        //if (ePopup != m_eCurPopup && !overlap)
        if (!overlap)
        {
            // HidePopup(m_eCurPopup);
            ComUtil.DestroyChildren(_tRootPopup);

            _liCurPopupType.Clear();
            _liCurPopup.Clear();
        }

        UIDialog uiDlg = GetPopup(ePopup);

        if (null != uiDlg)
        {
            ComUtil.SetParent(_tRootPopup, uiDlg.transform);
            // m_eCurPopup = ePopup;
            if ( !_liCurPopupType.Contains(ePopup) )
            {
                _liCurPopupType.Add(ePopup);
                _liCurPopup.Add(uiDlg);
            }

            uiDlg.Open();
        }

        return uiDlg as T;
    }

    public void ClosePopup(UIDialog popup)
    {
        if ( _liCurPopup.Contains(popup) )
        {
            int idx = _liCurPopup.IndexOf(popup);

            _liCurPopupType.RemoveAt(idx);
            _liCurPopup.RemoveAt(idx);
        }
    }

    public T GetPopup<T>(EUIPopup ePopup) where T : UIDialog
    {
        if (_dicUIPopup.TryGetValue((int)ePopup, out UIDialog uiDlg))
            return uiDlg as T;
        return LoadPopup(ePopup) as T;
    }

    public UIDialog GetPopup(EUIPopup ePopup)
    {
        if (_dicUIPopup.TryGetValue((int)ePopup, out UIDialog uiDlg))
            return uiDlg;
        return LoadPopup(ePopup);
    }

    public void ShowUI(bool bShow)
    {
        _tRootPage.gameObject.SetActive(bShow);
        _tRootAbove.gameObject.SetActive(bShow);
        _tRootPopup.gameObject.SetActive(bShow);
    }

    public void ShowUI(bool bPage, bool bAbove, bool bPopup, bool bShow)
    {
        if (bPage) _tRootPage.gameObject.SetActive(bShow);
        if (bAbove) _tRootAbove.gameObject.SetActive(bShow);
        if (bPopup) _tRootPopup.gameObject.SetActive(bShow);
    }

    public void HidePage(EUIPage ePage)
    {
        int nKey = (int)ePage;
        if (_dicUIPage.ContainsKey(nKey))
            _dicUIPage[nKey].Close();
    }

    public void HidePopup(EUIPopup ePopup)
    {
        int nKey = (int)ePopup;
        if (_dicUIPopup.ContainsKey(nKey))
            _dicUIPopup[nKey].MuteClose();
    }

    public bool IsUIPageShow(EUIPage page)
    {
        return _dicUIPage.ContainsKey((int)page);
    }

    public bool IsCheckEscape()
    {
        for (int i = 0; i < (int)EUIPopup.End; ++i)
        {
            if (_dicUIPopup.ContainsKey(i))
            {
                if (_dicUIPopup[i].IsActivate)
                {
                    _dicUIPopup[i].Escape();
                    return false;
                }
            }
        }

        if (_dicUIPage.ContainsKey((int)_eCurPage))
        {
            _dicUIPage[(int)_eCurPage].EscapePage();
            return false;
        }

        return true;
    }
}
