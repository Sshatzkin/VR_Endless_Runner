using System.Collections;
using System.Collections.Generic;
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

    public string serialport = "COM12";

    float next_time;

    private static string incomingMsg;

    SerialPort sp;
    float jumpTime = -1;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START");
        sp = new SerialPort(serialport, 9600);
        string[] ports = SerialPort.GetPortNames();

        Debug.Log("# Ports: " + ports.Length);
        for (int i = 0; i < ports.Length; i++)
            {
                Debug.Log(ports[i]);
            }

        sp.ReadTimeout = 30;
        sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        sp.Open();
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                incomingMsg = sp.ReadLine();
                Debug.Log("incomingMsg: " + incomingMsg);

                if (incomingMsg.Length >= 15) // Check if the incoming message is of the form "Jump Status = _"           
                    jumpcontroller.jumpState = (int)Char.GetNumericValue(incomingMsg[14]);
                    Debug.Log("JumpState = " + jumpcontroller.jumpState);

            }
            catch (TimeoutException)
            {
                //Debug.Log("TimeoutException");
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
}
