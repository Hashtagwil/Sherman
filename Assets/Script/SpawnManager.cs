using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> tanks;
    [SerializeField] List<GameObject> spawns;
    public GameObject ScorePrefab;
    public DebugPlayerInfo DefaultDebugPlayerInfo;
    public Action OnPlayersSpawned;

    public NetGame netGame;
    NetParty netParty;

    private int controllerLocal = 1;

    public void RandomInstantiate()
    {
        netParty = FindObjectOfType<NetParty>();

        int numberPlayersMax = (int)DataPersistence.Instance.NumberPlayers;
        int player = 1;
        int[] playerNumbers = netParty.GetPlayerNumbers();
        List<int> uniqueNumber = new List<int>();;

        for (int i = 0; i < spawns.Count; i++)
        {
            int spawnIndex = i;

            if (numberPlayersMax > 0 && (DataPersistence.Instance.TypeClient == TypeClient.Local || playerNumbers.Contains(player)))
            {
                FirstInstantiatePlayer(spawnIndex, player, isHumanPlayer : true);
                numberPlayersMax--;
            }
            else
            {
                FirstInstantiatePlayer(spawnIndex, player, isHumanPlayer: false);
            }
            player++;

            uniqueNumber.Add(spawnIndex);
        }
        if (OnPlayersSpawned != null)
        {
            OnPlayersSpawned();
        }
    }

    void FirstInstantiatePlayer(int spawnIndex, int playerNumber, bool isHumanPlayer)
    {
        PlayerInfo playerInfo = new PlayerInfo(
            netPlayer: 0,
            player: playerNumber,
            score: 0,
            killCount: 0,
            spawnIndex: spawnIndex,
            isHumanPlayer: isHumanPlayer,
            Color.black,
            Color.black,
            flagsCaptured: 0
        );

        if (DataPersistence.Instance.TypeClient == TypeClient.Local)
            playerInfo.Controller = SetupController(playerNumber);

        SpawnLocation spawn = GetSpawnByIndex(spawnIndex);

        if (isHumanPlayer)
        {
            NetPartyPlayerInfo netPartyPlayerInfo = netParty.players.FirstOrDefault(x => x.player == playerNumber);

            if(netPartyPlayerInfo == null)
            {
                //LOCAL PLAYER
                playerInfo.TankIndex = Random.Range(0, tanks.Count);
                playerInfo.PlayerAlias = "Player " + playerNumber;
                playerInfo.ColorBody = spawn.AssociatedColor;
                playerInfo.ColorTurret = spawn.AssociatedColor;
            }
            else
            {
                //ONLINE PLAYER
                playerInfo.NetPlayer = netPartyPlayerInfo.playerId;
                playerInfo.PlayerAlias = netPartyPlayerInfo.playerAlias;
                playerInfo.ColorBody = netPartyPlayerInfo.colorBody;
                playerInfo.ColorTurret = netPartyPlayerInfo.colorTurret;
                playerInfo.TankIndex = netPartyPlayerInfo.tankIndex;
            }

            if (playerInfo.Controller == ControllerType.Controller)
                playerInfo.ControllerLocal = controllerLocal++;
        }
        else
        {
            //AI
            playerInfo.TankIndex = Random.Range(0, tanks.Count);
            playerInfo.PlayerAlias = "(AI) Player " + playerNumber;
            playerInfo.ColorBody = spawn.AssociatedColor;
            playerInfo.ColorTurret = spawn.AssociatedColor;
        }

        netGame.InstantiatePlayer(playerInfo, spawn.transform);
    }

    public ControllerType SetupController(int player)
    {
        if (player == 1)
            return DataPersistence.Instance.data.Controller1;
        else if (player == 2)
            return DataPersistence.Instance.data.Controller2;
        else if (player == 3)
            return DataPersistence.Instance.data.Controller3;
        else if (player == 4)
            return DataPersistence.Instance.data.Controller4;

        return ControllerType.Undefined;
    }

    public void RespawnPlayer(PlayerInfo playerInfo)
    {
        SpawnLocation spawn = GetSpawnByIndex(playerInfo.SpawnIndex);
        netGame.InstantiatePlayer(playerInfo, spawn.transform);
    }

    public SpawnLocation GetSpawnByIndex(int spawnIndex)
    {
        return spawns[spawnIndex].GetComponent<SpawnLocation>();
    }

    public GameObject GetTankPrefabByIndex (int tankIndex)
    {
        return tanks[tankIndex];
    }
}
