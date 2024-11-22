using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBackground : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject[] moduleType;
    [SerializeField] GameObject[] moduleArray;

    [Header("ArraySettings")]
    [SerializeField] int moduleSelected;
    int modulesSpawned;
    int lastModuleIndex = -1;
    [SerializeField] float moduleLenght;
    [SerializeField] int modulesNumber;

    [Header("Conditional Values")]
    bool canSpawn;

    void Start()
    {
        // Se definen valores de inicio
        moduleArray = new GameObject[modulesNumber];
        canSpawn = true;

        // Spawn inicial de todos los módulos
        Spawn();
    }

    void Update()
    {
        // Comprobamos si el modulo anterior ha llegado lo suficientemente lejos como para que podamos spawnear el siguiente módulo
        if (lastModuleIndex >= 0 && moduleArray[lastModuleIndex] != null)
        {
            float distanceFromModule = transform.position.x - moduleArray[lastModuleIndex].transform.position.x;

            if (distanceFromModule >= moduleLenght) canSpawn = true;
        }


        // Crea módulo si el tiempo de spawneo lo permite
        if (canSpawn)
        {
            // Negamos la posibilidad de volver a spawnear temporalmente
            canSpawn = false;

            // Spawneamos
            Pool();
        }
    }

    void Spawn()
    {
        // Instanciamos todos los módulos 
        for (modulesSpawned = 0; modulesSpawned < moduleArray.Length; modulesSpawned++)
        {
            // Instanciamos módulo
            GameObject module = Instantiate(moduleType[modulesSpawned], this.gameObject.transform);
            module.SetActive(false);

            // Almacenamos el módulo instanciado para volverlo a spawnear más tarde
            moduleArray[modulesSpawned] = module;
        }
    }

    void Pool()
    {
        // Activamos el módulo actual
        moduleArray[moduleSelected].transform.position = transform.position;
        moduleArray[moduleSelected].SetActive(true);

        // Tomamos el modulo instanciado aparte para saber su posición 
        lastModuleIndex = moduleSelected;

        // Pasamos al siguiente módulo en el array
        moduleSelected = moduleSelected >= moduleArray.Length - 1 ? 0 : moduleSelected + 1;
    }
}
