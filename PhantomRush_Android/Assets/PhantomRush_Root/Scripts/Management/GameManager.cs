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

    [Header("Puntuation")]
    public int combo;
    public int maxCombo;
    public int miss;

    [Header("Signals")]
    public bool upHit;
    public bool downHit;

    [Header("Conditional Values")]
    public int sceneNumber;
    public bool gameplay;
    public bool boss;
    public bool bossDead;
    public bool levelDone;
    public bool pause;
    public bool death;
    public bool transition;

    [Header("Timer")]
    public float timeElapsed;
    [SerializeField] int lvlTime;
    public float timeScale;
    bool timerStarted;

    [Header("Player Settings")]
    public string characterSelected;
    [SerializeField] GameObject Kathy;
    [SerializeField] GameObject Cole;
    [SerializeField] GameObject Nate;
    public bool controlsInverted;
    public bool checkControlMap;

    [Header("Difficulty")]
    [SerializeField] int firstLvlTime;
    [SerializeField] int secondLvlTime;
    [SerializeField] int thirdLvlTime;
    [SerializeField] float firstLvlHealthDecrease;
    [SerializeField] float secondLvlHealthDecrease;
    [SerializeField] float thirdLvlHealthDecrease;

    [Header("Enemy Settings")]
    public GameObject[] enemyType;
    public int enemyIndex;
    public int bossHealth;
    public int maxBossHealth;

    void Awake()
    {
        // Verifica si ya existe una instancia del GameManager
        if (instance == null)
        {
            instance = this; // Si no, asignamos esta como la instancia única
            DontDestroyOnLoad(gameObject); // Evitar que se destruya al cambiar de escena
            LoadData(); // Cargamos los datos al iniciar el juego

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
        // Realentizamos tiempo si queremos hacer pruebas
        Time.timeScale = timeScale;

        // Se definen valores de inicio
        health = maxHealth;
        healthModified = maxHealth;
        death = false;
        timeElapsed = 0;
        combo = 0;
        maxCombo = 0;
        miss = 0;
        levelDone = false;
        checkControlMap = false;
    }

    private void Update()
    {
        // Acciones que solo realizamos durante el gameplay
        if (gameplay)
        {
            // Monitoreo de Golpes para interacción con enemigos
            HitInteraction();

            // Monitoreo de salud
            HealthMonitoring();

            // Monitoreo de combo
            ComboMonitoring();

            // Una vez comenzado el gameplay se inicia un temporizador
            Timer();
        }
    }

    void HealthMonitoring()
    {
        // Si estamos en medio de una transición no descontamos vida
        if (!transition)
        {
            // Multiplicamos el valor de la reducción de vida por el timpo de manera constante;
            healthModified -= (decreaseHealthSpeed * Time.deltaTime);
            // Reducimos el valor de manera constante
            health = Mathf.FloorToInt(healthModified);
            // Evitamos que el valor rebase el máximo o baje de 0
            if (healthModified > maxHealth) healthModified = maxHealth;
            // Si no tenemos salud activamos la secuencia de muerte
            else if (healthModified <= 0 && !death)
            {
                healthModified = 0;
                death = true;
                Invoke(nameof(GameOverScene), 2);
            }
        }
    }

    void HitInteraction()
    {
        // Depende de donde golpeamos, mandamos señales
        if (upHit && !downHit) { Invoke(nameof(RestartHit), .1f); }
        else if (!upHit && downHit) { Invoke(nameof(RestartHit), .1f); }
        else if (upHit && downHit) { Invoke(nameof(RestartHit), .1f); }
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

        // Almacenamos siempre el combo máximo
        if (combo > maxCombo) maxCombo = combo;
    }

    void Timer()
    {
        // Indicamos que ha empezado el temportizador para que no se pare una vez haya comenzado
        timerStarted = true;

        // Se almacena el timepo transcurrido
        timeElapsed += Time.deltaTime;

        // Activamos el Boss pasado el tiempo establecido
        if (timeElapsed >= lvlTime && !bossDead)
        {
            BossActive();
        }
    }

    void BossActive()
    {
        // Indicamos que el boss esta activado
        if (!boss) bossHealth = maxBossHealth;

        boss = true;

        // Al derrotar al boss, si no es el nivel tres, pasamos de nivel
        if (bossHealth <= 0 && !levelDone)
        {
            if (sceneNumber != 3)
            {
                levelDone = true;
                transition = true;

                Invoke(nameof(NextScene), 2);
            }

            // Indicamos que el boss ya no está activo
            boss = false;
            bossDead = true;
        }
    }

    public void PauseInput()
    {
        // Señal de pausar
        pause = true;
    }

    public void ChangeControlMap()
    {
        // Cambiamos el mapa de controles al contrario
        controlsInverted = controlsInverted == true? false: true;

        // Mandamos al script de player que compruebe el nuevo mapa de controles
        checkControlMap = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Almacenamos el número de escena
        sceneNumber = scene.buildIndex;

        // Establecemos valores al cambiar de escena
        ResetStatsSceneChange();

        if (sceneNumber == 0)
        {
            // Al volver al menú se resetean todos los stats
            ResetStats();

            // Musica correspondiente
            AudioManager.instance.PlayMusic(0);

            gameplay = false;
        }
        // Si estamos en gameplay ejecutamos spawneos
        else if (sceneNumber >= 1 && sceneNumber <= 3)
        {
            gameplay = true;

            // Valores independientes de cada nivel
            if (sceneNumber == 1)
            {
                // Seleccionamos Musica correspondiente 
                AudioManager.instance.PlayMusic(1);

                // Valores de dificultad
                lvlTime = firstLvlTime;
                decreaseHealthSpeed = firstLvlHealthDecrease;
            }
            else if (sceneNumber == 2)
            {
                // Seleccionamos Musica correspondiente 
                AudioManager.instance.PlayMusic(2);

                // Valores de dificultad
                lvlTime = secondLvlTime;
                decreaseHealthSpeed = secondLvlHealthDecrease;
            }
            else if (sceneNumber == 3)
            {
                // Seleccionamos Musica correspondiente 
                AudioManager.instance.PlayMusic(3);

                // Valores de dificultad
                lvlTime = thirdLvlTime;
                decreaseHealthSpeed = thirdLvlHealthDecrease;
            }

            // Seleccionamos el personaje que se instanciará
            GameObject characterToSpawn = null;

            if (characterSelected == "kathy") characterToSpawn = Kathy;
            else if (characterSelected == "cole") characterToSpawn = Cole;
            else if (characterSelected == "nate") characterToSpawn = Nate;

            Transform playerPosition = GameObject.Find("PlayerPosition").transform;

            Instantiate(characterToSpawn, playerPosition);

            // Indicamos a partir de que número del array de enemigos debemos fijarnos dependiendo del nivel
            if (sceneNumber == 1) enemyIndex = 0;
            if (sceneNumber == 2) enemyIndex = 4;
            if (sceneNumber == 3) enemyIndex = 8;
        }
        else gameplay = false;
    }

    public void LoadScene(int sceneToLoad)
    {
        // Cargamos la escena especificada
        SceneManager.LoadScene(sceneToLoad);
    }

    void NextScene()
    {
        // Comprobamos cual es la siguiente escena y la cargamos
        int nextScene = sceneNumber + 1;

        SceneManager.LoadScene(nextScene);
    }

    void GameOverScene()
    {
        // Solo vamos al GameOver si accedemos desde gameplay, de esta maner evitamos que si salimos del juego durante la transición, nos lleve a GameOver igualmente
        if (gameplay)
        {
            // Cargamos la escena de GameOver
            SceneManager.LoadScene(4);

            // Musica correspondiente
            AudioManager.instance.PlayMusic(4);
        }
    }

    void ResetStatsSceneChange()
    {
        // Reseteo de stats al cambiar de escena
        health = maxHealth;
        healthModified = maxHealth;
        death = false;
        boss = false;
        bossDead = false;
        bossHealth = maxBossHealth;
        timeElapsed = 0;
        levelDone = false;
        transition = false;
        Time.timeScale = timeScale;
    }

    public void ResetStats()
    {
        // Reseteo de stats al comenzar de nuevo la partida
        health = maxHealth;
        healthModified = maxHealth;
        death = false;
        bossDead = false;
        timeElapsed = 0;
        combo = 0;
        maxCombo = 0;
        miss = 0;
        levelDone = false;
        transition = false;
        Time.timeScale = timeScale;
    }

    public void SaveData()
    {
        // Recordamos al cerrar aplicación el personaje utilizado y el esquema de controles
        PlayerPrefs.SetString("characterSelected", characterSelected);

        int controlsInvertedNum = controlsInverted == true ? 1 : 0;
        PlayerPrefs.SetInt("controlsInverted", controlsInvertedNum);
    }

    public void LoadData()
    {
        // Cargamos al abrir la aplicación el personaje utilizado y el esquema de controles
        characterSelected = PlayerPrefs.GetString("characterSelected", "kathy");

        int controlsInvertedNum = PlayerPrefs.GetInt("controlsInverted", 0);
        controlsInverted = controlsInvertedNum == 1? true : false;
    }

    public void OnDestroy()
    {
        SaveData();
    }
}
