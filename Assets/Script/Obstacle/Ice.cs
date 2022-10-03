using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    private const float DEFAULT_DRAG = 10f;
    private const float DEFAULT_ANGULAR_DRAG = 10f;
    private const float ICE_DRAG = 1f;
    private const float ICE_ANGULAR_DRAG = 2f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsOnIce = true;
            tank.ModifiedStats.Drag = ICE_DRAG;
            tank.ModifiedStats.AngularDrag = ICE_ANGULAR_DRAG;
            tank.UpdateDrag();

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsOnIce = false;
            tank.ModifiedStats.Drag = DEFAULT_DRAG;
            tank.ModifiedStats.AngularDrag = DEFAULT_ANGULAR_DRAG;
            tank.UpdateDrag();
        }
    }
}
