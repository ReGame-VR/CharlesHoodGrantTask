using UnityEditor;

namespace ManusVR.PhysicalInteraction
{
    [CustomEditor(typeof(Interactable), true), CanEditMultipleObjects]
    public class InteractableEditor : Editor
    {
        private SerializedProperty highlightOnImpact;
        private SerializedProperty isGrabbable;
        private SerializedProperty highlightWhenGrabbed;
        private SerializedProperty dropDistance;
        private SerializedProperty releaseWithPhalanges;
        private SerializedProperty gravityWhenGrabbed;
        private SerializedProperty gravityWhenReleased;
        private SerializedProperty kinematicWhenReleased;

        void OnEnable()
        {
            highlightOnImpact = serializedObject.FindProperty("HighlightOnImpact");
            isGrabbable = serializedObject.FindProperty("IsGrabbable");
            highlightWhenGrabbed = serializedObject.FindProperty("HighlightWhenGrabbed");
            dropDistance = serializedObject.FindProperty("DropDistance");
            releaseWithPhalanges = serializedObject.FindProperty("ReleaseWithPhalanges");
            gravityWhenGrabbed = serializedObject.FindProperty("GravityWhenGrabbed");
            gravityWhenReleased = serializedObject.FindProperty("GravityWhenReleased");
            kinematicWhenReleased = serializedObject.FindProperty("KinematicWhenReleased");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            EditorGUILayout.PropertyField(highlightOnImpact);
            EditorGUILayout.LabelField("Grab Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isGrabbable);
            if (isGrabbable.boolValue)
            {
                EditorGUILayout.PropertyField(highlightWhenGrabbed);
                EditorGUILayout.PropertyField(dropDistance);
                EditorGUILayout.PropertyField(releaseWithPhalanges);
                EditorGUILayout.PropertyField(gravityWhenGrabbed);
                EditorGUILayout.PropertyField(gravityWhenReleased);
                EditorGUILayout.PropertyField(kinematicWhenReleased);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
