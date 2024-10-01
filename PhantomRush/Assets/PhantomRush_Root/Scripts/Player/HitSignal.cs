using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSignal : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto que entró está en el layer de "EnemyLayer"
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            
        }
    }
}
