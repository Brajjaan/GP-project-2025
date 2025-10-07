using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAllsaves : MonoBehaviour
{
    public void ResetAllSaves()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All save data has been reset.");
    }
}
