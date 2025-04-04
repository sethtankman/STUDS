using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class NetInteraction : NetworkBehaviour

{
    public NetPWRBill_Manager GameMaster;

    public int PowerCharge = 1;


    public GameObject Object_active;
    public GameObject Object_inactive;
    public NetVolumeTrigger trigger;

    public AK.Wwise.Event OnSound;
    public AK.Wwise.Event OffSound;


    public int TimerDelayAmount = 1;

    public Text BillTotalText;

    protected float CashTimer;

    public bool interactPressed = false;


    private void Start()
    {
        GameMaster = GameObject.Find("Game Manager").GetComponent<NetPWRBill_Manager>();

    }

    public void Update()
    {
        if (isServer && !interactPressed)
        {
            CashTimer += Time.deltaTime;

            if (CashTimer >= TimerDelayAmount)
            {
                CashTimer = 0f;
                GameMaster.AddScore(PowerCharge);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdToggleVisual(bool isMini)
    {
        RpcToggleVisual(isMini);
    }

    [ClientRpc]
    public void RpcToggleVisual(bool isMini)
    {
        ToggleVisual(isMini);
    }

    [ClientRpc]
    public void RpcToggleVisualGM()
    {
        ToggleVisualGM();
    }

    public void ToggleVisual(bool isMini)
    {
        try
        {
            if (isMini && !Object_active.activeSelf)
            {
                if (isServer)
                    GameMaster.NumItemsOn += 1;
                interactPressed = false;
                trigger.isSwitchActive = false;
                Object_active.SetActive(true);
                Object_inactive.SetActive(false);
                NotifyAvailableSwitchChange(true);
                OnSound.Post(gameObject);
            }
            else if (!isMini && Object_active.activeSelf)
            {
                if(isServer)
                    GameMaster.NumItemsOn -= 1;
                interactPressed = true;
                trigger.isSwitchActive = true;
                Object_active.SetActive(false);
                Object_inactive.SetActive(true);
                NotifyAvailableSwitchChange(false);
                OffSound.Post(gameObject);
            }
            else if (isMini)
            {
                Debug.Log("Child trying to turn on object that is already on");
            }
            else
            {
                Debug.Log("Parent trying to turn off object that is already off");
            }
        } catch (Exception e)
        {
            GameMaster.PowerTXT.text = e.Message;
        }
    }

    public void NotifyAvailableSwitchChange(bool wasTurnedOn)
    {
        PennyPincherAI[] allAI = FindObjectsOfType(typeof(PennyPincherAI)) as PennyPincherAI[];
        foreach (PennyPincherAI AI in allAI)
        {
            AI.CheckUpdateTarget(trigger.gameObject, wasTurnedOn);
        }
    }

    public void NotifyAvailableSwitchChange(bool wasTurnedOn, GameObject immuneOne)
    {
        PennyPincherAI[] allAI = FindObjectsOfType(typeof(PennyPincherAI)) as PennyPincherAI[];
        foreach (PennyPincherAI AI in allAI)
        {
            AI.CheckUpdateTarget(trigger.gameObject, wasTurnedOn, immuneOne);
        }
    }

    public void ToggleVisualGM()
    {
        if(isServer)
        {
            GameMaster.NumItemsOn -= 1;
        }
        interactPressed = true;
        Debug.Log("Toggle: " + gameObject.name);
        trigger.isSwitchActive = true;
        Object_active.SetActive(false);
        Object_inactive.SetActive(true);
        if (isServer)
        {
            NetPennyPincherAI[] allAI = FindObjectsOfType(typeof(NetPennyPincherAI)) as NetPennyPincherAI[];
            foreach (NetPennyPincherAI AI in allAI)
            {
                AI.CheckUpdateTarget(trigger.gameObject, false);
            }
        }
    }
}
