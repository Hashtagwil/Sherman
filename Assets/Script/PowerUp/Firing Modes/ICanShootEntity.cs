using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanShootEntity
{
    void ChangeFiringMode(IFiringMode firingMode);
}
