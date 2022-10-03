using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MenuBase
{
    public void Local()
    {
        DataPersistence.Instance.TypeClient = TypeClient.Local;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = "127.0.0.1";
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameSetting", LoadSceneMode.Single);
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #else
        Application.Quit(); // original code to quit Unity player
        #endif
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

}
