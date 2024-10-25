using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawnDrops : NetworkBehaviour
{
    public bool SpawnOnStart = true;
    [SerializeField] private bool AIIgnore = false;
    public GameObject[] DropTypes;
    [SerializeField] private Transform spawnTransform;
    public float lifetimeOfDrops = -1;
    [Tooltip("Each drop will be randomized.")]
    public bool _RandomizeAll;
    private GameObject item;


    // For the object to show up in Editor if need be.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(.25f, .25f, .25f));
    }

    private void Start()
    {
        if (SpawnOnStart)
            Invoke(nameof(Spawn), 0.1f);
    }

    /// <summary>
    /// Don't spawn anything unless it's on the server.
    /// Method is called by timers.
    /// </summary>
    public void Spawn()
    {

        if (isServer)
        {
            if (!_RandomizeAll)
            {
                item = Instantiate(DropTypes[0], spawnTransform.position, spawnTransform.rotation);
                if (!AIIgnore)
                    NetDBGameManager.Instance.enlistDodgeball(item);
                NetworkServer.Spawn(item);

            }
            else
            {
                int randIndex = Random.Range((int)0, DropTypes.Length);
                item = Instantiate(DropTypes[randIndex], spawnTransform.position, spawnTransform.rotation);
                if (!AIIgnore)
                    NetDBGameManager.Instance.enlistDodgeball(item);
                NetworkServer.Spawn(item);
            }

            Vector3 v = Random.insideUnitSphere.normalized * 500;
            v.Scale(new Vector3(1, 0, 1));
            Rigidbody rb = item.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(v);
                rb.AddTorque(Random.insideUnitSphere * 500);
            }

            if (lifetimeOfDrops > 0)
                StartCoroutine(CountdownForRemoval());
        }
    }



    protected virtual IEnumerator CountdownForRemoval()
    {
        yield return new WaitForSeconds(lifetimeOfDrops);
        NetworkServer.Destroy(item);
    }

}
