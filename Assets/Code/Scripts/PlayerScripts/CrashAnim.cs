using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class CrashAnim : NetworkBehaviour
{
    public ClientNetworkAnimator _anim;

    [ClientRpc]
    public void TriggerLaughAnimationClientRpc()
    {
        _anim.SetTrigger("Laugh");
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerLaughAnimationServerRpc()
    {
        TriggerLaughAnimationClientRpc();
    }
}

