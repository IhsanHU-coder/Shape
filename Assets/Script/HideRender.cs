using UnityEngine;

public class HideRender : MonoBehaviour
{
        private void OnTriggerEnter(Collider other)
{
    Debug.Log("Debug?");

    if(other.gameObject.CompareTag("Board")){
    //  this.gameObject.SetActive(false);
     Animator anim = this.gameObject.GetComponent<Animator>();

     anim.Play("GettingSmall");
    }
        
}

private void OnTriggerStay(Collider other)
{

    if(other.gameObject.CompareTag("Board")){
    // this.gameObject.SetActive(false);
    Animator anim = this.gameObject.GetComponent<Animator>();

     anim.Play("GettingSmall");
    }
}

private void OnTriggerExit(Collider other)
{
    if (!other.CompareTag("Board"))
        return;

    Renderer rend = other.GetComponent<Renderer>();
Renderer rend2 = other.GetComponentInChildren<Renderer>();
    if (rend2 != null)
        rend2.enabled = true;
    if (rend != null)
        rend.enabled = true;
}
}
