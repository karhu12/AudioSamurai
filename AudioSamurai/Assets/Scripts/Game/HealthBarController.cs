using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private const float MAX_HEALTH_FADE = 0.5f;
    public Image healthBar;
    public Image damageBar;
    private Color damageColor;
    private float damageHealthFadeTimer;

    private void Awake()
    {
        damageColor = damageBar.color;
        damageColor.a = 0f;
        damageBar.color = damageColor;
    }

    private void Update()
    {
        if (damageColor.a > 0)
        {
            damageHealthFadeTimer -= Time.deltaTime;
            if (damageHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damageColor.a -= fadeAmount * Time.deltaTime;
                damageBar.color = damageColor;
            }
        }
    }

    public void TakeDamageEffect(float startHealth, float currentHealth)
    {
            if (damageColor.a <= 0)
            {
                damageBar.fillAmount = healthBar.fillAmount;
            }
            damageColor.a = 0.1f;
            damageBar.color = damageColor;
            damageHealthFadeTimer = MAX_HEALTH_FADE;
            healthBar.fillAmount = currentHealth/ startHealth;
    }

    public void HealEffect(float startHealth, float currentHealth)
    {
            healthBar.fillAmount = currentHealth / startHealth;
    }
}
