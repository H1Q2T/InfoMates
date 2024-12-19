using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Carrusel : MonoBehaviour
{
    // Lista de imágenes y sprites
    public Image[] images;
    public List<Sprite> sprites;

    // Posiciones y escalas del carrusel
    private Vector3[] positions;
    private Vector3[] scales;

    // Cola para manejar los índices de sprites
    private Queue<int> indexQueue;

    // Configuración de animación
    private bool isAnimating = false;
    public float animationDuration = 0.5f;

    void Start()
    {
        // Inicializar posiciones y escalas
        positions = new Vector3[5];
        scales = new Vector3[5];

        positions[0] = images[0].rectTransform.anchoredPosition; // Oculto izquierda
        positions[1] = images[1].rectTransform.anchoredPosition; // Visible izquierda
        positions[2] = images[2].rectTransform.anchoredPosition; // Centro
        positions[3] = images[3].rectTransform.anchoredPosition; // Visible derecha
        positions[4] = images[4].rectTransform.anchoredPosition; // Oculto derecha

        scales[0] = new Vector3(0f, 0f, 1f);  // Escala oculta
        scales[1] = new Vector3(1f, 1f, 1f);  // Escala normal
        scales[2] = new Vector3(1.5f, 1.5f, 1f);  // Escala grande (centro)
        scales[3] = new Vector3(1f, 1f, 1f);  // Escala normal
        scales[4] = new Vector3(0f, 0f, 1f);  // Escala oculta

        // Inicializar cola de índices y sprites
        indexQueue = new Queue<int>();
        for (int i = 0; i < images.Length; i++)
        {
            int spriteIndex = i % sprites.Count;
            indexQueue.Enqueue(spriteIndex);
            images[i].sprite = sprites[spriteIndex];
            images[i].rectTransform.anchoredPosition = positions[i];
            images[i].rectTransform.localScale = scales[i];
        }
    }

    public void MoveCarrusel(int direction)
    {
        if (isAnimating) return; // Evita movimientos simultáneos
        StartCoroutine(AnimateCarrusel(direction));
    }

    private IEnumerator AnimateCarrusel(int direction)
    {
        isAnimating = true;

        // Guardar las posiciones iniciales y finales
        Vector3[] startPositions = new Vector3[images.Length];
        Vector3[] endPositions = new Vector3[images.Length];
        Vector3[] startScales = new Vector3[images.Length];
        Vector3[] endScales = new Vector3[images.Length];

        for (int i = 0; i < images.Length; i++)
        {
            startPositions[i] = images[i].rectTransform.anchoredPosition;
            startScales[i] = images[i].rectTransform.localScale;

            int targetIndex = (i - direction + positions.Length) % positions.Length;
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

        // Actualizar cola de índices y sprites al final de la animación
        if (direction == 1) // Derecha
        {
            int firstIndex = indexQueue.Dequeue();
            indexQueue.Enqueue(firstIndex);
        }
        else if (direction == -1) // Izquierda
        {
            int lastIndex = indexQueue.Last();
            indexQueue = new Queue<int>(indexQueue.Reverse());
            indexQueue.Dequeue();
            indexQueue = new Queue<int>(indexQueue.Reverse());
            indexQueue.Enqueue(lastIndex);
        } 

        UpdateSprites();

        isAnimating = false;
    }

    private void UpdateSprites()
    {
        int[] indices = indexQueue.ToArray();

        for (int i = 0; i < images.Length; i++)
        {
            int spriteIndex = indices[i];
            images[i].sprite = sprites[spriteIndex];

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
