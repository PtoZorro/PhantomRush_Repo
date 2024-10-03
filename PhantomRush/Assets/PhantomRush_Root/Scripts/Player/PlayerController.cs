using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Private References")]
    PlayerInput playerInput;
    Vector3 upPos;
    Vector3 downPos;

    [Header("Public References")]
    [SerializeField] Transform upPoint;
    [SerializeField] Transform downPoint;
    [SerializeField] HitSignal hitSignalUp;
    [SerializeField] HitSignal hitSignalDown;

    [Header("Input")]
    [SerializeField] float hitRate;
    [SerializeField] bool upKeyOnWait;
    [SerializeField] bool downKeyOnWait;
    [SerializeField] float graceTime;
    Coroutine inputCoroutine;
    Coroutine moveUpCoroutine;
    Coroutine moveDownCoroutine;
    Coroutine returnCoroutine;

    [Header("Player Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float timeUp;

    [Header("Conditional values")]
    [SerializeField] bool isUp;
    [SerializeField] bool isDown;
    [SerializeField] bool upPressed;
    [SerializeField] bool downPressed;
    [SerializeField] bool returning;
    [SerializeField] bool upCoroutineRunning;
    [SerializeField] bool downCoroutineRunning;
    [SerializeField] bool returnCoroutineRunning;
    bool canHit;
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        // Se definen las posiciones
        upPos = upPoint.position;
        downPos = downPoint.position;

        // Se definen valores de inicio
        canHit = true;
    }

    void Update()
    {
        // Monitoreo constante de movimientos del personaje
        LocationMonitoring();

        // Monitoreo constante de golpes del personaje
        HitMonitoring();
    }

    void LocationMonitoring()
    {
        // Comprobación si está arriba o abajo
        isUp = (transform.position.y == upPos.y);
        isDown = (transform.position.y == downPos.y);

        // Si la tecla arriba está presionada
        if (upPressed && !downPressed && !upCoroutineRunning)
        {
            if (!isUp) // Solo mover si no está ya en la posición superior
            {
                // Indicamos que Corutinas ya no están en marcha al interrumpirlas
                returnCoroutineRunning = false;

                // Interrumpimos las corutinas necesarias
                if (moveUpCoroutine != null) { StopCoroutine(moveUpCoroutine); }
                if (returnCoroutine != null) { StopCoroutine(returnCoroutine); }
                moveUpCoroutine = StartCoroutine(MovePlayerUp());
            }
        }
        // Si la tecla abajo está presionada
        else if (downPressed && !downCoroutineRunning)
        {
            if (!isDown) // Solo mover si no está ya en la posición inferior
            {
                // Indicamos que Corutinas ya no están en marcha al interrumpirlas
                upCoroutineRunning = false;
                returnCoroutineRunning = false;

                // Interrumpimos las corutinas necesarias
                if (moveUpCoroutine != null) { StopCoroutine(moveUpCoroutine); }
                if (moveDownCoroutine != null) { StopCoroutine(moveDownCoroutine); }
                if (returnCoroutine != null) { StopCoroutine(returnCoroutine); }

                // Iniciamos corutina de movimiento
                moveDownCoroutine = StartCoroutine(MovePlayerDown());
            }
        }
        // Si no se está presionando ninguna tecla
        else if (!upPressed && !downPressed && returning && isUp && !returnCoroutineRunning)
        {
            // Indicamos que Corutinas ya no están en marcha al interrumpirlas
            upCoroutineRunning = false;

            // Interrumpimos las corutinas necesarias
            if (returnCoroutine != null) StopCoroutine(returnCoroutine);

            // Iniciamos corutina de movimiento
            returnCoroutine = StartCoroutine(ReturnPlayer());
        }
    }

    void HitMonitoring()
    {
        // Comprobación de que tecla está pulsada
        if (upPressed && !downPressed && canHit)
        {
            // Quitamos permiso de golpe temporalmente
            canHit = false;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos señal de enemigo golpeado
            if (hitSignalUp.inHitZone) GameManager.instance.upHit = true;
        }
        else if (!upPressed && downPressed && canHit)
        {
            // Quitamos permiso de golpe temporalmente
            canHit = false;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos señal de enemigo golpeado
            if (hitSignalDown.inHitZone) GameManager.instance.downHit = true;
        }
        else if (upPressed && downPressed && canHit)
        {
            // Quitamos permiso de golpe temporalmente
            canHit = false;
            Invoke(nameof(RestartHit), hitRate);

            // Enviamos señal de enemigo golpeado
            if (hitSignalUp.inHitZone && hitSignalDown.inHitZone) { GameManager.instance.upHit = true; GameManager.instance.downHit = true; }
        }
    }

    void RestartHit()
    {
        // Reestablecer la posibilidad de golpear
        canHit = true;
    }

    private IEnumerator MovePlayerUp()
    {
        // Corrutina en marcha
        upCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno automático
        returning = false;

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, upPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Corutina finalizada
        upCoroutineRunning = false;
    }

    private IEnumerator MovePlayerDown()
    {
        // Corrutina en marcha
        downCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno automático
        returning = false;

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, downPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPos, moveSpeed * Time.deltaTime);
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

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, downPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Dejamos de avisa al dejar de movernos
        returning = false;

        // Corrutina en marcha
        returnCoroutineRunning = false;
    }

    public void InputMoveUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bool de tecla en espera a determinar si pulsamos una o varias teclas a ala vez
            upKeyOnWait = true;

            // Corutina de tiempo de gracia de pulsación de varias teclas a la vez para evitar problemas de lectura
            if (inputCoroutine != null) StopCoroutine(inputCoroutine);
            inputCoroutine = StartCoroutine(GracePeriod());
        }
        else if (context.canceled)
        {
            // Bools de estado
            upKeyOnWait = false;
            upPressed = false;

            // Bool que manda señal para que el personaje baje al cabo de un tiempo
            returning = true;
        }
    }

    public void InputMoveDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bool de tecla en espera a determinar si pulsamos una o varias teclas a ala vez
            downKeyOnWait = true;

            // Corutina de tiempo de gracia de pulsación de varias teclas a la vez para evitar problemas de lectura
            if (inputCoroutine != null) StopCoroutine(inputCoroutine);
            inputCoroutine = StartCoroutine(GracePeriod());
        }
        else if (context.canceled)
        {
            // Bools de estado
            downKeyOnWait = false;
            downPressed = false;
        }
    }

    IEnumerator GracePeriod()
    {
        // Esperamos el tiempo de gracia
        yield return new WaitForSeconds(graceTime);

        // Según la tecla o las teclas pulsadas pasado el tiempo, daremos las señales correctas
        if (upKeyOnWait && downKeyOnWait) { upPressed = true; downPressed = true; }
        else if (upKeyOnWait && !downKeyOnWait) { upPressed = true; downPressed = downPressed ? true : false; }
        else if (!upKeyOnWait && downKeyOnWait) { upPressed = upPressed ? true : false; downPressed = true; }

        // Se devuelven a falso las señales de tecla en espera al acabar la corutina
        upKeyOnWait = false;
        downKeyOnWait = false;
        returning = false;
    }
}
