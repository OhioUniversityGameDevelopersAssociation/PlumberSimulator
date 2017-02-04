/******************************************
 * 
 * Andrew Decker
 * 
 * Used to run game cycle. Grabs references
 * to all pipes, selects random pipes to go
 * off, controls water level, keeps score,
 * kills player at the end of a round, calls
 * function to stop all pipe flows at the end 
 * of game, handles any UI/UX
 * 
 * ***************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // All of the possible pipe bursts
    public Burst[] burstPoints;

    // Countdown UI at beginning of game
    public float imagePopSpeed, imagePauseSpeed;
    public Image alert;
    public Sprite[] countdownHands;

    // Transform of the water and the brown image we are using as a filter effect
    public Transform water, waterPlane;

    // Score multipliers for pipe and seconds lived respectively
    public int fixPipeScoreValue = 100, timeScoreValue = 10;

    // Reference to the opening and then the looping music
    public AudioClip leadIn, music;

    // Reference to the clipboard menu and the lamp (to kill it)
    public Animator clipboard, lamp;

    // Results text
    public Text leaksPlugged, timeSurvived, scoreText;

    // Used to "grade" the player
    public Sprite[] ratingImages;
    public Image rating, mark;

    // Audio source for the game music
    AudioSource source;

    // keep track of various game counters
    int burstPipeCount, maxBurstCount, currentBurstIndex, pipesFixed, score, evenFlowCount;
    float minTimeInterval, maxTimeInterval, waterLevel, rateDivisor, startTime, endTime;

    // Property for the water level that can't be altered by other scripts
    public float WaterLevel { get { return waterLevel; } }

    // We don'thave time to put in a menu, so we'll just leave this on hard, maybe put in a difficulty selector later
    string difficulty;

    // Endgame misc bools
    bool gameOver = false, lampDead = false;

    // Used to track keys in use so one key can't fix multiplepipes
    [HideInInspector]
    public List<string> inUse;

    // Event to stop the player from being able to fix the pipes at the end of the game
    public delegate void StopBurst();
    public static event StopBurst StopNow;

    // Reference to the crayon cursor at the end
    public GameObject crayon;


    // Use this for initialization
    void Start()
    {
        // Reset all the variables
        source = GetComponent<AudioSource>();
        startTime = Time.time;
        burstPipeCount = 0;
        pipesFixed = 0;
        difficulty = "Hard";

        // Start game loop
        StartCoroutine(BeginGame());
        // Set rate of pipe burst variables
        ChangeDifficulty();
    }

    void Update()
    {
        // if there are literally any pipes leaking ..
        if (burstPipeCount != 0)
        {
            // .. raise the water
            waterLevel += (2.207f * (burstPipeCount / rateDivisor)) * Time.deltaTime;
        }

        // make sure the water isn't going lower than 0
        if (waterLevel <= 0)
            waterLevel = 0;

        // change the waters position
        water.position = new Vector3(water.position.x, waterLevel, water.position.z);

        // Game over if the water is above the cameras halfway point
        if (waterLevel >= 2.207f && gameOver == false)
        {
            GameOver();
        }

        // Used to make short post-game animation for lamp play
        if (waterLevel >= 2.94f && lampDead == false)
            lamp.SetTrigger("Kill");

        // Stops the water from ascending forever
        if (waterLevel >= 3.5f)
            waterLevel = 3.5f;
    }

    // Intro to game
    IEnumerator BeginGame()
    {
        // Set up audio and countdown
        source.PlayOneShot(leadIn);
        yield return new WaitForSeconds(2.2f);
        alert.enabled = true;
        StartCoroutine(PopImage(countdownHands[3]));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PopImage(countdownHands[2]));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PopImage(countdownHands[3]));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PopImage(countdownHands[0]));

        // Start the main game loop
        StartCoroutine(BurstLoop());

        // Turn off the alers at the end of the sound and replace with main music
        yield return new WaitForSeconds(0.47f);
        alert.enabled = false;
        source.loop = true;
        source.clip = music;
        source.Play();
    }

    // Sets variables for the rate of pipes bursting depending on difficulty
    public void ChangeDifficulty()
    {
        switch (difficulty)
        {
            case "Easy":
                maxBurstCount = 5;
                minTimeInterval = 2.0f;
                maxTimeInterval = 2.5f;
                rateDivisor = 150f;
                evenFlowCount = 2;
                break;
            case "Normal":
                maxBurstCount = 8;
                minTimeInterval = 1.5f;
                maxTimeInterval = 2.0f;
                rateDivisor = 300f;
                evenFlowCount = 3;
                break;
            case "Hard":
                maxBurstCount = 12;
                minTimeInterval = 1.0f;
                maxTimeInterval = 1.5f;
                rateDivisor = 450f;
                evenFlowCount = 4;
                break;
        }
    }

    // Fires when a pipe is fixed
    public void DecrementPipeBursts()
    {
        burstPipeCount--;
        pipesFixed++;
    }

    // Main Game loop
    IEnumerator BurstLoop()
    {
        // Loop endlessly (until the player dies)
        while (true)
        {
            // If we aren't at our limit for how many pipes the player has to face at once ..
            if (burstPipeCount < maxBurstCount)
            {
                // set a flag that we haven't yet found a pipe that willwork
                bool pipeHasBurst = false;
                do
                {
                    // Get random pipe reference via random index
                    Burst targetPipe = burstPoints[Random.Range(0, burstPoints.Length)];

                    // if this pipe in not currently active ..
                    if (targetPipe.burst == false)
                    {
                        // .. Make it so and set flag to move on
                        targetPipe.StartBurst();
                        burstPipeCount++;
                        pipeHasBurst = true;
                    }
                } while (pipeHasBurst == false);
            }

            // Wait before bursting the next pipe
            float timeInterval = Random.Range(minTimeInterval, maxTimeInterval);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    // Make the countdown image pop out
    IEnumerator PopImage(Sprite image)
    {
        alert.overrideSprite = image;
        float startTime = Time.time;
        while (Time.time < startTime + imagePopSpeed)
        {
            alert.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / imagePopSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(imagePauseSpeed);
        startTime = Time.time;
    }

    // Fires when the water level gets too high
    public void GameOver()
    {
        // set a flag so this only runs once
        gameOver = true;

        // Flip the water upside down so normals look correct
        waterPlane.localRotation = Quaternion.Euler(180, 0, 0);

        // Scorn the player through the editor they'll never see
        Debug.Log("You and everyone you have ever loved, have drowned");

        // Mark when the player dies for score purposes
        endTime = Time.time - startTime;

        // if the player tried literally at all ..
        if (pipesFixed != 0)
        {
            // .. give them a legit score
            score = pipesFixed * fixPipeScoreValue + (int)(endTime * timeScoreValue);
        }
        else // .. othewise ..
        {
            // .. Take even their time score away for being lazy as hell
            score = 0;
        }

        // Calculate player score and display on clipboard
        leaksPlugged.text = pipesFixed.ToString();
        string minutes = Mathf.Floor(Time.time / 60).ToString();
        string seconds = Mathf.Floor(Time.time % 60).ToString();
        timeSurvived.text = minutes + ":" + seconds;
        scoreText.text = score.ToString();
        DetermineMarks();

        // Bring the clipboard into frame
        clipboard.SetTrigger("GameOver");

        // Turn the cursor on
        crayon.SetActive(true);

        // Stop all the pipes from running
        StopNow();
    }

    // Assign grade based on score
    void DetermineMarks()
    {
        // player gets an F- if they don't even try
        if (score == 0) // F-
        {
            rating.overrideSprite = ratingImages[4];
            mark.overrideSprite = ratingImages[7];
            return;
        }
        else if (score > 0 && score <= 1000) // F
        {
            rating.overrideSprite = ratingImages[4];
            mark.overrideSprite = ratingImages[6];
            return;
        }
        else if (score > 1000 && score <= 1500) // D-
        {
            rating.overrideSprite = ratingImages[3];
            mark.overrideSprite = ratingImages[7];
            return;
        }
        else if (score > 1500 && score <= 2000) // D
        {
            rating.overrideSprite = ratingImages[3];
            mark.overrideSprite = ratingImages[6];
            return;
        }
        else if (score > 2000 && score <= 2500) // D+
        {
            rating.overrideSprite = ratingImages[3];
            mark.overrideSprite = ratingImages[5];
            return;
        }
        else if (score > 2500 && score <= 3000) // C-
        {
            rating.overrideSprite = ratingImages[2];
            mark.overrideSprite = ratingImages[7];
            return;
        }
        else if (score > 3000 && score <= 3500) // C
        {
            rating.overrideSprite = ratingImages[2];
            mark.overrideSprite = ratingImages[6];
            return;
        }
        else if (score > 3500 && score <= 4000) // C+
        {
            rating.overrideSprite = ratingImages[2];
            mark.overrideSprite = ratingImages[5];
            return;
        }
        else if (score > 4000 && score <= 4500) // B-
        {
            rating.overrideSprite = ratingImages[1];
            mark.overrideSprite = ratingImages[7];
            return;
        }
        else if (score > 4500 && score <= 5000) // B
        {
            rating.overrideSprite = ratingImages[1];
            mark.overrideSprite = ratingImages[6];
            return;
        }
        else if (score > 5000 && score <= 5500) // B+
        {
            rating.overrideSprite = ratingImages[1];
            mark.overrideSprite = ratingImages[5];
            return;
        }
        else if (score > 5500 && score <= 6000) // A-
        {
            rating.overrideSprite = ratingImages[0];
            mark.overrideSprite = ratingImages[7];
            return;
        }
        else if (score > 6000 && score <= 6500) // A
        {
            rating.overrideSprite = ratingImages[0];
            mark.overrideSprite = ratingImages[5];
            return;
        }
        else if (score > 6500) // A+
        {
            rating.overrideSprite = ratingImages[0];
            mark.overrideSprite = ratingImages[5];
            return;
        }
    }

    // Reload scene when player selects "new game"
    public void Restart()
    {
        SceneManager.LoadScene("Suburban House");
    }

    // To main menu, only accessible from death screen
    public void BackToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}