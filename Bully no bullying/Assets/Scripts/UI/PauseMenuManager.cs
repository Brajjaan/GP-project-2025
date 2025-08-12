using UnityEngine;

namespace UI
{
    public class PauseMenuManager : MonoBehaviour
    {
        public GameObject pauseButton;
        public GameObject pauseMenu;
    
        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            pauseButton.SetActive(true);
            Time.timeScale = 1f;
        }

        public void ExitGame()
        {
            Application.Quit();
            Debug.Log("App exited");
        }
    }
}