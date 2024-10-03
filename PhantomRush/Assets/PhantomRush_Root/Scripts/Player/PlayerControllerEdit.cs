using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControllerEdit : MonoBehaviour
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
    Coroutine moveCoroutine;
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
    [SerializeField] bool isCoroutineRunning;
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
        if (upPressed && !downPressed && !isCoroutineRunning)
        {
            if (!isUp) // Solo mover si no está ya en la posición superior
            {
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MovePlayer(upPos));
            }
        }
        // Si la tecla abajo está presionada
        else if (downPressed)
        {
            if (!isDown) // Solo mover si no está ya en la posición inferior
            {
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                isCoroutineRunning = false;
                moveCoroutine = StartCoroutine(MovePlayer(downPos));
            }
        }
        // Si no se está presionando ninguna tecla
        else if (!upPressed && !downPressed && returning && isUp)
        {
            // Solo volver si está arriba y no se presionan otras teclas
            if (returnCoroutine != null) StopCoroutine(returnCoroutine);
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

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        // Corrutina en marcha
        isCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno automático
        returning = false;

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Corutina finalizada
        isCoroutineRunning = false;
    }

    private IEnumerator ReturnPlayer()
    {
        yield return new WaitForSeconds(timeUp);

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, downPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Dejamos de avisa al dejar de movernos
        returning = false;
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

