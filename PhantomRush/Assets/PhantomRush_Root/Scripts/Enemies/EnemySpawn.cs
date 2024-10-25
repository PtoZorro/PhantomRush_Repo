using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] GameObject upEnemyPrefab;
    [SerializeField] GameObject downEnemyPrefab;
    [SerializeField] Transform waitZone;
    [SerializeField] Transform upSpawnZone;
    [SerializeField] Transform downSpawnZone;
    [SerializeField] GameObject[] upEnemyArray;
    [SerializeField] GameObject[] downEnemyArray;

    [Header("EnemyArraySettings")]
    [SerializeField] int enemiesSpawned;
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int upEnemySelected;
    [SerializeField] int downEnemySelected;

    [Header("Settings")]
    [SerializeField] float minSpawnRate;
    [SerializeField] float maxSpawnRate;
    [SerializeField] float spacing;
    [SerializeField] int minPattern;
    [SerializeField] int maxPattern;

    [Header("Conditional Values")]
    bool canSpawn;

    void Start()
    {
        // Se definen valores de inicio
        canSpawn = true;
        enemiesSpawned = 0;
        upEnemySelected = 0;
        downEnemySelected = 0;
        upEnemyArray = new GameObject[maxEnemiesSpawned];
        downEnemyArray = new GameObject[maxEnemiesSpawned];

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

            // Valor random de SpawnRate para que la separación entre enemigos sea variada
            float randomRate = Random.Range(minSpawnRate, maxSpawnRate);

            // Restablecemos la posibilidad de spawnear pasado un tiempo configurable
            Invoke(nameof(RestartSpawn), randomRate);

            // Spawneamos enemigos
            Pool();
        }
    }

    void Spawn()
    {
        for (enemiesSpawned = 0; enemiesSpawned < maxEnemiesSpawned; enemiesSpawned++)
        {
            // Instanciamos tipo de enemigo
            GameObject enemy = Instantiate(upEnemyPrefab, waitZone);
            enemy.SetActive(false);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            upEnemyArray[enemiesSpawned] = enemy;

            // Instanciamos tipo de enemigo
            enemy = Instantiate(downEnemyPrefab, waitZone);
            enemy.SetActive(false);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            downEnemyArray[enemiesSpawned] = enemy;
        }
    }

    void Pool()
    {
        // Randomizamos el patron que hará spawn
        int randomPattern = Random.Range(minPattern, maxPattern + 1);

        // Cuando el contador de enemigos llegue al final volvemos a 0;
        upEnemySelected = upEnemySelected >= maxEnemiesSpawned ? 0 : upEnemySelected;
        downEnemySelected = downEnemySelected >= maxEnemiesSpawned ? 0 : downEnemySelected;

        // escogemos el patron
        switch (randomPattern)
        {
            case 0:
                // Patron un enemigo arriba
                SpawnSingleEnemy(upSpawnZone);
                break;
            case 1:
                // Patron un enemigo abajo
                SpawnSingleEnemy(downSpawnZone);
                break;
            case 2:
                // Patron de dos enemigos: uno arriba y uno abajo
                SpawnSingleEnemy(upSpawnZone);
                SpawnSingleEnemy(downSpawnZone);
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
        if (upEnemySelected >= maxEnemiesSpawned) upEnemySelected = 0;
        if (downEnemySelected >= maxEnemiesSpawned) downEnemySelected = 0;

        if (spawnZone == upSpawnZone)
        {
            // Reactivamos y posicionamos el enemigo seleccionado 
            upEnemyArray[upEnemySelected].SetActive(true);
            upEnemyArray[upEnemySelected].transform.position = spawnZone.position;
            upEnemySelected++;
        }
        else if (spawnZone == downSpawnZone)
        {
            // Reactivamos y posicionamos el enemigo seleccionado 
            downEnemyArray[downEnemySelected].SetActive(true);
            downEnemyArray[downEnemySelected].transform.position = spawnZone.position;
            downEnemySelected++;
        }
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
