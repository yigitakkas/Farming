using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectivesUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _objectiveListPanel;
    [SerializeField] private Transform _objectiveContainer;
    [SerializeField] private GameObject _objectiveEntryPrefab;
    
    private Dictionary<string, ObjectiveEntryUI> _objectiveEntries = new Dictionary<string, ObjectiveEntryUI>();
    
    private void Start()
    {
        // Subscribe to objective events
        ObjectiveManager.Instance.OnNewObjectiveAdded += AddObjectiveUI;
        ObjectiveManager.Instance.OnObjectiveCompleted += RemoveObjectiveUI;
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