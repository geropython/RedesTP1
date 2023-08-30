using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;

//public class PlayerController : NetworkBehaviour
//{
//    PlayerModel _model;

//    void Start()
//    {
//        if (IsOwner)
//        {
//            _model = GetComponent<PlayerModel>();
//        }
//        else
//        {
//            Destroy(this);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!IsOwner) return;
//        var dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
//        _model.Move(dir);
//    }
//}
