using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : INetworkSerializable
{
    public ulong NetPlayer;
    public int Player;
    public string PlayerAlias;
    public bool IsHumanPlayer;
    public int SpawnIndex;
    public int TankIndex;
    public Color ColorBody;
    public Color ColorTurret;
    public int Score;
    public int KillCount;
    public int ControllerLocal;
    public ControllerType Controller;
    public int FlagsCaptured;
    public SpawnLocation spawnLocation;
    public bool isHumanPlayer;
    public Color teamColor;

    public PlayerInfo(
        ulong netPlayer,
        int player,
        int score,
        int killCount,
        int spawnIndex,
        bool isHumanPlayer,
        Color colorBody,
        Color colorTurret,
        int tankIndex = 0,
        string playerAlias = "",
        int controllerLocal = 0,
        int flagsCaptured = 0,
        ControllerType controller = ControllerType.Undefined
        ) : this()
    { 
        NetPlayer = netPlayer;
        Player = player;
        Score = score;
        KillCount = killCount;
        SpawnIndex = spawnIndex;
        IsHumanPlayer = isHumanPlayer;
        TankIndex = tankIndex;
        PlayerAlias = playerAlias;
        ColorBody = colorBody;
        ColorTurret = colorTurret;
        ControllerLocal = controllerLocal;
        Controller = controller;
        FlagsCaptured = flagsCaptured;
        spawnLocation = null;
    }

    public PlayerInfo()
    {
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetPlayer);
        serializer.SerializeValue(ref Player);
        serializer.SerializeValue(ref Score);
        serializer.SerializeValue(ref KillCount);
        serializer.SerializeValue(ref SpawnIndex);
        serializer.SerializeValue(ref IsHumanPlayer);
        serializer.SerializeValue(ref TankIndex);
        serializer.SerializeValue(ref PlayerAlias);
        serializer.SerializeValue(ref ColorBody);
        serializer.SerializeValue(ref ColorTurret);
        serializer.SerializeValue(ref Controller);
        serializer.SerializeValue(ref ControllerLocal);
        serializer.SerializeValue(ref FlagsCaptured);
        serializer.SerializeValue(ref teamColor);
    }
}
