using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class JumpController : MonoBehaviour
{
  public float jumpForce = 10f;
  public bool VRMode = false;
  public float jumpRatio = 1f;

  public float powerLevelBonus = 0.2f;
  public static float powerLevel = 0;

  public Text powerLevelDisplay;

  void Update(){
    //powerLevel = Mathf.Floor(CollectibleController.coinCount / 5);

    // Set power level display text
    powerLevelDisplay.text = "Power Level: " + powerLevel.ToString();
  }

  public void jump()
  {
    Debug.Log("Power Level: " + powerLevel);
    jumpRatio = 1 + (powerLevel * powerLevelBonus);
    Debug.Log("Jump Ratio: " + jumpRatio);


    if (VRMode)
    {
      // VR Jump things

    }
    else{
      // Desktop Jump things
      
      

      // Apply Vertical Force
      GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce * jumpRatio, ForceMode.Impulse);
    }
  }


}
