using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class JumpController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float jumpRatio = 1f;

    public bool video = false;
    private float videoPowerLevelTimer = 5f;
    public float powerLevelBonus = 1f;
    public static float powerLevel = 0;
    public static float powerLevelTimer = 25f; // roughly the same as the detection zone
    private static float powerLevelCountdown = powerLevelTimer;


    public static float smashPower = 0;

    public TMP_Text powerLevelDisplay;

    public TMP_Text smashPowerDisplay;

    public float jumpHeight;

    public Transform vrCamera;

    //public Vector3 jump;
    public bool currentlyJumping;

    public int jumpState;

    public AudioSource runSFX;
    public AudioSource jumpSFX;

    bool jumpAudioPlaying = false;

    public static event Action<int> OnJumpStateChanged;
    public float jumpTimeElapsed = 0f;

    void Start() {
        if(video) {
            powerLevelTimer = videoPowerLevelTimer;
            powerLevelCountdown = powerLevelTimer;
            smashPower = 1;
        }
    }
    void Update()
    {

        // Set power level display text
        // // powerLevelDisplay.text = powerLevel.ToString();
        // Debug.Log("Timer: " + powerLevelTimer.ToString("00"));

        if (powerLevel == 0)
        {
            powerLevelDisplay.text = "OFF";
            powerLevelDisplay.color = Color.red;
            if(jumpRatio != 2.5f) {
                Debug.Log("Ratio: 2.5");
            }
            jumpRatio = 2.5F;
        }
        else
        {
            powerLevelDisplay.text = "0:" + powerLevelCountdown.ToString("00");//string.Format("{0:N2}", powerLevelTimer/100);
            powerLevelDisplay.color = Color.green;
            if(jumpRatio != 10) {
                Debug.Log("Ratio: 10");
                jumpTimeElapsed = 1;
            }
            jumpRatio = 10;
            jumpTimeElapsed -= Time.deltaTime;
            powerLevelCountdown -= Time.deltaTime;
            // reset timer and remove powerup
            if (powerLevelCountdown <= 0f)
            {
                powerLevel = 0;
                powerLevelCountdown = powerLevelTimer;
            }
        }

        // jumpRatio = powerLevel + 1;

        if (smashPower == 0)
        {
            smashPowerDisplay.text = "OFF";
            smashPowerDisplay.color = Color.red;
        }
        else
        {
            smashPowerDisplay.text = "ON";
            smashPowerDisplay.color = Color.green;
        }
    }

    public void updateJumpState(int newState)
    {
        jumpState = newState;

        switch (newState)
        {
            case 1:
                currentlyJumping = false;
                runSFX.Play();
                break;
            case 3:
                currentlyJumping = true;
                runSFX.Stop();
                jumpSFX.Play();
                break;
            case 9:
                currentlyJumping = false;
                runSFX.Play();
                break;
        }
        OnJumpStateChanged?.Invoke(newState);
    }


    // This function uses jump physics
    public void jump()
    {
        Debug.Log("Power Level: " + powerLevel);
        jumpRatio = 1 + (powerLevel * powerLevelBonus);
        Debug.Log("Jump Ratio: " + jumpRatio);

        float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * 9.81f * jumpRatio);
        GetComponent<Rigidbody>().AddForce(0, jumpSpeed, 0, ForceMode.VelocityChange);



        //GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * jumpRatio, ForceMode.Impulse);

    }


}


