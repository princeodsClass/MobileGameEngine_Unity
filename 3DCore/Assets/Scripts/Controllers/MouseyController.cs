using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseyController : MonoBehaviour
{
    [SerializeField]
    Slider _slider;

    [SerializeField]
    AudioClip[] _acRun;

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

        _animator.SetBool("isRun", Input.GetKey(KeyCode.W));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("Punch");
        }
    }

    public void FootStep()
    {
        if ( _slider.value < 0.5f )
            return;

        GameAudioManager.Singleton.PlaySFX3D(_acRun[Random.Range(0, _acRun.Length)], transform.position);
    }

    public void FootStepInjured()
    {
        if (_slider.value >= 0.5f)
            return;

        GameAudioManager.Singleton.PlaySFX3D(_acRun[Random.Range(0, _acRun.Length)], transform.position);
    }
}
