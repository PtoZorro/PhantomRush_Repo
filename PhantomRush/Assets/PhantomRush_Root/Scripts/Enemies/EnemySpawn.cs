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

    [Header("EnemyArraySettings")]
    [SerializeField] int enemiesSpawned;
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int enemySelected;

    [Header("Settings")]
    [SerializeField] float spawnRate;
    [SerializeField] float spacing;
    [SerializeField] int minPattern;
    [SerializeField] int maxPattern;
    [SerializeField] int minEnemiesInPattern;
    [SerializeField] int maxEnemiesInPattern;

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
        int randomPattern = Random.Range(minPattern, maxPattern + 1);  // Aumentado para más patrones

        // Randomizamos el número máximo de enemigos que aparecerán a la vez en una altura
        int enemiesInPatern = Random.Range(minEnemiesInPattern, maxEnemiesInPattern + 1);

        // Randomizamos altura a la que spawnearan
        Transform spawnZone = Random.Range(0, 2) == 0 ? upSpawnZone: downSpawnZone; // Randomizamos altura a la que spawnearan

        // Cuando el contador de enemigos llegue al final volvemos a 0;
        enemySelected = enemySelected >= maxEnemiesSpawned ? 0 : enemySelected;

        // escogemos el patron
        switch (randomPattern)
        {
            case 0:
                // Patron un enemigo
                SpawnSingleEnemy(spawnZone);
                break;
            case 1:
                // Patron de dos enemigos: uno arriba y uno abajo
                SpawnSingleEnemy(upSpawnZone);
                SpawnSingleEnemy(downSpawnZone);
                break;
            case 2:
                // Patron de más de un enemigo en una altura
                SpawnConsecutiveEnemies(enemiesInPatern, spawnZone);
                break;
            case 3:
                // Patron de número de enemigos distinto en ambas alturas
                SpawnMixedEnemies();
                break;
            default:
                Debug.LogWarning("Patrón de enemigo inesperado: " + randomPattern);
                break;
        }
    }


    // Método para spawnear un enemigo en una posición específica
    void SpawnSingleEnemy(Transform spawnZone)
    {
        // Cuando el contador de enemigos llegue al último, vuelve a 0 para repetir
        if (enemySelected >= maxEnemiesSpawned) enemySelected = 0;

        // Reactivamos y posicionamos el enemigo seleccionado 
        enemyArray[enemySelected].SetActive(true);
        enemyArray[enemySelected].transform.position = spawnZone.position;
        enemySelected++;
    }

    // Método general para spawnear varios enemigos consecutivos en una sola zona con desplazamiento
    void SpawnConsecutiveEnemies(int count, Transform spawnZone)
    {
        Vector3 basePosition = spawnZone.position;

        for (int i = 0; i < count; i++)
        {
            // Cuando el contador de enemigos llegue al último, vuelve a 0 para repetir
            if (enemySelected >= maxEnemiesSpawned) enemySelected = 0;

            // Reactivamos y posicionamos el enemigo con un desplazamiento
            enemyArray[enemySelected].SetActive(true);
            Vector3 offsetPosition = basePosition + new Vector3(spacing * i, 0, 0);
            enemyArray[enemySelected].transform.position = offsetPosition;
            enemySelected++;
        }
    }

    // Método para spawnear una cantidad variable de enemigos arriba y abajo, con desplazamiento
    void SpawnMixedEnemies()
    {
        // Randomizamos la cantidad de enemigos que aparecerán en cada zona (entre 1 y 4)
        int upCount = Random.Range(1, 5);
        int downCount = Random.Range(1, 5);

        // Llamamos al método general para spawnear los enemigos en la zona superior e inferior
        SpawnConsecutiveEnemies(upCount, upSpawnZone);
        SpawnConsecutiveEnemies(downCount, downSpawnZone);
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
