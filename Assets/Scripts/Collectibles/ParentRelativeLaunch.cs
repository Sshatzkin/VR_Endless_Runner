using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentRelativeLaunch : MonoBehaviour
{
    private Rigidbody phys;
    public float launchFactor;
    // Start is called before the first frame update
    void Start()
    {
        phys = transform.GetComponent<Rigidbody>();
        phys.AddForce((transform.position - transform.parent.position) * launchFactor, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
