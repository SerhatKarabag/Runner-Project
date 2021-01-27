using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /* Singleton design pattern */
    public static GameController instance;

    /* Booleans */
    public bool gameIsActive; // decides parkour completed or not. 

    /* Floats */
    public float LastCheckpointPositionZ; // player's last checkpoint z value. Maybe last level value can be saved on this script, while there is another level.

    /* Lists */
    public List<GameObject> allPlayers; // List of all active players on scene.  

    private void Awake()
    {
        SingletonDesignPattern(); // One instance will be enough.
    }
    private void Start()
    {
        gameIsActive = false; // The game has not started yet.
    }

    private void LateUpdate()
    {
        if (gameIsActive) // If player didnt triggered the wall trigger yet.
        {
            CalculateRank(); // Calculates rank via z axis. Then writes it top left of the screen.
        }        
    }

    private void SingletonDesignPattern() // Singleton 
    {
        if (GameController.instance == null)
        {
            GameController.instance = this;
        }
        else
        {
            if (GameController.instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void CalculateRank() // Calculates rank via z axis. Then writes it top left of the screen.
    {
        allPlayers = allPlayers.OrderByDescending(k => k.transform.position.z).ToList(); // order allPlayers list  by descending according to z axis.
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].CompareTag("Player")) // If this is player, get the name from player's script.
            {
                UIController.instance.rankTexts.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = (i+1) + "-" + allPlayers[i].GetComponent<PlayerController>().playerName;
            }
            else // If this is not player, get the name from AI's script.
            {
                UIController.instance.rankTexts.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = (i + 1) + "-" + allPlayers[i].GetComponent<AIPlayerController>().AiplayerName;
            }
        }
    }
}
