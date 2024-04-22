
using System.Collections.Generic;
using UnityEngine;

//
// Blockade 1976 v2021.02.24
//
// 2021.02.15
//

public class Player2ObjectPooler : MonoBehaviour
{
    public static Player2ObjectPooler player2ObjectPooler;


    public List<GameObject> pooledObject;

    public GameObject objectToPool;

    private GameObject pooledGameObject;

    private int amountToPool;

    private bool objectPoolCanExpand;



    private void Awake()
    {
        player2ObjectPooler = this;
    }


    private void Start()
    {
        Initialise();
    }


    private void Initialise()
    {
        pooledObject = new List<GameObject>();

        amountToPool = 10;

        for (int i = 0; i < amountToPool; i++)
        {
            CreatePooledObject();
        }

        objectPoolCanExpand = true;
    }


    private void CreatePooledObject()
    {
        pooledGameObject = Instantiate(objectToPool);

        pooledGameObject.name = "Player Body";

        pooledGameObject.transform.SetParent(transform);

        pooledGameObject.SetActive(false);

        pooledObject.Add(pooledGameObject);
    }


    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObject.Count; i++)
        {
            if (!pooledObject[i].activeInHierarchy)
            {
                return pooledObject[i];
            }
        }

        if (objectPoolCanExpand)
        {
            CreatePooledObject();

            return pooledGameObject;
        }

        return null;
    }


} // end of class
