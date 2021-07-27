using System.Collections.Generic;

/// <summary>
/// Holds all data that gets regularly saved.
/// </summary>
public static class SaveData
{
    private static double savedInvasionTimeRemaining;
    private static int soldiersNeeded;
    private static int totalMoney;
    private static int totalSoldiers;
    private static InvasionState currentInvasionState;
    private static List<PlacedBuilding> placedBuildings = new List<PlacedBuilding>();
	private static List<PlacedRoad> placedRoads = new List<PlacedRoad>();
    private static long savedDateTime;
    private static bool soundOn = true;

	#region Encapsulated variables

	public static double SavedInvasionTimeRemaining { get => savedInvasionTimeRemaining; set => savedInvasionTimeRemaining = value; }
    public static int SoldiersNeeded { get => soldiersNeeded; set => soldiersNeeded = value; }
    public static int TotalMoney { get => totalMoney; set => totalMoney = value; }
    public static int TotalSoldiers { get => totalSoldiers; set => totalSoldiers = value; }
    public static InvasionState CurrentInvasionState { get => currentInvasionState; set => currentInvasionState = value; }
    public static List<PlacedBuilding> PlacedBuildings { get => placedBuildings; set => placedBuildings = value; }
	public static List<PlacedRoad> PlacedRoads { get => placedRoads; set => placedRoads = value; }
    public static long SavedDateTime { get => savedDateTime; set => savedDateTime = value; }
	public static bool SoundOn { get => soundOn; set => soundOn = value; }

	#endregion
}