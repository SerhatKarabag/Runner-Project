using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    /* Singleton design pattern */
    public static UIController instance;

    /* Image */
    public Image PercentageFillingImage; // Image will fill while we are painting the wall.
    
    /* TextMeshPro */
    public TextMeshProUGUI PercentageText; // Percentage of painted wall.


    /* Gameobjects */
    public GameObject Menu; // Menu panel
    public GameObject playButton; // Play and Next button.
    public GameObject rankTexts; // Rank texts parent gameobject. I use GetChild();.

    private void Awake()
    {
        SingletonDesignPattern(); // One instance will be enough.
    }

    private void Start()
    {
        FindGameObjects();
    }

    public void onClickPlayButton()
    {
        if (playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == "NEXT")
        {
            SceneManager.LoadSceneAsync("game");
        }
        else
        {
            foreach (var item in GameController.instance.allPlayers.Where(k => k.CompareTag("AIplayer")))
            {
                item.GetComponent<AIPlayerController>().agent.destination = item.GetComponent<AIPlayerController>().agentTarget.position; // AI players will start moving via playButton.
                item.GetComponent<AIPlayerController>().animatorController.SetBool("GameIsActive", true); // Runnig forward animation.
            }
            GameController.instance.gameIsActive = true; // Player can use swerwe mechanics now.
            Menu.SetActive(false);
        }
    }

    private void SingletonDesignPattern()
    {
        if (UIController.instance == null)
        {
            UIController.instance = this;
        }
        else
        {
            if (UIController.instance != this)
            {
                Destroy(this);
            }
        }
    }
    private void FindGameObjects()
    {
        Menu = GameObject.FindWithTag("Menu");
        playButton = GameObject.FindWithTag("Play");
        rankTexts = GameObject.FindWithTag("Rank");
    }
}
