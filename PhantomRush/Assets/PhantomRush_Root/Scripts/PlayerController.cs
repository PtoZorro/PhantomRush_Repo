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
    [SerializeField] bool returnDown;

    //[Header("Combat Parameters")]

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        upPos = upPoint.position;
        downPos = downPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y == upPos.y) { isUp = true; }
        else if (transform.position.y == downPos.y) { isDown = true; returnDown = false; StopAllCoroutines(); }
        else { isUp = false; isDown = false; }
    }

    private IEnumerator MovePlayer(Vector3 targetPosition)
    {
        if (!returnDown)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        if (isUp && returnDown)
        {
            yield return new WaitForSeconds(timeUp);

            while (Vector3.Distance(transform.position, downPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, downPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            returnDown = false;
        }

        StopAllCoroutines();
    }

    void MoveCenter()
    {

    }

    public void InputMoveUp(InputAction.CallbackContext context)
    {
        if (context.started && !downPressed)
        {
            upPressed = true;

            StopAllCoroutines();
            StartCoroutine(MovePlayer(upPos));
        }
        else if (context.started && downPressed)
        {
            upPressed = true;
            downPressed = true;
        }
        else if (context.canceled)
        {
            upPressed = false;
            returnDown = true;

            StartCoroutine(MovePlayer(upPos));
        }
    }

    public void InputMoveDown(InputAction.CallbackContext context)
    {
        if (context.started && !upPressed)
        {
            downPressed = true;

            StopAllCoroutines(); 
            StartCoroutine(MovePlayer(downPos));
        }
        else if (context.started && upPressed)
        {
            upPressed = true;
            downPressed = true;

            StopAllCoroutines();
            StartCoroutine(MovePlayer(downPos));
        }
        else if (context.canceled && !upPressed)
        {
            downPressed = false;
        }
        else if (context.canceled && upPressed)
        {
            upPressed = true;
            downPressed = false;

            StartCoroutine(MovePlayer(upPos));
        }
    }
}
