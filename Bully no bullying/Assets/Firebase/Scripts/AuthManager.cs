using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI; // For Popup

namespace Firebase.Scripts
{
    public class AuthManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_InputField emailInputField;
        public TMP_InputField passwordInputField;
        public TMP_Text statusText;
        public Toggle rememberMeToggle;

        [Header("Loading Indicator")]
        public GameObject loadingPanel;

        [Header("Login Popup")]
        public Popup loginPopup; // Assign Register - Log In popup here

        private FirebaseAuth firebaseAuthInstance;
        private FirebaseUser currentUser;

        // PlayerPrefs keys
        private const string RememberMeKey = "RememberMe";
        private const string SavedEmailKey = "SavedEmail";

        void Start()
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(false);

            LoadRememberedCredentials();
            InitAuth();
        }

        private void LoadRememberedCredentials()
        {
            bool rememberMe = PlayerPrefs.GetInt(RememberMeKey, 0) == 1;
            if (rememberMeToggle != null)
                rememberMeToggle.isOn = rememberMe;

            if (rememberMe && emailInputField != null)
            {
                string savedEmail = PlayerPrefs.GetString(SavedEmailKey, "");
                emailInputField.text = savedEmail;
            }
        }

        private void SaveRememberedCredentials()
        {
            if (rememberMeToggle != null && rememberMeToggle.isOn)
            {
                PlayerPrefs.SetInt(RememberMeKey, 1);
                PlayerPrefs.SetString(SavedEmailKey, emailInputField.text);
            }
            else
            {
                PlayerPrefs.SetInt(RememberMeKey, 0);
                PlayerPrefs.DeleteKey(SavedEmailKey);
            }
            PlayerPrefs.Save();
        }

        public void InitAuth()
        {
            firebaseAuthInstance = FirebaseAuth.DefaultInstance;
            firebaseAuthInstance.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            Debug.Log("AuthManager: Firebase Auth initialized.");
        }

        void OnDestroy()
        {
            if (firebaseAuthInstance != null)
                firebaseAuthInstance.StateChanged -= AuthStateChanged;
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (firebaseAuthInstance.CurrentUser != currentUser)
            {
                bool signedIn = firebaseAuthInstance.CurrentUser != null;
                currentUser = firebaseAuthInstance.CurrentUser;

                if (signedIn)
                {
                    Debug.LogFormat("Signed in user: {0} ({1})",
                        currentUser.DisplayName ?? "No Display Name", currentUser.Email);
                    UpdateStatus("Currently signed in as: " + currentUser.Email);

                    if (loginPopup != null)
                        loginPopup.Close();

                    LoadMainGameScene();
                }
                else
                {
                    Debug.Log("No user currently signed in.");
                    UpdateStatus("Please sign in or register.");

                    if (loginPopup != null)
                        loginPopup.Open();
                }
            }
        }

        private void SetUIInteractable(bool interactable)
        {
            if (emailInputField != null) emailInputField.interactable = interactable;
            if (passwordInputField != null) passwordInputField.interactable = interactable;
            if (rememberMeToggle != null) rememberMeToggle.interactable = interactable;

            if (loadingPanel != null)
                loadingPanel.SetActive(!interactable);
        }

        public void RegisterUser()
        {
            string email = emailInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                UpdateStatus("Please enter both email and password.");
                return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                UpdateStatus("Please enter a valid email address.");
                return;
            }
            if (password.Length < 6)
            {
                UpdateStatus("Password must be at least 6 characters long.");
                return;
            }

            SetUIInteractable(false);

            firebaseAuthInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                SetUIInteractable(true);

                if (task.IsCanceled)
                {
                    Debug.LogError("Registration cancelled.");
                    UpdateStatus("Registration cancelled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Registration error: " + task.Exception);
                    UpdateStatus("Registration failed.");
                    return;
                }

                SaveRememberedCredentials();

                AuthResult result = task.Result;
                currentUser = result.User;
                UpdateStatus("Registration Successful! Signed in as: " + currentUser.Email);

                HandleAuthSuccessTransition();
            });
        }

        public void LoginUser()
        {
            string email = emailInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                UpdateStatus("Please enter both email and password.");
                return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                UpdateStatus("Please enter a valid email address.");
                return;
            }

            SetUIInteractable(false);

            firebaseAuthInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                SetUIInteractable(true);

                if (task.IsCanceled)
                {
                    Debug.LogError("Login cancelled.");
                    UpdateStatus("Login cancelled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Login error: " + task.Exception);
                    UpdateStatus("Login failed.");
                    return;
                }

                SaveRememberedCredentials();

                AuthResult result = task.Result;
                currentUser = result.User;
                UpdateStatus("Login Successful! Signed in as: " + currentUser.Email);

                HandleAuthSuccessTransition();
            });
        }

        public void SignOutUser()
        {
            if (firebaseAuthInstance != null && currentUser != null)
            {
                firebaseAuthInstance.SignOut();
                UpdateStatus("You have been signed out.");
                Debug.Log("User signed out.");
            }
            else
            {
                UpdateStatus("No user currently signed in to sign out.");
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        private void HandleAuthSuccessTransition()
        {
            emailInputField.text = "";
            passwordInputField.text = "";
            LoadMainGameScene();
        }

        private void LoadMainGameScene()
        {
            Debug.Log("Loading main game scene.");
        }

        private void LoadLoginScene()
        {
            Debug.Log("Loading login scene.");
        }
    }
}
