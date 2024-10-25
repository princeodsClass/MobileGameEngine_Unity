using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public abstract partial class UIRootBase : MonoBehaviour
{
	[SerializeField] GameObject _goBackground = null;
	[SerializeField] GameObject _goPopupDimmed = null;
	[SerializeField] List<Camera> _listCamera = null;

	protected GameManager _GameMgr = null;
	protected MenuManager _MenuMgr = null;
	protected Canvas _Canvas = null;
	protected CanvasScaler _CanvasScaler = null;
	protected RectTransform _RectTransform = null;

	public Canvas GetCanvas() { return _Canvas; }
	public Vector2 GetRectSize() { return _RectTransform.sizeDelta; }

	private void Awake()
	{
		if (null == _GameMgr) _GameMgr = GameManager.Singleton;
		if (null == _MenuMgr) _MenuMgr = MenuManager.Singleton;

		_MenuMgr.Initialize();
		_MenuMgr.SetUIRoot(this);

		Initialize();
	}

	protected virtual void Initialize()
	{
		_Canvas = GetComponent<Canvas>();
		_CanvasScaler = GetComponent<CanvasScaler>();
		_RectTransform = GetComponent<RectTransform>();

		SetCameraOption();
	}

	protected virtual void UpdatePerFrame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			_MenuMgr.IsCheckEscape();
	}

	private void Update() { UpdatePerFrame(); }
	public virtual void Clear() { }

	protected virtual void OnCallbackTopMenu() { }

	public virtual void ScreenCapture() { }

	public virtual void SetActiveBackground(bool bShow)
	{
		if (null != _goBackground) _goBackground.SetActive(bShow);
	}

	public virtual void SetPopupDimmed(bool bOpen)
	{
		if (null != _goPopupDimmed) _goPopupDimmed.SetActive(bOpen);
	}

	public void SetCameraOption()
	{
		if (null == _listCamera) return;

		for (int i = 0; i < _listCamera.Count; ++i)
		{
			if (null == _listCamera[i]) continue;

			UniversalAdditionalCameraData uacd = _listCamera[i].GetComponent<UniversalAdditionalCameraData>();
		}
	}
}