using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CanvasInteract : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] Image healthBarFill;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] Image bossBarFillRight;
    [SerializeField] Image bossBarFillLeft;
    [SerializeField] TextMeshProUGUI comboCountText;
    [SerializeField] TextMeshProUGUI maxComboText;
    [SerializeField] TextMeshProUGUI missText;
    [SerializeField] Animator comboAnim;
    [SerializeField] Animator transitionAnim;
    [SerializeField] Animator deathAnim;
    [SerializeField] Animator changeCharAnim;
    [SerializeField] GameObject kathyIcon;
    [SerializeField] GameObject coleIcon;
    [SerializeField] GameObject nateIcon;
    [SerializeField] GameObject kathyName;
    [SerializeField] GameObject coleName;
    [SerializeField] GameObject nateName;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject characterMenu;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject controlsInvertedPanel;
    [SerializeField] GameObject cinematicPanel;

    [Header("Values")]
    [SerializeField] float healthfill;
    [SerializeField] float bossHealthfill;
    [SerializeField] float timeScaleSpeed;
    [SerializeField] float cinemDuration;
    bool comboShowed;
    bool comboHided;
    bool transitioning;
    bool dying;
    bool gameOver;
    bool mainMenu;

    void Start()
    {
        // Se definen valores de inicio
        healthfill = 1;
        comboHided = true;
        comboShowed = false;
        transitioning = false;

        // Indicamos si estamos en Main Menu
        if (GameManager.instance.sceneNumber == 0)
        {
            mainMenu = true;
        }
        // Si estamos en Gameplay mostramos los elementos de la UI necesarios
        else if (GameManager.instance.sceneNumber >= 1 && GameManager.instance.sceneNumber <= 3)
        {
            // Se activan nombre e icono solo del personaje que se está usando
            if (GameManager.instance.characterSelected == "kathy") { kathyIcon.SetActive(true); kathyName.SetActive(true); }
            else if (GameManager.instance.characterSelected == "cole") { coleIcon.SetActive(true); coleName.SetActive(true); }
            else if (GameManager.instance.characterSelected == "nate") { nateIcon.SetActive(true); nateName.SetActive(true); }

            mainMenu = false;
            gameOver = false;
        }
        // Si estamos en la escena de GameOver mostramos puntuación
        else if (GameManager.instance.sceneNumber == 4)
        {
            gameOver = true;
            maxComboText.text = "Max Combo: " + GameManager.instance.maxCombo.ToString();
            missText.text = "Miss: " + GameManager.instance.miss.ToString();
        }
    }

    void Update()
    {
        // No ejecutamos la UI de gameplay si es GameOver
        if (!gameOver && !mainMenu)
        {
            // Llenado de barra de vida
            HealthFilling();

            // Llenado de barra de vida Boss
            BossHealthFilling();

            // Contador de combo
            ComboCount();

            // Comprobar si se pausa el juego
            if (GameManager.instance.pause) PauseInput();

            // Si acabamos el nivel
            if (GameManager.instance.levelDone && !transitioning) LevelTransition();
            // Si morimos
            else if (GameManager.instance.death && !dying) DeathTransition();
        }

        // Habilitamos el panel de controles correspondiente
        if (GameManager.instance.controlsInverted) { controlsInvertedPanel.SetActive(true); }
        else { controlsInvertedPanel.SetActive(false); }
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
        else
        {
            bossHealthBar.SetActive(false);
        }
    }

    public void PauseMenu(string action)
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        if (action == "on")
        {
            // Pausamos el tiempo de juego
            Time.timeScale = 0;

            // Habilitamos el menú de pausa
            pauseMenu.SetActive(true);

            // Detenemos corutina de reanudar tiempo
            StopAllCoroutines();
        }
        else if (action == "off")
        {
            // Reanudamos el tiempo gradualmente
            StartCoroutine(ResumeTimeGradually());

            // deshabilitamos el menú de pausa
            pauseMenu.SetActive(false);
        }
    }

    public void OptionsMenu(string action)
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        if (action == "on")
        {
            // Habilitamos Panel de opciones
            controlsPanel.SetActive(true);
        }
        else if (action == "off")
        {
            // Deshabilitamos panel de opciones
            controlsPanel.SetActive(false);
        }
    }

    public void ChangeControls()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Cambiamos los controles
        GameManager.instance.ChangeControlMap();
    }

    public void CharacterMenu(string action)
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        if (action == "on")
        {
            // Habilitamos Panel de personajes
            characterMenu.SetActive(true);
        }
        else if (action == "off")
        {
            // Deshabilitamos panel de personajes
            characterMenu.SetActive(false);
        }
    }

    public void ChangeCharacter(string action)
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Según el personaje seleccionado y la acción cambiamos a un personaje u a otro
        if (GameManager.instance.characterSelected == "kathy")
        {
            if (action == "right") { changeCharAnim.SetTrigger("changeR1"); GameManager.instance.characterSelected = "cole"; }
            else if (action == "left") { changeCharAnim.SetTrigger("changeL1"); GameManager.instance.characterSelected = "nate"; }
            else if (action == "check") { changeCharAnim.SetTrigger("kathy"); }
        }
        else if (GameManager.instance.characterSelected == "cole")
        {
            if (action == "right") { changeCharAnim.SetTrigger("changeR2"); GameManager.instance.characterSelected = "nate"; }
            else if (action == "left") { changeCharAnim.SetTrigger("changeL3"); GameManager.instance.characterSelected = "kathy"; }
            else if (action == "check") { changeCharAnim.SetTrigger("cole"); }
        }
        else if (GameManager.instance.characterSelected == "nate")
        {
            if (action == "right") { changeCharAnim.SetTrigger("changeR3"); GameManager.instance.characterSelected = "kathy"; }
            else if (action == "left") { changeCharAnim.SetTrigger("changeL2"); GameManager.instance.characterSelected = "cole"; }
            else if (action == "check") { changeCharAnim.SetTrigger("nate"); }
        }
    }

    IEnumerator ResumeTimeGradually()
    {
        while (Time.timeScale < GameManager.instance.timeScale)
        {
            // Aumenta Time.timeScale gradualmente usando unscaledDeltaTime
            Time.timeScale += Time.unscaledDeltaTime * timeScaleSpeed;

            // Asegurarse de que Time.timeScale no sobrepase 1
            if (Time.timeScale > GameManager.instance.timeScale)
            {
                Time.timeScale = GameManager.instance.timeScale;
            }
            yield return null; // Espera un frame
        }
    }

    void LevelTransition()
    {
        // Transición entre niveles
        transitioning = true;
        transitionAnim.SetTrigger("end");
    }

    void DeathTransition()
    {
        // Transición de muerte
        dying = true;
        deathAnim.SetTrigger("death");
    }

    public void Play()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Comenzamos a jugar
        GameManager.instance.LoadScene(1);
    }

    public void PlayCinem()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Activamos cinemática y comenzamos
        cinematicPanel.SetActive(true);

        Invoke(nameof(Play), cinemDuration);
    }

    public void ReturnMainMenu()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

    // Devolvemos el tiempo a la normalidad si estaba parado
    Time.timeScale = 1f;

        // Volvemos al menú y reseteamos stats
        GameManager.instance.ResetStats();
        GameManager.instance.LoadScene(0);
    }

    public void ExitGame()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Cerramos el juego
        Debug.Log("Aplication Quit");
        Application.Quit();
    }

    public void PauseInput()
    {
        // Sonido correspondiente
        AudioManager.instance.PlaySFX(0);

        // Enviar señal de pausa activada
        GameManager.instance.pause = false;

        // Si está activado se desactiva y viceversa 
        if (!pauseMenu.activeInHierarchy) PauseMenu("on");
        else PauseMenu("off");
    }
}
