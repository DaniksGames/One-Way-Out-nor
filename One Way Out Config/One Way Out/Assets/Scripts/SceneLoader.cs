using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;
    
    [Header("UI Elements")]
    public TMP_Text progressText;
    public GameObject loadingCanvas;
    
    [Header("Loading Settings")]
    public float minLoadingTime = 3f;
    [Range(0.1f, 1f)] public float animationSmoothness = 0.5f;
    public bool showUnitySplashScreen = false;
    
    private AsyncOperation loadingOperation;
    private float loadingStartTime;
    private float displayedProgress;
    private bool loadingComplete;
    private bool shadersCompiled;

    void Start()
    {
        if (!showUnitySplashScreen)
        {
            UnityEngine.Rendering.SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        }

        if (progressText == null) Debug.LogWarning("Progress Text not assigned!");
        if (loadingCanvas != null) loadingCanvas.SetActive(true);
        
        loadingStartTime = Time.time;
        displayedProgress = 0f;
        loadingComplete = false;
        shadersCompiled = false;
        
        StartCoroutine(PreloadGame());
    }

    IEnumerator PreloadGame()
    {
        // 1. Принудительная компиляция шейдеров
        yield return StartCoroutine(CompileShaders());
        shadersCompiled = true;

        // 2. Загрузка основной сцены
        yield return StartCoroutine(LoadNextSceneAsync());

        // 3. Дополнительная предзагрузка (по необходимости)
        yield return StartCoroutine(PreloadAssets());
    }

    IEnumerator CompileShaders()
    {
        float startTime = Time.time;
        float targetTime = minLoadingTime * 0.3f; // 30% времени на компиляцию
        
        // Эмулируем прогресс компиляции
        while (Time.time - startTime < targetTime)
        {
            float progress = Mathf.Clamp01((Time.time - startTime) / targetTime);
            UpdateProgressDisplay(progress * 0.3f); // 30% общего прогресса
            yield return null;
        }
    }

    IEnumerator LoadNextSceneAsync()
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneName);
        loadingOperation.allowSceneActivation = false;
        
        while (!loadingComplete)
        {
            float realProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01((Time.time - loadingStartTime) / minLoadingTime);
            
            // Комбинированный прогресс (30% шейдеры + 70% загрузка)
            float targetProgress = shadersCompiled ? 
                0.3f + (realProgress * 0.7f) : 
                (timeProgress * 0.3f);
            
            displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, 
                Time.deltaTime / animationSmoothness);
            
            UpdateProgressDisplay(displayedProgress);
            
            if (realProgress >= 0.999f && timeProgress >= 0.999f)
            {
                loadingComplete = true;
                loadingOperation.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }

    IEnumerator PreloadAssets()
    {
        // Здесь можно добавить предзагрузку ресурсов
        yield return null;
    }

    void UpdateProgressDisplay(float progress)
    {
        if (progressText != null)
        {
            progressText.text = $"{progress * 100:F0}%";
        }
    }
}