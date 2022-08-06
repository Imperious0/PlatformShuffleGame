using UnityEngine;

public class TruckUnloader : MonoBehaviour, IContactable
{
    [SerializeField] private Transform _lPark;
    [SerializeField] private Transform _rPark;

    public void InteractOTG()
    {

    }

    public void InteractOTG(int index)
    {

    }

    internal Transform[] getCarParks()
    {
        return new Transform[]{ _lPark, _rPark };
    }
}
