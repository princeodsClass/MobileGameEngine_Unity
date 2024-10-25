using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Networking;

public partial class GameResourceManager : SingletonMono<GameResourceManager>
{
	private Transform _tRootObjectPool = null;

	private Dictionary<string, Queue<GameObject>> m_dicObjectPool = new Dictionary<string, Queue<GameObject>>();

	private List<Dictionary<string, GameObject>> m_listCachedPrefabs = new List<Dictionary<string, GameObject>>();
	private Dictionary<string, GameObject> m_dicUIPrefabs = new Dictionary<string, GameObject>();
	private Dictionary<int, SpriteAtlas> m_dicAtlas = new Dictionary<int, SpriteAtlas>();
	private Dictionary<string, Sprite> m_dicCachedSprite = new Dictionary<string, Sprite>();
	private Dictionary<string, Object> m_dicCachedObject = new Dictionary<string, Object>();

	public bool IsInitTableData { get; private set; }

	private void Awake()
	{
		transform.hasChanged = false;

		if (0 == m_listCachedPrefabs.Count)
		{
			for (int i = 0; i < (int)EResourceType.End; ++i)
				m_listCachedPrefabs.Add(new Dictionary<string, GameObject>());
		}
	}

	private void OnDestroy()
	{
		m_dicAtlas.Clear();
		m_dicCachedSprite.Clear();
		m_dicCachedObject.Clear();
		m_dicObjectPool.Clear();

		for (int i = 0; i < m_listCachedPrefabs.Count; ++i)
			m_listCachedPrefabs[i].Clear();
	}

	public void Clear()
	{
		Dictionary<string, Queue<GameObject>>.Enumerator it = m_dicObjectPool.GetEnumerator();
		while (it.MoveNext())
			it.Current.Value.Clear();

		m_dicObjectPool.Clear();
		if (null != _tRootObjectPool)
			_tRootObjectPool.DestroyChildren(false);

		m_dicAtlas.Clear();
		m_dicCachedSprite.Clear();
		m_dicCachedObject.Clear();

		for (int i = 0; i < m_listCachedPrefabs.Count; ++i)
			m_listCachedPrefabs[i].Clear();
	}

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

