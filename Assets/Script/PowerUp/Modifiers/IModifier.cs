using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    float Duration { get; set; }
    void Activate(ModifiableStats stats);

    void Deactivate(ModifiableStats stats);
}
