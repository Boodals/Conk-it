using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EnvDisappear))]
public class EnvDisappearEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EnvDisappear dis = (EnvDisappear)target;

		GUI.enabled = false;
		EditorGUILayout.Toggle("Is in cycle:", dis.isInCycle);
		GUI.enabled = true;

		if(GUILayout.Button("Start Cycle"))
		{
			dis.StartCycle();
		}
		if(GUILayout.Button("Pause Cycle"))
		{
			dis.PauseCycle();
		}
		if(GUILayout.Button("Unpause Cycle"))
		{
			dis.UnpauseCycle();
		}
		if(GUILayout.Button("Stop Cycle"))
		{
			dis.StopCycle();
		}
		if(GUILayout.Button("Reset Cycle"))
		{
			dis.ResetCycle();
		}

		if(GUILayout.Button("Activate"))
		{
			dis.Activate();
		}
		if(GUILayout.Button("Deactivate"))
		{
			dis.Deactivate();
		}
		if(GUILayout.Button("Transition"))
		{
			dis.Transition();
		}
	}
}
