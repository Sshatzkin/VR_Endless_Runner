using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleController : MonoBehaviour
{
    public static int coinCount = 0;

    public Text coinCountDisplay;

    // Update is called once per frame
    void Update()
    {

        coinCountDisplay.text = "Cookies: " + coinCount.ToString();

    }
}
