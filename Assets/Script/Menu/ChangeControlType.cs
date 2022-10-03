using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;


public class ChangeControlType : MonoBehaviour
{
    [SerializeField] TMP_Dropdown Player1; 
    [SerializeField] TMP_Dropdown Player2; 
    [SerializeField] TMP_Dropdown Player3; 
    [SerializeField] TMP_Dropdown Player4;

    public void Start()
    {
        LoadFromDataPersistence();
        RefreshUI();
    }

    private void RefreshUI()
    {
        Player1.RefreshShownValue();
        Player2.RefreshShownValue();
        Player3.RefreshShownValue();
        Player4.RefreshShownValue();
    }

    private void LoadFromDataPersistence()
    {
        Player1.value = (int)DataPersistence.Instance.data.Controller1;
        Player2.value = (int)DataPersistence.Instance.data.Controller2;
        Player3.value = (int)DataPersistence.Instance.data.Controller3;
        Player4.value = (int)DataPersistence.Instance.data.Controller4;
    }

    public void ChangeValue(TMP_Dropdown dropdown)
    {
        if(dropdown.Equals(Player1))
        {
            DataPersistence.Instance.ChangeControllerType((ControllerType)dropdown.value, 1);
        }
        else if(dropdown.Equals(Player2))
        {
            DataPersistence.Instance.ChangeControllerType((ControllerType)dropdown.value, 2);
        }
        else if (dropdown.Equals(Player3))
        {
            DataPersistence.Instance.ChangeControllerType((ControllerType)dropdown.value, 3);
        }
        else if (dropdown.Equals(Player4))
        {
            DataPersistence.Instance.ChangeControllerType((ControllerType)dropdown.value, 4);
        }
        LoadFromDataPersistence();
        RefreshUI();
        DataPersistence.Instance.SaveData();
    }

    public void DesactivateControlType()
    {
        Player1.transform.position = GameObject.Find("PlayerInputMultiplayer").transform.position;
        Player2.gameObject.SetActive(false);
        Player3.gameObject.SetActive(false);
        Player4.gameObject.SetActive(false);
    }
}
