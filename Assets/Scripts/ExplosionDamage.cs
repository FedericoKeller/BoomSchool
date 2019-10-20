using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionDamage :NetworkBehaviour {
    /*
     * Se implementa el método OnTriggerEnter2D, que
     * será llamado llamado cada vez que otro objeto colisiona con la explosión.
     * Cuando esto ocurre, verificaremos el tipo de objeto según su tag (establecido
     * en cada prefab). Si es un 'Character' tag, llamará el método LoseLife en el script
     * PlayerLife. Si es un 'Bomb' tag, llamará al método Explode en el script BombExplosion.
     */
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Character" )
        {
            collider.gameObject.GetComponent<PlayerLife>().LoseLife();
 
        }
        else if (collider.tag == "Bomb")
        {
            collider.gameObject.GetComponent<BombExplosion>().CmdExplode();
        }
    }
}

