using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupTwoButton : UIDialog
{
    [SerializeField] protected TextMeshProUGUI _txtTitle = null;
    [SerializeField] protected TextMeshProUGUI _txtTopMessage = null;
    [SerializeField] protected TextMeshProUGUI _txtMessage = null;
    [SerializeField] protected TextMeshProUGUI _txtPositive = null;
    [SerializeField] protected TextMeshProUGUI _txtNegative = null;
    [SerializeField] protected Transform _tOutline = null;
    [SerializeField] protected RectTransform[] _resize;

    protected Action _onCallbackPositive = null;
    protected Action _onCallbackNegative = null;

    protected int m_nData = 0;

    string _title = "Infomation";

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Escape()
    {
        base.Escape();
    }

    private void Awake()
    {
        Initialize();
        _txtTitle.text = _title;
    }

    private void OnEnable()
    {
        _txtTopMessage.text = "";
        _txtMessage.text = "";
    }

    public void SetTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
            _txtTitle.text = title;
    }

    public void SetMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            _txtMessage.text = message;
        }
    }

    public void SetTopMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            _txtTopMessage.text = message;
        }
    }

    public void SetButtonText(string txtConfirm, string txtCancel, Action positive = null, Action negative = null, string goName = null)
    {
        if (!string.IsNullOrEmpty(txtConfirm))
        {
            _txtPositive.text = txtConfirm;
        }

        if (!string.IsNullOrEmpty(txtCancel))
        {
            _txtNegative.text = txtCancel;
        }

        _onCallbackPositive = positive;
        _onCallbackNegative = negative;

        SetImage(goName);
        StartCoroutine(Resize());
    }

    void SetImage(string goName)
    {
        ComUtil.DestroyChildren(_tOutline);

        if (string.IsNullOrEmpty(goName))
        {
            _tOutline.gameObject.SetActive(false);
        }
        else
        {
            GameResourceManager.Singleton.CreateObject(EResourceType.UIETC, goName, _tOutline);
            _tOutline.gameObject.SetActive(true);
        }
    }

    IEnumerator Resize()
    {
        yield return null;

        Array.ForEach(_resize, rt => LayoutRebuilder.ForceRebuildLayoutImmediate(rt));
    }

    public void OnClickButtonPositive()
    {
        Close();

        if (null != _onCallbackPositive) _onCallbackPositive.Invoke();
    }

    public void OnClickButtonNegative()
    {
        Close();
    }
}
