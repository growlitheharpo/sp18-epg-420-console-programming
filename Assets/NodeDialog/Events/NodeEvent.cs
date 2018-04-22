using System;
using System.Reflection;
using UnityEngine;

namespace NodeDialog.Events
{
	[Serializable]
	public class NodeEvent
	{
#pragma warning disable 169
		// exists only for ease of serialization
		[SerializeField] private TextAsset mTextAsset;
#pragma warning restore 169

		[SerializeField] private string mTypeName;
		[SerializeField] private string mMethodName;

		public void Invoke()
		{
			Type t = Type.GetType(mTypeName);
			if (t == null)
				throw new InvalidOperationException("Cannot access type: " + mTypeName);

			MethodInfo m = t.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Static);
			if (m == null)
				throw new InvalidOperationException("Cannot access method " + mMethodName + " on type " + mTypeName);

			m.Invoke(null, null);
		}

		public void Invoke(object[] args)
		{
			Type t = Type.GetType(mTypeName);
			if (t == null)
				throw new InvalidOperationException("Cannot access type: " + mTypeName);

			MethodInfo m = t.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Static);
			if (m == null)
				throw new InvalidOperationException("Cannot access method " + mMethodName + " on type " + mTypeName);

			m.Invoke(null, args);
		}
	}
}
