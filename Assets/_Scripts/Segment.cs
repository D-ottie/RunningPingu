using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public int SegID { get; set; }
    public bool transition;

    public int length;
    public int beginY1, beginY2, beginY3, endY1, endY2, endY3;

    private PieceSpawner[] pieces;

    private void Awake()
    {
        pieces = gameObject.GetComponentsInChildren<PieceSpawner>();

        for (int i = 0; i <  pieces.Length; i++)
        {
            foreach (var mr in pieces[i].GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = LevelManager.Instance.SHOW_COLLIDER; 
            }
        }
    }


    public void Spawn()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < pieces.Length; i++)
            pieces[i].Spawn();

    }
    public void DeSpawn()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < pieces.Length; i++)
            pieces[i].Spawn();
    }
}
