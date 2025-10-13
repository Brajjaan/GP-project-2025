using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;
using System.Collections;
using System;
using System.Threading.Tasks;

namespace Firebase.Scripts
{
    public class AuthManager : MonoBehaviour
    {
        [Header("UI Elements (Login)")]
        public TMP_InputField loginEmailInputField;
        public TMP_InputField loginPasswordInputField;
        public Toggle rememberMeToggle;

        [Header("UI Elements (Signup)")]
        public TMP_InputField signupEmailInputField;
        public TMP_InputField signupPasswordInputField;
        public Toggle showPasswordToggle;

        [Header("Common UI Elements")]
        public TMP_Text statusText;

        [Header("User Info Display")]
        public TMP_Text currentEmailText; // 👈 NEW FIELD for showing current user's email

        [Header("Popups")]
        public LoginPopup loginPopup;
        public LoginPopup signupPopup;
        public LoginPopup errorPopup;
        public LoginPopup successPopup;

        private FirebaseAuth firebaseAuthInstance;
        private FirebaseUser currentUser;
        private bool _isSigningInManually = false;
        private bool _loginPopupHasBeenOpened = false;
        
        private const string RememberMeKey = "RememberMe";
        private const string SavedEmailKey = "SavedEmail";

        /* private IEnumerator Start()
        {
            Debug.Log("[Auth] Checking Firebase dependencies...");
            var checkTask = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => checkTask.IsCompleted);

            if (checkTask.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("[Auth] Firebase dependencies ready.");
                LoadRememberedCredentials();
                InitAuth();
            }
            else
            {
                Debug.LogError($"[Auth] Could not resolve Firebase dependencies: {checkTask.Result}");
                ShowErrorPopup("Firebase initialization failed. Please restart the game.");
            }
        } */
        private IEnumerator Start()
        {
            Debug.Log("[Auth] Waiting for FirebaseInitializer to complete...");

            // Wait until FirebaseInitializer reports ready
            while (FirebaseInitializer.database == null)
                yield return null;

            Debug.Log("[Auth] Firebase ready, initializing Auth...");
            LoadRememberedCredentials();
            InitAuth();
        }

        
        public void OnRememberMeToggleChanged(bool isOn)
        {
            SaveRememberedCredentials();
            Debug.Log($"Remember Me toggle changed to: {isOn}. Credentials saved/cleared.");
        }

        private void LoadRememberedCredentials()
        {
            bool rememberMe = PlayerPrefs.GetInt(RememberMeKey, 0) == 1;
            if (rememberMeToggle != null)
                rememberMeToggle.isOn = rememberMe;

            if (rememberMe && loginEmailInputField != null)
                loginEmailInputField.text = PlayerPrefs.GetString(SavedEmailKey, "");
        }

        private void SaveRememberedCredentials()
        {
            if (rememberMeToggle != null && rememberMeToggle.isOn)
            {
                PlayerPrefs.SetInt(RememberMeKey, 1);
                if (loginEmailInputField != null)
                    PlayerPrefs.SetString(SavedEmailKey, loginEmailInputField.text);
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
            Debug.Log("[Auth] Firebase Auth initialized.");
        }

        void OnDestroy()
        {
            if (firebaseAuthInstance != null)
                firebaseAuthInstance.StateChanged -= AuthStateChanged;
        }

        void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            FirebaseUser newCurrentUser = firebaseAuthInstance.CurrentUser;

            if (newCurrentUser != currentUser || (currentUser == null && newCurrentUser == null))
            {
                currentUser = newCurrentUser;
                bool nowSignedIn = (currentUser != null);

                if (nowSignedIn)
                {
                    Debug.LogFormat("AuthStateChanged: User signed in: {0} ({1})",
                        currentUser.DisplayName ?? "No Display Name", currentUser.Email);
                    
                    if (!_isSigningInManually && rememberMeToggle != null && !rememberMeToggle.isOn)
                    {
                        Debug.Log("AuthStateChanged: User auto-signed in (not manual login), but 'Remember Me' is OFF. Signing out now.");
                        firebaseAuthInstance.SignOut();
                        return; 
                    }

                    UpdateStatus("Currently signed in as: " + currentUser.Email);
                    UpdateCurrentEmailDisplay(); // 👈 Update email display

                    if (loginPopup != null && loginPopup.gameObject.activeInHierarchy) loginPopup.Close();
                    if (signupPopup != null && signupPopup.gameObject.activeInHierarchy) signupPopup.Close();
                    if (errorPopup != null && errorPopup.gameObject.activeInHierarchy) errorPopup.Close();
                    if (successPopup != null && successPopup.gameObject.activeInHierarchy) successPopup.Close();

                    Debug.Log("AuthStateChanged: SetUIInteractable(true) after successful sign-in."); 
                    SetUIInteractable(true);
                    _loginPopupHasBeenOpened = false;
                    HandleAuthSuccessTransition();
                }
                else
                {
                    Debug.Log("AuthStateChanged: No user currently signed in."); 
                    UpdateStatus("Please sign in or register.");
                    UpdateCurrentEmailDisplay(); // 👈 Update to show "Not signed in"

                    if (loginPopup != null && !_isSigningInManually && !_loginPopupHasBeenOpened)
                    {
                        loginPopup.Open("AuthManager.AuthStateChanged");
                        _loginPopupHasBeenOpened = true;
                    }

                    if (loginPasswordInputField != null) loginPasswordInputField.text = "";
                    if (signupPasswordInputField != null) signupPasswordInputField.text = "";
                    
                    Debug.Log("AuthStateChanged: SetUIInteractable(true) when no user is signed in.");
                    SetUIInteractable(true);
                }
            }
        }

