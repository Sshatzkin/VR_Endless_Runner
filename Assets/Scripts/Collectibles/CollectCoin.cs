using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource coinFX;

    // Start
    void Start()
    {
        audioObject = GameObject.Find("CoinCollect");
        coinFX = audioObject.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        CollectibleController.coinCount++;

        Destroy(gameObject);
        
    }
}
