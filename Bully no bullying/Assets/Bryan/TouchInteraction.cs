// TouchInteraction.cs
using UnityEngine;
using UnityEngine.EventSystems;
using Interfaces;

public class TouchInteraction : MonoBehaviour
{
    void Update()
    {
        // Only respond to the beginning of a touch
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Ignore touches on UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            HandleTouch(Input.GetTouch(0).position);
        }
#endif
    }

    void HandleTouch(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}