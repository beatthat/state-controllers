using UnityEngine;
using System;

namespace BeatThat
{
	/// <summary>
	/// dispatched when a state param is updated
	/// </summary>
	public struct StateParamUpdate
	{
		public StateParamUpdate(string name, StateParamUpdateType type)
		{
			this.name = name;
			this.type = type;
		}

//		public static StateParam FromAnimatorControllerParam(AnimatorControllerParameter p)
//		{
//			StateParamType type;
//			switch(p.type) {
//			case AnimatorControllerParameterType.Bool:
//				type = StateParamType.Bool;
//				break;
//			case AnimatorControllerParameterType.Float:
//				type = StateParamType.Float;
//				break;
//			case AnimatorControllerParameterType.Int:
//				type = StateParamType.Int;
//				break;
//			case AnimatorControllerParameterType.Trigger:
//				type = StateParamType.Trigger;
//				break;
//			default: 
//				throw new ArgumentException("Invalid parameter type: " + p.type);
//			}
//
//			return new StateParam(p.name, type);
//		}


		public string name { get; private set; }
		public StateParamUpdateType type { get; private set; }

		override public string ToString() { return "[StateParamUpdate name=" + this.name + ", type=" + this.type + "]"; }
	}
}