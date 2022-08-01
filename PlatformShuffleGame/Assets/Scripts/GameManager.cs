using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler<ShuffleEventArgs> ShuffleEvent;

    private static GameManager instance = null;

    public static GameManager Instance { get => instance; }

    [SerializeField] private MotionCapturer mCapture;


    private void Awake()
    {
        if(Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(SwipeHandler());
    }

    private IEnumerator SwipeHandler()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT); });
            while (mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT))
            {
                yield return new WaitForSeconds(0.25f);
                mCapture.signalMotion();

                if(mCapture.getHorizontalMovementForce() != 0f)
                    ShuffleEvent?.Invoke(this, new ShuffleEventArgs(mCapture.getHorizontalMovementForce() > 0f));

            }
        }
    }
}

public class ShuffleEventArgs : EventArgs 
{
    private bool isShufleRight;

    public bool IsShufleRight { get => isShufleRight; }

    public ShuffleEventArgs(bool isShufleRight)
    {
        this.isShufleRight = isShufleRight;
    }
}