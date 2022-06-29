#include "JumpController.h"
#include "LSM6DS3.h"
#include "Wire.h"
#include <ArduinoBLE.h>
#include <SimpleFOC.h> // https://docs.simplefoc.com/low_pass_filter

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

unsigned long prevdt = 0;
bool isUpsideDown = false;

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
LowPassFilter filter = LowPassFilter(0.01);

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
    /* Jump prediction setup */
    // Read acceleration values
    accX = xiaoIMU.readFloatAccelX();
    accY = xiaoIMU.readFloatAccelY();
    accZ = xiaoIMU.readFloatAccelZ();
    accX *= 9.8 * 9.8 / 9.93;
    accY *= 9.8 * 9.8 / 9.93;
    accZ *= 9.8 * 9.8 / 9.93;
    
    // Apply a low-pass filter on the accelation data to reduce noise
    float YZ = filter(sqrt(sq(accY) + sq(accZ))) - 9.8;
    if(isUpsideDown) {
        YZ *= -1;
    }
    
    // Print plotting info
    if (myJC.debugPlot) {
        unsigned long dt = millis();
        unsigned long deltaTime = dt - prevdt;
        prevdt = dt;
        Serial.print(
            "YZ: " + String(YZ) +
            " Velocity: " + String(myJC.velocity) + " Displacement: " + String(myJC.displacement * 5) +
            " Jump Status: " + String(myJC.jumpStatus));
        Serial.println(myJC.printDT ? (" Delta Time: " + String(deltaTime)) : "");
    }
    
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
    bool connected = BLE.connected();
    if (connected && !isConnected) {
        isConnected = true;
        if (myJC.debug && !myJC.debugPlot) {
            Serial.println("Bluetooth device connected!");
        }
    }
    else if (!connected && isConnected) {
        isConnected = false;
        BLE.advertise();
        if (myJC.debug && !myJC.debugPlot) {
            Serial.println("Bluetooth device disconnected! Restarting advertising...");
        }
    }
    
    int sendCharValue = atoi((const char *) sendCharacteristic.value());
     // Ensure the user is not tilted on the Z-axis
    if (abs(myJC.displacement) > 4) {
        Serial.println("Error: You are not rotated correctly!");
        if(isConnected && sendCharValue != 0) {
            sendCharacteristic.writeValue("0");
        } 
    }
    else if(isConnected && sendCharValue == 0) {
        sendCharacteristic.writeValue((char *) String((int) myJC.jumpStatus).c_str());
        if (myJC.debug && !myJC.debugPlot) {
            Serial.println("Sent via BLE: " + String(myJC.jumpStatus));
        }
    }
    sendCharValue = atoi((const char *) sendCharacteristic.value());
    /* Update bluetooth jump status */
    if (isConnected && sendCharValue != (int) myJC.jumpStatus && sendCharValue != 0) {
        sendCharacteristic.writeValue((char *) String((int) myJC.jumpStatus).c_str());
        if (myJC.debug && !myJC.debugPlot) {
            Serial.println("Sent via BLE: " + String(myJC.jumpStatus));
        }
    }

    /* Read bluetooth messages */
    if (jumpCharacteristic.written()) {
        const unsigned char *value = jumpCharacteristic.value();
        strcpy(tempChars, (const char*) value);
        myJC.parseData(tempChars);
        if (myJC.debug && !myJC.debugPlot) {
            Serial.print("Message received: ");
            Serial.println((const char*) value);
        }
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

// Read data from the Serial monitor
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
