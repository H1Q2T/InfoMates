using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Carrusel : MonoBehaviour
{
    // Lista de sprites para el carrusel
// Lista de las imágenes del carrusel
public Image[] images;
public List<Sprite> sprites;

// Posiciones fijas del carrusel
private Vector3[] positions;
private Vector3[] scales;

// Índices de las imágenes
private Queue<int> indexQueue;

// Configuración de animación
private bool isAnimating = false;
public float animationDuration = 0.5f;

// Índice del elemento central
private int centerIndex = 2;



    void Start()
{
    // Posiciones y escalas fijas
    positions = new Vector3[5];
    scales = new Vector3[5];

    positions[0] = images[0].rectTransform.anchoredPosition; // Oculto a la izquierda
    positions[1] = images[1].rectTransform.anchoredPosition; // Visible izquierda
    positions[2] = images[2].rectTransform.anchoredPosition; // Centro
    positions[3] = images[3].rectTransform.anchoredPosition; // Visible derecha
    positions[4] = images[4].rectTransform.anchoredPosition; // Oculto a la derecha

    scales[0] = new Vector3(0f, 0f, 1f);  // Escala oculta
    scales[1] = new Vector3(1f, 1f, 1f);  // Escala normal
    scales[2] = new Vector3(1.5f, 1.5f, 1f);  // Escala grande (centro)
    scales[3] = new Vector3(1f, 1f, 1f);  // Escala normal
    scales[4] = new Vector3(0f, 0f, 1f);  // Escala oculta

    // Inicializar sprites e índices
    indexQueue = new Queue<int>();
    for (int i = 0; i < images.Length; i++)
    {
        indexQueue.Enqueue(i % sprites.Count);
        images[i].sprite = sprites[i % sprites.Count];
        images[i].rectTransform.anchoredPosition = positions[i];
        images[i].rectTransform.localScale = scales[i];
    }
}


public void MoveCarrusel(int direction)
{
    if (isAnimating) return;
    StartCoroutine(AnimateCarrusel(direction));
}

private IEnumerator AnimateCarrusel(int direction)
{
    isAnimating = true;

    // Actualizar la cola de índices
    if (direction == 1)
    {
        int firstIndex = indexQueue.Dequeue();
        indexQueue.Enqueue(firstIndex);
    }
    else if (direction == -1)
    {
        int lastIndex = indexQueue.Last();
        indexQueue = new Queue<int>(indexQueue.Reverse());
        indexQueue.Enqueue(lastIndex);
    }

    // Guardar las posiciones iniciales y finales
    Vector3[] startPositions = new Vector3[images.Length];
    Vector3[] endPositions = new Vector3[images.Length];
    Vector3[] startScales = new Vector3[images.Length];
    Vector3[] endScales = new Vector3[images.Length];

    for (int i = 0; i < images.Length; i++)
    {
        startPositions[i] = images[i].rectTransform.anchoredPosition;
        startScales[i] = images[i].rectTransform.localScale;

        int targetIndex = (i + direction + positions.Length) % positions.Length;
        endPositions[i] = positions[targetIndex];
        endScales[i] = scales[targetIndex];
    }

    // Animar las posiciones y escalas
    float elapsed = 0f;
    while (elapsed < animationDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / animationDuration;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].rectTransform.anchoredPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
            images[i].rectTransform.localScale = Vector3.Lerp(startScales[i], endScales[i], t);
        }

        yield return null;
    }

    // Actualizar los sprites después de la animación
    UpdateSprites();

    isAnimating = false;
}

private void UpdateSprites()
{
    int[] indices = indexQueue.ToArray();
    for (int i = 0; i < images.Length; i++)
    {
        images[i].sprite = sprites[indices[i] % sprites.Count];
        images[i].rectTransform.anchoredPosition = positions[i];
        images[i].rectTransform.localScale = scales[i];
    }
}


public void OnClickMoveLeft()
{
    MoveCarrusel(-1); // Mover hacia la izquierda
}

public void OnClickMoveRight()
{
    MoveCarrusel(1); // Mover hacia la derecha
}


}
