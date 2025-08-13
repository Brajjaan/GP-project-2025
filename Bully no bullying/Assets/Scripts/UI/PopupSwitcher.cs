using UnityEngine;

namespace UI
{
    public class PopupSwitcher : MonoBehaviour
    {
        public LoginPopup loginPopup;
        public LoginPopup signupPopup;
        public LoginPopup errorPopup;
        public LoginPopup signOutWarningPopup;

        private bool returnToLogin = true; // default

        public void ShowSignUp()
        {
            returnToLogin = false;

            if (loginPopup != null) loginPopup.Close();
            if (signupPopup != null) signupPopup.Open();
        }

        public void ShowLogin()
        {
            returnToLogin = true;

            if (signupPopup != null) signupPopup.Close();
            if (loginPopup != null) loginPopup.Open();
        }

        public void ShowError(bool backToLogin)
        {
            returnToLogin = backToLogin;

            if (loginPopup != null) loginPopup.Close();
            if (signupPopup != null) signupPopup.Close();
            if (errorPopup != null) errorPopup.Open();
        }

        public void TryAgainFromError()
        {
            if (errorPopup != null) errorPopup.Close();

            if (returnToLogin && loginPopup != null)
                loginPopup.Open();
            else if (!returnToLogin && signupPopup != null)
                signupPopup.Open();
        }

        public void ShowSignOutWarning()
        {
            if (loginPopup != null) loginPopup.Close();
            if (signupPopup != null) signupPopup.Close();
            if (errorPopup != null) errorPopup.Close();
            
            if (signOutWarningPopup != null) signOutWarningPopup.Open();
        }
    }
}
