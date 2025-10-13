using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class FirebaseInitializer : MonoBehaviour
{
    private FirebaseApp app;
    public static FirebaseDatabase database;

    [Header("Events")]
    [SerializeField] public UnityEngine.Events.UnityEvent onFirebaseReady;

    void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Checking Firebase dependencies...");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("All Firebase dependencies are available. Initializing FirebaseApp and Database...");
                
                app = FirebaseApp.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                Debug.Log($"Firebase App initialized: {app.Name}");
                Debug.Log($"Firebase Realtime Database initialized. URL: {database.App.Options.DatabaseUrl}");

                DatabaseReference reference = database.RootReference;
                Debug.Log($"Successfully got database reference to: {reference.Key}");

                onFirebaseReady?.Invoke();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}\n" +
                               "Firebase Unity SDK is not safe to use here. Check your Firebase setup in Unity.");
            }
        });
    }
}