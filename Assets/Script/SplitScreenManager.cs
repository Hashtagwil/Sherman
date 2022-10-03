using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public static List<string> SUPPORTED_MAP = new List<string>() { "MediumMap" };

    public GameObject cameraPlayer1;
    public GameObject cameraPlayer2;
    public GameObject cameraPlayer3;
    public GameObject cameraPlayer4;
    public GameObject splitScreenCanvas2P;
    public GameObject splitScreenCanvas4P;


    void Start()
    {
        if (IsTwoPlayerSplitScreen())
            InitializeSplitScreenTwoPlayer();
        else if (IsFourPlayerSplitScreen())
            InitializeSplitScreenFourPlayer();
        else
        {
            // Camera needs to be always active to assign them. We disable those we dont need afterward
            cameraPlayer2.SetActive(false);
            cameraPlayer3.SetActive(false);
            cameraPlayer4.SetActive(false);
        }
    }

    public static bool IsTwoPlayerSplitScreen()
    {
        if (DataPersistence.Instance.TypeClient == TypeClient.Local
            && SUPPORTED_MAP.Contains(DataPersistence.Instance.data.Map))
            return (DataPersistence.Instance.NumberPlayers == NumberPlayers.Two) ? true : false;
        return false;
    }

    public static bool IsFourPlayerSplitScreen()
    {
        if (DataPersistence.Instance.TypeClient == TypeClient.Local
            && SUPPORTED_MAP.Contains(DataPersistence.Instance.data.Map))
            return (DataPersistence.Instance.NumberPlayers == NumberPlayers.Three
                || DataPersistence.Instance.NumberPlayers == NumberPlayers.Four) ? true : false;
        return false;
    }

    private void InitializeSplitScreenFourPlayer()
    {
        cameraPlayer1.GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
        cameraPlayer2.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        cameraPlayer3.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 0.5f);
        cameraPlayer4.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        
        splitScreenCanvas4P.SetActive(true);
    }

    private void InitializeSplitScreenTwoPlayer()
    {
        cameraPlayer1.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1.0f);
        cameraPlayer2.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1.0f);

        cameraPlayer3.SetActive(false);
        cameraPlayer4.SetActive(false);
        splitScreenCanvas2P.SetActive(true);
    }
}
