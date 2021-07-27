using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles functionality for the Settings Menu state.
/// </summary>
public class GameSettingsMenuState : GameBaseState
{
	private Toggle audioToggle;

	#region Inherited functions

	public override void EnterState(GameManager_FSM _gameController)
	{
		_gameController.SettingsMenu.SetActive(true);

		audioToggle = _gameController.SettingsMenu.transform.GetChild(1).GetChild(0).GetComponent<Toggle>();

		if (SaveData.SoundOn == false)
		{
			audioToggle.isOn = true;
		}
		else
		{
			audioToggle.isOn = false;
		}
	}

	public override void Update(GameManager_FSM _gameController)
	{
		// Not needed.
	}

	public override void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag)
	{
		switch (_buttonTag)
		{
			case ButtonTag.Settings:
				_gameController.SettingsMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ViewState);
				break;
			case ButtonTag.Activities:
				_gameController.SettingsMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ActivityMenuState);
				break;
			case ButtonTag.Shop:
				_gameController.SettingsMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.ShopMenuState);
				break;
			case ButtonTag.ToggleSound:
				ToggleSound();
				_gameController.SaveGame();
				break;
			case ButtonTag.OpenHelpMenu:
				_gameController.SettingsMenu.SetActive(false);
				_gameController.TransitionToState(_gameController.HelpMenuState);
				break;
			default: 
				break;
		}
	}

	#endregion

	#region Private functions

	/// <summary>
	/// Mutes or unmutes sound based on the status of the audio toggle.
	/// </summary>
	private void ToggleSound()
	{
		if (audioToggle.isOn == true)
		{
			SaveData.SoundOn = false;
			AudioListener.volume = 0.0f;
		}
		else
		{
			SaveData.SoundOn = true;
			AudioListener.volume = 1.0f;
		}
	}

	#endregion
}
