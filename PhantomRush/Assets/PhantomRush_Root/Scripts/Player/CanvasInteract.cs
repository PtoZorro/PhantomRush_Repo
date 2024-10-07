using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInteract : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] Image healthBar;

    [Header("Values")]
    [SerializeField] float healthfill;

    void Start()
    {
        // Se definen valores de inicio
        healthfill = 1;
    }

    void Update()
    {
        // Llenado de barra de vida
        HealthFilling();
    }

    void HealthFilling()
    {
        healthfill = (float)GameManager.instance.health / GameManager.instance.maxHealth;

        healthBar.fillAmount = healthfill;
    }
}
