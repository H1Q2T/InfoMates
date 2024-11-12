using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI PuntuacionText; // Referencia al texto donde se mostrará la puntuación final
    public TMP_InputField NombreUsuarioInput; // Campo de entrada para el nombre de usuario
    public TextMeshProUGUI RankingText; // Texto donde se mostrará el ranking
    public TextMeshProUGUI ErrorText; // Texto para mostrar errores, como el nombre vacío

    private FirebaseFirestore db;

    void Start()
    {
        // Inicializar Firebase y obtener la base de datos
        db = FirebaseFirestore.DefaultInstance;

        // Mostrar la puntuación final obtenida del juego
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        PuntuacionText.text = "Puntuación Final: " + finalScore;

        // Recuperar y mostrar el ranking
        GetScores();
    }

    public void GuardarPuntuacion()
    {
        // Obtener el nombre de usuario y la puntuación final
        string username = NombreUsuarioInput.text.Trim(); // Usamos Trim() para eliminar espacios
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);

        if (string.IsNullOrEmpty(username))
        {
            // Si el nombre está vacío, mostramos un mensaje de error
            ErrorText.text = "El nombre no puede estar vacío.";
            return;
        }
        else
        {
            ErrorText.text = ""; // Limpiar el mensaje de error si el nombre es válido
        }

        // Crear un documento en la colección "Scores" con el nombre de usuario como ID
        DocumentReference docRef = db.Collection("Scores").Document(username);
        Dictionary<string, object> userScore = new Dictionary<string, object>
        {
            { "username", username },
            { "score", finalScore }
        };

        // Guardar la puntuación en Firebase
        docRef.SetAsync(userScore).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Puntuación guardada con éxito.");
                GetScores(); // Actualizar el ranking después de guardar
            }
            else
            {
                Debug.LogError("Error al guardar la puntuación: " + task.Exception);
            }
        });
    }

    private void GetScores()
    {
        // Obtener las 10 mejores puntuaciones
        db.Collection("Scores").OrderByDescending("score").Limit(10).GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                string ranking = "Ranking:\n";
                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    string username = document.GetValue<string>("username");
                    int score = document.GetValue<int>("score");
                    ranking += $"{username}: {score}\n";
                }

                // Mostrar el ranking en la interfaz de usuario
                RankingText.text = ranking;
            }
            else
            {
                Debug.LogError("Error al obtener el ranking: " + task.Exception);
            }
        });
    }

    public void BotonVolver()
    {
        // Volver al menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
