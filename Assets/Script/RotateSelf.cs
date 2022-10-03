using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    float speed = 100.0f;
    bool started = false;


    public void StartSpinning()
    {
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            Vector3 rotation = transform.localRotation.eulerAngles;
            rotation.z += speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }
    }
}
