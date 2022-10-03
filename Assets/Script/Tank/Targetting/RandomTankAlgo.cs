using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RandomTankAlgo : MonoBehaviour, ITargetAcquisitionAlgo
{
    public GameObject AcquireTarget(GameObject caller, List<GameObject> exclusions, Collider2D[] tanksColliders)
    {
        GameObject target = caller;
        int MaxAttempts = 10;
        int i = 0;
        while (target == caller || exclusions.Contains(target) && i < MaxAttempts)
        {
            target = AcquireRandomTarget(tanksColliders);
            i++;
        }
        return target;
    }
    private GameObject AcquireRandomTarget(Collider2D[] tanksColliders)
    {
        int index = Mathf.RoundToInt(Random.value * tanksColliders.Length);
        if (index >= tanksColliders.Length) return null;

        return tanksColliders[index].gameObject;
    }

}
