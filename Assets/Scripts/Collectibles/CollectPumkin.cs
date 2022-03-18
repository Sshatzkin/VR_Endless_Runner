using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPumkin : MonoBehaviour
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
        CollectibleController.pumkinCount++;

        Destroy(gameObject);
        
    }
}
