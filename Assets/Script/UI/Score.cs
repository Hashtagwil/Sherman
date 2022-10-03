using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public enum ScorePosition
    {
        TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT
    }

    public PlayerInfo Player;
    public ScorePosition Position = ScorePosition.TOP_LEFT;

    private TextMeshProUGUI Text;
    public void Initialize(TankController controller, ScorePosition scoreLocation)
    {
        Text = GetComponent<TextMeshProUGUI>();
        if (DataPersistence.Instance.data.IsTeamModeOn)
        {
            Text.color = controller.PlayerController.playerInfo.teamColor;
        }
        else
        {
            Text.color = controller.GetTurretColor();
        }
        Player = controller.PlayerController.playerInfo;
        Position = scoreLocation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
        {
            Text.text = GameManager.SceneInstance.CurrentGameMode.GetPlayerScore(Player);
        }
    }
}
