using UnityEngine;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [System.Serializable]
    public class TutorialStep
    {
        public string Message;
        public RectTransform Position;
        public float DisplayTime = 5f;
    }

    [Header("Tutorial Settings")]
    [SerializeField] private GameObject _tooltipPrefab;
    [SerializeField] private TutorialStep[] _tutorialSteps;
    [SerializeField] private Canvas _canvas;
    
    [Header("Player Prefs Key")]
    private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";

    private GameObject _currentTooltip;
    private int _currentStepIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(TUTORIAL_COMPLETED_KEY))
        {
            StartCoroutine(ShowTutorialSequence());
        }
    }

    private IEnumerator ShowTutorialSequence()
    {
        foreach (var step in _tutorialSteps)
        {
            yield return StartCoroutine(ShowTutorialStep(step));
        }

        // Clean up the last tooltip before completing
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
            _currentTooltip = null;
        }

        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        PlayerPrefs.Save();
    }

    private IEnumerator ShowTutorialStep(TutorialStep step)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
        }

        _currentTooltip = Instantiate(_tooltipPrefab, step.Position.position, Quaternion.identity, _canvas.transform);
        RectTransform tooltipRect = _currentTooltip.GetComponent<RectTransform>();
        if (tooltipRect != null)
        {
            tooltipRect.position = step.Position.position;
        }

        TMP_Text tooltipText = _currentTooltip.GetComponentInChildren<TMP_Text>();
        if (tooltipText != null)
        {
            tooltipText.text = step.Message;
        }

        yield return new WaitForSeconds(step.DisplayTime);
    }

    public void SkipCurrentTooltip()
    {
        if (_currentTooltip != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTutorialSequence());
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TUTORIAL_COMPLETED_KEY);
        _currentStepIndex = 0;
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
        }
    }
}