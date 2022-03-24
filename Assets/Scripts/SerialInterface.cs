using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

using UnityEngine;

using System.IO.Ports;

public class SerialInterface : MonoBehaviour
{

    public JumpController jumpcontroller;
    private int prevjumpState = 0;

    /*
     * jumparActive indicates whether we're triggering jumpAR pack
     * 0 - jumpAR inactive
     * 1 - will trigger jumpAR on user Jump
     */
    public int jumparActive = 0;

    public string serialport = "\\\\.\\COM13"; // "\\\\.\\" needed if COM > 9

    float next_time;

    private static string incomingMsg;

    SerialPort sp;

    private Thread spReadingThread;
    private bool looping;
    float jumpTime = -1;

    // Update is called once per frame
    // void Update()
    // {
    //     if (sp.IsOpen)
    //     {
    //         try
    //         {
    //             incomingMsg = sp.ReadLine();
    //             Debug.Log("incomingMsg: " + incomingMsg);

    //             if (incomingMsg.Length >= 15) // Check if the incoming message is of the form "Jump Status = _"           
    //                 jumpcontroller.updateJumpState((int)Char.GetNumericValue(incomingMsg[14]));
    //                 Debug.Log("JumpState = " + jumpcontroller.jumpState);

    //         }
    //         catch (Exception e)
    //         {
    //             Debug.Log("Serial Error: " + e);
    //         }
    //         // catch (TimeoutException)
    //         // {
    //         //     Debug.Log("TimeoutException");
    //         // }
    //     }
    // }
    
    private void OnEnable()
    {
        looping = true;
        spReadingThread = new Thread(ReadArduino);
        spReadingThread.Start();
    }

    private void OnDestroy()
    {
        looping = false;  // This is a necessary command to stop the thread.
                        // if you comment this line, Unity gets frozen when you stop the game in the editor.                           
        spReadingThread.Join();
        spReadingThread.Abort();
        sp.Close();
    }

    void ReadArduino()
    {
        Debug.Log("START");
        sp = new SerialPort(serialport, 9600);
        string[] ports = SerialPort.GetPortNames();

        Debug.Log("# Ports: " + ports.Length);
        for (int i = 0; i < ports.Length; i++)
            {
                Debug.Log(ports[i]);
            }

        sp.ReadTimeout = 500;
        sp.WriteTimeout = 1000;
        sp.Handshake = Handshake.None;
        sp.DtrEnable = true;
        sp.WriteTimeout = 1000;
        sp.Parity = Parity.None;
        sp.StopBits = StopBits.One;
        sp.DataBits = 8;
        sp.NewLine = "\n";
        sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        sp.Open();

        if(sp.IsOpen){
            Debug.Log("Serial started");
        }

        // init backpack
        ArduinoSetMode();
        ArduinoDisarm();
        ArduinoSetWeightBottom();

        // main serial loop
        while (looping)
        {   
            try
            {
                // incomingMsg = sp.ReadLine();
                // Debug.Log("Serial Incoming: " + incomingMsg);
                Thread.Sleep(0);
            }
            catch (Exception e)
            {
                Debug.Log("Serial error: " + e);
            }
            
        }

    }

    private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
        Debug.Log("Data Received:");
        Debug.Log(indata);
    }

    public void WriteToSerial(string message)
    {
        sp.WriteLine(message);
        sp.BaseStream.Flush();
    }

    public void ArduinoSetMode()
    {
        string arduinoMsg = "<c,a>";
        WriteToSerial(arduinoMsg);
    }

    public void ArduinoArm()
    {
        string arduinoMsg = "<p,1>";
        WriteToSerial(arduinoMsg);
    }

    public void ArduinoDisarm()
    {
        string arduinoMsg = "<p,0>";
        WriteToSerial(arduinoMsg);
    }

    public void ArduinoSetDCTime(int dutyCycle, int driveTime)
    {
        string arduinoMsg = "<s," + dutyCycle + "," + driveTime + ">";
        WriteToSerial(arduinoMsg); 
    }

    public void ArduinoSendManual(int dutyCycle, int driveTime)
    {
        string arduinoMsg = "<" + dutyCycle + "," + driveTime + ">";
        WriteToSerial(arduinoMsg); 
    }

    public void ArduinoSetWeightTop()
    {
        string arduinoMsg = "<-.1, 1500>";
        WriteToSerial(arduinoMsg); 
    }

    public void ArduinoSetWeightBottom()
    {
        string arduinoMsg = "<.1, 2000>";
        WriteToSerial(arduinoMsg); 
    }
    
}
