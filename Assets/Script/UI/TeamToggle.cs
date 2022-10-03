using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TeamToggle : MonoBehaviour
{
    int index = 0;
    private Color[] colors = { Color.red, Color.blue };
    private Color CurrentColor;
    public int PlayerId;

    // Update is called once per frame
    void Update()
    {
        CurrentColor = DataPersistence.Instance.GetPlayerTeamColor(PlayerId);
        if (CurrentColor == Color.red)
        {
            index = 0;
        }
        else if (CurrentColor == Color.blue)
        {
            index = 1;
        }
        GetComponent<Image>().color = CurrentColor;        
    }

    public void OnColorChange()
    {
        index++;
        if (index == colors.Length)
        {
            index = 0;
        }
        DataPersistence.Instance.SetPlayerColor(PlayerId, colors[index]);
        DataPersistence.Instance.SendGameSetting();
    }
}
