using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JuegoPorVidas : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI[] futureOperationsText;
    public GameObject[] hearts; // Representación visual de las vidas
    public GameObject enemigo; // Objeto del enemigo

    public GameObject menuPanel; // Panel del mini menú
    private bool isPaused = false; // Indica si el juego está pausado

    public Sprite spriteSuma; // Sprite para suma
    public Sprite spriteResta; // Sprite para resta
    public Sprite spriteMultiplicacion; // Sprite para multiplicación
    public Sprite spriteDivision; // Sprite para división

    private SpriteRenderer enemigoSpriteRenderer; // SpriteRenderer del enemigo

    public int vidas = 3; // Número inicial de vidas

    private int correctAnswer;
    private int score; // Almacena la puntuación del jugador
    private float timeTaken; // Almacena el tiempo de respuesta
    private int streakCount; // Contador de racha
    private int multiplier = 1; // Multiplicador de racha

    private float operationStartTime; // Tiempo en el que inicia cada operación
    private Queue<string> futureOperations = new Queue<string>(); // Cola para operaciones futuras
    private bool isGameOver = false; // Indica si el juego ha terminado

    void Start()
{
    if (enemigo != null)
    {
        enemigoSpriteRenderer = enemigo.GetComponent<SpriteRenderer>();
    }

    AdjustBackgroundScale(); // Ajustar el fondo al inicio

    ResetGame();
    UpdateMultiplierUI();
    UpdateHeartsUI();
    GenerateInitialOperations();
}


    void Update()
    {
        if (!isGameOver)
        {
            HandleInput(); // Maneja la entrada del jugador

            if (Input.GetKeyDown(KeyCode.Escape)) // Abrir/cerrar el menú con Escape
            {
                ToggleMenu();
            }
        }
    }

    void GenerateInitialOperations()
    {
        for (int i = 0; i < 3; i++) // Genera 3 operaciones para futuras
        {
            GenerateFutureOperation();
        }
        GenerateNextOperation(); // Genera la primera operación a resolver
    }

    void GenerateNextOperation()
    {
        if (futureOperations.Count > 0)
        {
            string nextOperation = futureOperations.Dequeue();
            operationText.text = nextOperation;
            correctAnswer = EvaluateOperation(nextOperation); // Evalúa la operación para obtener la respuesta correcta

            UpdateEnemigoSprite(nextOperation); // Cambiar el sprite del enemigo según la operación

            GenerateFutureOperation(); // Genera una nueva operación para rellenar la cola de futuras
            UpdateFutureOperationsUI(); // Actualiza la visualización de operaciones futuras
        }

        operationStartTime = Time.time; // Guarda el tiempo de inicio de la operación actual
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
    void HandleInput()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                answerText.text += c; // Añade el número presionado desde el teclado
            }
            else if (c == '\b' && answerText.text.Length > 0) // Retroceso
            {
                answerText.text = answerText.text.Substring(0, answerText.text.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                CheckAnswer(); // Valida la respuesta ingresada
            }
        }
    }
    void UpdateFutureOperationsUI()
    {
        int i = 0;
        foreach (var operation in futureOperations)
        {
            if (i < futureOperationsText.Length)
            {
                futureOperationsText[i].text = operation;
                i++;
            }
        }
    }

public GameObject background; // Objeto de fondo en la escena

