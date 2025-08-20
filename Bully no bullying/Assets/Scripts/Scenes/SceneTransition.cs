using UnityEngine;

namespace Scenes
{
    public class SceneTransition : MonoBehaviour
    {
        public string scene = "<Scene Name>";
        public float duration = 1.0f;
        public Color color = Color.black;

        public void PerformTransition()
        {
            if (AdsManager.Instance != null)
            {
                AdsManager.Instance.ShowInterstitial();
                StartCoroutine(WaitForAdThenTransition());
            }
            else
            {
                Transition.LoadLevel(scene, duration, color);
            }
        }

        private System.Collections.IEnumerator WaitForAdThenTransition()
        {
            yield return new WaitUntil(() => !UnityEngine.Advertisements.Advertisement.isShowing);
            Transition.LoadLevel(scene, duration, color);
        }
    }
}