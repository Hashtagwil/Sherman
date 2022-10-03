using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TanksDialogManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnModelChange(int tankIndex)
    {
        DataPersistence.Instance.data.TankIndex = tankIndex;
        DataPersistence.Instance.SaveData();
        SceneManager.LoadScene("Menu");
    }
}