void AdjustBackgroundScale()
{
    if (background != null)
    {
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            float cameraWidth = cameraHeight * Screen.width / Screen.height;

            Vector2 spriteSize = sr.sprite.bounds.size;
            background.transform.localScale = new Vector3(cameraWidth / spriteSize.x, cameraHeight / spriteSize.y, 1);
        }
        else
        {
            Debug.LogWarning("El SpriteRenderer del fondo no está asignado.");
        }
    }
    else
    {
        Debug.LogWarning("El objeto Background no está asignado.");
    }
}

    void GenerateFutureOperation()
    {
        int num1 = Random.Range(1, 16);
        int num2 = Random.Range(1, 16);
        int operationType = Random.Range(0, 4);
        string nuevaOperacion;

        switch (operationType)
        {
            case 0:
                nuevaOperacion = $"{num1} + {num2}";
                break;
            case 1:
                if (num1 < num2)
                {
                    int temp = num1;
                    num1 = num2;
                    num2 = temp;
                }
                nuevaOperacion = $"{num1} - {num2}";
                break;
            case 2:
                nuevaOperacion = $"{num1} x {num2}";
                break;
            case 3:
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
void CalculateScore(string operation)
{
    // Puntaje base según el tipo de operación (suma/resta: 1, multiplicación/división: 2)
    int baseScore = operation.Contains("+") || operation.Contains("-") ? 1 : 2;

    // Bonificación por tiempo: más puntos si la respuesta es rápida
    int timeBonus = timeTaken < 3 ? 5 : Mathf.Max(5 - Mathf.FloorToInt(timeTaken - 3), 0);

    // Cálculo de la puntuación total para esta operación
    score += (baseScore + timeBonus) * multiplier;

    Debug.Log($"Puntaje actualizado: {score}");
}

    void CheckAnswer()
{
    string playerInput = answerText.text.Trim();

    if (!string.IsNullOrEmpty(playerInput))
    {
        int playerAnswer;
        if (int.TryParse(playerInput, out playerAnswer))
        {
            timeTaken = Time.time - operationStartTime;

            if (playerAnswer == correctAnswer)
            {
                feedbackText.text = "¡Correcte!";
                CalculateScore(operationText.text); // Actualiza la puntuación
                streakCount++;

                if (streakCount % 5 == 0 && multiplier < 8)
                {
                    multiplier *= 2;
                    UpdateMultiplierUI();
                }

                answerText.text = "";
                GenerateNextOperation();
            }
            else
            {
                feedbackText.text = "Incorrecte. La resposta era: " + correctAnswer;
                vidas--;
                streakCount = 0;
                multiplier = 1;
                UpdateMultiplierUI();
                UpdateHeartsUI();
                answerText.text = "";
                GenerateNextOperation();

                if (vidas == 0)
                {
                    EndGame(false);
                }
            }

            StartCoroutine(ClearFeedbackText());
        }
        else
        {
            feedbackText.text = "Por favor, ingresa un número válido.";
            StartCoroutine(ClearFeedbackText());
        }
    }
    else
    {
        feedbackText.text = "Por favor, ingresa un número.";
        StartCoroutine(ClearFeedbackText());
    }
}


    void UpdateEnemigoSprite(string operation)
    {
        if (enemigoSpriteRenderer != null)
        {
            string[] parts = operation.Split(' ');
            string operador = parts[1];

            switch (operador)
            {
                case "+":
                    enemigoSpriteRenderer.sprite = spriteSuma;
                    break;
                case "-":
                    enemigoSpriteRenderer.sprite = spriteResta;
                    break;
                case "x":
                    enemigoSpriteRenderer.sprite = spriteMultiplicacion;
                    break;
                case "÷":
                    enemigoSpriteRenderer.sprite = spriteDivision;
                    break;
                default:
                    Debug.LogWarning("Operador desconocido: " + operador);
                    break;
            }
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < vidas);
        }
    }

    void EndGame(bool finalBueno)
    {
        isGameOver = true;
        SaveFinalScore();
        Time.timeScale = 1;

        if (finalBueno)
        {
            SceneManager.LoadScene("Final Bueno");
        }
        else
        {
            SceneManager.LoadScene("Final Malo");
        }
    }

    void SaveFinalScore()
{
    Debug.Log($"Guardando Puntuación Final: {score}, Modo de Juego: Por Vidas");
    PlayerPrefs.SetInt("FinalScore", score); // Guarda la puntuación final
    PlayerPrefs.SetString("GameMode", "Por Vidas"); // Guarda el modo de juego
    PlayerPrefs.Save(); // Asegúrate de guardar los datos
}


    public void ToggleMenu()
    {
        isPaused = !isPaused;
        menuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ContinuarJuego()
    {
        ToggleMenu();
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void AcabarPartida()
    {
        EndGame(true);
    }

    IEnumerator ClearFeedbackText()
{
    yield return new WaitForSeconds(1); // Cambiar a 1 segundo para limpiar el feedback más rápido
    feedbackText.text = "";
}


    void UpdateMultiplierUI()
    {
        multiplierText.text = $"Multiplicador: x{multiplier}";
    }
    public void BorrarNumero()
{
    if (answerText.text.Length > 0)
    {
        answerText.text = answerText.text.Substring(0, answerText.text.Length - 1); // Elimina el último carácter
    }
}
public void EscribirNumero(int numero)
{
    answerText.text += numero.ToString(); // Añade el número al texto de respuesta
}
public void BotonEnter()
{
    if (!string.IsNullOrEmpty(answerText.text))
    {
        CheckAnswer(); // Verifica la respuesta ingresada
        answerText.text = ""; // Limpia el campo de respuesta
    }
}

}
