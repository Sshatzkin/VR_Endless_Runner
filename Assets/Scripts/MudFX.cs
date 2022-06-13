using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudFX : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource mudFX;

    public ParticleSystem mudPS;

    public JumpController jumpController;
    
    public ArduinoController arduino;

    public static bool walkingOnMud = false;

    public Renderer crackedGroundRenderer;
    // Start
    void Start()
    {
        // audioObject = GameObject.Find("EarthquakeSound");
        mudFX = audioObject.GetComponent<AudioSource>();
        crackedGroundRenderer.enabled = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        mudFX.Play();
        walkingOnMud = true;
        Debug.Log("Walking on mud");
        crackedGroundRenderer.enabled = true;
    }

    void OnTriggerStay(Collider other)
    {
        mudPS.Play();
    }

    void OnTriggerExit(Collider other)
    {
        mudFX.Stop();
        mudPS.Stop();
        walkingOnMud = false;
        arduino.Disarm();
    }
}
