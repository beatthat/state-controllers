using System;
using UnityEngine;


namespace BeatThat
{
	/// <summary>
	/// A parameter has a 'param' name
	/// </summary>
	public interface Param 
	{
		string param { get; }

		Type paramType { get; }
	}

	public static class ParamExt
	{
		public static string DefaultParamName(this Param p)
		{
			return DefaultParamName (p.GetType ());
		}

		public static string DefaultParamName<T>() where T : Param
		{
			return DefaultParamName (typeof(T));
		}

		public static string DefaultParamName(Type type)
		{
			var t = type.Name;
			return Char.ToLower(t[0]) + t.Substring(1);
		}


		public static string DefaultParamNameRemovingSuffixes(this Param p, params string[] suffixes)
		{
			return DefaultParamNameRemovingSuffixes (p.GetType (), suffixes);
		}

		public static string DefaultParamNameRemovingSuffixes<T>(params string[] suffixes)
		{
			return DefaultParamNameRemovingSuffixes (typeof(T), suffixes);
		}

		public static string DefaultParamNameRemovingSuffixes(Type type, params string[] suffixes)
		{
			var t = type.Name;
			var n = Char.ToLower(t[0]) + t.Substring(1);
			foreach(var s in suffixes) { 
				if (n.EndsWith (s)) {
					n = n.Substring (0, n.Length - s.Length);
				}
			}
			return n;
		}
	}
}