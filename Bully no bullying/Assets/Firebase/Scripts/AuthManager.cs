using Firebase.Auth;
using TMPro;
using UnityEngine;

namespace Firebase.Scripts
{
    public class AuthManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public TMP_InputField emailInputField;
        public TMP_InputField passwordInputField;
        public TMP_Text statusText;

        [Header("Loading Indicator")]
        // ***
    
        public GameObject loadingPanel; // Todo if needed
    
        // ***

        private FirebaseAuth firebaseAuthInstance;
        private FirebaseUser currentUser;

        void Start()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
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
            {
                firebaseAuthInstance.StateChanged -= AuthStateChanged;
            }
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (firebaseAuthInstance.CurrentUser != currentUser)
            {
                bool signedIn = firebaseAuthInstance.CurrentUser != null;
                if (!signedIn && currentUser != null)
                {
                    Debug.Log("Signed out user: " + currentUser.UserId);
                    UpdateStatus("You have been signed out.");
                    LoadLoginScene();
                }
                currentUser = firebaseAuthInstance.CurrentUser;

                if (signedIn)
                {
                    Debug.LogFormat("Signed in user: {0} ({1})",
                        currentUser.DisplayName ?? "No Display Name", currentUser.Email);
                    UpdateStatus("Currently signed in as: " + currentUser.Email);
                    LoadMainGameScene();
                }
                else
                {
                    Debug.Log("No user currently signed in.");
                    UpdateStatus("Please sign in or register.");
                }
            }
        }

        private void SetUIInteractable(bool interactable)
        {
            emailInputField.interactable = interactable;
            passwordInputField.interactable = interactable;

            if (loadingPanel != null)
            {
                loadingPanel.SetActive(!interactable);
            }
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
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    UpdateStatus("Registration cancelled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    if (task.Exception != null)
                    {
                        FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                        string errorMessage = "Registration failed.";
                        if (firebaseEx != null)
                        {
                            AuthError authError = (AuthError)firebaseEx.ErrorCode;
                            switch (authError)
                            {
                                case AuthError.EmailAlreadyInUse:
                                    errorMessage = "This email is already registered.";
                                    break;
                                case AuthError.WeakPassword:
                                    errorMessage = "Password is too weak. Needs at least 6 characters.";
                                    break;
                                case AuthError.InvalidEmail:
                                    errorMessage = "Invalid email format.";
                                    break;
                                case AuthError.OperationNotAllowed:
                                    errorMessage = "Email/Password sign-up is not enabled in Firebase Console. Please enable it in Authentication -> Sign-in method!";
                                    break;
                                default:
                                    errorMessage = $"Registration error: {firebaseEx.Message}";
                                    break;
                            }
                        }
                        UpdateStatus(errorMessage);
                    }

                    return;
                }

                AuthResult result = task.Result;
                currentUser = result.User;
                Debug.LogFormat("User registered successfully: {0} ({1})",
                    currentUser.DisplayName ?? "No Display Name", currentUser.Email);
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
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    UpdateStatus("Login cancelled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    if (task.Exception != null)
                    {
                        FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                        string errorMessage = "Login failed.";
                        if (firebaseEx != null)
                        {
                            AuthError authError = (AuthError)firebaseEx.ErrorCode;
                            switch (authError)
                            {
                                case AuthError.WrongPassword:
                                    errorMessage = "Incorrect password.";
                                    break;
                                case AuthError.UserNotFound:
                                    errorMessage = "No account found with this email.";
                                    break;
                                case AuthError.InvalidEmail:
                                    errorMessage = "Invalid email format.";
                                    break;
                                case AuthError.UserDisabled:
                                    errorMessage = "This account has been disabled.";
                                    break;
                                default:
                                    errorMessage = $"Login error: {firebaseEx.Message}";
                                    break;
                            }
                        }
                        UpdateStatus(errorMessage);
                    }

                    return;
                }

                AuthResult result = task.Result;
                currentUser = result.User;
                Debug.LogFormat("User logged in successfully: {0} ({1})",
                    currentUser.DisplayName ?? "No Display Name", currentUser.Email);
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
            {
                statusText.text = message;
            }
        }
    
        private void HandleAuthSuccessTransition()
        {
            emailInputField.text = "";
            passwordInputField.text = "";
            LoadMainGameScene();
        }

        private void LoadMainGameScene()
        {
            // ***
        
            // TODO SceneManager.LoadScene(" ");
        
            // ***
            Debug.Log("Loading main game scene.");
        }

        private void LoadLoginScene()
        {
            // ***
        
            // TODO SceneManager.LoadScene(" ");
        
            // ***
            Debug.Log("Loading login scene.");
        }
    }
}
