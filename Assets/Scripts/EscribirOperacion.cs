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
    public TextMeshProUGUI multiplierText; // Para mostrar el multiplicador de racha
    public TextMeshProUGUI[] futureOperationsText; // Array de Textos para operaciones futuras
    public GameObject[] hearts; // Array de GameObjects de corazones

    public int vidas = 3;

    private int correctAnswer;
    private int score; // Almacena la puntuación del jugador
    private float timeTaken; // Almacena el tiempo de respuesta
    private int streakCount; // Contador de racha
    private int multiplier = 1; // Multiplicador de racha

    private float operationStartTime; // Tiempo en el que inicia cada operación
    private Queue<string> futureOperations = new Queue<string>(); // Cola para operaciones futuras

    void Start()
    {
        ResetGame();
        UpdateMultiplierUI(); // Actualiza el multiplicador de racha en pantalla al inicio
        UpdateHeartsUI(); // Actualiza la visualización de las vidas al inicio
        GenerateInitialOperations(); // Genera las operaciones iniciales (incluye futuras)
    }

    void Update()
    {
        HandleInput();
    }

    // Generación de operaciones iniciales
    void GenerateInitialOperations()
    {
        for (int i = 0; i < 3; i++) // Genera 3 operaciones para futuras
        {
            GenerateFutureOperation();
        }
        GenerateNextOperation(); // Genera la primera operación a resolver
    }

    // Generación de la siguiente operación actual
    void GenerateNextOperation()
    {
        if (futureOperations.Count > 0)
        {
            string nextOperation = futureOperations.Dequeue();
            operationText.text = nextOperation;
            correctAnswer = EvaluateOperation(nextOperation); // Evalúa la operación para obtener la respuesta correcta
            GenerateFutureOperation(); // Genera una nueva operación para rellenar la cola de futuras
            UpdateFutureOperationsUI(); // Actualiza la visualización de operaciones futuras
        }

        operationStartTime = Time.time; // Guarda el tiempo de inicio de la operación actual
    }

    // Genera una operación y la agrega a la cola de futuras operaciones
    void GenerateFutureOperation()
    {
        int num1 = Random.Range(1, 16);
        int num2 = Random.Range(1, 16);
        int operationType = Random.Range(0, 4);
        string nuevaOperacion;

        switch (operationType)
        {
            case 0: // Suma
                nuevaOperacion = $"{num1} + {num2}";
                break;
            case 1: // Resta
                if (num1 < num2)
                {
                    int temp = num1;
                    num1 = num2;
                    num2 = temp;
                }
                nuevaOperacion = $"{num1} - {num2}";
                break;
            case 2: // Multiplicación
                nuevaOperacion = $"{num1} x {num2}";
                break;
            case 3: // División
                while (num2 == 0 || num1 % num2 != 0)
                {
                    num1 = Random.Range(1, 16);
                    num2 = Random.Range(1, 16);
                }
                nuevaOperacion = $"{num1} ÷ {num2}";
                break;
            default:
                nuevaOperacion = "";
                break;
        }

        futureOperations.Enqueue(nuevaOperacion);
    }

    // Evalúa la operación dada y devuelve el resultado
    int EvaluateOperation(string operation)
    {
        string[] parts = operation.Split(' ');
        int num1 = int.Parse(parts[0]);
        int num2 = int.Parse(parts[2]);
        string op = parts[1];

        return op switch
        {
            "+" => num1 + num2,
            "-" => num1 - num2,
            "x" => num1 * num2,
            "÷" => num1 / num2,
            _ => 0
        };
    }

    // Manejo de la entrada del jugador
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && answerText.text != "")
        {
            CheckAnswer();
            answerText.text = ""; // Limpiar respuesta al presionar Enter
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
                timeTaken = Time.time - operationStartTime; // Calcula el tiempo de respuesta

                if (playerAnswer == correctAnswer)
                {
                    feedbackText.text = "¡Correcto!";
                    CalculateScore(operationText.text); // Calcula los puntos con el tiempo y racha

                    streakCount++;
                    if (streakCount % 5 == 0 && multiplier < 8)
                    {
                        multiplier *= 2; // Aumenta el multiplicador cada 5 respuestas correctas hasta x8
                        UpdateMultiplierUI(); // Actualiza la visualización del multiplicador
                    }

                    GenerateNextOperation(); // Mueve a la siguiente operación
                }
                else
                {
                    feedbackText.text = "Incorrecto. La respuesta era: " + correctAnswer;
                    vidas--;
                    streakCount = 0; // Reinicia la racha en caso de fallo
                    multiplier = 1; // Reinicia el multiplicador
                    UpdateMultiplierUI(); // Actualiza la visualización del multiplicador
                    UpdateHeartsUI(); // Actualiza la visualización de las vidas

                    if (vidas == 0)
                    {
                        EndGame();
                    }
                }

                StartCoroutine(ClearFeedbackText()); // Inicia la corrutina para limpiar el feedback
            }
            else
            {
                feedbackText.text = "Por favor, ingresa un número válido.";
                StartCoroutine(ClearFeedbackText()); // Inicia la corrutina para limpiar el feedback
            }
        }
        else
        {
            feedbackText.text = "Por favor, ingresa un número.";
            StartCoroutine(ClearFeedbackText()); // Inicia la corrutina para limpiar el feedback
        }
    }

    // Corrutina para limpiar el feedback después de 2 segundos
    IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(2);
        feedbackText.text = "";
    }

    // Función para calcular la puntuación basada en la operación, tiempo de respuesta y multiplicador de racha
    void CalculateScore(string operation)
    {
        int baseScore = operation.Contains("+") || operation.Contains("-") ? 1 : 2;
        int timeBonus = timeTaken < 3 ? 5 : Mathf.Max(5 - Mathf.FloorToInt(timeTaken - 3), 0);
        score += (baseScore + timeBonus) * multiplier;
    }

    // Función para finalizar el juego
    void EndGame()
    {
        SaveFinalScore();
        SceneManager.LoadScene("Game Over");
    }

    void SaveFinalScore()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        Debug.Log("Puntuación Final: " + score);
    }

    void ResetGame()
    {
        vidas = 3;
        score = 0;
        answerText.text = "";
        feedbackText.text = "";
        streakCount = 0;
        multiplier = 1;
        futureOperations.Clear();
        UpdateMultiplierUI();
        UpdateHeartsUI();
        GenerateInitialOperations();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2);
        GenerateNextOperation();
    }

    void UpdateMultiplierUI()
    {
        multiplierText.text = $"Multiplicador: x{multiplier}";
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < vidas);
        }
    }

    void UpdateFutureOperationsUI()
    {
        int i = 0;
        foreach (var operation in futureOperations)
        {
            futureOperationsText[i].text = operation;
            i++;
        }
    }
}
