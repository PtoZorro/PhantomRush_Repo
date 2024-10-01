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

    [Header("Input")]

    [Header("Player Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float timeUp;

    [Header("Conditional values")]
    [SerializeField] bool isUp;
    [SerializeField] bool isDown;
    [SerializeField] bool upPressed;
    [SerializeField] bool downPressed;
    [SerializeField] bool goingUp;
    [SerializeField] bool goingDown;
    bool returning;
    bool isCoroutineRunning;

    //[Header("Combat Parameters")]

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        // Donde se encuentra el Player
        upPos = upPoint.position;
        downPos = downPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Monitoreo constante de estados
        UptadeMonitoring();
    }

    void UptadeMonitoring()
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

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        // Corrutina en marcha
        isCoroutineRunning = true;

        // Al hacer cualquier movimiento se cancela el retorno automático
        returning = false;

        // Avisamos si nos estamos moviendo
        goingUp = (targetPosition == upPos);
        goingDown = (targetPosition == downPos);

        // Nos movemos hacia la posición especificada
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Dejamos de avisa al dejar de movernos
        goingUp = false;
        goingDown = false;

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
