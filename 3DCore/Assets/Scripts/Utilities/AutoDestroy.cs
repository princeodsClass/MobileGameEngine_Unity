using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	private GameObject _goCached = null;
	private float _fLifeTime = 0f;

	public void Init(float fLifeTime)
	{
		if (null == _goCached) _goCached = gameObject;
		_fLifeTime = fLifeTime;

		StopCoroutine("CoProc");
		StartCoroutine(CoProc());
	}

	public void OnDisable()
	{
		StopCoroutine("CoProc");
	}

	IEnumerator CoProc()
	{
		yield return new WaitForSeconds(_fLifeTime);
		GameResourceManager.Singleton.ReleaseObject(_goCached);
	}
}
