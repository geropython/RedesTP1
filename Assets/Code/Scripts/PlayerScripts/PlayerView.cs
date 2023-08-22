using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    Rigidbody _rb;
    Animator _anim;
    PlayerModel _playerModel;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerModel = GetComponent<PlayerModel>();
        _playerModel.onAttack += OnAttack;
    }
    private void Update()
    {
        _anim.SetFloat("Vel", _rb.velocity.magnitude);
        if (_rb.velocity.magnitude > 0)
            LookDir(_rb.velocity);
    }
    void LookDir(Vector3 dir)
    {
        dir.y = 0;
        transform.forward = dir.normalized;
    }
    void OnAttack()
    {
        _anim.SetTrigger("Spin");
    }
}
