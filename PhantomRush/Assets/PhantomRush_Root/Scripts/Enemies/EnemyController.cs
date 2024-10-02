using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float moveSpeed;
    bool inHitZone;

    void Update()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (inHitZone)
        {
            if (GameManager.instance.upHit || GameManager.instance.downHit)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si el objeto que entró está en el layer de "HitZone"
        if (other.gameObject.layer == LayerMask.NameToLayer("HitZone"))
        {
            inHitZone = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Comprobar si el objeto que entró está en el layer de "HitZone"
        if (other.gameObject.layer == LayerMask.NameToLayer("HitZone"))
        {
            inHitZone = false;
        }
    }
}
