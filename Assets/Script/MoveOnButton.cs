using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MoveOnButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private Animator animCam;
    [SerializeField] private string hoverAnimation = "PopUp";

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetCamera;
    [SerializeField] private Transform cameraPositionA;
    [SerializeField] private Transform cameraPositionB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    public float timeWait = 3f;

    private Vector3 cameraTargetPos;
    private Quaternion cameraTargetRot;

    private bool isInSetting = false;

    private void Start()
    {
        animCam.Play("CamAnim");
        cameraTargetPos = cameraPositionA.position;
        cameraTargetRot = cameraPositionA.rotation;

        targetCamera.SetPositionAndRotation(cameraPositionA.position, cameraPositionA.rotation);
    }

    private void Update()
    {
        targetCamera.position = Vector3.Lerp(
            targetCamera.position,
            cameraTargetPos,
            moveSpeed * Time.deltaTime);

        targetCamera.rotation = Quaternion.Slerp(
            targetCamera.rotation,
            cameraTargetRot,
            rotationSpeed * Time.deltaTime);

        // Klik kiri
        if (!isInSetting && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Setting"))
                {
                    OpenSetting();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.Play(hoverAnimation);
    }

    public void OpenSetting()
    {
        isInSetting = true;
        StopCoroutine(waitAnim());
        StopAllCoroutines();
        animCam.enabled = false;
        cameraTargetPos = cameraPositionB.position;
        cameraTargetRot = cameraPositionB.rotation;
        
    }

    // Hubungkan ke tombol Done
    public void CloseSetting()
    {
        
        StartCoroutine(waitAnim());
        isInSetting = false;
        cameraTargetPos = cameraPositionA.position;
        cameraTargetRot = cameraPositionA.rotation;
    }

    IEnumerator waitAnim()
    {
        yield return new WaitForSeconds(timeWait);
        
        animCam.enabled = true;
        animCam.Play("CamAnim",0,0f);
        

    }
}