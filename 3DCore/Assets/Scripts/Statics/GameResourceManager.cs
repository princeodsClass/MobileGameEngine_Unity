using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Networking;

public partial class GameResourceManager : SingletonMono<GameResourceManager>
{
	// 전역 접근성.
	// 관리하는 리소스의 일관성 ( 중복 처리 오류 ).
	// 메모리 / 성능 관리 ( 최적화 ).
	// 이 것들을 위해 singleton 처리.
	
	private Transform _tRootObjectPool = null;
	private Dictionary<string, Queue<GameObject>> _dicObjectPool = new Dictionary<string, Queue<GameObject>>();

	// Object Pooling 관련은 다음 시간에..!

	private List<Dictionary<string, GameObject>> _listCachedPrefabs = new List<Dictionary<string, GameObject>>();
	
	// 프리팹은 중복 존재할 수 있으므로.
	
	private Dictionary<string, GameObject> _dicUIPrefabs = new Dictionary<string, GameObject>();
	private Dictionary<int, SpriteAtlas> _dicAtlas = new Dictionary<int, SpriteAtlas>();
	private Dictionary<string, Sprite> _dicCachedSprite = new Dictionary<string, Sprite>();
	private Dictionary<string, Object> _dicCachedObject = new Dictionary<string, Object>();

	// 중복되지 말아야 할 것들은 딕셔너리로 처리.

	private void Awake()
	{
		// 모든 EResourceType 별 프리팹 캐시 리스트 생성 및 초기화.

		if (0 == _listCachedPrefabs.Count)
		{
			for (int i = 0; i < (int)EResourceType.End; ++i)
				_listCachedPrefabs.Add(new Dictionary<string, GameObject>());
		}
	}

	private void OnDestroy()
	{
		// 소멸자 역할

		_dicAtlas.Clear();
		_dicCachedSprite.Clear();
		_dicCachedObject.Clear();
		_dicObjectPool.Clear();

		for (int i = 0; i < _listCachedPrefabs.Count; ++i)
			_listCachedPrefabs[i].Clear();
	}

	public void Clear()
	{
		// 강제 초기화

		Dictionary<string, Queue<GameObject>>.Enumerator it = _dicObjectPool.GetEnumerator();
		while (it.MoveNext())
			it.Current.Value.Clear();

		_dicObjectPool.Clear();
		if (null != _tRootObjectPool)
			_tRootObjectPool.DestroyChildren(false);

		_dicAtlas.Clear();
		_dicCachedSprite.Clear();
		_dicCachedObject.Clear();

		for (int i = 0; i < _listCachedPrefabs.Count; ++i)
			_listCachedPrefabs[i].Clear();
	}

    /// <summary>
	/// UI 프리팹들의 경로 반환 메서드
	/// </summary>
	/// <param name="eType"></param>
	/// <param name="strPrefName"></param>
	/// <param name="a_nTheme"></param>
	/// <returns>
	/// Path(EResourceType eType, string strPrefName, int a_nTheme = 0)
	/// </returns>
	public string GetPrefabPath(EResourceType eType, string strPrefName, int a_nTheme = 0)
    {
        string oThemePath = string.Format("{0:00}-Theme/", a_nTheme + 1);

		switch (eType)
		{
			case EResourceType.UI: return $"{ComType.UI_PATH}{strPrefName}";
			case EResourceType.UIPage: return $"{ComType.UI_PATH_PAGE}{strPrefName}";
			case EResourceType.UIPopup: return $"{ComType.UI_PATH_POPUP}{strPrefName}";
			case EResourceType.UIButton: return $"{ComType.UI_PATH_BUTTON}{strPrefName}";
			case EResourceType.UIETC: return $"{ComType.UI_PATH_ETC}{strPrefName}";
			case EResourceType.UIComponent: return $"{ComType.UI_PATH_COMPO}{strPrefName}";
		}

		return string.Empty;
	}

	/// <summary>
	/// 프리팹 로드 메서드
	/// </summary>
	/// <param name="eType"></param>
	/// <param name="strPrefName"></param>
	/// <param name="a_oOrigin"></param>
	/// <param name="a_nTheme"></param>
	/// <returns>
	/// GameObject ( Prefab )
	/// </returns>
	public GameObject LoadPrefabs(EResourceType eType, string strPrefName, GameObject a_oOrigin = null, int a_nTheme = 0)
	{
		GameObject goPref;
		if (!_listCachedPrefabs[(int)eType].TryGetValue(strPrefName, out goPref))
		{
			string strPath = this.GetPrefabPath(eType, strPrefName, a_nTheme);
			goPref = Load<GameObject>(strPath) ?? a_oOrigin;

#if DEBUG || DEVELOPMENT_BUILD
			// 프리팹이 없을 경우
			if (goPref == null)
			{
				strPath = this.GetPrefabPath(eType, strPrefName, a_nTheme + 1);
				goPref = Load<GameObject>(strPath) ?? a_oOrigin;
			}
#endif // #if DEBUG || DEVELOPMENT_BUILD

			if (null == goPref)
			{
				GameManager.Log("Resource Path Error == Type:" + eType + " Path:" + strPath, "red");
				return null;
			}

			_listCachedPrefabs[(int)eType].Add(strPrefName, goPref);
		}
		return goPref;
	}

