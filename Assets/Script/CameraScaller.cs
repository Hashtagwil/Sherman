using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaller : MonoBehaviour
{
    private float lastWidth;
    private float lastHeight;

    void Update()
    {
        var width = Screen.width;
        var height = Screen.height;

        if (lastWidth != width) // if the user is changing the width
        {
            // update the height
            float heightAccordingToWidth = (float) (width / 16.0 * 9.0);
            Screen.SetResolution(width, Mathf.RoundToInt(heightAccordingToWidth), Screen.fullScreen, 0);
        }
        else if (lastHeight != height) // if the user is changing the height
        {
            // update the width
            var widthAccordingToHeight = (float) (height / 9.0 * 16.0);
            Screen.SetResolution(Mathf.RoundToInt(widthAccordingToHeight), height, Screen.fullScreen, 0);
        }

        lastWidth = width;
        lastHeight = height;
    }

}
