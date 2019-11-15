using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private const float MAX_HEALTH_FADE = 0.5f;
    public Image healthBar;
    public GameObject healthText;
    public Image damageBar;
    private Color damageColor;

    public const float startHealth = 100;
    private const float MIN_HEALTH = 0;
    private float damageHealthFadeTimer;
    private float health;
    public bool isGameOver { get; set; }

    private void Awake()
    {
        damageColor = damageBar.color;
        damageColor.a = 0f;
        damageBar.color = damageColor;
    }
    private void Start()
    {
        health = startHealth;
        isGameOver = false;
    }

    private void Update()
    {
        if(damageColor.a > 0)
        {
            damageHealthFadeTimer -= Time.deltaTime;
            if(damageHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damageColor.a -= fadeAmount * Time.deltaTime;
                damageBar.color = damageColor;
            }
        }
        if (health > MIN_HEALTH)
        {
            if (Input.anyKeyDown)
            {
                TakeDamage(10);
            }
        }
        else { GameOver(); }
        healthText.GetComponent<Text>().text = health.ToString();
    }

    public void TakeDamage(float damageToTake)
    {
        if (damageColor.a <= 0)
        {
            damageBar.fillAmount = healthBar.fillAmount;
        }
        damageColor.a = 0.5f;
        damageBar.color = damageColor;
        damageHealthFadeTimer = MAX_HEALTH_FADE;
        health -= damageToTake;
        healthBar.fillAmount = health / startHealth;
    }

    public void GameOver()
    {
        isGameOver = true;
        //Decide what to do when player dies.
    }
}
