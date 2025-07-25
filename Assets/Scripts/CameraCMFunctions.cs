using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCMFunctions : MonoBehaviour
{
    public Animator _animator;

    public void SetAnimatorVelocity(float speed)
    {
        _animator.speed = speed;
    }
}
