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
                // Prefer a Canvas on or above the popup (same hierarchy)
                m_canvas = GetComponentInParent<Canvas>();
                if (m_canvas == null)
                {
                    // fallback to global find
                    m_canvas = FindObjectOfType<Canvas>();
                }

                if (m_canvas == null)
                {
                    Debug.LogError("Popup: No Canvas found in the scene! UI will not render correctly for popup: " + gameObject.name);
                }
                else
                {
                    Debug.Log($"Popup: Using canvas '{m_canvas.name}' for popup '{gameObject.name}'.");
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
    Debug.Log($"Popup.AddBackground() for {gameObject.name}: m_background before check: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");

    // If there is already a background (maybe leftover) remove it first
    if (m_background != null)
    {
        Destroy(m_background);
        m_background = null;
    }

    // Make sure we have a canvas and prefer the one that's an ancestor of this popup
    if (m_canvas == null)
    {
        m_canvas = GetComponentInParent<Canvas>() ?? FindObjectOfType<Canvas>();
        if (m_canvas == null)
        {
            Debug.LogError("Popup: No Canvas found in the scene to attach background to for popup: " + gameObject.name);
            return;
        }
    }

    // Create a simple 1x1 texture and sprite for the background
    var bgTex = new Texture2D(1, 1);
    bgTex.SetPixel(0, 0, backgroundColor);
    bgTex.Apply();

    m_background = new GameObject($"PopupBackground_{gameObject.name}_AssociatedWith_{GetInstanceID()}");

    var image = m_background.AddComponent<Image>();
    var rect = new Rect(0, 0, bgTex.width, bgTex.height);
    var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
    image.sprite = sprite;
    image.color = backgroundColor;

    // IMPORTANT: let background receive raycasts so it blocks underlying UI,
    // but we will make sure popup is above it so popup is interactive.
    image.raycastTarget = true;

    // Parent the background to the SAME parent as the popup (so sibling indices are comparable)
    // If popup is not child of canvas directly, we attach to the popup's parent if possible.
    Transform targetParent = transform.parent != null ? transform.parent : m_canvas.transform;
    m_background.transform.SetParent(targetParent, false);

    // Make background stretch to fill parent
    RectTransform bgRectTransform = m_background.GetComponent<RectTransform>();
    if (bgRectTransform == null)
        bgRectTransform = m_background.AddComponent<RectTransform>();

    bgRectTransform.anchorMin = Vector2.zero;
    bgRectTransform.anchorMax = Vector2.one;
    bgRectTransform.sizeDelta = Vector2.zero;
    bgRectTransform.anchoredPosition = Vector2.zero;

    // Place the background right below the popup so popup remains on top.
    // If popup is under same parent, this will put background immediately before popup.
    int popupIndex = transform.GetSiblingIndex();
    int bgIndex = Mathf.Max(0, popupIndex);
    m_background.transform.SetSiblingIndex(bgIndex);
    // ensure popup is last sibling so it's visually above background and other siblings
    transform.SetAsLastSibling();

    // Fade in
    image.canvasRenderer.SetAlpha(0.0f);
    image.CrossFadeAlpha(1.0f, 0.4f, false);

    Debug.Log($"Popup.AddBackground() for {gameObject.name}: NEW background created (ID: {m_background.GetInstanceID()}). Parent: {targetParent.name}. Popup sibling index: {popupIndex}. Background sibling index: {m_background.transform.GetSiblingIndex()}.");
}


private void RemoveBackground()
{
    Debug.Log($"Popup.RemoveBackground() called for {gameObject.name}. m_background before check: {(m_background == null ? "null" : m_background.GetInstanceID().ToString())}");

    if (m_background != null)
    {
        var image = m_background.GetComponent<Image>();
        if (image != null)
            image.CrossFadeAlpha(0.0f, 0.2f, false);

        // Schedule destroy
        Destroy(m_background, 0.25f);
        m_background = null;
    }
    else
    {
        Debug.LogWarning($"Popup.RemoveBackground() for {gameObject.name}: m_background is null. Nothing to destroy.");
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
