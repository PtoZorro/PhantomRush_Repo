using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float moveSpeed;

    void Update()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Comprobar si el objeto que entró está en el layer de "EnemyLayer"
        if (other.gameObject.layer == LayerMask.NameToLayer("HitZone"))
        {
            if (GameManager.instance.upHit || GameManager.instance.downHit) { gameObject.SetActive(false); }
        }
    }
}
