using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerType { KeyBoard = 0, Controller = 1, Undefined};

///Need to stay only string from existing maps to work with RandomizeGameSetting.cs;
public static class Maps
{
    public const string SmallMap = "SmallMap";
    public const string MediumMap = "MediumMap";
};

///Need to stay only string from existing gamemode to work with RandomizeGameSetting.cs;
public static class GameModes
{
    public const string LastStanding = "LastStandingMode";
    public const string NbKill = "NbKillMode";
    public const string CaptureTheFlag = "CaptureTheFlag";
    public const string OneMin = "OneMin";
}
public enum NumberPlayers { One = 1, Two = 2, Three = 3, Four = 4 };
public enum TypeClient { Client, Server, Local };

public class Enums : MonoBehaviour
{
}
