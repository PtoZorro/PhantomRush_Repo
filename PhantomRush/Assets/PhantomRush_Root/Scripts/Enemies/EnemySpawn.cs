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

    [Header("Settings")]
    [SerializeField] float spawnRate;
    [SerializeField] float spacing;
    [SerializeField] int enemiesSpawned;
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int enemySelected;
    [SerializeField] int minEnemiesPattern;

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
        int randomValue = Random.Range(minEnemiesPattern, 8);  // Aumentado para más patrones

        // Cuando el contador de enemigos llegue al final volvemos a 0;
        enemySelected = enemySelected >= maxEnemiesSpawned ? 0 : enemySelected;

        if (randomValue == 0)
        {
            // Patrones simples: uno arriba
            SpawnSingleEnemy(upSpawnZone);
        }
        else if (randomValue == 1)
        {
            // Patrones simples: uno abajo
            SpawnSingleEnemy(downSpawnZone);
        }
        else if (randomValue == 2)
        {
            // Patrones de dos enemigos: uno arriba y uno abajo
            SpawnDoubleEnemies();
        }
        else if (randomValue == 3)
        {
            // Patrones de dos enemigos seguidos en la misma altura
            SpawnConsecutiveEnemies(2);
        }
        else if (randomValue == 4)
        {
            // Patrones de tres enemigos seguidos en la misma altura
            SpawnConsecutiveEnemies(3);
        }
        else if (randomValue == 5)
        {
            // Patrones de cuatro enemigos seguidos en la misma altura
            SpawnConsecutiveEnemies(4);
        }
        else if (randomValue == 6)
        {
            // Patrones de dos enemigos consecutivos en ambas alturas
            SpawnConsecutiveEnemiesInBothZones(2);
        }
        else if (randomValue == 7)
        {
            // Patrones de tres enemigos consecutivos en ambas alturas
            SpawnConsecutiveEnemiesInBothZones(3);
        }
    }

    // Método para spawnear un enemigo en una posición específica
    void SpawnSingleEnemy(Transform spawnZone)
    {
        // Reactivamos y posicionamos el enemigo seleccionado 
        enemyArray[enemySelected].SetActive(true);
        enemyArray[enemySelected].transform.position = spawnZone.position;
        enemySelected++;
    }

    // Método para spawnear dos enemigos, uno arriba y uno abajo
    void SpawnDoubleEnemies()
    {
        // Reactivamos y posicionamos el enemigo seleccionado arriba
        SpawnSingleEnemy(upSpawnZone);

        // Comprobamos si hemos sobrepasado el número máximo de enemigos
        if (enemySelected < maxEnemiesSpawned)
        {
            // Reactivamos y posicionamos el enemigo seleccionado abajo
            SpawnSingleEnemy(downSpawnZone);
        }
    }

    // Método para spawnear varios enemigos consecutivos en la misma altura
    void SpawnConsecutiveEnemies(int count)
    {
        // Elegimos aleatoriamente si los enemigos aparecerán arriba o abajo
        Transform spawnZone = (Random.Range(0, 2) == 0) ? upSpawnZone : downSpawnZone;

        // Guardamos la posición base inicial para el primer enemigo
        Vector3 basePosition = spawnZone.position;

        for (int i = 0; i < count; i++)
        {
            if (enemySelected < maxEnemiesSpawned)
            {
                // Reactivamos y posicionamos el enemigo con un desplazamiento
                enemyArray[enemySelected].SetActive(true);

                // Añadimos un desplazamiento aleatorio basado en el eje X para evitar solapamiento
                Vector3 offsetPosition = basePosition + new Vector3(spacing * i, 0, 0);

                enemyArray[enemySelected].transform.position = offsetPosition;
                enemySelected++;
            }
        }
    }

    // Método para spawnear varios enemigos consecutivos en ambas alturas
    void SpawnConsecutiveEnemiesInBothZones(int count)
    {
        // Guardamos las posiciones base iniciales para los enemigos
        Vector3 upBasePosition = upSpawnZone.position;
        Vector3 downBasePosition = downSpawnZone.position;

        for (int i = 0; i < count; i++)
        {
            if (enemySelected < maxEnemiesSpawned)
            {
                // Spawnear enemigo arriba
                enemyArray[enemySelected].SetActive(true);
                Vector3 upOffsetPosition = upBasePosition + new Vector3(spacing * i, 0, 0);
                enemyArray[enemySelected].transform.position = upOffsetPosition;
                enemySelected++;

                // Comprobamos si hay espacio para el enemigo abajo
                if (enemySelected < maxEnemiesSpawned)
                {
                    // Spawnear enemigo abajo
                    enemyArray[enemySelected].SetActive(true);
                    Vector3 downOffsetPosition = downBasePosition + new Vector3(spacing * i, 0, 0);
                    enemyArray[enemySelected].transform.position = downOffsetPosition;
                    enemySelected++;
                }
            }
        }
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}



