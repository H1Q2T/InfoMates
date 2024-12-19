using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JuegoMatematicas : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI tutorialText; // Texto para mostrar mensajes del tutorial
    public TextMeshProUGUI[] futureOperationsText;
    public GameObject[] hearts;

    public Image operationAreaHighlight; // Imagen para resaltar el área de operación
    public Image feedbackAreaHighlight; // Imagen para resaltar el área de feedback
    public Image livesAreaHighlight; // Imagen para resaltar el área de corazones
    public Image tutorialTextBackground; // Fondo del texto tutorial
    public Image inputButtonsBackground; // Fondo para los botones de entrada
    public Image futureOperationsHighlight; // Resaltado de las operaciones futuras


    public int vidas = 3;

    private int correctAnswer;
    private int score;
    private float timeTaken;
    private int streakCount;
    private int multiplier = 1;

    private float operationStartTime;
    private Queue<string> futureOperations = new Queue<string>();

    private int tutorialStep = 0;
    private bool isTutorial = true; // Indica si estamos en el tutorial
    public GameObject enemigo; // Referencia al GameObject del enemigo
    public Sprite spriteSuma; // Sprite para operaciones de suma
    public Sprite spriteResta; // Sprite para operaciones de resta
    public Sprite spriteMultiplicacion; // Sprite para operaciones de multiplicación
    public Sprite spriteDivision; // Sprite para operaciones de división

    private SpriteRenderer enemigoSpriteRenderer; // SpriteRenderer del enemigo

    public GameObject menuPanel; // Referencia al panel del menú
private bool isPaused = false; // Indica si el juego está pausado

public GameObject background;
    void Start()
    {
        AdjustBackgroundScale(background);
        if (isTutorial)
        {
            StartTutorial();
        }
        else
        {
            ResetGame();
        }

        if (enemigo != null)
        {
            enemigoSpriteRenderer = enemigo.GetComponent<SpriteRenderer>();
        }
    }


    void Update()
{
    Debug.Log("isTutorial: " + isTutorial);
    
    if (Input.GetKeyDown(KeyCode.Escape)) // Detectar tecla Escape
    {
        ToggleMenu();
    }

    if (isTutorial)
    {
        if (Input.GetMouseButtonDown(0)) // Avanzar el tutorial con clics
        {
            tutorialStep++;
            ExecuteTutorialStep();
        }
    }
    else
    {
        HandleInput(); // Permitir entrada del teclado
    }
}
public void AbrirMenuDesdeBoton()
{
    ToggleMenu(); // Activa o desactiva el menú
}
void AdjustBackgroundScale(GameObject background)
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

void ToggleMenu()
{
    isPaused = !isPaused; // Cambiar el estado de pausa
    menuPanel.SetActive(isPaused); // Activar o desactivar el panel del menú

    // Oscurecer y pausar el juego si está pausado
    Time.timeScale = isPaused ? 0 : 1;
}

public void ContinuarJuego()
{
    ToggleMenu(); // Cierra el menú y reanuda el juego
}

