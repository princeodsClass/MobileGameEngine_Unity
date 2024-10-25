using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIDialog : MonoBehaviour
{
	[HideInInspector] public GameObject m_CachedObject = null;
	[HideInInspector] public RectTransform m_CachedTransform = null;

	private bool m_bInitialized = false;
	private bool m_bActive = false;
	private bool m_isSound = true;

	protected MenuManager _MenuMgr = null;
	protected GameManager _GameMgr = null;
	protected GameResourceManager _ResourceMgr = null;

	protected Dictionary<string, string> m_LogDataDict = new Dictionary<string, string>();
	public bool IsActivate { get { return (m_bActive && m_CachedObject.activeSelf) ? true : false; } }

	private void Start()
	{
		if (m_bInitialized)
			m_CachedObject.SetActive(m_bActive);
	}

	public virtual void Initialize()
	{
		if (!m_bInitialized)
		{
			m_bInitialized = true;
			m_bActive = false;
		}

		m_CachedObject = gameObject;
		m_CachedTransform = GetComponent<RectTransform>();

		if (null == _MenuMgr) _MenuMgr = MenuManager.Singleton;
		if (null == _GameMgr) _GameMgr = GameManager.Singleton;
        if (null == _ResourceMgr) _ResourceMgr = GameResourceManager.Singleton;
    }

	public virtual void Open()
	{
		m_bActive = true;
		if (null != m_CachedObject) m_CachedObject.SetActive(m_bActive);

		if ( this.gameObject.name.Equals("PopupWait4Response"))
			return;
	}

	public virtual void Close()
	{
		m_bActive = false;
		if (null != m_CachedObject) m_CachedObject.SetActive(m_bActive);

        if (this.gameObject.name.Equals("PopupWait4Response"))
            return;

        _MenuMgr.ClosePopup(this);

		m_isSound = true;
	}

	public virtual void Escape()
	{
		Close();
	}

	public virtual void EscapePage()
	{
	}

	public void MuteClose()
	{
		m_isSound = false;
		Close();
	}

	public virtual void PopupAppear()
	{
	}

	public virtual void PopupDrop()
	{
	}

	public virtual void BoxLanding()
	{
	}
}
