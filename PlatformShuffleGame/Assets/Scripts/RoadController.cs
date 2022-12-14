using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoadController : MonoBehaviour, IContactable
{
    private Transform _transform;
    private Collider _collider;

    public event EventHandler<HideEventArgs> HideEvent;
    public event EventHandler<SeekEventArgs> SeekEvent;
    public event EventHandler<PositionChangeArgs> PositionChangeEvent;

    private int childCount = 4;
    private int decisionCount = 2;

    [SerializeField] private Vector3 RoadOffset;
    private void Awake()
    {
        _transform = transform;
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {
        HideEvent?.Invoke(this, new HideEventArgs());
        SeekEvent?.Invoke(this, new SeekEventArgs(childCount, decisionCount));
    }
    public void InteractOTG()
    {

        _collider.enabled = false;
        HideEvent?.Invoke(this, new HideEventArgs());
        _transform.localPosition += RoadOffset * GameManager.Instance.RoadInstanceCount;
        SeekEvent?.Invoke(this, new SeekEventArgs(childCount, decisionCount));
        _collider.enabled = true;
        PositionChangeEvent?.Invoke(this, new PositionChangeArgs(_transform.position + (RoadOffset / 2)));
    }

    public void InteractOTG(int index)
    {
        //NOP
    }
}

public class HideEventArgs : EventArgs
{

}
public class SeekEventArgs : EventArgs
{
    private int[] _visibleObjectArray;
    private int[] _visibleDecisionArray;

    public SeekEventArgs(int visibleObjectCount, int visibleDecisionCount)
    {
        _visibleObjectArray = new int[visibleObjectCount];
        for (int i = 0; i < visibleObjectCount; i++)
        {
            _visibleObjectArray[i] = UnityEngine.Random.Range(0, 4);
        }
        _visibleDecisionArray = new int[visibleDecisionCount];
        for (int i = 0; i < visibleDecisionCount; i++)
        {
            _visibleDecisionArray[i] = UnityEngine.Random.Range(0, 4);
        }

    }
    public int[] VisibleObjectArray { get => _visibleObjectArray; }
    public int[] VisibleDecisionArray { get => _visibleDecisionArray; }
}
public class PositionChangeArgs : EventArgs
{
    private Vector3 _currentPos = Vector3.zero;

    public PositionChangeArgs(Vector3 currentPos)
    {
        _currentPos = currentPos;
    }

    public Vector3 CurrentPos { get => _currentPos; }
}