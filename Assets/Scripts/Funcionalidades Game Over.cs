using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI PuntuacionText; // Referencia al texto donde se mostrará la puntuación

    public void BotonVolver()
    {
        SceneManager.LoadScene("Menu");
    }
    void Start()
    {
        // Obtener la puntuación almacenada al cargar esta escena
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        PuntuacionText.text = "Puntuación Final: " + finalScore;
    }

}
