using Assets.Script.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NbKillMode : IGameMode
{
    public int NbKillToWin = 3;

    public override bool isGameOver()
    {
        Dictionary<Color, int> killsPerTeam = new Dictionary<Color, int>();
        GameObject[] players = PlayerUtils.GetAllPlayer();
        foreach (var player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (killsPerTeam.ContainsKey(pc.playerInfo.teamColor))
            {
                killsPerTeam[pc.playerInfo.teamColor] += pc.playerInfo.KillCount;
            }
            else
            {
                killsPerTeam.Add(pc.playerInfo.teamColor, pc.playerInfo.KillCount);
            }
            if (killsPerTeam[pc.playerInfo.teamColor] >= 3)
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
    public override string GetPlayerScore(PlayerInfo player)
    {
        return player.KillCount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
