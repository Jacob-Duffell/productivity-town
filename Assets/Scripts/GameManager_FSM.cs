using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;

#region Enums

/// <summary>
/// Identifier for all static buttons.
/// </summary>
public enum ButtonTag
{
	// View State Buttons
    Activities = 0,
	Shop = 1,
	Settings = 2,
	EndBreakTime = 14,

	// Activities Menu Buttons
	NewActivity = 3,
	SortActivities = 4,

	// New Activity Menu Buttons
	CancelActivity = 5,
	SaveActivity = 6,

	// Shop Menu Buttons
	BuildingsTab = 7,
	RoadsTab = 8,

	// Build State Buttons
	PlaceBuilding = 9,
	RotateBuilding = 10,
	CancelBuilding = 11,

	// Set Deadline State Buttons
	SelectDifficulty = 12,

	// Enemy Invasion State Buttons
	StartNewWeek = 13,

	// Settings State Buttons
	ToggleSound = 15,
	OpenHelpMenu = 16,

	// Help State Buttons
	NextPage = 17,
	PreviousPage = 18,
	CloseHelpMenu = 19
};

/// <summary>
/// Levels of priority for activities.
/// </summary>
public enum ActivityPriority
{
	Low = 0,
	Medium = 1,
	High = 2
};

/// <summary>
/// All sorting methods for the Activities menu
/// </summary>
public enum ActivitySort
{
	DueDateDescending = 0,
	DueDateAscending = 1,
	ActivityTitleDescending = 2,
	ActivityTitleAscending = 3,
	PriorityDescending = 4,
	PriorityAscending = 5
};

/// <summary>
/// IDs for all types of buildings.
/// </summary>
public enum BuildingID
{
	HouseBrown = 0,
	HouseBlue = 1,
	HouseGreen = 2,
	HouseYellow = 3,
	BarberShop = 4,
	CoffeeShop = 5,
	Gym = 6,
	PawnShop = 7,
	PawnShopCorner = 8,
	Pharmacy = 9,
	PharmacyCorner = 10,
	VideoRentalShop = 11,
	VideoRentalShopCorner = 12,
	Garage = 13,
	Cinema = 14,
	ApartmentBrown = 15,
	ApartmentOrange = 16,
	ApartmentRed = 17,
	OfficeBrown = 18,
	OfficeBlue = 19,
	OfficeGrey = 20
};

/// <summary>
/// The current state of enemy invasion.
/// </summary>
public enum InvasionState
{
	DeadlineToBeSet = 0,
	CountdownToInvasion = 1,
	InvasionInProgress = 2,
	BreakTime = 3,
	Tutorial = 4
};

/// <summary>
/// IDs for all types of roads.
/// </summary>
public enum RoadID
{
	StraightRoad = 0,
	CornerRoad = 1,
	CrossRoad = 2,
	TSectionRoad = 3
};

#endregion

/// <summary>
/// The main class that manages all game states and overall functionality.
/// </summary>
public class GameManager_FSM : MonoBehaviour
{
	[Header("Audio Sources")]
	[SerializeField] private AudioSource activityCompleteHigh;
	[SerializeField] private AudioSource activityCompleteLow;
	[SerializeField] private AudioSource activityCompleteMedium;
	[SerializeField] private AudioSource backgroundMusic;
	[SerializeField] private AudioSource buildingPurchasedAndPlaced;
	[SerializeField] private AudioSource buttonTap;
	[SerializeField] private AudioSource enemyInvasionBad;
	[SerializeField] private AudioSource enemyInvasionGood;
	[SerializeField] private AudioSource insufficientFunds;
	[SerializeField] private AudioSource moneyCollected;
	[SerializeField] private AudioSource newWeekStarted;

	[Header("Canvases")]
	[SerializeField] private Canvas buildStateCanvas;
	[SerializeField] private Canvas enemyInvasionCanvas;
	[SerializeField] private Canvas helpCanvas;
	[SerializeField] private Canvas setDeadlineCanvas;
	[SerializeField] private Canvas viewStateCanvas;

