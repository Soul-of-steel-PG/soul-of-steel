using UnityEditor;
using UnityEngine;

public class CardTypeDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Get the enum value as an integer
        int enumValueIndex = property.enumValueIndex;

        // Get the enum names as a string array
        string[] enumNames = property.enumNames;

        // Display the dropdown with the enum names
        enumValueIndex = EditorGUI.Popup(position, label.text, enumValueIndex, enumNames);

        // Set the enum value back to the property
        property.enumValueIndex = enumValueIndex;
    }
}