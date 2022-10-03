using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Utils
{
    internal class PlayerUtils
    {
        public static GameObject[] GetAllPlayer()
        {
            var ai = GameObject.FindGameObjectsWithTag("AI_Tank");
            var player = GameObject.FindGameObjectsWithTag("Player");
            return ai.Concat(player).ToArray();

        }

        public static bool IsTeamHasFlag(Color teamColor)
        {
            GameObject[] allTank = GetAllPlayer();
            foreach (GameObject currentTank in allTank)
            {
                bool hasFlag = currentTank.GetComponent<TankController>().hasFlag;
                Color flagOwnerColor = currentTank.GetComponent<TankController>().PlayerController.playerInfo.teamColor;
                if (hasFlag && UnityUtils.IsSameColorIgnoringAlpha(flagOwnerColor, teamColor))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
