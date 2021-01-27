using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Singleton design pattern */
    public static PlayerController instance;

    /* Components */    
    private Rigidbody rigidBody; // Rigidbody component of player's gameobject.
    private Animator animatorController; // The animator controller which has player's movement animations.

    /* Floats */
    private float SpeedFactor = 6f; // Movement speed multiplier of player's gameobject.

    /* Position */
    private Vector3 playerStartPosition; // Gameobject start position. Also our first checkpoint.

    /* String */
    public string playerName; // Nickname of player who is playing this game now.

    /* Boolens */
    private bool RespawningStatus; // Helps us to prevent the coroutine from running 2 times.

    private void Awake()
    {
        SingletonDesignPattern(); // One instance will be enough.
    }

    private void Start()
    {
        playerName = "SERHAT";
        playerStartPosition = transform.position; // Get start position for respawn position.
        GameController.instance.LastCheckpointPositionZ = transform.position.z;
        GameController.instance.allPlayers.Add(gameObject); // Add gameobject to allPlayers list.
        rigidBody = GetComponent<Rigidbody>(); // Get rigidbody at start.   
        animatorController = GetComponent<Animator>(); // Get animator component at start.
        animatorController.SetBool("Alive", true); // Player is alive at the start of the game.
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && GameController.instance.gameIsActive) // If player holds mouse left click.
        {
            ClampMousePosition(Input.mousePosition.x, Input.mousePosition.y); // scale mouse inputs according to screen resolution.
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0)) // If player not clicking anymore.
        {
            rigidBody.velocity = Vector3.zero; // Make velocity vector zero for instant stop.
            SetAnimatorVelocityParameters(0f, 0f); // Set animator parameters for idle animation.
        }
    }

    private void ClampMousePosition(float x, float y)
    {
        float mouseXclamp = Mathf.Clamp((x / Screen.width) * 2f - 1f, -1.0F, 1.0F);
        float mouseYclamp = Mathf.Clamp((y / Screen.height) * 4f - 1.5f, -1.0F, 1.0F); 
        SetAnimatorVelocityParameters(mouseXclamp, mouseYclamp); // Send it to blend tree.
        MovePlayer(mouseXclamp, mouseYclamp); // Change gameobject's position via AddForce.
    }
    private void SetAnimatorVelocityParameters(float x, float z) // Helper function to write animator parameters.
    {
        animatorController.SetFloat("VelocityX", x); // Our mouse x position.
        animatorController.SetFloat("VelocityZ", z); // Our mouse y position.
    }

    private void MovePlayer(float mouseX, float mouseY) // Add force to gameobject according to deltaVelocity.
    {
        Vector3 targetVelocity = SpeedFactor * new Vector3(mouseX, 0, mouseY); // Find targetVelocity by scaled mouse inputs. 
        Vector3 currentVelocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z); // Get our gameobject's current velocity vector without y.
        Vector3 deltaVelocity = (targetVelocity - currentVelocity); // Take the difference between this 2 vectors.
        rigidBody.AddForce(deltaVelocity, ForceMode.VelocityChange); // Add force in line with the vector we find.
    }

    private void OnCollisionEnter(Collision collision) // Collision detection.
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            animatorController.SetBool("Alive", false);
            GameController.instance.gameIsActive = false;
            if (!RespawningStatus)
            {
                StartCoroutine(DelayedRespawnPlayer(3.5f));
            }
        }
    }

    private void OnCollisionStay(Collision collision) // Rotating platform affects here.
    {
        if (collision.gameObject.CompareTag("RotatingPlatform"))
        {
            rigidBody.AddForce(new Vector3(-collision.gameObject.GetComponent<ObstacleController>().zAxisRotateSpeed* 75, 0, 0), ForceMode.Force); 
        }
    }

    private void OnTriggerEnter(Collider other) // Trigger detection.
    {
        if (other.gameObject.name.Contains("PaintTrigger"))
        {
            Destroy(other.gameObject); // We dont need this trigger anymore.
            UIController.instance.PercentageText.text = "PAINT THE WALL";
            UIController.instance.rankTexts.SetActive(false); // We dont need rank anymore. 
            GameController.instance.gameIsActive = false; // Parkour end.
            PaintController.instance.enabled = true; // Painting start.
            rigidBody.isKinematic = true; // We cant move anymore.
            SetAnimatorVelocityParameters(0f, 0f); // Set animator parameters for idle animation.
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            GameController.instance.LastCheckpointPositionZ = transform.position.z; // Update last checkpoint position.
        }
    }

    private IEnumerator DelayedRespawnPlayer(float delay)
    {
        RespawningStatus = true; // Coroutine start.
        yield return new WaitForSeconds(delay);
        animatorController.SetBool("Alive", true);  // From fallingDown animation to blend tree.
        SetAnimatorVelocityParameters(0f, 0f); // Set idle animation.
        transform.position = new Vector3(playerStartPosition.x, playerStartPosition.y, GameController.instance.LastCheckpointPositionZ); // transform player to last checkpoint position.
        GameController.instance.gameIsActive = true; // Player can move now.
        RespawningStatus = false; // Coroutine end.
    }

    private void SingletonDesignPattern()
    {
        if (PlayerController.instance == null)
        {
            PlayerController.instance = this;
        }
        else
        {
            if (PlayerController.instance != this)
            {
                Destroy(this);
            }
        }
    }
}
