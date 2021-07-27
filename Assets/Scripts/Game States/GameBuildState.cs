using UnityEngine;

/// <summary>
/// Handles functionality for the Build state.
/// </summary>
public class GameBuildState : GameBaseState
{
	protected Plane plane;

	private BuildingID buildingID;
	private GameObject building;
	private GameObject road;
	private RoadID roadID;
	private GameObject preview;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(false);
		_gameController.BuildStateCanvas.gameObject.SetActive(true);
		
		float _cameraPosX = Camera.main.transform.position.x;
		float _cameraPosY = 0;
		float _cameraPosZ = Camera.main.transform.position.z;

		if (_gameController.SelectedBuilding != null)
		{
			buildingID = _gameController.SelectedBuilding.BuildingID;
			building = _gameController.SelectedBuilding.BuildingPrefab;
			preview = Object.Instantiate(_gameController.SelectedBuilding.PreviewPrefab, Vector3.zero, Quaternion.identity);

			_cameraPosY = 130;
		}
		else
		{
			roadID = _gameController.SelectedRoad.RoadID;
			road = _gameController.SelectedRoad.RoadPrefab;
			preview = Object.Instantiate(_gameController.SelectedRoad.PreviewPrefab, Vector3.zero, Quaternion.identity);

			_cameraPosY = 40;
		}

