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
    // JumpController();

    void loop(float YZ);
    void parseData(char* tempChars);
    void driveMotor();
    void startJump(float newDutyCycle, int newTimeInMS);
    void setup();
    void predictJumpStatus(float YZ);
    void printJumpStatus();

    bool debug = true;
    bool debugSW = false;
    bool debugPredict = true;

    bool drive = true;
    bool onlyPrediction = false;
    bool sendData = false;


    // driving var
    float dutyCycle = 0;
    int timeinms = 0;
    int predStartDrive = 3;
    int predStopDrive = 4;

    // Timer
    unsigned long timeZero, timePrev;
    unsigned long timeAtTarget;

    // Jump prediction variables
    int i = 0; //increments every frame
    int i2 = 0; //increments every jump
    int iMarker = -1;
    int statusStart = 0;
    float y_avg[20] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    float max_y_acc = -9999;
    float min_y_acc = 9999;
    float velocity = 0;
    float maxVelocity = -999;
    float minVelocity = 999;
    float apexVelocity[5] = {0,0,0,0,0};
    float apexAverage = 0;
    float status7Velocity[5] = {-5,-5,-5,-5,-5};
    float status7Average = -5;
    int jumpStatus = 1;
    float powerJumpDutyCycle = -.5;
    float powerJumpDriveDuration = 100;
    float lastPredUpdate = 0;

    /** Initiate VescUart class */
    VescUart UART;

    float current = 1.0; /** The current in amps */
    float currentBrake = 20.0;

    int firstDrive = 0;

    float lastPrint = 0;

    String driveStatus = "none";

    String controlMode = "arduino";
    boolean powerJump = false;
    String powerJumpPhase = "none";

  private:

};

#endif
