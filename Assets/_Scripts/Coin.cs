using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    private Animator anim; 
    private void Awake()
    {
        anim = GetComponent<Animator>();    
    }

    private void OnEnable()
    {
        anim.SetTrigger("Rotate"); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // call event for coin collected here. 
            GameManager.Instance.GetCoin(); 
            GetComponent<Animator>().SetTrigger("Collect"); 
        }
    }
}
