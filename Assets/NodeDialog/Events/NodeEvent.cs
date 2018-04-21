using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeDialog.Events
{
	[Serializable]
	public class NodeEvent
	{
		//[SerializeField] private ExposedReference<Object> mTarget;
		[SerializeField] private Object resolvedObject;

		[SerializeField] private string mScriptType;
		[SerializeField] private string mMethodName;

		public void Invoke(IExposedPropertyTable resolver)
		{
			//Object resolvedObject = mTarget.Resolve(resolver);
			Component trueTarget = resolvedObject as Component;

			if (trueTarget == null)
			{
				GameObject resolvedGameObject = resolvedObject as GameObject;
				if (resolvedGameObject != null)
					trueTarget = resolvedGameObject.GetComponent(mScriptType);
			}

			if (trueTarget != null)
				InvokeInternal(trueTarget);
			else
				InvokeInternal_FromName();
		}

		private void InvokeInternal(Component c)
		{
			Type realType = c.GetType();
			MethodInfo method = realType.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
			if (method == null)
				throw new InvalidOperationException("Could not find method on target!");

			method.Invoke(c, null);
		}

		private void InvokeInternal_FromName()
		{
			Type type = Type.GetType(mScriptType);
			if (type == null)
				throw new InvalidOperationException("Could not find method on target!");
			MethodInfo method = type.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
			if (method == null)
				throw new InvalidOperationException("Could not find method on target!");

			method.Invoke(null, null);
		}
	}
}
