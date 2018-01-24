using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public GameObject manager; // holds the global game manager
    public Image DamageFlash; // the ui element that does the damage flash
    public float InitialHealth = 100f; // starting health
    public float CurrentHealth; // current health
    public float InitialEnergy = 100f; // initial energy
    public float CurrentEnergy; // current energy
    public float EnergyRegenerationRate = 1f; // how often energy should regenerate
    public float EnergyRegenerationAmount = 1f; // how much the energy should regenerate by
    // note that i don't have a regeneration rate for health as i want to encourage the player to explore the terrain and keep them active
    public bool Alive = true; // quite self explanatory - used globally to check the player's state
    public float LerpTime = 0.05f; // the time for lerping on the UI
    public Image HealthBar; // health bar on hud
    public Text HealthText; // text stating health
    public Image EnergyBar; // energy bar on hud
    public Text EnergyText; // text stating energy

    // the following are just variables to control lerping
    float healthLerp;
    float oldHealthLerp;
    float desiredHealthLerp;
    bool healthLerping;
    float currentHealthTime;

    float energyLerp;
    float oldEnergyLerp;
    float desiredEnergyLerp;
    bool energyLerping;
    float currentEnergyTime;

    private float nextEnergyIncrement = 0f; // the amount of time remaining before the energy should be increased again
    public void DecreaseHealth(float amount = 0f) // public method to decrease health
    {
        DamageFlash.GetComponent<DamageFlash>().Flash(); // starts the damage flash
        CurrentHealth -= amount; // decrease health
        if (CurrentHealth < 0) // if the health is negative
        {
            CurrentHealth = 0; // set the health to 0
        }
    }
    public void IncreaseHealth(float amount = 0f) // the method is quite self explanatory, and follows the same structure as the one above
    {
        CurrentHealth += amount;
        if (CurrentHealth > InitialHealth)
        {
            CurrentHealth = InitialHealth;
        }
    }
    public void DecreaseEnergy(float amount = 0f) // the method is quite self explanatory, and follows the same structure as the one above
    {
        CurrentEnergy -= amount;
        if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;
        }
    }
    public void IncreaseEnergy(float amount = 0f) // the method is quite self explanatory, and follows the same structure as the one above
    {
        CurrentEnergy += amount;
        if (CurrentEnergy > InitialEnergy)
        {
            CurrentEnergy = InitialEnergy;
        }
    }
    void Start()
    {
        CurrentHealth = InitialHealth; // set the current health to initial
        CurrentEnergy = InitialEnergy; // same as above
    }
    void Update()
    {
        // TODO: REMOVE THESE DEBUGGING FUNCTIONS
        if (Input.GetKeyDown(KeyCode.U))
        {
            DecreaseHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            IncreaseHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DecreaseEnergy(10);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            IncreaseEnergy(10);
        }
        if (CurrentHealth <= 0f && Alive) // check that the player is alive, and run the death routine if the health is at or below 0
        {
            Die();
        }
        if (nextEnergyIncrement > 0) // if the energy increment is above 0 (hence shouldn't add energy)
        {
            nextEnergyIncrement -= Time.deltaTime; // reduce the time by the time since the last update
        }
        else if (nextEnergyIncrement <= 0f) // however if it is below the time
        {
            nextEnergyIncrement = EnergyRegenerationRate; // set the next increment to the configured time
            IncreaseEnergy(EnergyRegenerationAmount); // increase the player's energy by the specified amount
        }
        SetHealthBar(CurrentHealth); // update the HUD
        SetEnergyBar(CurrentEnergy); // same as above
        UpdateLerp(); // update the HUD smoothly
    }
    // TODO: POSSIBLY REWRITE THIS?
    void UpdateLerp() // run this at end of update to smoothly change the health bar
    {
        if (healthLerping) // if it is lerping
        {
            if (currentHealthTime <= LerpTime) // and the time is below the lerp time
            {
                currentHealthTime += Time.deltaTime; // increment by the time since the last update
                healthLerp = Mathf.Lerp(oldHealthLerp, desiredHealthLerp, currentHealthTime / LerpTime); // standard interpolation function
            }
            else // otherwise
            {
                healthLerp = desiredHealthLerp; // set to the desired lerp
                healthLerping = false; // stop lerping
                currentHealthTime = 0; // reset time
            }
            HealthBar.transform.localScale = new Vector3(healthLerp, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z); // changing scale on a left anchored image
        }
        if (energyLerping) // exact same structure as above
        {
            if (currentEnergyTime <= LerpTime)
            {
                currentEnergyTime += Time.deltaTime;
                energyLerp = Mathf.Lerp(oldEnergyLerp, desiredEnergyLerp, currentEnergyTime / LerpTime);
            }
            else
            {
                energyLerp = desiredEnergyLerp;
                energyLerping = false;
                currentEnergyTime = 0;
            }
            EnergyBar.transform.localScale = new Vector3(energyLerp, EnergyBar.transform.localScale.y, EnergyBar.transform.localScale.z);
        }
    }
    void SetHealthBar(float health) // change the text on health bar
    {
        HealthText.text = string.Format("HEALTH: {0}%", ((health / InitialEnergy) * 100).ToString("0")); // format it
        if (!healthLerping) // if it isn't already lerping
        {
            if (health != healthLerp) // and the health isn't already the desired health (since no change)
            {
                healthLerping = true; // start lerping process
                oldHealthLerp = healthLerp; // set the old health to the current health
                desiredHealthLerp = health / InitialHealth; // set desired to the parameter (and normalise it)
            }
        }
    }
    void SetEnergyBar(float energy) // exact same structure as above
    {
        EnergyText.text = string.Format("STAMINA: {0}%", ((energy / InitialEnergy) * 100).ToString("0"));
        if (!energyLerping)
        {
            if (energy != energyLerp)
            {
                energyLerping = true;
                oldEnergyLerp = energyLerp;
                desiredEnergyLerp = energy / InitialEnergy;
            }
        }
    }
    void Die() // death routine
    {
        Alive = false; // set alive to false
        manager.GetComponent<NotificationManager>().SetBottomText("You died!"); // alert the player through the text displayed at the bottom telling them they've died
    }
}
