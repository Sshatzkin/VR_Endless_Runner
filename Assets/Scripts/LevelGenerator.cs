using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject player;
    public GameObject[] segments;
    public GameObject cookieSegment;
    public bool[] segment_flipped;
    //public int xPos = 0;
    public int tileWidth = 60;
    public int zPos = 60;
    public int secNum;
    public int segmentIndex = 0;


    public bool spawnCookieNext = false;
    public bool creatingSection = false;

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z % 60 >= 50 && creatingSection == false)
        {
            creatingSection = true;
            StartCoroutine(CreateSection());
        }

        if (Input.GetKeyDown(KeyCode.Return)) // "ENTER" key
        {
            spawnCookieNext = true;
        }
    }

    IEnumerator CreateSection()
    {
        // Create a random section

        //secNum = Random.Range(0, segments.Length);

        // Create a new section
        if (spawnCookieNext)
        {
            int x_offset = 8;
            int y_rotation = 180;
            GameObject newSection = Instantiate(cookieSegment, new Vector3(x_offset, 0, zPos), Quaternion.identity);
            newSection.transform.eulerAngles = new Vector3(0, y_rotation, 0);
            zPos += tileWidth;
            spawnCookieNext = false;
        }
        else
        {
            //int seg_number = Random.Range(0, segments.Length);
            int x_offset = 0;
            float y_rotation = 0;
            if (segment_flipped[segmentIndex])
            {
                x_offset += 8;
                y_rotation = 180.0f;
            }
            //Quaternion rotation = new Quaternion (0, y_rotation, 0, 1);

            GameObject newSection = Instantiate(segments[segmentIndex], new Vector3(x_offset, 0, zPos), Quaternion.identity);//rotation);
            newSection.transform.eulerAngles = new Vector3(0, y_rotation, 0);

            zPos += tileWidth;

            segmentIndex++;
        }


        // Wait for a bit
        yield return new WaitForSeconds(8);

        // Create a new section
        creatingSection = false;
    }

}
