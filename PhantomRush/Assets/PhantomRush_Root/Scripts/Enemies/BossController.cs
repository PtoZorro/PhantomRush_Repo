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
        // Al activarse, el boss ir� a la zona de combate y al llegar se activar�n sus funciones
        if (transform.position.x != fightPosition.position.x && GameManager.instance.boss && !inFightPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, fightPosition.position, speed * Time.deltaTime);

            // Spawn desactivado hasta que el boss no est� en posici�n
            spawn.SetActive(false);
        }
        else if (transform.position.x == fightPosition.position.x && !fighting)
        {
            inFightPos = true;
            StartCombat();
        }

        // Si hemos acabado el nivel destruimos el boss
        if (GameManager.instance.levelDone && !deactivated) Deactivate();
    }

    void StartCombat()
    {
        // Indicamos que ya se ha activado el combate para no volver a entrar en la funci�n
        fighting = true;

        // Encendemos Spawn para que lance proyectiles
        spawn.SetActive(true);

        // Animaci�n de ataque constante
        anim.SetBool("attack", true);
    }

    void Deactivate()
    {
        // Desactivamos el boss y activamos explosi�n
        deactivated = true;
        spriteObject.SetActive(false);
        explosionVFX.SetActive(true);
    }
}
