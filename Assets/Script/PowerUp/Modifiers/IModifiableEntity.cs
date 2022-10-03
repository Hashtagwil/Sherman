using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiableEntity
{
    void AddModifer(IModifier modifier);
    void RemoveModifier(IModifier modifier);
}
