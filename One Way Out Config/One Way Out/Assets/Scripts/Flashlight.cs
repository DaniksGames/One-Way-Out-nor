using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    [Header("Light Settings")]
    public GameObject spotlight; // Перетащите Spot Light из инспектора
    public float followSmoothness = 5f;
    public Vector3 positionOffset = new Vector3(0.2f, -0.1f, 0.2f);
    public bool enable;

    [Header("Battery Settings")]
    public TMP_Text batteryDisplay; // TextMeshPro для отображения заряда
    public int maxBattery = 100;
    public float batteryDrainInterval = 8f; // Интервал расхода батареи в секундах
    public GameObject lowBatteryObject; // Объект, который активируется при разрядке
    public float lowBatteryDelay = 3f; // Задержка перед действиями после разрядки
    public UnityEvent onBatteryDepleted; // События при разрядке батареи
    public int drainAmount = 1; // Новое поле для настройки величины расхода

    [Header("Sound Settings")]
    public AudioClip turnOnSound; // Звук включения
    public AudioClip turnOffSound; // Звук выключения
    public AudioClip lowBatterySound; // Звук разрядки батареи
    public AudioSource audioSource; // Источник звука

    [Header("Input Settings")]
    public KeyCode toggleKey = KeyCode.F;

    private Transform playerCamera;
    private bool isEnabled = true;
    private int currentBattery;
    private Coroutine batteryDrainCoroutine;
    private const string BatterySaveKey = "FlashlightBattery";
    private bool isBatteryDepleted = false;

    void Start()
    {
        playerCamera = Camera.main.transform;
        
        // Настройка AudioSource если не назначен
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.5f;
        }

        // Загрузка сохраненного заряда батареи
        currentBattery = PlayerPrefs.GetInt(BatterySaveKey, maxBattery);
        UpdateBatteryDisplay();

        if (spotlight != null)
        {
            isEnabled = spotlight.gameObject.activeSelf;
        }
        else
        {
            Debug.LogError("Spot Light не назначен!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && !isBatteryDepleted)
        {
            ToggleFlashlight();
        }

        if (isEnabled && spotlight != null)
        {
            FollowCamera();
        }
    }

    void ToggleFlashlight()
    {
        if (enable == false || isBatteryDepleted) return;

        if (spotlight == null) return;

        isEnabled = !isEnabled;
        spotlight.gameObject.SetActive(isEnabled);

        // Управление корутиной расхода батареи
        if (isEnabled)
        {
            if (batteryDrainCoroutine == null)
            {
                batteryDrainCoroutine = StartCoroutine(DrainBattery());
            }
        }
        else
        {
            if (batteryDrainCoroutine != null)
            {
                StopCoroutine(batteryDrainCoroutine);
                batteryDrainCoroutine = null;
            }
        }

        // Проигрываем соответствующий звук
        if (audioSource != null)
        {
            AudioClip clip = isEnabled ? turnOnSound : turnOffSound;
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    void FollowCamera()
    {
        Vector3 targetPosition = playerCamera.position + 
                                playerCamera.forward * positionOffset.z + 
                                playerCamera.right * positionOffset.x + 
                                playerCamera.up * positionOffset.y;

        spotlight.transform.position = Vector3.Lerp(
            spotlight.transform.position, 
            targetPosition, 
            Time.deltaTime * followSmoothness
        );

        spotlight.transform.rotation = Quaternion.Lerp(
            spotlight.transform.rotation,
            playerCamera.rotation,
            Time.deltaTime * followSmoothness
        );
    }

IEnumerator DrainBattery()
{
    Debug.Log("Battery drain started"); // Добавьте эту строку
    while (currentBattery > 0)
    {
        yield return new WaitForSeconds(batteryDrainInterval);
        
        if (isEnabled)
        {
            currentBattery--;
            Debug.Log($"Battery decreased to: {currentBattery}%"); // И эту
            UpdateBatteryDisplay();
            SaveBattery();
        }
    }
    BatteryDepleted();
}

    void BatteryDepleted()
    {
        isBatteryDepleted = true;
        isEnabled = false;
        if (spotlight != null) spotlight.gameObject.SetActive(false);

        // Проигрываем звук разрядки
        if (audioSource != null && lowBatterySound != null)
        {
            audioSource.PlayOneShot(lowBatterySound);
        }

        // Активируем объект и запускаем последовательность действий
        if (lowBatteryObject != null)
        {
            lowBatteryObject.SetActive(true);
        }

        StartCoroutine(BatteryDepletedSequence());
    }

    IEnumerator BatteryDepletedSequence()
    {
        yield return new WaitForSeconds(lowBatteryDelay);

        // Вызываем все заданные события
        onBatteryDepleted.Invoke();

        // Сбрасываем прогресс фонарика
        ResetFlashlightProgress();

        // Перезагружаем сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void UpdateBatteryDisplay()
    {
        if (batteryDisplay != null)
        {
            batteryDisplay.text = currentBattery.ToString() + "%";
        }
    }

    void SaveBattery()
    {
        PlayerPrefs.SetInt(BatterySaveKey, currentBattery);
        PlayerPrefs.Save();
    }

    // Публичный метод для сброса прогресса фонарика
    public void ResetFlashlightProgress()
    {
        currentBattery = maxBattery;
        isBatteryDepleted = false;
        UpdateBatteryDisplay();
        SaveBattery();

        if (batteryDrainCoroutine != null)
        {
            StopCoroutine(batteryDrainCoroutine);
            batteryDrainCoroutine = null;
        }

        if (lowBatteryObject != null)
        {
            lowBatteryObject.SetActive(false);
        }
    }

    // Публичный метод для принудительной установки заряда
    public void SetBattery(int amount)
    {
        currentBattery = Mathf.Clamp(amount, 0, maxBattery);
        UpdateBatteryDisplay();
        SaveBattery();

        if (currentBattery <= 0 && !isBatteryDepleted)
        {
            BatteryDepleted();
        }
        else if (currentBattery > 0)
        {
            isBatteryDepleted = false;
        }
    }
}