using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[] segments;
    //public int xPos = 0;
    public int tileWidth = 60;
    public int zPos = 60;
    public int secNum;

    public bool creatingSection = false;

    // Update is called once per frame
    void Update()
    {
        if (creatingSection == false)
        {
            creatingSection = true;
            StartCoroutine(CreateSection());
        }
    }

    IEnumerator CreateSection()
    {
        // Create a random section
        
        secNum = Random.Range(0, segments.Length);
        
        // Create a new section
        GameObject newSection = Instantiate(segments[Random.Range(0, segments.Length)], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += tileWidth;

        // Wait for a bit
        yield return new WaitForSeconds(4);

        // Create a new section
        creatingSection = false;
    }

}
