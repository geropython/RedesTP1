using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public GameObject particleExpPrefab;
    public GameObject _box;

    public void OnTriggerEnter(Collider other)
    {
        //si lo choca poner particula de explosion
        Instantiate(particleExpPrefab, other.transform.position, Quaternion.identity);
        if (other.CompareTag("Player"))
        {
            // Obtén una referencia al script ResetCheckpoint en el mismo GameObject o en otro objeto apropiado
            ResetCheckpoint resetCheckpoint = GetComponent<ResetCheckpoint>();

            // Verifica si se encontró un script ResetCheckpoint
            if (resetCheckpoint != null)
            {
                // Llama a la función para restablecer el checkpoint más cercano
                resetCheckpoint.ResetLastCheakPoint();
            }
        }
        //IMPORTANTE PARA EL NON AUTHORITATIVE!
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;
        //ID player
        var playerID = player.OwnerClientId;

        RequestChangeColorServerRpc(playerID);
        _box.SetActive(false);
    }

    [ServerRpc]
    public void RequestChangeColorServerRpc(ulong playerId)
    {
        ClientRpcParams p = new ClientRpcParams();
        List<ulong> list = new List<ulong>();
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (id != playerId)
            {
                list.Add(id);
            }
        }
        p.Send.TargetClientIds = list;
        ChangeColorClientRpc(playerId, p);
    }

    [ClientRpc]
    public void ChangeColorClientRpc(ulong playerId, ClientRpcParams p = default)
    {
        print(playerId + "  " + NetworkManager.Singleton.LocalClientId);
        if (playerId == NetworkManager.Singleton.LocalClientId) return;
        //ChangeColor;
        print("No es el que collisiono");
    }
}