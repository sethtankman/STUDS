using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NetKidTimeout : NetworkBehaviour
{
    public double timeoutTimer;
    private double currTime;
    private bool isTimeout;
    GameObject mini;

    GameObject[] timeoutPos;
    GameObject[] backInPos;
    int currIdx;
    // Start is called before the first frame update
    void Start()
    {
        timeoutPos = new GameObject[3];
        timeoutPos[0] = GameObject.Find("KidTimeoutOutside1");
        timeoutPos[1] = GameObject.Find("KidTimeoutOutside2");
        timeoutPos[2] = GameObject.Find("KidTimeoutOutside3");

        backInPos = new GameObject[3];
        backInPos[0] = GameObject.Find("KidTimeoutBackInside1");
        backInPos[1] = GameObject.Find("KidTimeoutBackInside2");
        backInPos[2] = GameObject.Find("KidTimeoutBackInside3");
    }

    // Update is called once per frame
    void Update()
    {
        timeoutPos[0] = GameObject.Find("KidTimeoutOutside1");
        timeoutPos[1] = GameObject.Find("KidTimeoutOutside2");
        timeoutPos[2] = GameObject.Find("KidTimeoutOutside3");

        backInPos[0] = GameObject.Find("KidTimeoutBackInside1");
        backInPos[1] = GameObject.Find("KidTimeoutBackInside2");
        backInPos[2] = GameObject.Find("KidTimeoutBackInside3");
        if (isTimeout)
        {
            if (currTime > timeoutTimer)
            {
                //GameObject pos = GameObject.Find("KidTimeoutBackInside");
                gameObject.transform.position = backInPos[currIdx].transform.position;
                // We need to reset the navigation agent when we teleport it as well.
                if (currTime > (timeoutTimer + 0.1))
                {
                    currTime = 0;
                    if (mini.GetComponent<CharacterMovementController>())
                    {
                        mini.GetComponent<CharacterMovementController>().CanMove = true;
                        if (mini.GetComponent<CharacterMovementController>().isAI)
                        {
                            mini.GetComponent<NavMeshAgent>().Warp(backInPos[currIdx].transform.position);
                            mini.GetComponent<PennyPincherAI>().CanMove = true;
                        }
                    }
                    else
                    {
                        mini.GetComponent<NetworkCharacterMovementController>().CanMove = true;
                        if (mini.GetComponent<NetworkCharacterMovementController>().isAI)
                        {
                            mini.GetComponent<NavMeshAgent>().Warp(backInPos[currIdx].transform.position);
                            mini.GetComponent<NetPennyPincherAI>().CanMove = true;
                        }
                    }
                    mini = null;
                    isTimeout = false;
                    Debug.Log("Back in");
                }
                currTime += Time.deltaTime;
            }
            else
            {
                currTime += Time.deltaTime;
                //GameObject pos = GameObject.Find("KidTimeoutOutside");
                gameObject.transform.position = timeoutPos[currIdx].transform.position;
            }
        }
    }

    [ClientRpc]
    public void RpcTimeout()
    {
        if(GetComponent<NetworkCharacterMovementController>().isAI || isLocalPlayer)
            Timeout(gameObject);
    }

    public void Timeout(GameObject mini)
    {
        //GameObject pos = GameObject.Find("KidTimeoutOutside");
        int maxSize = timeoutPos.Length;
        int randomidx = Random.Range(0, maxSize);
        if (randomidx > maxSize - 1)
        {
            randomidx = maxSize - 1;
        }
        currIdx = randomidx;
        GameObject g = timeoutPos[currIdx];
        Vector3 newPos = g.transform.position;
        mini.transform.position = newPos;
        if (mini.GetComponent<NetworkCharacterMovementController>())
        {
            mini.GetComponent<NetworkCharacterMovementController>().CanMove = false;
            if (mini.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                mini.GetComponent<NetPennyPincherAI>().CanMove = false;
            }
        }
        this.mini = mini;
        isTimeout = true;
    }
}
