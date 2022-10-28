using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    public int maxCoins;
    public float chanceToSpawn;
    public bool forceSpawnAll = false;

    private GameObject[] coins;


    private void Awake()
    {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            coins[i] = transform.GetChild(i).gameObject;
        }
        OnDisable(); 
    }
    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) > chanceToSpawn)
            return;

        if (forceSpawnAll)
            for (int i = 0; i < maxCoins; i++)
            {
                coins[i].SetActive(true); 
            }
        else
        {
            int r = Random.Range(0, maxCoins);
            for (int i = 0; i < r; i++)
            {
                coins[i].SetActive(true); 
            }
        }
    }
    private void OnDisable()
    {
        foreach (var go in coins)
        {
            go.SetActive(false); 
        }
    }
}
