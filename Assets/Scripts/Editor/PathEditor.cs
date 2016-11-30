using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Path), true)]
public class PathEditor : Editor
{

	[MenuItem("GameObject/New Path", false, 1)]
	public static void MakeNewPathGameObject()
	{
		GameObject GO = new GameObject("Path");
		GO.AddComponent<Path>();
	}

	private Path path;

	private SerializedProperty nodesArrayProp;

	void OnEnable()
	{
		path = (Path)target;

		nodesArrayProp = serializedObject.FindProperty("nodes");

		if(nodesArrayProp.arraySize < 4)
		{
			nodesArrayProp.arraySize = 4;

			nodesArrayProp.GetArrayElementAtIndex(0).FindPropertyRelative("position").vector3Value = new Vector3(0f, 1f, 0f);
			nodesArrayProp.GetArrayElementAtIndex(1).FindPropertyRelative("position").vector3Value = new Vector3(1f, 2f, 0f);
			nodesArrayProp.GetArrayElementAtIndex(2).FindPropertyRelative("position").vector3Value = new Vector3(-1f, 4f, 0f);
			nodesArrayProp.GetArrayElementAtIndex(3).FindPropertyRelative("position").vector3Value = new Vector3(0f, 5f, 0f);

			serializedObject.ApplyModifiedProperties();
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUI.enabled = false;
		EditorGUILayout.IntField("Number of segments", (nodesArrayProp.arraySize - 1) / 3);
		GUI.enabled = true;

		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();

		if(GUILayout.Button("Add"))
		{
			nodesArrayProp.arraySize += 3;

			//Position of the end point of the path
			Vector3 end = nodesArrayProp.GetArrayElementAtIndex(nodesArrayProp.arraySize - 4).FindPropertyRelative("position").vector3Value;

			Vector3 endForward = path.GetNormal(path.length);

			nodesArrayProp.GetArrayElementAtIndex(nodesArrayProp.arraySize - 3).FindPropertyRelative("position").vector3Value = end + endForward * 1f;
			nodesArrayProp.GetArrayElementAtIndex(nodesArrayProp.arraySize - 2).FindPropertyRelative("position").vector3Value = end + endForward * 3f;
			nodesArrayProp.GetArrayElementAtIndex(nodesArrayProp.arraySize - 1).FindPropertyRelative("position").vector3Value = end + endForward * 4f;
		}
		if(nodesArrayProp.arraySize <= 4)
		{
			GUI.enabled = false;
		}
		if(GUILayout.Button("Remove"))
		{
			nodesArrayProp.arraySize -= 3;
		}
		GUI.enabled = true;

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		foreach(SerializedProperty nodeProp in nodesArrayProp)
		{
			SerializedProperty posProp = nodeProp.FindPropertyRelative("position");

			Vector3 worldPos = path.transform.TransformPoint(posProp.vector3Value);
			worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
			posProp.vector3Value = path.transform.InverseTransformPoint(worldPos);
		}

		serializedObject.ApplyModifiedProperties();
	}

}
