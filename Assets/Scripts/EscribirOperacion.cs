using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JuegoMatematicas : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public int vidas = 3;

    private int correctAnswer;
    private int score; // Almacena la puntuación del jugador

    void Start()
    {
        ResetGame();
        GenerateOperation();
    }

    void Update()
    {
        HandleInput();
    }

    // Generación de operaciones aleatorias
    void GenerateOperation()
    {
        int num1 = Random.Range(1, 16);
        int num2 = Random.Range(1, 16);
        int operationType = Random.Range(0, 4);
        string nuevaOperacion;

        switch (operationType)
        {
            case 0: // Suma
                nuevaOperacion = $"{num1} + {num2}";
                correctAnswer = num1 + num2;
                break;
            case 1: // Resta
                nuevaOperacion = $"{num1} - {num2}";
                correctAnswer = num1 - num2;
                break;
            case 2: // Multiplicación
                nuevaOperacion = $"{num1} x {num2}";
                correctAnswer = num1 * num2;
                break;
            case 3: // División
                while (num2 == 0 || num1 % num2 != 0)
                {
                    num1 = Random.Range(1, 16);
                    num2 = Random.Range(1, 16);
                }
                nuevaOperacion = $"{num1} ÷ {num2}";
                correctAnswer = num1 / num2;
                break;
            default:
                nuevaOperacion = "";
                break;
        }

        operationText.text = nuevaOperacion;
    }

    // Manejo de la entrada del jugador
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && answerText.text != "")
        {
            CheckAnswer();
        }

        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                answerText.text += c;
            }
            else if (c == '\b' && answerText.text.Length > 0)
            {
                answerText.text = answerText.text.Substring(0, answerText.text.Length - 1);
            }
        }
    }

    // Comprobación de la respuesta del jugador
    void CheckAnswer()
    {
        string playerInput = answerText.text.Trim();

        if (!string.IsNullOrEmpty(playerInput))
        {
            int playerAnswer;
            if (int.TryParse(playerInput, out playerAnswer))
            {
                if (playerAnswer == correctAnswer)
                {
                    feedbackText.text = "¡Correcto!";
                    CalculateScore(operationText.text); // Calcula los puntos
                }
                else
                {
                    feedbackText.text = "Incorrecto. La respuesta era: " + correctAnswer;
                    vidas--;
                    if (vidas == 0)
                    {
                        EndGame();
                    }
                }

                answerText.text = ""; // Limpiar respuesta

                StartCoroutine(Cooldown());
            }
            else
            {
                feedbackText.text = "Por favor, ingresa un número válido.";
            }
        }
        else
        {
            feedbackText.text = "Por favor, ingresa un número.";
        }
    }

    // Función para calcular la puntuación basada en la operación
    void CalculateScore(string operation)
    {
        // Lógica para calcular puntos
        if (operation.Contains("+") || operation.Contains("-"))
        {
            score += 1; // Puntos por operaciones simples
        }
        else if (operation.Contains("x") || operation.Contains("÷"))
        {
            score += 2; // Puntos por operaciones más complejas
        }
    }

    // Función para finalizar el juego
    void EndGame()
    {
        SaveFinalScore(); // Muestra la puntuación final
        SceneManager.LoadScene("Game Over");
    }

    // Función para mostrar la puntuación final
    void SaveFinalScore()
{
    PlayerPrefs.SetInt("FinalScore", score); // Guardar la puntuación en PlayerPrefs
    PlayerPrefs.Save(); // Asegúrate de guardar los cambios
    Debug.Log("Puntuación Final: " + score);
}

    // Reiniciar el juego
    void ResetGame()
    {
        vidas = 3; // Reinicia vidas
        score = 0; // Reinicia la puntuación
        answerText.text = ""; // Asegúrate de que AnswerText esté vacío
        feedbackText.text = ""; // Asegúrate de que FeedbackText esté vacío
        GenerateOperation(); // Genera la primera operación al inicio
    }

    // Corutina que espera antes de generar una nueva operación
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2);
        GenerateOperation(); // Genera una nueva operación después de un breve periodo de espera
    }
}
