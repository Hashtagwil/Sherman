using Assets.Script.GameMode;
using Assets.Script.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int AI_MAX_PLAYER_ID = 0;

    private float maxTime = 0;
    private float timeRemaining = 0;

    public bool isPaused = false;
    public bool isGameOver = false;
    public bool isGameStarted = false;

    private PlayerController winnerPlayerController;
    public Color winningTeam;
    public IGameMode[] GameModes;
    public IGameMode CurrentGameMode;
    private SpawnManager spawnManager;

    [SerializeField] TextMeshProUGUI Timer;

    public static GameManager SceneInstance;

    public Action<string, Color> OnGameWon;
    public Action<string, Color> OnGameLost;
    public Action<bool> OnPause;

    public NetGame netGame;

    private void Awake()
    {
        Time.timeScale = 1;

        if (SceneInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        SceneInstance = this;
        SetupNewGame();
        RegisterEvents();
    }

    private void Start()
    {
        AudioListener.volume = DataPersistence.Instance.data.Volume;
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    public void RespawnPlayer(PlayerInfo playerInfo)
    {
        StartCoroutine(RespawnRoutine(playerInfo));
    }

    private IEnumerator RespawnRoutine(PlayerInfo playerInfo)
    {
        yield return new WaitForSeconds(CurrentGameMode.GetRespawnDelay());
        spawnManager.RespawnPlayer(playerInfo);
    }

    private void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            if (DataPersistence.Instance.TypeClient != TypeClient.Client && CurrentGameMode is ITimedGameMode)
            {
                timeRemaining -= Time.deltaTime;
                netGame.SetTimerClientRpc(timeRemaining);
            }

            if (Input.GetButtonDown("Cancel"))
                PauseGame();

            if (DataPersistence.Instance.TypeClient != TypeClient.Client && CurrentGameMode.isGameOver())
            {
                netGame.GameOverClientRpc(CurrentGameMode.getWinner());
            }
        }
    }

    public void SetTimer(float timeRemaining)
    {
        ITimedGameMode gamemode = (ITimedGameMode)CurrentGameMode;
        gamemode.currentTime = timeRemaining;
        Timer.SetText(UnityUtils.GetTimeFromTimer(timeRemaining));
    }

    public void DisplayWinner()
    {
        if (OnGameWon != null)
        {
            if (DataPersistence.Instance.data.IsTeamModeOn)
            {
                OnGameWon("Team " + DataPersistence.Instance.GetTeamColorAsString(winningTeam) + " won", winningTeam);
            }
            else
            {
                GameObject[] players = PlayerUtils.GetAllPlayer();
                int winnerId = DataPersistence.Instance.GetPlayerId(winningTeam);
                GameObject winner = players.SingleOrDefault(player => player.GetComponent<PlayerController>().playerInfo.Player == winnerId);
                Color txtColor = winner.GetComponent<PlayerController>().tankController.GetTurretColor();
                OnGameWon("Player " + DataPersistence.Instance.GetPlayerIdAsString(winningTeam) + " won", txtColor);
            }
        }
        GameOver();
    }

    void GameOver()
    {
        isGameStarted = false;
        StartCoroutine(WaitForEndOfFrameThenGamOver());
    }

    private IEnumerator WaitForEndOfFrameThenGamOver()
    {
        yield return new WaitForEndOfFrame();
        isGameOver = true;
        Time.timeScale = 0;
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        if (DataPersistence.Instance.TypeClient == TypeClient.Local)
            Time.timeScale = isPaused ? 0 : 1;
        if (OnPause != null)
        {
            OnPause(isPaused);
        }
    }

    public void Restart()
    {
        SetupNewGame();
        NetworkManager.Singleton.SceneManager.LoadScene(DataPersistence.Instance.data.Map, LoadSceneMode.Single);
    }
    void RegisterEvents()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.OnPlayersSpawned += OnPlayersSpawned;
    }

    private void OnPlayersSpawned()
    {
        netGame.PlayersSpawningCompletedClientRpc();
    }

    public void PlayersSpawningCompleted()
    {
        isGameStarted = true;
    }

    public void SetupNewGame()
    {
        Time.timeScale = 1;
        isGameOver = false;
        isPaused = false;
        try
        {
            string gameModeScriptName = DataPersistence.Instance.data.GameModeScriptName;
            CurrentGameMode = (IGameMode)gameObject.AddComponent(Type.GetType(gameModeScriptName));
            if (CurrentGameMode == null)
            {
                Debug.Log("Invalid gamemode script name.");
                // We stop the game to make it obvious 
                Time.timeScale = 0;
            }
            if (CurrentGameMode is ITimedGameMode)
            {
                maxTime = CurrentGameMode.GetMaxTime();
                timeRemaining = maxTime;
            }
            else
            {
                Timer.SetText("");
            }
        }
        catch (Exception)
        {
            Debug.Log("Exception generating the gamemode");
        }
    }
}
