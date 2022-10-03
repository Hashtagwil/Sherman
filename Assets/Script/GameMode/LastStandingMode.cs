using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LastStandingMode : IGameMode
{
    public override bool isGameOver()
    {
        GameObject human = GameObject.FindGameObjectWithTag("Player");
        int remainingTeamCount = GetRemainingTeamCount();

        return (remainingTeamCount == 1);
    }

    public override bool isPermitRespawn()
    {
        return false;
    }
    public override string GetPlayerScore(PlayerInfo player)
    {
        return "";
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
