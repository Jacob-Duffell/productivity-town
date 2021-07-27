using UnityEngine;

/// <summary>
/// Handles all functionality for Road prefabs.
/// </summary>
public class RoadController : MonoBehaviour
{
	private GameObject gameManager;
	private int thisRoadIndex;
	private PlacedRoad thisRoad = new PlacedRoad();
	private RoadID roadID;

	private void Awake()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameController");

		thisRoad.RoadID = roadID;
		thisRoad.Position = transform.position;
		thisRoad.Rotation = transform.rotation;
		thisRoad.Scale = transform.localScale;
	}

	private void Update()
	{
		// Ensures road data is up-to-date.
		if (gameManager.GetComponent<GameManager_FSM>().TimeLeftToSave <= 0.1f)
		{
			thisRoad.RoadID = roadID;
			thisRoad.Position = transform.position;
			thisRoad.Rotation = transform.rotation;
			thisRoad.Scale = transform.localScale;

			SaveData.PlacedRoads[thisRoadIndex] = thisRoad;
		}
	}

	#region Public functions

	/// <summary>
	/// Populates road data after initialisation.
	/// </summary>
	/// <param name="_roadID"></param>
	/// <param name="_position"></param>
	/// <param name="_rotation"></param>
	/// <param name="_scale"></param>
	/// <param name="_roadIndex"></param>
	public void PopulateThisRoad(RoadID _roadID, Vector3 _position, Quaternion _rotation, Vector3 _scale, int _roadIndex)
	{
		roadID = _roadID;

		thisRoad.RoadID = _roadID;
		thisRoad.Position = _position;
		thisRoad.Rotation = _rotation;
		thisRoad.Scale = _scale;

		thisRoadIndex = _roadIndex;

		transform.position = _position;
		transform.rotation = _rotation;
		transform.localScale = _scale;
	}

	#endregion

	#region Encapsulated variables

	public RoadID RoadID { get => roadID; set => roadID = value; }
	public int ThisRoadIndex { get => thisRoadIndex; set => thisRoadIndex = value; }
	public PlacedRoad ThisRoad { get => thisRoad; set => thisRoad = value; }

	#endregion
}
