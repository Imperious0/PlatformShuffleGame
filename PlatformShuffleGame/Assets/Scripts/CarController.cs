using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CarController : MonoBehaviour
{
    private Vector3 _prevPosition;
    private Vector3 _nextPosition;
    private Vector3 _prevQuat;
    private Vector3 _nextQuat;

    private Transform _transform;
    private Renderer _renderer;

    private float _currentAnimationTime = 0f;

    [SerializeField, Range(0,5)] private float _carAnimationSpeed = 0.125f;

    [SerializeField] private int _carMess = 5;
    public int CarMess { get => _carMess; }

    private void Awake()
    {
        _transform = transform;
        _renderer = GetComponent<Renderer>();
    }
    private void FixedUpdate()
    {
        if (!_transform.localPosition.Equals(_nextPosition))
        {
            Vector3 desiredPosition = Vector3.Slerp(_prevPosition, _nextPosition, _currentAnimationTime);
            Quaternion desiredQuat = Quaternion.Euler(Vector3.Slerp(_prevQuat, _nextQuat, _currentAnimationTime));

            _currentAnimationTime += Time.fixedDeltaTime * _carAnimationSpeed;
            _transform.localPosition = desiredPosition;
            _transform.localRotation = desiredQuat;
            if(_currentAnimationTime >= 1f)
            {
                _transform.localPosition = _nextPosition;
                _transform.localRotation = Quaternion.identity;
                _currentAnimationTime = 0f;
            }
        }
    }
    public void InitializePosition(Vector3 nextPos)
    {
        _transform.localPosition = nextPos;
        _nextPosition = nextPos;
        _transform.localRotation = Quaternion.identity;
        _renderer.enabled = true;


    }
    public void MoveNextPos(Vector3 nextPos) 
    {
        _prevPosition = _transform.localPosition;
        _prevQuat = _transform.localRotation.eulerAngles;
        _nextPosition = nextPos;
        _nextQuat = new Vector3(0f, 0f, (nextPos.x > 0 ? -360f : 360f));
        _currentAnimationTime = 0f;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        gameObject.SetActive(true);
    }
}
