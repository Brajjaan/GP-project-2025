using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Popup : MonoBehaviour
    {
        public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        private GameObject m_background;

        // This makes the popup open as soon as the GameObject is active in the scene
        private void Start()
        {
            Open();
        }

        public void Open()
        {
            // Make sure the popup is active first
            gameObject.SetActive(true);

            AddBackground();

            var animator = GetComponent<Animator>();
            if (animator != null)
                animator.Play("Open");
        }


        public void Close()
        {
            var animator = GetComponent<Animator>();
            if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                animator.Play("Close");

            RemoveBackground();

            // Instead of destroying, just disable after animation time
            StartCoroutine(HideAfterDelay(0.5f));
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }


        private IEnumerator RunPopupDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(m_background);
            Destroy(gameObject);
        }

        private void AddBackground()
        {
            var bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, backgroundColor);
            bgTex.Apply();

            m_background = new GameObject("PopupBackground");
            var image = m_background.AddComponent<Image>();
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = bgTex;
            image.sprite = sprite;
            image.color = image.color;
            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 0.4f, false);

            var canvas = GameObject.Find("Canvas");
            m_background.transform.localScale = Vector3.one;
            m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
            m_background.transform.SetParent(canvas.transform, false);
            m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        private void RemoveBackground()
        {
            var image = m_background?.GetComponent<Image>();
            if (image != null)
                image.CrossFadeAlpha(0.0f, 0.2f, false);
        }
    }
}
