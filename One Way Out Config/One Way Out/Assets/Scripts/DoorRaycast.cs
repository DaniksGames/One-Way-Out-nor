using UnityEngine;

public class DoorRaycast : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask doorLayer;

    [Header("UI")]
    [SerializeField] private GameObject interactHint;

    private Camera _mainCamera;
    private Door _lastSeenDoor;
    private bool _hasInteractHint;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _hasInteractHint = interactHint != null;
        
        if (_hasInteractHint)
        {
            interactHint.SetActive(false);
        }
    }

    private void Update()
    {
        HandleDoorDetection();
        HandleDoorInteraction();
    }

    private void HandleDoorDetection()
    {
        if (!Physics.Raycast(
            _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)),
            out RaycastHit hit,
            maxDistance,
            doorLayer,
            QueryTriggerInteraction.Ignore))
        {
            ClearSelection();
            return;
        }

        if (!hit.collider.TryGetComponent(out Door door))
        {
            ClearSelection();
            return;
        }

        if (_lastSeenDoor != door)
        {
            _lastSeenDoor = door;
            ShowHint(true);
        }
    }

    private void HandleDoorInteraction()
    {
        if (Input.GetMouseButtonDown(0) && _lastSeenDoor != null)
        {
            _lastSeenDoor.Toggle();
        }
    }

    private void ClearSelection()
    {
        if (_lastSeenDoor == null) return;
        
        ShowHint(false);
        _lastSeenDoor = null;
    }

    private void ShowHint(bool state)
    {
        if (_hasInteractHint)
        {
            interactHint.SetActive(state);
        }
    }
}