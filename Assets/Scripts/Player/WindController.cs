using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource windFX;

    public ParticleSystem wind;

    // Start
    void Start()
    {
        audioObject = GameObject.Find("WindSound");
        windFX = audioObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (true) // jumpController.jumpState >=3 && jumpController.jumpState < 9)
        {
            Debug.Log("Wind zone entered");
            CreateWind();
            Destroy(gameObject);
        }
    }

    void CreateWind()
    {
        wind.Play();
        windFX.Play();
    }
}
