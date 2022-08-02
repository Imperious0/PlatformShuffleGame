using UnityEngine;

public class TruckLooper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform != null)
        {

            IContactable t = other.transform.GetComponent(typeof(IContactable)) as IContactable;
            if (t != null)
                t.InteractOTG();
        }
    }
}
