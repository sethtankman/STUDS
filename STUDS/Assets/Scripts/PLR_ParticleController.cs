using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLR_ParticleController : MonoBehaviour
{
    public ParticleSystem Dustcloud;
    public bool IsRunning = false;

    private void Start()
    {
        Dustcloud.Stop();
    }

    public void TurnOnRunning()
    {
        
        if (!IsRunning){
            IsRunning = true;
            Dustcloud.Play();
        }
    }

    public void TurnOffRunning()
    {
        if (IsRunning)
        {
            IsRunning = false;
            Dustcloud.Stop();
        }
    }
}
