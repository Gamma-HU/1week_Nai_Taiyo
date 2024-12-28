using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer_Director : MonoBehaviour
{
    public GameObject sourcePrefab;
    GameObject player;
    void Start(){
        player = GameObject.Find("Nise_Player");
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Z)){
            GameObject beam = Instantiate(sourcePrefab);
            beam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        }
    }
}
