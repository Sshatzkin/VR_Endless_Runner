using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationSetup : MonoBehaviour
{
    public ArduinoController arduino;
    public bool defaultDown = false;

    private int direction = 1;

    float next_time; 
    bool flip = false;

    void Start(){
        if (defaultDown){
            direction = -1;
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (Time.time > next_time) { 
            if (flip) 
            {
                arduino.SendManual(-.1f * direction, 150);
                next_time = Time.time + 0.15f;
            }
            else 
            {
                arduino.SendManual(.1f * direction, 50);
                next_time = Time.time + 0.05f;
            }
            flip = !flip;

            //Debug.Log("Flipping! " + next_time);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (defaultDown) arduino.SetWeightBottom();
        else arduino.SetWeightTop();
        // arduino.SendManual(0, 100);
    }
}



