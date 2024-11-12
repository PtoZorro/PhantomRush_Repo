using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    Transform bossPosition;
    Vector3 initialDirection;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject spriteObject;
    [SerializeField] GameObject explosionVFX;

    [Header("Stats")]
    [SerializeField] float moveSpeed;
    [SerializeField] float returnSpeed;
    [SerializeField] int healthRestored;
    [SerializeField] int healthTaken;
    [SerializeField] int damageToBoss;
    [SerializeField] bool isBullet;
    [SerializeField] bool isTrap;

    [Header("Conditional values")]
    [SerializeField] bool inUpZone;
    [SerializeField] bool inDownZone;
    bool bulletReturn;
    bool allowMove;

    [SerializeField] bool explode;

    private void Start()
    {
        // Se definen valores de inicio
        inUpZone = false;
        inDownZone = false;
        bulletReturn = false;
        allowMove = true;
        bossPosition = GameObject.Find("BossBulletTarget").transform;
        initialDirection = Vector3.left;
        explosionVFX.SetActive(false);
    }

    void Update()
    {
        // Movimiento constante o si ha sido golpeado se devuelve hacia el boss
        if (!bulletReturn && allowMove) Movement();
        else if (allowMove) BulletReturn();

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

            // Si es un enemigo normal se suma salud y combo, si es enemigo trampa se resta salud y se cancela combo
            if (!isTrap)
            {
                GameManager.instance.healthModified += healthRestored;
                GameManager.instance.combo++;
            }
            else
            {
                GameManager.instance.healthModified -= healthTaken;
                GameManager.instance.combo = 0;
            }

            // Si el enemigo no es de tipo bala desactivamos el objeto al cabo un tiempo mientras se reproduce la explosión
            if (!isBullet)
            {
                explode = true;

                Invoke(nameof(ResetBullet), 2);
                spriteObject.SetActive(false);
                explosionVFX.SetActive(true);
                allowMove = false;
            }
            else bulletReturn = true;
        }
        else if (inDownZone && GameManager.instance.downHit)
        {
            inDownZone = false;

            // Si es un enemigo normal se suma salud y combo, si es enemigo trampa se resta salud y se cancela combo
            if (!isTrap)
            {
                GameManager.instance.healthModified += healthRestored;
                GameManager.instance.combo++;
            }
            else
            {
                GameManager.instance.healthModified -= healthTaken;
                GameManager.instance.combo = 0;
            }

            // Si el enemigo no es de tipo bala desactivamos el objeto al cabo un tiempo mientras se reproduce la explosión
            if (!isBullet)
            {
                explode = true;

                Invoke(nameof(ResetBullet), 2);
                spriteObject.SetActive(false);
                explosionVFX.SetActive(true);
                allowMove = false;
            }
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
            // Si es enemigo trampa no penalizamos al saltarlo
            if (!isTrap) GameManager.instance.combo = 0;

            // Desactivamos el objeto al cabo un tiempo mientras se reproduce la explosión
            Invoke(nameof(ResetBullet), 2);
            spriteObject.SetActive(false);
            allowMove = false;
        }

        // En caso de ser un Bullet del boss
        if (other.gameObject.layer == LayerMask.NameToLayer("Boss") && bulletReturn)
        {
            bulletReturn = false;

            GameManager.instance.bossHealth -= damageToBoss;

            // Desactivamos el objeto al cabo un tiempo mientras se reproduce la explosión
            Invoke(nameof(ResetBullet), 2);
            spriteObject.SetActive(false);
            explosionVFX.SetActive(true);
            allowMove = false;
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
        // Reseteamos valores iniciales
        bulletReturn = false;
        spriteObject.SetActive(true);
        explosionVFX.SetActive(false);
        if (isBullet) spriteRenderer.flipX = false;
        allowMove = true;

        // Restablecemos la dirección inicial
        transform.right = -initialDirection;

        gameObject.SetActive(false);
    }
}
