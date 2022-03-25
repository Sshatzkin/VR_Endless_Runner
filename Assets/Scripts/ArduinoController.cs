/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;
using System.Threading;
using System;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */
public class ArduinoController : MonoBehaviour
{
    public SerialController serial;
    public JumpController jumpController;

    private Thread vibrationThread;

    public bool isVibrating = false;

    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        Debug.Log("Message arrived: " + msg);
        if (msg.Length >= 15 && msg[0].Equals('J')) // Check if the incoming message is of the form "Jump Status = _"           
            jumpController.updateJumpState((int)Char.GetNumericValue(msg[14]));
            Debug.Log("JumpState = " + jumpController.jumpState);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if (success)
        {
            Debug.Log("Arduino connected!");

            // init backpack
            SetMode();
            SetWeightBottom();
        }
            
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }

    // Arduino functions:
    public void SetMode()
    {
        string arduinoMsg = "<c,a>";
        serial.SendSerialMessage(arduinoMsg);
    }

    public void Arm()
    {
        string arduinoMsg = "<p,1>";
        // serial.SendSerialMessage(arduinoMsg);
        serial.SendSerialMessage(arduinoMsg);
    }

    public void Disarm()
    {
        string arduinoMsg = "<p,0>";
        serial.SendSerialMessage(arduinoMsg);
    }

    public void SetDCTime(float dutyCycle, int driveTime)
    {
        if (Math.Abs(dutyCycle) > 1) dutyCycle = dutyCycle / 100;
        string arduinoMsg = "<s," + dutyCycle + "," + driveTime + ">";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void SetDriveTime(int start, int end)
    {
        string arduinoMsg = "<t," + start + "," + end + ">";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void SendManual(float dutyCycle, int driveTime)
    {
        if (Math.Abs(dutyCycle) > 1) dutyCycle = dutyCycle / 100;
        string arduinoMsg = "<" + dutyCycle + "," + driveTime + ">";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void SetWeightTop()
    {
        string arduinoMsg = "<-.3, 1500>";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void SetWeightBottom()
    {
        string arduinoMsg = "<.3, 2000>";
        serial.SendSerialMessage(arduinoMsg); 
    }

    public void SetWeightMiddle()
    {
        string arduinoMsg = "<-.1, 1000>";
        serial.SendSerialMessage(arduinoMsg); 
        Debug.Log("Setting weight to middle");
    }

    private void Vibration(float intensity)
    {
        SetWeightMiddle();
        Thread.Sleep(1000);
        
        Debug.Log("Start vibbing");
        while(isVibrating)
        {
            serial.SendSerialMessage("<"  + intensity + ", 100>"); 
            // Thread.Sleep(100);
            serial.SendSerialMessage("<-" + intensity + ", 100>"); 
            // Thread.Sleep(100);
            Debug.Log("Vibbing!");
        }
        
    }
    
    public void VibrationStart(float intensity)
    {
        isVibrating = true;
        vibrationThread = new Thread(()=>Vibration(intensity));
        vibrationThread.Start();
        // if (!vibrationThread.IsAlive || vibrationThread == null){
        //     Debug.Log("Creating thread");
        //     vibrationThread = new Thread(()=>Vibration(intensity));
        //     vibrationThread.Start();
        // }
        // else{
        //     Debug.Log("Thread is already alive");
        // }
        
    }

    public void VibrationStop()
    {
        isVibrating = false;
        vibrationThread.Join();
        vibrationThread.Abort();
    }
}