public void SalirAlMenu()
{
    Time.timeScale = 1; // Asegúrate de restablecer el tiempo antes de cambiar de escena
    SceneManager.LoadScene("Menu"); // Cargar la escena del menú
}

    void StartTutorial()
    {
        tutorialTextBackground.gameObject.SetActive(true); // Activar el fondo del tutorial al inicio
        tutorialText.gameObject.SetActive(true); // Mostrar el texto del tutorial
        tutorialText.text = "Benvingut a Dont Get Bored! Fes clic per començar el tutorial.";
        HideAllHighlights(); // Asegúrate de que no se muestre ningún área resaltada al inicio.
    }


    void ExecuteTutorialStep()
    {
        HideAllHighlights(); // Ocultar cualquier resaltado previo.

        switch (tutorialStep)
        {
            case 1:
                tutorialText.text = "Aquesta és la zona d'operacions. Aquí veuràs l'operació actual i el multiplicador de punts.";
                operationAreaHighlight.gameObject.SetActive(true); // Resaltar el área de operación.
                break;

            case 2:
                tutorialText.text = "Aquesta és la zona de feedback. Aquí sabràs si la teva resposta ha estat correcta o no.";
                feedbackAreaHighlight.gameObject.SetActive(true); // Resaltar el área de feedback.
                break;

            case 3:
                tutorialText.text = "Aquí també podràs veure les operacions futures que et tocaran. Fes-les servir per planificar les teves respostes!";
                futureOperationsHighlight.gameObject.SetActive(true); // Resaltar las operaciones futuras.
                break;

            case 4:
                tutorialText.text = "Aquí tens els cors. Cada cor representa una vida. No els perdis!";
                livesAreaHighlight.gameObject.SetActive(true); // Resaltar el área de corazones.
                break;

            case 5:
                tutorialText.text = "Aquí tens botons per escriure en mòbil. Prova d'utilitzar-los!";
                inputButtonsBackground.gameObject.SetActive(true); // Activa el fondo de los botones de entrada
                break;

            case 6:
                tutorialText.text = "Això és tot! Ara comença a jugar i resol les operacions.";
                tutorialText.gameObject.SetActive(false); // Oculta el texto del tutorial
                tutorialTextBackground.gameObject.SetActive(false); // Oculta el fondo del tutorial
                inputButtonsBackground.gameObject.SetActive(false); // Oculta el fondo de los botones
                isTutorial = false; // Establece isTutorial como false al finalizar el tutorial
                ResetGame(); // Lógica para iniciar el juego
                Debug.Log("Tutorial terminado. isTutorial = " + isTutorial); // Confirmar fin del tutorial
                break;

            default:
                break;
        }
    }
    public void EscribirNumero(int numero)
    {
        // Añade el número pulsado al texto de respuesta
        answerText.text += numero.ToString();
    }

    public void BorrarNumero()
    {
        // Borra el último carácter del texto de respuesta, si existe
        if (answerText.text.Length > 0)
        {
            answerText.text = answerText.text.Substring(0, answerText.text.Length - 1);
        }
    }
    public void BotonEnter()
    {
        // Verifica si hay un texto ingresado en el campo de respuesta
        if (!string.IsNullOrEmpty(answerText.text))
        {
            CheckAnswer(); // Verifica la respuesta ingresada
            answerText.text = ""; // Limpia el campo de respuesta después de presionar Enter
        }
    }

    void StartGame()
    {
        tutorialText.gameObject.SetActive(false); // Oculta el texto del tutorial
        tutorialTextBackground.gameObject.SetActive(false); // Oculta el fondo del tutorial
        inputButtonsBackground.gameObject.SetActive(false); // Oculta el fondo de los botones

        ResetGame(); // Lógica para iniciar el juego
    }


    void HideAllHighlights()
    {
        operationAreaHighlight.gameObject.SetActive(false);
        feedbackAreaHighlight.gameObject.SetActive(false);
        livesAreaHighlight.gameObject.SetActive(false);
        futureOperationsHighlight.gameObject.SetActive(false);

        inputButtonsBackground.gameObject.SetActive(false);
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
                BotonEnter(); // Llama a la función para validar
            }
        }
    }

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
                feedbackText.text = "¡Correcte!";
                    feedbackText.color = Color.green;
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
                feedbackText.text = "Incorrecte";
                    feedbackText.color = Color.red;
                    vidas--;
                streakCount = 0; // Reinicia la racha en caso de fallo
                multiplier = 1; // Reinicia el multiplicador
                UpdateMultiplierUI(); // Actualiza la visualización del multiplicador
                UpdateHeartsUI(); // Actualiza la visualización de las vidas

                answerText.text = ""; // Vaciar el campo de respuesta
                GenerateNextOperation(); // Cambia a una nueva operación tras respuesta incorrecta

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


    IEnumerator ClearFeedbackText()
    {
        yield return new WaitForSeconds(2);
        feedbackText.text = "";
    }

    void CalculateScore(string operation)
    {
        int baseScore = operation.Contains("+") || operation.Contains("-") ? 1 : 2;
        int timeBonus = timeTaken < 3 ? 5 : Mathf.Max(5 - Mathf.FloorToInt(timeTaken - 3), 0);
        score += (baseScore + timeBonus) * multiplier;
    }

    void EndGame()
{
    SceneManager.LoadScene("Menu");
}


    void SaveFinalScore()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        Debug.Log("Puntuación Final: " + score);
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
    void UpdateEnemigoSprite(string operation)
    {
        // Extraer el operador de la operación
        string[] parts = operation.Split(' ');
        string operador = parts[1]; // "+" o "-" o "x" o "÷"

        // Cambiar el sprite según el operador
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
                Debug.LogWarning("Operador no reconocido: " + operador);
                break;
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

            // Cambiar el sprite del enemigo basado en la operación
            UpdateEnemigoSprite(nextOperation);

            GenerateFutureOperation(); // Genera una nueva operación para rellenar la cola de futuras
            UpdateFutureOperationsUI(); // Actualiza la visualización de operaciones futuras
        }

        operationStartTime = Time.time; // Guarda el tiempo de inicio de la operación actual
    }


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
            default
:
                nuevaOperacion = ""; break;
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
