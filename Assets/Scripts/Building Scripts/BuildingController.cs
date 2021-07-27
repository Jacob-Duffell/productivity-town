using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles functionality for all placed buiildings.
/// </summary>
public class BuildingController : MonoBehaviour
{
	private bool readyForCollection;
	private BuildingID buildingID;
	private float currentTimeRemaining;
	private float maxTime;
	private GameObject breakTimeUI;
	private GameObject buildingCanvas;
	private GameObject collectionUI;
	private GameManager_FSM gameManager;
	private GameObject timerUI;
	private int thisBuildingIndex;
	private PlacedBuilding thisBuilding = new PlacedBuilding();
	private TMP_Text timerText;

	private void Awake()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager_FSM>();

		thisBuilding.BuildingID = buildingID;
		thisBuilding.Position = transform.position;
		thisBuilding.Rotation = transform.rotation;
		thisBuilding.Scale = transform.localScale;
		buildingCanvas = transform.GetChild(1).gameObject;
		breakTimeUI = transform.GetChild(1).GetChild(2).gameObject;
		collectionUI = transform.GetChild(1).GetChild(0).gameObject;
		timerUI = transform.GetChild(1).GetChild(1).gameObject;
		timerText = timerUI.transform.GetChild(2).GetComponent<TMP_Text>();

		readyForCollection = false;

		TimeSpan _nextDay = TimeSpan.FromTicks(DateTime.Today.AddDays(1).Ticks);
		TimeSpan _now = TimeSpan.FromTicks(DateTime.Now.Ticks);

		maxTime = (float)(_nextDay - _now).TotalSeconds;
		currentTimeRemaining = maxTime;
	}

	private void Update()
	{
		buildingCanvas.transform.LookAt(buildingCanvas.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

		if (currentTimeRemaining <= 0)
		{
			currentTimeRemaining = 0;
			timerText.text = "00:00:00";

			ReadyForCollection(true);
		}
		else
		{
			ReadyForCollection(false);
			currentTimeRemaining -= Time.deltaTime;
			timerText.text = TimeSpan.FromSeconds((int)currentTimeRemaining).ToString();
		}

		if (gameManager.TimeLeftToSave <= 0.1f)
		{
			thisBuilding.Position = transform.position;
			thisBuilding.Rotation = transform.rotation;
			thisBuilding.Scale = transform.localScale;
			thisBuilding.BuildingID = buildingID;
			thisBuilding.CurrentTimeRemaining = currentTimeRemaining;

			SaveData.PlacedBuildings[thisBuildingIndex] = thisBuilding;
		}

		if (gameManager.CurrentState == gameManager.BuildState)
		{
			collectionUI.SetActive(false);
			timerUI.SetActive(false);
		}
	}

	#region Input functions

	private void OnMouseDown()
	{
		if (gameManager.CurrentState == gameManager.ViewState)
		{
			if (readyForCollection && gameManager.CurrentInvasionState != InvasionState.BreakTime)
			{
				gameManager.MoneyCollected.Play();

				gameManager.CurrentMoney += thisBuilding.Revenue;

				GetComponent<ParticleSystem>().Play();

				ReadyForCollection(false);

				TimeSpan _nextDay = TimeSpan.FromTicks(DateTime.Today.AddDays(1).Ticks);
				TimeSpan _now = TimeSpan.FromTicks(DateTime.Now.Ticks);

				maxTime = (float)(_nextDay - _now).TotalSeconds;

				currentTimeRemaining = maxTime;
			}
			else
			{
				timerUI.SetActive(true);
			}
		}
	}

	private void OnMouseUp()
	{
		if (gameManager.CurrentState == gameManager.ViewState)
		{
			if (!readyForCollection)
			{
				timerUI.SetActive(false);
			}
		}
	}

	#endregion

	#region Public functions

	/// <summary>
	/// Populates values after initialisation.
	/// </summary>
	/// <param name="_buildingID"></param>
	/// <param name="_position"></param>
	/// <param name="_rotation"></param>
	/// <param name="_scale"></param>
	/// <param name="_currentTimeRemaining"></param>
	/// <param name="_revenue"></param>
	/// <param name="_buildingIndex"></param>
	public void PopulateThisBuilding(BuildingID _buildingID, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _currentTimeRemaining, int _revenue, int _buildingIndex)
	{
		buildingID = _buildingID;

		thisBuilding.BuildingID = _buildingID;
		thisBuilding.Position = _position;
		thisBuilding.Rotation = _rotation;
		thisBuilding.Scale = _scale;
		thisBuilding.CurrentTimeRemaining = _currentTimeRemaining;
		thisBuilding.Revenue = _revenue;

		thisBuildingIndex = _buildingIndex;

		transform.position = _position;
		transform.rotation = _rotation;
		transform.localScale = _scale;

		DateTime _savedTimeRemaining = new DateTime(TimeSpan.FromSeconds(_currentTimeRemaining).Ticks);
		DateTime _timeDifference = new DateTime(gameManager.TimeDifference);

		currentTimeRemaining = (float)(_savedTimeRemaining - _timeDifference).TotalSeconds;
	}

	/// <summary>
	/// Calculates if money is ready to be collected from building.
	/// </summary>
	/// <param name="_isReady"></param>
	private void ReadyForCollection(bool _isReady)
	{
		if (_isReady)
		{
			readyForCollection = true;
			timerUI.SetActive(false);

			if (gameManager.CurrentInvasionState == InvasionState.BreakTime)
			{
				collectionUI.SetActive(false);
				breakTimeUI.SetActive(true);
			}
			else
			{
				breakTimeUI.SetActive(false);
				collectionUI.SetActive(true);
			}
		}
		else
		{
			readyForCollection = false;
			breakTimeUI.SetActive(false);
			collectionUI.SetActive(false);
		}
	}

	#endregion

	#region Encapsulated variables

	public BuildingID BuildingID { get => buildingID; set => buildingID = value; }
	public int ThisBuildingIndex { get => thisBuildingIndex; set => thisBuildingIndex = value; }
	public PlacedBuilding ThisBuilding { get => thisBuilding; set => thisBuilding = value; }

	#endregion
}
