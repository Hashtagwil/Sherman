using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NumberPlayersSelect : MonoBehaviour
{
    [SerializeField] Toggle toggle1;
    [SerializeField] Toggle toggle2;
    [SerializeField] Toggle toggle3;
    [SerializeField] Toggle toggle4;

    private void Start()
    {
        SeptupToggle(toggle1, NumberPlayers.One);
        SeptupToggle(toggle2, NumberPlayers.Two);
        SeptupToggle(toggle3, NumberPlayers.Three);
        SeptupToggle(toggle4, NumberPlayers.Four);
    }

    void SeptupToggle(Toggle toggle, NumberPlayers numberPlayers)
    {
        toggle.isOn = DataPersistence.Instance.NumberPlayers == numberPlayers;
        toggle.onValueChanged.AddListener((bool check) => OnChangeNumberPlayers(check, numberPlayers));
    }

    public void OnChangeNumberPlayers(bool check, NumberPlayers numberPlayers)
    {
        if(check)
            DataPersistence.Instance.NumberPlayers = numberPlayers;
    }

    public void DesactivateAll()
    {
        toggle1.gameObject.SetActive(false);
        toggle2.gameObject.SetActive(false);
        toggle3.gameObject.SetActive(false);
        toggle4.gameObject.SetActive(false);
    }
}
