using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    private bool teamMode = false;
    public TeamToggle[] TeamToggles;
    public Toggle TeamModeToggle;
    public GameModeSelect DisableIfNoTeams;

    private void Start()
    {
        if (DataPersistence.Instance.data.IsTeamModeOn)
        {
            TeamModeToggle.isOn = true;
            DisableIfNoTeams.SetEnabled(true);
        }
        else
        {
            DisableIfNoTeams.SetEnabled(false);
        }

        if (DataPersistence.Instance.TypeClient != TypeClient.Local)
        {
            FindObjectOfType<ChangeControlType>().DesactivateControlType();
            FindObjectOfType<NumberPlayersSelect>().DesactivateAll();
            UpdatePlayerList(FindObjectOfType<NetParty>().players);
        }

        if(DataPersistence.Instance.TypeClient == TypeClient.Client)
        {
            GameObject.Find("GoButton").SetActive(false);
            GameObject.Find("RandomButton").SetActive(false);
        }
    }

    private void Update()
    {
        TeamModeToggle.isOn = DataPersistence.Instance.data.IsTeamModeOn;

    }

    public void Play()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(DataPersistence.Instance.data.Map, LoadSceneMode.Single);
    }

    public void ChangeTeamMode()
    {
        teamMode = !teamMode;
        foreach (TeamToggle toggle in TeamToggles)
        {
            toggle.gameObject.SetActive(teamMode);
        }
        DisableIfNoTeams.SetEnabled(teamMode);
        DataPersistence.Instance.data.IsTeamModeOn = teamMode;
        DataPersistence.Instance.SaveData();
        DataPersistence.Instance.SendGameSetting();
    }

    public void OnReturn()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void UpdatePlayerList(List<NetPartyPlayerInfo> playerInfos)
    {
        for (int player = 1; player <= 4; player++)
        {
            UpdatePlayerTextList(playerInfos, player);
        }
    }

    void UpdatePlayerTextList(List<NetPartyPlayerInfo> playerInfos, int player)
    {
        NetPartyPlayerInfo playerInfo = playerInfos.FirstOrDefault(x => x.player == player);
        TextMeshProUGUI playerText = GameObject.Find("Player" + player + "Text").GetComponent<TextMeshProUGUI>();
        if (playerInfo != null)
        {
            playerText.text = playerInfo.playerAlias;
            playerText.color = playerInfo.colorBody;
        }
        else
        {
            playerText.text = "(AI) Player " + player;
            playerText.color = Color.black;
        }
    }
}
