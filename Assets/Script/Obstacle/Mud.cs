using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsMuddy = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsMuddy = false;
        }
    }

}
