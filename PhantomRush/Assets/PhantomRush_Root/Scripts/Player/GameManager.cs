using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Declaración de Singleton
    public static GameManager instance;

    [Header("Settings")]
    public int health;
    public int maxHealth;
    public float decreaseHealthSpeed;
    public float healthModified;

    [Header("Signals")]
    public bool upHit;
    public bool downHit;

    void Awake()
    {
        // Verifica si ya existe una instancia del GameManager
        if (instance == null)
        {
            instance = this; // Si no, asignamos esta como la instancia única
            DontDestroyOnLoad(gameObject); // Evitar que se destruya al cambiar de escena
        }
        else
        {
            // Si ya existe una instancia, destruir el objeto para evitar duplicados
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        // Se definen valores de inicio
        health = maxHealth;
        healthModified = maxHealth;
    }

    private void Update()
    {
        // Monitoreo de Golpes para interacción con enemigos
        HitInteraction();

        // Monitoreo de salud
        HealthMonitoring();
    }

    void HealthMonitoring()
    {
        // Multiplicamos el valor de la reducción de vida por el timpo de manera constante;
        healthModified -= (decreaseHealthSpeed * Time.deltaTime);
        // Reducimos el valor de manera constante
        health = Mathf.FloorToInt(healthModified);
        // Evitamos que el valor rebase el máximo o baje de 0
        if (healthModified > maxHealth) healthModified = maxHealth;
        else if (healthModified < 0) healthModified = 0;
    }

    void HitInteraction()
    {
        // Depende de donde golpeamos, mandamos una o dos señales
        if (upHit && !downHit) { Debug.Log("Up Enemy Hitted"); upHit = false; }
        else if (!upHit && downHit) { Debug.Log("Down Enemy Hitted"); downHit = false; }
        else if (upHit && downHit) { Debug.Log("Both Enemies Hitted"); upHit = false; downHit = false; }
    }
}
