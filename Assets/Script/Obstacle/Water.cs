using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsInWater = true;
            StartCoroutine(tank.StartSinking());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AI_Tank") || collision.gameObject.CompareTag("Player"))
        {
            TankController tank = collision.gameObject.GetComponent<TankController>();
            tank.ModifiedStats.IsInWater = false;
        }
    }
}
