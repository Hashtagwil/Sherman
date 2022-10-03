using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDefaultColor : MonoBehaviour
{
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        TankController controller = GetComponent<TankController>();
        controller.SetTurretColor(color);
        controller.SetBodyColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
