using BeatThat.GetComponentsExt;
using BeatThat.CollectionsExt;
using BeatThat.Controllers;
using BeatThat.Bindings;
using BeatThat.Properties;
using UnityEngine;

namespace BeatThat.StateControllers
{
	public class AnimatorControllerBehaviour : AnimatorControllerBehaviour<Animator> {}

	public class AnimatorControllerBehaviour<ControllerType> : BindingStateBehaviour<ControllerType> where ControllerType : class
	{
		protected StateController state { get; private set; }	

		sealed override protected void BindState() 
		{
			this.state = animator.AddIfMissing<StateController, AnimatorController>();
			BindControllerState();
		}

		sealed override protected void UnbindState() 
		{
			this.state = null;
			UnbindControllerState();
		}

		virtual protected void UnbindControllerState() {}
		virtual protected void BindControllerState() {}

		public void SetBool<T>(bool value, MissingComponentOptions opts = MissingComponentOptions.AddAndWarn) where T : Component, IHasBool
		{
			if(this.animator == null) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::SetBool<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return;
			}

			this.animator.SetBool<T>(value, opts);
		}

		public bool GetBool<T>(MissingComponentOptions opts = MissingComponentOptions.AddAndWarn, bool dftVal = false) where T : Component, IHasBool
		{
			if(this.animator == null) {

#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::GetBool<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return dftVal;
			}

			return this.animator.GetBool<T>(opts, dftVal);
		}

		public void SetInt<T>(int value, MissingComponentOptions opts = MissingComponentOptions.AddAndWarn) where T : Component, IHasInt
		{
            if(this.animator == null) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::SetInt<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return;
			}

			this.animator.SetInt<T>(value, opts);
		}

		public int GetInt<T>(MissingComponentOptions opts = MissingComponentOptions.AddAndWarn, int dftVal = 0) where T : Component, IHasInt
		{
            if(this.animator == null) {
#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::GetInt<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return dftVal;
			}

			return this.animator.GetInt<T>(opts, dftVal);
		}

		public void SetFloat<T>(float value, MissingComponentOptions opts = MissingComponentOptions.AddAndWarn) where T : Component, IHasFloat
		{
			if(this.animator == null) {

#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::SetFloat<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return;
			}

			this.animator.SetFloat<T>(value, opts);
		}

		public float GetFloat<T>(MissingComponentOptions opts = MissingComponentOptions.AddAndWarn, float dftVal = 0) where T : Component, IHasFloat
		{
			if(this.animator == null) {

#if UNITY_EDITOR || DEBUG_UNSTRIP
                Debug.LogError("[" + Time.frameCount + "] " + GetType()
                    + "::GetFloat<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif

                return dftVal;
			}
			return this.animator.GetFloat<T>(opts, dftVal);
		}

		public void Invoke<T>() where T : Component, Invocable
		{
			if(this.animator == null) {

#if UNITY_EDITOR || DEBUG_UNSTRIP
				Debug.LogError("[" + Time.frameCount + "] " + GetType() 
					+ "::Invoke<" + typeof(T) + "> called when animator is not set (maybe from WillEnter)?");
#endif
                return;
			}
			this.animator.Invoke<T>();
		}
	}

	public class AnimatorControllerBehaviour<PresenterType, ModelType> : AnimatorControllerBehaviour<PresenterType>
		where PresenterType : class, IController<ModelType>
		where ModelType : class
	{
		protected ModelType model  { get { return this.controller != null? this.controller.model: null; } }
	}
}




