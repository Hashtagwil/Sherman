using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NetCodeHelper
{

    public const ulong SERVER_ID = 0;

    public static ClientRpcParams SingleTarget(ulong clientId)
    {
        ulong[] singleTarget = new ulong[1];
        singleTarget[0] = clientId;
        ClientRpcParams rpcParams = default;
        rpcParams.Send.TargetClientIds = singleTarget;
        return rpcParams;
    }
}
