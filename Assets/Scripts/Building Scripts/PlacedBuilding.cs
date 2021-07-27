using UnityEngine;

/// <summary>
/// Contains data for a building that have been purchased and placed, and is saved in the Save Data.
/// </summary>
[System.Serializable]
public class PlacedBuilding
{
	[SerializeField] private BuildingID buildingID;
	[SerializeField] private Vector3 position;
	[SerializeField] private Quaternion rotation;
	[SerializeField] private Vector3 scale;
	[SerializeField] private float currentTimeRemaining;
	[SerializeField] private int revenue;

	#region Encapsulated variables

	public BuildingID BuildingID { get => buildingID; set => buildingID = value; }
	public Vector3 Position { get => position; set => position = value; }
	public Quaternion Rotation { get => rotation; set => rotation = value; }
	public Vector3 Scale { get => scale; set => scale = value; }
	public float CurrentTimeRemaining { get => currentTimeRemaining; set => currentTimeRemaining = value; }
	public int Revenue { get => revenue; set => revenue = value; }

	#endregion
}