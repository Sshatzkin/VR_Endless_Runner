using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_helper : MonoBehaviour
{
    public OSC osc;

    public string path = "/command";

    public bool Debug_enabled = true;

    public void SendString(string msg)
    {
        OscMessage message = new OscMessage();

        message.address = path;
        message.values.Add(msg);
        osc.Send(message);

        if (Debug_enabled){
            Debug.Log("OSC Sent: " + message.ToString());
        }
    }
}
