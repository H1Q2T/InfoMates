using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class RankingsManager : MonoBehaviour
{
    public TextMeshProUGUI rankingsText; // Texto para mostrar el ranking
    public TMP_InputField searchInput; // Campo para buscar usuarios
    public TextMeshProUGUI searchResultText; // Texto para mostrar el resultado de la búsqueda
    public TextMeshProUGUI modeTitleText; // Texto para mostrar el modo actual

    private FirebaseFirestore db;
    private string currentCollection = "ScoresContrarreloj"; // Colección inicial

    void Start()
    {
        // Inicializar Firebase
        db = FirebaseFirestore.DefaultInstance;

        // Cargar el ranking de la colección inicial
        LoadTopScores(currentCollection);
    }

    public void LoadTopScores(string collection)
    {
        currentCollection = collection; // Actualizar la colección actual
        modeTitleText.text = $"Top 15 - {(collection == "ScoresContrarreloj" ? "Contrarreloj" : "Por Vidas")}"; // Mostrar el título del modo

        rankingsText.text = "Cargando ranking...";

        // Consultar Firebase para el top 15 de la colección actual
        db.Collection(collection)
          .OrderByDescending("score")
          .Limit(15)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task => {
              if (task.IsCompleted)
              {
                  string ranking = "";
                  int position = 1;

                  foreach (DocumentSnapshot document in task.Result.Documents)
                  {
                      string username = document.GetValue<string>("username");
                      int score = document.GetValue<int>("score");
                      ranking += $"{position}. {username}: {score}\n";
                      position++;
                  }

                  rankingsText.text = ranking;
              }
              else
              {
                  Debug.LogError("Error al cargar el ranking: " + task.Exception);
                  rankingsText.text = "Error al cargar el ranking.";
              }
          });
    }

    public void SearchScoreByUsername()
    {
        string username = searchInput.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            searchResultText.text = "Introduce un nombre.";
            return;
        }

        searchResultText.text = "Buscando...";

        // Consultar ambas colecciones para buscar al usuario
        List<string> collections = new List<string> { "ScoresContrarreloj", "ScoresPorVidas" };

        foreach (string collection in collections)
        {
            db.Collection(collection)
              .WhereEqualTo("username", username)
              .GetSnapshotAsync()
              .ContinueWithOnMainThread(task => {
                  if (task.IsCompleted && task.Result.Count > 0)
                  {
                      foreach (DocumentSnapshot document in task.Result.Documents)
                      {
                          int score = document.GetValue<int>("score");
                          string mode = (collection == "ScoresContrarreloj") ? "Contrarreloj" : "Por Vidas";
                          searchResultText.text = $"{username}: {score} puntos ({mode})";
                          return;
                      }
                  }
                  else if (task.IsFaulted)
                  {
                      Debug.LogError("Error al buscar usuario: " + task.Exception);
                      searchResultText.text = "Error al buscar usuario.";
                  }
              });
        }

        // Si no se encuentra, mostrar mensaje
        searchResultText.text = "Usuario no encontrado.";
    }

    public void SwitchToContrarreloj()
    {
        LoadTopScores("ScoresContrarreloj");
    }

    public void SwitchToPorVidas()
    {
        LoadTopScores("ScoresPorVidas");
    }
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
