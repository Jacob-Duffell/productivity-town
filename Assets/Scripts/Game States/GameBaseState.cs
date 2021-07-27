/// <summary>
/// Base class that all Game State classes inherit from.
/// </summary>
public abstract class GameBaseState
{
	/// <summary>
	/// Handles functionality for when the game has entered a new state.
	/// </summary>
	/// <param name="_gameController"></param>
	public abstract void EnterState(GameManager_FSM _gameController);

	/// <summary>
	/// Called in the core update function in the game manager.
	/// </summary>
	/// <param name="_gameController"></param>
	public abstract void Update(GameManager_FSM _gameController);

	/// <summary>
	/// Handles all button presses.
	/// </summary>
	/// <param name="_gameController"></param>
	/// <param name="_buttonTag"></param>
	public abstract void OnButtonPressed(GameManager_FSM _gameController, ButtonTag _buttonTag);
}
