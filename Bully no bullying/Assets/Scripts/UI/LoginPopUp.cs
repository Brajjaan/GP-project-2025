using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UI
{
    public class LoginPopup : MonoBehaviour
    {
        public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

        private GameObject m_background;
        private Canvas m_canvas;

        public TMP_Text errorMessageText; 

        private Action _onOkCallback;

        private void Awake()
        {
            Debug.Log($"<color=blue>[{Time.frameCount}] Popup.Awake() for {gameObject.name} (ID: {GetInstanceID()}).</color> activeSelf: {gameObject.activeSelf}, activeInHierarchy: {gameObject.activeInHierarchy}. m_background state: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");
            m_background = null;
        }

        private void OnEnable()
        {
            Debug.Log($"<color=blue>[{Time.frameCount}] Popup.OnEnable() for {gameObject.name} (ID: {GetInstanceID()}).</color> activeSelf: {gameObject.activeSelf}, activeInHierarchy: {gameObject.activeInHierarchy}. m_background state: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");
        }

        private void Start()
        {
            if (m_canvas == null)
            {
                m_canvas = FindObjectOfType<Canvas>();
                if (m_canvas == null)
                {
                    Debug.LogError("Popup: No Canvas found in the scene! UI will not render correctly for popup: " + gameObject.name);
                }
            }
        }

        public void Open(string caller = "UnknownCaller", Action onOk = null) // <<< MODIFIED: Accept a callback
        {
            gameObject.SetActive(true); 

            AddBackground();

            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"Popup.Open() for {gameObject.name}: Playing 'Open' animation.");
                animator.Play("Open");
            }
            else
            {
                Debug.LogWarning($"Popup.Open() for {gameObject.name}: No Animator found.");
            }
            
            _onOkCallback = onOk;
        }

        public void Close()
        {
            Debug.Log($"<color=red>[{Time.frameCount}] Popup.Close() called for {gameObject.name} (ID: {GetInstanceID()}).</color>");
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            var animator = GetComponent<Animator>();
            if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                animator.Play("Close");
            }
            else if (animator != null)
            {
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                }
            }
            else
            {
                Debug.LogWarning($"Popup.Close() for {gameObject.name}: No Animator found. Manual hide might be needed.");
            }

            RemoveBackground();

            StartCoroutine(HideAfterDelay(0.5f));
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            Debug.Log($"Popup.HideAfterDelay() for {gameObject.name} (ID: {GetInstanceID()}): Waiting {delay} seconds.");
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        private void AddBackground()
        {
            Debug.Log($"<color=orange>[{Time.frameCount}] Popup.AddBackground() called for {gameObject.name} (Popup ID: {GetInstanceID()}).</color> m_background before check: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");

            if (m_background != null)
            {
                Debug.LogWarning($"<color=orange>[{Time.frameCount}] Popup.AddBackground() for {gameObject.name}: Existing m_background (ID: {m_background.GetInstanceID()}) found. Destroying it before creating new.</color>");
                Destroy(m_background);
                m_background = null;
            }
            else
            {
                Debug.Log($"<color=orange>[{Time.frameCount}] Popup.AddBackground() for {gameObject.name}: No existing m_background found. Proceeding to create new.</color>");
            }

            if (m_canvas == null)
            {
                m_canvas = FindObjectOfType<Canvas>();
                if (m_canvas == null)
                {
                    Debug.LogError("Popup: No Canvas found in the scene to attach background to for popup: " + gameObject.name);
                    return;
                }
            }

            var bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, backgroundColor);
            bgTex.Apply();

            m_background = new GameObject($"PopupBackground_{gameObject.name}_AssociatedWith_{GetInstanceID()}");
            
            var image = m_background.AddComponent<Image>();
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
            image.sprite = sprite;
            image.color = backgroundColor;

            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 0.4f, false);

            m_background.transform.SetParent(m_canvas.transform, false);
            
            RectTransform bgRectTransform = m_background.GetComponent<RectTransform>();
            if (bgRectTransform == null)
                bgRectTransform = m_background.AddComponent<RectTransform>();

            bgRectTransform.anchorMin = Vector2.zero;
            bgRectTransform.anchorMax = Vector2.one;
            bgRectTransform.sizeDelta = Vector2.zero;
            bgRectTransform.anchoredPosition = Vector2.zero;

            m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());

            Debug.Log($"<color=orange>[{Time.frameCount}] Popup.AddBackground() for {gameObject.name}: NEW background created (ID: {m_background.GetInstanceID()}). Parent: {m_canvas.name}. Sibling Index: {m_background.transform.GetSiblingIndex()}.</color>");
        }

        private void RemoveBackground()
        {
            Debug.Log($"<color=red>[{Time.frameCount}] Popup.RemoveBackground() called for {gameObject.name} (Popup ID: {GetInstanceID()}).</color> m_background before check: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");

            if (m_background != null)
            {
                Debug.Log($"<color=red>[{Time.frameCount}] Popup.RemoveBackground() for {gameObject.name}: Fading out and destroying m_background (ID: {m_background.GetInstanceID()}) with delay.</color>");
                var image = m_background.GetComponent<Image>();
                if (image != null)
                {
                    image.CrossFadeAlpha(0.0f, 0.2f, false);
                }
                else
                {
                    Debug.LogWarning($"<color=red>[{Time.frameCount}] Popup.RemoveBackground() for {gameObject.name}: Image component on m_background is null. This should not happen.</color>");
                }

                Destroy(m_background, 0.25f);
                m_background = null;
            }
            else
            {
                Debug.LogWarning($"<color=red>[{Time.frameCount}] Popup.RemoveBackground() for {gameObject.name}: m_background is null. Nothing to destroy.</color>");
            }
        }

        public void SetMessage(string message)
        {
            if (errorMessageText != null)
            {
                errorMessageText.text = message;
                Debug.Log($"LoginPopup: Set message for {gameObject.name} to: '{message}'");
            }
            else
            {
                Debug.LogWarning($"LoginPopup: errorMessageText not assigned for {gameObject.name}. Message '{message}' will not be displayed.");
            }
        }
        
        public void OnOkButtonClicked()
        {
            Debug.Log($"LoginPopup: OK button clicked for {gameObject.name}.");
            Close();


            _onOkCallback?.Invoke();
            _onOkCallback = null; 
        }
    }
}
