using System;


namespace BeatThat
{
	/// <summary>
	/// A parameter has a 'param' name
	/// </summary>
	public interface Param 
	{
		string param { get; }
	}

	public static class ParamExt
	{
		public static string DefaultParamName(this Param p)
		{
			var t = p.GetType().Name;
			return Char.ToLower(t[0]) + t.Substring(1);
		}
	}
}