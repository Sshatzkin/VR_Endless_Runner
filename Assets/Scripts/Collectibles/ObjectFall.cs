using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    [SerializeField, Tooltip("The amount to rotate the object by each frame once falling begins")]
    private float deltaRotation = 10;
    [SerializeField, Tooltip("The offset for position of the rotation point in case the calculated rotation point is inside of the ground")]
    private Vector3 deltaPosition;
    [SerializeField, Tooltip("The forwards velocity to knock the object so it remains visible without looking backwards")]
    private float velocity;
    [SerializeField, Tooltip("The amount to decellerate the object by each frame")]
    private float decelleration;
    private Vector3 pivotPoint; // The rotation point of the object
    private bool falling = false;
    private float rotation = 0;
    // Start is called before the first frame update
    private void Start()
    {
        pivotPoint = transform.position;
        pivotPoint.y -= transform.localScale.y / 2;
        pivotPoint += deltaPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        falling = true;
    }

    private void FixedUpdate() {
        if(falling && rotation < 90) {
            float currentRotation = rotation + deltaRotation > 90 ? 90 - rotation : deltaRotation;
            transform.RotateAround(pivotPoint, Vector3.right, deltaRotation);
            rotation += deltaRotation;
            float deltaZ = velocity * Time.fixedDeltaTime;
            velocity = Mathf.MoveTowards(velocity, 0, decelleration * Time.fixedDeltaTime);
            pivotPoint = new Vector3(pivotPoint.x, pivotPoint.y, pivotPoint.z + deltaZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + deltaZ);
        }
    }
}
