using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBalls : MonoBehaviour, IModifier
{
    private TankController tankController;
    private float _duration = 10.0f;
    public int NbBouncesAllowed = 3;
    private ModifiableStats statsCopy;

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
        stats.BulletCanBounce = true;
        stats.NbBounces = NbBouncesAllowed;
        statsCopy = stats;
    }

    public void Deactivate(ModifiableStats stats)
    {
        stats.BulletCanBounce = false;
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
