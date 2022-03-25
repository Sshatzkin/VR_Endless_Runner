using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationSetUp : MonoBehaviour
{
    public ArduinoController arduino;

    float next_time; 
    bool flip = false;
    
    void OnTriggerStay(Collider other)
    {
        if (Time.time > next_time) { 
            if (flip) 
            {
                arduino.SendManual(-.1f, 150);
                next_time = Time.time + 0.15f;
            }
            else 
            {
                arduino.SendManual(.1f, 50);
                next_time = Time.time + 0.05f;
            }
            flip = !flip;

            Debug.Log("Flipping! " + next_time);
        }
    }

    void OnTriggerExit(Collider other)
    {
        arduino.SendManual(0, 100);
    }
}



