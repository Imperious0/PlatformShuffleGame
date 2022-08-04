using System;
using UnityEngine;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private Vector3 endlineOffset;
    private void Start()
    {
        GameManager.Instance.SecondPhaseEvent += SecondPhaseListener;
    }
    private void SecondPhaseListener(object sender, SecondPhaseArgs e)
    {
        
        transform.position = e.EndLinePos + endlineOffset;
    }
}
