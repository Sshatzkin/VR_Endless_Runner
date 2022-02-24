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

  public TMP_Text powerLevelDisplay;

  public float jumpHeight;

  void Update(){
    //powerLevel = Mathf.Floor(CollectibleController.coinCount / 5);

    // Set power level display text
    powerLevelDisplay.text = powerLevel.ToString();
  }

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
