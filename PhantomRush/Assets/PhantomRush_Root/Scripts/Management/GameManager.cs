using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Declaración de Singleton
    public static GameManager instance;

    [Header("health Settings")]
    public int health;
    public int maxHealth;
    public float decreaseHealthSpeed;
    public float healthModified;

    [Header("Signals")]
    public bool upHit;
    public bool downHit;

    [Header("Conditional Values")]
    int sceneNumber;
    public bool firstLvl;
    public bool secondLvl;
    public bool thirdLvl;
    public bool boss;
    bool timerStarted;

    [Header("Timer")]
    public float timeElapsed;
    [SerializeField] int lvlTime;

    [Header("EnemySettings")]
    public GameObject[] enemyType;
    public int enemyIndex;

    void Awake()
    {
        // Verifica si ya existe una instancia del GameManager
        if (instance == null)
        {
            instance = this; // Si no, asignamos esta como la instancia única
            DontDestroyOnLoad(gameObject); // Evitar que se destruya al cambiar de escena

            // Suscribirse al evento de cambio de escena, lo que hará que cada vez que cambiamos de escena, se ejecute la función indicada
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        timeElapsed = 0;
    }

    private void Update()
    {
        // Monitoreo de Golpes para interacción con enemigos
        HitInteraction();

        // Monitoreo de salud
        HealthMonitoring();

        // Una vez comenzado el gameplay se inicia un temporizador
        if (firstLvl || timerStarted) Timer();
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
        // Depende de donde golpeamos, mandamos señales
        if (upHit && !downHit) { Debug.Log("Up Enemy Hitted"); upHit = false; }
        else if (!upHit && downHit) { Debug.Log("Down Enemy Hitted"); downHit = false; }
        else if (upHit && downHit) { Debug.Log("Both Enemies Hitted"); upHit = false; downHit = false; }
    }

    void Timer()
    {
        // Indicamos que ha empezado el temportizador para que no se pare una vez haya comenzado
        timerStarted = true;

        // Se almacena el timepo transcurrido
        timeElapsed += Time.deltaTime;

        // Activamos el Boss pasado el tiempo establecido
        if (timeElapsed >= lvlTime && !boss)
        {
            BossEnter();
        }
    }

    void BossEnter()
    {
        boss = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Almacenamos el número de escena
        sceneNumber = scene.buildIndex;

        // Indicamos en que nivel nos encontramos
        firstLvl = scene.name == "Level1" ? true : false;
        secondLvl = scene.name == "Level2" ? true : false;
        thirdLvl = scene.name == "Level3" ? true : false;

        // Indicamos a partir de que número del array de enemigos debemos fijarnos dependiendo del nivel
        if (firstLvl) enemyIndex = 0;
        if (secondLvl) enemyIndex = 4;
        if (thirdLvl) enemyIndex = 8;
    }
}
