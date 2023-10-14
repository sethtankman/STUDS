using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class NetTimerEvents : NetworkBehaviour {

    public float duration;

    public UnityEvent OnStartTimer;
    public UnityEvent OnEndTimer;

    private bool _timing;
    [SerializeField] private float time;

    [ClientRpc]
    public void RpcStartTimer()
    {
        StartTimer();
    }

	public void StartTimer ()
    {
        if (!_timing)
        {
            Debug.Log(gameObject.name + " started");
            _timing = true;
            OnStartTimer.Invoke();
        }
    }

    public void StopTimer()
    {
        _timing = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(_timing)
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                OnEndTimer.Invoke();
                Debug.Log(gameObject.name + " ended");
                _timing = false;
            }
        }
	}
}
