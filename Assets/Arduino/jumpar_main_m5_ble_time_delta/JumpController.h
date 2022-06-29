/*
    Morse.h - Library for flashing Morse code.
    Created by David A. Mellis, November 2, 2007.
    Released into the public domain.
*/

#ifndef JumpController_h
#define JumpController_h
#include <Arduino.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <VescUart.h>
#include <Wire.h>
#include <math.h>

class JumpController
{
    public:
        // Functions
        void loop(float YZ);
        void parseData(char* tempChars);
        void driveMotor();
        void startJump(float newDutyCycle, int newTimeInMS);
        void setup();
        void predictJumpStatus(float YZ);
        void printJumpStatus();

        bool debug = true;
        bool debugPlot = false;
        bool printDT = false;
        bool debugSW = false;
        bool debugPredict = true;

        bool drive = true;
        bool onlyPrediction = false;


        // driving var
        float dutyCycle = 0;
        int timeinms = 0;
        int predStartDrive = 3;
        int predStopDrive = 4;

        // Timer
        unsigned long timeZero, timePrev;
        unsigned long timeAtTarget;
        unsigned long prevLoopTime = 0;

        // Jump Status
        enum JumpStatus {
            STAND = 1,
            CROUCH,
            LAUNCH,
            LIFTOFF,
            ASCENT,
            APEX,
            DESCENT,
            LANDING,
            REBOUND
        };
        JumpStatus jumpStatus = STAND;
        int statusStart = 0;

        // Callibration
        int sameCount = 0;
        int sameCountMax = 3; // Reset to 0 velocity when sameCount reaches this value
        float crouchBound = -0.3;
        float liftoffErrorFactor = -0.5;
        float liftoffErrorConstant = 0.5;
        float landingErrorFactor = -0.6;
        float landingErrorConstant = 0;
        unsigned long resetTime = 1100;

        // Physics parameters
        float velocity = 0;
        float displacement = 0;
        
        // Extrema values
        float maxVelocity = -999;
        float minVelocity = 999;
        int timeSinceMax = 0;
        int timeSinceMin = 0;
        int extremaDelta = 1;
        
        float powerJumpDutyCycle = -.5;
        float powerJumpDriveDuration = 100;
        float lastPredUpdate = 0;

        unsigned long deltaTime = 0;

        /** Initiate VescUart class */
        VescUart UART;

        float current = 1.0; /** The current in amps */
        float currentBrake = 20.0;

        int firstDrive = 0;

        float lastPrint = 0;

        String driveStatus = "none";

        boolean powerJump = false;
        String powerJumpPhase = "none";

    private:
        void resetJumpStatus();

};

#endif