	[Header("UI Prefabs")]
	[SerializeField] private GameObject activityItemPrefab;
	[SerializeField] private GameObject shopItemPrefab;

	[Header("Menus")]
	[SerializeField] private GameObject activityMenu;
	[SerializeField] private GameObject enemyInvasionMenu;
	[SerializeField] private GameObject helpMenu;
	[SerializeField] private GameObject newActivityMenu;
	[SerializeField] private GameObject setDeadlinePanel;
	[SerializeField] private GameObject settingsMenu;
	[SerializeField] private GameObject shopMenu;

	[Header("Lists")]
	[SerializeField] private List<Activity> activities = new List<Activity>();
	[SerializeField] private List<Building> buildings = new List<Building>();
	[SerializeField] private List<Road> roads = new List<Road>();

	[Header("Containers")]
	[SerializeField] private Transform activityItemContainer;
	[SerializeField] private Transform shopItemContainer;

	private Building selectedBuilding;
	private Road selectedRoad;

	private float timeLeftToSave;
	private double timeUntilEnemyInvasion;
	private GameBaseState currentState;
	private int currentMoney = 1000;
	private int currentSoldiers;
	private int soldiersNeeded;
	private InvasionState currentInvasionState;
	private List<GameObject> currentBuildings = new List<GameObject>();
	private long currentTime;
	private long savedTime;
	private long timeDifference;
	private TMP_Text moneyText;
	private TMP_Text soldierText;
	private TMP_Text timeUntilInvasionText;

	private readonly GameActivityMenuState activityMenuState = new GameActivityMenuState();
	private readonly GameBuildState buildState = new GameBuildState();
	private readonly GameEnemyInvasionState enemyInvasionState = new GameEnemyInvasionState();
	private readonly GameHelpMenuState helpMenuState = new GameHelpMenuState();
	private readonly GameNewActivityMenuState newActivityMenuState = new GameNewActivityMenuState();
	private readonly GameSetDeadlineState setDeadlineState = new GameSetDeadlineState();
	private readonly GameSettingsMenuState settingsMenuState = new GameSettingsMenuState();
	private readonly GameShopMenuState shopMenuState = new GameShopMenuState();
	private readonly GameViewState viewState = new GameViewState();

	private void Start()
	{
		// Uncomment the line of code below when building the game.
		//SetUpNotifications();

		CreateDirectories();

		currentTime = DateTime.Now.Ticks;

		if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData" + Path.DirectorySeparatorChar + "savedata.txt"))
		{
			SaveDataHandler.Load();

			currentInvasionState = SaveData.CurrentInvasionState;
			currentMoney = SaveData.TotalMoney;
			currentSoldiers = SaveData.TotalSoldiers;
			soldiersNeeded = SaveData.SoldiersNeeded;

			savedTime = SaveData.SavedDateTime;
			timeDifference = currentTime - savedTime;

			TimeSpan _timeDifference = new TimeSpan(timeDifference);
			timeUntilEnemyInvasion = SaveData.SavedInvasionTimeRemaining - _timeDifference.TotalSeconds;

			if (SaveData.SoundOn == false)
			{
				AudioListener.volume = 0.0f;
			}
			else
			{
				AudioListener.volume = 1.0f;
			}

			LoadBuildings();
			LoadRoads();
		}
		else
		{
			currentInvasionState = InvasionState.Tutorial;
		}
		
		if (currentInvasionState == InvasionState.DeadlineToBeSet)
		{
			TransitionToState(setDeadlineState);
		}
		else if (currentInvasionState == InvasionState.Tutorial)
		{
			TransitionToState(helpMenuState);
		}
		else
		{
			backgroundMusic.Play();

			TransitionToState(ViewState);
		}

		TimeLeftToSave = 15;

		moneyText = viewStateCanvas.transform.GetChild(6).GetChild(1).GetComponent<TMP_Text>();
		timeUntilInvasionText = viewStateCanvas.transform.GetChild(6).GetChild(2).GetComponent<TMP_Text>();
		soldierText = viewStateCanvas.transform.GetChild(6).GetChild(4).GetComponent<TMP_Text>();
	}

