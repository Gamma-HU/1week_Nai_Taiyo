using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_collisionDirector : MonoBehaviour
{
    public GameObject CC_Prefab; // CollisionCircle
    [SerializeField] private GameObject LightSaber;
    GameObject LSE;
    void Start(){
        this.LSE = GameObject.Find("LightSaberEnemy");
    }

    void Update()
    {
        
    }

    public void CC_Generator(){
        GameObject CC = Instantiate(CC_Prefab);
        CC.transform.position = LSE.transform.position;
        LSC_Motion LSCM = CC.GetComponent<LSC_Motion>();
        LSCM.Cos_Man = -Mathf.Cos(LSE.transform.eulerAngles.z);
        LSCM.Sin_Man = -Mathf.Sin(LSE.transform.eulerAngles.z);

        //LSCM.Cos_Man = Mathf.Cos(LightSaber.transform.eulerAngles.x);
        //LSCM.Sin_Man = Mathf.Sin(LightSaber.transform.eulerAngles.x);
    }
}
