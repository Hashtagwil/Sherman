using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSpeedDouble : MonoBehaviour, IModifier
{
    private TankController tankController;
    private float _duration = 10.0f;
    public float Duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }

    public void Activate(ModifiableStats stats)
    {
        stats.TurnSpeedMultiplier = 2.0f;
    }

    public void Deactivate(ModifiableStats stats)
    {
        stats.TurnSpeedMultiplier = 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("AI_Tank"))
        {
            tankController = collision.gameObject.GetComponent<TankController>();
            tankController.AddModifer(this);
        }
    }
}
