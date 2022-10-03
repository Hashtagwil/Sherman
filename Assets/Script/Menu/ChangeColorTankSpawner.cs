using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeColorTankSpawner : MonoBehaviour
{
    public GameObject[] tanks;
    public TankController tankController; 

    // Start is called before the first frame update
    void Start()
    {
        //Create tank
        GameObject baseTank = GameObject.Find("BaseTank");
        GameObject currentTank = tanks[DataPersistence.Instance.data.TankIndex];
        Canvas sliderCanvas = currentTank.GetComponentInChildren<Canvas>();
        sliderCanvas.enabled = false;
        Instantiate(currentTank, baseTank.transform.position, baseTank.transform.rotation);
        TankController currentTankObj = FindObjectOfType<TankController>();//To be sure to get the object that actually contain the tankController; 
        currentTankObj.transform.localScale = baseTank.transform.localScale;
        TankController newTankController = currentTankObj.GetComponent<TankController>();
        tankController = newTankController;
        Destroy(baseTank);

        //Setup button
        ButtonColorSelection[] allButton = FindObjectsOfType<ButtonColorSelection>();
        foreach (ButtonColorSelection button in allButton)
        {
            button.TankController = newTankController;
        }

        tankController.SetBodyColor(DataPersistence.Instance.data.BodyColor);
        tankController.SetTurretColor(DataPersistence.Instance.data.TurretColor);
    }

    public void ChangeColorBody()
    {
        tankController.SelectBody();
    }

    public void ChangeColorTurret()
    {
        tankController.SelectTurret();
    }

    public void OnCancel()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnSave()
    {
        DataPersistence.Instance.data.BodyColor = tankController.GetBodyColor();
        DataPersistence.Instance.data.TurretColor = tankController.GetTurretColor();
        DataPersistence.Instance.SaveData();
        SceneManager.LoadScene("Menu");
    }
}
