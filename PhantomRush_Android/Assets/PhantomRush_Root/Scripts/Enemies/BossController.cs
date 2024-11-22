using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform fightPosition;
    [SerializeField] GameObject spawn;
    [SerializeField] Animator anim;
    [SerializeField] GameObject explosionVFX;
    [SerializeField] GameObject spriteObject;

    [Header("Settings")]
    [SerializeField] float speed;

    [Header("Conditional Values")]
    bool inFightPos;
    bool fighting;
    bool deactivated;

    void Start()
    {
        // Se definen valores de inicio
        inFightPos = false;
        fighting = false;
        spawn.SetActive(true);
        anim.SetBool("attack", false);
        explosionVFX.SetActive(false);
    }

    void Update()
    {
        // Al activarse, el boss irá a la zona de combate y al llegar se activarán sus funciones
        if (transform.position.x != fightPosition.position.x && GameManager.instance.boss && !inFightPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, fightPosition.position, speed * Time.deltaTime);

            // Spawn desactivado hasta que el boss no esté en posición
            spawn.SetActive(false);

            // Indicamos que durante la transición no se quita vida
            GameManager.instance.transition = true;
        }
        else if (transform.position.x == fightPosition.position.x && !fighting)
        {
            inFightPos = true;
            StartCombat();

            // Indicamos que se acabó la transición
            GameManager.instance.transition = false;
        }

        // Si hemos acabado el nivel destruimos el boss
        if (GameManager.instance.bossDead && !deactivated) Deactivate();
    }

    void StartCombat()
    {
        // Indicamos que ya se ha activado el combate para no volver a entrar en la función
        fighting = true;

        // Encendemos Spawn para que lance proyectiles
        spawn.SetActive(true);

        // Animación de ataque constante
        anim.SetBool("attack", true);
    }

    void Deactivate()
    {
        // Desactivamos el boss y activamos explosión
        deactivated = true;
        spriteObject.SetActive(false);
        explosionVFX.SetActive(true);

        // Desactivamos Spawn si no es el último nivel
        spawn.SetActive(false);

        // Si es el último nivel devolvemos el spawn
        if (GameManager.instance.sceneNumber == 3) spawn.SetActive(true);
    }
}
