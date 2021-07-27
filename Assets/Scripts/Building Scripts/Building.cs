using UnityEngine;

/// <summary>
/// Contains data for all buildings.
/// </summary>
[CreateAssetMenu(fileName = "New Building", menuName = "Building", order = 2)]
public class Building : ScriptableObject
{
	[SerializeField] private BuildingID buildingID;
	[SerializeField] private string buildingName;
    [SerializeField] private Sprite buildingIcon;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private GameObject previewPrefab;
    [SerializeField] private int purchasePrice;
    [SerializeField] private int revenue;

	#region Encapsulated variables

	public BuildingID BuildingID { get => buildingID; set => buildingID = value; }
	public string BuildingName { get => buildingName; set => buildingName = value; }
	public Sprite BuildingIcon { get => buildingIcon; set => buildingIcon = value; }
	public GameObject BuildingPrefab { get => buildingPrefab; set => buildingPrefab = value; }
	public GameObject PreviewPrefab { get => previewPrefab; set => previewPrefab = value; }
	public int PurchasePrice { get => purchasePrice; set => purchasePrice = value; }
	public int Revenue { get => revenue; set => revenue = value; }

	#endregion
}
