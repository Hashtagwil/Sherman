using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetParty : NetworkBehaviour
{
    const int PLAYER_NUMBER_LIMITS = 4;

    public List<NetPartyPlayerInfo> players;

    int countLoaded = 0;

    #region Initialize

    private void Awake()
    {
        players = new List<NetPartyPlayerInfo>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsServer)
            RegisterEventsServer();
    }

    private void RegisterEventsServer()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    #endregion

    #region Join Players

    private bool CanJoin()
    {
        return players.Count < PLAYER_NUMBER_LIMITS && (DataPersistence.Instance.TypeClient == TypeClient.Server || players.Count == 0);
    }

    public void Join(ulong clientId, NetPartyPlayerInfo info)
    {
        if (CanJoin())
        {
            SendPlayerListNewPlayer(clientId);
            info.player = GetAvailablePlayerNumber();
            info.playerAlias = "Player " + info.player;
            AddClientRpc(info);
            Debug.Log("New player : " + info.playerId + " Player : " + info.player + " PlayerAlias : " + info.playerAlias);
        }
        else
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log("Client disconnected " + clientId);

        int index = players.FindIndex(x => x.playerId == clientId);
        if (index >= 0)
            RemoveClientRpc(index);
    }

    #endregion

    #region Update players list

    void SendPlayerListNewPlayer(ulong clientId)
    {
        foreach (NetPartyPlayerInfo player in players)
            AddClientRpc(player, NetCodeHelper.SingleTarget(clientId));
    }

    [ClientRpc]
    void AddClientRpc(NetPartyPlayerInfo info, ClientRpcParams rpcParams = default)
    {
        players.Add(info);
        PlayersUpdated();
    }

    [ClientRpc]
    void RemoveClientRpc(int index)
    {
        players.RemoveAt(index);
        PlayersUpdated();
    }

    [ClientRpc]
    public void EditClientRpc(NetPartyPlayerInfo info)
    {
        players[players.FindIndex(x => x.playerId == info.playerId)] = info;
        PlayersUpdated();
    }

    void PlayersUpdated()
    {
        FindObjectOfType<Multiplayer>()?.UpdatePlayerList(players);
        FindObjectOfType<GameSettings>()?.UpdatePlayerList(players);
    }

    #endregion

    #region Load Scene Event

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName.Contains("Map"))
        {
            LoadNetGameServerRpc(clientId);
        }
        else
        {
            Debug.Log("Not a map scene ");
        }
    }

    [ServerRpc]
    public void LoadNetGameServerRpc(ulong clientId)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (!gameManager.isGameStarted)
        {
            countLoaded++;

            if (DataPersistence.Instance.TypeClient != TypeClient.Local)
                DataPersistence.Instance.NumberPlayers = (NumberPlayers)players.Count;

            if (countLoaded >= players.Count)
            {
                NetworkObject netGame = Instantiate(Resources.Load("Net/NetGame") as GameObject).GetComponent<NetworkObject>();
                netGame.Spawn(destroyWithScene: true);
                countLoaded = 0;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetupGameServerRpc(
        string gameModeScript,
        string map,
        Color colorPlayer1,
        Color colorPlayer2,
        Color colorPlayer3,
        Color colorPlayer4,
        bool IsTeamModeOn,
        bool LeaveCarcass
        )
    {
        SetupGameClientRpc(
            gameModeScript,
            map,
            colorPlayer1,
            colorPlayer2,
            colorPlayer3,
            colorPlayer4,
            IsTeamModeOn,
            LeaveCarcass
            );
    }

    [ClientRpc]
    public void SetupGameClientRpc( 
        string gameModeScript,
        string map, 
        Color colorPlayer1, 
        Color colorPlayer2, 
        Color colorPlayer3, 
        Color colorPlayer4, 
        bool IsTeamModeOn,
        bool LeaveCarcass,
        ClientRpcParams singleTarget = default
        )
    {
        DataPersistence.Instance.data.GameModeScriptName = gameModeScript;
        DataPersistence.Instance.data.Map = map;
        DataPersistence.Instance.data.Player1Color = colorPlayer1;
        DataPersistence.Instance.data.Player2Color = colorPlayer2;
        DataPersistence.Instance.data.Player3Color = colorPlayer3;
        DataPersistence.Instance.data.Player4Color = colorPlayer4;
        DataPersistence.Instance.data.IsTeamModeOn = IsTeamModeOn;
        DataPersistence.Instance.data.LeaveCarcass = LeaveCarcass;
        DataPersistence.Instance.SaveData();
        
    }

    #endregion

    #region Computed function for Players list

    public int[] GetPlayerNumbers()
    {
        return players.Select(x => x.player).ToArray();
    }

    public int GetAvailablePlayerNumber()
    {
        List<int> availablePlayerNumbers = Enumerable.Range(1, PLAYER_NUMBER_LIMITS).ToList();
        availablePlayerNumbers.RemoveAll(x => players.Select(x => x.player).Any(y => x == y));
        return availablePlayerNumbers.Min();
    }

    #endregion

    public override void OnDestroy()
    {
        DataPersistence.Instance.TypeClient = TypeClient.Local;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        base.OnDestroy();
    }
}
