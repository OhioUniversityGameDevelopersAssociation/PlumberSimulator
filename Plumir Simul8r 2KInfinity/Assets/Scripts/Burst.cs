/**************************************
 * 
 * Created by Andrew Decker
 * 
 * We will be attaching this to all of the pipes
 * that water can burst from.
 * 
 * ***********************************/

using UnityEngine;
using UnityEngine.UI;

public class Burst : MonoBehaviour
{
    // Is this leak currently actie?
    public bool burst;

    // We will use these to switch the effect when the leak is happenning underwater
    public GameObject aboveWaterParticles, belowWaterParticles;
    public ParticleSystem water;

    // Used to represent the progress in fixing the leak
    public Image healthImage, baseImage;


    // Reference to the Game Controller
    GameController gc;

    // The Text object used to display which key needed to plug hole
    Text keyText;

    // Store what key needs to be pressed to fix the leak
    string key;

    void Start()
    {
        // Grab necessary references
        gc = FindObjectOfType<GameController>();
        keyText = GetComponentInChildren<Text>();
        // Stop the water particles from flowing at the beginning of the game
        water.Pause();
    }

    public void OnEnable()
    {
        // Follow the delegate event from the game controller
        GameController.StopNow += StopBurst;
    }

    public void OnDisable()
    {
        // Unfollow the delegate event from the game controller
        GameController.StopNow -= StopBurst;
    }

    void FixedUpdate()
    {
        // if we are currently active ..
        if (burst)
        {
            // .. have the health image represent the 
            Color healthImageColor = Color.Lerp(Color.red, Color.green, healthImage.fillAmount);
            healthImage.color = healthImageColor;

            // if the water is now above the leak and the correct particles aren't on ..
            if (gc.WaterLevel > transform.position.y && aboveWaterParticles.activeSelf == true)
            {
                // .. flip them off
                aboveWaterParticles.SetActive(false);
                belowWaterParticles.SetActive(true);
            }
            else if (gc.WaterLevel < transform.position.y && aboveWaterParticles.activeSelf == false)
            {
                // .. or reset it going the other way
                aboveWaterParticles.SetActive(true);
                belowWaterParticles.SetActive(false);
            }

            // if we're inputing the correct input ..
            if (Input.GetKey(key))
            {
                // .. fix the leak
                healthImage.fillAmount += 0.01f;
            }
            else
            {
                // .. if not, return the leak to it's damaged state
                healthImage.fillAmount -= 0.01f;
            }
        }

        // if the leak is fully fixed ..
        if (healthImage.fillAmount == 1.0f && burst)
        {
            // .. make it appear so and give the player some extra score
            gc.DecrementPipeBursts();
            gc.inUse.Remove(key);
            baseImage.color = Color.clear;
            healthImage.color = Color.clear;
            keyText.color = Color.clear;
            aboveWaterParticles.SetActive(false);
            belowWaterParticles.SetActive(false);
            burst = false;
        }
    }

    // Used when game is over and we want leaks to stop being able to be fixed, 
    // Different from player fix because it keeps the particle systems on to rub it in their loser faces
    public void StopBurst()
    {
        // Turn off health
        baseImage.color = Color.clear;
        healthImage.color = Color.clear;
        for (int i = 0; i < gc.inUse.Count; i++)
        {
            // Return the fix key to the pool of fix keys
            if (gc.inUse[i] == key)
            {
                gc.inUse.RemoveAt(i);
            }
        }
        keyText.color = Color.clear;
        key = "-";
    }

    // Called by GameController to start the leak
    public void StartBurst()
    {
        // Reset health
        healthImage.fillAmount = 0.0f;
        baseImage.color = Color.gray;
        bool alreadyInUse;
        // find a key that isn't being used
        // TODO: there should be a more effective way to do this
        do
        {
            alreadyInUse = false;
            // Grab a random key from our utility script
            key = Utility.KeyGen();
            for (int i = 0; i < gc.inUse.Count; i++)
            {
                // if we're using it anywhere else
                if (key == gc.inUse[i])
                {
                    // Stop and grab another one
                    alreadyInUse = true;
                    break;
                }
            }
        } while (alreadyInUse);
        gc.inUse.Add(key);

        // Display the key
        keyText.color = Color.black;
        keyText.text = key.ToUpper();

        // Start the leaky pipe loop
        burst = true;
    }
}
