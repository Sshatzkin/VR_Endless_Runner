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

    void Update()
    {

        // Set power level display text
        // // powerLevelDisplay.text = powerLevel.ToString();
        // Debug.Log("Timer: " + powerLevelTimer.ToString("00"));

        if (powerLevel == 0)
        {
            powerLevelDisplay.text = "OFF";
            powerLevelDisplay.color = Color.red;
            jumpRatio = 2.5F;
        }
        else
        {
            powerLevelDisplay.text = "0:" + powerLevelCountdown.ToString("00");//string.Format("{0:N2}", powerLevelTimer/100);
            powerLevelDisplay.color = Color.green;
            jumpRatio = 10;
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
                break;

            case 2:
                break;

            case 3:
                currentlyJumping = true;
                runSFX.Stop();
                jumpSFX.Play();
                break;

            case 8:
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


