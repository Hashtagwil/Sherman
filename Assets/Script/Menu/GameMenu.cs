using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MenuBase
{
    [SerializeField] GameObject GameSettingButton;

    private void Start()
    {
        GameManager.SceneInstance.OnPause += Pausing;
        gameObject.SetActive(false);

        if (DataPersistence.Instance.TypeClient == TypeClient.Client)
        {
            GameSettingButton.SetActive(false);
        }
    }

    public void Pausing(bool isPaused)
    {
        Back();
        gameObject.SetActive(isPaused);
    }

    public void Resume()
    {
        GameManager.SceneInstance.PauseGame();
    }

    public void ReturnMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void ReturnGameSetting()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameSetting", LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        GameManager.SceneInstance.OnPause -= Pausing;    
    }
}
