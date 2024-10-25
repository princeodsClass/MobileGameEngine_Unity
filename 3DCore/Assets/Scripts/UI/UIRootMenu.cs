using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class UIRootMenu : UIRootBase
{
	protected override void Initialize()
	{
		base.Initialize();

        _MenuMgr.OpenPage<PageMenu>(EUIPage.PageMenu);
	}
}