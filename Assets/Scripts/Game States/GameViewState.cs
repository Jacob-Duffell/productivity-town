using UnityEngine;

/// <summary>
/// Handles functionality for the View State
/// </summary>
public class GameViewState : GameBaseState
{
	protected Plane plane;

	private GameObject viewStateButtons;
	private GameObject breakTimeButtons;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		viewStateButtons = _gameController.ViewStateCanvas.transform.GetChild(0).gameObject;
		breakTimeButtons = _gameController.ViewStateCanvas.transform.GetChild(1).gameObject;

		_gameController.ViewStateCanvas.gameObject.SetActive(true);

		if (_gameController.CurrentInvasionState == InvasionState.CountdownToInvasion)
		{
			viewStateButtons.SetActive(true);
			breakTimeButtons.SetActive(false);
		}
		else if (_gameController.CurrentInvasionState == InvasionState.BreakTime)
		{
			viewStateButtons.SetActive(false);
			breakTimeButtons.SetActive(true);
		}
	}

	public override void Update(GameManager_FSM _gameController)
	{
		TouchInput(_gameController);

		if (_gameController.TimeUntilEnemyInvasion <= 0)
		{
			_gameController.TransitionToState(_gameController.EnemyInvasionState);
		}
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.Activities:
				_gameController.TransitionToState(_gameController.ActivityMenuState);
				break;
			case ButtonTag.Shop:
				_gameController.TransitionToState(_gameController.ShopMenuState);
				break;
			case ButtonTag.Settings:
				_gameController.TransitionToState(_gameController.SettingsMenuState);
				break;
			case ButtonTag.EndBreakTime:
				_gameController.CurrentInvasionState = InvasionState.DeadlineToBeSet;
				_gameController.TransitionToState(_gameController.SetDeadlineState);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Limits the camera movment so that the user cannot leave the bounds of the game.
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
	/// Register the touch input during the view state.
	/// </summary>
	/// <param name="_gameController"></param>
	private void TouchInput(GameManager_FSM _gameController)
	{
		if (Input.touchCount >= 1)
		{
			plane.SetNormalAndPosition(_gameController.transform.up, _gameController.transform.position);
		}

		if (Input.touchCount >= 2)          // Pinch Screen
		{
			ZoomAndRotateCamera();
		}
		else if (Input.touchCount >= 1)     // Scroll Screen
		{
			MoveCamera();
		}

		ClampCamera(-650, 650, 30, 140, -650, 650);
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
