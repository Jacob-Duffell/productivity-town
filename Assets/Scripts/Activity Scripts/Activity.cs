using System;
using UnityEngine;

/// <summary>
/// Contains data for an activity.
/// </summary>
[CreateAssetMenu(fileName = "New Activity", menuName = "Activity", order = 1)]
public class Activity : ScriptableObject
{
    [SerializeField] private string activityTitle;
    [SerializeField] private string dueDate;
    [SerializeField] private ActivityPriority priority;

    public int GetPriorityEnumLength()
    {
        return Enum.GetValues(typeof(ActivityPriority)).Length;
    }

    public string GetActivityPriorityName(int index)
    {
        return Enum.GetName(typeof(ActivityPriority), index);
    }

	#region Encapsulated variables

	public string ActivityTitle { get => activityTitle; set => activityTitle = value; }
    public string DueDate { get => dueDate; set => dueDate = value; }
    public ActivityPriority Priority { get => priority; set => priority = value; }

	#endregion
}
