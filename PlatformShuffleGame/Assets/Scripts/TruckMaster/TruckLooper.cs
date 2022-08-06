using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TruckLooper : MonoBehaviour
{
    private BoxCollider _collider;

    private bool isItDone = false;

    public event EventHandler<EventArgs> HitEndlineEvent;
    public event EventHandler<CarUnloadArgs> HitTruckUnloadEvent;

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
        if (other.CompareTag("TruckUnload"))
        {
            TruckUnloader tmp = other.GetComponent(typeof(IContactable)) as TruckUnloader;
            if(tmp != null)
            {
                Transform[] Parks = tmp.getCarParks();
                HitTruckUnloadEvent?.Invoke(this, new CarUnloadArgs(Parks[0], Parks[1], tmp.IsEndUnloader));
            }

        }
    }
    private void SecondPhaseListener(object sender, EventArgs e)
    {
        isItDone = true;
        _collider.center = new Vector3(_collider.center.x, _collider.center.y, 5f);
        
    }
}

public class CarUnloadArgs : EventArgs
{
    private Transform _lParkTransform;
    private Transform _rParkTransform;
    private bool _isEndUnloader = false;

    public CarUnloadArgs(Transform lParkTransform, Transform rParkTransform, bool isEndUnloader)
    {
        _lParkTransform = lParkTransform;
        _rParkTransform = rParkTransform;
        _isEndUnloader = isEndUnloader;
    }
    public Transform LParkTransform { get => _lParkTransform; }
    public Transform RParkTransform { get => _rParkTransform; }
    public bool IsEndUnloader { get => _isEndUnloader; }
}