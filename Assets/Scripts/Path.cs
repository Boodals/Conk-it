using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
	[System.Serializable]
	private class PathNode
	{
		[SerializeField]
		public Vector3 position;

		public float DistTo(PathNode otherNode)
		{
			return Vector3.Distance(position, otherNode.position);
		}
		public float DistTo(Vector3 pos)
		{
			return Vector3.Distance(position, pos);
		}
	}

	private struct PathSegment
	{
		public PathNode[] nodes;

		private float _length;

		public float length
		{
			get
			{
				return _length;
			}
		}

		public PathSegment(PathNode start, PathNode startLine, PathNode endLine, PathNode end)
		{
			nodes = new PathNode[4];

			nodes[0] = start;
			nodes[1] = startLine;
			nodes[2] = endLine;
			nodes[3] = end;

			_length = start.DistTo(end); //TODO: make this actual distance along curve, somehow.
		}


		public Vector3 GetPos(float dist)
		{
			float t = dist / _length;
			float oneMinusT = 1f - t;

			Vector3 a = nodes[0].position;
			Vector3 b = nodes[1].position;
			Vector3 c = nodes[2].position;
			Vector3 d = nodes[3].position;

			return
				oneMinusT * oneMinusT * oneMinusT * a +
				3f * oneMinusT * oneMinusT * t * b +
				3f * oneMinusT * t * t * c +
				t * t * t * d;


			/*  This is the expanded version of the equation above 
			Vector3 ab = Vector3.Lerp(a, b, t);
			Vector3 bc = Vector3.Lerp(b, c, t);
			Vector3 cd = Vector3.Lerp(c, d, t);

			Vector3 abc = Vector3.Lerp(ab, bc, t);
			Vector3 bcd = Vector3.Lerp(bc, cd, t);

			return Vector3.Lerp(abc, bcd, t);
			*/
		}

		public Vector3 GetVelocity(float dist)
		{
			float t = dist / _length;
			float oneMinusT = 1f - t;

			Vector3 a = nodes[0].position;
			Vector3 b = nodes[1].position;
			Vector3 c = nodes[2].position;
			Vector3 d = nodes[3].position;

			return
				3f * oneMinusT * oneMinusT * (b - a) +
				8f * oneMinusT * t * (c - b) +
				3f * t * t * (d - c);
		}
	}

	[SerializeField]
	private List<PathNode> nodes = new List<PathNode>(4);

	private List<PathSegment> segments = new List<PathSegment>();

	private bool isValid = false;
	private float totalDistance = 0f;

	public float length
	{
		get
		{
			CheckValid();
			return totalDistance;
		}
	}

	void OnValidate()
	{
		//Make sure we always have atleast 4 nodes (one segment)
		//while(nodes.Count < 4)
		//{
		//	nodes.Add(new PathNode());
		//}

		//Make sure to recalculate metadata when variables are changed in the inspector
		Validate();
	}

	void Awake()
	{
		Validate();
	}

	public Vector3 GetPos(float dist)
	{
		CheckValid();

		if(dist <= 0f)
		{
			if(segments.Count <= 0)
			{
				return Vector3.zero;
			}
			return transform.TransformPoint(segments[0].GetPos(0f));
		}
		if(dist >= totalDistance)
		{
			PathSegment last = segments[segments.Count - 1];
			return transform.TransformPoint(last.GetPos(last.length));
		}

		foreach(PathSegment segment in segments)
		{
			if(dist >= segment.length)
			{
				dist -= segment.length;
				continue;
			}

			return transform.TransformPoint(segment.GetPos(dist));
		}

		Debug.LogError("Path length mis-calculation");
		return transform.position;
	}

	public Vector3 GetVelocity(float dist)
	{
		CheckValid();

		if(dist <= 0f)
		{
			return transform.TransformVector(segments[0].GetVelocity(0f));
		}
		if(dist >= totalDistance)
		{
			PathSegment last = segments[segments.Count - 1];
			return transform.TransformVector(last.GetVelocity(last.length));
		}

		foreach(PathSegment segment in segments)
		{
			if(dist >= segment.length)
			{
				dist -= segment.length;
				continue;
			}

			return transform.TransformVector(segment.GetVelocity(dist));
		}

		Debug.LogError("Path length mis-calculation");
		return transform.forward;
	}

	public Vector3 GetNormal(float dist)
	{
		CheckValid();
		return Vector3.Normalize(GetVelocity(dist));
	}


	private void CheckValid()
	{
		if(!isValid)
		{
			Validate();
		}
	}

	private void Validate()
	{
		AssignSegments();
		CalculateTotalDistance();

		isValid = true;
	}

	private void AssignSegments()
	{
		segments = new List<PathSegment>();

		for(int i = 1; i < nodes.Count; i += 3)
		{
			//Each segment shares its first node with the previous segments last node
			segments.Add(new PathSegment(nodes[i - 1], nodes[i + 0], nodes[i + 1], nodes[i + 2]));
		}
	}

	private void CalculateTotalDistance()
	{
		//Sum up the total distance
		totalDistance = 0f;
		foreach(PathSegment segment in segments)
		{
			totalDistance += segment.length;
		}
	}


	void OnDrawGizmos()
	{
		CheckValid();

		const float res = 0.1f;

		Gizmos.color = Color.grey;

		Vector3 lastPos = GetPos(0f);
		for(float d = res; d <= totalDistance; d += res)
		{
			Vector3 pos = GetPos(d);

			Gizmos.DrawLine(lastPos, pos);

			lastPos = pos;
		}
	}

	void OnDrawGizmosSelected()
	{
		CheckValid();


		Gizmos.color = Color.grey;
		for(int i = 1; i < nodes.Count; i += 3)
		{
			Gizmos.DrawLine(transform.TransformPoint(nodes[i - 1].position), transform.TransformPoint(nodes[i + 0].position));
			Gizmos.DrawLine(transform.TransformPoint(nodes[i + 1].position), transform.TransformPoint(nodes[i + 2].position));
		}
		Gizmos.color = Color.white;


		const float res = 0.1f;

		Vector3 lastPos = GetPos(0f);
		for(float d = res; d <= totalDistance; d += res)
		{
			Vector3 pos = GetPos(d);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(lastPos, pos);

			Gizmos.color = new Color(0.65f, 0.65f, 0.65f);
			Gizmos.DrawRay(pos, GetVelocity(d) * res * 0.5f);

			//Debug.Log(pos);

			lastPos = pos;
		}
	}

}
