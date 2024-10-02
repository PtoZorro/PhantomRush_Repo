using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] GameObject Enemy;
    [SerializeField] Transform upSpawnZone;
    [SerializeField] Transform downSpawnZone;

    [Header("Stats")]
    [SerializeField] float spawnRate;

    [Header("Conditional Values")]
    bool canSpawn;

    void Start()
    {
        // Se definen valores de inicio
        canSpawn = true;
    }

    void Update()
    {
        // Crea enemigo si el tiempo de spawneo lo permite
        if (canSpawn)
        {
            // Negamos la posibilidad de volver a spawnear temporalmente
            canSpawn = false;

            // Restablecemos la posibilidad de spawnear pasado un tiempo configurable
            Invoke(nameof(RestartSpawn), spawnRate);

            // Spawneamos enemigos
            Spawn();
        }
    }

    void Spawn()
    {
        // Randomizamos la posición de aparición de enemigos e instanciamos
        int randomValue = Random.Range(0, 3);

        if (randomValue == 0)
        {
            // Instanciamos enemigo
            Instantiate(Enemy, upSpawnZone);
        }
        else if (randomValue == 1)
        {
            // Instanciamos enemigo
            Instantiate(Enemy, downSpawnZone);
        }
        else if (randomValue == 2)
        {
            // Instanciamos dos enemigos a la vez 
            Instantiate(Enemy, upSpawnZone);
            Instantiate(Enemy, downSpawnZone);
        }
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
