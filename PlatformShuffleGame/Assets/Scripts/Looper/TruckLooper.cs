using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TruckLooper : MonoBehaviour
{
    private BoxCollider _collider;

    private bool isItDone = false;

    public event EventHandler<EventArgs> HitEndlineEvent;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        GameManager.Instance.SecondPhaseEvent += SecondPhaseListener;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform != null && !isItDone)
        {

            IContactable t = other.transform.GetComponent(typeof(IContactable)) as IContactable;
            if (t != null)
                t.InteractOTG();
        }
        if (other.CompareTag("Finish"))
        {
            HitEndlineEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    private void SecondPhaseListener(object sender, EventArgs e)
    {
        isItDone = true;
        _collider.center = new Vector3(_collider.center.x, _collider.center.y, 5f);
        
    }
}
