using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Declaración de Singleton
    public static GameManager instance;

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
            Destroy(gameObject); // Si ya existe una instancia, destruir el objeto para evitar duplicados
        }
    }

    private void Update()
    {
        HitInteraction();
    }

    void HitInteraction()
    {
        if (upHit && !downHit) { Debug.Log("Up Enemy Hitted"); upHit = false; }
        else if (!upHit && downHit) { Debug.Log("Down Enemy Hitted"); downHit = false; }
        else if (upHit && downHit) { Debug.Log("Both Enemies Hitted"); upHit = false; downHit = false; }
    }
}
