using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamera : MonoBehaviour
{
    public GameObject follow;
    public float followOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, follow.transform.position.z + followOffset);
    }
}
