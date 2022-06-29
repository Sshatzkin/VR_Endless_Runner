/*
    Morse.cpp - Library for flashing Morse code.
    Created by David A. Mellis, November 2, 2007.
    Released into the public domain.
*/

#include "JumpController.h"
#include <Arduino.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <VescUart.h>
#include <math.h>


#define SWITCH_BOT 10 // limit switch bottom
#define SWITCH_TOP 11 // limit switch top

#define SERIAL1_RX 32 // yellow
#define SERIAL1_TX 33 // white

// JumpController::JumpController(bool op)
// {
//   onlyPrediction = op;
// }

//HardwareSerial Serial1(1);

void JumpController::setup() {
    /** Setup serial port for computer com **/
    Serial.begin(115200);

    //  while (!Serial); // Hold the code until serial monitor opens

    Serial.println("JumpAR program begins...");

    if (!onlyPrediction) {
        /** Setup UART port for VESC (Serial1 on Atmega32u4) */
        //    Serial1.begin(115200, SERIAL1_RX, SERIAL1_TX);
        //    Serial1.begin(115200, SERIAL_8N1, SERIAL1_RX, SERIAL1_TX);
        Serial1.begin(115200);

        //  while (!Serial1); // Hold the code until it can connect to VESC

        /** Define which ports to use as UART */
        UART.setSerialPort(&Serial1);

        Serial.println("UART initialization successful!");

        pinMode(SWITCH_BOT, INPUT);
        pinMode(SWITCH_TOP, INPUT);

    }

}

void JumpController::loop(float YZ) {
    // Predict user's jump status every 10ms
    if (millis() - lastPredUpdate > 10) {
        predictJumpStatus(YZ);
        lastPredUpdate = millis();
    }


    if (!onlyPrediction) {
        // Perform powered jump if we are in powered jump mode and the user jumps
        if (controlMode == "arduino") {
            if (jumpStatus == predStartDrive && powerJumpPhase == "none" && powerJump) { // condition to start drive
                startJump(powerJumpDutyCycle, powerJumpDriveDuration);
                powerJumpPhase = "up";
            }
            else if (jumpStatus == predStopDrive && powerJumpPhase == "up" && driveStatus == "initial" && powerJump) // condition to stop drive
            {
                driveStatus = "brake";
                if (debug) Serial.println("Apex reached, stopping drive early");
            }
            else if (jumpStatus == 1 && powerJumpPhase == "up") { // condition to reset weight
                if (powerJumpDutyCycle < 0)
                    startJump(.05, 4000);
                else if (powerJumpDutyCycle > 0)
                    startJump(-.05, 4000);
                powerJumpPhase = "down";
            }
            else if (powerJumpPhase == "down" && driveStatus == "none")
            {
                powerJumpPhase = "none";
            }
        }

        if (drive) {
            driveMotor();

        }

    }


    if (millis() - lastPrint > 500) {
        lastPrint = millis();
        // pinging the ESC
        if (!onlyPrediction)
            if (driveStatus == "none")
                UART.setDuty(0);
    }

    // debug limit switches
    if (debugSW) {
        Serial.print("SW_B = ");
        Serial.print(digitalRead(SWITCH_BOT));
        Serial.print(" -- ");
        Serial.print("SW_T = ");
        Serial.println(digitalRead(SWITCH_TOP));

    }
}


void JumpController::driveMotor() {
    if (driveStatus == "initial") {
        if (timeAtTarget > millis() - timeZero) {
            // limit switch when driving down
            if (dutyCycle > 0) {
                if (digitalRead(SWITCH_BOT) == 1) driveStatus = "brake";
            }
            // limit switch when driving up
            if (dutyCycle < 0) {
                if (digitalRead(SWITCH_TOP) == 1) driveStatus = "brake";
            }
            if (driveStatus != "brake") {
                if (millis() - timePrev >= 900 || firstDrive == 0) { // send message every 900ms so it doesn't fill up ESC buffer
                    if (drive) UART.setDuty(dutyCycle);
                    timePrev = millis();
                    firstDrive = 1;
                }
            }
        }
        else {
            driveStatus = "brake";
        }
    }
    if (driveStatus == "brake") {
        if (drive) {
            UART.setDuty(0);
            driveStatus = "none";
        }
    }
}


void JumpController::printJumpStatus()
{
    if (debug) Serial.print("Jump Status = ");
    if (debug) Serial.print(jumpStatus);
    if (debug) Serial.print(" at ");
    if (debug) Serial.println(millis());
    sendData = true;
    statusStart = millis();
}

