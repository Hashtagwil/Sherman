using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    public Toggle CarcassToggle;
    public Toggle TurnTurretLeftRightToggle;

    private void Start()
    {
        volumeSlider.value = DataPersistence.Instance.data.Volume;
        CarcassToggle.SetIsOnWithoutNotify(DataPersistence.Instance.data.LeaveCarcass);
        TurnTurretLeftRightToggle.SetIsOnWithoutNotify(DataPersistence.Instance.data.TurnTurretLeftRight);
    }

    public void OnVolumeChange()
    {
        DataPersistence.Instance.data.Volume = volumeSlider.value;
        AudioListener.volume = DataPersistence.Instance.data.Volume;
        DataPersistence.Instance.SaveData();
    }

    public void LoadTankColorChooser()
    {
        SceneManager.LoadScene("TankColorChoosing");
    }
    public void LoadTankModelChooser()
    {
        SceneManager.LoadScene("TankModelChoosing");
    }

    public void ChangeCarcassMode()
    {
        DataPersistence.Instance.data.LeaveCarcass = !DataPersistence.Instance.data.LeaveCarcass;
        DataPersistence.Instance.SaveData();
    }

    public void ChangeTurnMode()
    {
        DataPersistence.Instance.data.TurnTurretLeftRight = !DataPersistence.Instance.data.TurnTurretLeftRight;
        DataPersistence.Instance.SaveData();
    }
}