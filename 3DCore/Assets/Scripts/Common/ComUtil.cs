using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public static class ComUtil
{
	static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

	public static void SetParent(Transform tParent, Transform tChild)
	{
		if (null == tParent) return;

		RectTransform tRect = tChild.GetComponent<RectTransform>();

		if (null == tRect)
		{
			tChild.SetParent(tParent);
			tChild.Reset();
		}
		else
		{
			SetParent(tParent, tRect);
		}

		tChild.SetAsLastSibling();
	}

	public static void SetParent(Transform tParent, RectTransform tChild)
	{
		//if (null == tParent) return;

		tChild.SetParent(tParent);
		tChild.Reset();
	}

	public static string EnUTC(double ms = 0)
	{
		return DateTime.UtcNow.AddMilliseconds(ms).ToString("yyyy-MM-dd HH:mm:ss");
	}

	public static string EnDatetime(DateTime dt)
	{
		return dt.ToString("yyyy-MM-dd HH:mm:ss");
	}

	public static Vector2 GetTouchUIPos()
	{
		Vector2 vPos;
#if UNITY_EDITOR
		vPos = Input.mousePosition;
#else
		vPos = Input.GetTouch(0).position;
#endif
		return vPos;
	}

	public static string GetFullFilePath(string strURL, string strFolderName)
	{
		return $"{Application.persistentDataPath}/{strFolderName}/{GetSafeFilePath(strURL)}";
	}

	public static string GetSafeFilePath(string strURL)
	{
		UnityEngine.Debug.Assert(GetAbsolutePath(strURL).Length < ComType.FILE_NAME_LEN_MAX, "FileName must be less than name max");
		return string.Join("_", GetAbsolutePath(strURL).Split(Path.GetInvalidFileNameChars()));
	}

	public static string GetAbsolutePath(string strURL)
	{
		Uri uri = new Uri(strURL);
		return uri.AbsoluteUri;
	}

	public static void DeleteFile(string strFolderName)
	{
		string strDir = $"{Application.persistentDataPath}/{strFolderName}";
		string[] strFiles = Directory.GetFiles(strDir);
		for (int i = 0; i < strFiles.Length; ++i)
			File.Delete(strFiles[i]);
	}

	public static bool ContainsParam(this Animator _Anim, string _ParamName)
	{
		for (int i = 0; i < _Anim.parameters.Length; ++i)
		{
			if (_Anim.parameters[i].name.Equals(_ParamName, StringComparison.OrdinalIgnoreCase))
				return true;
		}
		return false;
	}

	public static bool AlmostEquals(this float fTarget, float fSecond, float fDiff)
	{
		return Mathf.Abs(fTarget - fSecond) < fDiff;
	}

	public static bool AlmostEquals(this Quaternion qTarget, Quaternion qSecond, float fAngle)
	{
		return Quaternion.Angle(qTarget, qSecond) < fAngle;
	}

	public static bool AlmostEquals(this Vector3 qTarget, Vector3 qSecond, float fDistance)
	{
		return Vector3.Distance(qTarget, qSecond) < fDistance;
	}

	public static T GetComponentInChildren<T>(this GameObject obj, bool includeInactive) where T : Component
	{
		T[] ret = obj.GetComponentsInChildren<T>(includeInactive);
		if (null == ret || ret.Length == 0) return null;
		return ret[0];
	}

	static public void DestroyChildren(this Transform tf, bool bInsertPool = true)
	{
		if ( null == tf ) return;

		while (0 != tf.childCount)
		{
			Transform tfChild = tf.GetChild(0);
			tfChild.SetParent(null);
			GameResourceManager.Singleton.ReleaseObject(tfChild.gameObject, bInsertPool);
		}
	}

	static public void DestroyChildren(this Transform tf, int nIndex)
	{
		if (nIndex < tf.childCount)
		{
			Transform tfChild = tf.GetChild(nIndex);
			tfChild.SetParent(null);
			GameResourceManager.Singleton.ReleaseObject(tfChild.gameObject);
		}
	}

	static public void Reset(this Transform tf)
	{
		tf.localPosition = Vector3.zero;
		tf.localRotation = Quaternion.identity;
		tf.localScale = Vector3.one;
	}

	static public void Reset(this RectTransform tf)
	{
		tf.anchoredPosition3D = Vector3.zero;
		tf.sizeDelta = Vector2.zero;
		tf.localScale = Vector3.one;
	}

	/// <summary>
	/// 실행중인 os 확인.
	/// </summary>
	public static EOSType CheckOS()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
            return EOSType.Android;
#elif UNITY_IOS && !UNITY_EDITOR
            return EOSType.iOS;
#else
		return EOSType.Editor;
#endif
	}

	/// <summary>
	/// 디바이스 고유 값 획득.
	/// </summary>
	public static string GetUniqueID()
	{
		switch (CheckOS())
		{
			case EOSType.Editor:
				string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				path = Path.Combine(path, "ninetap");

				string filePath = Path.Combine(path, "uuid");
				string uuid = "";

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				if (File.Exists(filePath))
				{
					using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					using (var reader = new BinaryReader(stream))
					{
						uuid = Encoding.UTF8.GetString(reader.ReadBytes((int)stream.Length));
					}
				}
				else
				{
					uuid = Guid.NewGuid().ToString();
					using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
					using (var writer = new BinaryWriter(stream))
					{
						writer.Write(Encoding.UTF8.GetBytes(uuid));
					}
				}

				return uuid;

			case EOSType.Android:
				string androidId = "";

				try
				{
					AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
					AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
					androidId = settingsSecure.CallStatic<string>("getString", context.Call<AndroidJavaObject>("getContentResolver"), "android_id");
				}
				catch (AndroidJavaException e)
				{
					GameManager.Log("AndroidJavaException: " + e.Message, "red");
				}

				return androidId;
#if UNITY_IOS
			case EOSType.iOS:
				return Device.vendorIdentifier;
#endif
		}

		return null;
	}

	/// <summary>
	/// min ~ max 사이의 랜덤 영문 스트링
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	/// <returns></returns>
	public static string RandString(int min, int max)
	{
		string result = null;

		int length = new System.Random().Next(min, max + 1);

		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < length; i++)
		{
			int index = new System.Random().Next(chars.Length);
			result += chars[index];
		}

		return result;
	}

	/// <summary>
	/// csv 형태로 스트링 스플릿
	/// </summary>
	/// <param name="line"></param>
	/// <returns></returns>
	public static string[] SplitCsvLine(string line)
	{
		bool inQuotes = false;
		var columns = new List<string>();
		var currentColumn = "";

		foreach (char c in line)
		{
			if (c == '"')
			{
				inQuotes = !inQuotes;
			}
			else if (c == ',' && !inQuotes)
			{
				columns.Add(currentColumn.Trim('"'));
				currentColumn = "";
			}
			else
			{
				currentColumn += c;
			}
		}

		columns.Add(currentColumn.Trim('"'));

		return columns.ToArray();
	}

	public static void ReStart()
	{
		MenuManager.Singleton.SceneEnd();
		MenuManager.Singleton.SceneNext(ESceneType.MemuScene);
	}

	/// <summary>
	/// 두 개의 딕셔너리 합치기. key 가 같으면, value 더하기.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="dict1"></param>
	/// <param name="dict2"></param>
	/// <returns></returns>
	public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
	{
		Dictionary<TKey, TValue> mergedDict = new Dictionary<TKey, TValue>(dict1);

		foreach (KeyValuePair<TKey, TValue> kvp in dict2)
		{
			TKey key = kvp.Key;
			TValue value = kvp.Value;

			if (mergedDict.ContainsKey(key))
			{
				TValue existingValue = mergedDict[key];
				TValue newValue = AddValues(existingValue, value);
				mergedDict[key] = newValue;
			}
			else
			{
				mergedDict.Add(key, value);
			}
		}

		return mergedDict;
	}

	private static TValue AddValues<TValue>(TValue value1, TValue value2)
	{
		if (typeof(TValue) == typeof(int))
		{
			int result = Convert.ToInt32(value1) + Convert.ToInt32(value2);
			return (TValue)Convert.ChangeType(result, typeof(TValue));
		}
		else if (typeof(TValue) == typeof(long))
		{
			long result = Convert.ToInt64(value1) + Convert.ToInt64(value2);
			return (TValue)Convert.ChangeType(result, typeof(TValue));
		}

		return value1;
	}

	public static string ChangeNumberFormat(int number)
	{
		if (number >= 10000000)
		{
			float shortenedNumber = number / 1000000f;
			return shortenedNumber.ToString("0.###") + "M";
		}
		else if (number >= 10000)
		{
			float shortenedNumber = number / 1000f;
			return shortenedNumber.ToString("0.#") + "K";
		}
		else
		{
			return number.ToString();
		}
	}

	public static void FindFileFromFolder(string path, ref List<string> list)
	{

		string[] directories = Directory.GetDirectories(path);
		for (int i = 0, ii = directories.Length; ii > i; ++i)
		{
			FindFileFromFolder(directories[i], ref list);
		}

		string[] files = Directory.GetFiles(path);
		for (int i = 0, ii = files.Length; ii > i; ++i)
		{
			string ext = Path.GetExtension(files[i]);
			if (ext == ".meta") continue;

			list.Add(files[i]);
		}
	}

	public static string[] DivideString(string word)
	{
		char[] tok = new char[1] { '|' };

		string[] arrWord = word.Split(tok);

		return arrWord;
	}

	public static Transform Search(Transform target, string name)
	{
		if (target.name == name) return target;

		for (int i = 0; i < target.childCount; ++i)
		{
			Transform result = Search(target.GetChild(i), name);

			if (result != null) return result;
		}

		return null;
	}

	public static Transform[] SearchAll(Transform target, string name)
	{
		List<Transform> list = new List<Transform>();

		for (int i = 0; i < target.childCount; ++i)
		{
			Transform result = Search(target.GetChild(i), name);

			if (result != null)
				list.Add(result);
		}

		return list.ToArray();
	}

	public static void ChangeLayer(Transform t, int layer)
	{
		t.gameObject.layer = layer;

		for (int i = 0, ii = t.childCount; ii > i; ++i)
		{
			Transform tmp = t.GetChild(i);
			tmp.gameObject.layer = layer;

			if (tmp.childCount != 0)
				ChangeLayer(tmp, layer);
		}
	}

	public static string LoadFile(string output)
	{
		if (true == Application.isMobilePlatform)
		{
			TextAsset _textAsset = Resources.Load<TextAsset>(output);
			return _textAsset.text;
		}

		string str = string.Empty;

		if (File.Exists(output) == false)
			return str;

		using (FileStream reader = File.OpenRead(output))
		{
			byte[] temp = new byte[reader.Length];
			reader.Read(temp, 0, (int)reader.Length);
			reader.Close();

			str = System.Text.Encoding.UTF8.GetString(temp);
		}

		return str;
	}

	public static void SaveFile(string output, string str)
	{
		if (!Directory.Exists(Path.GetDirectoryName(output)))
			Directory.CreateDirectory(Path.GetDirectoryName(output));

		if (File.Exists(output))
			File.Delete(output);

		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
		using (FileStream writer = File.Create(output))
		{
			writer.Write(bytes, 0, bytes.Length);
			writer.Close();
		}
	}

	public static long GetCurrentTimestamp()
	{
		System.TimeSpan t = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1));
		return System.Convert.ToInt64(t.TotalSeconds * 1000);
	}

	public static System.DateTime ConvertTimestamp(long timestamp)
	{
		return (new System.DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(timestamp * 0.001);
	}

	public static void StartCheckTimer(string functionName = "")
	{
		sw.Start();

		if (string.IsNullOrEmpty(functionName) == true)
			GameManager.Log("시간 측정을 시작합니다.");
		else
			GameManager.Log(functionName + "의 시간 측정을 시작합니다.");
	}

	public static void StopCheckTimer()
	{
		sw.Stop();
		GameManager.Log(string.Format("총 {0} 초가 걸렸습니다.", sw.ElapsedMilliseconds * 0.001f));
		sw.Reset();
	}

	public static float GetNowStopWatchTime()
	{
		return sw.ElapsedMilliseconds;
	}

	public static DateTime String2Datetime(string value)
	{
		if (DateTime.TryParse(value, out DateTime result))
			return result;
		else
			return default(DateTime);
	}

	public static void GetChildrenNames(ref List<string> childrenNames, Transform root)
	{
		foreach (Transform child in root)
		{
			childrenNames.Add(child.name);
			GetChildrenNames(ref childrenNames, child);
		}
	}

	public static void GetComponentsInChildren<T>(GameObject a_oGameObj, List<T> a_oOutComponentList)
	{
		for (int i = 0; i < a_oGameObj.transform.childCount; ++i)
		{
			// 컴포넌트가 존재 할 경우
			if (a_oGameObj.transform.GetChild(i).TryGetComponent(out T oComponent))
			{
				a_oOutComponentList.Add(oComponent);
			}

			ComUtil.GetComponentsInChildren(a_oGameObj.transform.GetChild(i).gameObject, a_oOutComponentList);
		}
	}

	public static Transform FindChildByName(string ThisName, Transform ThisObj)
	{
		Transform ReturnObj;

		if (ThisObj.name == ThisName)
			return ThisObj.transform;

		foreach (Transform child in ThisObj)
		{
			ReturnObj = FindChildByName(ThisName, child);

			if (ReturnObj != null)
				return ReturnObj;
		}

		return null;
	}

	public static T FindComponentByName<T>(string ThisName, Transform ThisObj) where T : Component
	{
		return ComUtil.FindChildByName(ThisName, ThisObj)?.GetComponentInChildren<T>();
	}

	public static void Collect<T>(List<T> container, Transform parent) where T : Component
	{
		foreach (Transform tr in parent)
		{
			T[] actorModels = tr.gameObject.GetComponents<T>();
			if (null != actorModels)
			{
				container.AddRange(actorModels);
			}

			Collect(container, tr);
		}
	}

    public static void ChangeLayersRecursively(Transform trans, string name)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach (Transform child in trans)
		{
			ChangeLayersRecursively(child, name);
		}
	}

	public static void ChangeLayersRecursively(Transform trans, int layer)
	{
		trans.gameObject.layer = layer;
		foreach (Transform child in trans)
		{
			ChangeLayersRecursively(child, layer);
		}
	}

	public static float Random(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	public static int Random(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	public static int ChangeHex(string hex)
	{
		return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
	}

	public static string ConvertByteToHexString(byte[] convertArr)
	{
		string convertArrString = string.Empty;
		convertArrString = string.Concat(Array.ConvertAll(convertArr, byt => byt.ToString("X2")));
		return convertArrString;
	}

	public static byte[] ConvertByteArray(int value)
	{
		byte[] intByte = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian)
			Array.Reverse(intByte);

		return intByte;
	}
}
