using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[] segments;
    public bool[] segment_flipped;
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
        int seg_number = Random.Range(0, segments.Length);
        int x_offset = 0;
        float y_rotation = 0;
        if (segment_flipped[seg_number]){
            x_offset += 8;
            y_rotation = 180.0f;
        }
        //Quaternion rotation = new Quaternion (0, y_rotation, 0, 1);
        
        GameObject newSection = Instantiate(segments[seg_number], new Vector3(x_offset, -0.6f, zPos), Quaternion.identity);//rotation);
        newSection.transform.eulerAngles = new Vector3(0, y_rotation, 0);

        zPos += tileWidth;

        // Wait for a bit
        yield return new WaitForSeconds(4);

        // Create a new section
        creatingSection = false;
    }

}
