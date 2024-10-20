using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseyController : MonoBehaviour
{
    [SerializeField]
    Slider _slider;

    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _animator.SetFloat("HP", _slider.value);

        if ( Input.GetKeyDown(KeyCode.Alpha1) )
        {
            _animator.SetTrigger("Dance");
        }

        /*
        if ( Input.GetKey(KeyCode.W) )
        {
            _animator.SetBool("isRun", true);
        }
        else
        {
            _animator.SetBool("isRun", false);
        }
        */

        _animator.SetBool("isRun", Input.GetKey(KeyCode.W));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("Punch");
        }
    }
}