	public GameObject LoadPrefabs(EResourceType eType, string strPrefName, GameObject a_oOrigin = null, int a_nTheme = 0)
	{
		GameObject goPref;
		if (!m_listCachedPrefabs[(int)eType].TryGetValue(strPrefName, out goPref))
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

			m_listCachedPrefabs[(int)eType].Add(strPrefName, goPref);
		}
		return goPref;
	}

	public T Load<T>(string path) where T : UnityEngine.Object
	{
		return Resources.Load<T>(path);
	}

	public IEnumerator LoadAsync<T>(string path, System.Action<T> callBack) where T : UnityEngine.Object
	{
		ResourceRequest req = Resources.LoadAsync<T>(path);
		yield return req;
		if (req.asset) callBack(req.asset as T);
		if (req.asset) yield break;
	}


	private GameObject LoadUIPrefabs(EResourceType eType, string strPrefName)
	{
		GameObject goPref;

		if (!m_dicUIPrefabs.TryGetValue(strPrefName, out goPref))
		{
			string strPath = GetPrefabPath(eType, strPrefName);
			goPref = Load<GameObject>(strPath);

			if (null == goPref)
			{
				GameManager.Log("Resource Path Error == Type:" + eType + " Path:" + strPath, "red");
				return null;
			}

			m_dicUIPrefabs.Add(strPrefName, goPref);
		}

		return goPref;
	}

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
		if (m_dicObjectPool.ContainsKey(strObjName))
		{
			if (0 != m_dicObjectPool[strObjName].Count)
			{
				go = m_dicObjectPool[strObjName].Dequeue();
				if (null != go) go.transform.SetParent(null);
			}
		}
		else
		{
			if (bCached)
				m_dicObjectPool.Add(strObjName, new Queue<GameObject>());
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

	public GameObject CreateObject(EResourceType eType, string strPrefName, Transform tParent, float fLifeTime = 0f, bool bCached = true, int a_nTheme = 0)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity, fLifeTime, bCached, a_nTheme: a_nTheme);
		if (null == go) return null;

		ComUtil.SetParent(tParent, go.transform);
		return go;
	}

	public GameObject CreateObject(EResourceType eType, string strPrefName, Vector3 vec, bool bCached = true)
	{
		GameObject go = CreateObject(eType, strPrefName, vec, Quaternion.identity, 0f, bCached);
		return go;
	}

	public GameObject CreateObject(EResourceType eType, string strPrefName, bool bCached = true)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity, 0f, bCached);
		return go;
	}

	public GameObject CreateUIObject(EResourceType eType, string strPrefName, Transform tParent)
	{
		GameObject go = CreateObject(eType, strPrefName, Vector3.zero, Quaternion.identity);
		if (null == go) return null;

		ComUtil.SetParent(tParent, go.transform);
		go.SetActive(true);
		return go;
	}

	private void CheckDestroyObject(GameObject go, float fLifeTime)
	{
		AutoDestroy goAD = go.GetComponent<AutoDestroy>();
		if (null == goAD) goAD = go.AddComponent<AutoDestroy>();

		goAD.Init(fLifeTime);
	}

	public void ReleaseObject(GameObject go, bool bInsertPool = true)
	{
		if (null == go) return;
		if (string.IsNullOrEmpty(go.name)) return;

		try
		{
			if (bInsertPool)
			{
				if (!m_dicObjectPool.ContainsKey(go.name))
					m_dicObjectPool.Add(go.name, new Queue<GameObject>());

				go.SetActive(false);

				if (!m_dicObjectPool[go.name].Contains(go))
				{
					m_dicObjectPool[go.name].Enqueue(go);

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

	private SpriteAtlas GetAtlas(EAtlasType eAtlas)
	{
		int nKey = (int)eAtlas;
		if (m_dicAtlas.ContainsKey(nKey))
			return m_dicAtlas[nKey];

		string strPath = $"{ComType.ATLAS_PATH}{eAtlas}";
		SpriteAtlas atlas = Load<SpriteAtlas>(strPath);
		if (null != atlas)
			m_dicAtlas.Add(nKey, atlas);

		return atlas;
	}

	public Sprite LoadSprite(EAtlasType eAtlas, string strSpriteName)
	{
		if (m_dicCachedSprite.ContainsKey(strSpriteName))
			return m_dicCachedSprite[strSpriteName];

		SpriteAtlas atlas = GetAtlas(eAtlas);
		if (null != atlas)
		{
			Sprite spr = atlas.GetSprite(strSpriteName);
			if (null != spr) m_dicCachedSprite.Add(strSpriteName, spr);
			return spr;
		}
		return null;
	}

	public Texture LoadTexture(string strTextureName)
	{
		if (m_dicCachedObject.ContainsKey(strTextureName))
			return m_dicCachedObject[strTextureName] as Texture;

		Object obj = Load<Object>($"{ComType.TEXTURE_PATH}{strTextureName}");
		if (null != obj)
		{
			m_dicCachedObject.Add(strTextureName, obj);
			return obj as Texture;
		}
		return null;
	}

	public Material LoadMaterial(string strMaterialName)
	{
		if (m_dicCachedObject.ContainsKey(strMaterialName))
			return m_dicCachedObject[strMaterialName] as Material;

		Object obj = Load<Object>($"{ComType.MATERIAL_PATH}{strMaterialName}");
		if (null != obj)
		{
			m_dicCachedObject.Add(strMaterialName, obj);
			return obj as Material;
		}
		return null;
	}

	public void GetDownloadTexture(string strURL, string strFolderName, System.Action<Texture2D> callBack)
	{
		if (string.IsNullOrEmpty(strURL))
		{
			callBack.Invoke(null);
			return;
		}

		if (m_dicCachedObject.ContainsKey(strURL))
		{
			GameManager.Log("yellow", $"============== Caching URL : {strURL}");
			callBack.Invoke(m_dicCachedObject[strURL] as Texture2D);
			return;
		}

		if (!File.Exists(ComUtil.GetFullFilePath(strURL, strFolderName)))
		{
			StartCoroutine(CoDownloadImage(strURL, strFolderName, callBack));
			return;
		}

		byte[] bytes = File.ReadAllBytes(ComUtil.GetFullFilePath(strURL, strFolderName));
		if (0 == bytes.Length)
		{
			callBack.Invoke(null);
			return;
		}

		//GameManager.Log("yellow", $"============== LoadImage URL : {strURL}");
		Texture2D tex = new Texture2D(128, 128, TextureFormat.ASTC_6x6, false);
		tex.LoadImage(bytes);
		callBack.Invoke(tex);
	}

	IEnumerator CoDownloadImage(string strURL, string strFolderName, System.Action<Texture2D> callBack)
	{
		UnityWebRequest cReq = UnityWebRequestTexture.GetTexture(strURL);
		yield return cReq.SendWebRequest();

		string strPath = ComUtil.GetFullFilePath(strURL, strFolderName);
		string strDir = strPath.Substring(0, strPath.LastIndexOf('/') + 1);
		if (!Directory.Exists(strDir)) Directory.CreateDirectory(strDir);

		if (string.IsNullOrEmpty(cReq.error))
		{
			//GameManager.Log("yellow", $"============== CoDownloadImage SaveDir:{strDir}\n SavePath: {strPath}");

			File.WriteAllBytes(strPath, cReq.downloadHandler.data);
			Texture2D tex = ((DownloadHandlerTexture)cReq.downloadHandler).texture;
			m_dicCachedObject.Add(strURL, tex);

			callBack.Invoke(tex);
		}
		else
		{
			GameManager.Log($"Event CoDownloadImage Error: {cReq.error}", "red");
			callBack.Invoke(null);
		}
	}

	public IEnumerator GetRequest<T>(string strURL, System.Action<string> callBack)
	{
		UnityWebRequest cReq = UnityWebRequest.Get(strURL);
		yield return cReq.SendWebRequest();

		if (string.IsNullOrEmpty(cReq.error))
		{
			if (null != callBack)
				callBack.Invoke(cReq.downloadHandler.text);
		}
		else
		{
			GameManager.Log($"GetRequestDownload Error: {cReq.error} URL:{strURL}", "red");
		}
	}

	public GameObject CreateObject(GameObject a_oObj, Transform tParent, Transform a_oDummy, float fLifeTime = 0f, bool bCached = true, int a_nTheme = 0)
	{
		GameObject go = CreateObject(EResourceType.Custom, a_oObj.name, Vector3.zero, Quaternion.identity, fLifeTime, bCached, a_oObj, a_nTheme);
		if (null == go) return null;

		ComUtil.SetParent(tParent, go.transform);

		// 더미가 존재 할 경우
		if (a_oDummy != null)
		{
			go.transform.rotation = a_oDummy.rotation;
			go.transform.position = a_oDummy.position;
		}

		return go;
	}

	public T CreateObject<T>(EResourceType eType, string strPrefName, Transform tParent, float fLifeTime = 0f, bool bCached = true, int a_nTheme = 0) where T : Component
	{
		return this.CreateObject(eType, strPrefName, tParent, fLifeTime, bCached, a_nTheme)?.GetComponentInChildren<T>();
	}

	public T CreateObject<T>(GameObject a_oObj, Transform tParent, Transform a_oDummy, float fLifeTime = 0f, bool bCached = true, int a_nTheme = 0) where T : Component
	{
		return this.CreateObject(a_oObj, tParent, a_oDummy, fLifeTime, bCached, a_nTheme)?.GetComponentInChildren<T>();
	}
}