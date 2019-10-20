using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombExplosion : NetworkBehaviour {

	[SerializeField]
	private BoxCollider2D collider2D;

	[SerializeField]
	private GameObject explosionPrefab;

	[SerializeField]
	private int explosionRange;

	[SerializeField]
	private float explosionDuration;

    /* Siempre y cuando el colisionador de la Bomba sea un disparador, el Jugador no 
     * colisionará con la misma. Sin embargo, si desmarcamos la casilla de disparador, 
     * el jugador será empujado cuando explote una bomba, pues ambos colisionarán entre ello.
     * 
     * Una manera de solucionar esto es creando la Bomba con un disparador de colisión. Sin embargo,
     * una vez que el jugador no esté colisionado con la Bomba, cambiamos el colisionador para que
     * ya no sea un disparador. A partir de este punto, tanto la bomba como el jugador colisionarán
     * normalmente.
     * 
     * En este script se busca lograr eso. También se utiliza para explotar las bombas.  
     */

        /*
         * A continuación implementamos el método OnTriggerExit2D, que, cuando es llamado, cambiará
         * el estado de colisionador de la Bomba para que ya no sea un disparador. El método es llamado
         * cuando el colisionador de otro objeto abandona el disparador de la bomba. En otras palabras, será
         * llamado cuando el jugador ya no esté sobre la bomba.
         */
	void OnTriggerExit2D(Collider2D other) {
		this.collider2D.isTrigger = false;
	}

    [Command]
    public void CmdExplode() {
        if (NetworkServer.active)
        {
            GameObject explosion = Instantiate(explosionPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(explosion);
            Destroy(explosion, this.explosionDuration);
            CmdCreateExplosions(Vector2.left);
            CmdCreateExplosions(Vector2.right);
            CmdCreateExplosions(Vector2.up);
            CmdCreateExplosions(Vector2.down);
            NetworkServer.Destroy(this.gameObject);
        }
    }

    /*
 * Este método creará explosiones en una dirección dada. El número de explosiones está establecido por la variable
 * explosionRange. De esta manera, itera desde 0 hasta el número de explosiones dado. Sin embargo, una vez que
 * encuentre una pared o un bloque, debe parar de seguir creando explosiones. Si encuentra un bloque en el
 * camino, éste debe ser destruido.
 */
    [Command]
    private void CmdCreateExplosions(Vector2 direction) {
		ContactFilter2D contactFilter = new ContactFilter2D ();

		Vector2 explosionDimensions = explosionPrefab.GetComponent<SpriteRenderer> ().bounds.size;
		Vector2 explosionPosition = (Vector2)this.gameObject.transform.position + (explosionDimensions.x * direction);
		for (int explosionIndex = 1; explosionIndex < explosionRange; explosionIndex++) {
			Collider2D[] colliders = new Collider2D[4];

            /*
             * Lo anteriormente mencionado se logra a partir del método Physics2D.OverlapBox de Unity.
             * De esta manera, podemos verificar si la región en la que la explosión se debe realizar ya
             * está ocupda por una pared o un bloque. De esta manera, es posible iterar sobre la lista de
             * colisionadores, con el fin de ver si encontramos una pared o un bloque. Si encontramos a uno de
             * ellos, establecemos a una variable como verdadero y terminamos el ciclo. Si se encuentra a un bloque,
             * se destruye. Finalmente, en el ciclo externo, si se encuentra un bloque o una pred, también
             * rompemos el ciclo. Caso contrario, creamos una nueva explosión del prefab Explosion y la establecemos
             * para ser destruida luego de la duración de la explosión.
             */
            Physics2D.OverlapBox (explosionPosition, explosionDimensions, 0.0f, contactFilter, colliders);
			bool foundBlockOrWall = false;
			foreach (Collider2D collider in colliders) {
				if (collider) {
					foundBlockOrWall = collider.tag == "Wall" || collider.tag == "Block";
					if (collider.tag == "Block") {
                        NetworkServer.Destroy(collider.gameObject);
                    }
					if (foundBlockOrWall) {
						break;
					}
				}
			}
			if (foundBlockOrWall) {
				break;
			}
			GameObject explosion = Instantiate (explosionPrefab, explosionPosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(explosion);
            Destroy(explosion, this.explosionDuration);
			explosionPosition += (explosionDimensions.x * direction);
		}
	}
}
