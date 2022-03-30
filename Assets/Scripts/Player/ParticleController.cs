using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem dust;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground Plane")
        {
            //Debug.Log("Landng on the ground!!");
            CreateDust();
        }
    }    
    void CreateDust()
    {
        dust.Play();
    }
}
