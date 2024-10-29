using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInteract : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] Image healthBarFill;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] Image bossBarFillRight;
    [SerializeField] Image bossBarFillLeft;

    [Header("Values")]
    [SerializeField] float healthfill;
    [SerializeField] float bossHealthfill;

    void Start()
    {
        // Se definen valores de inicio
        healthfill = 1;
    }

    void Update()
    {
        // Llenado de barra de vida
        HealthFilling();

        // Llenado de barra de vida Boss
        BossHealthFilling();
    }

    void HealthFilling()
    {
        healthfill = (float)GameManager.instance.health / GameManager.instance.maxHealth;

        healthBarFill.fillAmount = healthfill;
    }

    void BossHealthFilling()
    {
        if (GameManager.instance.boss)
        {
            bossHealthBar.SetActive(true);

            bossHealthfill = (float)GameManager.instance.bossHealth / GameManager.instance.maxBossHealth;

            bossBarFillRight.fillAmount = bossHealthfill;
            bossBarFillLeft.fillAmount = bossHealthfill;
        }

    }
}
