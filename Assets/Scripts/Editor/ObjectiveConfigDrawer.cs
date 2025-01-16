#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObjectiveConfig))]
public class ObjectiveConfigDrawer : PropertyDrawer
{
    private enum ObjectiveType
    {
        Money,
        Crop
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Draw the type selector
        var typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var objectiveType = (ObjectiveType)EditorGUI.EnumPopup(typeRect, "Objective Type", ObjectiveType.Money);
        
        // Draw the appropriate fields based on type
        var yOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        var idRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(idRect, property.FindPropertyRelative("Id"));
        
        yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        var titleRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("Title"));
        
        yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        var rewardRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rewardRect, property.FindPropertyRelative("Reward"));
        
        if (objectiveType == ObjectiveType.Money)
        {
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var targetMoneyRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(targetMoneyRect, property.FindPropertyRelative("TargetMoney"));
        }
        else
        {
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var targetCountRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(targetCountRect, property.FindPropertyRelative("TargetCount"));
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
    }
}
#endif 