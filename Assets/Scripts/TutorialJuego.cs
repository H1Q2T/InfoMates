using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscribirOperación : MonoBehaviour
{
    public TextMeshProUGUI operationText;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;

    private int correctAnswer;
    private string currentOperation;

    void Start()
    {
        GenerateOperation();
    }

    void Update()
    {
        HandleInput();
    }

    // Función para generar operaciones aleatorias
    void GenerateOperation()
    {
        int num1 = Random.Range(1, 91); // Número entre 1 y 90
        int num2 = Random.Range(1, 91); // Número entre 1 y 90
        int operationType = Random.Range(0, 4); // 0 = suma, 1 = resta, 2 = multiplicación, 3 = división

        switch (operationType)
        {
            case 0: // Suma
                operationText.text = $"{num1} + {num2}";
                correctAnswer = num1 + num2;
                break;

            case 1: // Resta
                operationText.text = $"{num1} - {num2}";
                correctAnswer = num1 - num2;
                break;

            case 2: // Multiplicación
                operationText.text = $"{num1} x {num2}";
                correctAnswer = num1 * num2;
                break;

            case 3: // División (solo resultado exacto, sin decimales)
                // Aseguramos que el divisor no sea cero y que la división sea exacta
                while (num2 == 0 || num1 % num2 != 0)
                {
                    num1 = Random.Range(1, 91);
                    num2 = Random.Range(1, 91);
                }
                operationText.text = $"{num1} ÷ {num2}";
                correctAnswer = num1 / num2;
                break;
        }

        // Limpiar el texto de respuesta y feedback
        answerText.text = "";
        feedbackText.text = "";
    }

    // Manejo de la entrada del jugador
    void HandleInput()
    {
        // Si se pulsa Enter y el campo de respuesta no está vacío
        if (Input.GetKeyDown(KeyCode.Return) && answerText.text != "")
        {
            CheckAnswer();
        }

        // Manejar la entrada numérica del jugador
        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c)) // Solo se aceptan dígitos
            {
                answerText.text += c; // Agregar el número ingresado al texto de respuesta
            }
            else if (c == '\b' && answerText.text.Length > 0) // Si se pulsa retroceso
            {
                answerText.text = answerText.text.Substring(0, answerText.text.Length - 1); // Borrar el último carácter
            }
        }
    }

    // Función para comprobar la respuesta del jugador
    void CheckAnswer()
    {
        string playerInput = answerText.text.Trim(); // Elimina espacios en blanco

        if (!string.IsNullOrEmpty(playerInput)) // Verificar si el texto no está vacío
        {
            int playerAnswer;
            if (int.TryParse(playerInput, out playerAnswer))
            {
                if (playerAnswer == correctAnswer)
                {
                    feedbackText.text = "¡Correcto!";
                }
                else
                {
                    feedbackText.text = "Incorrecto. La respuesta era: " + correctAnswer;
                }

                // Iniciar una corutina para esperar 2 segundos antes de generar una nueva operación
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

    // Corutina que espera 2 segundos antes de generar una nueva operación
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1); // Los segundos del Cooldown entre operación y operación
        GenerateOperation(); // Generar una nueva operación después de la espera
    }
}
