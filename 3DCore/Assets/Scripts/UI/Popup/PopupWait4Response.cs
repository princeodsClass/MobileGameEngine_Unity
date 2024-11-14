using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupWait4Response : UIDialog
{
    private void Awake() { Initialize(); }
    public override void Initialize() { base.Initialize(); }
    public override void Open() { base.Open(); }
    public override void Close() { base.Close(); }
    public override void Escape()
    {
        return;
    }
}
