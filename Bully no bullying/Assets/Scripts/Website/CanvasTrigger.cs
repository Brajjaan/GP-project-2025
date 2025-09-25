using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTrigger : MonoBehaviour
{
    public CanvasManagerChatroom chatroomCanvasManager;

    void OnMouseDown() // for PC
    {
        TriggerCanvas();
    }

    void Update() // for mobile
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    TriggerCanvas();
                }
            }
        }
    }

    public void TriggerCanvas()
    {
        if (chatroomCanvasManager != null)
        {
            chatroomCanvasManager.EnableCanvas();
        }
        else
        {
            Debug.LogWarning("CanvasTrigger: CanvasManagerChatroom not assigned!");
        }
    }
}
