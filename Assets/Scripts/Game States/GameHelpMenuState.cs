using TMPro;
using UnityEngine;

/// <summary>
/// Handles functionality for the Help Menu state.
/// </summary>
public class GameHelpMenuState : GameBaseState
{
	private int currentPageNumber;
	private TMP_Text pageTitle;
	private Transform pageHolder;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(false);
		_gameController.HelpCanvas.gameObject.SetActive(true);

		currentPageNumber = 0;
		pageHolder = _gameController.HelpMenu.transform.GetChild(1);
		pageTitle = _gameController.HelpMenu.transform.GetChild(0).GetComponent<TMP_Text>();

		pageHolder.GetChild(currentPageNumber).gameObject.SetActive(true);
	}

	public override void Update(GameManager_FSM _gameController)
	{
		switch(currentPageNumber)
		{
			case 0:
				pageTitle.text = "Introduction";
				break;
			case 1:
				pageTitle.text = "Setting Your Target";
				break;
			case 2:
				pageTitle.text = "Purchasing Buildings";
				break;
			case 3:
				pageTitle.text = "Viewing Activities";
				break;
			case 4:
				pageTitle.text = "Creating An Activity";
				break;
			case 5:
				pageTitle.text = "Viewing Your Town";
				break;
			case 6:
				pageTitle.text = "Collecting Revenue";
				break;
			case 7:
				pageTitle.text = "Enemy Invasion";
				break;
			case 8:
				pageTitle.text = "Other Tips";
				break;
			default: 
				break;
		}
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch(_buttonTag)
		{
			case ButtonTag.CloseHelpMenu:
				pageHolder.GetChild(currentPageNumber).gameObject.SetActive(false);
				_gameController.HelpCanvas.gameObject.SetActive(false);

				if (_gameController.CurrentInvasionState == InvasionState.Tutorial)
				{
					_gameController.BackgroundMusic.Play();
					_gameController.CurrentInvasionState = InvasionState.DeadlineToBeSet;
					_gameController.TransitionToState(_gameController.SetDeadlineState);
				}
				else
				{
					_gameController.TransitionToState(_gameController.ViewState);
				}
				break;
			case ButtonTag.NextPage:
				TurnPage(1);
				break;
			case ButtonTag.PreviousPage:
				TurnPage(-1);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Changes the "page" of the help menu.
	/// </summary>
	/// <param name="_newPage"></param>
	private void TurnPage(int _newPage)
	{
		pageHolder.GetChild(currentPageNumber).gameObject.SetActive(false);

		currentPageNumber += _newPage;

		pageHolder.GetChild(currentPageNumber).gameObject.SetActive(true);
	}

	#endregion
}