	/// <summary>
	/// 리소스 로드 후 로드한 개체를 반환하는 제너릭 메서드
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="path"></param>
	/// <returns>T</returns>
	public T Load<T>(string path) where T : UnityEngine.Object
	{
		return Resources.Load<T>(path);
	}

	/// <summary>
	/// UI 프리팹 로드 후 게임 오브젝트를 반환하는 메서드
	/// </summary>
	/// <param name="eType"></param>
	/// <param name="strPrefName"></param>
	/// <returns>Prefab as GameObject</returns>
	private GameObject LoadUIPrefabs(EResourceType eType, string strPrefName)
	{
		GameObject goPref;

		if (!_dicUIPrefabs.TryGetValue(strPrefName, out goPref))
		{
			string strPath = GetPrefabPath(eType, strPrefName);
			goPref = Load<GameObject>(strPath);

			if (null == goPref)
			{
				GameManager.Log("Resource Path Error == Type:" + eType + " Path:" + strPath, "red");
				return null;
			}

			_dicUIPrefabs.Add(strPrefName, goPref);
		}

		return goPref;
	}
	
	/// <summary>
	/// 오브젝트 생성 후 생성 오브젝트 반환. 캐싱 과정 포함.
	/// </summary>
	/// <param name="eType"></param>
	/// <param name="strPrefName"></param>
	/// <param name="vPos"></param>
	/// <param name="qRot"></param>
	/// <param name="fLifeTime"></param>
	/// <param name="bCached"></param>
	/// <param name="a_oOrigin"></param>
	/// <param name="a_nTheme"></param>
	/// <returns>GameObject</returns>
	public GameObject CreateObject(EResourceType eType, string strPrefName, Vector3 vPos, Quaternion qRot, float fLifeTime = 0f, bool bCached = true, GameObject a_oOrigin = null, int a_nTheme = 0)
	{
		if (null == _tRootObjectPool)
		{
			GameObject goPool = new GameObject("ObjectPool");
			_tRootObjectPool = goPool.transform;
			_tRootObjectPool.position = Vector3.zero;
		}

		GameObject go = null;
		string strObjName = strPrefName;
		if (_dicObjectPool.ContainsKey(strObjName))
		{
			if (0 != _dicObjectPool[strObjName].Count)
			{
				go = _dicObjectPool[strObjName].Dequeue();
				if (null != go) go.transform.SetParent(null);
			}
		}
		else
		{
			if (bCached)
				_dicObjectPool.Add(strObjName, new Queue<GameObject>());
		}

		if (null == go)
		{
			GameObject goPref = LoadPrefabs(eType, strPrefName, a_oOrigin, a_nTheme);
			if (null == goPref) return null;

			go = Instantiate(goPref, vPos, qRot) as GameObject;
			go.name = strObjName;

			if (go.activeSelf == false)
				go.SetActive(true);
		}
		else
		{
			go.SetActive(true);
			go.transform.position = vPos;
			go.transform.rotation = qRot;
		}

		if (0f != fLifeTime)
			CheckDestroyObject(go, fLifeTime);

		return go;
	}

