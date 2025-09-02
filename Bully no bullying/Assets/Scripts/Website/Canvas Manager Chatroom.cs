using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManagerChatroom : MonoBehaviour
{
    public Canvas canvas;
    
    public void EnableCanvas()
    {
        canvas.gameObject.SetActive(true);
    }
    
    public void DisableCanvas()
    {
        canvas.gameObject.SetActive(false);
    }
}
