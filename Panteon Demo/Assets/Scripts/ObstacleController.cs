using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField]
    private enum ObstacleType // Type of obstacle. Manages movements of obstacle.
    {
        Horizontal,
        Vertical,
        RotatingY,
        RotatingPlatform
    }

    /* Floats */
    [SerializeField] private float targetHorizontalPosition; // The target x axis of horizontally moving objects.
    [SerializeField] private float speedFactor; // Speed multiplier.
    [SerializeField] private float rotateDirectionChangeTime; // The total time of rotating one direction. 
    public float zAxisRotateSpeed; //Rotate speed of "rotating platform". This value will effect on player while player is waking on rotating platform. 

    /* Positions */
    private Vector3 startPosition; // First position of gameobject.
        
    [SerializeField] private bool startRotatingtoLeft; // If true, gameobject will rotate left first.

    /*Enum*/
    [SerializeField] private ObstacleType Type; // Type of obstacle.

    void Start()
    {
        startPosition = transform.position;
        rotateDirectionChangeTime = 3 / rotateDirectionChangeTime;
    }

    void FixedUpdate()
    {
        if (Type == ObstacleType.Horizontal) // If this is horizontal obstacle.
        {
            float lerpInterpolant = Mathf.PingPong(Time.timeSinceLevelLoad * speedFactor, 1); // PingPong returns a value that will increment and decrement between the value 0 and length.
            transform.position = Vector3.Lerp(new Vector3(startPosition.x, transform.position.y, transform.position.z),new Vector3(targetHorizontalPosition, transform.position.y, transform.position.z), lerpInterpolant); // Transform process.
        }
        else if(Type == ObstacleType.RotatingY) // If this gameobject rotates around y axis.
        {
            transform.Rotate(new Vector3(0, Mathf.Sin(Time.timeSinceLevelLoad) * speedFactor, 0)); // Sin helps us to make shake effect between -1 and 1.
        }
        else // Rotating platform.
        {
            zAxisRotateSpeed = Mathf.Sin(Time.timeSinceLevelLoad * rotateDirectionChangeTime) * speedFactor;
            transform.Rotate(new Vector3(0, 0, zAxisRotateSpeed));
        }
    }
}
