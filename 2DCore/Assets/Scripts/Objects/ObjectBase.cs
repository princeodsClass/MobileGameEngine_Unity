using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    private static int m_nNextValidIndex = 0;
    private int _nIndex;
    private string _sName;

    public int Index
    {
        set
        {
            _nIndex = value;
            m_nNextValidIndex++;
        }
        get => _nIndex;
    }

    public virtual void SetObject(string name)
    {
        _nIndex = m_nNextValidIndex;
        _sName = name;
    }

    public abstract void Updated();
}
