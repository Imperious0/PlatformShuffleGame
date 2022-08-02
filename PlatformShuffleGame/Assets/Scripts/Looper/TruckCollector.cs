using UnityEngine;

public class TruckCollector : MonoBehaviour
{
    [SerializeField] private int _truckInteractID = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != null)
        {

            IContactable t = other.transform.GetComponent(typeof(IContactable)) as IContactable;
            if (t != null)
                t.InteractOTG(_truckInteractID);
        }
    }
}
