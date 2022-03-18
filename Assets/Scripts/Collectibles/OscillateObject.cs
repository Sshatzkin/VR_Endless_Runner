using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateObject : MonoBehaviour
{
    public float oscillationSpeed = 1.5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        float y = Mathf.PingPong(Time.time * oscillationSpeed, 1) / 2 + 1.5f; // values that look good in animation
        transform.position = new Vector3(pos.x, y, pos.z);
    }
}
