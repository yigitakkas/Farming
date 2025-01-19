using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class ObjectivesUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _objectiveListPanel;
    [SerializeField] private Transform _objectiveContainer;
    [SerializeField] private GameObject _objectiveEntryPrefab;
    
    private Dictionary<string, ObjectiveEntryUI> _objectiveEntries = new Dictionary<string, ObjectiveEntryUI>();
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) // Game scene
        {
            LoadExistingObjectives();
        }
    }

    private void Start()
    {
        // Subscribe to objective events
        ObjectiveManager.Instance.OnNewObjectiveAdded += AddObjectiveUI;
        ObjectiveManager.Instance.OnObjectiveCompleted += RemoveObjectiveUI;
        
        // Load any existing objectives
        LoadExistingObjectives();
    }

    private void LoadExistingObjectives()
    {
        // Clear existing UI first
        foreach (var entry in _objectiveEntries.Values)
        {
            if (entry != null && entry.gameObject != null)
            {
                Destroy(entry.gameObject);
            }
        }
        _objectiveEntries.Clear();

        // Add UI for all current objectives
        foreach (var objective in ObjectiveManager.Instance.CurrentObjectives)
        {
            AddObjectiveUI(objective);
        }
    }
    
    private void AddObjectiveUI(Objective objective)
    {
        GameObject entryObj = Instantiate(_objectiveEntryPrefab, _objectiveContainer);
        ObjectiveEntryUI entryUI = entryObj.GetComponent<ObjectiveEntryUI>();
        
        if (entryUI != null)
        {
            entryUI.SetupObjective(objective);
            _objectiveEntries[objective.Id] = entryUI;
        }
    }
    
    private void RemoveObjectiveUI(Objective objective)
    {
        if (_objectiveEntries.TryGetValue(objective.Id, out ObjectiveEntryUI entryUI))
        {
            StartCoroutine(HandleObjectiveCompletion(objective.Id, entryUI));
        }
    }
    
    private IEnumerator HandleObjectiveCompletion(string objectiveId, ObjectiveEntryUI entryUI)
    {
        entryUI.ShowCompleted();
        yield return new WaitForSeconds(2f);
        
        if (_objectiveEntries.ContainsKey(objectiveId))
        {
            Destroy(entryUI.gameObject);
            _objectiveEntries.Remove(objectiveId);
        }
    }
    
    private void OnDestroy()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnNewObjectiveAdded -= AddObjectiveUI;
            ObjectiveManager.Instance.OnObjectiveCompleted -= RemoveObjectiveUI;
        }
    }
} 