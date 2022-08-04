using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Renderer)), RequireComponent(typeof(Collider))]
public class PrizeDoor : MonoBehaviour, IVisibleObject, IContactable, IPrize
{
    public event EventHandler<PrizeCollectedArgs> PrizeCollectedEvent;
    [Header("Prefabs")]
    [SerializeField] private TextMeshPro _prizeText;

    [Space, SerializeField] private int _prizeIndex = 0;
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private Vector2 _moveAbleOffset;

    [Space, SerializeField] private PrizeType _pType;
    [SerializeField] private Color32 _positiveColor;
    [SerializeField] private Color32 _negativeColor;
    [SerializeField] private string _pText = "+";
    [SerializeField] private int _prizeAmount = 1;
    [SerializeField] private int _prizeAmountPrimitiveMax = 15;
    [SerializeField] private int _prizeAmountComplexMax = 4;

    private bool isDynamic = false;

    private Renderer _renderer;
    private Collider _collider;
    private Transform _transform;
    private Vector3 _desiredPosition;
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _transform = transform;
        _initialPosition = _transform.localPosition;
        _desiredPosition = _initialPosition;

        transform.parent.GetComponent<RoadController>().HideEvent += Hide;
        transform.parent.GetComponent<RoadController>().SeekEvent += Seek;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDynamic)
            return;

        float timescale = Mathf.PingPong(Time.time, 1);
        _desiredPosition.x = Mathf.Lerp(_moveAbleOffset.x, _moveAbleOffset.y, timescale);
        _transform.localPosition = _desiredPosition;
    }

    public void Hide(object sender, HideEventArgs e)
    {
        HideIt();
    }
    private void HideIt()
    {
        _collider.enabled = false;
        _renderer.enabled = false;
        _prizeText.enabled = false;
        isDynamic = false;
    }
    public void InteractOTG()
    {
        //NOP
    }
    public void InteractOTG(int index)
    {
        PrizeCollectedEvent?.Invoke(this, new PrizeCollectedArgs(index, _pType, _prizeAmount));
        HideIt();
    }
    public void Seek(object sender, SeekEventArgs e)
    {
        if (e.VisibleDecisionArray[_prizeIndex] < 2)
            return;
        bool isAbleToMove = true;
        for (int i = 0; i < e.VisibleDecisionArray.Length; i++)
        {
            if (i == _prizeIndex)
                continue;
            if (e.VisibleDecisionArray[i] > 1)
                isAbleToMove = false;
        }
        if(isAbleToMove)
        {
            int decide = UnityEngine.Random.Range(0, 4);
            if (decide > 2)
                isDynamic = true;
            else
                isDynamic = false;
        }
        else
        {
            isDynamic = isAbleToMove;
        }
        if(!isDynamic)
        {
            _transform.localPosition = _initialPosition;
        }
        Array values = Enum.GetValues(typeof(PrizeType));
        System.Random random = new System.Random();
        _pType = (PrizeType)values.GetValue(random.Next(values.Length));

        _collider.enabled = true;
        _renderer.enabled = true;
        _prizeText.enabled = true;
        createPrize();
    }
    private void createPrize()
    {
        switch (_pType)
        {
            case PrizeType.ADD:
                _prizeAmount = UnityEngine.Random.Range(1, _prizeAmountPrimitiveMax);
                _pText = "+";
                _renderer.material.color = _positiveColor;
                break;
            case PrizeType.REMOVE:
                _prizeAmount = UnityEngine.Random.Range(1, _prizeAmountPrimitiveMax);
                _pText = "-";
                _renderer.material.color = _negativeColor;
                break;
            case PrizeType.MUL:
                _prizeAmount = UnityEngine.Random.Range(1, _prizeAmountComplexMax);
                _pText = "x";
                _renderer.material.color = _positiveColor;
                break;
            case PrizeType.DIV:
                _prizeAmount = UnityEngine.Random.Range(1, _prizeAmountComplexMax);
                _pText = "÷";
                _renderer.material.color = _negativeColor;
                break;
            default:
                break;
        }
        RefreshGUI();
    }

    private void RefreshGUI()
    {
        _prizeText.text = _pText + "" + _prizeAmount;
    }
}

public enum PrizeType
{
    ADD, REMOVE, MUL, DIV
}
public class PrizeCollectedArgs : EventArgs
{
    private int _truckIndex;
    private PrizeType _pType;
    private int _prizeAmount;

    public PrizeCollectedArgs(int truckIndex, PrizeType pType, int prizeAmount)
    {
        _truckIndex = truckIndex;
        _pType = pType;
        _prizeAmount = prizeAmount;
    }

    public int TruckIndex { get => _truckIndex; }
    public PrizeType PType { get => _pType; }
    public int PrizeAmount { get => _prizeAmount; }
}