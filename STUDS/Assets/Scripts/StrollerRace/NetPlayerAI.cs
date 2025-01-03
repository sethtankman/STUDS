using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Player AI for Stroller Race Levels.
/// </summary>
public class NetPlayerAI : NetworkBehaviour
{
    public GameObject path;

    private List<Transform> pathNodes;

    [SerializeField] private int index, stuckTimer;
    private int stuckModifier = -1;

    public GameObject stroller;

    [SerializeField] private Vector3 prevPosition;

    public bool start;

    public int ID;

    public bool cycle;

    // Start is called before the first frame update
    void Start()
    {
        stuckTimer = 500;
        pathNodes = path.GetComponent<Path_AI>().getPath();
        GetComponent<NetworkCharacterMovementController>().SetGrabbedObjectNet(stroller);
        GetComponent<NetworkCharacterMovementController>().SetPlayerID(ID);
        index = 0;
        prevPosition = transform.position;
    }

    private void Update()
    {
        if(isServer)
            SelectCurrentNode();
    }

    void FixedUpdate()
    {
        if (start)
        {
            stuckTimer--;
            if (stuckTimer < 0)
            {
                if ((new Vector3(transform.position.x, transform.position.z) - new Vector3(prevPosition.x, prevPosition.z)).magnitude < 0.1f && index > 1)
                {
                    index += stuckModifier;
                    stuckModifier *= -1;
                }
                prevPosition = transform.position;
                stuckTimer = 500;
            }
        }
    }

    private void SelectCurrentNode()
    {
        if (!GetComponent<NetworkCharacterMovementController>().GetHasGrabbed())
        {
            transform.LookAt(new Vector3(stroller.transform.position.x, transform.position.y, stroller.transform.position.z));
            GetComponent<NetworkCharacterMovementController>().Move(transform.forward);
            if (Math.Abs((stroller.transform.position - transform.position).magnitude) < 3)
            {
                GetComponent<NetworkCharacterMovementController>().SetGrabbedObjectNet(stroller);
            }
        }
        else if (index+1 <= pathNodes.Count && start)
        {
            if (Math.Abs((pathNodes[index].position - transform.position).magnitude) < 1)
            {
                index++;
            }
            else
            {
                transform.LookAt(new Vector3(pathNodes[index].position.x, transform.position.y, pathNodes[index].position.z));
                GetComponent<NetworkCharacterMovementController>().Move(transform.forward);
                if (pathNodes[index].CompareTag("JumpTime"))
                {
                    GetComponent<NetworkCharacterMovementController>().isJumping = true;
                }
                else
                {
                    GetComponent<NetworkCharacterMovementController>().isJumping = false;
                }
            }
        }else if (cycle && index+1 > pathNodes.Count)
        {
            index = 0;
        }
        else
        {
            GetComponent<NetworkCharacterMovementController>().Move(new Vector3(0,0,0));
        }
    }

    public void StartAI()
    {
        start = true;
    }

    [ClientRpc]
    public void RpcSetActive(bool _tf)
    {
        transform.parent.gameObject.SetActive(_tf);
    }
}
