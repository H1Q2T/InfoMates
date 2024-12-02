using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JuegoRELOJ : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI multiplierText; // Para mostrar el multiplicador de racha
    public TextMeshProUGUI[] futureOperationsText; // Array de Textos para operaciones futuras
    public TextMeshProUGUI timerText;

    private int correctAnswer;
    private int score; // Almacena la puntuación del jugador
    private float timeTaken; // Almacena el tiempo de respuesta
    private int streakCount; // Contador de racha
    private int multiplier = 1; // Multiplicador de racha

    private float operationStartTime; // Tiempo en el que inicia cada operación
    private Queue<string> futureOperations = new Queue<string>(); // Cola para operaciones futuras

    private float gameTime = 60f; // Tiempo total del juego en segundos (1 minuto)
    private bool isGameOver = false;

    public GameObject menuPanel; // Panel del menú
    private bool isPaused = false; // Indica si el juego está pausado

    public GameObject background; // Referencia al objeto de fondo

public GameObject enemigo; // Objeto del enemigo
public Sprite spriteSuma; // Sprite para operaciones de suma
public Sprite spriteResta; // Sprite para operaciones de resta
public Sprite spriteMultiplicacion; // Sprite para operaciones de multiplicación
public Sprite spriteDivision; // Sprite para operaciones de división

private SpriteRenderer enemigoSpriteRenderer; // SpriteRenderer del enemigo


    void Start()
{
    // Inicializa el SpriteRenderer del enemigo
    if (enemigo != null)
    {
        enemigoSpriteRenderer = enemigo.GetComponent<SpriteRenderer>();
        if (enemigoSpriteRenderer == null)
        {
            Debug.LogError("El enemigo no tiene un componente SpriteRenderer asignado.");
        }
    }
    else
    {
        Debug.LogError("El GameObject 'enemigo' no está asignado en el Inspector.");
    }

    AdjustBackgroundScale(); // Ajustar el fondo automáticamente
    ResetGame();
    UpdateMultiplierUI(); // Actualiza el multiplicador de racha en pantalla al inicio
    GenerateInitialOperations(); // Genera las operaciones iniciales (incluye futuras)
}



    void Update()
    {
        if (!isGameOver)
        {
            UpdateTimer(); // Actualiza el temporizador en cada frame
            HandleInput(); // Maneja la entrada del jugador

            if (Input.GetKeyDown(KeyCode.Escape)) // Abrir/cerrar el menú con Escape
            {
                ToggleMenu();
            }
        }
    }

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
        }
        else
        {
            Debug.LogWarning("El objeto Background no está asignado.");
        }
    }

void UpdateEnemigoSprite(string operation)
{
    if (enemigoSpriteRenderer != null)
    {
        string[] parts = operation.Split(' ');
        string operador = parts[1]; // Extraer el operador de la operación

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
    else
    {
        Debug.LogWarning("El SpriteRenderer del enemigo no está asignado.");
    }
}

    void ToggleMenu()
    {
        isPaused = !isPaused; // Cambiar el estado de pausa
        menuPanel.SetActive(isPaused); // Activar o desactivar el panel del menú

        // Pausar o reanudar el juego
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ContinuarJuego()
    {
        ToggleMenu(); // Cierra el menú y reanuda el juego
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1; // Restablecer el tiempo
        SceneManager.LoadScene("Menu"); // Cargar la escena del menú
    }

    public void AcabarPartida()
    {
        EndGame(false); // Llama a EndGame indicando que el jugador decidió terminar la partida
    }

public void BorrarNumero()
{
    if (answerText.text.Length > 0)
    {
        answerText.text = answerText.text.Substring(0, answerText.text.Length - 1); // Elimina el último carácter
    }
}

public void BotonEnter()
{
    if (!string.IsNullOrEmpty(answerText.text))
    {
        CheckAnswer(); // Verifica la respuesta ingresada
        answerText.text = ""; // Limpia el campo de respuesta
    }
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

        UpdateEnemigoSprite(nextOperation); // Cambiar el sprite del enemigo según la operación

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
                feedbackText.text = "¡Correcto! +2";
                gameTime += 2f; // Sumar 2 segundos al temporizador
                CalculateScore(operationText.text); // Calcula los puntos con el tiempo y racha

                streakCount++;
                if (streakCount % 5 == 0 && multiplier < 8)
                {
                    multiplier *= 2; // Aumenta el multiplicador cada 5 respuestas correctas hasta x8
                    UpdateMultiplierUI(); // Actualiza la visualización del multiplicador
                }

                answerText.text = ""; // Vaciar el campo de respuesta
                GenerateNextOperation(); // Mueve a la siguiente operación
            }
            else
            {
                feedbackText.text = "Incorrecto. La respuesta era: " + correctAnswer + " (-5)";
                gameTime -= 5f; // Restar 5 segundos al temporizador
                streakCount = 0; // Reinicia la racha en caso de fallo
                multiplier = 1; // Reinicia el multiplicador
                UpdateMultiplierUI(); // Actualiza la visualización del multiplicador

                answerText.text = ""; // Vaciar el campo de respuesta
                GenerateNextOperation(); // Cambiar a una nueva operación
            }

            StartCoroutine(ClearFeedbackText()); // Inicia la corrutina para limpiar el feedback
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

    void UpdateTimer()
    {
        gameTime -= Time.deltaTime;

        if (gameTime <= 0)
        {
            gameTime = 0;
            EndGame(true); // Llama a EndGame indicando que fue por tiempo agotado
        }

        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Función para finalizar el juego
void EndGame(bool tiempoAgotado)
{
    isGameOver = true; // Detener el juego

    string modoJuego = "Contrarreloj"; // Define el modo de juego para este script

    SaveFinalScore(modoJuego); // Guarda la puntuación y el modo de juego

    Time.timeScale = 1; // Asegúrate de reanudar el tiempo antes de cambiar de escena

    if (tiempoAgotado)
    {
        Debug.Log("Tiempo agotado. Redirigiendo a Final Malo...");
        SceneManager.LoadScene("Final Malo"); // Transición a Final Malo
    }
    else
    {
        Debug.Log("Partida terminada por el jugador. Redirigiendo a Final Bueno...");
        SceneManager.LoadScene("Final Bueno"); // Transición a Final Bueno
    }
}


public void EscribirNumero(int numero)
{
    answerText.text += numero.ToString(); // Añade el número presionado al texto de respuesta
}


    void SaveFinalScore(string modoJuego)
{
    PlayerPrefs.SetInt("FinalScore", score); // Guarda la puntuación final
    PlayerPrefs.SetString("GameMode", modoJuego); // Guarda el modo de juego
    PlayerPrefs.Save(); // Asegúrate de guardar los datos
    Debug.Log($"Puntuación Final Guardada: {score}, Modo de Juego: {modoJuego}");
}


    void ResetGame()
    {
        score = 0;
        answerText.text = "";
        feedbackText.text = "";
        streakCount = 0;
        multiplier = 1;
        gameTime = 60f; // Reinicia el tiempo total del juego (1 minuto)
        isGameOver = false;
        futureOperations.Clear();
        UpdateMultiplierUI();
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
