using UnityEngine;
using UnityEngine.EventSystems;
using Interfaces;

public class TouchInteraction : MonoBehaviour
{
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector2? touchPos = GetInputPosition();
        if (touchPos.HasValue)
        {
            HandleTouch(touchPos.Value);
        }
    }

    private Vector2? GetInputPosition()
    {
        // Mouse input
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
                return Input.mousePosition;
        }

        // Touch input
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !IsPointerOverUI(touch.fingerId))
                return touch.position;
        }

        return null;
    }

    private bool IsPointerOverUI(int fingerId = -1)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private void HandleTouch(Vector2 screenPos)
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.GetComponent<IInteractable>()?.Interact();
        }
    }
}