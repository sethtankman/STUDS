using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour {

    [Header("Trigger Settings")]
    public string TriggerTag = "Player";

    public Color GizmoColor = Color.white;

    [Header("EVENTS")]
    public UnityEvent EnterEvent;
    public UnityEvent ExitEvent;
    public UnityEvent StayEvent;

    public bool hideGizmo = false;
    public bool isSwitchActive = false;
    public Interaction interact;

    public void FlipSwitch()
    {
        interact.ToggleVisual(true);
    }

    public void NotifyInteractionOfSwitchTargetted(bool wasTurnedOn, GameObject immuneOne)
    {
        interact.NotifyAvailableSwitchChange(wasTurnedOn, immuneOne);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Check(other))
            EnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (Check(other))
            ExitEvent.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Check(other))
            StayEvent.Invoke();
    }

    private bool Check(Collider other)
    {
        if (other.CompareTag(TriggerTag))
            return true;
        else
            return false;
    }

    void OnDrawGizmos()
    {
        if (!hideGizmo)
        {
            Gizmos.color = GizmoColor;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}
