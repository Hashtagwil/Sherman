using Assets.Script.GameMode;
using Assets.Script.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OneMin : IGameMode, ITimedGameMode
{
    private float maxTime = 60;
    public float currentTime { get; set; }
    public override string GetPlayerScore(PlayerInfo player)
    {
        return player.KillCount.ToString();
    }
    public override float GetMaxTime()
    {
        return maxTime;
    }

    public override bool isPermitRespawn()
    {
        return true;
    }

    public override bool isGameOver()
    {
        if (currentTime <= 0)
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
            }
            winningTeam = killsPerTeam.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return true;
        }
        return false;
    }
}