		Camera.main.transform.position = new Vector3(_cameraPosX, _cameraPosY, _cameraPosZ);
	}

	public override void Update(GameManager_FSM _gameController)
	{
		if (Input.touchCount >= 1)
		{
			plane.SetNormalAndPosition(_gameController.transform.up, _gameController.transform.position);
		}

		// Scroll
		if (Input.touchCount >= 1)
		{
			MoveCamera();
		}

		// Pinch
		if (Input.touchCount >= 2)
		{
			ZoomAndRotateCamera();
		}

		ClampCamera(-650, 650, 30, 140, -650, 650);

		CalculatePreviewPosition();
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.PlaceBuilding:
				PlaceBuilding(_gameController);
				break;
			case ButtonTag.RotateBuilding:
				RotateBuilding();
				break;
			case ButtonTag.CancelBuilding:
				ExitBuildMode(_gameController);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private build functions

	/// <summary>
	/// Places the building or road and removes money from the user.
	/// </summary>
	/// <param name="_gameController"></param>
	private void Build(GameManager_FSM _gameController)
	{
		if (_gameController.SelectedBuilding != null)
		{
			if (_gameController.CurrentMoney >= _gameController.SelectedBuilding.PurchasePrice)
			{
				_gameController.BuildingPurchasedAndPlaced.Play();

				_gameController.CurrentMoney -= _gameController.SelectedBuilding.PurchasePrice;

				GameObject _placedBuilding = Object.Instantiate(building, preview.transform.position, preview.transform.rotation);
				_placedBuilding.GetComponent<BuildingController>().BuildingID = buildingID;
				_placedBuilding.GetComponent<BuildingController>().ThisBuilding.Revenue = _gameController.SelectedBuilding.Revenue;
				SaveData.PlacedBuildings.Add(_placedBuilding.GetComponent<BuildingController>().ThisBuilding);
				_placedBuilding.GetComponent<BuildingController>().ThisBuildingIndex = SaveData.PlacedBuildings.Count - 1;
			}
			else
			{
				_gameController.InsufficientFunds.Play();
			}
		}
		else
		{
			if (_gameController.CurrentMoney >= _gameController.SelectedRoad.PurchasePrice)
			{
				_gameController.BuildingPurchasedAndPlaced.Play();

				_gameController.CurrentMoney -= _gameController.SelectedRoad.PurchasePrice;

				GameObject _placedRoad = Object.Instantiate(road, preview.transform.position, preview.transform.rotation);
				_placedRoad.GetComponent<RoadController>().RoadID = roadID;
				SaveData.PlacedRoads.Add(_placedRoad.GetComponent<RoadController>().ThisRoad);
				_placedRoad.GetComponent<RoadController>().ThisRoadIndex = SaveData.PlacedRoads.Count - 1;
			}
			else
			{
				_gameController.InsufficientFunds.Play();
			}
		}
	}

	/// <summary>
	/// Calculates the position of the preview object and limits it to a grid placement.
	/// </summary>
	private void CalculatePreviewPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

		if (Physics.Raycast(ray, out RaycastHit _hit))
		{
			float gridSize = 10;

			float _x = Mathf.Floor(_hit.point.x / gridSize) * gridSize;
			float _z = Mathf.Floor(_hit.point.z / gridSize) * gridSize;

			preview.transform.position = new Vector3(_x + gridSize, 0, _z + gridSize);
		}
	}

	#endregion

	#region Private preview object button functions

	/// <summary>
	/// Cancels the build.
	/// </summary>
	/// <param name="_gameController"></param>
	private void ExitBuildMode(GameManager_FSM _gameController)
	{
		Object.Destroy(preview);

		preview = null;
		building = null;
		road = null;

		_gameController.SelectedBuilding = null;
		_gameController.SelectedRoad = null;

		_gameController.BuildStateCanvas.gameObject.SetActive(false);
		_gameController.TransitionToState(_gameController.ViewState);
	}

	/// <summary>
	/// Places the building.
	/// </summary>
	/// <param name="_gameController"></param>
	private void PlaceBuilding(GameManager_FSM _gameController)
	{
		if (preview.GetComponent<PreviewObject>().CanBuild)
		{
			Build(_gameController);

			_gameController.TimeLeftToSave = 0.5f;

			if (_gameController.SelectedBuilding != null)
			{
				ExitBuildMode(_gameController);
			}
		}
		else
		{
			_gameController.InsufficientFunds.Play();
		}
	}

	/// <summary>
	/// Rotates the building or road by 90 degrees.
	/// </summary>
	private void RotateBuilding()
	{
		preview.transform.Rotate(0f, 90f, 0f);
	}

	#endregion

	#region Private camera Classes

	/// <summary>
	/// Restricts the camera within the bounds of the game world.
	/// </summary>
	/// <param name="_minX"></param>
	/// <param name="_maxX"></param>
	/// <param name="_minY"></param>
	/// <param name="_maxY"></param>
	/// <param name="_minZ"></param>
	/// <param name="_maxZ"></param>
	private void ClampCamera(int _minX, int _maxX, int _minY, int _maxY, int _minZ, int _maxZ)
	{
		if (Camera.main.transform.position.x < _minX)
		{
			Camera.main.transform.position = new Vector3(_minX, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
		else if (Camera.main.transform.position.x > _maxX)
		{
			Camera.main.transform.position = new Vector3(_maxX, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}

		if (Camera.main.transform.position.y < _minY)
		{
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, _minY, Camera.main.transform.position.z);
		}
		else if (Camera.main.transform.position.y > _maxY)
		{
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, _maxY, Camera.main.transform.position.z);
		}

		if (Camera.main.transform.position.z < _minZ)
		{
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, _minZ);
		}
		else if (Camera.main.transform.position.z > _maxZ)
		{
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, _maxZ);
		}
	}

	/// <summary>
	/// Pans the camera when the user drags their finger.
	/// </summary>
	private void MoveCamera()
	{
		Touch _touch = Input.GetTouch(0);
		Vector3 _delta = WorldToGridHelper.PlanePositionDelta(Camera.main, plane, _touch);

		if (_touch.phase == TouchPhase.Moved)
		{
			Camera.main.transform.Translate(_delta, Space.World);
		}
	}

	/// <summary>
	/// Zooms and rotates the camera when the user pinches the screen.
	/// </summary>
	private void ZoomAndRotateCamera()
	{
		Touch _touch1 = Input.GetTouch(0);
		Touch _touch2 = Input.GetTouch(1);

		Vector3 _pos1 = WorldToGridHelper.PlanePosition(Camera.main, plane, _touch1.position);
		Vector3 _pos2 = WorldToGridHelper.PlanePosition(Camera.main, plane, _touch2.position);
		Vector3 _pos1b = WorldToGridHelper.PlanePosition(Camera.main, plane, (_touch1.position - _touch1.deltaPosition));
		Vector3 _pos2b = WorldToGridHelper.PlanePosition(Camera.main, plane, (_touch2.position - _touch2.deltaPosition));

		// Calculate zoom
		float _zoom = Vector3.Distance(_pos1, _pos2) / Vector3.Distance(_pos1b, _pos2b);

		// Edge case
		if (_zoom == 0 || _zoom > 10)
		{
			return;
		}

		// Zoom camera between two touch points
		Camera.main.transform.position = Vector3.LerpUnclamped(_pos1, Camera.main.transform.position, 1 / _zoom);

		// Rotate camera
		if (_pos2b != _pos2)
		{
			Camera.main.transform.RotateAround(_pos1, plane.normal, Vector3.SignedAngle(_pos2 - _pos1, _pos2b - _pos1b, plane.normal));
		}
	}

	#endregion
}
