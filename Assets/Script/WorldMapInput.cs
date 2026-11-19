using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapInput : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = worldCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            LevelNode level = hit.collider.GetComponent<LevelNode>();

            if (level == null)
                return;

            WorldMapManager.Instance.SelectLevel(level);
        }
    }
}