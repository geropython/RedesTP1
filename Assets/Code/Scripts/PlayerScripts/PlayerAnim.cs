using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnim : NetworkBehaviour
{
    //public Animator anim;
    //PlayerModel _playerModel;
    //Rigidbody _rb;
    //private void Awake()
    //{
    //    _rb = GetComponent<Rigidbody>();
    //    _playerModel = GetComponent<PlayerModel>();
    //}
    //private void Start()
    //{
    //    if (!IsOwner)
    //    {
    //        enabled = false;
    //    }
    //    else
    //    {
    //        _playerModel.score.OnValueChanged += OnScoreChange;
    //    }
    //}
    //void Update()
    //{
    //    if (!IsOwner) return;
    //    anim.SetFloat("Vel", _rb.velocity.magnitude);
    //}
    //void OnScoreChange(int p, int n)
    //{
    //    if (!IsOwner) return;
    //    anim.SetTrigger("Spin");
    //}
}
