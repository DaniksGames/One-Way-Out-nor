using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class InteractiveButtons : MonoBehaviour
{
    [System.Serializable]
    public class SequenceItem
    {
        public GameObject targetObject;
        public bool setActive;
        public float delayAfterAction;
    }

    [System.Serializable]
    public class ButtonSettings
    {
        public Button button;
        public Color highlightColor = Color.gray;
        public AudioClip hoverEnterSound;
        public AudioClip hoverExitSound;
        public AudioClip clickSound;
        public GameObject hoverObject;
        public List<SequenceItem> sequenceItems = new List<SequenceItem>();
    }

    [Header("Настройки кнопок")]
    public List<ButtonSettings> buttons = new List<ButtonSettings>();

    [Header("Audio Source")]
    [Tooltip("Если не назначен, создастся автоматически")]
    public AudioSource audioSource;

    [Header("Общие настройки")]
    public RawImage sharedEffectImage;
    public float fadeSpeed = 1f;
    public bool isEffectPlaying = false;

    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();
    private Dictionary<Button, bool> isHovering = new Dictionary<Button, bool>();

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (sharedEffectImage != null)
        {
            sharedEffectImage.gameObject.SetActive(false);
            sharedEffectImage.color = new Color(1, 1, 1, 0);
        }

        SetupButtons();
    }

    private void SetupButtons()
    {
        foreach (ButtonSettings settings in buttons)
        {
            if (settings.button == null) continue;

            originalColors[settings.button] = settings.button.colors.normalColor;
            isHovering[settings.button] = false;
            
            // Гарантированно скрываем hoverObject при старте
            if (settings.hoverObject != null)
            {
                settings.hoverObject.SetActive(false);
            }

            AddEventTriggers(settings.button);
        }
    }

    private void AddEventTriggers(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((data) => OnPointerEnter(button));
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((data) => OnPointerExit(button));
        trigger.triggers.Add(pointerExit);

        button.onClick.AddListener(() => OnButtonClick(button));
    }

    private void Update()
    {
        // Дополнительная проверка для случаев, когда курсор ушел с кнопки без события PointerExit
        foreach (var settings in buttons)
        {
            if (settings.button == null || settings.hoverObject == null) continue;

            if (!isHovering[settings.button] && settings.hoverObject.activeSelf)
            {
                settings.hoverObject.SetActive(false);
            }
        }
    }

    private void OnPointerEnter(Button button)
    {
        ButtonSettings settings = GetSettings(button);
        if (settings == null || isEffectPlaying) return;

        isHovering[button] = true;

        var colors = button.colors;
        colors.highlightedColor = settings.highlightColor;
        button.colors = colors;

        if (settings.hoverObject != null)
        {
            settings.hoverObject.SetActive(true);
        }

        if (settings.hoverEnterSound != null)
        {
            audioSource.PlayOneShot(settings.hoverEnterSound);
        }
    }

    private void OnPointerExit(Button button)
    {
        ButtonSettings settings = GetSettings(button);
        if (settings == null) return;

        isHovering[button] = false;

        if (originalColors.TryGetValue(button, out Color originalColor))
        {
            var colors = button.colors;
            colors.highlightedColor = originalColor;
            button.colors = colors;
        }

        if (settings.hoverObject != null)
        {
            settings.hoverObject.SetActive(false);
        }

        if (settings.hoverExitSound != null)
        {
            audioSource.PlayOneShot(settings.hoverExitSound);
        }
    }

    private void OnButtonClick(Button button)
    {
        if (isEffectPlaying) return;

        ButtonSettings settings = GetSettings(button);
        if (settings == null) return;

        if (settings.clickSound != null)
        {
            audioSource.PlayOneShot(settings.clickSound);
        }

        StartCoroutine(PlayFullEffectSequence(settings));
    }

    private IEnumerator PlayFullEffectSequence(ButtonSettings settings)
    {
        isEffectPlaying = true;

        if (sharedEffectImage != null)
        {
            sharedEffectImage.gameObject.SetActive(true);
            sharedEffectImage.color = new Color(1, 1, 1, 1);
        }

        foreach (SequenceItem item in settings.sequenceItems)
        {
            if (item.targetObject != null)
            {
                item.targetObject.SetActive(item.setActive);
            }
            yield return new WaitForSeconds(item.delayAfterAction);
        }

        if (sharedEffectImage != null)
        {
            float alpha = 1f;
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime * fadeSpeed;
                sharedEffectImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            sharedEffectImage.gameObject.SetActive(false);
        }

        isEffectPlaying = false;
    }

    private ButtonSettings GetSettings(Button button)
    {
        foreach (var settings in buttons)
        {
            if (settings.button == button)
                return settings;
        }
        return null;
    }
}