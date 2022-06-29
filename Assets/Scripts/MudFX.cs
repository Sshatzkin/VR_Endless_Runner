using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudFX : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource mudFX;

    public ParticleSystem mudPS;
    [SerializeField, Tooltip("The scale of the landing mud particles compared to the walking mud particles")]
    private float particleScale;

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
        if(mudPS != null) {
            mudPS.Play();
        }
        else {
            crackedGroundRenderer.enabled = true;
        }
        ScaleMudParticles(1f / particleScale);
        walkingOnMud = true;
        Debug.Log("Walking on mud");
    }

    private void ScaleMudParticles(float scale) {
        if(mudPS == null) {
            return;
        }
        var main = mudPS.main;
        var startSize = main.startSize;
        startSize.constant *= scale;
        main.startSize = startSize;
    }

    void OnTriggerStay(Collider other)
    {
        if(mudPS != null) {
            mudPS.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        mudFX.Stop();
        ScaleMudParticles(particleScale);
        walkingOnMud = false;
        if(arduino != null) {
            arduino.Disarm();
        }
    }
}
