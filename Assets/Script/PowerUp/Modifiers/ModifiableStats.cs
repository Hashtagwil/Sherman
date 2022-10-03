using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiableStats
{
    private float _speedMultiplier;
    private float _maxLife;


    public bool IsInvincible;
    public bool IsMuddy;
    public bool IsInWater;
    public bool IsOnIce;
    public float TurnSpeedMultiplier;
    public float BulletSizeMultiplier;
    public bool BulletCanBounce;
    public int NbBounces;
    public float LerpTime;
    public float Life;
    public float Drag;
    public float AngularDrag;

    public float SpeedMultiplier
    {
        get
        {
            if (IsMuddy || IsInWater)
                return _speedMultiplier * 0.5f;
            if (IsOnIce)
                return _speedMultiplier * 0.25f;
            return _speedMultiplier;
        }
        set => _speedMultiplier = value;
    }

    public float MaxLife { get; private set; }

    public ModifiableStats(int maxLife)
    {
        IsInvincible = false;
        IsMuddy = false;
        SpeedMultiplier = 1.0f;
        TurnSpeedMultiplier = 1.0f;
        BulletSizeMultiplier = 1.0f;
        BulletCanBounce = false;
        LerpTime = 1;
        Life = maxLife;
        Drag = 10;
        AngularDrag = 10;
        MaxLife = maxLife;
    }

    public ModifiableStats(ModifiableStats stats)
    {
        IsInvincible = stats.IsInvincible;
        IsMuddy = stats.IsMuddy;
        SpeedMultiplier = stats.SpeedMultiplier;
        TurnSpeedMultiplier = stats.TurnSpeedMultiplier;
        BulletSizeMultiplier = stats.BulletSizeMultiplier;
        BulletCanBounce = stats.BulletCanBounce;
        LerpTime = stats.LerpTime;
        Life = stats.Life;
        Drag = stats.Drag;
        AngularDrag = stats.AngularDrag;
        MaxLife = stats.MaxLife;
    }
}