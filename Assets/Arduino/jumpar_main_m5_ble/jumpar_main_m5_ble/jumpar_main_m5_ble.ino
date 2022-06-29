/*
    Name:    setCurrent.ino
    Created: 19-08-2018
    Author:  SolidGeek
    Description: This is a very simple example of how to set the current for the motor
*/
#include "JumpController.h"
#include "LSM6DS3.h"
#include "Wire.h"
#include <ArduinoBLE.h>

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define GET_CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"
#define SEND_CHARACTERISTIC_UUID "8628FE7C-A4E9-4056-91BD-FD6AA7817E39"

JumpController myJC;

//Create a instance of class LSM6DS3
LSM6DS3 xiaoIMU(I2C_MODE, 0x6A);    //I2C device address 0x6A

/* XIAO BLE Sense Params */
float accX = 0.0F;
float accY = 0.0F;
float accZ = 0.0F;

float gyroX = 0.0F;
float gyroY = 0.0F;
float gyroZ = 0.0F;

float temp = 0;

/* Communication Params */
boolean newData = false;
const byte numChars = 32;
char receivedChars[numChars];
char tempChars[numChars];        // temporary array for use when parsing


BLEService jumpService(SERVICE_UUID); // Bluetooth® Low Energy LED Service
// Bluetooth® Low Energy LED Switch Characteristic - custom 128-bit UUID, read and writable by central
BLECharacteristic jumpCharacteristic(GET_CHARACTERISTIC_UUID, BLERead | BLEWrite, "                                "); // Sets max size to 32 with a 32 length string
BLECharacteristic sendCharacteristic(SEND_CHARACTERISTIC_UUID, BLERead | BLEWrite | BLENotify, "    "); // Sets max size to 4 with a 4 length string
bool isConnected = false;

// Blinks the LED every 100 interations to show the code is running
int blinkCounter = 0;

void setup() {
    xiaoIMU.begin();

    myJC.onlyPrediction = false;
    myJC.setup();

    // Setup BLE
    if (!BLE.begin()) {
        Serial.println("starting Bluetooth® Low Energy module failed!");
        while (1);
    }

    // set advertised local name and service UUID:
    BLE.setLocalName("Jumper");
    BLE.setAdvertisedService(jumpService);

    // add the characteristic to the service
    jumpService.addCharacteristic(jumpCharacteristic);
    jumpService.addCharacteristic(sendCharacteristic);

    // add service
    BLE.addService(jumpService);

    // start advertising
    BLE.advertise();

    //Setup LED
    pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
    /* Set values for the XIAO BLE Sense */
    gyroX = xiaoIMU.readFloatGyroX();
    gyroY = xiaoIMU.readFloatGyroY();
    gyroZ = xiaoIMU.readFloatGyroZ();
    accX = xiaoIMU.readFloatAccelX();
    accY = xiaoIMU.readFloatAccelY();
    accZ = xiaoIMU.readFloatAccelZ();
    temp = xiaoIMU.readTempC();

    float YZ = sqrt(sq(accY * 9.8) + sq(accZ * 9.8));
    myJC.loop(YZ);

    /* Handle commands */
    if (myJC.drive) {
        recvWithStartEndMarkers();
        if (newData == true) {
            strcpy(tempChars, receivedChars);
            // this temporary copy is necessary to protect the original data
            //   because strtok() used in parseData() replaces the commas with \0
            myJC.parseData(tempChars);
            newData = false;
        }
    }

    /* BLE */
    if (BLE.connected() && !isConnected) {
        isConnected = true;
        Serial.println("Bluetooth device connected!");
    }
    else if (!BLE.connected() && isConnected) {
        isConnected = false;
        Serial.println("Bluetooth device disconnected! Restarting advertising...");
        BLE.advertise();
    }

    /* Update bluetooth jump status */
    if (atoi((const char*) sendCharacteristic.value()) != myJC.jumpStatus) {
        sendCharacteristic.writeValue((char *) String(myJC.jumpStatus).c_str());
        Serial.println("Sent via BLE: " + String(myJC.jumpStatus));
        myJC.sendData = false;
    }

    /* Read bluetooth messages */
    if (jumpCharacteristic.written()) {
        const unsigned char *value = jumpCharacteristic.value();
        Serial.print("Message received: ");
        Serial.println((const char*) value);
        strcpy(tempChars, (const char*) value);
        myJC.parseData(tempChars);
    }

    /* Blink Test */
    if (blinkCounter == 100) {
        digitalWrite(LED_BUILTIN, HIGH);
    }
    if (blinkCounter == 0) {
        digitalWrite(LED_BUILTIN, LOW);
    }
    blinkCounter = (blinkCounter + 1) % 200;
}

void recvWithStartEndMarkers() {
    static boolean recvInProgress = false;
    static byte ndx = 0;
    char startMarker = '<';
    char endMarker = '>';
    char rc;

    while (Serial.available() > 0 && newData == false) {
        rc = Serial.read();

        if (recvInProgress == true) {
            if (rc != endMarker) {
                receivedChars[ndx] = rc;
                ndx++;
                if (ndx >= numChars) {
                    ndx = numChars - 1;
                }
            }
            else {
                receivedChars[ndx] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                newData = true;
            }
        }

        else if (rc == startMarker) {
            recvInProgress = true;
        }
    }
}
