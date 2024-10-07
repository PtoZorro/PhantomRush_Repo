using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform upSpawnZone;
    [SerializeField] Transform downSpawnZone;
    GameObject[] enemyArray;

    [Header("Stats")]
    [SerializeField] float spawnRate;
    int enemiesSpawned;
    int maxEnemiesSpawned;
    int enemySelected;

    [Header("Conditional Values")]
    bool canSpawn;

    void Start()
    {
        // Se definen valores de inicio
        canSpawn = true;
        enemiesSpawned = 0;
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
            if (enemiesSpawned < maxEnemiesSpawned) Spawn();
            else Pool();
        }
    }

    void Spawn()
    {
        // Almacenamos el enemigo actual en una variable para ser utilizado posteriormente
        GameObject thisEnemyUp = enemyPrefab;
        GameObject thisEnemyDown = enemyPrefab;

        // Randomizamos la posición de aparición de enemigos e instanciamos
        int randomValue = Random.Range(0, 3);

        if (randomValue == 0)
        {
            // Instanciamos enemigo
            Instantiate(thisEnemyUp, upSpawnZone);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            enemiesSpawned++;
            enemyArray[enemiesSpawned - 1] = thisEnemyUp;
        }
        else if (randomValue == 1)
        {
            // Instanciamos enemigo
            Instantiate(thisEnemyDown, downSpawnZone);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            enemiesSpawned++;
            enemyArray[enemiesSpawned - 1] = thisEnemyDown;
        }
        else if (randomValue == 2)
        {
            // Instanciamos dos enemigos a la vez 
            Instantiate(thisEnemyUp, upSpawnZone);
            Instantiate(thisEnemyDown, downSpawnZone);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            enemiesSpawned++;
            enemyArray[enemiesSpawned - 1] = thisEnemyUp;

            // Almacenamos el siguiente enemigo al ser dos a la vez
            enemiesSpawned++;
            enemyArray[enemiesSpawned - 1] = thisEnemyDown;
        }
    }

    void Pool()
    {

    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