	private void Update()
	{
		CurrentState.Update(this);

		if (currentInvasionState != InvasionState.BreakTime)
		{
			EnemyInvasionCountdown();
		}
		
		if (viewStateCanvas.gameObject.activeSelf)
		{
			UpdateHUD();
		}

		timeLeftToSave -= Time.deltaTime;

		if (timeLeftToSave <= 0)
		{
			SaveGame();
		}
	}

	#region Private functions

	/// <summary>
	/// If save data paths do not exist, they are created here.
	/// </summary>
	private void CreateDirectories()
	{
		string _activityPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities";
		string _saveDataPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData";

		if (!Directory.Exists(_activityPath))
		{
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities");
		}

		if (!Directory.Exists(_saveDataPath))
		{
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "SaveData");
		}
	}

	/// <summary>
	/// Counts down to the enemy invasion.
	/// </summary>
	private void EnemyInvasionCountdown()
	{
		timeUntilEnemyInvasion -= Time.deltaTime;

		if (timeUntilEnemyInvasion <= 0)
		{
			timeUntilEnemyInvasion = 0;
		}
	}

	/// <summary>
	/// Loads and initialises all buildings.
	/// </summary>
	private void LoadBuildings()
	{
		GameObject _currentBuilding;
		GameObject _currentBuildingPrefab;
		int _currentBuildingIndex;
		int _placedBuildingCount = SaveData.PlacedBuildings.Count;

		for (int i = 0; i < _placedBuildingCount; i++)
		{
			_currentBuildingIndex = (int)SaveData.PlacedBuildings[i].BuildingID;
			_currentBuildingPrefab = buildings[_currentBuildingIndex].BuildingPrefab;
			
			_currentBuilding = Instantiate(_currentBuildingPrefab);
			_currentBuilding.GetComponent<BuildingController>().PopulateThisBuilding(SaveData.PlacedBuildings[i].BuildingID, 
				SaveData.PlacedBuildings[i].Position, 
				SaveData.PlacedBuildings[i].Rotation, 
				SaveData.PlacedBuildings[i].Scale, 
				SaveData.PlacedBuildings[i].CurrentTimeRemaining,
				SaveData.PlacedBuildings[i].Revenue,
				i);

			currentBuildings.Add(_currentBuilding);
		}
	}

	/// <summary>
	/// Loads all initialised roads.
	/// </summary>
	private void LoadRoads()
	{
		GameObject _currentRoad;
		GameObject _currentRoadPrefab;
		int _currentRoadIndex;
		int _placedRoadCount = SaveData.PlacedRoads.Count;

		for (int i = 0; i < _placedRoadCount; i++)
		{
			_currentRoadIndex = (int)SaveData.PlacedRoads[i].RoadID;
			_currentRoadPrefab = roads[_currentRoadIndex].RoadPrefab;

			_currentRoad = Instantiate(_currentRoadPrefab);
			_currentRoad.GetComponent<RoadController>().PopulateThisRoad(SaveData.PlacedRoads[i].RoadID,
				SaveData.PlacedRoads[i].Position,
				SaveData.PlacedRoads[i].Rotation,
				SaveData.PlacedRoads[i].Scale,
				i);
		}
	}	

	/// <summary>
	/// Sets up notifications for when the game is not running.
	/// </summary>
	private void SetUpNotifications()
	{
		AndroidNotificationCenter.CancelAllDisplayedNotifications();

		AndroidNotificationChannel _channel = new AndroidNotificationChannel()
		{
			Id = "channel_id",
			Name = "Reminder Channel",
			Importance = Importance.Default,
			Description = "Scheduled Reminder Notifications",
			CanBypassDnd = false,
			CanShowBadge = true,
			EnableLights = true,
			EnableVibration = true,
			LockScreenVisibility = LockScreenVisibility.Public
		};

		AndroidNotificationCenter.RegisterNotificationChannel(_channel);

		AndroidNotification _notification = new AndroidNotification()
		{
			Title = "Your Buildings are Ready!",
			Text = "Don't forget to collect your revenue, and check off some tasks too!",
			SmallIcon = "icon_0",
			LargeIcon = "icon_1",
			FireTime = DateTime.Today.AddDays(1).AddHours(18),
			RepeatInterval = new TimeSpan(1, 0, 0, 0),
			ShouldAutoCancel = true,
			Style = NotificationStyle.BigTextStyle
		};

		int _id = AndroidNotificationCenter.SendNotification(_notification, "channel_id");

		if (AndroidNotificationCenter.CheckScheduledNotificationStatus(_id) == NotificationStatus.Scheduled)
		{
			AndroidNotificationCenter.CancelAllNotifications();
			AndroidNotificationCenter.SendNotification(_notification, "channel_id");
		}
	}

	/// <summary>
	/// Constantly keeps the HUD updated.
	/// </summary>
	private void UpdateHUD()
	{
		moneyText.text = "£" + currentMoney;

		if (currentInvasionState == InvasionState.BreakTime)
		{
			timeUntilInvasionText.text = "Break Time!";
		}
		else
		{
			timeUntilInvasionText.text = TimeSpan.FromSeconds((int)timeUntilEnemyInvasion).ToString(@"d\d\:h\h\:m\m\:s\s", System.Globalization.CultureInfo.InvariantCulture);
		}
		

		soldierText.text = currentSoldiers + "/" + soldiersNeeded;
	}

	#endregion

	#region Public functions

	/// <summary>
	/// Handles functionality for all static button presses.
	/// </summary>
	/// <param name="_buttonTag"></param>
	public void OnButtonPressed(int _buttonTag)
	{
		buttonTap.Play();
		currentState.OnButtonPressed(this, (ButtonTag)_buttonTag);
	}

	/// <summary>
	/// Saves the game and resets the save timer.
	/// </summary>
	public void SaveGame()
	{
		SaveData.SavedDateTime = DateTime.Now.Ticks;
		SaveData.SavedInvasionTimeRemaining = timeUntilEnemyInvasion;
		SaveData.TotalMoney = currentMoney;
		SaveData.TotalSoldiers = currentSoldiers;
		SaveData.SoldiersNeeded = soldiersNeeded;
		SaveData.CurrentInvasionState = currentInvasionState;
		SaveDataHandler.Save();
		timeLeftToSave = 15;
	}

	/// <summary>
	/// Transitions between game states.
	/// </summary>
	/// <param name="_state"></param>
	public void TransitionToState(GameBaseState _state)
	{
        currentState = _state;
        currentState.EnterState(this);
	}

	#endregion

	#region Encapsulated variables

	public AudioSource ActivityCompleteHigh { get => activityCompleteHigh; set => activityCompleteHigh = value; }
	public AudioSource ActivityCompleteLow { get => activityCompleteLow; set => activityCompleteLow = value; }
	public AudioSource ActivityCompleteMedium { get => activityCompleteMedium; set => activityCompleteMedium = value; }
	public AudioSource BackgroundMusic { get => backgroundMusic; set => backgroundMusic = value; }
	public AudioSource BuildingPurchasedAndPlaced { get => buildingPurchasedAndPlaced; set => buildingPurchasedAndPlaced = value; }
	public AudioSource ButtonTap { get => buttonTap; set => buttonTap = value; }
	public AudioSource EnemyInvasionBad { get => enemyInvasionBad; set => enemyInvasionBad = value; }
	public AudioSource EnemyInvasionGood { get => enemyInvasionGood; set => enemyInvasionGood = value; }
	public AudioSource InsufficientFunds { get => insufficientFunds; set => insufficientFunds = value; }
	public AudioSource MoneyCollected { get => moneyCollected; set => moneyCollected = value; }
	public AudioSource NewWeekStarted { get => newWeekStarted; set => newWeekStarted = value; }
	public Canvas BuildStateCanvas { get => buildStateCanvas; set => buildStateCanvas = value; }
	public Canvas EnemyInvasionCanvas { get => enemyInvasionCanvas; set => enemyInvasionCanvas = value; }
	public Canvas HelpCanvas { get => helpCanvas; set => helpCanvas = value; }
	public Canvas SetDeadlineCanvas { get => setDeadlineCanvas; set => setDeadlineCanvas = value; }
	public Canvas ViewStateCanvas { get => viewStateCanvas; set => viewStateCanvas = value; }
	public GameObject ActivityItemPrefab { get => activityItemPrefab; set => activityItemPrefab = value; }
	public GameObject ShopItemPrefab { get => shopItemPrefab; set => shopItemPrefab = value; }
	public GameObject ActivityMenu { get => activityMenu; set => activityMenu = value; }
	public GameObject EnemyInvasionMenu { get => enemyInvasionMenu; set => enemyInvasionMenu = value; }
	public GameObject HelpMenu { get => helpMenu; set => helpMenu = value; }
	public GameObject NewActivityMenu { get => newActivityMenu; set => newActivityMenu = value; }
	public GameObject SetDeadlinePanel { get => setDeadlinePanel; set => setDeadlinePanel = value; }
	public GameObject SettingsMenu { get => settingsMenu; set => settingsMenu = value; }
	public GameObject ShopMenu { get => shopMenu; set => shopMenu = value; }
	public List<Activity> Activities { get => activities; set => activities = value; }
	public List<Building> Buildings { get => buildings; set => buildings = value; }
	public List<Road> Roads { get => roads; set => roads = value; }
	public Transform ActivityItemContainer { get => activityItemContainer; set => activityItemContainer = value; }
	public Transform ShopItemContainer { get => shopItemContainer; set => shopItemContainer = value; }
	public Building SelectedBuilding { get => selectedBuilding; set => selectedBuilding = value; }
	public Road SelectedRoad { get => selectedRoad; set => selectedRoad = value; }
	public float TimeLeftToSave { get => timeLeftToSave; set => timeLeftToSave = value; }
	public double TimeUntilEnemyInvasion { get => timeUntilEnemyInvasion; set => timeUntilEnemyInvasion = value; }
	public GameBaseState CurrentState { get => currentState; set => currentState = value; }
	public int CurrentMoney { get => currentMoney; set => currentMoney = value; }
	public int CurrentSoldiers { get => currentSoldiers; set => currentSoldiers = value; }
	public int SoldiersNeeded { get => soldiersNeeded; set => soldiersNeeded = value; }
	public InvasionState CurrentInvasionState { get => currentInvasionState; set => currentInvasionState = value; }
	public List<GameObject> CurrentBuildings { get => currentBuildings; set => currentBuildings = value; }
	public long CurrentTime { get => currentTime; set => currentTime = value; }
	public long SavedTime { get => savedTime; set => savedTime = value; }
	public long TimeDifference { get => timeDifference; set => timeDifference = value; }
	public GameActivityMenuState ActivityMenuState => activityMenuState;
	public GameBuildState BuildState => buildState;
	public GameEnemyInvasionState EnemyInvasionState => enemyInvasionState;
	public GameHelpMenuState HelpMenuState => helpMenuState;
	public GameNewActivityMenuState NewActivityMenuState => newActivityMenuState;
	public GameSetDeadlineState SetDeadlineState => setDeadlineState;
	public GameSettingsMenuState SettingsMenuState => settingsMenuState;
	public GameShopMenuState ShopMenuState => shopMenuState;
	public GameViewState ViewState => viewState;

	#endregion
}