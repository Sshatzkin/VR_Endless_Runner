using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionMudLanding : MonoBehaviour
{
    public JumpController jumpController;
    public ArduinoController arduino;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        // Makes sure this gets triggered only once and avoids getting stuck in collision
        Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
        arduino.SetWeightBottom();
        arduino.SetDCTime(-50, 150);
        arduino.SetDriveTime(8,10);
        arduino.Arm();

    }
}
