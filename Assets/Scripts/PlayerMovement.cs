using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private Animator animator;



    void FixedUpdate()
    {
        if (this.isLocalPlayer)
        {
            /*
             * Se obtienen los ejes de movimiento (horizontal y vertical) con el fin
             * de determinar la velocidad del jugador en ambas direcciones. La manera 
             * en el que el jugador se mueve es la siguiente: el mismo se puede mover
             * hacia una dirección dada (por ejemplo, la izquierda) si no se está ya moviendo
             * a la dirección opuesta (a la derecha).
             */
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector2 currentVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

            float newVelocityX = 0f;

            /*
             * La primer condición verifica si la velocidad del jugador es menor o igual a zero
             * (lo que significaría no se está moviendo a la derecha). De ser así, cambiamos la velocidad
             * en el eje X para que se mueva hacia la izquierda. Hacemos lo mismo para todas las direcciones.
             */

            if (moveHorizontal < 0 && currentVelocity.x <= 0)
            {
                newVelocityX = -speed;
                animator.SetInteger("DirectionX", -1);
            }
            else if (moveHorizontal > 0 && currentVelocity.x >= 0)
            {
                newVelocityX = speed;

               /*
               * Se establecen los párametros DirectionX y DirectionY del controlador de animaciones en el script.
               * Una vez que se establece la velocidad en una dirección dada, estos parámetros serán actualizados. Si
               * el jugador no se está moviendo en un eje determinado, establecemos dicho parámetro en 0.
               */

                animator.SetInteger("DirectionX", 1);
            }
            else
            {
                animator.SetInteger("DirectionX", 0);
            }

            float newVelocityY = 0f;
            if (moveVertical < 0 && currentVelocity.y <= 0)
            {
                newVelocityY = -speed;
                animator.SetInteger("DirectionY", -1);
            }
            else if (moveVertical > 0 && currentVelocity.y >= 0)
            {
                newVelocityY = speed;
                animator.SetInteger("DirectionY", 1);
            }
            else
            {
                animator.SetInteger("DirectionY", 0);
            }

            //Se establece la velocidad del objeto.
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(newVelocityX, newVelocityY);
        }
    }
}