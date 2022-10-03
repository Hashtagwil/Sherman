using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelect : MonoBehaviour
{
    [SerializeField] GameObject SelectedImage;
    [SerializeField] string GameModeScriptName;
    public bool TeamOnly;
    public Image EnabledImage;
    public Image DisabledImage;
    public Toggle TeamToggle;

    public void OnClickGameMode()
    {
        TeamToggle.interactable = !TeamOnly;
        DataPersistence.Instance.data.GameModeScriptName = GameModeScriptName;
        DataPersistence.Instance.SaveData();
        DataPersistence.Instance.SendGameSetting();
    }

    internal void SetEnabled(bool isEnabled)
    {
        GetComponentInChildren<Button>().interactable = isEnabled;
        EnabledImage?.gameObject.SetActive(isEnabled);
        DisabledImage?.gameObject.SetActive(!isEnabled);
        GetComponentInChildren<TextMeshProUGUI>().color = isEnabled ? Color.black : Color.gray;
    }

    private void Update()
    {

        if (DataPersistence.Instance.data.GameModeScriptName == GameModeScriptName)
        {
            TeamToggle.interactable = !TeamOnly;
            if(TeamOnly)
            {
                TeamToggle.SetIsOnWithoutNotify(true);
            }
            SelectedImage.SetActive(true);
        }
        else
        {
            SelectedImage.SetActive(false);
        }
    }
}
