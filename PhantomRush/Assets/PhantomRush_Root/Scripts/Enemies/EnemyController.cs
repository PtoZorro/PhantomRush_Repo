using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    Transform bossPosition;
    Vector3 initialDirection;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float returnSpeed;
    [SerializeField] int healthRestored;
    [SerializeField] bool isBullet;

    [Header("Conditional values")]
    bool inUpZone;
    bool inDownZone;
    bool bulletReturn;

    private void Start()
    {
        // Se definen valores de inicio
        inUpZone = false;
        inDownZone = false;
        bulletReturn = false;
        bossPosition = GameObject.Find("BossBulletTarget").transform;
        initialDirection = Vector3.left;
    }

    void Update()
    {
        // Movimiento constante o si ha sido golpeado se devuelve hacia el boss
        if (!bulletReturn) Movement();
        else BulletReturn();

        // Monitoreo de cuando es golpeado
        Hitted();
    }

    void Movement()
    {
        // Mover el objeto a una velocidad constante en la dirección negativa del eje X
        transform.Translate(initialDirection * moveSpeed * Time.deltaTime);
    }

    void Hitted()
    {
        //Si el player ha golpeado en la zona adecuada eliminamos al enemigo y sumamos salud
        if (inUpZone && GameManager.instance.upHit)
        {
            inUpZone = false;
            GameManager.instance.healthModified += healthRestored;

            if (!isBullet) gameObject.SetActive(false);
            else bulletReturn = true;
        }
        else if (inDownZone && GameManager.instance.downHit)
        {
            inDownZone = false;
            GameManager.instance.healthModified += healthRestored;

            if (!isBullet) gameObject.SetActive(false);
            else bulletReturn = true;
        }
    }

    void BulletReturn()
    {
        // Definimos la posición en el plano 2D, ignorando la coordenada Z del boss
        Vector3 bossPosition2D = new Vector3(bossPosition.position.x, bossPosition.position.y, transform.position.z);

        // Hacemos que la bala mire hacia la posición del boss en 2D
        Vector3 directionToBoss = (bossPosition2D - transform.position).normalized;
        transform.right = directionToBoss;

        // Giramos solo el sprite de la bala
        spriteRenderer.flipX = true;

        // Movemos la bala hacia el boss con una velocidad constante
        transform.position = Vector3.MoveTowards(transform.position, bossPosition2D, returnSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si hemos entrado en la zona de golpeo
        if (other.gameObject.layer == LayerMask.NameToLayer("UpHitZone"))
        {
            inUpZone = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("DownHitZone"))
        {
            inDownZone = true;
        }
        // Si entra en la zona límite se desactiva
        else if (other.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            gameObject.SetActive(false);
        }

        // En caso de ser un Bullet del boss
        if (other.gameObject.layer == LayerMask.NameToLayer("Boss") && bulletReturn)
        {
            bulletReturn = false;

            //Dejamos la bala con la configuración original
            ResetBullet();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Comprobar si hemos entrado en la zona de golpeo
        if (other.gameObject.layer == LayerMask.NameToLayer("UpHitZone"))
        {
            inUpZone = false;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("DownHitZone"))
        {
            inDownZone = false;
        }
    }

    void ResetBullet()
    {
        bulletReturn = false;
        spriteRenderer.flipX = false;

        // Restablecemos la dirección inicial
        transform.right = -initialDirection;

        gameObject.SetActive(false);
    }
}
