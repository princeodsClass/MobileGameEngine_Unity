using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWalk : StateMachineBehaviour
{
    private CharacterController _characterController;

    private Vector3 _mousePosition;
    private Vector3 _direction;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _characterController = animator.GetComponent<CharacterController>();

        Debug.Log("Ÿ���� ã�� �ȱ� �����մϴ�.");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_characterController == null)
            return;

        _characterController.MoveToTarget();
    }
}
