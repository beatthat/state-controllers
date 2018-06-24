using BeatThat.Controllers;
using BeatThat.Properties;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace BeatThat.StateControllers
{
	public enum StateParamOptions { RequireParam = 0, DontRequireParam = 1 }

	/// <summary>
	/// Controller for set of params that drive a state machine.
	/// Used first and foremost as a wrapper for mechanim Animator components which have various problems
	/// when used as a state-machine solution.
	/// 
	/// First among the mechanim issues is that Animators lose their state when (their GameObjects are) disabled.
	/// Using a wrapper allows for some mitigation of this issue.
	/// 
	/// Beyond the disabled issue above, abstracting away the concrete Animator from controllers,
	/// removes at least one stumbling block from migrating to an alternate FSM solution in the future.
	/// </summary>
	public interface StateController 
	{
		UnityEvent<StateParamUpdate> paramUpdated { get; }

		/// <summary>
		/// An update event that fires just for the specified param
		/// </summary>
		UnityEvent<StateParamUpdate> GetParamUpdated(string param);

		/// <summary>
		/// True when the underlying statemachine is ready to get/set params.
		/// </summary>
		bool isReady { get; }

		int GetInt(string name);
		void SetInt(string name, int value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam);

		float GetFloat(string name);
		void SetFloat(string name, float value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam);

		Func<bool> GetterForBool(string name);
		Action<bool> SetterForBool(string name);

		bool GetBool(string name);
		void SetBool(string name, bool value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam);

		void SetTrigger(string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam);

		void ClearTrigger(string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam);

//		bool HasParam (string name);

		void GetParams(ICollection<StateParam> results);

		void GetParamNames(ICollection<string> results);
	}

	namespace StateControllerExtensions
	{
//		public static class StateControllerExt
//		{
//			/// <summary>
//			/// Add a param type to a GameObject if
//			/// </summary>
//			/// <typeparam name="T">The 1st type parameter.</typeparam>
//			public static bool SetIfSupported<ParamType, ValueType>(this GameObject go, ValueType value) where ParamType : Component, Param, IHasValue<ValueType>
//			{
//				var p = go.GetComponent<ParamType> ();
//				if (p != null) {
//					p.value = value;
//					return true;
//				}
//
//				var stateController = go.GetComponent<StateController> ();
//				if (stateController == null) {
//					return false;
//				}
//
//				return true;
//			}
//		}
	}
}


