using UnityEngine;

/// <summary>
/// Handles collision and colour swapping for Preview prefabs.
/// </summary>
public class PreviewObject : MonoBehaviour
{
    [SerializeField] private Material badMaterial;
    [SerializeField] private Material goodMaterial;

    private bool canBuild;

    private void Start()
    {
        canBuild = true;

        ChangeMaterial();
    }

	#region Collision functions

	/// <summary>
	/// Handles collision with already-placed buildings.
	/// </summary>
	/// <param name="_other"></param>
	private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Building"))
        {
            canBuild = false;
            ChangeMaterial();
        }
    }

    /// <summary>
    /// Handles collision with already-placed buildings.
    /// </summary>
    /// <param name="_other"></param>
    private void OnTriggerStay(Collider _other)
	{
        if (_other.CompareTag("Building"))
        {
            canBuild = false;
            ChangeMaterial();
        }
    }

    /// <summary>
    /// Handles collision with already-placed buildings.
    /// </summary>
    /// <param name="_other"></param>
	private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Building"))
        {
            canBuild = true;
            ChangeMaterial();
        }
    }

	#endregion

	#region Private functions

	/// <summary>
	/// Changes the material of the prefab based on whether it is colliding with an object or not.
	/// </summary>
	private void ChangeMaterial()
	{
        int _childCount = transform.childCount;

        if (canBuild)
		{
            if (_childCount == 0)
			{
                GetComponent<MeshRenderer>().material = goodMaterial;
			}
            else
			{
                for (int i = 0; i < _childCount; i++)
				{
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = goodMaterial;
				}
			}
        }
        else
		{
            if (_childCount == 0)
            {
                GetComponent<MeshRenderer>().material = badMaterial;
            }
            else
            {
                for (int i = 0; i < _childCount; i++)
                {
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = badMaterial;
                }
            }
        } 
	}

	#endregion

	#region Encapsulated variables

	public bool CanBuild => canBuild;

	#endregion
}
