using System;
using System.Reflection;
using UnityEngine;

namespace NodeDialog.Events
{
	/// <summary>
	/// Public-facing class for node events, our custom serialized event delegates.
	/// </summary>
	[Serializable]
	public class NodeEvent
	{
		/// <summary>
		/// Super base-level internal class
		/// </summary>
		[Serializable]
		public abstract class NodeEventBase
		{
			/// <summary>
			/// Invoke this event.
			/// </summary>
			public abstract void InvokeInternal();
		}

		/// <summary>
		/// NodeEvent for when a direct reference to a component is available
		/// </summary>
		[Serializable]
		public class NodeEventComponent : NodeEventBase
		{
			[SerializeField] public Component mTarget;
			[SerializeField] public string mMethodName;

			/// <inheritdoc />
			public override void InvokeInternal()
			{
				Type realType = mTarget.GetType();
				MethodInfo method = realType.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (method == null)
					throw new InvalidOperationException("Could not resolve event target.");

				method.Invoke(mTarget, null);
			}
		}

		/// <summary>
		/// NodeEvent for when the function is public and static inside a script.
		/// </summary>
		[Serializable]
		public class NodeEventStatic : NodeEventBase
		{
			[SerializeField] public string mClassName;
			[SerializeField] public string mMethodName;

			/// <inheritdoc />
			public override void InvokeInternal()
			{
				Type realType = Type.GetType(mClassName);
				if (realType == null)
					throw new InvalidOperationException("Could not resolve event target.");

				MethodInfo method = realType.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Static);
				if (method == null)
					throw new InvalidOperationException("Could not resolve event target.");

				method.Invoke(null, null);
			}
		}

		/// <summary>
		/// NodeEvent for when we're referencing a component, but the reference must be
		/// injected at runtime.
		/// </summary>
		[Serializable]
		public class NodeEventComponentNotStaticResolved : NodeEventBase
		{
			[SerializeField] public string mFullyQualifiedClassName;
			[SerializeField] public string mTargetClassName;
			[SerializeField] public string mMethodName;

			private Component injectedComponent { get; set; }

			/// <summary>
			/// Inject a reference to the necessary component using the gameobject.
			/// </summary>
			/// <param name="go">The gameobject that holds the component we're injecting.</param>
			public void InjectObjectReference(GameObject go)
			{
				if (injectedComponent == null)
				{
					injectedComponent = go.GetComponent(mTargetClassName);
					if (injectedComponent == null)
						Debug.LogWarning("Injecting reference to event that does not match the needed component");
				}
				else
				{
					Component @new = go.GetComponent(mTargetClassName);
					if (@new == null)
						return;

					if (ReferenceEquals(@new, injectedComponent))
						return;

					injectedComponent = @new;
				}
			}

			/// <summary>
			/// Inject a reference directly to the component that we're using.
			/// </summary>
			/// <param name="g"></param>
			public void InjectComponentReference(Component g)
			{
				if (string.CompareOrdinal(g.GetType().Name, mTargetClassName) == 0)
					injectedComponent = g;
				else
					Debug.LogWarning("Injecting reference to event that does not match the needed component");
			}

			/// <inheritdoc />
			public override void InvokeInternal()
			{
				if (injectedComponent == null)
					throw new InvalidOperationException("Attempting to invoke event without injecting the necessary reference!");

				Type realType = injectedComponent.GetType();
				MethodInfo method = realType.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if (method == null)
					throw new InvalidOperationException("Could not resolve event target.");

				method.Invoke(injectedComponent, null);
			}
		}

		/// <summary>
		/// The inner event (one of the above types)
		/// </summary>
		[SerializeField] private NodeEventBase mInnerEvent;

		/// <summary>
		/// True if this node event needs a reference injected to be invoked.
		/// </summary>
		public bool needsInjection { get { return mInnerEvent is NodeEventComponentNotStaticResolved; } }

		/// <summary>
		/// Get the inner event.
		/// </summary>
		public NodeEventBase innerEvent { get { return mInnerEvent; } }

		/// <summary>
		/// Get the type of the component that needs to be injected.
		/// </summary>
		public Type injectionType
		{
			get
			{
				NodeEventComponentNotStaticResolved realEvent = mInnerEvent as NodeEventComponentNotStaticResolved;
				if (realEvent == null)
					return null;

				return Type.GetType(realEvent.mFullyQualifiedClassName);
			}
		}

		/// <summary>
		/// Invoke this event.
		/// </summary>
		public void Invoke()
		{
			if (mInnerEvent != null)
				mInnerEvent.InvokeInternal();
		}

		public void InitializeStatic(Type t, string targetMethod)
		{
			mInnerEvent = new NodeEventStatic
			{
				mClassName = t.AssemblyQualifiedName,
				mMethodName = targetMethod
			};
		}

		public void InitializeComponent(Component c, string targetMethod)
		{
			mInnerEvent = new NodeEventComponent
			{
				mTarget = c,
				mMethodName = targetMethod
			};
		}

		public void InitializeNonResolvedComponent(Component c, string targetMethod)
		{
			mInnerEvent = new NodeEventComponentNotStaticResolved
			{
				mFullyQualifiedClassName = c.GetType().AssemblyQualifiedName,
				mTargetClassName = c.GetType().Name,
				mMethodName = targetMethod
			};
		}

		public void InjectReference(Component c)
		{
			NodeEventComponentNotStaticResolved realEvent = mInnerEvent as NodeEventComponentNotStaticResolved;
			if (realEvent != null)
				realEvent.InjectComponentReference(c);
			else
				Debug.LogWarning("Injecting reference into event that does not support this!");
		}

		public void InjectReference(GameObject c)
		{
			NodeEventComponentNotStaticResolved realEvent = mInnerEvent as NodeEventComponentNotStaticResolved;
			if (realEvent != null)
				realEvent.InjectObjectReference(c);
			else
				Debug.LogWarning("Injecting reference into event that does not support this!");
		}
	}
}
