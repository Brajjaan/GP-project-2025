using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class TestNPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interact");
    }
}
