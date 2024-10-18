using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    public float foroward, up, tup;

    void Update()
    {
        transform.position = _target.position + _target.forward * foroward + _target.up * up;
        transform.LookAt(_target.position + _target.up * tup);
    }
}
