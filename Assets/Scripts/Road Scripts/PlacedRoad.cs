using UnityEngine;

/// <summary>
/// Data of a road that has been purchased and built, to be stored in Save Data.
/// </summary>
[System.Serializable]
public class PlacedRoad
{
	[SerializeField] private RoadID roadID;
	[SerializeField] private Vector3 position;
	[SerializeField] private Quaternion rotation;
	[SerializeField] private Vector3 scale;

	#region Encapsulated variables

	public RoadID RoadID { get => roadID; set => roadID = value; }
	public Vector3 Position { get => position; set => position = value; }
	public Quaternion Rotation { get => rotation; set => rotation = value; }
	public Vector3 Scale { get => scale; set => scale = value; }

	#endregion
}