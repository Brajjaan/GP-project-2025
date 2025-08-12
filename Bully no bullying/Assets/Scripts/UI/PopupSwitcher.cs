using UnityEngine;

namespace UI
{
    public class PopupSwitcher : MonoBehaviour
    {
        public Popup loginPopup;
        public Popup signupPopup;

        public void ShowSignUp()
        {
            if (loginPopup != null)
                loginPopup.Close();

            if (signupPopup != null)
                signupPopup.Open();
        }

        public void ShowLogin()
        {
            if (signupPopup != null)
                signupPopup.Close();

            if (loginPopup != null)
                loginPopup.Open();
        }
    }
}