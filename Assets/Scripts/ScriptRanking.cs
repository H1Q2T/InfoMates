using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class RankingsManager : MonoBehaviour
{
    public TextMeshProUGUI rankingsText; // Texto para mostrar el ranking
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
        modeTitleText.text = $"Top 15 - {(collection == "ScoresContrarreloj" ? "Temps Limit" : "Per Vides")}"; // Mostrar el título del modo

        rankingsText.text = "Cargant ranking...";

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
