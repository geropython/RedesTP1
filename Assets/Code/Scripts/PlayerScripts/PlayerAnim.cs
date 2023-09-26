using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnim : NetworkBehaviour
{
    public Animator anim;

    private void Awake()
    {
    }
    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        else
        {
        }
    }
    void Update()
    {
        if (!IsOwner) return;
    }
}
