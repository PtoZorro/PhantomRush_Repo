using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Private References")]
    PlayerInput playerInput;
    [SerializeField] Animator anim;
    [SerializeField] Animator batAnim;
    [SerializeField] Animator upHitAnim;
    [SerializeField] Animator downHitAnim;
    [SerializeField] Transform upPos;
    [SerializeField] Transform midPos;
    [SerializeField] Transform downPos;

    [Header("Public References")]
    [SerializeField] HitSignal hitSignalUp;
    [SerializeField] HitSignal hitSignalDown;

    [Header("Input")]
    bool upKeyOnWait;
    bool downKeyOnWait;
    Coroutine startedCoroutine;
    Coroutine canceledCoroutine;
    Coroutine moveUpCoroutine;
    Coroutine moveDownCoroutine;
    Coroutine returnCoroutine;

    [Header("Player Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float hitRate;
    [SerializeField] float timeUp;
    [SerializeField] float graceTime;

    [Header("Conditional values")]
    [SerializeField] bool upPressed;
    [SerializeField] bool downPressed;
    bool isUp;
    bool isMid;
    bool isDown;
    bool returning;
    bool upCoroutineRunning;
    bool midCoroutineRunning;
    bool downCoroutineRunning;
    bool returnCoroutineRunning;
    bool canHit;
    bool avoidHold;
    
    void Start()
    {
        // Obtenemos referencias
        playerInput = GetComponent<PlayerInput>();

        // Se definen valores de inicio
        canHit = true;
        avoidHold = false;
        upPressed = false;
        downPressed = false;
        GameManager.instance.checkControlMap = false;

        // Chequea el mapa de controles seleccionado;
        CheckControlScheme();
    }

    void Update()
    {
        // Si es GameOver se deshabilitan los controles
        if (!GameManager.instance.death)
        {
            // Monitoreo constante de movimientos del personaje
            LocationMonitoring();

            // Monitoreo constante de golpes del personaje
            HitMonitoring();
        }

        // Comprobamos la configuraci�n de controles que hay que usar cada vez que hay un cambio
        if (GameManager.instance.checkControlMap)
        {
            CheckControlScheme();
        }
    }

    #region Movement Logic
    void LocationMonitoring()
    {
        // Comprobaci�n si est� arriba o abajo
        isUp = (transform.position.y == upPos.position.y);
        isMid = (transform.position.y == midPos.position.y);
        isDown = (transform.position.y == downPos.position.y);

        // Si la tecla arriba est� presionada
        if (upPressed && !downPressed && !upCoroutineRunning)
        {
            if (!isUp) // Solo mover si no est� ya en la posici�n superior
            {
                // Indicamos que Corutinas ya no est�n en marcha al interrumpirlas
                midCoroutineRunning = false;
                downCoroutineRunning = false;
                returnCoroutineRunning = false;

                // Interrumpimos las corutinas necesarias
                if (moveUpCoroutine != null) { StopCoroutine(moveUpCoroutine); upCoroutineRunning = false; }
                if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutineRunning = false; }
                moveUpCoroutine = StartCoroutine(MovePlayerUp());
            }
        }
        // Si la tecla abajo est� presionada
        else if (downPressed && !upPressed && !downCoroutineRunning)
        {
            if (!isDown) // Solo mover si no est� ya en la posici�n inferior
            {
                // Indicamos que Corutinas ya no est�n en marcha al interrumpirlas
                upCoroutineRunning = false;
                midCoroutineRunning = false;
                returnCoroutineRunning = false;

                // Interrumpimos las corutinas necesarias
                if (moveUpCoroutine != null) { StopCoroutine(moveUpCoroutine); upCoroutineRunning = false; }
                if (moveDownCoroutine != null) { StopCoroutine(moveDownCoroutine); downCoroutineRunning = false; }
                if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutineRunning = false; }

                // Iniciamos corutina de movimiento
                moveDownCoroutine = StartCoroutine(MovePlayerDown());
            }
        }
        // Si ambas teclas est�n presionada
        else if (downPressed && upPressed && !midCoroutineRunning)
        {
            // Indicamos que Corutinas ya no est�n en marcha al interrumpirlas
            upCoroutineRunning = false;
            downCoroutineRunning = false;
            returnCoroutineRunning = false;

            // Interrumpimos las corutinas necesarias
            if (moveUpCoroutine != null) { StopCoroutine(moveUpCoroutine); upCoroutineRunning = false; }
            if (moveDownCoroutine != null) { StopCoroutine(moveDownCoroutine); downCoroutineRunning = false; }
            if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutineRunning = false; }

            // Iniciamos corutina de movimiento
            moveDownCoroutine = StartCoroutine(MovePlayerMid());
        }
        // Si no se est� presionando ninguna tecla
        else if (!upPressed && !downPressed && returning && !returnCoroutineRunning)
        {
            // Indicamos que Corutinas ya no est�n en marcha al interrumpirlas
            upCoroutineRunning = false;

            // Interrumpimos las corutinas necesarias
            if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutineRunning = false; }

            // Iniciamos corutina de movimiento
            returnCoroutine = StartCoroutine(ReturnPlayer());
        }
    }
    #endregion

    #region Hits & Animations
    void HitMonitoring()
    {
        // Comprobaci�n de que tecla est� pulsada
        if (upPressed && !downPressed && canHit && !avoidHold)
        {
            // Quitamos permiso de golpe temporalmente y negamos posibilidad de mantener para golpear siempre
            canHit = false;
            avoidHold = true;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos se�al de enemigo golpeado
            if (hitSignalUp.inHitZone) GameManager.instance.upHit = true;

            // Animaci�n de golpeo
            anim.SetTrigger("airAttack");
            upHitAnim.SetTrigger("attack");
            if (GameManager.instance.characterSelected == "kathy") batAnim.SetTrigger("attack");

            // Sonido correspondiente
            AudioManager.instance.PlaySFX(2);
        }
        else if (!upPressed && downPressed && canHit && !avoidHold)
        {
            // Quitamos permiso de golpe temporalmente y negamos posibilidad de mantener para golpear siempre
            canHit = false;
            avoidHold = true;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos se�al de enemigo golpeado
            if (hitSignalDown.inHitZone) GameManager.instance.downHit = true;

            // Animaci�n de golpeo
            anim.SetTrigger("floorAttack");
            downHitAnim.SetTrigger("attack");
            if (GameManager.instance.characterSelected == "kathy") batAnim.SetTrigger("attack");

            // Sonido correspondiente
            AudioManager.instance.PlaySFX(1);
        }
        else if (upPressed && downPressed && canHit && !avoidHold)
        {
            // Quitamos permiso de golpe temporalmente y negamos posibilidad de mantener para golpear siempre
            canHit = false;
            avoidHold = true;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos se�al de enemigo golpeado
            if (hitSignalUp.inHitZone && hitSignalDown.inHitZone) { GameManager.instance.upHit = true; GameManager.instance.downHit = true; }

            // Animaci�n de golpeo
            anim.SetTrigger("doubleAttack");
            upHitAnim.SetTrigger("attack");
            downHitAnim.SetTrigger("attack");
            if (GameManager.instance.characterSelected == "kathy") batAnim.SetTrigger("attack");

            // Sonido correspondiente
            AudioManager.instance.PlaySFX(3);
        }
    }

    void RestartHit()
    {
        // Reestablecer la posibilidad de golpear
        canHit = true;
    }
    #endregion 

    #region Movement Execution
    private IEnumerator MovePlayerUp()
    {
        // Corrutina en marcha
        upCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno autom�tico
        returning = false;

        // Nos movemos hacia la posici�n especificada
        while (Vector3.Distance(transform.position, upPos.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Corutina finalizada
        upCoroutineRunning = false;
    }

    private IEnumerator MovePlayerMid()
    {
        // Corrutina en marcha
        midCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno autom�tico
        returning = false;

        // Nos movemos hacia la posici�n especificada
        while (Vector3.Distance(transform.position, midPos.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, midPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Corutina finalizada
        midCoroutineRunning = false;
    }

    private IEnumerator MovePlayerDown()
    {
        // Corrutina en marcha
        downCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno autom�tico
        returning = false;

        // Nos movemos hacia la posici�n especificada
        while (Vector3.Distance(transform.position, downPos.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Corutina finalizada
        downCoroutineRunning = false;
    }

    private IEnumerator ReturnPlayer()
    {
        // Corrutina en marcha
        returnCoroutineRunning = true;

        yield return new WaitForSeconds(timeUp);

        // Nos movemos hacia la posici�n especificada
        while (Vector3.Distance(transform.position, downPos.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Dejamos de avisar al dejar de movernos
        returning = false;

        // Corrutina en marcha
        returnCoroutineRunning = false;
    }
    #endregion

    #region Inputs Logic
    public void InputUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bool de tecla en espera a determinar si pulsamos una o varias teclas a ala vez
            upKeyOnWait = true;
            avoidHold = false;


            // Corutina de tiempo de gracia de pulsaci�n de varias teclas a la vez para evitar problemas de lectura
            if (startedCoroutine != null) StopCoroutine(startedCoroutine);
            startedCoroutine = StartCoroutine(GracePeriodStarted());

            // Paramos la corutina de vuelta lo antes posible si hay input
            if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutineRunning = false; }
            returning = false;
        }
        else if (context.canceled)
        {
            // Bools de estado
            upKeyOnWait = false;

            // Corutina de tiempo de gracia de pulsaci�n de varias teclas a la vez para evitar problemas de lectura
            if (canceledCoroutine == null)
            {
                canceledCoroutine = StartCoroutine(GracePeriodCanceled());
            }
        }
    }

    public void InputDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bool de tecla en espera a determinar si pulsamos una o varias teclas a ala vez
            downKeyOnWait = true;
            avoidHold = false;

            // Corutina de tiempo de gracia de pulsaci�n de varias teclas a la vez para evitar problemas de lectura
            if (startedCoroutine != null) StopCoroutine(startedCoroutine);
            startedCoroutine = StartCoroutine(GracePeriodStarted());
        }
        else if (context.canceled)
        {
            // Bools de estado
            downKeyOnWait = false;

            // Corutina de tiempo de gracia de pulsaci�n de varias teclas a la vez para evitar problemas de lectura
            if (canceledCoroutine == null)
            {
                canceledCoroutine = StartCoroutine(GracePeriodCanceled());
            }
        }
    }

    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Pausamos el juego
            GameManager.instance.PauseInput();
        }
    }

    IEnumerator GracePeriodStarted()
    {
        // Esperamos el tiempo de gracia
        yield return new WaitForSeconds(graceTime);

        // Seg�n la tecla o las teclas pulsadas pasado el tiempo, daremos las se�ales correctas
        if (upKeyOnWait && downKeyOnWait) { upPressed = true; downPressed = true; }
        else if (upKeyOnWait && !downKeyOnWait) { upPressed = true; downPressed = downPressed ? true : false; }
        else if (!upKeyOnWait && downKeyOnWait) { upPressed = upPressed ? true : false; downPressed = true; }
        else { upPressed = false; downPressed = false; }

        // Corutina finalizada
        startedCoroutine = null;
    }

    IEnumerator GracePeriodCanceled()
    {
        // Esperamos el tiempo de gracia
        yield return new WaitForSeconds(graceTime);

        // Seg�n la tecla o las teclas pulsadas pasado el tiempo, daremos las se�ales correctas
        upPressed = upKeyOnWait ? true : false;
        downPressed = downKeyOnWait ? true : false;

        // Bool que manda se�al para que el personaje baje al cabo de un tiempo
        if (!upPressed && !downPressed) returning = true;

        // Corutina finalizada
        canceledCoroutine = null;
    }

    void CheckControlScheme()
    {
        // Devolvemos se�al de que ya hemos comprobado
        GameManager.instance.checkControlMap = false;

        if (!GameManager.instance.controlsInverted)
        {
            playerInput.SwitchCurrentActionMap("Normal");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Inverted");
        }
    }

    #endregion
}
