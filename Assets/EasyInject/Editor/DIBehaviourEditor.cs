using UnityEditor;
using UnityEngine;

namespace EasyInject.Editor
{
    [CustomEditor(typeof(DIBehaviour), true)]
    public class DIBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Resolve Dependencies"))
            {
                foreach (var target in targets)
                {
                    var diBehaviour = (DIBehaviour)target;
                    DependencyResolver.ResolveDependencies(diBehaviour);
                    EditorUtility.SetDirty(diBehaviour);
                }
            }
        }
    }
}