    /// <summary>
	/// 오브젝트 생성 후 생성 오브젝트 반환
	/// </summary>
	/// <param name="eType"></param>
	/// <param name="strPrefName"></param>
	/// <param name="tParent"></param>
	/// <param name="fLifeTime"></param>
	/// <param name="bCached"></param>
	/// <param name="a_nTheme"></param>
	/// <returns></returns>
	public GameObject CreateObject(EResourceType eType, string strPrefName, Transform tParent, float fLifeTime = 0f, bool bCached = true, int a_nTheme = 0)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity, fLifeTime, bCached, a_nTheme: a_nTheme);
		if (null == go) return null;

		ComUtil.SetParent(tParent, go.transform);
		return go;
	}

    /// <summary>
    /// 오브젝트 생성 후 생성 오브젝트 반환
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="strPrefName"></param>
    /// <param name="vec"></param>
    /// <param name="bCached"></param>
    /// <returns></returns>
    public GameObject CreateObject(EResourceType eType, string strPrefName, Vector3 vec, bool bCached = true)
	{
		GameObject go = CreateObject(eType, strPrefName, vec, Quaternion.identity, 0f, bCached);
		return go;
	}

    /// <summary>
    /// 오브젝트 생성 후 생성 오브젝트 반환
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="strPrefName"></param>
    /// <param name="bCached"></param>
    /// <returns></returns>
    public GameObject CreateObject(EResourceType eType, string strPrefName, bool bCached = true)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity, 0f, bCached);
		return go;
	}

    /// <summary>
    /// 오브젝트 생성 후 생성 오브젝트 반환
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="strPrefName"></param>
    /// <param name="tParent"></param>
    /// <returns></returns>
    public GameObject CreateUIObject(EResourceType eType, string strPrefName, Transform tParent)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity);
		if (null == go) return null;

		ComUtil.SetParent(tParent, go.transform);
		go.SetActive(true);
		return go;
	}

    /// <summary>
    /// 지정된 시간 후 오브젝트 소멸 처리.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="fLifeTime"></param>
    private void CheckDestroyObject(GameObject go, float fLifeTime)
	{
		AutoDestroy goAD = go.GetComponent<AutoDestroy>();
		if (null == goAD) goAD = go.AddComponent<AutoDestroy>();

		goAD.Init(fLifeTime);
	}

	/// <summary>
	/// 생성된-로드된 오브젝트 삭제 메서드.
	/// </summary>
	/// <param name="go"></param>
	/// <param name="bInsertPool"></param>
	public void ReleaseObject(GameObject go, bool bInsertPool = true)
	{
		if (null == go) return;
		if (string.IsNullOrEmpty(go.name)) return;

		try
		{
			if (bInsertPool)
			{
				if (!_dicObjectPool.ContainsKey(go.name))
					_dicObjectPool.Add(go.name, new Queue<GameObject>());

				go.SetActive(false);

				if (!_dicObjectPool[go.name].Contains(go))
				{
					_dicObjectPool[go.name].Enqueue(go);

#if DISABLE_THIS
					go.transform.SetParent(_tRootObjectPool);
#endif // #if DISABLE_THIS
				}
			}
			else
			{
				Destroy(go);
			}
		}
		catch (System.Exception e)
		{
			GameManager.Log($"Excption GameObject Name:{go.name}", "red");
			GameManager.Log(e.ToString());
		}
	}

    /// <summary>
    /// 스프라이트 아틀라스를 로드하고 반환하는 메서드.
    /// </summary>
    /// <param name="eAtlas"></param>
    /// <returns>SpriteAtlas</returns>
    private SpriteAtlas GetAtlas(EAtlasType eAtlas)
	{
		int nKey = (int)eAtlas;
		if (_dicAtlas.ContainsKey(nKey))
			return _dicAtlas[nKey];

		string strPath = $"{ComType.ATLAS_PATH}{eAtlas}";
		SpriteAtlas atlas = Load<SpriteAtlas>(strPath);
		if (null != atlas)
			_dicAtlas.Add(nKey, atlas);

		return atlas;
	}

	/// <summary>
	/// 로드된 스프라이트 아틀라스에서 스프라이트를 꺼내 반환하는 메서드
	/// </summary>
	/// <param name="eAtlas"></param>
	/// <param name="strSpriteName"></param>
	/// <returns>Sprite</returns>
	public Sprite LoadSprite(EAtlasType eAtlas, string strSpriteName)
	{
		if (_dicCachedSprite.ContainsKey(strSpriteName))
			return _dicCachedSprite[strSpriteName];

		SpriteAtlas atlas = GetAtlas(eAtlas);
		if (null != atlas)
		{
			Sprite spr = atlas.GetSprite(strSpriteName);
			if (null != spr) _dicCachedSprite.Add(strSpriteName, spr);
			return spr;
		}
		return null;
	}

    /// <summary>
    /// 텍스쳐 로드 후 반환하는 메서드.
    /// </summary>
    /// <param name="strTextureName"></param>
    /// <returns>Texture</returns>
    public Texture LoadTexture(string strTextureName)
	{
		if (_dicCachedObject.ContainsKey(strTextureName))
			return _dicCachedObject[strTextureName] as Texture;

		Object obj = Load<Object>($"{ComType.TEXTURE_PATH}{strTextureName}");
		if (null != obj)
		{
			_dicCachedObject.Add(strTextureName, obj);
			return obj as Texture;
		}
		return null;
	}

    /// <summary>
    /// 머터리얼 로드 후 반환하는 메서드.
    /// </summary>
    /// <param name="strMaterialName"></param>
    /// <returns>Material</returns>
    public Material LoadMaterial(string strMaterialName)
	{
		if (_dicCachedObject.ContainsKey(strMaterialName))
			return _dicCachedObject[strMaterialName] as Material;

		Object obj = Load<Object>($"{ComType.MATERIAL_PATH}{strMaterialName}");
		if (null != obj)
		{
			_dicCachedObject.Add(strMaterialName, obj);
			return obj as Material;
		}
		return null;
	}

    public AudioClip LoadAudioClip(string strAudioName)
    {
        if (_dicCachedObject.ContainsKey(strAudioName))
            return _dicCachedObject[strAudioName] as AudioClip;

        Object obj = Load<Object>($"{ComType.AUDIO_PATH}{strAudioName}");
        if (null != obj)
        {
            _dicCachedObject.Add(strAudioName, obj);
            return obj as AudioClip;
        }
        return null;
    }
}