using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetGame : NetworkBehaviour
{
    [SerializeField] GameObject waterSplash;

    GameManager gameManager;
    SpawnManager spawnManager;
    RandomSpawnManager powerUpSpawner;
    RotateSelf[] rotateSelf;
    GameMap gameMap;

    public override void OnNetworkSpawn()
    {
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        gameManager.netGame = this;
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        spawnManager.netGame = this;
        powerUpSpawner = GameObject.Find("PowerUpSpawner")?.GetComponent<RandomSpawnManager>();
        powerUpSpawner.netGame = this;
        rotateSelf = FindObjectsOfType<RotateSelf>();
        gameMap = FindObjectOfType<GameMap>();
        gameMap.netGame = this;

        if (IsOwner && IsServer)
        {
            spawnManager.RandomInstantiate();
            powerUpSpawner.StartSpawnRandomPowerUps();
            gameMap.SpawnFlagsAndZones();
            StartSpinningClientRpc();
        }
    }

    public void InstantiatePlayer(PlayerInfo playerInfo, Transform transform)
    {
        GameObject avatar = Instantiate(Resources.Load("Net/NetPlayerAvatar") as GameObject, transform);
        avatar.GetComponent<NetPlayerAvatar>()._playerInfo = playerInfo;
        avatar.GetComponent<NetworkObject>().SpawnWithOwnership(playerInfo.NetPlayer, true);
    }

    [ClientRpc]
    public void SpawnPowerUpClientRpc(int index, Vector3 position)
    {
        powerUpSpawner.InstantiatePowerUp(index, position);
    }

    [ClientRpc]
    public void StartSpinningClientRpc()
    {
        foreach (var cross in rotateSelf)
        {
            cross.StartSpinning();
        };
    }

    [ClientRpc]
    public void PlayersSpawningCompletedClientRpc()
    {
        gameManager.PlayersSpawningCompleted();
    }

    [ClientRpc]
    public void SetTimerClientRpc(float timeRemaining)
    {
        gameManager.SetTimer(timeRemaining);
    }

    [ClientRpc]
    public void InstantiateZonesFlagClientRpc(Vector3 pos, Color color)
    {
         gameMap.InstantiateZonesFlag(pos, color);
    }

    [ServerRpc]
    public void InstantiateFlagsServerRpc(Vector3 pos, Color color)
    {
        GameObject netFlag = Instantiate(Resources.Load("Net/NetFlag") as GameObject, pos, Quaternion.identity);
        netFlag.GetComponent<NetFlag>()._color = color;
        netFlag.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyObjectServerRpc(string name)
    {
        DestroyObjectClientRpc(name);
    }

    [ClientRpc]
    public void DestroyObjectClientRpc(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    [ClientRpc]
    public void IncreaseKillCountClientRpc(int player)
    {
        PlayerInfo playerInfo = GetPlayerScore(player);
        if(playerInfo != null)
            playerInfo.KillCount++;
    }

    [ClientRpc]
    public void IncreaseFlagsCapturedClientRpc(int player)
    {
        PlayerInfo playerInfo = GetPlayerScore(player);
        if (playerInfo != null)
            playerInfo.FlagsCaptured++;
    }

    [ClientRpc]
    public void GameOverClientRpc(Color color)
    {
        gameManager.winningTeam = color;
        gameManager.DisplayWinner();
    }

    [ClientRpc]
    public void SinkClientRpc(Vector3 position, Quaternion rotation)
    {
        Instantiate(waterSplash, position, rotation);
    }

    private PlayerInfo GetPlayerScore(int player)
    {
        return FindObjectsOfType<Score>().FirstOrDefault(x => x.Player.Player == player)?.Player;
    }
}
