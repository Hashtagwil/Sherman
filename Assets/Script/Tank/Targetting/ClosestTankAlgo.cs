using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ClosestTankAlgo : MonoBehaviour, ITargetAcquisitionAlgo
{
    public GameObject AcquireTarget(GameObject caller, List<GameObject> exclusions, Collider2D[] tanksColliders)
    {
        GameObject target = null;
        float minDistance = float.MaxValue;
        foreach (Collider2D collider in tanksColliders)
        {
            if (collider.gameObject != caller && !exclusions.Contains(collider.gameObject))
            {
                float distance = Vector2.Distance(caller.transform.position, collider.gameObject.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = collider.gameObject;
                }
            }
        }
        return target;
    }
}
