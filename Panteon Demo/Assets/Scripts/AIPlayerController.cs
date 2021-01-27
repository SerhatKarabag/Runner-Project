using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayerController : MonoBehaviour
{
    /* Strings */
    public string AiplayerName; // Nickname of AI player.

    /* Components */
    public NavMeshAgent agent; // Navmesh component of gameobject.
    public Animator animatorController; // The animator controller which has AI's movement animations.

    /* Agent destination */
    public Transform agentTarget; // Parkour finish point.

    /* Booleans */
    private bool RespawningStatus; // Helps us to prevent the coroutine from running 2 times.

    /* Position */
    private Vector3 AIstartPosition; // Gameobject start position. Also their first checkpoint.
    private float lastCheckpointZ; // When checkpoint trigger works, this value updates.
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get navmesh component.
        animatorController = GetComponent<Animator>(); // Get animator component.
        agentTarget = GameObject.Find("AgentTarget").GetComponent<Transform>(); // Find navmesh destination on scene by its name.
        GameController.instance.allPlayers.Add(gameObject); // Add this gameobject to all players list.
        AIstartPosition = transform.position; // Take start position.
        lastCheckpointZ = transform.position.z; // Update checkpoint. Start position is our first chekpoint position at the sama time.
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle")) // Collided gameobject's tag is obstacle
        {
            animatorController.SetBool("canRun", false); // exit run animation, enter idle animation
            agent.enabled = false; // disable agent until respawned.
            if (!RespawningStatus) // Check if coroutine is working
            {
                StartCoroutine(DelayedRespawnAI(3.5f)); // transform player to last checkpoint position. Enable agent. Start running. 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint")) // When checkpoint triggered
        {
            lastCheckpointZ = transform.position.z; // Update checkpoint. Z parameter is enough for us. Cause we are moving forward on Z direction.  
        }
    }

    private IEnumerator DelayedRespawnAI(float delay)
    {
        RespawningStatus = true;
        yield return new WaitForSeconds(delay);
        transform.position = new Vector3(AIstartPosition.x, AIstartPosition.y, lastCheckpointZ); // transform player to last checkpoint position.
        animatorController.SetBool("canRun", true); // Start running.
        agent.enabled = true; // Go for target.
        agent.destination = agentTarget.position; // Set target.
        RespawningStatus = false; // Coroutine end status. 
    }
}
