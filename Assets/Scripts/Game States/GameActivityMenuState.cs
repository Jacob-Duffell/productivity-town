using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles functionality for the Activity Menu state
/// </summary>
public class GameActivityMenuState : GameBaseState
{
	private TMP_Dropdown activitySortDropdown;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(true);
		_gameController.ActivityMenu.SetActive(true);

		activitySortDropdown = _gameController.ActivityMenu.transform.GetChild(3).GetChild(1).GetComponent<TMP_Dropdown>();
		activitySortDropdown.value = 0;

		for (int i = 0; i < _gameController.ActivityItemContainer.childCount; i++)
		{
			UnityEngine.Object.Destroy(_gameController.ActivityItemContainer.GetChild(i).gameObject);
		}

		_gameController.Activities = LoadActivities();

		PopulateActivityList(_gameController);
	}

	public override void Update(GameManager_FSM _gameController)
	{
		// No code needed here.
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.Activities:
				_gameController.ActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ViewState);
				break;
			case ButtonTag.NewActivity:
				_gameController.ActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.NewActivityMenuState);
				break;
			case ButtonTag.Settings:
				_gameController.ActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.SettingsMenuState);
				break;
			case ButtonTag.Shop:
				_gameController.ActivityMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ShopMenuState);
				break;
			case ButtonTag.SortActivities:
				SortActivities(_gameController);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private activity functions

	/// <summary>
	/// Loads all saved activities.
	/// </summary>
	/// <returns></returns>
	private List<Activity> LoadActivities()
	{
		List<Activity> _savedActivities = new List<Activity>();

		string _activitiesPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities";
		DirectoryInfo _info = new DirectoryInfo(_activitiesPath);
		FileInfo[] _fileInfos = _info.GetFiles();

		for (int i = 0; i < _fileInfos.Length; i++)
		{
			StreamReader _reader = new StreamReader(_fileInfos[i].FullName);
			string _fileText = _reader.ReadToEnd();
			Activity _savedActivity = ScriptableObject.CreateInstance<Activity>();
			JsonUtility.FromJsonOverwrite(_fileText, _savedActivity);
			_savedActivities.Add(_savedActivity);
			_reader.Close();
		}

		_savedActivities = _savedActivities.OrderBy(activity => DateTime.Parse(activity.DueDate)).ToList();

		return _savedActivities;
	}

	/// <summary>
	/// Populates the activity menu with the saved activities.
	/// </summary>
	/// <param name="_gameController"></param>
	private void PopulateActivityList(GameManager_FSM _gameController)
	{
		for (int i = 0; i < _gameController.Activities.Count; i++)
		{
			Activity _activity = _gameController.Activities[i];
			GameObject _activityObject = UnityEngine.Object.Instantiate(_gameController.ActivityItemPrefab, _gameController.ActivityItemContainer);

			_activityObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _activity.ActivityTitle;
			_activityObject.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Priority: " + _activity.Priority.ToString();
			_activityObject.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = "Due Date: " + _activity.DueDate;
			_activityObject.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = SetActivityWarningText(_activity);
			_activityObject.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().color = SetActivityWarningTextColour(_activity);
			_activityObject.GetComponent<Button>().onClick.AddListener(() => OnActivityButtonPressed(_activityObject));
			_activityObject.transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnCompleteActivityPressed(_activityObject, _gameController));
			_activityObject.transform.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnDeleteActivityPressed(_activityObject, _gameController));
		}
	}

	/// <summary>
	/// Reloads the Activities list.
	/// </summary>
	/// <param name="_gameController"></param>
	private void ReloadActivities(GameManager_FSM _gameController)
	{
		for (int i = 0; i < _gameController.ActivityItemContainer.childCount; i++)
		{
			UnityEngine.Object.Destroy(_gameController.ActivityItemContainer.GetChild(i).gameObject);
		}

		_gameController.Activities = LoadActivities();
		PopulateActivityList(_gameController);
	}

	/// <summary>
	/// Sets the colour of the activity warning text based on how close the due date is.
	/// </summary>
	/// <param name="_activity"></param>
	/// <returns></returns>
	private Color SetActivityWarningTextColour(Activity _activity)
	{
		Color _warningColour = Color.black;
		int _differenceInDays = CompareDueDateToNow(_activity);

		if (_differenceInDays > 3)
		{
			_warningColour = Color.green;
		}
		else if (_differenceInDays > 1)
		{
			_warningColour = new Color32(255, 186, 0, 255);
		}
		else
		{
			_warningColour = Color.red;
		}

		return _warningColour;
	}

	/// <summary>
	/// Sets the activit warning based on how close the due date is.
	/// </summary>
	/// <param name="_activity"></param>
	/// <returns></returns>
	private string SetActivityWarningText(Activity _activity)
	{
		string _warningText = "";
		int _differenceInDays = CompareDueDateToNow(_activity);

		if (_differenceInDays > 0)
		{
			if (_differenceInDays == 1)
			{
				_warningText = _differenceInDays + " Day Remaining!";
			}
			else
			{
				_warningText = _differenceInDays + " Days Remaining!";
			}

		}
		else if (_differenceInDays == 0)
		{
			_warningText = "Due Today!!";
		}
		else if (_differenceInDays < 0)
		{
			if (_differenceInDays == -1)
			{
				_warningText = (_differenceInDays * -1) + " Day Overdue!";
			}
			else
			{
				_warningText = (_differenceInDays * -1) + " Days Overdue!";
			}
		}

		return _warningText;
	}

	/// <summary>
	/// Sorts activities based on selected sort method.
	/// </summary>
	/// <param name="_gameController"></param>
	private void SortActivities(GameManager_FSM _gameController)
	{
		ActivitySort _sortType = (ActivitySort)activitySortDropdown.value;

		for (int i = 0; i < _gameController.ActivityItemContainer.childCount; i++)
		{
			UnityEngine.Object.Destroy(_gameController.ActivityItemContainer.GetChild(i).gameObject);
		}

		_gameController.Activities = LoadActivities();

		switch (_sortType)
		{
			case ActivitySort.ActivityTitleDescending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => activity.ActivityTitle).ToList();
				break;
			case ActivitySort.ActivityTitleAscending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => activity.ActivityTitle).ToList();
				_gameController.Activities.Reverse();
				break;
			case ActivitySort.DueDateDescending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => DateTime.Parse(activity.DueDate)).ToList();
				break;
			case ActivitySort.DueDateAscending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => DateTime.Parse(activity.DueDate)).ToList();
				_gameController.Activities.Reverse();
				break;
			case ActivitySort.PriorityDescending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => activity.Priority).ToList();
				break;
			case ActivitySort.PriorityAscending:
				_gameController.Activities = _gameController.Activities.OrderBy(activity => activity.Priority).ToList();
				_gameController.Activities.Reverse();
				break;
			default:
				break;
		}

		PopulateActivityList(_gameController);
	}

	#endregion

	#region Private comparison functions

	/// <summary>
	/// Calculates whether the due date is before, during or after today's date.
	/// </summary>
	/// <param name="_activity"></param>
	/// <returns></returns>
	private int CompareDueDateToNow(Activity _activity)
	{
		DateTime _today = DateTime.Now;
		DateTime _dueDate = DateTime.Parse(_activity.DueDate);
		TimeSpan _compareDays = _dueDate - _today;
		int _differenceInDays = Mathf.CeilToInt((float)_compareDays.TotalDays);

		return _differenceInDays;
	}

	/// <summary>
	/// Calculates whether the due date is before, during or after today's date.
	/// </summary>
	/// <param name="_activityDate"></param>
	/// <returns></returns>
	private int CompareDueDateToNow(string _activityDate)
	{
		DateTime _today = DateTime.Now;
		DateTime _dueDate = DateTime.Parse(_activityDate);
		TimeSpan _compareDays = _dueDate - _today;
		int _differenceInDays = Mathf.CeilToInt((float)_compareDays.TotalDays);

		return _differenceInDays;
	}

	#endregion

	#region Private button functions

	/// <summary>
	/// Handles functionality for the Activity buttons.
	/// </summary>
	/// <param name="_activityObject"></param>
	private void OnActivityButtonPressed(GameObject _activityObject)
	{
		GameObject _detailsPanel = _activityObject.transform.GetChild(1).gameObject;
		GameObject _completePanel = _activityObject.transform.GetChild(2).gameObject;

		if (_detailsPanel.activeSelf)
		{
			_detailsPanel.SetActive(false);
			_completePanel.SetActive(true);
		}
		else if (_completePanel.activeSelf)
		{
			_completePanel.SetActive(false);
			_detailsPanel.SetActive(true);
		}
	}

	/// <summary>
	/// Handles functionality for the Complete button for each activity.
	/// </summary>
	/// <param name="_activityObject"></param>
	/// <param name="_gameController"></param>
	private void OnCompleteActivityPressed(GameObject _activityObject, GameManager_FSM _gameController)
	{
		int _differenceInDays = CompareDueDateToNow(_activityObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text.Substring(9));

		if (_differenceInDays >= 0)
		{
			switch (_activityObject.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text)
			{
				case "Priority: Low":
					_gameController.ActivityCompleteLow.Play();
					break;
				case "Priority: Medium":
					_gameController.ActivityCompleteMedium.Play();
					break;
				case "Priority: High":
					_gameController.ActivityCompleteHigh.Play();
					break;
				default:
					break;
			}

			_gameController.CurrentSoldiers++;
		}
		else
		{
			_gameController.InsufficientFunds.Play();
		}

		string _activityTitle = _activityObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;

		File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities" + Path.DirectorySeparatorChar + _activityTitle + ".txt");

		ReloadActivities(_gameController);

		_gameController.TimeLeftToSave = 0.5f;
	}

	/// <summary>
	/// Handles functionality for the Delete button for each activity.
	/// </summary>
	/// <param name="_activityObject"></param>
	/// <param name="_gameController"></param>
	private void OnDeleteActivityPressed(GameObject _activityObject, GameManager_FSM _gameController)
	{
		_gameController.ButtonTap.Play();

		string _activityTitle = _activityObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;

		File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "Activities" + Path.DirectorySeparatorChar + _activityTitle + ".txt");

		ReloadActivities(_gameController);
	}

	#endregion
}