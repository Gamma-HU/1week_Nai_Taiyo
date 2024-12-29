using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MonoBehaviour
{
    public float rx;
    GameObject enemy;
    [SerializeField] private ParticleSystem particle;
    void Start(){
        this.enemy = transform.parent.gameObject;
    }

    void Update(){
        rx = 20 * Mathf.Sin(Time.time * 6);
        transform.rotation = Quaternion.Euler(rx + enemy.transform.eulerAngles.z, -90, 0);
    }

    public void Play_Man(){
        
        particle.Play();
    }

    public void Stop_Man(){
        particle.Stop();
    }
}
