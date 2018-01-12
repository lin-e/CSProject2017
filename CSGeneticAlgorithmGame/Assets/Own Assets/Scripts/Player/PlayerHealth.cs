using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public GameObject manager;
    public Image DamageFlash;
    public float InitialHealth = 100f;
    public float CurrentHealth;
    public float InitialEnergy = 100f;
    public float CurrentEnergy;
    public float EnergyRegenerationRate = 1f;
    public float EnergyRegenerationAmount = 1f;
    public bool Alive = true;
    public float LerpTime = 0.05f;
    public Image HealthBar;
    public Text HealthText;
    public Image EnergyBar;
    public Text EnergyText;

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

    private float nextEnergyIncrement = 0f;
    public void DecreaseHealth(float amount = 0f)
    {
        DamageFlash.GetComponent<DamageFlash>().Flash();
        CurrentHealth -= amount;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
    public void IncreaseHealth(float amount = 0f)
    {
        CurrentHealth += amount;
        if (CurrentHealth > InitialHealth)
        {
            CurrentHealth = InitialHealth;
        }
    }
    public void DecreaseEnergy(float amount = 0f)
    {
        CurrentEnergy -= amount;
        if (CurrentEnergy < 0)
        {
            CurrentEnergy = 0;
        }
    }
    public void IncreaseEnergy(float amount = 0f)
    {
        CurrentEnergy += amount;
        if (CurrentEnergy > InitialEnergy)
        {
            CurrentEnergy = InitialEnergy;
        }
    }
    void Start()
    {
        CurrentHealth = InitialHealth;
        CurrentEnergy = InitialEnergy;
    }
    void Update()
    {
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
        if (CurrentHealth <= 0f && Alive)
        {
            Die();
        }
        if (nextEnergyIncrement > 0)
        {
            nextEnergyIncrement -= Time.deltaTime;
        }
        else if (nextEnergyIncrement <= 0f)
        {
            nextEnergyIncrement = EnergyRegenerationRate;
            IncreaseEnergy(EnergyRegenerationAmount);
        }
        SetHealthBar(CurrentHealth);
        SetEnergyBar(CurrentEnergy);
        UpdateLerp();
    }
    void UpdateLerp()
    {
        if (healthLerping)
        {
            if (currentHealthTime <= LerpTime)
            {
                currentHealthTime += Time.deltaTime;
                healthLerp = Mathf.Lerp(oldHealthLerp, desiredHealthLerp, currentHealthTime / LerpTime);
            }
            else
            {
                healthLerp = desiredHealthLerp;
                healthLerping = false;
                currentHealthTime = 0;
            }
            HealthBar.transform.localScale = new Vector3(healthLerp, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
        }
        if (energyLerping)
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
    void SetHealthBar(float health)
    {
        HealthText.text = string.Format("HEALTH: {0}%", ((health / InitialEnergy) * 100).ToString("0"));
        if (!healthLerping)
        {
            if (health != healthLerp)
            {
                healthLerping = true;
                oldHealthLerp = healthLerp;
                desiredHealthLerp = health / InitialHealth;
            }
        }
    }
    void SetEnergyBar(float energy)
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
    void Die()
    {
        Alive = false;
        manager.GetComponent<NotificationManager>().SetBottomText("You died!");
    }
}
