using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameSettings : MonoBehaviour
{
    [Header("Player Controller")]
    public FirstPersonController playerController;

    [Header("Volume Profile")]
    public VolumeProfile volumeProfile;

    [Header("Ray Tracing Settings")]
    public bool rayTracingSupported = false;
    public bool pathTracingSupported = false;
    public Button enableRayTracingButton;
    public Button enablePathTracingButton;
    public Button disableTracingButton;
    public TextMeshProUGUI tracingWarningText;
    public TextMeshProUGUI fpsCounterText;
    public Toggle showFPSToggle;
    public Toggle pathTracingToggle;
    public Toggle recursiveRenderingToggle;
    public Toggle subsurfaceScatteringToggle;
    public Slider rayBiasSlider;
    public Slider pathTracingSamplesSlider;
    public Slider pathTracingBouncesSlider;

    [Header("Player Settings")]
    public Slider mouseSensitivitySlider;
    public Slider cameraSmoothTimeSlider;
    public Slider normalFOVSlider;
    public Slider runFOVSlider;
    public Slider fovChangeSpeedSlider;
    public Toggle tiltToggle;

    [Header("Camera Bobbing Settings")]
    public Toggle cameraBobbingToggle;
    public Slider walkBobbingSpeedSlider;
    public Slider walkBobbingAmountSlider;
    public Slider runBobbingSpeedSlider;
    public Slider runBobbingAmountSlider;
    public Slider horizontalBobbingAmountSlider;

    [System.Serializable]
    public class KeyBindingButton
    {
        public Button button;
        public Text buttonText;
        public KeyCode defaultKey;
        [System.NonSerialized] public KeyCode currentKey;
        public string actionName;
        public string playerPrefKey;
    }

    [Header("Key Bindings")]
    public KeyBindingButton forwardKey;
    public KeyBindingButton backwardKey;
    public KeyBindingButton leftKey;
    public KeyBindingButton rightKey;
    public KeyBindingButton jumpKey;
    public KeyBindingButton runKey;

    [Header("Vignette Settings")]
    public Toggle vignetteToggle;
    public Slider vignetteIntensitySlider;
    public Slider vignetteSmoothnessSlider;
    public Slider vignetteRoundnessSlider;

    [Header("SSAO Settings")]
    public Toggle ssaoToggle;
    public Slider ssaoIntensitySlider;
    public Slider ssaoRadiusSlider;
    public Slider ssaoDirectLightingSlider;

    [Header("Fog Settings")]
    public Toggle fogToggle;

    [Header("Bloom Settings")]
    public Toggle bloomToggle;
    public Slider bloomIntensitySlider;
    public Slider bloomThresholdSlider;
    public Slider bloomScatterSlider;

    [Header("Depth of Field")]
    public Toggle dofToggle;

    [Header("Film Grain")]
    public Toggle grainToggle;
    public Slider grainIntensitySlider;
    public Slider grainResponseSlider;

    [Header("Motion Blur")]
    public Toggle motionBlurToggle;
    public Slider motionBlurIntensitySlider;

    [Header("Lens Flare")]
    public Toggle lensFlareToggle;
    public Slider lensFlareIntensitySlider;

    [Header("Chromatic Aberration")]
    public Toggle chromaticAberrationToggle;

    [Header("Water Settings")]
    public Toggle waterToggle;
    public List<GameObject> waterObjects = new List<GameObject>();

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;

    [Header("Resolution Settings")]
    public Toggle fullscreenToggle;
    public Button resolution360pButton;
    public Button resolution480pButton;
    public Button resolution720pButton;
    public Button resolution1080pButton;
    public Button resolution1440pButton;
    public Button resolution2160pButton;

    [System.Serializable]
    public class LanguagePair
    {
        public GameObject russianObject;
        public GameObject englishObject;
    }

    [Header("Language Settings")]
    public Button russianLanguageButton;
    public Button englishLanguageButton;
    public List<LanguagePair> languageObjects = new List<LanguagePair>();

    [Header("Default Settings")]
    public float defaultMouseSensitivity = 100f;
    public float defaultCameraSmoothTime = 0.1f;
    public float defaultNormalFOV = 60f;
    public float defaultRunFOV = 65f;
    public float defaultFOVChangeSpeed = 5f;
    public bool defaultTiltEnabled = true;
    public KeyCode defaultForwardKey = KeyCode.W;
    public KeyCode defaultBackwardKey = KeyCode.S;
    public KeyCode defaultLeftKey = KeyCode.A;
    public KeyCode defaultRightKey = KeyCode.D;
    public KeyCode defaultJumpKey = KeyCode.Space;
    public KeyCode defaultRunKey = KeyCode.LeftShift;
    public bool defaultVignetteEnabled = true;
    public float defaultVignetteIntensity = 0.4f;
    public float defaultVignetteSmoothness = 0.2f;
    public float defaultVignetteRoundness = 1f;
    public bool defaultSSAOEnabled = true;
    public float defaultSSAOIntensity = 1f;
    public float defaultSSAORadius = 0.5f;
    public float defaultSSAODirectLighting = 0.5f;
    public bool defaultFogEnabled = true;
    public bool defaultBloomEnabled = true;
    public float defaultBloomIntensity = 0.8f;
    public float defaultBloomThreshold = 0.5f;
    public float defaultBloomScatter = 0.7f;
    public bool defaultDOFEnabled = false;
    public bool defaultGrainEnabled = false;
    public float defaultGrainIntensity = 0.3f;
    public float defaultGrainResponse = 0.8f;
    public bool defaultMotionBlurEnabled = false;
    public float defaultMotionBlurIntensity = 0.5f;
    public bool defaultLensFlareEnabled = true;
    public float defaultLensFlareIntensity = 0.5f;
    public bool defaultChromaticAberrationEnabled = false;
    public bool defaultWaterEnabled = true;
    public float defaultMasterVolume = 1f;
    public bool defaultFullscreenEnabled = true;
    public bool defaultIsEnglish = true;
    public bool defaultCameraBobbingEnabled = true;
    public float defaultWalkBobbingSpeed = 8f;
    public float defaultWalkBobbingAmount = 0.05f;
    public float defaultRunBobbingSpeed = 12f;
    public float defaultRunBobbingAmount = 0.08f;
    public float defaultHorizontalBobbingAmount = 0.03f;
    public int defaultPathTracingSamples = 256;
    public int defaultPathTracingBounces = 4;
    public float defaultRayBias = 0.01f;
    public bool defaultRecursiveRenderingEnabled = true;
    public bool defaultSubsurfaceScatteringEnabled = true;

    private Vignette _vignette;
    private ScreenSpaceAmbientOcclusion _ssao;
    private Fog _fog;
    private Bloom _bloom;
    private DepthOfField _dof;
    private FilmGrain _grain;
    private MotionBlur _motionBlur;
    private ScreenSpaceLensFlare _lensFlare;
    private ChromaticAberration _chromaticAberration;
    private RayTracingSettings _rayTracingSettings;
    private PathTracing _pathTracing;
    private RecursiveRendering _recursiveRendering;
    private SubSurfaceScattering _subsurfaceScattering;

    private bool isWaitingForKey = false;
    private KeyBindingButton currentKeyButton = null;
    private float _mouseSensitivity = 100f;
    private float _cameraSmoothTime = 0.1f;
    private float _normalFOV = 60f;
    private float _runFOV = 65f;
    private float _fovChangeSpeed = 5f;
    private bool _tiltEnabled = true;
    private bool _vignetteEnabled = true;
    private float _vignetteIntensity = 0.4f;
    private float _vignetteSmoothness = 0.2f;
    private float _vignetteRoundness = 1f;
    private bool _ssaoEnabled = true;
    private float _ssaoIntensity = 1f;
    private float _ssaoRadius = 0.5f;
    private float _ssaoDirectLighting = 0.5f;
    private bool _fogEnabled = true;
    private bool _bloomEnabled = true;
    private float _bloomIntensity = 0.8f;
    private float _bloomThreshold = 0.5f;
    private float _bloomScatter = 0.7f;
    private bool _dofEnabled = false;
    private bool _grainEnabled = false;
    private float _grainIntensity = 0.3f;
    private float _grainResponse = 0.8f;
    private bool _motionBlurEnabled = false;
    private float _motionBlurIntensity = 0.5f;
    private bool _lensFlareEnabled = true;
    private float _lensFlareIntensity = 0.5f;
    private bool _chromaticAberrationEnabled = false;
    private bool _waterEnabled = true;
    private float _masterVolume = 1f;
    private bool _fullscreenEnabled = true;
    private bool _isEnglish = true;
    private int _resolutionWidth = 1920;
    private int _resolutionHeight = 1080;
    private bool _cameraBobbingEnabled = true;
    private float _walkBobbingSpeed = 8f;
    private float _walkBobbingAmount = 0.05f;
    private float _runBobbingSpeed = 12f;
    private float _runBobbingAmount = 0.08f;
    private float _horizontalBobbingAmount = 0.03f;
    private int _targetFPS = 60;
    private static string _instanceID;

    private float _saveTimer = 0f;
    private const float SAVE_INTERVAL = 1f;
    private List<float> _fpsMeasurements = new List<float>();
    private bool _isMeasuringFPS = false;
    private float _fpsMeasureTimer = 0.5f;
    private float _fpsUpdateTimer = 0f;
    private const float FPS_UPDATE_INTERVAL = 0.5f;
    private float _currentFPS = 0f;
    private int _frameCount = 0;

    void Awake()
    {
        CheckHardwareSupport();
        InitializeEffects();
        LoadSettings();
        SetupUI();
        ApplyAllSettings();
        SetLanguage(_isEnglish);
    }

void CheckHardwareSupport()
{
    // Проверяем поддержку Ray Tracing
    rayTracingSupported = SystemInfo.supportsRayTracing;
    
    // Для Path Tracing нужна не только поддержка RT, но и DirectX 12
    pathTracingSupported = rayTracingSupported && 
                         (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 ||
                          SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan);
    
    // Дополнительная проверка для карт NVIDIA RTX
    bool isNvidiaRTX = SystemInfo.graphicsDeviceName.Contains("RTX") || 
                      SystemInfo.graphicsDeviceName.Contains("Ray Tracing");
    
    // Если карта RTX, но SystemInfo не показывает поддержку, попробуем вручную
    if (isNvidiaRTX && !rayTracingSupported)
    {
        Debug.LogWarning("NVIDIA RTX card detected but SystemInfo.supportsRayTracing is false. Trying to enable anyway.");
        rayTracingSupported = true;
        pathTracingSupported = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12;
    }
    
    PlayerPrefs.SetInt("RayTracingSupported", rayTracingSupported ? 1 : 0);
    PlayerPrefs.SetInt("PathTracingSupported", pathTracingSupported ? 1 : 0);
    
    Debug.Log($"Ray Tracing Supported: {rayTracingSupported}");
    Debug.Log($"Path Tracing Supported: {pathTracingSupported}");
    Debug.Log($"GPU: {SystemInfo.graphicsDeviceName}");
    Debug.Log($"GPU Type: {SystemInfo.graphicsDeviceType}");
}

    void InitializeEffects()
    {
        if (volumeProfile != null)
        {
            volumeProfile.TryGet(out _vignette);
            volumeProfile.TryGet(out _ssao);
            volumeProfile.TryGet(out _fog);
            volumeProfile.TryGet(out _bloom);
            volumeProfile.TryGet(out _dof);
            volumeProfile.TryGet(out _grain);
            volumeProfile.TryGet(out _motionBlur);
            volumeProfile.TryGet(out _lensFlare);
            volumeProfile.TryGet(out _chromaticAberration);
            volumeProfile.TryGet(out _rayTracingSettings);
            volumeProfile.TryGet(out _pathTracing);
            volumeProfile.TryGet(out _recursiveRendering);
            volumeProfile.TryGet(out _subsurfaceScattering);
        }
    }

    void LoadSettings()
    {
        // Player Settings
        _mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
        _cameraSmoothTime = PlayerPrefs.GetFloat("CameraSmoothTime", defaultCameraSmoothTime);
        _normalFOV = PlayerPrefs.GetFloat("NormalFOV", defaultNormalFOV);
        _runFOV = PlayerPrefs.GetFloat("RunFOV", defaultRunFOV);
        _fovChangeSpeed = PlayerPrefs.GetFloat("FOVChangeSpeed", defaultFOVChangeSpeed);
        _tiltEnabled = PlayerPrefs.GetInt("TiltEnabled", defaultTiltEnabled ? 1 : 0) == 1;
        _cameraBobbingEnabled = PlayerPrefs.GetInt("CameraBobbingEnabled", defaultCameraBobbingEnabled ? 1 : 0) == 1;
        _walkBobbingSpeed = PlayerPrefs.GetFloat("WalkBobbingSpeed", defaultWalkBobbingSpeed);
        _walkBobbingAmount = PlayerPrefs.GetFloat("WalkBobbingAmount", defaultWalkBobbingAmount);
        _runBobbingSpeed = PlayerPrefs.GetFloat("RunBobbingSpeed", defaultRunBobbingSpeed);
        _runBobbingAmount = PlayerPrefs.GetFloat("RunBobbingAmount", defaultRunBobbingAmount);
        _horizontalBobbingAmount = PlayerPrefs.GetFloat("HorizontalBobbingAmount", defaultHorizontalBobbingAmount);

        // Key Bindings
        forwardKey.currentKey = (KeyCode)PlayerPrefs.GetInt("ForwardKey", (int)defaultForwardKey);
        backwardKey.currentKey = (KeyCode)PlayerPrefs.GetInt("BackwardKey", (int)defaultBackwardKey);
        leftKey.currentKey = (KeyCode)PlayerPrefs.GetInt("LeftKey", (int)defaultLeftKey);
        rightKey.currentKey = (KeyCode)PlayerPrefs.GetInt("RightKey", (int)defaultRightKey);
        jumpKey.currentKey = (KeyCode)PlayerPrefs.GetInt("JumpKey", (int)defaultJumpKey);
        runKey.currentKey = (KeyCode)PlayerPrefs.GetInt("RunKey", (int)defaultRunKey);

        // Graphics Settings
        _vignetteEnabled = PlayerPrefs.GetInt("VignetteEnabled", defaultVignetteEnabled ? 1 : 0) == 1;
        _vignetteIntensity = PlayerPrefs.GetFloat("VignetteIntensity", defaultVignetteIntensity);
        _vignetteSmoothness = PlayerPrefs.GetFloat("VignetteSmoothness", defaultVignetteSmoothness);
        _vignetteRoundness = PlayerPrefs.GetFloat("VignetteRoundness", defaultVignetteRoundness);
        _ssaoEnabled = PlayerPrefs.GetInt("SSAOEnabled", defaultSSAOEnabled ? 1 : 0) == 1;
        _ssaoIntensity = PlayerPrefs.GetFloat("SSAOIntensity", defaultSSAOIntensity);
        _ssaoRadius = PlayerPrefs.GetFloat("SSAORadius", defaultSSAORadius);
        _ssaoDirectLighting = PlayerPrefs.GetFloat("SSAODirectLighting", defaultSSAODirectLighting);
        _fogEnabled = PlayerPrefs.GetInt("FogEnabled", defaultFogEnabled ? 1 : 0) == 1;
        _bloomEnabled = PlayerPrefs.GetInt("BloomEnabled", defaultBloomEnabled ? 1 : 0) == 1;
        _bloomIntensity = PlayerPrefs.GetFloat("BloomIntensity", defaultBloomIntensity);
        _bloomThreshold = PlayerPrefs.GetFloat("BloomThreshold", defaultBloomThreshold);
        _bloomScatter = PlayerPrefs.GetFloat("BloomScatter", defaultBloomScatter);
        _dofEnabled = PlayerPrefs.GetInt("DOFEnabled", defaultDOFEnabled ? 1 : 0) == 1;
        _grainEnabled = PlayerPrefs.GetInt("GrainEnabled", defaultGrainEnabled ? 1 : 0) == 1;
        _grainIntensity = PlayerPrefs.GetFloat("GrainIntensity", defaultGrainIntensity);
        _grainResponse = PlayerPrefs.GetFloat("GrainResponse", defaultGrainResponse);
        _motionBlurEnabled = PlayerPrefs.GetInt("MotionBlurEnabled", defaultMotionBlurEnabled ? 1 : 0) == 1;
        _motionBlurIntensity = PlayerPrefs.GetFloat("MotionBlurIntensity", defaultMotionBlurIntensity);
        _lensFlareEnabled = PlayerPrefs.GetInt("LensFlareEnabled", defaultLensFlareEnabled ? 1 : 0) == 1;
        _lensFlareIntensity = PlayerPrefs.GetFloat("LensFlareIntensity", defaultLensFlareIntensity);
        _chromaticAberrationEnabled = PlayerPrefs.GetInt("ChromaticAberrationEnabled", defaultChromaticAberrationEnabled ? 1 : 0) == 1;
        _waterEnabled = PlayerPrefs.GetInt("WaterEnabled", defaultWaterEnabled ? 1 : 0) == 1;
        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        _fullscreenEnabled = PlayerPrefs.GetInt("FullscreenEnabled", defaultFullscreenEnabled ? 1 : 0) == 1;
        _isEnglish = PlayerPrefs.GetInt("IsEnglish", defaultIsEnglish ? 1 : 0) == 1;
        _resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        _resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        _targetFPS = PlayerPrefs.GetInt("TargetFPS", 60);

        // Ray Tracing
        if (rayTracingSupported)
        {
            if (_rayTracingSettings != null)
            {
                _rayTracingSettings.rayBias.value = PlayerPrefs.GetFloat("RayBias", defaultRayBias);
            }
            if (_pathTracing != null)
            {
                _pathTracing.maximumSamples.value = (int)PlayerPrefs.GetFloat("PathTracingSamples", defaultPathTracingSamples);
                _pathTracing.maximumDepth.value = (int)PlayerPrefs.GetFloat("PathTracingBounces", defaultPathTracingBounces);
            }
            if (_recursiveRendering != null)
                _recursiveRendering.active = PlayerPrefs.GetInt("RecursiveRenderingEnabled", defaultRecursiveRenderingEnabled ? 1 : 0) == 1;
            if (_subsurfaceScattering != null)
                _subsurfaceScattering.active = PlayerPrefs.GetInt("SubsurfaceScatteringEnabled", defaultSubsurfaceScatteringEnabled ? 1 : 0) == 1;
        }
    }

    void SetupKeyBindingButton(KeyBindingButton keyButton, string prefKey)
    {
        keyButton.button.onClick.AddListener(() => StartKeyRebind(keyButton));
        keyButton.currentKey = (KeyCode)PlayerPrefs.GetInt(prefKey, (int)keyButton.defaultKey);
        keyButton.buttonText.text = keyButton.currentKey.ToString();
        keyButton.playerPrefKey = prefKey;
    }

    void StartKeyRebind(KeyBindingButton keyButton)
    {
        isWaitingForKey = true;
        currentKeyButton = keyButton;
        keyButton.buttonText.text = "Press any key...";
    }

    void UpdatePlayerKeyBindings()
    {
        if (playerController != null)
        {
            playerController.forwardKey = forwardKey.currentKey;
            playerController.backwardKey = backwardKey.currentKey;
            playerController.leftKey = leftKey.currentKey;
            playerController.rightKey = rightKey.currentKey;
            playerController.jumpKey = jumpKey.currentKey;
            playerController.runKey = runKey.currentKey;
        }
    }

    void SetLanguage(bool isEnglish)
    {
        _isEnglish = isEnglish;
        PlayerPrefs.SetInt("IsEnglish", isEnglish ? 1 : 0);
        PlayerPrefs.Save();
        
        foreach (var langPair in languageObjects)
        {
            if (langPair.russianObject != null) 
                langPair.russianObject.SetActive(!isEnglish);
            if (langPair.englishObject != null) 
                langPair.englishObject.SetActive(isEnglish);
        }
        
        UpdateTracingWarningText();
    }

    void UpdateTracingWarningText()
    {
        if (tracingWarningText != null)
        {
            tracingWarningText.text = _isEnglish ? 
                "! Your GPU does not support Ray Tracing or Path Tracing !" : 
                "! Ваша видеокарта не поддерживает трассировку лучей или пути! ";
        }
    }

    void SetupUI()
    {
        forwardKey.currentKey = (KeyCode)PlayerPrefs.GetInt("ForwardKey", (int)forwardKey.defaultKey);
        backwardKey.currentKey = (KeyCode)PlayerPrefs.GetInt("BackwardKey", (int)backwardKey.defaultKey);
        leftKey.currentKey = (KeyCode)PlayerPrefs.GetInt("LeftKey", (int)leftKey.defaultKey);
        rightKey.currentKey = (KeyCode)PlayerPrefs.GetInt("RightKey", (int)rightKey.defaultKey);
        jumpKey.currentKey = (KeyCode)PlayerPrefs.GetInt("JumpKey", (int)jumpKey.defaultKey);
        runKey.currentKey = (KeyCode)PlayerPrefs.GetInt("RunKey", (int)runKey.defaultKey);

        forwardKey.buttonText.text = forwardKey.currentKey.ToString();
        backwardKey.buttonText.text = backwardKey.currentKey.ToString();
        leftKey.buttonText.text = leftKey.currentKey.ToString();
        rightKey.buttonText.text = rightKey.currentKey.ToString();
        jumpKey.buttonText.text = jumpKey.currentKey.ToString();
        runKey.buttonText.text = runKey.currentKey.ToString();

        SetupKeyBindingButton(forwardKey, "ForwardKey");
        SetupKeyBindingButton(backwardKey, "BackwardKey");
        SetupKeyBindingButton(leftKey, "LeftKey");
        SetupKeyBindingButton(rightKey, "RightKey");
        SetupKeyBindingButton(jumpKey, "JumpKey");
        SetupKeyBindingButton(runKey, "RunKey");

        if (russianLanguageButton != null)
            russianLanguageButton.onClick.AddListener(() => SetLanguage(false));
        if (englishLanguageButton != null)
            englishLanguageButton.onClick.AddListener(() => SetLanguage(true));

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = _mouseSensitivity;
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        }
        if (cameraSmoothTimeSlider != null)
        {
            cameraSmoothTimeSlider.value = _cameraSmoothTime;
            cameraSmoothTimeSlider.onValueChanged.AddListener(SetCameraSmoothTime);
        }
        if (normalFOVSlider != null)
        {
            normalFOVSlider.value = _normalFOV;
            normalFOVSlider.onValueChanged.AddListener(SetNormalFOV);
        }
        if (runFOVSlider != null)
        {
            runFOVSlider.value = _runFOV;
            runFOVSlider.onValueChanged.AddListener(SetRunFOV);
        }
        if (fovChangeSpeedSlider != null)
        {
            fovChangeSpeedSlider.value = _fovChangeSpeed;
            fovChangeSpeedSlider.onValueChanged.AddListener(SetFOVChangeSpeed);
        }
        if (tiltToggle != null)
        {
            tiltToggle.isOn = _tiltEnabled;
            tiltToggle.onValueChanged.AddListener(SetTiltEnabled);
        }

        if (cameraBobbingToggle != null)
        {
            cameraBobbingToggle.isOn = _cameraBobbingEnabled;
            cameraBobbingToggle.onValueChanged.AddListener(SetCameraBobbingEnabled);
        }
        if (walkBobbingSpeedSlider != null)
        {
            walkBobbingSpeedSlider.value = _walkBobbingSpeed;
            walkBobbingSpeedSlider.onValueChanged.AddListener(SetWalkBobbingSpeed);
        }
        if (walkBobbingAmountSlider != null)
        {
            walkBobbingAmountSlider.value = _walkBobbingAmount;
            walkBobbingAmountSlider.onValueChanged.AddListener(SetWalkBobbingAmount);
        }
        if (runBobbingSpeedSlider != null)
        {
            runBobbingSpeedSlider.value = _runBobbingSpeed;
            runBobbingSpeedSlider.onValueChanged.AddListener(SetRunBobbingSpeed);
        }
        if (runBobbingAmountSlider != null)
        {
            runBobbingAmountSlider.value = _runBobbingAmount;
            runBobbingAmountSlider.onValueChanged.AddListener(SetRunBobbingAmount);
        }
        if (horizontalBobbingAmountSlider != null)
        {
            horizontalBobbingAmountSlider.value = _horizontalBobbingAmount;
            horizontalBobbingAmountSlider.onValueChanged.AddListener(SetHorizontalBobbingAmount);
        }

        if (vignetteToggle != null)
        {
            vignetteToggle.isOn = _vignetteEnabled;
            vignetteToggle.onValueChanged.AddListener(SetVignetteEnabled);
        }
        if (vignetteIntensitySlider != null)
        {
            vignetteIntensitySlider.value = _vignetteIntensity;
            vignetteIntensitySlider.onValueChanged.AddListener(SetVignetteIntensity);
        }
        if (vignetteSmoothnessSlider != null)
        {
            vignetteSmoothnessSlider.value = _vignetteSmoothness;
            vignetteSmoothnessSlider.onValueChanged.AddListener(SetVignetteSmoothness);
        }
        if (vignetteRoundnessSlider != null)
        {
            vignetteRoundnessSlider.value = _vignetteRoundness;
            vignetteRoundnessSlider.onValueChanged.AddListener(SetVignetteRoundness);
        }

        if (ssaoToggle != null)
        {
            ssaoToggle.isOn = _ssaoEnabled;
            ssaoToggle.onValueChanged.AddListener(SetSSAOEnabled);
        }
        if (ssaoIntensitySlider != null)
        {
            ssaoIntensitySlider.value = _ssaoIntensity;
            ssaoIntensitySlider.onValueChanged.AddListener(SetSSAOIntensity);
        }
        if (ssaoRadiusSlider != null)
        {
            ssaoRadiusSlider.value = _ssaoRadius;
            ssaoRadiusSlider.onValueChanged.AddListener(SetSSAORadius);
        }
        if (ssaoDirectLightingSlider != null)
        {
            ssaoDirectLightingSlider.value = _ssaoDirectLighting;
            ssaoDirectLightingSlider.onValueChanged.AddListener(SetSSAODirectLighting);
        }

        if (fogToggle != null)
        {
            fogToggle.isOn = _fogEnabled;
            fogToggle.onValueChanged.AddListener(SetFogEnabled);
        }

        if (bloomToggle != null)
        {
            bloomToggle.isOn = _bloomEnabled;
            bloomToggle.onValueChanged.AddListener(SetBloomEnabled);
        }
        if (bloomIntensitySlider != null)
        {
            bloomIntensitySlider.value = _bloomIntensity;
            bloomIntensitySlider.onValueChanged.AddListener(SetBloomIntensity);
        }
        if (bloomThresholdSlider != null)
        {
            bloomThresholdSlider.value = _bloomThreshold;
            bloomThresholdSlider.onValueChanged.AddListener(SetBloomThreshold);
        }
        if (bloomScatterSlider != null)
        {
            bloomScatterSlider.value = _bloomScatter;
            bloomScatterSlider.onValueChanged.AddListener(SetBloomScatter);
        }

        if (dofToggle != null)
        {
            dofToggle.isOn = _dofEnabled;
            dofToggle.onValueChanged.AddListener(SetDOFEnabled);
        }

        if (grainToggle != null)
        {
            grainToggle.isOn = _grainEnabled;
            grainToggle.onValueChanged.AddListener(SetGrainEnabled);
        }
        if (grainIntensitySlider != null)
        {
            grainIntensitySlider.value = _grainIntensity;
            grainIntensitySlider.onValueChanged.AddListener(SetGrainIntensity);
        }
        if (grainResponseSlider != null)
        {
            grainResponseSlider.value = _grainResponse;
            grainResponseSlider.onValueChanged.AddListener(SetGrainResponse);
        }

        if (motionBlurToggle != null)
        {
            motionBlurToggle.isOn = _motionBlurEnabled;
            motionBlurToggle.onValueChanged.AddListener(SetMotionBlurEnabled);
        }
        if (motionBlurIntensitySlider != null)
        {
            motionBlurIntensitySlider.value = _motionBlurIntensity;
            motionBlurIntensitySlider.onValueChanged.AddListener(SetMotionBlurIntensity);
        }

        if (lensFlareToggle != null)
        {
            lensFlareToggle.isOn = _lensFlareEnabled;
            lensFlareToggle.onValueChanged.AddListener(SetLensFlareEnabled);
        }
        if (lensFlareIntensitySlider != null)
        {
            lensFlareIntensitySlider.value = _lensFlareIntensity;
            lensFlareIntensitySlider.onValueChanged.AddListener(SetLensFlareIntensity);
        }

        if (chromaticAberrationToggle != null)
        {
            chromaticAberrationToggle.isOn = _chromaticAberrationEnabled;
            chromaticAberrationToggle.onValueChanged.AddListener(SetChromaticAberrationEnabled);
        }

        if (waterToggle != null)
        {
            waterToggle.isOn = _waterEnabled;
            waterToggle.onValueChanged.AddListener(SetWaterEnabled);
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = _masterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = _fullscreenEnabled;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreenEnabled);
        }

        if (resolution360pButton != null)
            resolution360pButton.onClick.AddListener(SetResolution360p);
        if (resolution480pButton != null)
            resolution480pButton.onClick.AddListener(SetResolution480p);
        if (resolution720pButton != null)
            resolution720pButton.onClick.AddListener(SetResolution720p);
        if (resolution1080pButton != null)
            resolution1080pButton.onClick.AddListener(SetResolution1080p);
        if (resolution1440pButton != null)
            resolution1440pButton.onClick.AddListener(SetResolution1440p);
        if (resolution2160pButton != null)
            resolution2160pButton.onClick.AddListener(SetResolution2160p);

        if (enableRayTracingButton != null)
        {
            enableRayTracingButton.onClick.AddListener(EnableRayTracingOnly);
            enableRayTracingButton.interactable = rayTracingSupported;
        }
        
        if (enablePathTracingButton != null)
        {
            enablePathTracingButton.onClick.AddListener(EnablePathTracingOnly);
            enablePathTracingButton.interactable = pathTracingSupported;
        }
        
        if (disableTracingButton != null)
        {
            disableTracingButton.onClick.AddListener(DisableAllTracing);
            disableTracingButton.interactable = rayTracingSupported || pathTracingSupported;
        }

        if (tracingWarningText != null)
        {
            tracingWarningText.gameObject.SetActive(!rayTracingSupported);
            UpdateTracingWarningText();
        }

    if (showFPSToggle != null)
    {
        showFPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        showFPSToggle.onValueChanged.AddListener(SetShowFPS);
    
        if (fpsCounterText != null)
        {
            fpsCounterText.gameObject.SetActive(showFPSToggle.isOn);
        }
    }

        if (fpsCounterText != null)
        {
            fpsCounterText.gameObject.SetActive(showFPSToggle.isOn);
        }

        if (pathTracingToggle != null)
        {
            pathTracingToggle.isOn = _pathTracing != null && _pathTracing.active;
            pathTracingToggle.onValueChanged.AddListener(SetPathTracingEnabled);
            pathTracingToggle.interactable = pathTracingSupported;
        }

        if (recursiveRenderingToggle != null)
        {
            recursiveRenderingToggle.isOn = _recursiveRendering != null && _recursiveRendering.active;
            recursiveRenderingToggle.onValueChanged.AddListener(SetRecursiveRenderingEnabled);
            recursiveRenderingToggle.interactable = rayTracingSupported;
        }

        if (subsurfaceScatteringToggle != null)
        {
            subsurfaceScatteringToggle.isOn = _subsurfaceScattering != null && _subsurfaceScattering.active;
            subsurfaceScatteringToggle.onValueChanged.AddListener(SetSubsurfaceScatteringEnabled);
            subsurfaceScatteringToggle.interactable = rayTracingSupported;
        }

        if (rayBiasSlider != null && _rayTracingSettings != null)
        {
            rayBiasSlider.value = _rayTracingSettings.rayBias.value;
            rayBiasSlider.onValueChanged.AddListener(SetRayBias);
            rayBiasSlider.interactable = rayTracingSupported;
        }

        if (pathTracingSamplesSlider != null && _pathTracing != null)
        {
            pathTracingSamplesSlider.value = _pathTracing.maximumSamples.value;
            pathTracingSamplesSlider.onValueChanged.AddListener(SetPathTracingSamples);
            pathTracingSamplesSlider.interactable = pathTracingSupported;
        }

        if (pathTracingBouncesSlider != null && _pathTracing != null)
        {
            pathTracingBouncesSlider.value = _pathTracing.maximumDepth.value;
            pathTracingBouncesSlider.onValueChanged.AddListener(SetPathTracingBounces);
            pathTracingBouncesSlider.interactable = pathTracingSupported;
        }
    }

    void Update()
    {
        _saveTimer += Time.unscaledDeltaTime;
        if (_saveTimer >= SAVE_INTERVAL)
        {
            _saveTimer = 0f;
            SaveCurrentSettings();
        }
        
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreenMode();
        }

        if (isWaitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        currentKeyButton.currentKey = keyCode;
                        currentKeyButton.buttonText.text = keyCode.ToString();
                        isWaitingForKey = false;
                        currentKeyButton = null;

                        UpdatePlayerKeyBindings();
                        SaveCurrentSettings();
                        break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && currentKeyButton != null)
            {
                isWaitingForKey = false;
                currentKeyButton.buttonText.text = currentKeyButton.currentKey.ToString();
                currentKeyButton = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            ToggleFullscreen();
        }

        if (showFPSToggle != null && showFPSToggle.isOn && fpsCounterText != null)
        {
            _frameCount++;
            _fpsUpdateTimer += Time.unscaledDeltaTime;

            if (_fpsUpdateTimer >= FPS_UPDATE_INTERVAL)
            {
                _currentFPS = _frameCount / _fpsUpdateTimer;
                fpsCounterText.text = $"FPS: {_currentFPS:0}";
            
                // Сбрасываем счетчики
                _frameCount = 0;
                _fpsUpdateTimer = 0f;
            }
        }
    }

    void ApplyAllSettings()
    {
        if (rayTracingSupported)
        {
            bool rtEnabled = PlayerPrefs.GetInt("RayTracingEnabled", 1) == 1;
            bool ptEnabled = PlayerPrefs.GetInt("PathTracingEnabled", 0) == 1;
            
            if (_rayTracingSettings != null) _rayTracingSettings.active = rtEnabled && !ptEnabled;
            if (_pathTracing != null) _pathTracing.active = ptEnabled;
            if (_recursiveRendering != null) 
                _recursiveRendering.active = PlayerPrefs.GetInt("RecursiveRenderingEnabled", 1) == 1;
            if (_subsurfaceScattering != null) 
                _subsurfaceScattering.active = PlayerPrefs.GetInt("SubsurfaceScatteringEnabled", 1) == 1;
            
            if (_rayTracingSettings != null)
            {
                _rayTracingSettings.rayBias.value = PlayerPrefs.GetFloat("RayBias", defaultRayBias);
            }
            
            if (_pathTracing != null)
            {
                _pathTracing.maximumSamples.value = (int)PlayerPrefs.GetFloat("PathTracingSamples", defaultPathTracingSamples);
                _pathTracing.maximumDepth.value = (int)PlayerPrefs.GetFloat("PathTracingBounces", defaultPathTracingBounces);
            }
        }
        else
        {
            DisableAllTracing();
        }

        if (playerController != null)
        {
            playerController.mouseSensitivity = _mouseSensitivity;
            playerController.cameraSmoothTime = _cameraSmoothTime;
            playerController.normalFOV = _normalFOV;
            playerController.runFOV = _runFOV;
            playerController.fovChangeSpeed = _fovChangeSpeed;
            playerController.tiltEnabled = _tiltEnabled;
            playerController.walkBobbingSpeed = _walkBobbingSpeed;
            playerController.walkBobbingAmount = _walkBobbingAmount;
            playerController.runBobbingSpeed = _runBobbingSpeed;
            playerController.runBobbingAmount = _runBobbingAmount;
            playerController.horizontalBobbingAmount = _horizontalBobbingAmount;
            
            UpdatePlayerKeyBindings();
        }

        if (_vignette != null)
        {
            _vignette.active = _vignetteEnabled;
            _vignette.intensity.value = _vignetteIntensity;
            _vignette.smoothness.value = _vignetteSmoothness;
            _vignette.roundness.value = _vignetteRoundness;
        }

        if (_ssao != null)
        {
            _ssao.active = _ssaoEnabled;
            _ssao.intensity.value = _ssaoIntensity;
            _ssao.radius.value = _ssaoRadius;
            _ssao.directLightingStrength.value = _ssaoDirectLighting;
        }

        if (_fog != null) _fog.active = _fogEnabled;

        if (_bloom != null)
        {
            _bloom.active = _bloomEnabled;
            _bloom.intensity.value = _bloomIntensity;
            _bloom.threshold.value = _bloomThreshold;
            _bloom.scatter.value = _bloomScatter;
        }

        if (_dof != null) _dof.active = _dofEnabled;

        if (_grain != null)
        {
            _grain.active = _grainEnabled;
            _grain.intensity.value = _grainIntensity;
            _grain.response.value = _grainResponse;
        }

        if (_motionBlur != null)
        {
            _motionBlur.active = _motionBlurEnabled;
            _motionBlur.intensity.value = _motionBlurIntensity;
        }

        if (_lensFlare != null)
        {
            _lensFlare.active = _lensFlareEnabled;
            _lensFlare.intensity.value = _lensFlareIntensity;
        }

        if (_chromaticAberration != null) 
            _chromaticAberration.active = _chromaticAberrationEnabled;

        foreach (var waterObj in waterObjects)
        {
            if (waterObj != null) waterObj.SetActive(_waterEnabled);
        }

        AudioListener.volume = _masterVolume;
        Screen.SetResolution(_resolutionWidth, _resolutionHeight, _fullscreenEnabled);
        Application.targetFrameRate = _targetFPS;
    }

    public void EnableRayTracingOnly()
    {
        if (_rayTracingSettings != null)
        {
            _rayTracingSettings.active = true;
            if (_pathTracing != null) _pathTracing.active = false;
            
            if (pathTracingToggle != null) pathTracingToggle.isOn = false;
            
            StartFPSMeasurement();
            PlayerPrefs.SetInt("RayTracingEnabled", 1);
            PlayerPrefs.SetInt("PathTracingEnabled", 0);
            PlayerPrefs.Save();
        }
    }

    public void EnablePathTracingOnly()
    {
        if (_pathTracing != null)
        {
            _pathTracing.active = true;
            if (_rayTracingSettings != null) _rayTracingSettings.active = false;
            
            if (pathTracingToggle != null) pathTracingToggle.isOn = true;
            
            StartFPSMeasurement();
            PlayerPrefs.SetInt("RayTracingEnabled", 0);
            PlayerPrefs.SetInt("PathTracingEnabled", 1);
            PlayerPrefs.Save();
        }
    }

    public void DisableAllTracing()
    {
        if (_rayTracingSettings != null) _rayTracingSettings.active = false;
        if (_pathTracing != null) _pathTracing.active = false;
        
        if (pathTracingToggle != null) pathTracingToggle.isOn = false;
        
        PlayerPrefs.SetInt("RayTracingEnabled", 0);
        PlayerPrefs.SetInt("PathTracingEnabled", 0);
        PlayerPrefs.Save();
    }

    public void SetPathTracingEnabled(bool enabled)
    {
        if (_pathTracing != null)
        {
            _pathTracing.active = enabled;
            if (enabled && _rayTracingSettings != null)
            {
                _rayTracingSettings.active = false;
            }
            PlayerPrefs.SetInt("PathTracingEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void SetRecursiveRenderingEnabled(bool enabled)
    {
        if (_recursiveRendering != null)
        {
            _recursiveRendering.active = enabled;
            PlayerPrefs.SetInt("RecursiveRenderingEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void SetSubsurfaceScatteringEnabled(bool enabled)
    {
        if (_subsurfaceScattering != null)
        {
            _subsurfaceScattering.active = enabled;
            PlayerPrefs.SetInt("SubsurfaceScatteringEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void SetRayBias(float value)
    {
        if (_rayTracingSettings != null)
        {
            _rayTracingSettings.rayBias.value = value;
            PlayerPrefs.SetFloat("RayBias", value);
            PlayerPrefs.Save();
        }
    }

    public void SetPathTracingSamples(float value)
    {
        if (_pathTracing != null)
        {
            _pathTracing.maximumSamples.value = (int)value;
            PlayerPrefs.SetFloat("PathTracingSamples", value);
            PlayerPrefs.Save();
        }
    }

    public void SetPathTracingBounces(float value)
    {
        if (_pathTracing != null)
        {
            _pathTracing.maximumDepth.value = (int)value;
            PlayerPrefs.SetFloat("PathTracingBounces", value);
            PlayerPrefs.Save();
        }
    }

public void SetShowFPS(bool show)
{
    PlayerPrefs.SetInt("ShowFPS", show ? 1 : 0);
    PlayerPrefs.Save();
    
    if (fpsCounterText != null)
    {
        fpsCounterText.gameObject.SetActive(show);
        
        // Сбрасываем таймер при включении/выключении
        if (show)
        {
            _frameCount = 0;
            _fpsUpdateTimer = 0f;
            fpsCounterText.text = "FPS: -";
        }
    }
}

    void StartFPSMeasurement()
    {
        if (PlayerPrefs.GetInt("FirstTimeRayTracing", 1) == 1)
        {
            _fpsMeasurements.Clear();
            _fpsMeasureTimer = 0f;
            _isMeasuringFPS = true;
            PlayerPrefs.SetInt("FirstTimeRayTracing", 0);
            PlayerPrefs.Save();
        }
    }

    public void SetResolution360p() => SetResolution(640, 360);
    public void SetResolution480p() => SetResolution(854, 480);
    public void SetResolution720p() => SetResolution(1280, 720);
    public void SetResolution1080p() => SetResolution(1920, 1080);
    public void SetResolution1440p() => SetResolution(2560, 1440);
    public void SetResolution2160p() => SetResolution(3840, 2160);

    private void SetResolution(int width, int height)
    {
        _resolutionWidth = width;
        _resolutionHeight = height;
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.Save();
        Screen.SetResolution(width, height, _fullscreenEnabled);
    }

    private void ToggleFullscreenMode()
{
    _fullscreenEnabled = !_fullscreenEnabled;
    if (fullscreenToggle != null)
    {
        fullscreenToggle.isOn = _fullscreenEnabled;
    }
    SetFullscreenEnabled(_fullscreenEnabled);
}
    public void SaveSettings()
    {
        SaveCurrentSettings();
        Debug.Log("Настройки успешно сохранены");
    }

    public void LoadSettingsManual()
    {
        LoadSettings();
        ApplyAllSettings();
        UpdateUI();
        Debug.Log("Настройки успешно загружены");
    }

    public void SetCameraBobbingEnabled(bool enabled)
    {
        _cameraBobbingEnabled = enabled;
        PlayerPrefs.SetInt("CameraBobbingEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetWalkBobbingSpeed(float value)
    {
        _walkBobbingSpeed = value;
        PlayerPrefs.SetFloat("WalkBobbingSpeed", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.walkBobbingSpeed = value;
    }

    public void SetWalkBobbingAmount(float value)
    {
        _walkBobbingAmount = value;
        PlayerPrefs.SetFloat("WalkBobbingAmount", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.walkBobbingAmount = value;
    }

    public void SetRunBobbingSpeed(float value)
    {
        _runBobbingSpeed = value;
        PlayerPrefs.SetFloat("RunBobbingSpeed", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.runBobbingSpeed = value;
    }

    public void SetRunBobbingAmount(float value)
    {
        _runBobbingAmount = value;
        PlayerPrefs.SetFloat("RunBobbingAmount", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.runBobbingAmount = value;
    }

    public void SetHorizontalBobbingAmount(float value)
    {
        _horizontalBobbingAmount = value;
        PlayerPrefs.SetFloat("HorizontalBobbingAmount", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.horizontalBobbingAmount = value;
    }

    public void ToggleFullscreen()
    {
        _fullscreenEnabled = !_fullscreenEnabled;
        PlayerPrefs.SetInt("FullscreenEnabled", _fullscreenEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyAllSettings();

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = _fullscreenEnabled;
    }

    public void SetMouseSensitivity(float value)
    {
        _mouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.mouseSensitivity = value;
    }

    public void SetCameraSmoothTime(float value)
    {
        _cameraSmoothTime = value;
        PlayerPrefs.SetFloat("CameraSmoothTime", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.cameraSmoothTime = value;
    }

    public void SetNormalFOV(float value)
    {
        _normalFOV = value;
        PlayerPrefs.SetFloat("NormalFOV", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.normalFOV = value;
    }

    public void SetRunFOV(float value)
    {
        _runFOV = value;
        PlayerPrefs.SetFloat("RunFOV", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.runFOV = value;
    }

    public void SetFOVChangeSpeed(float value)
    {
        _fovChangeSpeed = value;
        PlayerPrefs.SetFloat("FOVChangeSpeed", value);
        PlayerPrefs.Save();
        if (playerController != null) playerController.fovChangeSpeed = value;
    }

    public void SetTiltEnabled(bool enabled)
    {
        _tiltEnabled = enabled;
        PlayerPrefs.SetInt("TiltEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        if (playerController != null) playerController.tiltEnabled = enabled;
    }

    public void SetVignetteEnabled(bool enabled)
    {
        _vignetteEnabled = enabled;
        if (_vignette != null) _vignette.active = enabled;
        PlayerPrefs.SetInt("VignetteEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVignetteIntensity(float value)
    {
        _vignetteIntensity = value;
        if (_vignette != null) _vignette.intensity.value = value;
        PlayerPrefs.SetFloat("VignetteIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetVignetteSmoothness(float value)
    {
        _vignetteSmoothness = value;
        if (_vignette != null) _vignette.smoothness.value = value;
        PlayerPrefs.SetFloat("VignetteSmoothness", value);
        PlayerPrefs.Save();
    }

    public void SetVignetteRoundness(float value)
    {
        _vignetteRoundness = value;
        if (_vignette != null) _vignette.roundness.value = value;
        PlayerPrefs.SetFloat("VignetteRoundness", value);
        PlayerPrefs.Save();
    }

    public void SetSSAOEnabled(bool enabled)
    {
        _ssaoEnabled = enabled;
        if (_ssao != null) _ssao.active = enabled;
        PlayerPrefs.SetInt("SSAOEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSSAOIntensity(float value)
    {
        _ssaoIntensity = value;
        if (_ssao != null) _ssao.intensity.value = value;
        PlayerPrefs.SetFloat("SSAOIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetSSAORadius(float value)
    {
        _ssaoRadius = value;
        if (_ssao != null) _ssao.radius.value = value;
        PlayerPrefs.SetFloat("SSAORadius", value);
        PlayerPrefs.Save();
    }

    public void SetSSAODirectLighting(float value)
    {
        _ssaoDirectLighting = value;
        if (_ssao != null) _ssao.directLightingStrength.value = value;
        PlayerPrefs.SetFloat("SSAODirectLighting", value);
        PlayerPrefs.Save();
    }

    public void SetFogEnabled(bool enabled)
    {
        _fogEnabled = enabled;
        if (_fog != null) _fog.active = enabled;
        PlayerPrefs.SetInt("FogEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetBloomEnabled(bool enabled)
    {
        _bloomEnabled = enabled;
        if (_bloom != null) _bloom.active = enabled;
        PlayerPrefs.SetInt("BloomEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetBloomIntensity(float value)
    {
        _bloomIntensity = value;
        if (_bloom != null) _bloom.intensity.value = value;
        PlayerPrefs.SetFloat("BloomIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetBloomThreshold(float value)
    {
        _bloomThreshold = value;
        if (_bloom != null) _bloom.threshold.value = value;
        PlayerPrefs.SetFloat("BloomThreshold", value);
        PlayerPrefs.Save();
    }

    public void SetBloomScatter(float value)
    {
        _bloomScatter = value;
        if (_bloom != null) _bloom.scatter.value = value;
        PlayerPrefs.SetFloat("BloomScatter", value);
        PlayerPrefs.Save();
    }

    public void SetDOFEnabled(bool enabled)
    {
        _dofEnabled = enabled;
        if (_dof != null) _dof.active = enabled;
        PlayerPrefs.SetInt("DOFEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetGrainEnabled(bool enabled)
    {
        _grainEnabled = enabled;
        if (_grain != null) _grain.active = enabled;
        PlayerPrefs.SetInt("GrainEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetGrainIntensity(float value)
    {
        _grainIntensity = value;
        if (_grain != null) _grain.intensity.value = value;
        PlayerPrefs.SetFloat("GrainIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetGrainResponse(float value)
    {
        _grainResponse = value;
        if (_grain != null) _grain.response.value = value;
        PlayerPrefs.SetFloat("GrainResponse", value);
        PlayerPrefs.Save();
    }

    public void SetMotionBlurEnabled(bool enabled)
    {
        _motionBlurEnabled = enabled;
        if (_motionBlur != null) _motionBlur.active = enabled;
        PlayerPrefs.SetInt("MotionBlurEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMotionBlurIntensity(float value)
    {
        _motionBlurIntensity = value;
        if (_motionBlur != null) _motionBlur.intensity.value = value;
        PlayerPrefs.SetFloat("MotionBlurIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetLensFlareEnabled(bool enabled)
    {
        _lensFlareEnabled = enabled;
        if (_lensFlare != null) _lensFlare.active = enabled;
        PlayerPrefs.SetInt("LensFlareEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetLensFlareIntensity(float value)
    {
        _lensFlareIntensity = value;
        if (_lensFlare != null) _lensFlare.intensity.value = value;
        PlayerPrefs.SetFloat("LensFlareIntensity", value);
        PlayerPrefs.Save();
    }

    public void SetChromaticAberrationEnabled(bool enabled)
    {
        _chromaticAberrationEnabled = enabled;
        if (_chromaticAberration != null) _chromaticAberration.active = enabled;
        PlayerPrefs.SetInt("ChromaticAberrationEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetWaterEnabled(bool enabled)
    {
        _waterEnabled = enabled;
        foreach (var waterObj in waterObjects)
        {
            if (waterObj != null) waterObj.SetActive(enabled);
        }
        PlayerPrefs.SetInt("WaterEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        _masterVolume = value;
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

public void SetFullscreenEnabled(bool enabled)
{
    _fullscreenEnabled = enabled;
    PlayerPrefs.SetInt("FullscreenEnabled", enabled ? 1 : 0);
    PlayerPrefs.Save();
    
    // Изменяем режим на FullScreenWindow (резизибл) вместо обычного Windowed
    Screen.fullScreenMode = enabled ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
    ApplyAllSettings();
}

public void SetFPS60()
{
    _targetFPS = 60;
    QualitySettings.vSyncCount = 0; // Отключаем вертикальную синхронизацию
    Application.targetFrameRate = _targetFPS;
    PlayerPrefs.SetInt("TargetFPS", _targetFPS);
    PlayerPrefs.Save();
}

public void SetFPS120()
{
    _targetFPS = 120;
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = _targetFPS;
    PlayerPrefs.SetInt("TargetFPS", _targetFPS);
    PlayerPrefs.Save();
}

public void SetFPS240()
{
    _targetFPS = 240;
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = _targetFPS;
    PlayerPrefs.SetInt("TargetFPS", _targetFPS);
    PlayerPrefs.Save();
}

public void SetFPSUnlimited()
{
    _targetFPS = 0;
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = -1; // Для unlimited лучше использовать -1
    PlayerPrefs.SetInt("TargetFPS", _targetFPS);
    PlayerPrefs.Save();
}

    public void ResetToDefaultSettings()
    {
        // Player Settings
        _mouseSensitivity = defaultMouseSensitivity;
        _cameraSmoothTime = defaultCameraSmoothTime;
        _normalFOV = defaultNormalFOV;
        _runFOV = defaultRunFOV;
        _fovChangeSpeed = defaultFOVChangeSpeed;
        _tiltEnabled = defaultTiltEnabled;
        _cameraBobbingEnabled = defaultCameraBobbingEnabled;
        _walkBobbingSpeed = defaultWalkBobbingSpeed;
        _walkBobbingAmount = defaultWalkBobbingAmount;
        _runBobbingSpeed = defaultRunBobbingSpeed;
        _runBobbingAmount = defaultRunBobbingAmount;
        _horizontalBobbingAmount = defaultHorizontalBobbingAmount;

        // Key Bindings
        forwardKey.currentKey = defaultForwardKey;
        backwardKey.currentKey = defaultBackwardKey;
        leftKey.currentKey = defaultLeftKey;
        rightKey.currentKey = defaultRightKey;
        jumpKey.currentKey = defaultJumpKey;
        runKey.currentKey = defaultRunKey;

        // Vignette
        _vignetteEnabled = defaultVignetteEnabled;
        _vignetteIntensity = defaultVignetteIntensity;
        _vignetteSmoothness = defaultVignetteSmoothness;
        _vignetteRoundness = defaultVignetteRoundness;

        // SSAO
        _ssaoEnabled = defaultSSAOEnabled;
        _ssaoIntensity = defaultSSAOIntensity;
        _ssaoRadius = defaultSSAORadius;
        _ssaoDirectLighting = defaultSSAODirectLighting;

        // Fog
        _fogEnabled = defaultFogEnabled;

        // Bloom
        _bloomEnabled = defaultBloomEnabled;
        _bloomIntensity = defaultBloomIntensity;
        _bloomThreshold = defaultBloomThreshold;
        _bloomScatter = defaultBloomScatter;

        // Depth of Field
        _dofEnabled = defaultDOFEnabled;

        // Film Grain
        _grainEnabled = defaultGrainEnabled;
        _grainIntensity = defaultGrainIntensity;
        _grainResponse = defaultGrainResponse;

        // Motion Blur
        _motionBlurEnabled = defaultMotionBlurEnabled;
        _motionBlurIntensity = defaultMotionBlurIntensity;

        // Lens Flare
        _lensFlareEnabled = defaultLensFlareEnabled;
        _lensFlareIntensity = defaultLensFlareIntensity;

        // Chromatic Aberration
        _chromaticAberrationEnabled = defaultChromaticAberrationEnabled;

        // Water
        _waterEnabled = defaultWaterEnabled;

        // Audio
        _masterVolume = defaultMasterVolume;

        // Resolution
        _fullscreenEnabled = defaultFullscreenEnabled;
        _isEnglish = defaultIsEnglish;
        _resolutionWidth = 1920;
        _resolutionHeight = 1080;

        // Ray Tracing
        if (_rayTracingSettings != null)
        {
            _rayTracingSettings.rayBias.value = defaultRayBias;
        }
        if (_pathTracing != null)
        {
            _pathTracing.maximumSamples.value = defaultPathTracingSamples;
            _pathTracing.maximumDepth.value = defaultPathTracingBounces;
        }
        if (_recursiveRendering != null)
            _recursiveRendering.active = defaultRecursiveRenderingEnabled;
        if (_subsurfaceScattering != null)
            _subsurfaceScattering.active = defaultSubsurfaceScatteringEnabled;

        ApplyAllSettings();
        UpdateUI();
        SaveCurrentSettings();
    }

    void SaveCurrentSettings()
    {
        // Player Settings
        PlayerPrefs.SetFloat("MouseSensitivity", _mouseSensitivity);
        PlayerPrefs.SetFloat("CameraSmoothTime", _cameraSmoothTime);
        PlayerPrefs.SetFloat("NormalFOV", _normalFOV);
        PlayerPrefs.SetFloat("RunFOV", _runFOV);
        PlayerPrefs.SetFloat("FOVChangeSpeed", _fovChangeSpeed);
        PlayerPrefs.SetInt("TiltEnabled", _tiltEnabled ? 1 : 0);
        PlayerPrefs.SetInt("CameraBobbingEnabled", _cameraBobbingEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("WalkBobbingSpeed", _walkBobbingSpeed);
        PlayerPrefs.SetFloat("WalkBobbingAmount", _walkBobbingAmount);
        PlayerPrefs.SetFloat("RunBobbingSpeed", _runBobbingSpeed);
        PlayerPrefs.SetFloat("RunBobbingAmount", _runBobbingAmount);
        PlayerPrefs.SetFloat("HorizontalBobbingAmount", _horizontalBobbingAmount);

        // Key Bindings
        PlayerPrefs.SetInt("ForwardKey", (int)forwardKey.currentKey);
        PlayerPrefs.SetInt("BackwardKey", (int)backwardKey.currentKey);
        PlayerPrefs.SetInt("LeftKey", (int)leftKey.currentKey);
        PlayerPrefs.SetInt("RightKey", (int)rightKey.currentKey);
        PlayerPrefs.SetInt("JumpKey", (int)jumpKey.currentKey);
        PlayerPrefs.SetInt("RunKey", (int)runKey.currentKey);

        // Vignette
        PlayerPrefs.SetInt("VignetteEnabled", _vignetteEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("VignetteIntensity", _vignetteIntensity);
        PlayerPrefs.SetFloat("VignetteSmoothness", _vignetteSmoothness);
        PlayerPrefs.SetFloat("VignetteRoundness", _vignetteRoundness);

        // SSAO
        PlayerPrefs.SetInt("SSAOEnabled", _ssaoEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("SSAOIntensity", _ssaoIntensity);
        PlayerPrefs.SetFloat("SSAORadius", _ssaoRadius);
        PlayerPrefs.SetFloat("SSAODirectLighting", _ssaoDirectLighting);

        // Fog
        PlayerPrefs.SetInt("FogEnabled", _fogEnabled ? 1 : 0);

        // Bloom
        PlayerPrefs.SetInt("BloomEnabled", _bloomEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("BloomIntensity", _bloomIntensity);
        PlayerPrefs.SetFloat("BloomThreshold", _bloomThreshold);
        PlayerPrefs.SetFloat("BloomScatter", _bloomScatter);

        // Depth of Field
        PlayerPrefs.SetInt("DOFEnabled", _dofEnabled ? 1 : 0);

        // Film Grain
        PlayerPrefs.SetInt("GrainEnabled", _grainEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("GrainIntensity", _grainIntensity);
        PlayerPrefs.SetFloat("GrainResponse", _grainResponse);

        // Motion Blur
        PlayerPrefs.SetInt("MotionBlurEnabled", _motionBlurEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("MotionBlurIntensity", _motionBlurIntensity);

        // Lens Flare
        PlayerPrefs.SetInt("LensFlareEnabled", _lensFlareEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("LensFlareIntensity", _lensFlareIntensity);

        // Chromatic Aberration
        PlayerPrefs.SetInt("ChromaticAberrationEnabled", _chromaticAberrationEnabled ? 1 : 0);

        // Water
        PlayerPrefs.SetInt("WaterEnabled", _waterEnabled ? 1 : 0);

        // Audio
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);

        // Resolution
        PlayerPrefs.SetInt("FullscreenEnabled", _fullscreenEnabled ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", _resolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", _resolutionHeight);
        PlayerPrefs.SetInt("IsEnglish", _isEnglish ? 1 : 0);

        // Ray Tracing
        PlayerPrefs.SetInt("RayTracingEnabled", _rayTracingSettings != null && _rayTracingSettings.active ? 1 : 0);
        PlayerPrefs.SetInt("PathTracingEnabled", _pathTracing != null && _pathTracing.active ? 1 : 0);
        PlayerPrefs.SetFloat("RayBias", _rayTracingSettings != null ? _rayTracingSettings.rayBias.value : defaultRayBias);
        PlayerPrefs.SetFloat("PathTracingSamples", _pathTracing != null ? _pathTracing.maximumSamples.value : defaultPathTracingSamples);
        PlayerPrefs.SetFloat("PathTracingBounces", _pathTracing != null ? _pathTracing.maximumDepth.value : defaultPathTracingBounces);
        PlayerPrefs.SetInt("RecursiveRenderingEnabled", _recursiveRendering != null && _recursiveRendering.active ? 1 : 0);
        PlayerPrefs.SetInt("SubsurfaceScatteringEnabled", _subsurfaceScattering != null && _subsurfaceScattering.active ? 1 : 0);

        // FPS
        PlayerPrefs.SetInt("TargetFPS", _targetFPS);
        PlayerPrefs.SetInt("ShowFPS", showFPSToggle != null && showFPSToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FirstTimeRayTracing", 1);

        PlayerPrefs.Save();
    }

    [ContextMenu("Find All Water Objects")]
    void FindAllWaterObjects()
    {
        waterObjects = GameObject.FindGameObjectsWithTag("Water").ToList();
        Debug.Log($"Found {waterObjects.Count} water objects");
    }

    void OnEnable()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (russianLanguageButton != null)
            russianLanguageButton.interactable = _isEnglish;
        if (englishLanguageButton != null)
            englishLanguageButton.interactable = !_isEnglish;

        if (forwardKey.buttonText != null)
            forwardKey.buttonText.text = forwardKey.currentKey.ToString();
        if (backwardKey.buttonText != null)
            backwardKey.buttonText.text = backwardKey.currentKey.ToString();
        if (leftKey.buttonText != null)
            leftKey.buttonText.text = leftKey.currentKey.ToString();
        if (rightKey.buttonText != null)
            rightKey.buttonText.text = rightKey.currentKey.ToString();
        if (jumpKey.buttonText != null)
            jumpKey.buttonText.text = jumpKey.currentKey.ToString();
        if (runKey.buttonText != null)
            runKey.buttonText.text = runKey.currentKey.ToString();

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = _mouseSensitivity;
        if (cameraSmoothTimeSlider != null)
            cameraSmoothTimeSlider.value = _cameraSmoothTime;
        if (normalFOVSlider != null)
            normalFOVSlider.value = _normalFOV;
        if (runFOVSlider != null)
            runFOVSlider.value = _runFOV;
        if (fovChangeSpeedSlider != null)
            fovChangeSpeedSlider.value = _fovChangeSpeed;
        if (tiltToggle != null)
            tiltToggle.isOn = _tiltEnabled;
        if (cameraBobbingToggle != null)
            cameraBobbingToggle.isOn = _cameraBobbingEnabled;
        if (walkBobbingSpeedSlider != null)
            walkBobbingSpeedSlider.value = _walkBobbingSpeed;
        if (walkBobbingAmountSlider != null)
            walkBobbingAmountSlider.value = _walkBobbingAmount;
        if (runBobbingSpeedSlider != null)
            runBobbingSpeedSlider.value = _runBobbingSpeed;
        if (runBobbingAmountSlider != null)
            runBobbingAmountSlider.value = _runBobbingAmount;
        if (horizontalBobbingAmountSlider != null)
            horizontalBobbingAmountSlider.value = _horizontalBobbingAmount;

        if (vignetteToggle != null)
            vignetteToggle.isOn = _vignetteEnabled;
        if (vignetteIntensitySlider != null)
            vignetteIntensitySlider.value = _vignetteIntensity;
        if (vignetteSmoothnessSlider != null)
            vignetteSmoothnessSlider.value = _vignetteSmoothness;
        if (vignetteRoundnessSlider != null)
            vignetteRoundnessSlider.value = _vignetteRoundness;

        if (ssaoToggle != null)
            ssaoToggle.isOn = _ssaoEnabled;
        if (ssaoIntensitySlider != null)
            ssaoIntensitySlider.value = _ssaoIntensity;
        if (ssaoRadiusSlider != null)
            ssaoRadiusSlider.value = _ssaoRadius;
        if (ssaoDirectLightingSlider != null)
            ssaoDirectLightingSlider.value = _ssaoDirectLighting;

        if (fogToggle != null)
            fogToggle.isOn = _fogEnabled;

        if (bloomToggle != null)
            bloomToggle.isOn = _bloomEnabled;
        if (bloomIntensitySlider != null)
            bloomIntensitySlider.value = _bloomIntensity;
        if (bloomThresholdSlider != null)
            bloomThresholdSlider.value = _bloomThreshold;
        if (bloomScatterSlider != null)
            bloomScatterSlider.value = _bloomScatter;

        if (dofToggle != null)
            dofToggle.isOn = _dofEnabled;

        if (grainToggle != null)
            grainToggle.isOn = _grainEnabled;
        if (grainIntensitySlider != null)
            grainIntensitySlider.value = _grainIntensity;
        if (grainResponseSlider != null)
            grainResponseSlider.value = _grainResponse;

        if (motionBlurToggle != null)
            motionBlurToggle.isOn = _motionBlurEnabled;
        if (motionBlurIntensitySlider != null)
            motionBlurIntensitySlider.value = _motionBlurIntensity;

        if (lensFlareToggle != null)
            lensFlareToggle.isOn = _lensFlareEnabled;
        if (lensFlareIntensitySlider != null)
            lensFlareIntensitySlider.value = _lensFlareIntensity;

        if (chromaticAberrationToggle != null)
            chromaticAberrationToggle.isOn = _chromaticAberrationEnabled;

        if (waterToggle != null)
            waterToggle.isOn = _waterEnabled;
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = _masterVolume;
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = _fullscreenEnabled;

        // Ray Tracing UI
        if (enableRayTracingButton != null)
            enableRayTracingButton.interactable = rayTracingSupported;
        if (enablePathTracingButton != null)
            enablePathTracingButton.interactable = pathTracingSupported;
        if (disableTracingButton != null)
            disableTracingButton.interactable = rayTracingSupported || pathTracingSupported;
        if (pathTracingToggle != null)
            pathTracingToggle.interactable = pathTracingSupported;
        if (recursiveRenderingToggle != null)
            recursiveRenderingToggle.interactable = rayTracingSupported;
        if (subsurfaceScatteringToggle != null)
            subsurfaceScatteringToggle.interactable = rayTracingSupported;
        if (rayBiasSlider != null)
            rayBiasSlider.interactable = rayTracingSupported;
        if (pathTracingSamplesSlider != null)
            pathTracingSamplesSlider.interactable = pathTracingSupported;
        if (pathTracingBouncesSlider != null)
            pathTracingBouncesSlider.interactable = pathTracingSupported;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}