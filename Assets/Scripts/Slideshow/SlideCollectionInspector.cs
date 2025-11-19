using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlideCollection))]
public class SlideCollectionInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SlideCollection slideCollection = (SlideCollection)target;

        if (GUILayout.Button("Add text slide"))
        {
            slideCollection.AddTextSlide();
        }

        if (GUILayout.Button("Add image slide"))
        {
            slideCollection.AddImageSlide();
        }
    }
}
