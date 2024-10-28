using UnityEngine;
using TMPro;

public class Puntuacion : MonoBehaviour
{
    public static Puntuacion Instance; // Instancia Singleton

    public int score = 0; // Puntuación actual del jugador
    public static int finalScore; // Puntuación final para Game Over

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject); // Elimina instancias duplicadas
        }
    }

    // Calcula la puntuación en base a la operación y la dificultad
    public void CalculateScore(string operacion)
    {
        int points = 0;

        if (operacion.Contains("+") || operacion.Contains("-"))
            points = 10;
        else if (operacion.Contains("x") || operacion.Contains("÷"))
            points = 20;

        score += points;
    }

    public void SaveFinalScore()
    {
        finalScore = score; // Guarda el puntaje final para Game Over
    }

    // Resetea el puntaje para una nueva partida
    public void ResetScore()
    {
        score = 0;
    }
}
