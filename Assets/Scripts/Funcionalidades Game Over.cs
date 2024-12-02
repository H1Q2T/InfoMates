using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI PuntuacionText; // Referencia al texto donde se mostrará la puntuación final
    public TMP_InputField NombreUsuarioInput; // Campo de entrada para el nombre de usuario
    public TextMeshProUGUI ErrorText; // Texto para mostrar errores, como el nombre vacío

    private FirebaseFirestore db;

    void Start()
    {
        // Inicializar Firebase y obtener la base de datos
        db = FirebaseFirestore.DefaultInstance;

        // Mostrar la puntuación final obtenida del juego
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        string gameMode = PlayerPrefs.GetString("GameMode", "Desconocido");

        PuntuacionText.text = $"Puntuación Final: {finalScore}\nModo de Juego: {gameMode}";
    }

    public void GuardarPuntuacion()
    {
        // Obtener el nombre de usuario, puntuación final y modo de juego
        string username = NombreUsuarioInput.text.Trim();
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        string gameMode = PlayerPrefs.GetString("GameMode", "Desconocido");

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

        // Determinar la colección según el modo de juego
        string collectionName = gameMode == "Por Vidas" ? "ScoresPorVidas" : "ScoresContrarreloj";

        // Crear un documento en la colección correspondiente
        DocumentReference docRef = db.Collection(collectionName).Document(username);
        Dictionary<string, object> userScore = new Dictionary<string, object>
        {
            { "username", username },
            { "score", finalScore }
        };

        // Guardar la puntuación en Firebase
        docRef.SetAsync(userScore).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Puntuación guardada con éxito en {collectionName}.");
            }
            else
            {
                Debug.LogError("Error al guardar la puntuación: " + task.Exception);
            }
        });
    }

    public void BotonVolver()
    {
        // Volver al menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
