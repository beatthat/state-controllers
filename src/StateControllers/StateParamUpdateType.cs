namespace BeatThat
{
	/// <summary>
	/// On state param updates, particularly in the case of a trigger that has cleared, better to have an explicit TriggerClear instead of hack Bool = false 
	/// </summary>
	public enum StateParamUpdateType 
	{
		Float = 0, Int = 1, Bool = 2, TriggerSet = 3, TriggerClear = 4
	}
}