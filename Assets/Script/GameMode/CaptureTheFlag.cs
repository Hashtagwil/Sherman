using Assets.Script.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureTheFlag : IGameMode
{
    public override string GetPlayerScore(PlayerInfo player)
    {
        return player.FlagsCaptured.ToString();
    }

    public override bool isGameOver()
    {
        Dictionary<Color, int> flagsPerTeam = new Dictionary<Color, int>();
        GameObject[] players = PlayerUtils.GetAllPlayer();
        foreach (var player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (flagsPerTeam.ContainsKey(pc.playerInfo.teamColor))
            {
                flagsPerTeam[pc.playerInfo.teamColor] += pc.playerInfo.FlagsCaptured;
            }
            else
            {
                flagsPerTeam.Add(pc.playerInfo.teamColor, pc.playerInfo.FlagsCaptured);
            }
            if (flagsPerTeam[pc.playerInfo.teamColor] >= 3)
            {
                winningTeam = pc.playerInfo.teamColor;
                return true;
            }
        }
        return false;
    }

    public override bool isPermitRespawn()
    {
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool isRequiresFlags()
    {
        return true;
    }

}
