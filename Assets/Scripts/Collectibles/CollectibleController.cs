using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectibleController : MonoBehaviour
{
    public static int coinCount = 0;

    public static int pumpkinCount = 0;

    public TMP_Text coinCountDisplay;

    public TMP_Text pumpkinCountDisplay;

    // Update is called once per frame
    void Update()
    {

        coinCountDisplay.text = coinCount.ToString();
        pumpkinCountDisplay.text = pumpkinCount.ToString();

    }
}