        private void SetUIInteractable(bool interactable)
        {
            Debug.Log($"<color=cyan>--- SetUIInteractable called with: {interactable} ---</color>");
            if (loginEmailInputField != null) loginEmailInputField.interactable = interactable;
            if (loginPasswordInputField != null) loginPasswordInputField.interactable = interactable;
            if (signupEmailInputField != null) signupEmailInputField.interactable = interactable;
            if (signupPasswordInputField != null) signupPasswordInputField.interactable = interactable;
            if (rememberMeToggle != null) rememberMeToggle.interactable = interactable;
            if (showPasswordToggle != null) showPasswordToggle.interactable = interactable;
        }

        private void ShowErrorPopup(string message)
        {
            Debug.LogWarning("Error Popup: " + message);
            if (errorPopup != null)
            {
                errorPopup.SetMessage(message);
                errorPopup.Open("AuthManager.ShowErrorPopup");
            }
            else
            {
                Debug.LogWarning("ErrorPopup reference is null.");
            }
            SetUIInteractable(true); 
        }

        private string GetAuthErrorMessage(AggregateException exception)
        {
            string defaultMessage = "An unexpected error occurred. Please try again.";
            if (exception == null || exception.InnerExceptions.Count == 0)
                return defaultMessage;

            Exception innerEx = exception.InnerExceptions[0];
            string firebaseMessage = innerEx.Message;

            if (firebaseMessage.Contains("email address is badly formatted"))
                return "The email address you entered is not valid.";
            if (firebaseMessage.Contains("no user record") || firebaseMessage.Contains("user-not-found"))
                return "No account found with this email address.";
            if (firebaseMessage.Contains("password is invalid") || firebaseMessage.Contains("wrong-password"))
                return "Incorrect password.";
            if (firebaseMessage.Contains("must be 6 characters") || firebaseMessage.Contains("weak-password"))
                return "The password is too weak.";
            if (firebaseMessage.Contains("already in use"))
                return "This email address is already registered.";
            if (firebaseMessage.Contains("user-disabled"))
                return "This account has been disabled.";
            if (firebaseMessage.Contains("operation-not-allowed"))
                return "Authentication method not enabled.";
            if (firebaseMessage.Contains("account-exists-with-different-credential"))
                return "Account exists with the same email but different sign-in credentials.";

            return firebaseMessage;
        }

        public void RegisterUser()
        {
            string email = signupEmailInputField?.text ?? "";
            string password = signupPasswordInputField?.text ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowErrorPopup("Please enter both email and password.");
                return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                ShowErrorPopup("Please enter a valid email address.");
                return;
            }
            if (password.Length < 6)
            {
                ShowErrorPopup("Password must be at least 6 characters long.");
                return;
            }

            SetUIInteractable(false);
            _isSigningInManually = true; 
            Debug.Log("[Auth] Starting registration attempt...");

            firebaseAuthInstance.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    _isSigningInManually = false; 

