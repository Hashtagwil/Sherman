using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockSlider : MonoBehaviour
{
    [SerializeField] Transform tank;
    public Image Fill;

    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        transform.position = tank.position + transform.up * 0.7f;
    }
}
