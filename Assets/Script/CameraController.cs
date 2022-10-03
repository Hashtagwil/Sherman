using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject topBoundary;
    public GameObject bottomBoundary;
    public GameObject leftBoundary;
    public GameObject rightBoundary;

    [SerializeField] TankController player;
    private float yMax;
    private float yMin;
    private float xMax;
    private float xMin;
    private float wallOffset = 0.25f;
    private Camera _camera;

    private void Start()
    {
        _camera = this.GetComponent<Camera>();
        SetCameraXYLimit();
    }

    void Update()
    {
        if (player != null)
        {
            var x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
            var y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
            // For some reason I cannot set transform.position.x and ..y, I have to new Vector it
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }


    public void AssignTarget(TankController player)
    {
        this.player = player;
    }
    

    /// <summary>
    /// Set the camera X and Y min max values based on map boundaries and camera width / height
    /// </summary>
    void SetCameraXYLimit()
    {
        float cameraHalfHeight = _camera.orthographicSize;
        float cameraHalfWidth = GetCameraHalfWidth(cameraHalfHeight);

        yMax = (topBoundary.transform.position.y + wallOffset) - cameraHalfHeight;
        yMin = (bottomBoundary.transform.position.y - wallOffset) + cameraHalfHeight;

        xMax = (rightBoundary.transform.position.x + wallOffset) - cameraHalfWidth;
        xMin = (leftBoundary.transform.position.x - wallOffset) + cameraHalfWidth;
    }

    private float GetCameraHalfWidth(float cameraHalfHeight)
    {
        if (SplitScreenManager.IsTwoPlayerSplitScreen())
        {
            return _camera.aspect * cameraHalfHeight / 2;
        }
        else
        {
            return _camera.aspect * cameraHalfHeight; 
        }
    }

    
}
