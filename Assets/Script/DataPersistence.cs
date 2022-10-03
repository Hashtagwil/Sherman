using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistence : MonoBehaviour
{
    public enum Menus
    {
        MAIN, OPTIONS, TUTORIAL, MULTIPLAYER
    }

    public static DataPersistence Instance;

    public Data data;
    public Menus CurrentMenu = Menus.MAIN;
    public NumberPlayers NumberPlayers = NumberPlayers.One;
    public TypeClient TypeClient = TypeClient.Local;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (FindObjectOfType<NetworkManager>() == null)
            Instantiate(Resources.Load("Net/NetworkManager"));
    }

    [Serializable]
    public class Data
    {
        public float Volume = 1;
        public int TankIndex = 0;
        public ControllerType Controller1 = ControllerType.KeyBoard;
        public ControllerType Controller2 = ControllerType.Controller;
        public ControllerType Controller3 = ControllerType.Controller;
        public ControllerType Controller4 = ControllerType.Controller;
        public Color BodyColor = Color.green;
        public Color TurretColor = Color.green;
        public string Map = Maps.SmallMap;
        public string GameModeScriptName; 
        public Color Player1Color = Color.red;
        public Color Player2Color = Color.blue;
        public Color Player3Color = Color.green;
        public Color Player4Color = Color.yellow;
        public bool IsTeamModeOn = false;
        public bool LeaveCarcass = false;
        public string HostIpAddress = "127.0.0.1";
        public bool TurnTurretLeftRight = true;

        public int CLUSTER_BOMB_PROJECTILE_COUNT = 6;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void SendGameSetting()
    {
        FindObjectOfType<NetParty>().SetupGameServerRpc(
            Instance.data.GameModeScriptName,
            Instance.data.Map,
            Instance.data.Player1Color,
            Instance.data.Player2Color,
            Instance.data.Player3Color,
            Instance.data.Player4Color,
            Instance.data.IsTeamModeOn,
            Instance.data.LeaveCarcass
            );
    }

    public Data LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<Data>(json);
        }

        return data;
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            GameObject.FindObjectOfType<MainMenu>().RestoreCurrentMenu();
        }
    }

    public void ChangeControllerType(ControllerType type, int player)
    {
        if(type == ControllerType.KeyBoard)
        {
            //Set all to controller then set the right one
            data.Controller1 = ControllerType.Controller;
            data.Controller2 = ControllerType.Controller;
            data.Controller3 = ControllerType.Controller;
            data.Controller4 = ControllerType.Controller;
        }
        switch(player)
        {
            case 1:
                data.Controller1 = type;
                break;
            case 2:
                data.Controller2 = type;
                break;
            case 3:
                data.Controller3 = type;
                break;
            case 4:
                data.Controller4 = type;
                break;
            default:
                Debug.Log("Unkown Player Number");
                break;
        }
    }

    internal Color GetPlayerTeamColor(int playerId)
    {
        Color color = Color.white;
        if (playerId == 1)
        {
            if (data.IsTeamModeOn) color = Instance.data.Player1Color;
            else color = Color.red;
        }
        if (playerId == 2 || playerId == -3)
        {
            if (data.IsTeamModeOn) color = Instance.data.Player2Color;
            else color = Color.blue;
        }
        else if (playerId == 3 || playerId == -2)
        {
            if (data.IsTeamModeOn) color = Instance.data.Player3Color;
            else color = Color.green;
        }
        else if (playerId == 4 || playerId == -1)
        {
            if (data.IsTeamModeOn) color = Instance.data.Player4Color;
            else color = Color.yellow;
        }
        return color;
    }

    internal void SetPlayerColor(int playerId, Color color)
    {
        if (playerId == 1)
        {
            Instance.data.Player1Color = color;
        }
        if (playerId == 2)
        {
            Instance.data.Player2Color = color;
        }
        else if (playerId == 3)
        {
            Instance.data.Player3Color = color;
        }
        else if (playerId == 4)
        {
            Instance.data.Player4Color = color;
        }
        SaveData();
    }

    internal string GetTeamColorAsString(Color winningTeam)
    {
        string team = "";
        if (winningTeam == Color.red)
        {
            team = "RED";
        }
        else if (winningTeam == Color.blue)
        {
            team = "BLUE";
        }
        else if (winningTeam == Color.green)
        {
            team = "GREEN";
        }
        else if (winningTeam == Color.yellow)
        {
            team = "YELLOW";
        }
        return team;
    }

    internal string GetPlayerIdAsString(Color winningTeam)
    {
        return GetPlayerId(winningTeam).ToString();
    }

    internal int GetPlayerId(Color winningTeam)
    {
        int player = 0;
        if (winningTeam == Color.red)
        {
            player = 1;
        }
        else if (winningTeam == Color.blue)
        {
            player = 2;
        }
        else if (winningTeam == Color.green)
        {
            player = 3;
        }
        else if (winningTeam == Color.yellow)
        {
            player = 4;
        }
        return player;
    }
}
