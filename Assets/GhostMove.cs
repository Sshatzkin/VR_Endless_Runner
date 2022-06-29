using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMove : MonoBehaviour
{
    public Vector3 deltaPosition;
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position + deltaPosition;
    }
}
