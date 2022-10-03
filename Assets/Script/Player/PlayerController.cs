using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public DebugPlayerInfo DebugPlayerInfo;

    //Components
    public TankController tankController;

    public PlayerController()
    {
        playerInfo = new PlayerInfo();
    }

    private void Awake()
    {
        tankController = GetComponent<TankController>();
    }

    void Start()
    {

    }

    private void Update()
    {
    }

    void FixedUpdate()
    {
    }

    public Color GetColor()
    {
        return tankController.GetBodyColor();
    }
}
