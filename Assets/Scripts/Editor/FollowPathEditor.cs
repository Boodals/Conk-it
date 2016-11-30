using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(FollowPath), true)]
public class FollowPathEditor : Editor
{

	private FollowPath followPath;

	private bool isPreviewing = false;

	private double previewStartTime;

	//Holds the original progress while previewing
	private float prevDistOnPath;

	void OnEnable()
	{
		followPath = (FollowPath)target;
	}

	void OnDisable()
	{
		if(isPreviewing)
		{
			StopPreview();
		}
	}

	public override void OnInspectorGUI()
	{
		if(isPreviewing)
		{
			GUI.enabled = false;
		}
		DrawDefaultInspector();
		GUI.enabled = true;

		if(!isPreviewing)
		{
			if(GUILayout.Button("Preview"))
			{
				StartPreview();
			}
		}
		else
		{
			if(GUILayout.Button("Stop"))
			{
				StopPreview();
			}
		}
	}

	private void StartPreview()
	{
		isPreviewing = true;
		previewStartTime = EditorApplication.timeSinceStartup;

		prevDistOnPath = followPath.distOnPath;
	}

	private void StopPreview()
	{
		isPreviewing = false;

		followPath.distOnPath = prevDistOnPath;
		followPath.UpdateObject();
	}

	void OnSceneGUI()
	{
		if(isPreviewing)
		{
			followPath.distOnPath = (float)(followPath.speed * (EditorApplication.timeSinceStartup - previewStartTime));
			followPath.UpdateObject();

			if(followPath.distOnPath >= followPath.path.length)
			{
				StopPreview();
			}
		}
	}
}
