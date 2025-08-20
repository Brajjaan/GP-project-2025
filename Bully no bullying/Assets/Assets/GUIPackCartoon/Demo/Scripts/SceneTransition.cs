using UnityEngine;

namespace Ricimi
{
    public class SceneTransition : MonoBehaviour
    {
        public string scene = "<Insert scene name>";
        public float duration = 1.0f;
        public Color color = Color.black;

        public void PerformTransition()
        {
            if (AdsManager.Instance != null)
            {
            }
            else
            {
                Transition.LoadLevel(scene, duration, color);
            }
        }
    }
}