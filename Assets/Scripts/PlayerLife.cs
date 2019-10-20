using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLife : NetworkBehaviour
{
    /*
     * Las explosiones deben interactuar con el jugador y las bombas. Si una explosión toca a un jugador,
     * debe matar al mismo. Si una explosión toca una bomba, la misma debe explotar inmediatamente.
     * 
     * De esta manera, este script es responsable de actualizar el número actual de vidas, así como también
     * de hacer al jugador invencible por corto período de tiempo luego de que éste haya sido dañado. Esto 
     * se hace para prevenir que que el jugador pueda ser dañado por varias bombas al mismo tiempo.
     */


    [SerializeField]
    private int numberOfLives = 3;

    [SerializeField]
    private float invulnerabilityDuration = 2;

    [SerializeField]
    private float messageDuration = 3;
    private bool isHost = false;
    private bool isInvulnerable = false;

    [SerializeField]
    private GameObject playerLifeImage;

    private List<GameObject> lifeImages;

    public int Score = 0;


    private Vector2 initialPosition;
    private int initialNumberOfLives;

    /*
     * En el método Start se crean el objeto PlayerLifeImage por cada una de las vidas del jugador. Es de 
     * aclarar que es necesario tener el objeto PlayerLivesGrid para crear las images como hijos del mismo.
     * Luego de crear cada imagen, la agregamos a la lista.
     */
    void Start()
    {

        if (this.isLocalPlayer)
        {

            if (this.isServer)
            {
                isHost = true;
            }

            this.initialPosition = this.transform.position;
            this.initialNumberOfLives = this.numberOfLives;

            GameObject playerLivesGrid = GameObject.Find("PlayerLivesGrid");

            this.lifeImages = new List<GameObject>();
            for (int lifeIndex = 0; lifeIndex < this.numberOfLives; ++lifeIndex)
            {
                GameObject lifeImage = Instantiate(playerLifeImage, playerLivesGrid.transform) as GameObject;
                this.lifeImages.Add(lifeImage);
            }
        }

    }

    /*
     * En primer lugar, se verifica si el jugador es actualmente vulnerable. De serlo, reducirá el número de 
     * vidas y verificará si era la última vida. Si es así, también destruirá al objeto Jugador. Una vez que se
     * reduce el número de vidas, convierte al jugador en invencible, invocando al método BecomeVulnerable una vez
     * superado el tiempo de invencibilidad. 
     */

    public void LoseLife()
    {

        if (!this.isInvulnerable && this.isLocalPlayer)
        {

            this.numberOfLives--;
            GameObject lifeImage = this.lifeImages[this.lifeImages.Count - 1];
            Destroy(lifeImage);
            this.lifeImages.RemoveAt(this.lifeImages.Count - 1);
            if (this.numberOfLives == 0)
            {

                Respawn();
            }
            this.isInvulnerable = true;
            Invoke("BecomeVulnerable", this.invulnerabilityDuration);

        }




    }



    //El método BelcomeVulnerable, una vez llamado, simplemente quita la condición de invencibilidad al jugador.



    [Command]
    public void CmdAdjustPoints()
    {
        if (NetworkServer.active)
        {
            Score += 1;


            RpcManageScore(Score);
        }



    }

    [ClientRpc]
    public void RpcManageScore(int Score)
    {


        GameObject.Find("Score").GetComponent<Text>().text = "" + Score;

    }



    private void BecomeVulnerable()
    {

        this.isInvulnerable = false;
    }


    void Respawn()
    {

        this.numberOfLives = this.initialNumberOfLives;

        GameObject playerLivesGrid = GameObject.Find("PlayerLivesGrid");

        this.lifeImages = new List<GameObject>();
        for (int lifeIndex = 0; lifeIndex < this.numberOfLives; ++lifeIndex)
        {
            GameObject lifeImage = Instantiate(playerLifeImage, playerLivesGrid.transform) as GameObject;
            this.lifeImages.Add(lifeImage);
        }

        this.transform.position = this.initialPosition;
    }





}
