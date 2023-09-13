using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkAnimator : NetworkAnimator
{
    //Client Network para que sea Non Authoritative
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
