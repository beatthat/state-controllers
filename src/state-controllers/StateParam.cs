using UnityEngine;
using System;

namespace BeatThat
{
	/// <summary>
	/// Direct copy of UnityEngine AnimatorControllerParam
	/// </summary>
	public struct StateParam 
	{
		public StateParam(string name, StateParamType type)
		{
			this.name = name;
			this.type = type;
		}

		public static StateParam FromAnimatorControllerParam(AnimatorControllerParameter p)
		{
			StateParamType type;
			switch(p.type) {
			case AnimatorControllerParameterType.Bool:
				type = StateParamType.Bool;
				break;
			case AnimatorControllerParameterType.Float:
				type = StateParamType.Float;
				break;
			case AnimatorControllerParameterType.Int:
				type = StateParamType.Int;
				break;
			case AnimatorControllerParameterType.Trigger:
				type = StateParamType.Trigger;
				break;
			default: 
				throw new ArgumentException("Invalid parameter type: " + p.type);
			}

			return new StateParam(p.name, type);
		}


		public string name { get; private set; }
		public StateParamType type { get; private set; }

		override public string ToString() { return "[StateParam name=" + this.name + ", type=" + this.type + "]"; }
	}
}