using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLauncher : MonoBehaviour
{
    [Header("First Time Settings")]
    [Tooltip("Object to show for first-time players")]
    public GameObject firstTimeObject;
    [Tooltip("Button to continue game (optional)")]
    public Button continueButton;
    [Tooltip("Time before auto-continue (seconds)")]
    public float autoContinueTime = 7f;

    [Header("Player Settings")]
    [Tooltip("Player object to enable")]
    public GameObject playerObject;
    public GameObject playerCanvasObject;

    [Header("Save Settings")]
    public string firstTimeKey = "FirstTimePlayed";

    private bool _isFirstTime;
    private bool _gameStarted;
    private Coroutine _autoContinueCoroutine;

    private void Awake()
    {
        // Инициализация объектов
        if (firstTimeObject != null) firstTimeObject.SetActive(false);
        if (playerObject != null) playerObject.SetActive(false);
        if (playerCanvasObject != null) playerCanvasObject.SetActive(false);
        
        // Настройка кнопки продолжения
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
            continueButton.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (_gameStarted) return;
        _gameStarted = true;

        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        // Начальная задержка 1 секунда
        yield return new WaitForSeconds(1f);

        _isFirstTime = PlayerPrefs.GetInt(firstTimeKey, 1) == 1;

        // Показываем объект для первого запуска
        if (_isFirstTime && firstTimeObject != null)
        {
            firstTimeObject.SetActive(true);
            
            // Активируем кнопку продолжения
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
            
            // Запускаем таймер авто-продолжения
            _autoContinueCoroutine = StartCoroutine(AutoContinueTimer());
        }
        else
        {
            // Если не первый запуск - сразу активируем игрока
            ActivatePlayer();
        }
    }

    private IEnumerator AutoContinueTimer()
    {
        float timer = 0f;
        
        while (timer < autoContinueTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Автоматическое продолжение по истечении времени
        ContinueGame();
    }

    public void ContinueGame()
    {
        if (!_gameStarted || !_isFirstTime) return;

        // Останавливаем таймер авто-продолжения
        if (_autoContinueCoroutine != null)
        {
            StopCoroutine(_autoContinueCoroutine);
            _autoContinueCoroutine = null;
        }

        // Скрываем объект первого запуска и кнопку
        if (firstTimeObject != null)
        {
            firstTimeObject.SetActive(false);
        }
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }

        // Помечаем, что первый запуск уже был
        PlayerPrefs.SetInt(firstTimeKey, 0);
        PlayerPrefs.Save();

        // Активируем игрока
        ActivatePlayer();
    }

    private void ActivatePlayer()
    {
        if (playerObject != null)
        {
            playerObject.SetActive(true);
        }
        
        if (playerCanvasObject != null)
        {
            playerCanvasObject.SetActive(true);
        }
    }

    public void ResetAllProgress()
    {
        // Сбрасываем флаг первого запуска
        PlayerPrefs.SetInt(firstTimeKey, 1);
        
        // Если есть контроллер игрока, сбрасываем его прогресс
        if (playerObject != null)
        {
            FirstPersonController controller = playerObject.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.ResetPlayerSaves();
            }
        }
        
        PlayerPrefs.Save();
    }
}