using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _cameraOffset; // Vector difference between camera and player.
    [SerializeField] private Transform Player; // Player's transform component.

    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>(); // Get player's transform component on scene via its tag.
        _cameraOffset = transform.position - Player.position; // Calculate ofsett.
    }

    private void LateUpdate()
    {
        transform.position = Player.position + _cameraOffset ; // Follow player.
    }
}
