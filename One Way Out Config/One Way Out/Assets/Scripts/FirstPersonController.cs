using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour 
{
    public static FirstPersonController Instance { get; private set; }

    [Header("Key Bindings")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float acceleration = 12f;
    public float deceleration = 15f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float doubleJumpForce = 12f;
    public float doubleJumpWindow = 0.1f;
    public float singleJumpDelay = 0.1f;
    public float gravity = -25f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.1f;

    [Header("Mouse Settings")]
    [Range(0.1f, 500f)] public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public float cameraSmoothTime = 0.1f;
    public float normalFOV = 60f;
    public float runFOV = 65f;
    public float fovChangeSpeed = 5f;
    public float cameraHeight = 1.6f;

    [Header("Camera Bobbing Settings")]
    public float walkBobbingSpeed = 8f;
    public float walkBobbingAmount = 0.05f;
    public float runBobbingSpeed = 12f;
    public float runBobbingAmount = 0.08f;
    public float horizontalBobbingAmount = 0.03f;

    [Header("Camera Tilt Settings")]
    public float tiltAmount = 3f;
    public float tiltSmoothTime = 0.2f;
    public bool tiltEnabled = true;

    [Header("Audio Settings")]
    public List<AudioClip> defaultFootstepSounds;
    public AudioClip jumpStartSound;
    public AudioClip landSound;
    public float footstepIntervalWalk = 0.5f;
    public float footstepIntervalRun = 0.3f;

    [Header("Footstep Sounds by Surface")]
    public List<FootstepSurface> footstepSurfaces = new List<FootstepSurface>();
    public LayerMask surfaceDetectionLayer = ~0;
    public float surfaceCheckDistance = 0.2f;

    [Header("Landing Sound Settings")]
    public float minLandingHeight = 2f;
    public float maxLandingHeight = 10f;
    public AnimationCurve landingVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Pause Settings")]
    public GameObject pauseMenu;
    public bool isGamePaused = false;

    [Header("Tutorial Settings")]
    public List<GameObject> tutorialPanels;
    public float tutorialDelay = 3f; 
    public bool forceTutorial = false;
    public bool showCursorDuringTutorial = true;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    [Header("State Objects")]
    public GameObject standingObject;
    public GameObject walkingObject;
    public GameObject runningObject;
    public GameObject airbornObject;

    [Header("Scare Triggers")]
    public List<ScareTrigger> scareTriggers = new List<ScareTrigger>();
    private const string ScareTriggerPrefix = "ScareTrigger_";

    [Header("Serious Scare Triggers")]
    public List<SeriousScareTrigger> seriousScareTriggers = new List<SeriousScareTrigger>();
    private const string SeriousScareTriggerPrefix = "SeriousScareTrigger_";

    [Header("Cutscene Triggers")]
    public List<CutsceneTrigger> cutsceneTriggers = new List<CutsceneTrigger>();

    [Header("Trigger Visibility Settings")]
    public List<TriggerVisibilityGroup> triggerVisibilityGroups;

    [Header("Save System Settings")]
    public List<Collider> saveTriggers = new List<Collider>();
    public string savePositionKey = "PlayerSavedPosition";
    public string saveRotationKey = "PlayerSavedRotation";

    [Header("Final Trigger Settings")]
    public Collider finalTrigger;
    public float finalSequenceDelay = 3f;
    public GameObject objectToActivateOnFinalTrigger;

    [Header("Final Sequence Events")]
    public UnityEngine.Events.UnityEvent onFinalSequenceComplete;

    [System.Serializable]
    public class FootstepSurface
    {
        public string surfaceName;
        public List<AudioClip> footstepSounds;
        public PhysicsMaterial matchingPhysicMaterial;
        public Texture matchingTexture;
    }

    [System.Serializable]
    public class ScareTrigger
    {
        public GameObject triggerObject;
        public AudioClip scareSound;
        public bool oneTimeOnly = true;
        [HideInInspector] public bool hasTriggered = false;
    }

    [System.Serializable]
    public class SeriousScareTrigger
    {
        public Collider triggerCollider;
        public Collider resetCollider;
        public GameObject monsterObject;
        public float delayBeforeMove = 0.5f;
        public Vector3 moveOffset = new Vector3(0, 0, 2f);
        public float moveSpeed = 2f;
        public bool oneTimeOnly = true;
        [HideInInspector] public bool hasTriggered = false;
        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public Coroutine currentCoroutine;
    }

    [System.Serializable]
    public class CutsceneTrigger
    {
        public Collider triggerCollider;
        public Transform playerTargetPosition;
        public float delayBeforeCutscene = 1f;
        public GameObject cutsceneObject;
        public AudioClip triggerSound;
        public AudioClip cutsceneStartSound;
        public AudioClip cutsceneEndSound;
        public float cutsceneDuration = 3f;
        [HideInInspector] public bool isPlaying;
    }

    [System.Serializable]
    public class TriggerVisibilityGroup
    {
        public List<Collider> triggerColliders;
        public GameObject targetObject;
        public float checkInterval = 1f;
        [HideInInspector] public float nextCheckTime;
        [HideInInspector] public bool isInside;
    }

    // Private variables
    private float _timer;
    private Vector3 _originalCameraPos;
    private bool _isMoving;
    private bool _isRunning;
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _cameraPitch;
    private Vector2 _currentMouseDelta;
    private Vector2 _currentMouseDeltaVelocity;
    private AudioSource _audioSource;
    private float _nextFootstepTime;
    private bool _isControlEnabled = true;
    private Camera _mainCamera;
    private float _currentSpeed;
    private bool _isFirstTime;
    private bool _inTutorial;
    private List<CanvasGroup> _tutorialCanvasGroups = new List<CanvasGroup>();
    private bool _isPlayerActive = true;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private float _tiltVelocity;
    private float _currentTilt;
    private bool _wasGrounded = true;
    private bool _awaitingSecondJump;
    private float _lastJumpPressTime;
    private bool _jumpPerformedThisFrame;
    private Coroutine _singleJumpCoroutine;
    private Coroutine _finalSequenceCoroutine;
    private float _fallStartHeight;
    private bool _isFalling;
    private Coroutine _currentCutsceneCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _controller = GetComponent<CharacterController>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.5f;
        _mainCamera = playerCamera.GetComponent<Camera>();
        
        playerCamera.localPosition = new Vector3(0, cameraHeight, 0);
        _originalCameraPos = playerCamera.localPosition;

        _isFirstTime = forceTutorial || PlayerPrefs.GetInt("TutorialCompleted", 0) == 0;
        PlayerPrefs.SetInt("TutorialCompleted", 1);

        LoadPlayerPosition();
    }

    void Start()
    {
        foreach (var trigger in seriousScareTriggers)
        {
            if (trigger.monsterObject != null)
            {
                trigger.originalPosition = trigger.monsterObject.transform.position;
            }
        }

        InitializeCursor();
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (_mainCamera != null) _mainCamera.fieldOfView = normalFOV;

        if (_isFirstTime && tutorialPanels != null && tutorialPanels.Count >= 3)
        {
            StartCoroutine(RunTutorial());
        }

        UpdateStateObjects();
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (var trigger in saveTriggers)
        {
            if (trigger != null && other == trigger)
            {
                SavePlayerPosition();
                break;
            }
        }

        foreach (var scareTrigger in scareTriggers)
        {
            if (scareTrigger.triggerObject != null && other.gameObject == scareTrigger.triggerObject)
            {
                string triggerKey = ScareTriggerPrefix + scareTrigger.triggerObject.name;
                
                if (!scareTrigger.oneTimeOnly || PlayerPrefs.GetInt(triggerKey, 0) == 0)
                {
                    if (scareTrigger.scareSound != null)
                    {
                        _audioSource.PlayOneShot(scareTrigger.scareSound);
                    }
                    
                    PlayerPrefs.SetInt(triggerKey, 1);
                    PlayerPrefs.Save();
                    scareTrigger.hasTriggered = true;
                }
                break;
            }
        }

        foreach (var seriousTrigger in seriousScareTriggers)
        {
            if (seriousTrigger.triggerCollider != null && 
                other == seriousTrigger.triggerCollider)
            {
                string triggerKey = SeriousScareTriggerPrefix + seriousTrigger.triggerCollider.name;
                
                if (!seriousTrigger.oneTimeOnly || PlayerPrefs.GetInt(triggerKey, 0) == 0)
                {
                    if (seriousTrigger.currentCoroutine != null)
                    {
                        StopCoroutine(seriousTrigger.currentCoroutine);
                        seriousTrigger.monsterObject.SetActive(false);
                        seriousTrigger.monsterObject.transform.position = seriousTrigger.originalPosition;
                    }
                    
                    seriousTrigger.currentCoroutine = StartCoroutine(ActivateSeriousScare(seriousTrigger));
                    
                    PlayerPrefs.SetInt(triggerKey, 1);
                    PlayerPrefs.Save();
                    seriousTrigger.hasTriggered = true;
                }
                break;
            }
            
            if (seriousTrigger.resetCollider != null && 
                other == seriousTrigger.resetCollider &&
                !seriousTrigger.oneTimeOnly)
            {
                if (seriousTrigger.currentCoroutine != null)
                {
                    StopCoroutine(seriousTrigger.currentCoroutine);
                    seriousTrigger.monsterObject.SetActive(false);
                    seriousTrigger.monsterObject.transform.position = seriousTrigger.originalPosition;
                    
                    string triggerKey = SeriousScareTriggerPrefix + seriousTrigger.triggerCollider.name;
                    PlayerPrefs.DeleteKey(triggerKey);
                    seriousTrigger.hasTriggered = false;
                }
            }
        }

        HandleCutsceneTriggers(other);

        if (finalTrigger != null && other == finalTrigger)
        {
            StartFinalSequence();
        }
    }

    private void HandleCutsceneTriggers(Collider other)
    {
        foreach (var cutsceneTrigger in cutsceneTriggers)
        {
            if (cutsceneTrigger.triggerCollider != null && 
                other == cutsceneTrigger.triggerCollider &&
                !cutsceneTrigger.isPlaying)
            {
                if (_currentCutsceneCoroutine != null)
                {
                    StopCoroutine(_currentCutsceneCoroutine);
                }
                _currentCutsceneCoroutine = StartCoroutine(PlayCutsceneSequence(cutsceneTrigger));
                break;
            }
        }
    }

    private IEnumerator PlayCutsceneSequence(CutsceneTrigger trigger)
    {
        trigger.isPlaying = true;
        DisablePlayerControl();

        if (trigger.triggerSound != null)
        {
            _audioSource.PlayOneShot(trigger.triggerSound);
        }
        
        yield return new WaitForSeconds(trigger.delayBeforeCutscene);
        
        if (trigger.cutsceneObject != null)
        {
            trigger.cutsceneObject.SetActive(true);
        }
        
        if (trigger.cutsceneStartSound != null)
        {
            _audioSource.PlayOneShot(trigger.cutsceneStartSound);
        }
        
        yield return new WaitForSeconds(trigger.cutsceneDuration);
        
        if (trigger.cutsceneObject != null)
        {
            trigger.cutsceneObject.SetActive(false);
        }
        
        if (trigger.playerTargetPosition != null)
        {
            _controller.enabled = false;
            transform.position = trigger.playerTargetPosition.position;
            transform.rotation = trigger.playerTargetPosition.rotation;
            _controller.enabled = true;
        }
        
        if (trigger.cutsceneEndSound != null)
        {
            _audioSource.PlayOneShot(trigger.cutsceneEndSound);
        }
        
        EnablePlayerControl();
        trigger.isPlaying = false;
        _currentCutsceneCoroutine = null;
    }

    public void StartFinalSequence()
    {
        if (_finalSequenceCoroutine != null)
        {
            StopCoroutine(_finalSequenceCoroutine);
        }
        _finalSequenceCoroutine = StartCoroutine(FinalSequenceRoutine());
    }

    public void SkipFinalSequence()
    {
        if (_finalSequenceCoroutine != null)
        {
            StopCoroutine(_finalSequenceCoroutine);
            _finalSequenceCoroutine = StartCoroutine(FinalSequenceRoutine(true));
        }
    }

private IEnumerator FinalSequenceRoutine(bool skipWait = false)
{
    DisablePlayerControl();
    
    if (objectToActivateOnFinalTrigger != null)
    {
        objectToActivateOnFinalTrigger.SetActive(true);
    }
    
    if (!skipWait) yield return new WaitForSeconds(finalSequenceDelay);
    
    // Вызываем пользовательские события
    if (onFinalSequenceComplete != null)
    {
        onFinalSequenceComplete.Invoke();
    }
    
    ResetPlayerSaves();
    
    GameLauncher gameLauncher = FindObjectOfType<GameLauncher>();
    if (gameLauncher != null)
    {
        gameLauncher.ResetAllProgress();
    }
    
    FinalDoor finalDoor = finalTrigger?.GetComponent<FinalDoor>();
    if (finalDoor != null)
    {
        finalDoor.ResetObjectSaves();
        yield return null;
    }
    
    RestartLevel();
    _finalSequenceCoroutine = null;
}

    IEnumerator ActivateSeriousScare(SeriousScareTrigger trigger)
    {
        if (trigger.monsterObject == null) yield break;
        
        trigger.monsterObject.SetActive(true);
        trigger.monsterObject.transform.position = trigger.originalPosition;
        
        yield return new WaitForSeconds(trigger.delayBeforeMove);
        
        Vector3 targetPosition = trigger.originalPosition + trigger.moveOffset;
        float journeyLength = Vector3.Distance(trigger.originalPosition, targetPosition);
        float startTime = Time.time;
        
        while (trigger.monsterObject.activeSelf && 
               Vector3.Distance(trigger.monsterObject.transform.position, targetPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * trigger.moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            trigger.monsterObject.transform.position = Vector3.Lerp(
                trigger.originalPosition, 
                targetPosition, 
                fractionOfJourney
            );
            yield return null;
        }
        
        trigger.monsterObject.SetActive(false);
        trigger.currentCoroutine = null;
    }

    void Update()
    {
        if (float.IsNaN(_cameraPitch)) _cameraPitch = 0;
        if (float.IsNaN(_currentTilt)) _currentTilt = 0;
        if (float.IsNaN(_timer)) _timer = 0;

        if (!gameObject.activeSelf || !_isPlayerActive) 
        {
            UpdateStateObjects();
            return;
        }

        if (_isControlEnabled && Input.GetKeyDown(pauseKey))
        {
            TogglePauseMenu();
        }

        if (isGamePaused) return;

        UpdateGroundedState();
        UpdateTriggerVisibilityGroups();
        
        if (_isControlEnabled)
        {
            HandleMovement();
            HandleCameraRotation();
            UpdateFOV();
            HandleCameraBobbingAndTilt();
            HandleJumpInput();
        }
        
        ApplyGravity();
        HandleFootsteps();
        
        UpdateStateObjects();
    }

    void SavePlayerPosition()
    {
        PlayerPrefs.SetFloat(savePositionKey + "_x", transform.position.x);
        PlayerPrefs.SetFloat(savePositionKey + "_y", transform.position.y);
        PlayerPrefs.SetFloat(savePositionKey + "_z", transform.position.z);
        
        PlayerPrefs.SetFloat(saveRotationKey + "_x", transform.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat(saveRotationKey + "_y", transform.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat(saveRotationKey + "_z", transform.rotation.eulerAngles.z);
        
        PlayerPrefs.Save();
    }

    void LoadPlayerPosition()
    {
        if (PlayerPrefs.HasKey(savePositionKey + "_x"))
        {
            float x = PlayerPrefs.GetFloat(savePositionKey + "_x");
            float y = PlayerPrefs.GetFloat(savePositionKey + "_y");
            float z = PlayerPrefs.GetFloat(savePositionKey + "_z");
            
            _controller.enabled = false;
            transform.position = new Vector3(x, y, z);
            _controller.enabled = true;

            float rx = PlayerPrefs.GetFloat(saveRotationKey + "_x");
            float ry = PlayerPrefs.GetFloat(saveRotationKey + "_y");
            float rz = PlayerPrefs.GetFloat(saveRotationKey + "_z");
            
            transform.rotation = Quaternion.Euler(rx, ry, rz);
        }
    }

    public void ResetPlayerSaves()
    {
        PlayerPrefs.DeleteKey(savePositionKey + "_x");
        PlayerPrefs.DeleteKey(savePositionKey + "_y");
        PlayerPrefs.DeleteKey(savePositionKey + "_z");
        
        PlayerPrefs.DeleteKey(saveRotationKey + "_x");
        PlayerPrefs.DeleteKey(saveRotationKey + "_y");
        PlayerPrefs.DeleteKey(saveRotationKey + "_z");
        
        foreach (var trigger in scareTriggers)
        {
            if (trigger.triggerObject != null)
            {
                string triggerKey = ScareTriggerPrefix + trigger.triggerObject.name;
                PlayerPrefs.DeleteKey(triggerKey);
            }
        }
        
        foreach (var trigger in seriousScareTriggers)
        {
            if (trigger.triggerCollider != null && trigger.oneTimeOnly)
            {
                string triggerKey = SeriousScareTriggerPrefix + trigger.triggerCollider.name;
                PlayerPrefs.DeleteKey(triggerKey);
                trigger.hasTriggered = false;
            }
        }
        
        PlayerPrefs.Save();
    }

    void UpdateTriggerVisibilityGroups()
    {
        if (triggerVisibilityGroups == null) return;

        foreach (var group in triggerVisibilityGroups)
        {
            if (group.triggerColliders == null || group.targetObject == null) 
                continue;

            if (Time.time >= group.nextCheckTime)
            {
                bool nowInside = false;
                foreach (var collider in group.triggerColliders)
                {
                    if (collider != null && collider.bounds.Contains(transform.position))
                    {
                        nowInside = true;
                        break;
                    }
                }

                if (nowInside != group.isInside)
                {
                    group.isInside = nowInside;
                    group.targetObject.SetActive(nowInside);
                }

                group.nextCheckTime = Time.time + group.checkInterval;
            }
        }
    }

    void UpdateStateObjects()
    {
        bool isAirborn = !_isGrounded;

        if (standingObject != null) 
            standingObject.SetActive(_isGrounded && !_isMoving);
        
        if (walkingObject != null) 
            walkingObject.SetActive(_isGrounded && _isMoving && !_isRunning);
        
        if (runningObject != null) 
            runningObject.SetActive(_isGrounded && _isRunning);

        if (airbornObject != null)
            airbornObject.SetActive(isAirborn);
    }

    void UpdateGroundedState()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _controller.isGrounded;

        if (_wasGrounded && !_isGrounded)
        {
            _fallStartHeight = transform.position.y;
            _isFalling = true;
        }

        if (!_wasGrounded && _isGrounded && _isFalling)
        {
            _isFalling = false;
            float fallHeight = _fallStartHeight - transform.position.y;

            if (fallHeight > minLandingHeight)
            {
                float normalizedHeight = Mathf.InverseLerp(minLandingHeight, maxLandingHeight, fallHeight);
                float volume = landingVolumeCurve.Evaluate(normalizedHeight);
                
                if (landSound != null && _audioSource != null)
                {
                    _audioSource.PlayOneShot(landSound, volume);
                }
            }
        }

        if (!_isGrounded)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            _coyoteTimeCounter = coyoteTime;
        }

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(forwardKey)) verticalInput += 1f;
        if (Input.GetKey(backwardKey)) verticalInput -= 1f;
        if (Input.GetKey(leftKey)) horizontalInput -= 1f;
        if (Input.GetKey(rightKey)) horizontalInput += 1f;

        Vector2 input = new Vector2(horizontalInput, verticalInput).normalized;

        _isMoving = input.magnitude > 0.1f;
        _isRunning = _isMoving && Input.GetKey(runKey);
        _currentSpeed = _isRunning ? runSpeed : walkSpeed;

        _coyoteTimeCounter = _isGrounded ? coyoteTime : _coyoteTimeCounter - Time.deltaTime;
        _jumpBufferCounter = Input.GetKeyDown(jumpKey) ? jumpBufferTime : _jumpBufferCounter - Time.deltaTime;

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        Vector3 targetVelocity = moveDirection * _currentSpeed;

        _velocity.x = Mathf.Lerp(
            _velocity.x,
            targetVelocity.x,
            (_isMoving ? acceleration : deceleration) * Time.deltaTime
        );
        
        _velocity.z = Mathf.Lerp(
            _velocity.z,
            targetVelocity.z,
            (_isMoving ? acceleration : deceleration) * Time.deltaTime
        );

        _controller.Move(_velocity * Time.deltaTime);
    }

    void HandleJumpInput()
    {
        _jumpPerformedThisFrame = false;

        if (Input.GetKeyDown(jumpKey))
        {
            if (Time.time - _lastJumpPressTime <= doubleJumpWindow && _isGrounded && !_awaitingSecondJump)
            {
                if (_singleJumpCoroutine != null)
                {
                    StopCoroutine(_singleJumpCoroutine);
                    _singleJumpCoroutine = null;
                }
                
                PerformJump(doubleJumpForce);
                _lastJumpPressTime = 0;
                _awaitingSecondJump = false;
                _jumpPerformedThisFrame = true;
            }
            else
            {
                _lastJumpPressTime = Time.time;
                _awaitingSecondJump = true;
                
                if (_singleJumpCoroutine != null)
                {
                    StopCoroutine(_singleJumpCoroutine);
                }
                _singleJumpCoroutine = StartCoroutine(SingleJumpCheck());
            }
        }

        if ((_jumpPerformedThisFrame || _jumpBufferCounter > 0) && _coyoteTimeCounter > 0)
        {
            PerformJump(jumpForce);
            _jumpBufferCounter = 0;
            _jumpPerformedThisFrame = true;
        }
    }

    IEnumerator SingleJumpCheck()
    {
        yield return new WaitForSeconds(singleJumpDelay);
        
        if (_awaitingSecondJump && _isGrounded)
        {
            PerformJump(jumpForce);
            _awaitingSecondJump = false;
        }
    }

    void PerformJump(float force)
    {
        _velocity.y = Mathf.Sqrt(force * -2f * gravity);
        PlaySound(jumpStartSound);
    }

    void HandleCameraRotation()
    {
        Vector2 mouseInput = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        _currentMouseDelta = Vector2.SmoothDamp(
            _currentMouseDelta,
            mouseInput,
            ref _currentMouseDeltaVelocity,
            cameraSmoothTime
        );

        float sensitivityFactor = mouseSensitivity * 0.01f;
        transform.Rotate(Vector3.up * _currentMouseDelta.x * sensitivityFactor);

        _cameraPitch -= _currentMouseDelta.y * sensitivityFactor;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
    }

    void HandleCameraBobbingAndTilt()
    {
        if (float.IsNaN(_timer)) _timer = 0;
        if (float.IsNaN(_currentTilt)) _currentTilt = 0;
        if (float.IsNaN(_cameraPitch)) _cameraPitch = 0;

        Vector3 bobbingOffset = Vector3.zero;
        if (_isGrounded && _isMoving)
        {
            float bobbingSpeed = _isRunning ? runBobbingSpeed : walkBobbingSpeed;
            float bobbingAmount = _isRunning ? runBobbingAmount : walkBobbingAmount;
            
            if (float.IsNaN(bobbingSpeed)) bobbingSpeed = 8f;
            if (float.IsNaN(bobbingAmount)) bobbingAmount = 0.05f;
            
            _timer += Time.deltaTime * bobbingSpeed;
            float sinValue = Mathf.Sin(_timer * 1.3f);
            float cosValue = Mathf.Cos(_timer * 0.8f);
            
            if (!float.IsNaN(sinValue)) bobbingOffset.y = sinValue * bobbingAmount;
            if (!float.IsNaN(cosValue)) bobbingOffset.x = cosValue * horizontalBobbingAmount;
        }
        else
        {
            _timer = 0;
            bobbingOffset = Vector3.Lerp(playerCamera.localPosition - _originalCameraPos, Vector3.zero, Time.deltaTime * 5f);
        }

        float targetTilt = tiltEnabled ? -_currentMouseDelta.x * tiltAmount : 0f;
        if (float.IsNaN(targetTilt)) targetTilt = 0;
        
        _currentTilt = Mathf.SmoothDamp(_currentTilt, targetTilt, ref _tiltVelocity, tiltSmoothTime);
        if (float.IsNaN(_currentTilt)) _currentTilt = 0;

        Quaternion tiltRotation = Quaternion.Euler(0, 0, _currentTilt);
        Quaternion pitchRotation = Quaternion.Euler(_cameraPitch, 0, 0);
        
        playerCamera.localRotation = pitchRotation * tiltRotation;
        
        if (!float.IsNaN(bobbingOffset.x) && !float.IsNaN(bobbingOffset.y))
        {
            playerCamera.localPosition = _originalCameraPos + bobbingOffset;
        }
    }

    void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _velocity.y += gravity * Time.deltaTime;
        }
    }

    void UpdateFOV()
    {
        if (_mainCamera == null) return;

        float targetFOV = _isRunning ? runFOV : normalFOV;
        _mainCamera.fieldOfView = Mathf.Lerp(
            _mainCamera.fieldOfView,
            targetFOV,
            fovChangeSpeed * Time.deltaTime
        );
    }

    FootstepSurface GetCurrentSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, surfaceCheckDistance, surfaceDetectionLayer))
        {
            if (hit.collider.sharedMaterial != null)
            {
                foreach (var surface in footstepSurfaces)
                {
                    if (surface.matchingPhysicMaterial == hit.collider.sharedMaterial)
                    {
                        return surface;
                    }
                }
            }

            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
            {
                Texture currentTexture = renderer.sharedMaterial.mainTexture;
                foreach (var surface in footstepSurfaces)
                {
                    if (surface.matchingTexture == currentTexture)
                    {
                        return surface;
                    }
                }
            }
        }

        return null;
    }

    void HandleFootsteps()
    {
        if (!_isGrounded || !_isMoving || _audioSource == null) return;

        FootstepSurface currentSurface = GetCurrentSurface();
        List<AudioClip> availableSounds = currentSurface != null ? 
            currentSurface.footstepSounds : defaultFootstepSounds;

        if (availableSounds == null || availableSounds.Count == 0) return;

        float interval = _isRunning ? footstepIntervalRun : footstepIntervalWalk;

        if (Time.time >= _nextFootstepTime)
        {
            AudioClip clip = availableSounds[Random.Range(0, availableSounds.Count)];
            _audioSource.PlayOneShot(clip);
            _nextFootstepTime = Time.time + interval;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && _audioSource != null) _audioSource.PlayOneShot(clip);
    }

    void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateCursorState()
    {
        bool shouldLockCursor = !isGamePaused && _isControlEnabled && !_inTutorial;
        Cursor.lockState = shouldLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !shouldLockCursor || (_inTutorial && showCursorDuringTutorial);
    }

    public void TogglePauseMenu()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isGamePaused);
        }

        UpdateCursorState();

        if (_audioSource != null)
        {
            if (isGamePaused) _audioSource.Pause();
            else _audioSource.UnPause();
        }
    }

    public void ClosePauseMenu()
    {
        if (!isGamePaused) return;

        isGamePaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        UpdateCursorState();
        
        if (_audioSource != null)
        {
            _audioSource.UnPause();
        }
    }

    IEnumerator RunTutorial()
    {
        _inTutorial = true;
        UpdateCursorState();
        
        yield return new WaitForSeconds(tutorialDelay);
        
        SetTutorialControl(false);
        yield return StartCoroutine(ShowTutorialPanel(0, 
            () => Input.GetKeyDown(pauseKey) || Input.GetKeyDown(KeyCode.F)));
        SetTutorialControl(true);

        yield return new WaitForSeconds(tutorialDelay);
        
        SetTutorialControl(false);
        yield return StartCoroutine(ShowTutorialPanel(1, 
            () => Input.GetKeyDown(pauseKey) || Input.GetMouseButtonDown(0)));
        SetTutorialControl(true);

        yield return new WaitForSeconds(tutorialDelay);
        
        SetTutorialControl(false);
        yield return StartCoroutine(ShowTutorialPanel(2, 
            () => Input.GetKeyDown(pauseKey) || Input.GetMouseButtonDown(1)));
        SetTutorialControl(true);

        _inTutorial = false;
        UpdateCursorState();
    }

    IEnumerator ShowTutorialPanel(int panelIndex, System.Func<bool> skipCondition)
    {
        if (panelIndex >= tutorialPanels.Count || tutorialPanels[panelIndex] == null)
            yield break;

        GameObject panel = tutorialPanels[panelIndex];
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();
        
        panel.SetActive(true);
        canvasGroup.alpha = 0f;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer/fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        float waitTimer = 10f;
        while (waitTimer > 0 && !skipCondition())
        {
            waitTimer -= Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer/fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }

    void SetTutorialControl(bool enable)
    {
        _isControlEnabled = enable;
        UpdateCursorState();
    }

    public void EnablePlayerControl()
    {
        _isControlEnabled = true;
        _isPlayerActive = true;
        UpdateCursorState();
    }

    public void DisablePlayerControl()
    {
        _isControlEnabled = false;
        UpdateCursorState();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}