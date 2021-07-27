using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable version of SaveData, for use when saving or loading.
/// </summary>
public class SaveDataScriptable : ScriptableObject
{
    [SerializeField] private double savedInvasionTimeRemaining;
    [SerializeField] private int soldiersNeeded;
    [SerializeField] private int totalMoney;
    [SerializeField] private int totalSoldiers;
    [SerializeField] private InvasionState currentInvasionState;
    [SerializeField] private List<PlacedBuilding> placedBuildings = new List<PlacedBuilding>();
    [SerializeField] private List<PlacedRoad> placedRoads = new List<PlacedRoad>();
    [SerializeField] private long savedDateTime;
    [SerializeField] private bool soundOn;

	#region Encapsulated variables

	public double SavedInvasionTimeRemaining { get => savedInvasionTimeRemaining; set => savedInvasionTimeRemaining = value; }
    public int SoldiersNeeded { get => soldiersNeeded; set => soldiersNeeded = value; }
    public int TotalMoney { get => totalMoney; set => totalMoney = value; }
    public int TotalSoldiers { get => totalSoldiers; set => totalSoldiers = value; }
    public InvasionState CurrentInvasionState { get => currentInvasionState; set => currentInvasionState = value; }
    public List<PlacedBuilding> PlacedBuildings { get => placedBuildings; set => placedBuildings = value; }
    public List<PlacedRoad> PlacedRoads { get => placedRoads; set => placedRoads = value; }
    public long SavedDateTime { get => savedDateTime; set => savedDateTime = value; }
	public bool SoundOn { get => soundOn; set => soundOn = value; }

	#endregion
}