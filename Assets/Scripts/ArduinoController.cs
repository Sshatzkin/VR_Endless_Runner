/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */
public class ArduinoController : MonoBehaviour
{
    public SerialController serial;
    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        Debug.Log("Message arrived: " + msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
        {
            Debug.Log("Connection established");

            // init backpack
            // ArduinoSetMode();
            // ArduinoDisarm();
            // ArduinoSetWeightBottom();
        }
            
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

    // Arduino functions:
    public void ArduinoSetMode()
    {
        string arduinoMsg = "<c,a>";
        serial.SendSerialMessage(arduinoMsg);
    }

    public void ArduinoArm()
    {
        string arduinoMsg = "<p,1>";
        // serial.SendSerialMessage(arduinoMsg);
        serial.SendSerialMessage(arduinoMsg);
    }

    public void ArduinoDisarm()
    {
        string arduinoMsg = "<p,0>";
        serial.SendSerialMessage(arduinoMsg);
    }

    public void ArduinoSetDCTime(int dutyCycle, int driveTime)
    {
        string arduinoMsg = "<s," + dutyCycle + "," + driveTime + ">";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void ArduinoSendManual(int dutyCycle, int driveTime)
    {
        string arduinoMsg = "<" + dutyCycle + "," + driveTime + ">";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void ArduinoSetWeightTop()
    {
        string arduinoMsg = "<-.1, 1500>";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void ArduinoSetWeightBottom()
    {
        string arduinoMsg = "<.1, 2000>";
        serial.SendSerialMessage(arduinoMsg); 
    }
    
}
