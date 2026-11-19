using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 moveDirection = Vector3.right;
    [SerializeField] private float speed = 1f;

    [Header("Loop")]
    [SerializeField] private float resetDistance = 100f;
    [SerializeField] private Vector3 resetOffset = new Vector3(-200f, 0f, 0f);

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        moveDirection.Normalize();
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= resetDistance)
        {
            transform.position = startPosition + resetOffset;
        }
    }
}