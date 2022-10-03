using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static Score;
using static UnityEngine.GraphicsBuffer;

public class NetPlayerAvatar : NetworkBehaviour
{
    
    public PlayerInfo _playerInfo;
    
    const int PLAYER_NUMBER_ONE = 1;
    const float RESPAWN_INVINCIBILITY_TIMER = 4f;
    const float AIPathFindingGridSize = 0.5f;
    private const float GRIDSIZE_TO_TANK_SIZE_FACTOR = 1.75f;

    TankController tankController;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeClientRpc(_playerInfo);
        }
    }

    [ClientRpc]
    void InitializeClientRpc(PlayerInfo playerInfo)
    {
        if (_playerInfo == null)
        {
            _playerInfo = playerInfo;
        }
        Initialize();
    }

    private void Initialize()
    {
        GameObject tankPrefab = FindObjectOfType<SpawnManager>().GetTankPrefabByIndex(_playerInfo.TankIndex);
        Instantiate(tankPrefab, gameObject.transform);
        tankController = GetComponentInChildren<TankController>();
        GameObject tank = tankController.gameObject;

        if (IsServer & !_playerInfo.IsHumanPlayer)
        {
            //AI DU SERVER
            tank.AddComponent<PlayerController>();
            EnemyPathFindingController aiBrain = tank.AddComponent<EnemyPathFindingController>();
            // Currently hard-coded to RANDOM
            aiBrain.targetAquisitionAlgo = AI_Manager.Instance.targetAcquisitionAlgos[0];
            aiBrain.obstacles = LayerMask.GetMask("Default");
            aiBrain.gridSize = GRIDSIZE_TO_TANK_SIZE_FACTOR * AIPathFindingGridSize;
            tank.tag = "AI_Tank";
        }
        else if (IsOwner)
        {
            //SOI-MEME - Set la cameraController ici si pas Local - FindObjectOfType<CameraController>().AssignTarget(tankController);
            HumanPlayerController humanPlayerController = tank.AddComponent<HumanPlayerController>();
            if (DataPersistence.Instance.TypeClient == TypeClient.Local)
            {
                humanPlayerController.SetController(_playerInfo.Controller, _playerInfo.ControllerLocal);
            }
            else
            {
                _playerInfo.ControllerLocal = PLAYER_NUMBER_ONE;
                _playerInfo.Controller = DataPersistence.Instance.data.Controller1;
                humanPlayerController.SetController(_playerInfo.Controller, PLAYER_NUMBER_ONE);
            }

            // Assign camera to cameraController script - this script is used in bigger maps i.e MediumMap and above
            if(DataPersistence.Instance.TypeClient == TypeClient.Local)
                AssigneCameraToPlayer(_playerInfo.Player);
            else
                AssigneCameraToPlayer(PLAYER_NUMBER_ONE);

            tank.tag = "Player";
        }
        else
        {
            //AUTRES CLIENTS
            tank.AddComponent<PlayerController>();
            tank.tag = "Player";
        }

        PlayerController playerController = tank.GetComponent<PlayerController>();
        tankController.PlayerController = playerController;
        playerController.tankController = tankController;
        // To debug certain fields of PlayerInfo
        //if (tankController.PlayerController.DebugPlayerInfo == null)
        //{
        //    tankController.PlayerController.DebugPlayerInfo = DebugPlayerInfo.CreateDebugPlayerInfo(_playerInfo);
        //}
        if (tankController.PlayerController.DebugPlayerInfo != null)
            tankController.PlayerController.DebugPlayerInfo.name = _playerInfo.PlayerAlias;
        tankController.PlayerController.playerInfo = _playerInfo;

        Color teamColor = DataPersistence.Instance.GetPlayerTeamColor(_playerInfo.Player);
        if (DataPersistence.Instance.data.IsTeamModeOn)
        {
            tankController.SetBodyColor(teamColor);
            tankController.SetTurretColor(teamColor);
        }
        else
        {
            tankController.SetBodyColor(_playerInfo.ColorBody);
            tankController.SetTurretColor(_playerInfo.ColorTurret);
        }

        playerController.playerInfo.teamColor = teamColor;

        Score score = FindObjectsOfType<Score>().FirstOrDefault(x => x.Player != null && x.Player.Player == _playerInfo.Player);
        SpawnLocation spawn = FindObjectOfType<SpawnManager>().GetSpawnByIndex(_playerInfo.SpawnIndex);
        if (score == null)
            InstantiateScore(spawn.ScoreLocation);

        StartCoroutine(SpawnInvincibility());
    }

    private void AssigneCameraToPlayer(int player)
    {
        var camera = GameObject.Find("CameraPlayer" + player); //CameraPlayer1 2 3 4 used in mediumMap
        if (camera != null)
        {
            var cameraController = camera.GetComponent<CameraController>();
            if (cameraController != null)
                cameraController.AssignTarget(tankController);
        }
    }

    [ServerRpc]
    public void UpdatePositionServerRpc(Vector3 position, Quaternion rotation)
    {
        UpdatePositionClientRpc(position, rotation);
    }

    [ServerRpc]
    public void UpdateTurretServerRpc(float angle)
    {
        UpdateTurretClientRpc(angle);
    }

    [ServerRpc]
    public void RotateTurretFromControllerServerRpc(float aimHorizontalInput, float lastAngle)
    {
        RotateTurretFromControllerClientRpc(aimHorizontalInput, lastAngle);
    }

    [ClientRpc]
    public void RotateTurretFromControllerClientRpc(float aimHorizontalInput, float lastAngle)
    {
        float newLastAngle = tankController.RotateTurretFromController(aimHorizontalInput, lastAngle);
        HumanPlayerController humanPlayerController = tankController.PlayerController as HumanPlayerController;
        if (humanPlayerController != null)
            humanPlayerController.SetLastAngleTurret(newLastAngle);
    }



    [ClientRpc]
    public void ChangeColorClientRpc(Color body, Color turret)
    {
        if (tankController != null)
        {
            tankController.SetTurretColor(turret);
            tankController.SetBodyColor(body);
        }
    }

    Vector3 _posVel;
    float _rotVelX;
    float _rotVelY;
    float _rotVelZ;
    [ClientRpc]
    public void UpdatePositionClientRpc(Vector3 position, Quaternion rotation)
    {
        if (IsOwner) return;

        if (tankController != null)
        {
            tankController.transform.SetPositionAndRotation(position, rotation);
        }
    }

    [ClientRpc]
    public void UpdateTurretClientRpc(float angle)
    {
        if (tankController != null)
            tankController.RotateTurret(angle);
    }

    [ServerRpc]
    public void FireServerRpc(Vector3 pos, Quaternion rotation)
    {
        FireClientRpc(pos, rotation);
    }

    [ClientRpc]
    public void FireClientRpc(Vector3 pos, Quaternion rotation)
    {
        if (IsOwner) return;
        TakeShot(pos, rotation);
    }

    public void TakeShot(Vector3 pos, Quaternion rotation)
    {
        if (tankController != null)
            tankController.InstantiateBullet(pos, rotation);
    }

    public void InstantiateScore(ScorePosition scorePosition)
    {
        if (tankController != null)
            tankController.InstantiateScore(scorePosition);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeHitServerRpc(int playerSource, bool applyToOwner)
    {
        TakeHitClientRpc(playerSource, applyToOwner);
    }

    [ClientRpc]
    public void TakeHitClientRpc(int playerSource, bool applyToOwner)
    {
        PlayerController player = FindObjectsOfType<PlayerController>().FirstOrDefault(x => x.playerInfo.Player == playerSource);
        if (player != null && tankController != null && (!IsOwner || applyToOwner))
        {
            tankController.TakeHit(player);
        }
    }

    private IEnumerator SpawnInvincibility()
    {
        yield return new WaitForEndOfFrame();
        if (tankController != null)
            tankController.StartSpawnInvincibility(RESPAWN_INVINCIBILITY_TIMER);
    }

    [ServerRpc]
    public void GenerateNewRandomDirectionServerRpc()
    {
        var newRandomArray = new float[tankController.RandomDirection.Length];
        for(var i = 0; i < tankController.RandomDirection.Length; i++)
        {
            var value = UnityEngine.Random.Range(0.0f, 360.0f);
            newRandomArray[i] = value;
        }
        SetNewRandomDirectionClientRpc(newRandomArray);
    }

    [ClientRpc]
    public void SetNewRandomDirectionClientRpc(float[] newRandomArray)
    {
        tankController.RandomDirection = newRandomArray;
    }
}
