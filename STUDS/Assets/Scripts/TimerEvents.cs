using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvents : MonoBehaviour {

    public float duration;

    public UnityEvent OnStartTimer;
    public UnityEvent OnEndTimer;

    private bool _timing;
    [SerializeField] private float time;

    public void SetCanPause(bool tf)
    {
        PauseV2.canPause = tf;
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

    public void ResetTimer()
    {
        _timing = false;
        time = 0f;
    }

    public void SetTimer(float _time)
    {
        time = _time;
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
