using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles functionality for the shop menu state.
/// </summary>
public class GameShopMenuState : GameBaseState
{
	private bool buildingListActive;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.ViewStateCanvas.gameObject.SetActive(true);
		_gameController.ShopMenu.SetActive(true);

		buildingListActive = true;

		PopulateBuildingShopList(_gameController);
	}

	public override void Update(GameManager_FSM _gameController)
	{
		// No code needed here at the moment.
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.Activities:
				_gameController.ShopMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ActivityMenuState);
				break;
			case ButtonTag.Shop:
				_gameController.ShopMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ViewState);
				break;
			case ButtonTag.Settings:
				_gameController.ShopMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.SettingsMenuState);
				break;
			case ButtonTag.BuildingsTab:
				if (!buildingListActive)
				{
					buildingListActive = true;
					PopulateBuildingShopList(_gameController);
				}
				break;
			case ButtonTag.RoadsTab:
				if (buildingListActive)
				{
					buildingListActive = false;
					PopulateRoadShopList(_gameController);
				}
				break;
			default:
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Handles functionality for the Purchase button of each Building shop item.
	/// </summary>
	/// <param name="_gameController"></param>
	/// <param name="_building"></param>
	private void OnPurchaseButtonPressed(GameManager_FSM _gameController, Building _building)
	{
		if (_gameController.CurrentMoney >= _building.PurchasePrice)
		{
			_gameController.ButtonTap.Play();

			_gameController.SelectedBuilding = _building;
			_gameController.ShopMenu.SetActive(false);
			_gameController.TransitionToState(_gameController.BuildState);
		}
		else
		{
			_gameController.InsufficientFunds.Play();
		}
	}

	/// <summary>
	/// Handles functionality for the Purchase button of each Road shop item.
	/// </summary>
	/// <param name="_gameController"></param>
	/// <param name="_road"></param>
	private void OnPurchaseButtonPressed(GameManager_FSM _gameController, Road _road)
	{
		if (_gameController.CurrentMoney >= _road.PurchasePrice)
		{
			_gameController.ButtonTap.Play();

			_gameController.SelectedRoad = _road;
			_gameController.ShopMenu.SetActive(false);
			_gameController.TransitionToState(_gameController.BuildState);
		}
		else
		{
			_gameController.InsufficientFunds.Play();
		}
	}

	/// <summary>
	/// Populates shop list with Building items.
	/// </summary>
	/// <param name="_gameController"></param>
	private void PopulateBuildingShopList(GameManager_FSM _gameController)
	{
		for (int i = 0; i < _gameController.ShopItemContainer.childCount; i++)
		{
			Object.Destroy(_gameController.ShopItemContainer.GetChild(i).gameObject);
		}

		for (int i = 0; i < _gameController.Buildings.Count; i++)
		{
			Building _building = _gameController.Buildings[i];
			GameObject _shopObject = Object.Instantiate(_gameController.ShopItemPrefab, _gameController.ShopItemContainer);

			_shopObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _building.BuildingName;
			_shopObject.transform.GetChild(1).GetChild(2).GetComponent<Image>().sprite = _building.BuildingIcon;
			_shopObject.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnPurchaseButtonPressed(_gameController, _building));
			_shopObject.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "£" + _building.PurchasePrice;
		}
	}

	/// <summary>
	/// Populates shop list with Road items.
	/// </summary>
	/// <param name="_gameController"></param>
	private void PopulateRoadShopList(GameManager_FSM _gameController)
	{
		for (int i = 0; i < _gameController.ShopItemContainer.childCount; i++)
		{
			Object.Destroy(_gameController.ShopItemContainer.GetChild(i).gameObject);
		}

		for (int i = 0; i < _gameController.Roads.Count; i++)
		{
			Road _road = _gameController.Roads[i];
			GameObject _shopObject = Object.Instantiate(_gameController.ShopItemPrefab, _gameController.ShopItemContainer);

			_shopObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _road.RoadName;
			_shopObject.transform.GetChild(1).GetChild(2).GetComponent<Image>().sprite = _road.RoadIcon;
			_shopObject.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnPurchaseButtonPressed(_gameController, _road));
			_shopObject.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "£" + _road.PurchasePrice;
		}
	}

#endregion
}
