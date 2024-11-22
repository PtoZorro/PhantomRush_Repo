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

        // Spawn inicial de todos los m�dulos
        Spawn();
    }

    void Update()
    {
        // Comprobamos si el modulo anterior ha llegado lo suficientemente lejos como para que podamos spawnear el siguiente m�dulo
        if (lastModuleIndex >= 0 && moduleArray[lastModuleIndex] != null)
        {
            float distanceFromModule = transform.position.x - moduleArray[lastModuleIndex].transform.position.x;

            if (distanceFromModule >= moduleLenght) canSpawn = true;
        }


        // Crea m�dulo si el tiempo de spawneo lo permite
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
        // Instanciamos todos los m�dulos 
        for (modulesSpawned = 0; modulesSpawned < moduleArray.Length; modulesSpawned++)
        {
            // Instanciamos m�dulo
            GameObject module = Instantiate(moduleType[modulesSpawned], this.gameObject.transform);
            module.SetActive(false);

            // Almacenamos el m�dulo instanciado para volverlo a spawnear m�s tarde
            moduleArray[modulesSpawned] = module;
        }
    }

    void Pool()
    {
        // Activamos el m�dulo actual
        moduleArray[moduleSelected].transform.position = transform.position;
        moduleArray[moduleSelected].SetActive(true);

        // Tomamos el modulo instanciado aparte para saber su posici�n 
        lastModuleIndex = moduleSelected;

        // Pasamos al siguiente m�dulo en el array
        moduleSelected = moduleSelected >= moduleArray.Length - 1 ? 0 : moduleSelected + 1;
    }
}
