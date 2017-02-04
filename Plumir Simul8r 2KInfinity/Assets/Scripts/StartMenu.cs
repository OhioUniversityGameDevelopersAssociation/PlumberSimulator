/**********************************************
 * 
 * Andrew Decker
 * 
 * Used to drive the behavior of the main menu
 * for PS2K8
 * 
 * *******************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Get references to the different menu panels to just turn them off/on as needed
    public GameObject start, options, credits;
    public AudioMixer master;


    void Start()
    {
        // Make sure our menu is on the main panel when we launch
        start.SetActive(true);
        options.SetActive(false);
        credits.SetActive(false);
    }

    public void StartGame()
    {
        // Load the level
        Debug.Log("Starting Game");
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        // Give it a nice pause for the audio clip to play
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Suburban House");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void Options()
    {
        Debug.Log("Opening Options");
        start.SetActive(false);
        options.SetActive(true);
        credits.SetActive(false);
    }

    public void Main()
    {
        Debug.Log("Opening Start Menu");
        start.SetActive(true);
        options.SetActive(false);
        credits.SetActive(false);
    }

    public void Credits()
    {
        Debug.Log("Opening Credits");
        start.SetActive(false);
        options.SetActive(false);
        credits.SetActive(true);
    }

    public void SetMaster(float level)
    {
        // Used to adjust overall volume of game
        master.SetFloat("masterVol", level);
    }

    public void SetMusic(float level)
    {
        // used to adjust music volume of game
        master.SetFloat("musicVol", level);
    }

    public void SetSFX(float level)
    {
        // used to adjust SFX volume of game
        master.SetFloat("SFXVol", level);
    }
}