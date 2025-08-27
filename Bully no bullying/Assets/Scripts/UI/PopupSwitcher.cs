using UnityEngine;

namespace UI
{
    public class PopupSwitcher : MonoBehaviour
    {
        public LoginPopup loginPopup;
        public LoginPopup signupPopup;
        public LoginPopup errorPopup;
        public LoginPopup signOutWarningPopup;
        
        public LoginPopup informationalPopup1;
        public LoginPopup informationalPopup2;

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
        
        public void ShowInformational(int popupIndex)
        {
            if (loginPopup != null) loginPopup.Close();
            if (signupPopup != null) signupPopup.Close();
            if (errorPopup != null) errorPopup.Close();
            if (signOutWarningPopup != null) signOutWarningPopup.Close();
            
            if (popupIndex == 1 && informationalPopup1 != null)
            {
                informationalPopup1.Open();
            }
            else if (popupIndex == 2 && informationalPopup2 != null)
            {
                informationalPopup2.Open();
            }
        }
    }
}
