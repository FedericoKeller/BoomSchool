using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombDropping : NetworkBehaviour {

	[SerializeField]
	private GameObject bombPrefab;

    /*
     * En la función Update de este script verificamos si se está
     * presionando el Espacio del teclado. Si la misma lo está, se
     * llama al método DropBomb.
     */
    void Update () {
        if (this.isLocalPlayer && Input.GetKeyDown("space"))
        {
            CmdDropBomb();
        }
    }

    /*
     * El método DropBomb instanciará una nueva bomba de su prefab (el cual es un atributo
     * del script). La posición de la nueva bomba será la misma que la del jugador.
     */
    [Command]
    void CmdDropBomb()
    {
        if (NetworkServer.active)
        {
            GameObject bomb = Instantiate(bombPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(bomb);
        }
    }
}
