using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankModelUI : MonoBehaviour
{
    public TankController Tank;
    public TextMeshProUGUI HealthTxt;
    public TextMeshProUGUI ShotDistanceTxt;
    public TextMeshProUGUI ReloadTimeTxt;
    public TextMeshProUGUI SpeedTxt;

    // Start is called before the first frame update
    void Start()
    {
        HealthTxt.text += Tank.ModelLifeCount.ToString();
        
        string shotDistance = " LONG";
        if (Tank.MaxProjectileDistance != 0)
        {
            shotDistance = " SHORT";
        }
        ShotDistanceTxt.text += shotDistance;

        string reloadTime = " MEDIUM";
        if (Tank.ReloadTime < 1)
        {
            reloadTime = " FAST";
        }
        else if (Tank.ReloadTime > 1)
        {
            reloadTime = " LONG";
        }
        ReloadTimeTxt.text += reloadTime;

        string speed = " MEDIUM";
        if (Tank.speed > 300)
        {
            speed = " FAST";
        }
        else if (Tank.speed < 200)
        {
            speed = " SLOW";
        }
        SpeedTxt.text += speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
