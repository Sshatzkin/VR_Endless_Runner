using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWeightMiddle : MonoBehaviour
{
    public ArduinoController arduino;
    
    // Start is called before the first frame update
    void OnTriggerStay(Collider other)
    {
        // Makes sure this gets triggered only once and avoids getting stuck in collision
        // Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
        arduino.SendManual(-.1f, 100);
    }

    void OnTriggerExit(Collider other)
    {
        // Makes sure this gets triggered only once and avoids getting stuck in collision
        // Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
        arduino.SendManual(0, 100);
    }
}
