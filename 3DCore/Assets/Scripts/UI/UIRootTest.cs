using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIRootTest : UIRootBase
{
	protected override void Initialize()
	{
		base.Initialize();

		_MenuMgr.OpenPage<PageTest>(EUIPage.PageTest);
	}

	public override void Clear()
	{
		base.Clear();
	}

	private void Start()
	{
		StartCoroutine(this.DoStart());
	}

	private IEnumerator DoStart()
	{
		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();
		System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Optimized, true, true);
	}
}