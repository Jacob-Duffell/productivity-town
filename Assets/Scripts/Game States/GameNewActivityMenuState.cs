using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles functionality for the New Activity Menu state.
/// </summary>
public class GameNewActivityMenuState : GameBaseState
{
	private Activity newActivity;
	private TMP_Dropdown priorityChoice;
	private TMP_InputField titleInput;
	private TMP_Text dateInput;
	private TMP_Text warningText;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(true);
		_gameController.NewActivityMenu.SetActive(true);

		titleInput = _gameController.NewActivityMenu.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<TMP_InputField>();
		priorityChoice = _gameController.NewActivityMenu.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<TMP_Dropdown>();
		dateInput = _gameController.NewActivityMenu.transform.GetChild(3).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
		warningText = _gameController.NewActivityMenu.transform.GetChild(3).GetChild(3).GetComponent<TMP_Text>();

		titleInput.text = "";
		warningText.text = "";
		priorityChoice.ClearOptions();
		warningText.gameObject.SetActive(false);

		newActivity = ScriptableObject.CreateInstance<Activity>();

		List<string> _priorityChoiceOptions = new List<string>();

		for (int i = 0; i < newActivity.GetPriorityEnumLength(); i++)
		{
			_priorityChoiceOptions.Add(newActivity.GetActivityPriorityName(i));
		}

		priorityChoice.AddOptions(_priorityChoiceOptions);
	}

	public override void Update(GameManager_FSM _gameController)
	{
		// No code needed.
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.Activities:
				_gameController.NewActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ActivityMenuState);
				break;
			case ButtonTag.CancelActivity:
				_gameController.NewActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ActivityMenuState);
				break;
			case ButtonTag.SaveActivity:
				OnSaveActivityPressed(_gameController);
				break;
			case ButtonTag.Settings:
				_gameController.NewActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.SettingsMenuState);
				break;
			case ButtonTag.Shop:
				_gameController.NewActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ShopMenuState);
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Decides if the due date if before, during or after today's date.
	/// </summary>
	/// <param name="_dueDateInput"></param>
	/// <returns></returns>
	private bool CompareDueDateToNow(string _dueDateInput)
	{
		DateTime _today = DateTime.Now;
		DateTime _dueDate = DateTime.Parse(_dueDateInput);
		TimeSpan _compareDays = _dueDate - _today;
		int _differenceInDays = Mathf.CeilToInt((float)_compareDays.TotalDays);

		if (_differenceInDays < 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Functionality for the Save Activity button.
	/// </summary>
	/// <param name="gameController"></param>
	private void OnSaveActivityPressed(GameManager_FSM gameController)
	{
		bool _activityExists = false;
		bool _dateNotSelected = false;

		if (dateInput.text.Equals("Select a date..."))
		{
			_dateNotSelected = true;
		}

		for (int i = 0; i < gameController.Activities.Count; i++)
		{
			if (gameController.Activities[i].ActivityTitle == titleInput.text)
			{
				_activityExists = true;
			}
		}

		if (_activityExists)
		{
			warningText.text = "An activity with this title already exists - Please choose another title!";
			warningText.gameObject.SetActive(true);
		}
		else if (_dateNotSelected)
		{
			warningText.text = "Please select a date before saving the activity!";
			warningText.gameObject.SetActive(true);
		}
		else if (CompareDueDateToNow(dateInput.text))
		{
			warningText.text = "Please select a date that is today or later!";
			warningText.gameObject.SetActive(true);
		}
		else
		{
			newActivity.ActivityTitle = titleInput.text;
			newActivity.Priority = (ActivityPriority)priorityChoice.value;
			newActivity.DueDate = dateInput.text;

			string _activityJSON = JsonUtility.ToJson(newActivity);
			string _path = Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities" + Path.DirectorySeparatorChar;

			File.WriteAllText(_path + newActivity.ActivityTitle + ".txt", _activityJSON);

			gameController.NewActivityMenu.SetActive(false);
			gameController.TransitionToState(gameController.ActivityMenuState);
		}
	}

	#endregion
}
