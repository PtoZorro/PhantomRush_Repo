using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
}
