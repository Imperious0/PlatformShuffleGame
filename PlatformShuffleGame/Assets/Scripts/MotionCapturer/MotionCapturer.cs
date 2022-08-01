using UnityEngine;

public class MotionCapturer : MonoBehaviour
{
    private Vector2 beginTouch = Vector2.zero;
    private Vector2 currentTouch = Vector2.zero;
    private Vector2 diffTouch = Vector2.zero;
    private Vector2 movementDir = Vector2.zero;

    [SerializeField]
    private MotionCaptureSettings mCapturerSettings;

    //Temporary Variable
    float tapCurrentTime = 0f;
 


    MotionType currentMotion = MotionType.NONE;
    private void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    //Used Cuz FPS Rate > FixedUpdate Frequency
    private void Update()
    {
        checkMotions();
    }
    private void checkMotions() 
    {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || PLATFORM_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
        {
            beginTouch = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            tapCurrentTime = Time.time;

            currentMotion = MotionType.NONE;
        }
        if (Input.GetMouseButton(0))
        {
            currentTouch = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if((beginTouch - currentTouch).magnitude > mCapturerSettings.MovementBrake)
            {
                if(currentMotion != MotionType.MOVEMENT)
                {
                    currentMotion = MotionType.MOVEMENT;
                    diffTouch = (currentTouch - beginTouch);
                    movementDir = diffTouch.normalized;
                }
                
            }
            else
            {
                if(currentMotion != MotionType.NONE)
                {
                    diffTouch = Vector2.zero;
                    movementDir = Vector2.zero;
                    currentMotion = MotionType.NONE;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            tapCurrentTime = Time.time - tapCurrentTime;
            
            if(checkIsTap())
            {
                currentMotion = MotionType.TAP;
            }
            else
            {
                currentMotion = MotionType.NONE;
            }

            diffTouch = Vector2.zero;
            movementDir = Vector2.zero;
        }
#else
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                beginTouch = new Vector2(t.position.x, t.position.y);
                tapCurrentTime = Time.time;
                currentMotion = MotionType.NONE;
            }else if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                currentTouch = new Vector2(t.position.x, t.position.y);
                if((currentTouch - beginTouch).magnitude > mCapturerSettings.MovementBrake)
                {
                    if(currentMotion != MotionType.MOVEMENT)
                    {
                        currentMotion = MotionType.MOVEMENT;
                        diffTouch = (currentTouch - beginTouch);
                        movementDir = diffTouch.normalized;
                    }
                }
                else
                {
                    if (currentMotion != MotionType.NONE)
                    {
                        diffTouch = Vector2.zero;
                        movementDir = Vector2.zero;
                        currentMotion = MotionType.NONE;
                    }
                }
            }
            
            if(t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended)
            {
                tapCurrentTime = Time.time - tapCurrentTime;
                if (checkIsTap())
                {
                    currentMotion = MotionType.TAP;
                }else
                {
                    currentMotion = MotionType.NONE;
                }

                diffTouch = Vector2.zero;
                movementDir = Vector2.zero;
                
            }
        }
#endif
    }
    private bool checkIsTap()
    {
        return (tapCurrentTime < mCapturerSettings.TapTimeBrake && diffTouch.magnitude < 1f);
    }
    /// <summary>
    /// Get Last Captured Motion Type from the Script
    /// <para>Also there is another types can be checked from <seealso cref="MotionType"/></para>
    /// </summary>
    /// <returns>Last Motion that captured from this script <see cref="currentMotion"/></returns>
    public MotionType getCurrentMotion()
    {
        return currentMotion;
    }
    /// <summary>
    /// <para>If you need to reset last captured motion use it.</para>
    /// </summary>
    public void signalMotion()
    {
        currentMotion = MotionType.NONE;
    }
    /// <summary>
    /// Return Motion Vector based on Sensitivity if there is motion
    /// </summary>
    /// <returns>
    /// <see cref="Vector2"/> Movement Direction based on Sensitivity If Not <see cref="Vector2.zero"/>
    /// </returns>
    public Vector2 getMovementDirection()
    {
        Vector2 tmpVector = Vector2.zero;
        float sensitivity = Mathf.InverseLerp(0, mCapturerSettings.MovementSensitivity, diffTouch.magnitude);
        if (currentMotion.Equals(MotionType.MOVEMENT))
        {
            //Last Normalize for set vector length exact -1,1 range
            tmpVector = movementDir;
            tmpVector = tmpVector.normalized;
            tmpVector *= sensitivity;
        }
        return tmpVector;
    }
    /// <summary>
    /// Get Mouse or Android Touch Horizontal force that based on Sensitivity
    /// </summary>
    /// <returns>-1 to 1 Force type <see cref="float"/></returns>
    public float getHorizontalMovementForce()
    {
        float sensitivity = Mathf.InverseLerp(0, mCapturerSettings.MovementSensitivity, diffTouch.magnitude);
        float movementRatio = Vector2.Dot(Vector2.right, movementDir.normalized);
        return movementRatio * sensitivity;
    }
    /// <summary>
    /// Get Mouse or Android Touch Vertical force that based on Sensitivity
    /// </summary>
    /// <returns>-1 to 1 Force type <see cref="float"/></returns>
    public float getVerticalMovementForce()
    {
        float sensitivity = Mathf.InverseLerp(0, mCapturerSettings.MovementSensitivity, diffTouch.magnitude);
        float movementRatio = Vector2.Dot(Vector2.up, movementDir.normalized);
        return movementRatio * sensitivity;
    }
    [System.Serializable]
    private class MotionCaptureSettings
    {
        [SerializeField]
        private float _movementSensitivity = 500f;
        [SerializeField]
        private float _movementBrake = 10f;
        [SerializeField]
        private float _tapTimeBrake = 0.2f;

        public float MovementSensitivity { get => _movementSensitivity; }
        public float MovementBrake { get => _movementBrake; }
        public float TapTimeBrake { get => _tapTimeBrake; }
    }
}

public enum MotionType
{
    NONE,
    TAP,//Not Necessary
    MOVEMENT
}

