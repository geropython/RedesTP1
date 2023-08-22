using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public float speed;
    Rigidbody _rb;

    public Action onAttack;
    //public Func<int, bool> onAttack2 = delegate { };

    //public delegate int myTest(Vector3 dir, float speed, int crash);
    //public myTest onTest;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }
    public void Attack()
    {
        if (onAttack != null)
            onAttack();
    }
}
