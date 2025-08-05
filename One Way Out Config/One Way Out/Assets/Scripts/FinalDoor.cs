using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Добавляем пространство имён для TextMeshPro

public class FinalDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float smoothSpeed = 2f;
    public bool isOpen = false;
    public bool opensInward = false;

    [Header("Audio Settings")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip collectSound;
    private AudioSource audioSource;

    [Header("Object to Activate")]
    public GameObject objectToActivate;

    [Header("Collectable Objects")]
    public List<GameObject> requiredObjects = new List<GameObject>();
    public float maxCollectDistance = 3f;
    private HashSet<GameObject> collectedObjects = new HashSet<GameObject>();

    [Header("UI Settings")]
    public TMP_Text collectiblesCounterText; // Ссылка на TextMeshPro элемент

    [Header("Save System")]
    public string collectedObjectsKey = "CollectedObjects";

    private Quaternion initialRotation;
    private bool shouldOpen = false;

    void Start()
    {
        initialRotation = transform.rotation;

        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }

        InitializeCollectableObjects();
        LoadCollectedObjects();
        UpdateCollectiblesCounter(); // Обновляем счётчик при старте
    }

    void Update()
    {
        if (shouldOpen && !isOpen)
        {
            OpenDoor();
        }

        float direction = opensInward ? -1f : 1f;
        Quaternion targetRotation = isOpen ?
            Quaternion.Euler(0, openAngle * direction, 0) * initialRotation :
            initialRotation;

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    private void InitializeCollectableObjects()
    {
        foreach (var obj in requiredObjects)
        {
            if (obj != null)
            {
                var collectable = obj.GetComponent<CollectableObject>() ?? obj.AddComponent<CollectableObject>();
                collectable.finalDoor = this;
                collectable.maxCollectDistance = maxCollectDistance;
            }
        }
    }

    private void LoadCollectedObjects()
    {
        string savedData = PlayerPrefs.GetString(collectedObjectsKey, "");

        if (!string.IsNullOrEmpty(savedData))
        {
            string[] objectNames = savedData.Split(',');

            foreach (string name in objectNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    GameObject obj = requiredObjects.Find(o => o != null && o.name == name);
                    if (obj != null)
                    {
                        collectedObjects.Add(obj);
                        obj.SetActive(false);
                    }
                }
            }

            UpdateCollectiblesCounter(); // Обновляем счётчик после загрузки
            CheckAllObjectsCollected();
        }
    }

    private void SaveCollectedObjects()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (GameObject obj in collectedObjects)
        {
            if (obj != null)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(obj.name);
            }
        }

        PlayerPrefs.SetString(collectedObjectsKey, sb.ToString());
        PlayerPrefs.Save();
    }

    public void CollectObject(GameObject obj)
    {
        if (requiredObjects.Contains(obj) && !collectedObjects.Contains(obj))
        {
            collectedObjects.Add(obj);
            obj.SetActive(false);

            SaveCollectedObjects();
            UpdateCollectiblesCounter(); // Обновляем счётчик после сбора

            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            CheckAllObjectsCollected();
        }
    }

    // Новый метод для обновления текста счётчика
    private void UpdateCollectiblesCounter()
    {
        if (collectiblesCounterText != null)
        {
            collectiblesCounterText.text = $"{collectedObjects.Count:00}/{requiredObjects.Count:00}";
        }
    }

    private void CheckAllObjectsCollected()
    {
        if (collectedObjects.Count == requiredObjects.Count)
        {
            shouldOpen = true;
            Debug.Log("Все объекты собраны! Дверь открывается.");
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        shouldOpen = false;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

    public void ResetObjectSaves()
    {
        // Очищаем сохранённые данные
        PlayerPrefs.DeleteKey(collectedObjectsKey);
        PlayerPrefs.Save();

        // Очищаем коллекцию собранных объектов
        collectedObjects.Clear();

        // Сбрасываем состояние двери
        shouldOpen = false;
        isOpen = false;
        transform.rotation = initialRotation;

        // Обновляем счётчик
        UpdateCollectiblesCounter();

        // Активируем все требуемые объекты снова
        StartCoroutine(ReactivateObjectsWithDelay());

        // Деактивируем связанный объект, если есть
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    private IEnumerator ReactivateObjectsWithDelay(float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        
        foreach (var obj in requiredObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                
                var collectable = obj.GetComponent<CollectableObject>();
                if (collectable == null)
                {
                    collectable = obj.AddComponent<CollectableObject>();
                }
                
                collectable.finalDoor = this;
                collectable.maxCollectDistance = maxCollectDistance;
            }
        }
    }

    public class CollectableObject : MonoBehaviour
    {
        [HideInInspector]
        public FinalDoor finalDoor;
        [HideInInspector]
        public float maxCollectDistance = 3f;

        private void OnMouseDown()
        {
            if (finalDoor != null && IsPlayerCloseEnough())
            {
                finalDoor.CollectObject(gameObject);
            }
        }

        private bool IsPlayerCloseEnough()
        {
            if (Camera.main == null) return false;

            float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            return distance <= maxCollectDistance;
        }
    }
}