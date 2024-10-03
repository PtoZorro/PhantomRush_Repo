using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControllerOld : MonoBehaviour
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

    // [Header("Input")]

    [Header("Player Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float timeUp;
    [SerializeField] float hitRate;

    [Header("Conditional values")]
    [SerializeField] bool isUp;
    [SerializeField] bool isDown;
    [SerializeField] bool upPressed;
    [SerializeField] bool downPressed;
    bool returning;
    bool isCoroutineRunning;
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

        // Comprobación de que tecla está pulsada
        if (upPressed)
        {
            // El player se mueve a la posición indicada
            if (!downPressed && !isCoroutineRunning) { StopAllCoroutines(); StartCoroutine(MovePlayer(upPos)); }
        }
        // Comprobación de si se ha soltado la tecla para volver
        else if (!upPressed && returning)
        {
            // El player se mueve a la posición indicada
            if (!downPressed && !isCoroutineRunning) { StartCoroutine(ReturnPlayer()); }
        }

        // Comprobación de que tecla está pulsada
        if (downPressed)
        {
            // El player se mueve a la posición indicada
            if (!isCoroutineRunning) { StopAllCoroutines(); StartCoroutine(MovePlayer(downPos)); }
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
        StopAllCoroutines();
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

    void MoveCenter()
    {

    }

    public void InputMoveUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bools de estado
            upPressed = true;
        }
        else if (context.canceled)
        {
            // Bools de estado
            upPressed = false;

            returning = true;
        }
    }

    public void InputMoveDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Bools de estado
            downPressed = true;
        }
        else if (context.canceled)
        {
            // Bools de estado
            downPressed = false;
        }
    }
}
