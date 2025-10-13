using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class DeleteAccountManager : MonoBehaviour
{
    public Text messageText;
    private FirebaseAuth auth;
    private string mainThreadMessage = null;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(mainThreadMessage))
        {
            if (messageText != null)
                messageText.text = mainThreadMessage;
            Debug.Log(mainThreadMessage);
            mainThreadMessage = null;
        }
    }

    public void OnDeleteAccountButtonPressed()
    {
        DeleteCurrentUser();
    }

    public void DeleteCurrentUser()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            ShowMessage("No user is signed in.");
            return;
        }

        user.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                ShowMessage("Account deletion was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                ShowMessage("Error deleting account: " + task.Exception);
                return;
            }

            ShowMessage("Account deleted successfully.");
        });
    }

    private void ShowMessage(string msg)
    {
        mainThreadMessage = msg;
    }
}