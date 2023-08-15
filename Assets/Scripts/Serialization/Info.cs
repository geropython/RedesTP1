using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Info
{
    public int bullets = 20;
    [SerializeField]
    int _maxLife;

    public int SetMaxLife
    {
        set
        {
            _maxLife = value;
        }
    }
}
