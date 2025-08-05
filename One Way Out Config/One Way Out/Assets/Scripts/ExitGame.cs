using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitGame : MonoBehaviour
{
    [Header("Настройки эффекта")]
    public RawImage fadeImage; // Объект для затемнения (аналог sharedEffectImage)
    public float fadeDuration = 1f; // Длительность эффекта в секундах
    public Color fadeColor = Color.black; // Цвет затемнения

    private bool isExiting = false;

    // Метод для вызова из кнопки
    public void StartExitSequence()
    {
        if (!isExiting)
        {
            StartCoroutine(ExitWithFade());
        }
    }

    private IEnumerator ExitWithFade()
    {
        isExiting = true;

        // Инициализация изображения
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }

        // Плавное затемнение
        float timer = 0f;
        while (timer < fadeDuration)
        {
            if (fadeImage != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Гарантируем полную непрозрачность в конце
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
        }

        // Задержка перед выходом (опционально)
        yield return new WaitForSeconds(0.2f);

        // Выход из игры
        QuitGame();
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}