using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{
    public NetPartyPlayerInfo info;

    public override void OnNetworkSpawn()
    {
        // Load player client side
        if (IsOwner)
        {
            // Is this the host? Create a Party
            if (IsServer)
            {
                NetworkObject party = Instantiate(Resources.Load("Net/NetParty") as GameObject).GetComponent<NetworkObject>();
                party.Spawn(destroyWithScene: false);
            }

            // Get from account info
            info = new NetPartyPlayerInfo()
            {
                playerId = OwnerClientId,
                tankIndex = DataPersistence.Instance.data.TankIndex,
                colorBody = DataPersistence.Instance.data.BodyColor,
                colorTurret = DataPersistence.Instance.data.TurretColor
            };
            JoinPartyServerRpc(OwnerClientId, info);
        }
    }

    [ServerRpc]
    public void JoinPartyServerRpc(ulong clientId, NetPartyPlayerInfo player)
    {
        FindObjectOfType<NetParty>().Join(clientId, player);

        ClientRpcParams singleTarget = NetCodeHelper.SingleTarget(clientId);

        FindObjectOfType<NetParty>().SetupGameClientRpc(
            DataPersistence.Instance.data.GameModeScriptName,
            DataPersistence.Instance.data.Map,
            DataPersistence.Instance.data.Player1Color,
            DataPersistence.Instance.data.Player2Color,
            DataPersistence.Instance.data.Player3Color,
            DataPersistence.Instance.data.Player4Color,
            DataPersistence.Instance.data.IsTeamModeOn,
            DataPersistence.Instance.data.LeaveCarcass,
            singleTarget
        );
    }
}
