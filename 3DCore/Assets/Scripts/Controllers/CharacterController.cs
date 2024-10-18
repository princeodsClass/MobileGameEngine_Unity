using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    Image _imgFill;

    Animator _animator;

    float _fRatioHP = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();      
    }

    // Update is called once per frame
    void Update()
    {
        SetHP();
        MoveRun();
        Punch();
    }

    void MoveRun()
    {
        _animator.SetBool(ComType.RUNVALUE, Input.GetKey(KeyCode.W));
        _animator.SetFloat(ComType.HPRATIO, _fRatioHP);
    }

    void Punch()
    {
        _animator.SetFloat("fPunchType", Random.Range(0, 2));

        if ( Input.GetKey(KeyCode.Space) )
        {
            if ( _animator.GetBool(ComType.RUNVALUE) )
            {
                _animator.SetTrigger("RunningPunch");
            }
            else
            {
                _animator.SetTrigger("Punch");
            }
        }
    }

    public void SetHP()
    {
        _fRatioHP = _imgFill.fillAmount < 0.5f ? 0f : 1f;
    }
}
