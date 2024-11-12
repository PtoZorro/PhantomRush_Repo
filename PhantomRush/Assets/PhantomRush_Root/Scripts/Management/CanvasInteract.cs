using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasInteract : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] Image healthBarFill;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] Image bossBarFillRight;
    [SerializeField] Image bossBarFillLeft;
    [SerializeField] TextMeshProUGUI comboCountText;
    [SerializeField] Animator comboAnim;
    [SerializeField] Animator transitionAnim;

    [Header("Values")]
    [SerializeField] float healthfill;
    [SerializeField] float bossHealthfill;
    bool comboShowed;
    bool comboHided;
    bool transitioning;

    void Start()
    {
        // Se definen valores de inicio
        healthfill = 1;
        comboHided = true;
        comboShowed = false;
        transitioning = false;
    }

    void Update()
    {
        // Llenado de barra de vida
        HealthFilling();

        // Llenado de barra de vida Boss
        BossHealthFilling();

        // Contador de combo
        ComboCount();

        // Si aacabamos el nivel
        if (GameManager.instance.levelDone && !transitioning) TransitionLevel();
    }

    void HealthFilling()
    {
        healthfill = (float)GameManager.instance.health / GameManager.instance.maxHealth;

        healthBarFill.fillAmount = healthfill;
    }

    void ComboCount()
    {
        int currentCombo = GameManager.instance.combo;

        comboCountText.text = currentCombo.ToString();

        if (currentCombo < 1 && !comboHided)
        {
            comboAnim.SetTrigger("hide");
            comboHided = true;
            comboShowed = false;
        }
        else if (currentCombo == 1 && !comboShowed)
        {
            comboAnim.SetTrigger("show");
            comboHided = false;
            comboShowed = true;
        }
        else if (GameManager.instance.upHit || GameManager.instance.downHit)
        {
            comboAnim.SetTrigger("hit");
            comboHided = false;
            comboShowed = true;
        }
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

    void TransitionLevel()
    {
        transitioning = true;
        transitionAnim.SetTrigger("end");
    }
}
