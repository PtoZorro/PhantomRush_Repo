using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] int healthRestored;
    bool inUpZone;
    bool inDownZone;

    private void Start()
    {
        // Se definen valores de inicio
        inUpZone = false;
        inDownZone = false;
    }

    void Update()
    {
        // Movimiento constante
        Movement();

        // Monitoreo de cuando es golpeado
        Hitted();
    }

    void Movement()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    void Hitted()
    {
        //Si el player ha golpeado en la zona adecuada eliminamos al enemigo
        if (inUpZone && GameManager.instance.upHit)
        {
            inUpZone = false;
            GameManager.instance.health += healthRestored;
            gameObject.SetActive(false);
        }
        else if (inDownZone && GameManager.instance.downHit)
        {
            inDownZone = false;
            GameManager.instance.health += healthRestored;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si el objeto que entró está en el layer de "HitZone"
        if (other.gameObject.layer == LayerMask.NameToLayer("UpHitZone"))
        {
            inUpZone = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("DownHitZone"))
        {
            inDownZone = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            inUpZone = false;
            inDownZone = false;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Comprobar si el objeto que entró está en el layer de "HitZone"
        if (other.gameObject.layer == LayerMask.NameToLayer("UpHitZone"))
        {
            inUpZone = false;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("DownHitZone"))
        {
            inDownZone = false;
        }
    }
}
