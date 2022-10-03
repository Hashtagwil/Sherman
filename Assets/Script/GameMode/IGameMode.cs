using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class IGameMode : MonoBehaviour
{
    private const float DEFAULT_RESPAWN_DELAY = 4f;
    private const float DEFAULT_TIMER_MAX_TIME = 0f;

    protected Color winningTeam;

    public abstract bool isGameOver();
    public abstract bool isPermitRespawn();
    public virtual float GetMaxTime()
    {
        return DEFAULT_TIMER_MAX_TIME;
    }
    public Color getWinner()
    {
        return winningTeam;
    }
    public abstract string GetPlayerScore(PlayerInfo player);
    public float GetRespawnDelay()
    {
        return DEFAULT_RESPAWN_DELAY;
    }

    protected int GetRemainingTeamCount()
    {
        var players = GameObject.FindObjectsOfType<PlayerController>();
        Dictionary<Color, int> playerPerColor = new Dictionary<Color, int>();
        foreach (var player in players)
        {
            Color team = player.GetComponent<PlayerController>().playerInfo.teamColor;
            if (playerPerColor.ContainsKey(team))
            {
                playerPerColor[team]++;
            }
            else
            {
                playerPerColor.Add(team, 1);
            }
        }
        if (playerPerColor.Count == 1)
        {
            winningTeam = playerPerColor.Keys.ElementAt(0);
        }
        return playerPerColor.Count;
    }

    public virtual bool isRequiresFlags()
    { 
        return false; 
    }

}
