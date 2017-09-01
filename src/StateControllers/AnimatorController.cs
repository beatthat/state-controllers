using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace BeatThat
{
	/// <summary>
	/// Default StateParams impl for Animator.
	/// Just does a pass through of param values.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimatorController : MonoBehaviour, StateController
	{
		public bool m_debug;

		public string[] m_debugParams;
		#region StateParams implementation

		class StateParamEvent : UnityEvent<StateParamUpdate> {}

		/// <summary>
		/// Invoked when a param value changes
		/// </summary>
		/// <value>The parameter updated.</value>
		public UnityEvent<StateParamUpdate> paramUpdated { get { return m_paramUpdated?? (m_paramUpdated = new StateParamEvent()); } }
		private UnityEvent<StateParamUpdate> m_paramUpdated;

		public bool isReady { get { return this.animator.isInitialized; } }

		public UnityEvent<StateParamUpdate> GetParamUpdated(string param)
		{
			return GetParamUpdated(param, true);
		}

		private UnityEvent<StateParamUpdate> GetParamUpdated(string param, bool create)
		{
			if(m_updateEventsByParamName == null) {
				if(!create) {
					return null;
				}

				m_updateEventsByParamName = new Dictionary<string, UnityEvent<StateParamUpdate>>();
			}

			UnityEvent<StateParamUpdate> pEvent;
			if(!m_updateEventsByParamName.TryGetValue(param, out pEvent)) {
				if(!create) {
					return null;
				}

				pEvent = new StateParamEvent();	
				m_updateEventsByParamName[param] = pEvent;
			}

			return pEvent;
		}

		public int GetInt (string name)
		{
			return this.animator.GetInteger(name);
		}

		private bool IsDebugParam(string name)
		{
			if(m_debugParams == null || m_debugParams.Length == 0) {
				return false;
			}

			foreach(var p in m_debugParams) {
				if(p == name) {
					return true;
				}
			}

			return false;
		}

		public void SetInt (string name, int value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			if(!this.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetInt " + name + "->" + value + " is NOT ready");
				}
				#endif
				return;
			}

			if(!HasParam(name)) {
				if(reqOpts == StateParamOptions.RequireParam || m_debug) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					#endif
				}
				return;
			}

			if(GetInt(name) == value && eventOpts != PropertyEventOptions.Force) {
				return;
			}

			this.animator.SetInteger(name, value);

			if(eventOpts != PropertyEventOptions.Disable) {
				InvokeUpdated(name, StateParamUpdateType.Int);
			}
		}

		public float GetFloat (string name)
		{
			return this.animator.GetFloat(name);
		}

		public void SetFloat (string name, float value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			if(!this.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetFloat " + name + "->" + value + " is NOT ready");
				}
				#endif
				return;
			}

			if(!HasParam(name)) {
				if(reqOpts == StateParamOptions.RequireParam || m_debug) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
						Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					#endif
				}
				return;
			}

			if(Mathf.Approximately(GetFloat(name), value) && eventOpts != PropertyEventOptions.Force) {
				return;
			}

			this.animator.SetFloat(name, value);

			if(eventOpts != PropertyEventOptions.Disable) {
				InvokeUpdated(name, StateParamUpdateType.Float);
			}
		}

		public Func<bool> GetterForBool(string name)
		{
			int hash = Animator.StringToHash(name);
			return () => this.animator.GetBool(hash); // for a call that may be used a lot, would be nice to avoid allocating closure
		}

		public Action<bool> SetterForBool(string name)
		{
			int hash = Animator.StringToHash(name);

			// for a call that may be used a lot, would be nice to avoid allocating closure, but not sure possible

			return (val) => {

				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(IsDebugParam(name)) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set " + name + "->" + val + " (from " + this.animator.GetBool(hash) + ")");
				}
				#endif

				if(!this.isReady) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
					if(m_debug) {
						Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set " + name + "->" + val + " is NOT ready");
					}
					#endif
					return;
				}

				if(!HasParam(name)) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
					if(m_debug) {
						Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					}
					#endif
					return;
				}

				this.animator.SetBool(hash, val);

				InvokeUpdated(name, StateParamUpdateType.Bool);
			};
		}

		public bool GetBool (string name)
		{
			return this.animator.GetBool(name);
		}

		public void SetBool (string name, bool value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(IsDebugParam(name)) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetBool " + name + "->" + value + " (from " + GetBool(name) + ") eventsOpts=" + eventOpts);
			}
			#endif

			if(!this.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetBool " + name + "->" + value + " is NOT ready");
				}
				#endif
				return;
			}

			if(!HasParam(name)) {
				if(reqOpts == StateParamOptions.RequireParam || m_debug) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
						Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					#endif
				}
				return;
			}

			if(GetBool(name) == value && eventOpts != PropertyEventOptions.Force) {
				return;
			}

			this.animator.SetBool(name, value);

			if(eventOpts != PropertyEventOptions.Disable) {
				InvokeUpdated(name, StateParamUpdateType.Bool);
			}
		}

		public void SetTrigger(string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			if(!this.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetBool " + name + " is NOT ready");
				}
				#endif
				return;
			}

			if(!HasParam(name)) {
				if(reqOpts == StateParamOptions.RequireParam || m_debug) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					#endif
				}
				return;
			}

			if(GetBool(name) && eventOpts != PropertyEventOptions.Force) {
				return;
			}

			FireEventWhenTriggerClears(name);

			this.animator.SetTrigger(name);

			if(eventOpts != PropertyEventOptions.Disable) {
				InvokeUpdated(name, StateParamUpdateType.TriggerSet);
			}
		}

		public void ClearTrigger(string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			if(!this.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::SetBool " + name + " is NOT ready");
				}
				#endif
				return;
			}

			if(!HasParam(name)) {
				if(reqOpts == StateParamOptions.RequireParam || m_debug) {
					#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
					#endif
				}
				return;
			}

			if(!GetBool(name) && eventOpts != PropertyEventOptions.Force) {
				return;
			}
				
			this.animator.SetBool(name, false);

			if(eventOpts != PropertyEventOptions.Disable) {
				InvokeUpdated(name, StateParamUpdateType.TriggerClear);
			}
		}
			
		private void InvokeUpdated(string name, StateParamUpdateType type)
		{
			var pEvent = GetParamUpdated(name, false);
			if(pEvent != null) {
				pEvent.Invoke(new StateParamUpdate(name, type));
			}

			if(m_paramUpdated != null) {
				m_paramUpdated.Invoke(new StateParamUpdate(name, type));
			}
		}

		private bool HasParam(string name)
		{
			foreach(var p in this.animator.parameters) {
				if(p.name == name) {
					return true;
				}
			}

			return false;
		}

		public void GetParams(ICollection<StateParam> results)
		{
			foreach(var p in this.animator.parameters) {
				results.Add(StateParam.FromAnimatorControllerParam(p));
			}
		}

		public void GetParamNames(ICollection<string> results)
		{
			foreach(var p in this.animator.parameters) {
				results.Add(p.name);
			}
		}

		#endregion

		void OnDisable()
		{
			Cleanup();
		}

		void OnDestroy()
		{
			Cleanup();
		}

		private void Cleanup()
		{
			var activeTriggers = m_activeTriggers;
			m_activeTriggers = null;
			if(activeTriggers != null) {
				ListPool<string>.Return(activeTriggers);
			}
		}

		private void FireEventWhenTriggerClears(string name)
		{
			if(m_paramUpdated == null) {
				return; // no need to send event if noone is listening
			}

			if(m_activeTriggers == null) {
				m_activeTriggers = ListPool<string>.Get();
				m_activeTriggers.Add(name);
				this.enabled = true;
				return;
			}

			if(m_activeTriggers.Contains(name)) {
				return;
			}

			m_activeTriggers.Add(name);
		}

		void LateUpdate()
		{
			if(m_activeTriggers == null) {
				this.enabled = false;
				return;
			}

			for(int i = m_activeTriggers.Count - 1; i >= 0; i--) {
				var pname = m_activeTriggers[i];
				if(!GetBool(pname)) {
					m_activeTriggers.RemoveAt(i);
					if(m_paramUpdated != null) {
						m_paramUpdated.Invoke(new StateParamUpdate(pname, StateParamUpdateType.TriggerClear)); // notify trigger is cleared (now false)
					}
				}
			}

			if(m_activeTriggers.Count == 0) {
				ListPool<string>.Return(m_activeTriggers);
				m_activeTriggers = null;
				this.enabled = false;
			}
		}
			
		private Dictionary<string, UnityEvent<StateParamUpdate>> m_updateEventsByParamName;
		private ListPoolList<string> m_activeTriggers;

		public Animator animator { get { return m_animator?? (m_animator = GetComponent<Animator>()); } }
		private Animator m_animator;

	}
}
