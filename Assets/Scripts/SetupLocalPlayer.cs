using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour
{


    [SyncVar]
    public Color playerColor = Color.white;



    // Start is called before the first frame update
    void Start()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
            r.material.color = playerColor;
    }

 
}
