using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDrops : MonoBehaviour {

    public int NumToSpawn = 3;
    public bool SpawnOnStart = true;
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
        if(SpawnOnStart)
            Spawn(NumToSpawn);
    }

    void Spawn(int num)
    {
        items = new GameObject[num];
        int numTrack = 0;
        //int numSpawns = 0;

        if (!_RandomizeAll)
        {
            while (numTrack < num)
            {
                for (int i = 0; i < DropTypes.Length; i++)
                {
                    items[i] = (Instantiate(DropTypes[i], transform.position, transform.rotation));
                    numTrack++;
                    if (numTrack >= num)
                        break;
                }
            }
        }
        else
        {
            while (numTrack < num)
            {
                for (int i = 0; i < num; i++)
                {
                    int randIndex = Random.Range((int)0, DropTypes.Length);
                    items[numTrack] = (Instantiate(DropTypes[randIndex], transform.position, transform.rotation));
                    numTrack++;
                    if (numTrack >= num)
                        break;
                }
            }
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
            Destroy(this.gameObject);
        else
            StartCoroutine(CountdownForRemoval());
    }



    protected virtual IEnumerator CountdownForRemoval()
    {
        yield return new WaitForSeconds(lifetimeOfDrops);

        foreach (GameObject i in items)
            Destroy(i);

        Destroy(this.gameObject);
    }

}
