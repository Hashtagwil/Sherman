using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseMenu : MonoBehaviour
{
    public TextMeshProUGUI WinLoseText;
    [SerializeField] GameObject ClientButton;
    [SerializeField] GameObject ServerButton;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.SceneInstance.OnGameWon += GameStatusChanged;
        GameManager.SceneInstance.OnGameLost += GameStatusChanged;
        if (DataPersistence.Instance.TypeClient == TypeClient.Client)
        {
            ClientButton.SetActive(true);
            ServerButton.SetActive(false);
        }
        else
        {
            ClientButton.SetActive(false);
            ServerButton.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    public void GameStatusChanged(string text, Color color)
    {
        gameObject.SetActive(true);
        WinLoseText.text = text;
        WinLoseText.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Restart()
    {
        FindObjectOfType<GameManager>().Restart();
    }

    public void GoToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu");
    }

    public void ReturnGameSetting()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameSetting", LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        GameManager.SceneInstance.OnGameWon -= GameStatusChanged;
        GameManager.SceneInstance.OnGameLost -= GameStatusChanged;
    }
}
