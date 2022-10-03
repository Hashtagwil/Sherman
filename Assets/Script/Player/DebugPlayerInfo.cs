using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugPlayerInfo : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public int FlagsCaptured;
    public Color TeamColor;

    public static DebugPlayerInfo CreateDebugPlayerInfo(PlayerInfo playerInfo)
    {
        DebugPlayerInfo defaultPlayerInfos = GameObject.FindObjectOfType<SpawnManager>().DefaultDebugPlayerInfo;
        DebugPlayerInfo debgPlayerInfos = Instantiate<DebugPlayerInfo>(defaultPlayerInfos);
        debgPlayerInfos.playerInfo = playerInfo;
        debgPlayerInfos.TeamColor = playerInfo.teamColor;
        
        return debgPlayerInfos;
    }

    private void Update()
    {
        FlagsCaptured = playerInfo.FlagsCaptured;
        TeamColor = playerInfo.teamColor;
    }
}