void JumpController::predictJumpStatus(float YZ)
{

    y_avg[i % 20] = YZ;
    i++;
    velocity += YZ - 9.8;

    //Dampen velocity
    if (abs(y_avg[i % 20] - y_avg[(i - 1) % 20]) < .05 && abs(y_avg[i % 20] - 9.8) < 2)
        velocity = velocity * 0.95;

    if (velocity > maxVelocity)
    {
        maxVelocity = velocity;
        iMarker = i;
    }
    if (velocity < minVelocity)
    {
        minVelocity = velocity;
        iMarker = i;
    }

    if (jumpStatus == 1)
    {
        if (velocity < -30 && i > 3 + iMarker)
        {
            jumpStatus = 2;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 2)
    {
        if (velocity > 0)
        {
            jumpStatus = 3;
            maxVelocity = -9999;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 3)
    {
        if (velocity < maxVelocity and i > 3 + iMarker)
        {
            jumpStatus = 4;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 4)
    {
        if (velocity < maxVelocity / 2)
        {
            jumpStatus = 5;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 5)
    {
        if (velocity < apexAverage)
        {
            jumpStatus = 6;
            minVelocity = 9999;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 6)
    {
        //      if (velocity < status7Average)
        if (velocity < apexAverage - 10)
        {
            jumpStatus = 7;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 7)
    {
        if (velocity > minVelocity and i > 3 + iMarker)
        {
            jumpStatus = 8;
            if (debugPredict) Serial.println("cal:" + (String) apexAverage +
                                                 ",max:" + (String) maxVelocity +
                                                 ",min:" + (String) minVelocity + ",");
            printJumpStatus();
            apexVelocity[i2 % 5] = maxVelocity - (maxVelocity - minVelocity) / 2;
            apexAverage = (apexVelocity[0] + apexVelocity[1] + apexVelocity[2] + apexVelocity[3] + apexVelocity[4]) / 5;
            status7Velocity[i2 % 5] = apexVelocity[i2 % 5] - (apexVelocity[i2 % 5] - minVelocity) / 2;
            status7Average = (status7Velocity[0] + status7Velocity[1] + status7Velocity[2] + status7Velocity[3] + status7Velocity[4]) / 5;
            i2++;
        }
    }
    else if (jumpStatus == 8)
    {
        if (velocity > 0)
        {
            jumpStatus = 9;
            printJumpStatus();
        }
    }
    else if (jumpStatus == 9)
    {
        // Nothing, just wait to reset
    }
    if (jumpStatus != 1 && millis() - statusStart > 1000)
    {
        minVelocity = 9999;
        maxVelocity = -9999;
        velocity = 0;
        jumpStatus = 1;
        if (debug) Serial.println("Jump Status = 9");
        if (debug) Serial.println("Jump Status = 1");
    }
}

void JumpController::startJump(float newDutyCycle, int newTimeInMS)
{
    dutyCycle = newDutyCycle;
    timeinms = newTimeInMS;

    // Init params for timer
    firstDrive = 0;
    timeZero = millis();
    timePrev = timeZero;
    timeAtTarget = timeinms;// + timeZero;
    driveStatus = "initial";
    if (debug) Serial.print("Duty cycle: ");
    if (debug) Serial.print(dutyCycle);
    if (debug) Serial.print(" timing: ");
    if (debug) Serial.println(timeinms);
}


void JumpController::parseData(char* tempChars) {
    char * strtokIndx; // this is used by strtok() as an index

    strtokIndx = strtok(tempChars, ","); // arming the device: <p,0> = disarm, <p,1> = arm
    if (strtokIndx[0] == 'p')
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        if (atoi(strtokIndx) == 1) { // convert this part to an integer
            powerJump = true;
        }
        else {
            powerJump = false;
        }
        if (debug) Serial.print("Set powerJump to ");
        if (debug) Serial.println(powerJump);
    }
    else if (strtokIndx[0] == 'c') // setting the way the backpack is controlled, arduino or unity
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        controlMode = strtokIndx[0] == 'a' ?  "arduino" : "unity";     // convert this part to an integer
        if (debug) Serial.print("Set controlMode to ");
        if (debug) Serial.println(controlMode);
    }
    else if (strtokIndx[0] == 't') // setting different timing modes
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        predStartDrive = atoi(strtokIndx);     // convert this part to an integer
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        predStopDrive = atoi(strtokIndx);
        if (debug) Serial.print("Start driving at mode ");
        if (debug) Serial.print(predStartDrive);
        if (debug) Serial.print(" and stop driving at mode ");
        if (debug) Serial.println(predStopDrive);
    }
    else if (strtokIndx[0] == 's') // setting the jump DC and timing
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        powerJumpDutyCycle = atof(strtokIndx);     // convert this part to an integer
        if (debug) Serial.print("Set power jump duty cycle to ");
        if (debug) Serial.println(powerJumpDutyCycle);
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        powerJumpDriveDuration = atof(strtokIndx);     // convert this part to an integer
        if (debug) Serial.print("Set power jump drive duration to ");
        if (debug) Serial.println(powerJumpDriveDuration);
    }
    else if (strtokIndx[0] == 'v') // set velocity of the apex
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        apexAverage = atof(strtokIndx);     // convert this part to an integer
        apexVelocity[0] = atof(strtokIndx);
        apexVelocity[1] = atof(strtokIndx);
        apexVelocity[2] = atof(strtokIndx);
        apexVelocity[3] = atof(strtokIndx);
        apexVelocity[4] = atof(strtokIndx);
        if (debug) Serial.print("Set apex velocity to ");
        if (debug) Serial.println(apexAverage);
    }
    else
    {   // manual mode
        dutyCycle = atof(strtokIndx);  // copy it to messageFromPC
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        timeinms = atoi(strtokIndx);     // convert this part to an integer
        startJump(dutyCycle, timeinms);
    }

}
