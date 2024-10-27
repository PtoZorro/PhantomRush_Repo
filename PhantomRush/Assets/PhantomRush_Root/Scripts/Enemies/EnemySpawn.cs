using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Public References")]
    GameObject upEnemyPrefab;
    GameObject downEnemyPrefab;
    GameObject trapEnemyPrefab;
    GameObject bossBulletPrefab;
    [SerializeField] GameObject[] upEnemyArray;
    [SerializeField] GameObject[] downEnemyArray;
    [SerializeField] GameObject[] trapEnemyArray;
    [SerializeField] GameObject[] bossBulletArray;
    [SerializeField] Transform waitZone;
    [SerializeField] Transform upSpawnZone;
    [SerializeField] Transform downSpawnZone;
    

    [Header("EnemyArraySettings")]
    [SerializeField] int enemiesSpawned;
    [SerializeField] int maxEnemiesSpawned;
    [SerializeField] int upEnemySelected;
    [SerializeField] int downEnemySelected;
    [SerializeField] int trapEnemySelected;
    [SerializeField] int bossBulletSelected;

    [Header("Settings")]
    [SerializeField] float minSpawnRate;
    [SerializeField] float maxSpawnRate;
    [SerializeField] float spacing;
    [SerializeField] int minPattern;
    [SerializeField] int maxPattern;
    [SerializeField] int trapEnemyRate;

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
        trapEnemyArray = new GameObject[maxEnemiesSpawned];
        bossBulletArray = new GameObject[maxEnemiesSpawned];

        // Instanciamos al principio todos los enemigos que vamos a utilizar
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
        // Elegimos enemigos que vamos a utilizar durante el nivel según la información del Singelton
        int enemyIndex = GameManager.instance.enemyIndex;

        upEnemyPrefab = GameManager.instance.enemyType[enemyIndex];
        enemyIndex++;
        downEnemyPrefab = GameManager.instance.enemyType[enemyIndex];
        enemyIndex++;
        trapEnemyPrefab = GameManager.instance.enemyType[enemyIndex];
        enemyIndex++;
        bossBulletPrefab = GameManager.instance.enemyType[enemyIndex];
        enemyIndex++;

        // Devolvemos al Singleton la información
        GameManager.instance.enemyIndex = enemyIndex;

        // Instanciamos todos los tipos de enemigos del nivel
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

            // Instanciamos tipo de enemigo
            enemy = Instantiate(trapEnemyPrefab, waitZone);
            enemy.SetActive(false);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            trapEnemyArray[enemiesSpawned] = enemy;

            // Instanciamos tipo de enemigo
            enemy = Instantiate(bossBulletPrefab, waitZone);
            enemy.SetActive(false);

            // Almacenamos el enemigo instanciado para volverlo a spawnear más tarde
            bossBulletArray[enemiesSpawned] = enemy;
        }
    }

    void Pool()
    {
        // Randomizamos el patron que hará spawn
        int randomPattern = Random.Range(minPattern, maxPattern + 1);

        // Randomizador para posibilidad de que salga un enemigo trampa
        bool trapEnemy;

        if (!GameManager.instance.boss)
        {
            int randomTrapEnemy = Random.Range(0, trapEnemyRate + 1);
            trapEnemy = randomTrapEnemy >= trapEnemyRate ? true : false;
        }
        else trapEnemy = false;

        // Cuando el contador de enemigos llegue al final volvemos a 0;
        upEnemySelected = upEnemySelected >= maxEnemiesSpawned ? 0 : upEnemySelected;
        downEnemySelected = downEnemySelected >= maxEnemiesSpawned ? 0 : downEnemySelected;
        trapEnemySelected = trapEnemySelected >= maxEnemiesSpawned ? 0 : trapEnemySelected;
        bossBulletSelected = bossBulletSelected >= maxEnemiesSpawned ? 0 : bossBulletSelected;

        // escogemos el patron
        switch (randomPattern)
        {
            case 0:
                // Patron un enemigo arriba
                SpawnSingleEnemy(upSpawnZone, trapEnemy);

                break;
            case 1:
                // Patron un enemigo abajo
                SpawnSingleEnemy(downSpawnZone, trapEnemy);

                break;
            case 2:
                // Patron de dos enemigos: uno arriba y uno abajo

                //Si ha salido enemigo trampa randomizamos en cual de las dos alturas saldrá para que no salgan siempre dos
                bool upTrap = false;
                bool downTrap = false;

                if (trapEnemy)
                {
                    int randomTrapRepeat = Random.Range(0, 2);
                    upTrap = randomTrapRepeat > 0 ? true : false;
                    downTrap = randomTrapRepeat < 1 ? true : false;
                }

                SpawnSingleEnemy(upSpawnZone, upTrap);
                SpawnSingleEnemy(downSpawnZone, downTrap);

                break;
            default:
                Debug.LogWarning("Patrón de enemigo inesperado: " + randomPattern);
                break;
        }
    }


    // Método para spawnear un enemigo en una posición específica
    void SpawnSingleEnemy(Transform spawnZone, bool isTrap)
    {
        int enemySelected = 0;
        GameObject[] enemyArray = null;

        // Almacenamos el tipo de enemigo que nos toca instanciar
        if (!GameManager.instance.boss)
        {
            if (spawnZone == upSpawnZone && !isTrap)
            {
                enemySelected = upEnemySelected;
                enemyArray = upEnemyArray;
            }
            else if (spawnZone == downSpawnZone && !isTrap)
            {
                enemySelected = downEnemySelected;
                enemyArray = downEnemyArray;
            }
            else if (isTrap)
            {
                enemySelected = trapEnemySelected;
                enemyArray = trapEnemyArray;
            }
        }
        else
        {
            enemySelected = bossBulletSelected;
            enemyArray = bossBulletArray;
        }

        // Cuando el contador de enemigos llegue al último, vuelve a 0 para repetir
        if (enemySelected >= maxEnemiesSpawned) enemySelected = 0;

        // Reactivamos y posicionamos el enemigo seleccionado 
        enemyArray[enemySelected].SetActive(true);
        enemyArray[enemySelected].transform.position = spawnZone.position;
        enemySelected++;

        // devolvemos los nuevos datos a su variable original
        if (!GameManager.instance.boss)
        {
            if (spawnZone == upSpawnZone && !isTrap)
            {
                 upEnemySelected = enemySelected;
            }
            else if (spawnZone == downSpawnZone && !isTrap)
            {
                 downEnemySelected = enemySelected;
            }
            else if (isTrap)
            {
                 trapEnemySelected = enemySelected;
            }
        }
        else
        {
             bossBulletSelected = enemySelected;
        }
    }

    void RestartSpawn()
    {
        // Permitimos Spawneo
        canSpawn = true;
    }
}
