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


#define SWITCH_BOT 0 // limit switch bottom
#define SWITCH_TOP 1 // limit switch top

void JumpController::setup() {
    /** Setup serial port for computer com **/
    Serial.begin(115200);
    Serial.println("JumpAR program begins...");

    if (!onlyPrediction) {
        /** Setup UART port for VESC (Serial1 on Atmega32u4) */
        Serial1.begin(115200);

        /** Define which ports to use as UART */
        UART.setSerialPort(&Serial1);

        Serial.println("UART initialization successful!");

        pinMode(SWITCH_BOT, INPUT);
        pinMode(SWITCH_TOP, INPUT);
    }
}

void JumpController::loop(float YZ) {
    // Predict user's jump status every 10ms
    predictJumpStatus(YZ);

    if (!onlyPrediction) {
        // Perform powered jump if we are in powered jump mode and the user jumps
        if (jumpStatus == predStartDrive && powerJumpPhase == "none" && powerJump) { // condition to start drive
            startJump(powerJumpDutyCycle, powerJumpDriveDuration);
            powerJumpPhase = "up";
        }
        else if (jumpStatus == predStopDrive && powerJumpPhase == "up" && driveStatus == "initial" && powerJump) // condition to stop drive
        {
            driveStatus = "brake";
            if(debug && !debugPlot) {
              Serial.println("Apex reached, stopping drive early");
            }
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
    if (debugSW && !debugPlot) {
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
    const char *jumpStatusStrs[] = {"Stand", "Crouch", "Launch", "Liftoff", "Ascent", "Apex", "Descent", "Landing", "Rebound"};
    if (debug && !debugPlot) {
        Serial.print("Jump Status = ");
        Serial.print(jumpStatusStrs[(int) jumpStatus - 1]);
        Serial.println(" at " + String(millis()));
    }
    statusStart = millis();
}

void JumpController::predictJumpStatus(float YZ)
{
    // Calculate time since last check
    unsigned long curLoopTime = millis();
    deltaTime = curLoopTime - prevLoopTime;

    // Integrate acceleration to calculate velocity and displacement
    velocity += YZ * ((float) deltaTime) / 1000;
    displacement += velocity * ((float) deltaTime) / 1000;

    // Save the current time for next iteration
    prevLoopTime = curLoopTime;

    // Recallibrate when the user is standning still
    if (abs(YZ) <= 0.1) {
        sameCount = sameCount > sameCountMax ? sameCount : sameCount + 1;
    }
    else {
        sameCount = 0;
    }
    if (sameCount >= sameCountMax) {
        velocity = 0;
        displacement = 0;
        resetJumpStatus();
    }

    // Store velocity extrema values
    if (velocity > maxVelocity)
    {
        maxVelocity = velocity;
        timeSinceMax = -1;
    }
    timeSinceMax = timeSinceMax < extremaDelta ? timeSinceMax + 1 : timeSinceMax;
    if (velocity < minVelocity)
    {
        minVelocity = velocity;
        timeSinceMin = -1;
    }
    timeSinceMin = timeSinceMin < extremaDelta ? timeSinceMin + 1 : timeSinceMin;

    // Determine Jump Status
    switch (jumpStatus) {
        case STAND:
            if (velocity < crouchBound && timeSinceMin >= extremaDelta)
            {
                jumpStatus = CROUCH;
                printJumpStatus();
            }
            break;
        case CROUCH:
            if (velocity > 0)
            {
                jumpStatus = LAUNCH;
                maxVelocity = -9999;
                printJumpStatus();
            }
            break;
        case LAUNCH:
            if (velocity < maxVelocity && timeSinceMax >= extremaDelta)
            {
                if (displacement < -0.07) {
                    minVelocity = 9999;
                    maxVelocity = -9999;
                    jumpStatus = STAND;
                    Serial.println("Crouch detected!");
                    printJumpStatus();
                }
                else {
                    jumpStatus = LIFTOFF;
                    velocity += liftoffErrorConstant;
                    maxVelocity += liftoffErrorConstant;
                    velocity += maxVelocity * liftoffErrorFactor;
                    maxVelocity += maxVelocity * liftoffErrorFactor;
                    printJumpStatus();
                }
            }
            break;
        case LIFTOFF:
            if (velocity < maxVelocity / 2)
            {
                jumpStatus = ASCENT;
                printJumpStatus();
            }
            break;
        case ASCENT:
            if (velocity < 0)
            {
                jumpStatus = APEX;
                minVelocity = 9999;
                printJumpStatus();
            }
            break;
        case APEX:
            if (velocity < -maxVelocity / 2)
            {
                jumpStatus = DESCENT;
                printJumpStatus();
            }
            break;
        case DESCENT:
            if (velocity > minVelocity && timeSinceMin >= extremaDelta)
            {
                jumpStatus = LANDING;
                velocity += landingErrorConstant;
                minVelocity += landingErrorConstant;
                velocity += abs(minVelocity) * landingErrorFactor;
                printJumpStatus();
            }
            break;
        case LANDING:
            if (velocity >= 0)
            {
                jumpStatus = REBOUND;
                printJumpStatus();
            }
            break;
        case REBOUND:
            // Nothing, just wait to reset
            if (velocity == 0) {
                resetJumpStatus();
            }
            break;
    }

    // Reset if nothing changes for a while in case an error occurs
    if (jumpStatus != STAND && millis() - statusStart > resetTime)
    {
        jumpStatus = REBOUND;
        printJumpStatus();
        resetJumpStatus();
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
    if (debug && !debugPlot) {
        Serial.print("Duty cycle: ");
        Serial.print(dutyCycle);
        Serial.print(" timing: ");
        Serial.println(timeinms);
    }
}

void JumpController::resetJumpStatus() {
    minVelocity = 9999;
    maxVelocity = -9999;
    if (jumpStatus != STAND) {
        jumpStatus = STAND;
        printJumpStatus();
    }
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
        if (debug && !debugPlot) {
            Serial.println("Set powerJump to " + String(powerJump));
        }
    }
    else if (strtokIndx[0] == 't') // setting different timing modes
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        predStartDrive = atoi(strtokIndx);     // convert this part to an integer
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        predStopDrive = atoi(strtokIndx);
        if (debug && !debugPlot) {
            Serial.print("Start driving at mode ");
            Serial.print(predStartDrive);
            Serial.print(" and stop driving at mode ");
            Serial.println(predStopDrive);
        }
    }
    else if (strtokIndx[0] == 's') // setting the jump DC and timing
    {
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        powerJumpDutyCycle = atof(strtokIndx);     // convert this part to an integer
        if (debug && !debugPlot) {
            Serial.print("Set power jump duty cycle to ");
            Serial.println(powerJumpDutyCycle);
        }
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        powerJumpDriveDuration = atof(strtokIndx);     // convert this part to an integer
        if (debug && !debugPlot) {
            Serial.print("Set power jump drive duration to ");
            Serial.println(powerJumpDriveDuration);
        }
    }
    else
    {   // manual mode
        dutyCycle = atof(strtokIndx);  // copy it to messageFromPC
        strtokIndx = strtok(NULL, ","); // get the 2nd part - the interation
        timeinms = atoi(strtokIndx);     // convert this part to an integer
        startJump(dutyCycle, timeinms);
    }

}
