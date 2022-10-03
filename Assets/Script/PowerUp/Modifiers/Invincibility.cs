using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : MonoBehaviour, IModifier
{
    private TankController tankController;
    private float _duration = 5.0f;
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
        stats.IsInvincible = true;
        stats.LerpTime = 0;
    }

    public void Deactivate(ModifiableStats stats)
    {
        stats.IsInvincible = false;
        stats.LerpTime = 0;
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
