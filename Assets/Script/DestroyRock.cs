using Unity.VisualScripting;
using UnityEngine;

public class DestroyRock : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Destroy(gameObject);
        }
    }
}
