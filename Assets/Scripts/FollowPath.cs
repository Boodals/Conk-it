using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour
{
	public enum RepeatMode
	{
		None,
		Repeat,
		PingPong,
	}

	public Path path;

	public bool lockRotation = false;

	public float speed = 3f;


	public float distOnPath = 0f;

	public RepeatMode repeatMode = RepeatMode.None;

	private bool pingPongDirInv = false;


	void OnValidate()
	{
		UpdateObject();

		if(path != null)
		{
			distOnPath = Mathf.Clamp(distOnPath, 0f, path.length);
		}
	}

	void LateUpdate()
	{
		distOnPath += speed * Time.deltaTime;
		switch(repeatMode)
		{
			case RepeatMode.None:
				distOnPath = Mathf.Clamp(distOnPath, 0f, path.length);
				UpdateObject();
				break;
			case RepeatMode.Repeat:
				distOnPath = Mathf.Repeat(distOnPath, path.length);
				UpdateObject();
				break;
			case RepeatMode.PingPong:
				if(!pingPongDirInv)
				{
					distOnPath = Mathf.Repeat(distOnPath, path.length * 2f);
					UpdateObject(Mathf.PingPong(distOnPath, path.length));
				}
				else
				{
					distOnPath = path.length - Mathf.Clamp(path.length - distOnPath, 0f, path.length);
				}
				break;
		}
	}

	public void UpdateObject()
	{
		UpdateObject(distOnPath);
	}
	public void UpdateObject(float dist)
	{
		if(path != null)
		{
			transform.position = path.GetPos(dist);

			if(lockRotation)
			{
				transform.rotation = Quaternion.LookRotation(path.GetNormal(distOnPath));
			}
		}
	}
}
