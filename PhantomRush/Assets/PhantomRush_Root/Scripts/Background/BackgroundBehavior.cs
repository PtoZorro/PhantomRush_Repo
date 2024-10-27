using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBehavior : MonoBehaviour
{
    [Header("References")]
    Vector3 initialDirection;

    [Header("Stats")]
    [SerializeField] float moveSpeed;

    private void Start()
    {
        // Se definen valores de inicio
        initialDirection = Vector3.left;
    }

    void Update()
    {
        // Movimiento constante
        Movement();
    }

    void Movement()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(initialDirection * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            gameObject.SetActive(false);
        }
    }
}
