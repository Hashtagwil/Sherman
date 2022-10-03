using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetAcquisitionAlgo
{
    public GameObject AcquireTarget(GameObject caller, List<GameObject> exclusions, Collider2D[] tanksColliders);
}