                    if (task.IsCanceled)
                    {
                        ShowErrorPopup("Registration cancelled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        string msg = GetAuthErrorMessage(task.Exception as AggregateException);
                        ShowErrorPopup(msg); 
                        return;
                    }
                    Debug.Log("[Auth] User registered successfully. Signing out immediately to present custom success message.");
                        
                    firebaseAuthInstance.SignOut(); 
                    UpdateCurrentEmailDisplay(); // 👈 Make sure it clears email

                    if (successPopup != null)
                    {
                        successPopup.SetMessage("Registration successful! Please sign in with your new account.");
                        successPopup.Open(
                            "AuthManager.RegisterSuccess",
                            () =>
                            {
                                if (loginPopup != null)
                                    loginPopup.Open("AuthManager.RegisterSuccessCallback");
                                else
                                    Debug.LogError("AuthManager: Login Popup reference is null after successful registration callback.");
                            }
                        );
                    }
                    else
                    {
                        Debug.LogError("AuthManager: Success Popup reference is null. Falling back to direct login popup.");
                        if (loginPopup != null) loginPopup.Open();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); 
        }

        public void LoginUser()
        {
            string email = loginEmailInputField?.text ?? "";
            string password = loginPasswordInputField?.text ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowErrorPopup("Please enter both email and password.");
                return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                ShowErrorPopup("Please enter a valid email address.");
                return;
            }

            SetUIInteractable(false);
            _isSigningInManually = true;
            Debug.Log("[Auth] Starting login attempt...");

            firebaseAuthInstance.SignInWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    _isSigningInManually = false; 

                    if (task.IsCanceled)
                    {
                        ShowErrorPopup("Login cancelled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        string msg = GetAuthErrorMessage(task.Exception as AggregateException);
                        ShowErrorPopup(msg);
                        return;
                    }

                    Debug.Log("[Auth] User logged in successfully.");
                    UpdateCurrentEmailDisplay(); // 👈 Show signed-in email

                    if (rememberMeToggle != null && rememberMeToggle.isOn)
                        SaveRememberedCredentials();
                    else
                    {
                        PlayerPrefs.SetInt(RememberMeKey, 0);
                        PlayerPrefs.DeleteKey(SavedEmailKey);
                        PlayerPrefs.Save();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); 
        }

        public void SignOutUser()
        {
            if (firebaseAuthInstance != null && currentUser != null)
            {
                _isSigningInManually = false; 
                firebaseAuthInstance.SignOut();
                currentUser = null;
                UpdateCurrentEmailDisplay(); // 👈 Clear the text
                UpdateStatus("You have been signed out.");
                Debug.Log("[Auth] User signed out.");
                _loginPopupHasBeenOpened = false; 
            }
            else
            {
                UpdateStatus("No user currently signed in.");
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        private void UpdateCurrentEmailDisplay()
        {
            if (currentEmailText != null)
            {
                if (currentUser != null)
                {
                    currentEmailText.text = $"Logged in as: {currentUser.Email}";
                    Debug.Log($"[Auth] Displaying current user email: {currentUser.Email}");
                }
                else
                {
                    currentEmailText.text = "Not signed in";
                    Debug.Log("[Auth] No user signed in; clearing email display.");
                }
            }
            else
            {
                Debug.LogWarning("[Auth] currentEmailText reference is missing in Inspector!");
            }
        }

        private void HandleAuthSuccessTransition()
        {
            if (loginEmailInputField != null) loginEmailInputField.text = "";
            if (loginPasswordInputField != null) loginPasswordInputField.text = "";
            if (signupEmailInputField != null) signupEmailInputField.text = "";
            if (signupPasswordInputField != null) signupPasswordInputField.text = "";
        }

        public void TryAgainFromError(bool goToLogin)
        {
            if (errorPopup != null) errorPopup.Close();

            if (goToLogin && loginPopup != null) loginPopup.Open();
            else if (!goToLogin && signupPopup != null) signupPopup.Open();
            SetUIInteractable(true);
        }

        public void OnShowPasswordToggleChanged(bool showPassword)
        {
            if (signupPasswordInputField != null)
            {
                string currentText = signupPasswordInputField.text;
                int caretPosition = signupPasswordInputField.caretPosition;
                int selectionAnchorPosition = signupPasswordInputField.selectionAnchorPosition;
                int selectionFocusPosition = signupPasswordInputField.selectionFocusPosition;

                signupPasswordInputField.contentType = showPassword
                    ? TMP_InputField.ContentType.Standard
                    : TMP_InputField.ContentType.Password;

                signupPasswordInputField.text = currentText;
                signupPasswordInputField.textComponent.ForceMeshUpdate();
                LayoutRebuilder.MarkLayoutForRebuild(signupPasswordInputField.GetComponent<RectTransform>());

                if (signupPasswordInputField.isFocused)
                {
                    signupPasswordInputField.caretPosition = caretPosition;
                    signupPasswordInputField.selectionAnchorPosition = selectionAnchorPosition;
                    signupPasswordInputField.selectionFocusPosition = selectionFocusPosition;
                }
            }
        }
    }
}
