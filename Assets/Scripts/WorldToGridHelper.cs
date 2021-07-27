using UnityEngine;

/// <summary>
/// Helper class for calculating user touch inputs.
/// </summary>
public static class WorldToGridHelper
{
	/// <summary>
	/// Calculates plane position where user is touching.
	/// </summary>
	/// <param name="_cam"></param>
	/// <param name="_plane"></param>
	/// <param name="_screenPos"></param>
	/// <returns></returns>
	public static Vector3 PlanePosition(Camera _cam, Plane _plane, Vector2 _screenPos)
	{
		// Calculate plane position
		var _rayNow = _cam.ScreenPointToRay(_screenPos);

		if (_plane.Raycast(_rayNow, out var _enterNow))
		{
			return _rayNow.GetPoint(_enterNow);
		}

		// If not on plane
		return Vector3.zero;
	}

	/// <summary>
	/// Calculates delta plane position.
	/// </summary>
	/// <param name="_cam"></param>
	/// <param name="_plane"></param>
	/// <param name="_touch"></param>
	/// <returns></returns>
	public static Vector3 PlanePositionDelta(Camera _cam, Plane _plane, Touch _touch)
	{
		// If the user's finger has not moved
		if (_touch.phase != TouchPhase.Moved)
		{
			return Vector3.zero;
		}

		// Calculate delta position
		var _rayBefore = _cam.ScreenPointToRay(_touch.position - _touch.deltaPosition);
		var _rayNow = _cam.ScreenPointToRay(_touch.position);

		if (_plane.Raycast(_rayBefore, out var _enterBefore) && _plane.Raycast(_rayNow, out var _enterNow))
		{
			return _rayBefore.GetPoint(_enterBefore) - _rayNow.GetPoint(_enterNow);
		}

		// If not on plane
		return Vector3.zero;
	}
}