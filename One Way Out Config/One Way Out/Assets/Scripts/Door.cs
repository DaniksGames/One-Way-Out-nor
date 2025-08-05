using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private bool opensInward = false;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
    [Header("Object to Activate")]
    [SerializeField] private GameObject objectToActivate;

    private Quaternion _initialRotation;
    private Quaternion _targetRotation;
    private AudioSource _audioSource;
    private bool _isOpen;
    private bool _canInteract = true;
    private float _direction;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _initialRotation = transform.rotation;
        _direction = opensInward ? -1f : 1f;
        
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    private void Update()
    {
        if (Quaternion.Angle(transform.rotation, _targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                _targetRotation, 
                smoothSpeed * Time.deltaTime
            );
        }
    }

    public void Toggle()
    {
        if (!_canInteract) return;
        
        _isOpen = !_isOpen;
        UpdateDoorState();
    }

    public void Open()
    {
        if (!_canInteract || _isOpen) return;
        _isOpen = true;
        UpdateDoorState();
    }

    public void Close()
    {
        if (!_canInteract || !_isOpen) return;
        _isOpen = false;
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        _targetRotation = _isOpen 
            ? Quaternion.Euler(0, openAngle * _direction, 0) * _initialRotation 
            : _initialRotation;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(_isOpen);
        }

        if (_audioSource != null && _audioSource.enabled)
        {
            _audioSource.PlayOneShot(_isOpen ? openSound : closeSound);
        }

        StartCoroutine(InteractionCooldown());
    }

    private System.Collections.IEnumerator InteractionCooldown()
    {
        _canInteract = false;
        yield return new WaitForSeconds(0.5f);
        _canInteract = true;
    }
}