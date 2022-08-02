using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VisibleObjects : MonoBehaviour, IVisibleObject
{
    [SerializeField] private int _visibleIndex = 0;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        transform.parent.GetComponent<RoadController>().HideEvent += Hide;
        transform.parent.GetComponent<RoadController>().SeekEvent += Seek;
    }
    public void Hide(object sender, HideEventArgs e)
    {
        _renderer.enabled = false;
    }

    public void Seek(object sender, SeekEventArgs e)
    {
        if(e.VisibleObjectArray[_visibleIndex] > 1)
            _renderer.enabled = true;
    }
}
