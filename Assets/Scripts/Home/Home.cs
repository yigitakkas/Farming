using UnityEngine;

public class Home : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private GameObject _sleepPromptUI;
    [SerializeField] private float _sleepDuration = 6f;
    [SerializeField] private float _timeAcceleration = 1000f; // How much faster time passes while sleeping
    
    private bool _canInteract;
    private bool _isSleeping;
    private float _sleepStartHour;
    private float _originalTimeScale;
    private PlayerInteraction _playerInteraction;

    private void Start()
    {
        if (_sleepPromptUI != null)
        {
            _sleepPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Keep prompt facing camera and check for interaction
        if (_canInteract && _sleepPromptUI != null && _sleepPromptUI.activeSelf)
        {
            _sleepPromptUI.transform.rotation = Camera.main.transform.rotation;
            
            if (Input.GetKeyDown(KeyCode.E) && !_isSleeping)
            {
                StartSleep();
            }
        }

        // Check if sleep duration has passed
        if (_isSleeping)
        {
            float currentHour = TimeManager.Instance.CurrentHour;
            float hoursPassed = currentHour - _sleepStartHour;
            if (hoursPassed < 0) // If we crossed midnight
            {
                hoursPassed += 24f;
            }

            if (hoursPassed >= _sleepDuration)
            {
                EndSleep();
            }
        }
    }

    private void StartSleep()
    {
        _isSleeping = true;
        _sleepStartHour = TimeManager.Instance.CurrentHour;
        _originalTimeScale = TimeManager.Instance.RealMinutesPerDay;
        TimeManager.Instance.RealMinutesPerDay /= _timeAcceleration;
    }

    private void EndSleep()
    {
        _isSleeping = false;
        TimeManager.Instance.RealMinutesPerDay = _originalTimeScale;
    }

    public void Interact(PlayerInteraction player)
    {
        if (_canInteract && !_isSleeping)
        {
            StartSleep();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = true;
            _playerInteraction = other.GetComponent<PlayerInteraction>();
            if (_sleepPromptUI != null)
            {
                _sleepPromptUI.SetActive(true);
                _sleepPromptUI.transform.position = transform.position + Vector3.up * 2f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = false;
            _playerInteraction = null;
            if (_sleepPromptUI != null)
            {
                _sleepPromptUI.SetActive(false);
            }
        }
    }
} 