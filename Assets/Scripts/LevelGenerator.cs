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
    private static int[] segmentOrder = new int[] {1, 2, 5, 6, 3, 7, 8, 4, 1, 2, 5, 6, 3, 7, 8, 4}; // order for auto generator


    public int interruptSegmentFor = 0;
    private int newSegmentIndex;
    public bool creatingSection = false; // stall the auto generator

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.z % 60 >= 50 && creatingSection == false)
        {
            creatingSection = true;
            StartCoroutine(CreateSection());
        }

        if (Input.GetKeyDown(KeyCode.Keypad0)) // "0" key = cookie
        {
            interruptSegmentFor = 1;
            newSegmentIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1)) // "1" key = strip 1
        {
            interruptSegmentFor = 1;
            newSegmentIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) // "2" key = strip 2
        {
            interruptSegmentFor = 1;
            newSegmentIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) // "3" key = wind - strip 3
        {
            interruptSegmentFor = 1;
            newSegmentIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) // "4" key = mud landing - strip 4
        {
            interruptSegmentFor = 1;
            newSegmentIndex = 4;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5)) // "5" key = jump boost strips, combines strip 5 and 6
        {
            interruptSegmentFor = 2;
            newSegmentIndex = 5;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6)) // "6" key = pumpkin strips, combines 7 and 8
        {
            interruptSegmentFor = 2;
            newSegmentIndex = 7;
        }
    }

    IEnumerator CreateSection()
    {
        // Create a random section

        //secNum = Random.Range(0, segments.Length);

        // Create a new section
        if (interruptSegmentFor > 0)
        {
            int x_offset = 0;
            float y_rotation = 0;
            if (segment_flipped[newSegmentIndex])
            {
                x_offset += 8;
                y_rotation = 180.0f;
            }

            GameObject newSection = Instantiate(segments[newSegmentIndex], new Vector3(x_offset, 0, zPos), Quaternion.identity);
            newSection.transform.eulerAngles = new Vector3(0, y_rotation, 0);

            zPos += tileWidth;
            interruptSegmentFor--;
            newSegmentIndex++;

        }
        else // runs normal order
        {
            //int seg_number = Random.Range(0, segments.Length);
            int x_offset = 0;
            float y_rotation = 0;
            if (segment_flipped[segmentOrder[segmentIndex]])
            {
                x_offset += 8;
                y_rotation = 180.0f;
            }
            //Quaternion rotation = new Quaternion (0, y_rotation, 0, 1);

            GameObject newSection = Instantiate(segments[segmentOrder[segmentIndex]], new Vector3(x_offset, 0, zPos), Quaternion.identity);//rotation);
            newSection.transform.eulerAngles = new Vector3(0, y_rotation, 0);

            zPos += tileWidth;

            segmentIndex++;
        }


        // Wait for a bit
        yield return new WaitForSeconds(8);
        // yield return true;

        // Create a new section
        creatingSection = false;
    }

}
