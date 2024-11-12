using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase inicializado correctamente.");
                // Ahora puedes utilizar Firestore y otros servicios de Firebase
            }
            else
            {
                Debug.LogError("No se pudo resolver todas las dependencias de Firebase: " + task.Result);
            }
        });
    }
}
