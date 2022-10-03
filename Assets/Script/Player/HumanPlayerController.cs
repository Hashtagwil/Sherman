using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayerController : PlayerController
{
    private float horizontalInput;
    private float forwardInput;

    private float aimVerticalInput;
    private float aimHorizontalInput;

    private string HorizontalAxisName;
    private string VerticalAxisName;
    private string FireAxisName;
    private string AimHorizontalAxisName;
    private string AimVerticalAxisName;

    //Components
    private GameManager gameManager;
    private NetPlayerAvatar netPlayerAvatar;

    private float lastAngleTurret = float.NaN;

    public HumanPlayerController()
    {
        playerInfo = new PlayerInfo();
    }

    void Start()
    {
        netPlayerAvatar = gameObject.GetComponentInParent<NetPlayerAvatar>();
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetAxis(FireAxisName) > 0 && !tankController.Fired && (gameManager == null || !(gameManager.isGameOver || gameManager.isPaused)))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        //Movement
        horizontalInput = Input.GetAxis(HorizontalAxisName);
        forwardInput = Input.GetAxis(VerticalAxisName);

        tankController.Move(horizontalInput, forwardInput);

        //Aim
        if (playerInfo.Controller == ControllerType.Controller)
        {
            aimVerticalInput = Input.GetAxis(AimVerticalAxisName);
            aimHorizontalInput = Input.GetAxis(AimHorizontalAxisName);
        }

        if (DataPersistence.Instance.data.TurnTurretLeftRight && playerInfo.Controller == ControllerType.Controller && (aimVerticalInput != 0 || aimHorizontalInput != 0))
        {
            // Controller left-right only are used to turn the turret insteand of aiming with thumb stick.
            // Just comment the whole "else if" block to return to previous behavior.
            if (float.IsNaN(lastAngleTurret))
            {
                lastAngleTurret = transform.parent.eulerAngles.z;
            }
            netPlayerAvatar.RotateTurretFromControllerServerRpc(aimHorizontalInput, lastAngleTurret);
        }
        else
        {
            float turretAngle = AnglePlayerTurret();
            lastAngleTurret += aimHorizontalInput * 10f;

            if (!float.IsNaN(lastAngleTurret)) netPlayerAvatar.UpdateTurretServerRpc(turretAngle);
        }

        netPlayerAvatar.UpdatePositionServerRpc(transform.position, transform.rotation);
    }

    public void SetLastAngleTurret(float newAngle)
    {
        lastAngleTurret = newAngle;
    }

    /// <summary>
    /// Set the control to use for this player
    /// We get the controller count to know what controller to check
    /// Controller start with 1 for the name, so the first controller is Horizontal1; 
    /// </summary>
    public void SetController(ControllerType controller, int controllerCount)
    {
        //In the input manager, 1 is keyboard
        if (controller == ControllerType.KeyBoard)
        {
            HorizontalAxisName = "Horizontal";
            VerticalAxisName = "Vertical";
            FireAxisName = "Fire";
        }
        else if (controller == ControllerType.Controller)
        {
            HorizontalAxisName = "Horizontal" + (controllerCount).ToString();
            VerticalAxisName = "Vertical" + (controllerCount).ToString();
            FireAxisName = "Fire" + (controllerCount).ToString();
            AimVerticalAxisName = "AimVertical" + (controllerCount).ToString();
            AimHorizontalAxisName = "AimHorizontal" + (controllerCount).ToString();
        }
    }

    private float AnglePlayerTurret()
    {
        if (playerInfo.Controller == ControllerType.KeyBoard)
        {
            Vector2 mousePos = Input.mousePosition;
            Camera camera = GameObject.Find("CameraPlayer" + playerInfo.Player)?.GetComponent<Camera>() ?? Camera.main;
            Vector2 objectPos = camera.WorldToScreenPoint(transform.position);

            mousePos.x -= objectPos.x;
            mousePos.y -= objectPos.y;

            lastAngleTurret = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        }
        else if (playerInfo.Controller == ControllerType.Controller && (aimVerticalInput != 0 || aimHorizontalInput != 0))
        {
            lastAngleTurret = Mathf.Atan2(aimVerticalInput, aimHorizontalInput) * Mathf.Rad2Deg;
        }

        return lastAngleTurret;
    }

    private void Shoot()
    {
        tankController.Fire();
    }
}
