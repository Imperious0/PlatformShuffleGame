using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler<ShuffleEventArgs> ShuffleEvent;

    public event EventHandler<SecondPhaseArgs> SecondPhaseEvent;
    public event EventHandler<EventArgs> ThirdPhaseEvent;

    public event EventHandler<EventArgs> GameOverEvent;
    public event EventHandler<GameWonEventArgs> GameWonEvent;

    private static GameManager instance = null;
    public static GameManager Instance { get => instance; }

    //OutSources
    [SerializeField] private MotionCapturer mCapture;

    [SerializeField, Range(0, 1f)] private float _carShuffleDelay = 0.25f;

    [SerializeField] private float _remainingTime = 30f;
    
    private Coroutine _swipeHandler;


    private int _roadInstanceCount = 12;
    private Vector3 _endlinePos = Vector3.zero;
    private bool _roadRunnerEnd = false;

    public int RoadInstanceCount { get => _roadInstanceCount; }
    public float RemainingTime { get => _remainingTime; }

    private void Awake()
    {
        if(Instance == null)
        {
            instance = this;

            GameObject[] _roadInstances = GameObject.FindGameObjectsWithTag("Road");
            _roadInstanceCount = _roadInstances.Length;
            foreach (var item in _roadInstances) 
            {
                item.GetComponent<RoadController>().PositionChangeEvent += RoadPositionChangeListener;
            }
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    private void Start()
    {
        _swipeHandler = StartCoroutine(SwipeHandler());

        TruckController tmp = GameObject.FindGameObjectWithTag("Truck").GetComponent<TruckController>();
        tmp.TrucksEmptyEvent += IsTruckEmptyListener;
        tmp.TruckHitsEndLineEvent += IsTruckEndsTheLineListener;
        tmp.TruckUnloadedCargoEvent += IsTruckTotallyUnloadedListener;
    }


    private void Update()
    {
        if(RemainingTime < 10f)
        {
            if (!_roadRunnerEnd)
            {
                _roadRunnerEnd = true;
                SecondPhaseEvent?.Invoke(this, new SecondPhaseArgs(_endlinePos));
                Debug.LogError("Game Status: Second Phase");
            }
        }else
        {
            _remainingTime -= Time.deltaTime;
        }
    }
    private IEnumerator SwipeHandler()
    {
        while (true)
        {
            yield return new WaitUntil(() => { return mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT); });
            while (mCapture.getCurrentMotion().Equals(MotionType.MOVEMENT))
            {
                yield return new WaitForSeconds(_carShuffleDelay);
                mCapture.signalMotion();

                if(mCapture.getHorizontalMovementForce() != 0f)
                    ShuffleEvent?.Invoke(this, new ShuffleEventArgs(mCapture.getHorizontalMovementForce() > 0f));

            }
        }
    }
    #region OutSource Listeners
    private void IsTruckEmptyListener(object sender, EventArgs e)
    {
        GameOverEvent?.Invoke(this, EventArgs.Empty);
        StopCoroutine(_swipeHandler);
        Debug.LogError("GameOver: TruckEmpty");
    }
    private void IsTruckEndsTheLineListener(object sender, EventArgs e)
    {
        //Stop Game First
        
        //Triggered Game Over
        ThirdPhaseEvent?.Invoke(this, EventArgs.Empty);
    }

    private void IsTruckTotallyUnloadedListener(object sender, TruckUnloadArgs e)
    {
        GameWonEvent?.Invoke(this, new GameWonEventArgs(e.TotalMess));

    }
    private void RoadPositionChangeListener(object sender, PositionChangeArgs e)
    {
        _endlinePos = e.CurrentPos;
    }
    #endregion
}

public class GameWonEventArgs : EventArgs
{
    private int _LevelScore = 0;

    public int LevelScore { get => _LevelScore; }

    public GameWonEventArgs(int levelScore)
    {
        _LevelScore = levelScore;
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

public class SecondPhaseArgs : EventArgs
{
    private Vector3 _endLinePos = Vector3.zero;
    public Vector3 EndLinePos { get => _endLinePos; }

    public SecondPhaseArgs(Vector3 endLinePos)
    {
        _endLinePos = endLinePos;
    }
}

