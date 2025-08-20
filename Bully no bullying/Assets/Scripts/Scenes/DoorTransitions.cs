using Interfaces;
using UnityEngine;

namespace Scenes
{
    public class DoorTransitions : MonoBehaviour, IInteractable
    {
        public string scene = "<Scene Name>";
        public float duration = 1.0f;
        public Color color = Color.black;
        
        public void Interact()
        {
            Transition.LoadLevel(scene, duration, color);
            Debug.Log("Transition Complete");
        }
    }
}