using Unity.Netcode;

public class CrashAnim : NetworkBehaviour
{
    //UTILIZA CLIENTNETWORK ANIMATOR PAR QUE SEA NON AUTHORITATIVE.
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