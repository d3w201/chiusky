using UnityEngine;

public class Interact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if ("Chiusky".Equals(other.gameObject.tag))
        {
            ChiuskyController chiuskyCtrl = other.gameObject.GetComponent<ChiuskyController>();
            chiuskyCtrl.SetFocusItem(this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ("Chiusky".Equals(other.gameObject.tag))
        {
            var chiuskyCtrl = other.gameObject.GetComponent<ChiuskyController>();
            if (this.gameObject.Equals(chiuskyCtrl.GetFocusItem()))
            {
                chiuskyCtrl.SetFocusItem(null);
            }
        }
    }
}
