#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FollowUpObjective))]
public class FollowUpObjectiveDrawer : PropertyDrawer
{
    private enum ObjectiveType
    {
        Money,
        Crop
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var triggerIdRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(triggerIdRect, property.FindPropertyRelative("TriggerObjectiveId"), new GUIContent("Trigger Objective ID"));

        // Type selector
        var typeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 
            position.width, EditorGUIUtility.singleLineHeight);
            
        var newObjectiveProp = property.FindPropertyRelative("NewObjective");
        bool isMoney = newObjectiveProp.managedReferenceValue is MoneyObjectiveConfig;
        ObjectiveType currentType = isMoney ? ObjectiveType.Money : ObjectiveType.Crop;
        
        EditorGUI.BeginChangeCheck();
        var selectedType = (ObjectiveType)EditorGUI.EnumPopup(typeRect, "New Objective Type", currentType);
        if (EditorGUI.EndChangeCheck())
        {
            // Create new objective config based on selected type
            if (selectedType == ObjectiveType.Money)
            {
                newObjectiveProp.managedReferenceValue = new MoneyObjectiveConfig();
            }
            else
            {
                newObjectiveProp.managedReferenceValue = new CropObjectiveConfig();
            }
        }

        if (newObjectiveProp.managedReferenceValue != null)
        {
            // Basic fields
            float yOffset = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
            
            var idRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(idRect, newObjectiveProp.FindPropertyRelative("Id"));
            
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var titleRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(titleRect, newObjectiveProp.FindPropertyRelative("Title"));
            
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var descRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.PropertyField(descRect, newObjectiveProp.FindPropertyRelative("Description"));
            
            yOffset += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            var rewardRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rewardRect, newObjectiveProp.FindPropertyRelative("Reward"));

            // Type-specific fields
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var targetRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            
            if (selectedType == ObjectiveType.Money)
            {
                EditorGUI.PropertyField(targetRect, newObjectiveProp.FindPropertyRelative("TargetMoney"));
            }
            else
            {
                EditorGUI.PropertyField(targetRect, newObjectiveProp.FindPropertyRelative("TargetCount"));
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 8 + EditorGUIUtility.standardVerticalSpacing * 7;
    }
}
#endif 