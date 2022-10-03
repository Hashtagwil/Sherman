using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IModifier
{
    private const float HEALTH_TO_GAIN = 1f;
    private TankController tankController;
    private float _duration = 1.0f;
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
        if (stats.Life < stats.MaxLife)
            tankController.GainLife(HEALTH_TO_GAIN);
    }

    public void Deactivate(ModifiableStats stats)
    {
        //Nothing to do
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
