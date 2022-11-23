using UnityEngine;
using Firebase;
// using CodeStage.AntiCheat.Genuine.CodeHash;

public class FirebaseInit : MonoBehaviour
{
    //FirebaseApp app = null;
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                
                // Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}",dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        
        //CodeHashGenerator.AddToSceneOrGetExisting();
        
        DontDestroyOnLoad(this.gameObject);
    }
}
