using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace BeatThat
{
	/// <summary>
	/// Access to animator params with workarounds for a few animator issues:
	/// 
	/// 1) if Set<T> is call before animator is ready, stores the param/value and then sets it on activate
	/// 2) when animator is disabled/reenable, restores all parameter values
	/// 
	/// NOTE: in order for the restore function above to work, all Set<T> calls to the Animator must be made through this class
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimatorParamsSurviveDisable : MonoBehaviour, StateController
	{
		public void ClearTrigger (string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions reqOpts = StateParamOptions.RequireParam)
		{
			throw new NotImplementedException ();
		}

		public UnityEvent<StateParam> setParamCalled {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public UnityEvent<StateParamUpdate> GetParamUpdated (string param)
		{
			throw new System.NotImplementedException ();
		}

		public bool m_debug;

		#region StateParams implementation

		class StateParamEvent : UnityEvent<StateParamUpdate> {}
		public UnityEvent<StateParamUpdate> paramUpdated { get { return m_paramUpdated?? (m_paramUpdated = new StateParamEvent()); } }
		private UnityEvent<StateParamUpdate> m_paramUpdated;

		public bool isReady { get; private set; }
		private bool transferParamsPending { get; set; }

		void OnEnable()
		{
			this.transferParamsPending = true;
		}

		void Update()
		{
			if(!this.transferParamsPending) {
				return;
			}

			if(!this.animator.isActiveAndEnabled) {
				return;
			}

			TransferParams();

			this.transferParamsPending = false;
			this.isReady = true;
		}
			
		void OnDestroy()
		{
			ReleaseArrays();
		}

		public int GetInt (string name)
		{
			int v;
			return Get<int> (name, out v, m_ints) ? v : this.animator.GetInteger (name);
		}

		public void SetInt (string name, int value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions opts = StateParamOptions.RequireParam)
		{
			if(!HasParam(name)) {
				if(opts == StateParamOptions.RequireParam || m_debug) {
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
				}
				return;
			}

			if(GetInt(name) == value) {
				return;
			}

			Set<int>(name, value, ref m_ints);

			if(this.isReady) {
				this.animator.SetInteger(name, value);
			}

			if(m_paramUpdated != null) {
				m_paramUpdated.Invoke(new StateParamUpdate(name, StateParamUpdateType.Int));
			}
		}

		public float GetFloat (string name)
		{
			float v;
			return Get<float> (name, out v, m_floats) ? v : this.animator.GetFloat (name);
		}

		public void SetFloat (string name, float value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions opts = StateParamOptions.RequireParam)
		{
			if(!HasParam(name)) {
				if(opts == StateParamOptions.RequireParam || m_debug) {
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
				}
				return;
			}

			if(Mathf.Approximately(GetFloat(name), value)) {
				return;
			}

			Set<float>(name, value, ref m_floats);

			if(this.isReady) {
				this.animator.SetFloat(name, value);
			}

			if(m_paramUpdated != null) {
				m_paramUpdated.Invoke(new StateParamUpdate(name, StateParamUpdateType.Float));
			}
		}

		public Func<bool> GetterForBool(string name)
		{
			int hash = Animator.StringToHash(name);
			return () => this.animator.GetBool(hash); // for a call that may be used a lot, would be nice to avoid allocating closure, but not sure possible
		}

		public Action<bool> SetterForBool(string name)
		{
			int hash = Animator.StringToHash(name);
			return (val) => this.animator.SetBool(hash, val); // for a call that may be used a lot, would be nice to avoid allocating closure, but not sure possible
		}

		public bool GetBool (string name)
		{
			bool v;
			return Get<bool> (name, out v, m_bools) ? v : this.animator.GetBool (name);
		}

		public void SetBool (string name, bool value, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions opts = StateParamOptions.RequireParam)
		{
			if(!HasParam(name)) {
				if(opts == StateParamOptions.RequireParam || m_debug) {
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
				}
				return;
			}

			if(GetBool(name) == value) {
				return;
			}

			Set<bool>(name, value, ref m_bools);

			if(this.isReady) {
				this.animator.SetBool(name, value);
			}

			if(m_paramUpdated != null) {
				m_paramUpdated.Invoke(new StateParamUpdate(name, StateParamUpdateType.Bool));
			}
		}

		public void SetTrigger(string name, PropertyEventOptions eventOpts = PropertyEventOptions.SendOnChange, StateParamOptions opts = StateParamOptions.RequireParam)
		{
			if(!HasParam(name)) {
				if(opts == StateParamOptions.RequireParam || m_debug) {
					Debug.LogError("[" + Time.frameCount + "][" + this.Path() + "] animator does not have param '" + name + "'");
				}
				return;
			}

			if(this.animator.GetBool(name)) {
				return;
			}

			this.animator.SetTrigger(name);
			if(m_paramUpdated != null) {
				m_paramUpdated.Invoke(new StateParamUpdate(name, StateParamUpdateType.TriggerSet));
			}
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

		private bool HasParam(string name)
		{
			foreach(var p in this.animator.parameters) {
				if(p.name == name) {
					return true;
				}
			}

			return false;
		}

		private void TransferParams()
		{
			var a = this.animator;

			if(m_ints != null) {
				foreach(var i in m_ints) {
					a.SetInteger(i.name, i.value);
				}
			}

			if(m_floats != null) {
				foreach(var i in m_floats) {
					a.SetFloat(i.name, i.value);
				}
			}

			if(m_bools != null) {
				foreach(var i in m_bools) {
					a.SetBool(i.name, i.value);
				}
			}
		}

		private static bool Get<T>(string name, out T value, List<Param<T>> list)
		{
			if(list == null) {
				value = default(T);
				return false;
			}

			for(int i = 0; i < list.Count; i++) {
				if(list[i].name == name) {
					value = list[i].value;
					return true;
				}
			}

			value = default(T);
			return false;
		}

		private static int Set<T>(string name, T value, ref ListPoolList<Param<T>> list)
		{
			if(list == null) {
				list = ListPool<Param<T>>.Get();
			}

			for(int i = 0; i < list.Count; i++) {
				if(list[i].name == name) {
					list[i] = new Param<T>(name, value);
					return i;
				}
			}

			list.Add(new Param<T>(name, value));
			return list.Count - 1;
		}

		public struct Param<T>
		{
			public Param(string name, T value) 
			{
				this.name = name;
				this.value = value;
			}

			public string name { get; private set; }
			public T value { get; private set; }

			override public string ToString() { return "[name=" + this.name + ", value=" + this.value + "]"; }
		}

		private void ReleaseArrays()
		{
			if(m_debug) { Debug.Log("[" + Time.frameCount + "][" + this.Path() + "]::ReleaseArrays"); }

			if(m_ints != null) {
				m_ints.Dispose();
				m_ints = null;
			}

			if(m_floats != null) {
				m_floats.Dispose();
				m_floats = null;
			}
			if(m_floats != null) {
				m_floats.Dispose();
				m_floats = null;
			}
		}
			
		public Animator animator { get { return m_animator?? (m_animator = GetComponent<Animator>()); } }
		private Animator m_animator;

		private ListPoolList<Param<int>> m_ints;
		private ListPoolList<Param<float>> m_floats;
		private ListPoolList<Param<bool>> m_bools;

	}
}
