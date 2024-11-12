using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Declaraci�n de Singleton
    public static GameManager instance;

    [Header("health Settings")]
    public int health;
    public int maxHealth;
    public float decreaseHealthSpeed;
    public float healthModified;

    [Header("Puntuation")]
    public int combo;

    [Header("Signals")]
    public bool upHit;
    public bool downHit;

    [Header("Conditional Values")]
    int sceneNumber;
    public bool firstLvl;
    public bool secondLvl;
    public bool thirdLvl;
    public bool boss;
    public bool levelDone;

    [Header("Timer")]
    public float timeElapsed;
    [SerializeField] int lvlTime;
    bool timerStarted;

    [Header("EnemySettings")]
    public GameObject[] enemyType;
    public int enemyIndex;
    public int bossHealth;
    public int maxBossHealth;

    void Awake()
    {
        // Verifica si ya existe una instancia del GameManager
        if (instance == null)
        {
            instance = this; // Si no, asignamos esta como la instancia �nica
            DontDestroyOnLoad(gameObject); // Evitar que se destruya al cambiar de escena

            // Suscribirse al evento de cambio de escena, lo que har� que cada vez que cambiamos de escena, se ejecute la funci�n indicada
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
        // Realentizamos tiempo si queremos hacer pruebas
        //Time.timeScale = 0.2f;

        // Se definen valores de inicio
        health = maxHealth;
        healthModified = maxHealth;
        timeElapsed = 0;
        combo = 0;
        levelDone = false;
    }

    private void Update()
    {
        // Monitoreo de Golpes para interacci�n con enemigos
        HitInteraction();

        // Monitoreo de salud
        HealthMonitoring();

        // Monitoreo de combo
        ComboMonitoring();

        // Una vez comenzado el gameplay se inicia un temporizador
        if (firstLvl || timerStarted) Timer();
    }

    void HealthMonitoring()
    {
        // Multiplicamos el valor de la reducci�n de vida por el timpo de manera constante;
        healthModified -= (decreaseHealthSpeed * Time.deltaTime);
        // Reducimos el valor de manera constante
        health = Mathf.FloorToInt(healthModified);
        // Evitamos que el valor rebase el m�ximo o baje de 0
        if (healthModified > maxHealth) healthModified = maxHealth;
        else if (healthModified < 0) healthModified = 0;
    }

    void HitInteraction()
    {
        // Depende de donde golpeamos, mandamos se�ales
        if (upHit && !downHit) { Debug.Log("Up Enemy Hitted"); Invoke(nameof(RestartHit), .1f); }
        else if (!upHit && downHit) { Debug.Log("Down Enemy Hitted"); Invoke(nameof(RestartHit), .1f); }
        else if (upHit && downHit) { Debug.Log("Both Enemies Hitted"); Invoke(nameof(RestartHit), .1f); }
    }

    void RestartHit()
    {
        upHit = false;
        downHit = false;
    }

    void ComboMonitoring()
    {
        // El combo no debe bajar nunca de 0
        if (combo <= 0)
        {
            combo = 0; 
        }
    }

    void Timer()
    {
        // Indicamos que ha empezado el temportizador para que no se pare una vez haya comenzado
        timerStarted = true;

        // Se almacena el timepo transcurrido
        timeElapsed += Time.deltaTime;

        // Activamos el Boss pasado el tiempo establecido
        if (timeElapsed >= lvlTime)
        {
            BossActive();
        }
    }

    void BossActive()
    {
        // Indicamos que el boss esta activado
        if (!boss) bossHealth = maxBossHealth;

        boss = true;

        // Al derrotar al boss pasamos de nivel
        if (bossHealth <= 0 && !levelDone)
        {
            levelDone = true;

            Invoke(nameof(NextScene), 2);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Almacenamos el n�mero de escena
        sceneNumber = scene.buildIndex;

        // Establecemos valores al cambiar de escena
        levelDone = false;
        boss = false;
        health = maxHealth;
        bossHealth = maxBossHealth;
        timeElapsed = 0;

        // Indicamos en que nivel nos encontramos
        firstLvl = scene.name == "Level1" ? true : false;
        secondLvl = scene.name == "Level2" ? true : false;
        thirdLvl = scene.name == "Level3" ? true : false;

        // Indicamos a partir de que n�mero del array de enemigos debemos fijarnos dependiendo del nivel
        if (firstLvl) enemyIndex = 0;
        if (secondLvl) enemyIndex = 4;
        if (thirdLvl) enemyIndex = 8;
    }

    void NextScene()
    {
        // Comprobamos cual es la siguiente escena y la cargamos
        int nextScene = sceneNumber + 1;

        SceneManager.LoadScene(nextScene);
    }
}
