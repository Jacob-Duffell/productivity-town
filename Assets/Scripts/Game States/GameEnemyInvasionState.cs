using TMPro;
using UnityEngine;

/// <summary>
/// Handles functionality for the Enemy Invasion state.
/// </summary>
public class GameEnemyInvasionState : GameBaseState
{
	private TMP_Text soldiersEarnedText;
	private TMP_Text buildingsDestroyedText;
	private TMP_Text bonusText;

	private int buildingsDestroyed;
	private int bonusMoney;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.BackgroundMusic.Stop();

		_gameController.ViewStateCanvas.gameObject.SetActive(false);
		_gameController.EnemyInvasionCanvas.gameObject.SetActive(true);

		soldiersEarnedText = _gameController.EnemyInvasionMenu.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
		buildingsDestroyedText = _gameController.EnemyInvasionMenu.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
		bonusText = _gameController.EnemyInvasionMenu.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();

		buildingsDestroyed = 0;
		bonusMoney = 0;

		if (_gameController.CurrentSoldiers < _gameController.SoldiersNeeded)
		{
			_gameController.EnemyInvasionBad.Play();
		}
		else
		{
			_gameController.EnemyInvasionGood.Play();
		}

		soldiersEarnedText.text = "Soldiers Earned: " + _gameController.CurrentSoldiers + "/" + _gameController.SoldiersNeeded;

		if (_gameController.CurrentSoldiers < _gameController.SoldiersNeeded)
		{
			float _soldiersMissingPercentage = 1 - ((float)_gameController.CurrentSoldiers / _gameController.SoldiersNeeded);

			buildingsDestroyed = Mathf.FloorToInt(_soldiersMissingPercentage * SaveData.PlacedBuildings.Count);

			if (buildingsDestroyed == SaveData.PlacedBuildings.Count) // Prevents the user from losing all their buildings.
			{
				buildingsDestroyed = SaveData.PlacedBuildings.Count - 1;
			}
			else if (buildingsDestroyed == 0) // If the user didn't meet their target, but the buildings destroyed formula results in 0
			{
				buildingsDestroyed = 1;
			}
			
			if (buildingsDestroyed < 0)	// If the amount of buildings destroyed is negative for some reason
			{
				buildingsDestroyed = 0;
			}
		}

		buildingsDestroyedText.text = "Buildings Destroyed: " + buildingsDestroyed;

		if (_gameController.CurrentSoldiers > _gameController.SoldiersNeeded)
		{
			float _soldiersOveragePercentage = (float)_gameController.SoldiersNeeded / _gameController.CurrentSoldiers;
			bonusMoney =  Mathf.CeilToInt(_gameController.CurrentMoney / (_soldiersOveragePercentage * _gameController.SoldiersNeeded));

			Debug.Log("Soldiers overage percentage = " + _soldiersOveragePercentage);
			Debug.Log("Current money = " + _gameController.CurrentMoney);
			Debug.Log("Bonus money = " + bonusMoney);
		}

		bonusText.text = "Bonus: £" + bonusMoney;
	}

	public override void Update(GameManager_FSM _gameController)
	{
		// Not needed.
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch(_buttonTag)
		{
			case ButtonTag.StartNewWeek:
				StartNewWeek(_gameController);
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Sets the new soldier target and begins the countdown to the enemy invasion.
	/// </summary>
	/// <param name="_gameController"></param>
	private void StartNewWeek(GameManager_FSM _gameController)
	{
		_gameController.BackgroundMusic.Play();

		for (int i = 0; i < buildingsDestroyed; i++)
		{
			int _randomBuildingIndex = Random.Range(0, _gameController.CurrentBuildings.Count - 1);

			Object.Destroy(_gameController.CurrentBuildings[_randomBuildingIndex].GetComponent<BuildingController>());
			Object.Destroy(_gameController.CurrentBuildings[_randomBuildingIndex].gameObject);

			_gameController.CurrentBuildings.RemoveAt(_randomBuildingIndex);
			SaveData.PlacedBuildings.RemoveAt(_randomBuildingIndex);
		}

		for (int j = 0; j < _gameController.CurrentBuildings.Count; j++)
		{
			_gameController.CurrentBuildings[j].GetComponent<BuildingController>().ThisBuildingIndex = j;
		}

		_gameController.CurrentMoney += bonusMoney;
		_gameController.CurrentSoldiers = 0;

		_gameController.TimeLeftToSave = 0.5f;

		_gameController.EnemyInvasionCanvas.gameObject.SetActive(false);
		_gameController.TransitionToState(_gameController.SetDeadlineState);
	}

	#endregion
}
