using UnityEngine;

/// <summary>
/// A scriptable object that contains all road data.
/// </summary>
[CreateAssetMenu(fileName = "New Road", menuName = "Road", order = 3)]
public class Road : ScriptableObject
{
	[SerializeField] private RoadID roadID;
	[SerializeField] private string roadName;
	[SerializeField] private Sprite roadIcon;
	[SerializeField] private GameObject roadPrefab;
	[SerializeField] private GameObject previewPrefab;
	[SerializeField] private int purchasePrice;

	#region Encapsulated variables

	public RoadID RoadID { get => roadID; set => roadID = value; }
	public string RoadName { get => roadName; set => roadName = value; }
	public Sprite RoadIcon { get => roadIcon; set => roadIcon = value; }
	public GameObject RoadPrefab { get => roadPrefab; set => roadPrefab = value; }
	public GameObject PreviewPrefab { get => previewPrefab; set => previewPrefab = value; }
	public int PurchasePrice { get => purchasePrice; set => purchasePrice = value; }

	#endregion
}
