using TMPro;
using UnityEngine.UI;

/// <summary>
/// Functionality for the Set Deadline state
/// </summary>
public class GameSetDeadlineState : GameBaseState
{
	private const int SEVEN_DAYS = 604800;

	private int soldiersNeeded;
	private Slider difficultySlider;
	private TMP_Text difficultyDescription;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(false);
		_gameController.SetDeadlineCanvas.gameObject.SetActive(true);

		difficultyDescription = _gameController.SetDeadlinePanel.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
		difficultySlider = _gameController.SetDeadlinePanel.transform.GetChild(1).GetChild(3).GetComponent<Slider>();

		soldiersNeeded = (int)difficultySlider.value;
	}

	public override void Update(GameManager_FSM _gameController)
	{
		soldiersNeeded = (int)difficultySlider.value;

		if (soldiersNeeded == 0)
		{
			difficultyDescription.text = "If you select this option, you will enter Break Time. There will be no invasion countdown, but you cannot collect money, build new buildings or collect soldiers from completing activities.";
		}
		else if (soldiersNeeded == 1)
		{
			difficultyDescription.text = "If you select this option, you must complete a minimum of " + soldiersNeeded + " task within the next 7 days.";
		}
		else
		{
			difficultyDescription.text = "If you select this option, you must complete a minimum of " + soldiersNeeded + " tasks within the next 7 days.";
		}
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.SelectDifficulty:
				SetDifficulty(_gameController);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private functions

	private void SetDifficulty(GameManager_FSM _gameController)
	{
		_gameController.NewWeekStarted.Play();

		_gameController.SoldiersNeeded = soldiersNeeded;
		_gameController.TimeUntilEnemyInvasion = SEVEN_DAYS;

		if (soldiersNeeded == 0)
		{
			_gameController.CurrentInvasionState = InvasionState.BreakTime;
		}
		else
		{
			_gameController.CurrentInvasionState = InvasionState.CountdownToInvasion;
		}

		_gameController.TimeLeftToSave = 0.5f;

		_gameController.SetDeadlineCanvas.gameObject.SetActive(false);
		_gameController.TransitionToState(_gameController.ViewState);
	}

	#endregion
}
