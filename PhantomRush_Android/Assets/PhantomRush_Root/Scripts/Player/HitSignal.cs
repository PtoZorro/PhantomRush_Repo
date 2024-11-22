using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSignal : MonoBehaviour
{
    public bool inHitZone;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprobar si el objeto que entr� est� en el layer de "EnemyLayer"
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            inHitZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Comprobar si el objeto que sali� est� en el layer de "EnemyLayer"
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            inHitZone = false;
        }
    }
}
