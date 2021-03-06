﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 36;

    private bool didStartDeath = false;
    private bool didStartConsumed = false;
    public static int playerOneLevel = 1;

    public int playerOnePelletsConsumed = 0;


    public int totalPellets = 0;
    public int score = 0;
    public static int playerOneScore = 0;
    public static int playerOneHighScore = 0;
    
    public static int pacManLives = 3;

    public static bool isPlayerOneUp  = true;
    public bool shouldBlink = false;

    public float blinkIntervalTime = 0.1f;
    private float blinkIntervalTimer = 0;

    public AudioClip backgroundAudioNormal;
    public AudioClip backgroundAudioFrightened;
    public AudioClip backgroundAudioPacManDeath;
    public AudioClip consumedGhostAudioClip;

    public Sprite mazeBlue;
    public Sprite mazeWhite;

    public Text playerText;
    public Text readyText;

    public Text highScoreText;
    public Text playerOneUp;
    public Text playerOneScoreText;
    public Image playerLives2;
    public Image playerLives3;

    public Text consumedGhostScoreText;

    public GameObject[,] board = new GameObject[boardWidth,boardHeight];

    private bool didIncrementLevel = false;

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            
            if(o.name != "PacMan" && o.name != "Maze" && o.name != "Pellets" && o.name != "Nodes" && o.name != "None Nodes" && o.tag != "Maze" && o.name != "Game" && o.tag != "Ghost" && o.tag != "GhostHome" && o.name != "Canvas" && o.tag != "UIElements")
            {
                if(o.GetComponent<Tile>() != null)
                {
                    if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
                    {
                        totalPellets++;
                    }
                }
                board [(int)pos.x,(int)pos.y] = o;
            }
            else
            {
                //
            }
        }       

        if(isPlayerOneUp)
        {
            if(playerOneLevel == 1)
            {
                GetComponent<AudioSource>().Play();
                highScoreText.text = playerOneHighScore.ToString();
                pacManLives = 3;
            }
        }

        StartGame();
    }

    void Update()
    {
        UpdateUI();
        CheckPelletsConsumed();

        CheckShouldBlink();
    }

    void UpdateUI()
    {
        playerOneScoreText.text = playerOneScore.ToString();

        if(pacManLives ==3)
        {
            playerLives3.enabled = true;
            playerLives2.enabled = true;
        }
        else if(pacManLives ==2)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = true;
        }
        else if(pacManLives ==1)
        {
            playerLives3.enabled = false;
            playerLives2.enabled = false;
        }

    }

    void CheckPelletsConsumed()
    {
        if(isPlayerOneUp)
        {
            //-Player one is playing
            if((totalPellets-14) == playerOnePelletsConsumed)
            {
                PlayerWin(1);
            }
        }
    }

    void PlayerWin(int playerNum)
    {
        if(playerNum == 1)
        {
            if(!didIncrementLevel)
            {
                didIncrementLevel = true;
                playerOneLevel++;
            }
        }
        StartCoroutine(ProcessWin(2));
    }

    IEnumerator ProcessWin(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<PacMan>().canMove = false;
        pacMan.transform.GetComponent<Animator>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Animator>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }

        yield return new WaitForSeconds(delay);
        StartCoroutine(BlinkBoard(2));
    }

    IEnumerator BlinkBoard(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        //-Blink The Board
        shouldBlink = true;

        yield return  new WaitForSeconds(delay);

        //-Restart the game at the next level
        shouldBlink = false;
        StartNextLevel();
        
    }

    private void StartNextLevel()
    {
        SceneManager.LoadScene("Level1");
    }


    private void CheckShouldBlink()
    {
        if(shouldBlink)
        {
            if(blinkIntervalTimer < blinkIntervalTime)
            {
                blinkIntervalTimer += Time.deltaTime;
            }
            else
            {
                blinkIntervalTimer = 0;
                if(GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite == mazeBlue)
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeWhite;
                }
                else
                {
                    GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = mazeBlue;
                }

            }
        }
    }


    public void StartGame()
    {

        if(isPlayerOneUp)
        {
            StartCoroutine(StartBlinking(playerOneUp));
        }
        //Hide All Ghosts
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
            ghost.transform.GetComponent<Ghost>().canMove = false;
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
        pacMan.transform.GetComponent<PacMan>().canMove = false;

        StartCoroutine(ShowObjectsAfter(2.25f));
    }

    public void StartConsumed(Ghost consumedGhost)
    {
        if(!didStartConsumed)
        {
            didStartConsumed = true;
            //-Pause All the Ghosts
            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }
            //-Pause Pacman
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove=false;

            //-Hide Pacman
            pacMan.transform.GetComponent<SpriteRenderer>().enabled=false;

            //-Hide Consumed Ghost
            consumedGhost.transform.GetComponent<SpriteRenderer>().enabled=false;

            Vector2 pos = consumedGhost.transform.position;

            Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

            consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
            consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

            consumedGhostScoreText.GetComponent<Text>().enabled = true;

            //-Play the consumed sound
            transform.GetComponent<AudioSource>().PlayOneShot(consumedGhostAudioClip);

            //-Wait for Audio Clip to finish
            StartCoroutine(ProcessConsumedAfter(0.75f,consumedGhost));

        }
    }

    IEnumerator StartBlinking(Text blinkText)
    {
        yield return new WaitForSeconds(0.25f);

        blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
        StartCoroutine(StartBlinking(blinkText));
    }

    IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost)
    {
        yield return new WaitForSeconds(delay);

        //-Hide the score
         consumedGhostScoreText.GetComponent<Text>().enabled = false;

        //-Show Pacman
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled=true;
        
        //-Show Consumed Ghost
        consumedGhost.transform.GetComponent<SpriteRenderer>().enabled=true;

        //-Resume Ghost
        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        //-Resume PacMan
        pacMan.transform.GetComponent<PacMan>().canMove=true;

        didStartConsumed = false;


    }   
    IEnumerator ShowObjectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        
        playerText.transform.GetComponent<Text>().enabled=false;
        StartCoroutine(StartGameAfter(2));

    }

    IEnumerator StartGameAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().canMove = true;
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().canMove = true;

        readyText.transform.GetComponent<Text>().enabled=false;

    }

    public void StartDeath()
    {
        if(!didStartDeath)
        {
        
            StopAllCoroutines();

            if(GameMenu.isOnePlayerGame)
            {
                playerOneUp.GetComponent<Text>().enabled = true;
            }
            
            didStartDeath = true;

            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
            {
                ghost.transform.GetComponent<Ghost>().canMove = false;
            }

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<PacMan>().canMove = false;

            pacMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeathAfter(2));

        }
    }

    IEnumerator ProcessDeathAfter(float delay) //We can process these methods to run without locking up the rest of the game. Run Parallel to other processes
    {
        yield return new WaitForSeconds(delay);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
                ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(1.9f));

    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pacMan = GameObject.Find("PacMan");

        pacMan.transform.localScale = new Vector3(1,1,1);
        pacMan.transform.localRotation = Quaternion.Euler(0,0,0);

        pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().deathAnimation;
        pacMan.transform.GetComponent<Animator>().enabled=true;

        transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(1));

    }

    IEnumerator ProcessRestart(float delay)
    {
        pacManLives -= 1;

        if(pacManLives == 0)
        {
            if(playerOneHighScore < playerOneScore)
            {
                playerOneHighScore = playerOneScore;
            }
            playerOneScore = 0;

            playerText.transform.GetComponent<Text>().enabled = true;
            
            readyText.transform.GetComponent<Text>().text = "GAME OVER";
            readyText.transform.GetComponent<Text>().color = Color.red;

            
            readyText.transform.GetComponent<Text>().enabled = true;

            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
            
            transform.GetComponent<AudioSource>().Stop();
            
            StartCoroutine(ProcessGameOver(2));
        }
        else 
        {
            playerText.transform.GetComponent<Text>().enabled = true;
            readyText.transform.GetComponent<Text>().enabled = true;
            
            GameObject pacMan = GameObject.Find("PacMan");
            pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
            
            transform.GetComponent<AudioSource>().Stop();

            yield return new WaitForSeconds(delay);

            StartCoroutine(ProcessRestartShowObjects(1));
        }
    }

    IEnumerator ProcessGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("GameMenu");
    }

    IEnumerator ProcessRestartShowObjects(float delay)
    {
        playerText.transform.GetComponent<Text>().enabled = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
            ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
        }

        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<Animator>().enabled=false;
        pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();


        yield return new WaitForSeconds(delay);

        Restart();
    }

    public void Restart()
    {

        readyText.transform.GetComponent<Text>().enabled=false;
        
        GameObject pacMan = GameObject.Find("PacMan");
        pacMan.transform.GetComponent<PacMan>().Restart();

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in o)
        {
            ghost.transform.GetComponent<Ghost>().Restart();
        }
        didStartDeath = false;
    }

}
