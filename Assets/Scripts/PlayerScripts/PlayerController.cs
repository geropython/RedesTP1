using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerModel _playerModel;
    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        //SaveLoad.SerializationJSON()
    }
    private void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerModel.Move(dir.normalized);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerModel.Attack();
        }
    }
}
