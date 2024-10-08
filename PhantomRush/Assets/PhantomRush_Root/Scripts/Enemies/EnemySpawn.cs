using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform waitZone;
    [SerializeField] Transform upSpawnZone;
    [SerializeField] Transform downSpawnZone;
    [SerializeField] GameObject[] enemyArray;

    [Header("Stats")]
    [SerializeField] float spawnRate;
    [SerializeField] int enemiesSpawned;
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int enemySelected;

    [Header("Conditional Values")]
    bool canSpawn;

    void Start()
    {
        // Se definen valores de inicio
        canSpawn = true;
        enemiesSpawned = 0;
        enemySelected = 0;
        enemyArray = new GameObject[maxEnemiesSpawned];

        // Hacemos Spawn inicial de los enemigos que vamos a utilizar
        Spawn();
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
            Pool();
        }
    }

    void Spawn()
    {
        for (enemiesSpawned = 0; enemiesSpawned < maxEnemiesSpawned; enemiesSpawned++)
        {
            // Instanciamos enemigo
            GameObject enemy = Instantiate(enemyPrefab, waitZone);
            enemy.SetActive(false);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            enemyArray[enemiesSpawned] = enemy;
        }
    }
    

    void Pool()
    {
        // Randomizamos la posición de aparición de enemigos e instanciamos
        int randomValue = Random.Range(0, 2);

        // Cuando el contador de enemigos llegue al final volvemos a 0;
        enemySelected = enemySelected >= maxEnemiesSpawned ? 0 : enemySelected;

        if (randomValue == 0)
        {
            // Reactivamos y posicionamos el enemigo seleccionado 
            enemyArray[enemySelected].SetActive(true);
            enemyArray[enemySelected].transform.position = upSpawnZone.position;
            enemySelected++;
        }
        else if (randomValue == 1)
        {
            // Reactivamos y posicionamos el enemigo seleccionado
            enemyArray[enemySelected].SetActive(true);
            enemyArray[enemySelected].transform.position = downSpawnZone.position;
            enemySelected++;
        }
        else if (randomValue == 2)
        {
            // Reactivamos y posicionamos el enemigo seleccionado
            enemyArray[enemySelected].SetActive(true);
            enemyArray[enemySelected].transform.position = upSpawnZone.position;
            enemySelected++;

            // Comporbamos si hemos sobrepasado el número máximo de enemigos
            if (enemySelected < maxEnemiesSpawned)
            {
                enemyArray[enemySelected].SetActive(true);
                enemyArray[enemySelected].transform.position = downSpawnZone.position;
                enemySelected++;
            }
        }
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
