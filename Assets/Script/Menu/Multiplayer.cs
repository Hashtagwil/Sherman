using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Multiplayer : MonoBehaviour
{
    public string hostIpAddress = "127.0.0.1";

    GameObject hostClient;
    GameObject playButton;
    GameObject listPlayers;

    UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        hostClient = GameObject.Find("HostClient");
        playButton = GameObject.Find("PlayButton");
        listPlayers = GameObject.Find("ListPlayers");

        playButton.SetActive(false);
        listPlayers.SetActive(false);
        hostIpAddress = DataPersistence.Instance.data.HostIpAddress;
        GameObject.Find("IpAddressInput").GetComponent<TMP_InputField>().text = hostIpAddress;
    }

    #region Buttons events

    public void OnHostButton()
    {
        DataPersistence.Instance.TypeClient = TypeClient.Server;
        BeforeStartNetwork();
        NetworkManager.Singleton.StartHost();
    }

    public void OnClientButton()
    {
        DataPersistence.Instance.TypeClient = TypeClient.Client;
        BeforeStartNetwork();
        NetworkManager.Singleton.StartClient();
        StartCoroutine(TryConnectingText());
    }

    private void BeforeStartNetwork()
    {
        ShowPlayerList();
        DataPersistence.Instance.data.HostIpAddress = hostIpAddress;
        DataPersistence.Instance.SaveData();
        transport.ConnectionData.Address = hostIpAddress;
    }

    public void OnLaunch()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameSetting", LoadSceneMode.Single);
    }

    public void OnReturn()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void OnIpAddressChange(string newAddress)
    {
        hostIpAddress = newAddress;
    }

    #endregion

    #region Players list

    void ShowPlayerList()
    {
        hostClient.SetActive(false);
        listPlayers.SetActive(true);
    }

    public void UpdatePlayerList(List<NetPartyPlayerInfo> playerInfos)
    {
        for (int player = 1; player <= 4; player++)
        {
            UpdatePlayerTextList(playerInfos, player);
        }

        HostCanLaunch(CanLaunch(playerInfos));
    }

    private bool CanLaunch(List<NetPartyPlayerInfo> playerInfos)
    {
        return playerInfos.Count() >= 2 && NetworkManager.Singleton.IsServer;
    }

    private void HostCanLaunch(bool canLaunch)
    {
        playButton.SetActive(canLaunch);
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
            playerText.text = "";
        }
    }

    IEnumerator TryConnectingText()
    {
        TextMeshProUGUI playerText = GameObject.Find("Player" + 3 + "Text").GetComponent<TextMeshProUGUI>();
        string tryConnectingMessage = "Try connecting to " + hostIpAddress;
        playerText.color = Color.black;
        string dots = "";

        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            playerText.text = tryConnectingMessage + " " + dots;
            dots += ".";
            if (dots.Length > 3) dots = "";
            yield return new WaitForSeconds(1);
        }
    }

    #endregion
}
