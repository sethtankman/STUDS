﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawnDrops : NetworkBehaviour
{

    public int NumToSpawn = 3;
    public bool SpawnOnStart = true;
    [SerializeField] private bool AIIgnore = false;
    public GameObject[] DropTypes;
    public float lifetimeOfDrops = -1;
    [Tooltip("Each drop will be randomized.")]
    public bool _RandomizeAll;
    GameObject[] items;


    // For the object to show up in Editor if need be.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(.25f, .25f, .25f));
    }

    private void Start()
    {
        if (isServer && SpawnOnStart)
            Invoke(nameof(Spawn), 0.1f);
    }

    /// <summary>
    /// Don't spawn anything unless it's on the server.
    /// </summary>
    void Spawn()
    {
        items = new GameObject[1];

        if (!_RandomizeAll)
        {
            items[0] = Instantiate(DropTypes[0], transform.position, transform.rotation);
            if (!AIIgnore)
                NetDBGameManager.Instance.enlistDodgeball(items[0]);
            NetworkServer.Spawn(items[0]);
        }
        else
        {
            int randIndex = Random.Range((int)0, DropTypes.Length);
            items[0] = (Instantiate(DropTypes[randIndex], transform.position, transform.rotation));
            if (!AIIgnore)
                NetDBGameManager.Instance.enlistDodgeball(items[0]);
            NetworkServer.Spawn(items[0]);
        }

        foreach (GameObject i in items)
        {
            Vector3 v = Random.insideUnitSphere.normalized * 500;
            v.Scale(new Vector3(1, 0, 1));
            Rigidbody rb = i.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(v);
                rb.AddTorque(Random.insideUnitSphere * 500);
            }
        }

        if (lifetimeOfDrops <= 0)
            NetworkServer.Destroy(this.gameObject);
        else
            StartCoroutine(CountdownForRemoval());
    }



    protected virtual IEnumerator CountdownForRemoval()
    {
        yield return new WaitForSeconds(lifetimeOfDrops);
        foreach (GameObject i in items)
            NetworkServer.Destroy(i);

        NetworkServer.Destroy(this.gameObject);
    }

}
