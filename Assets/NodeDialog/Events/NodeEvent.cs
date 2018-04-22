using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NodeDialog.Events
{
	[Serializable]
	public class NodeEvent
	{
		public enum ParameterType
		{
			String,
			Int,
			Float,
			Bool,
		}

		[Serializable]
		public class EventParameter
		{
			[SerializeField] private string mStringValue;
			[SerializeField] private int mIntValue;
			[SerializeField] private float mFloatValue;
			[SerializeField] private bool mBoolValue;

			[SerializeField] private ParameterType mType;

			public object GetValue()
			{
				switch (mType)
				{
					case ParameterType.String:
						return mStringValue;
					case ParameterType.Int:
						return mIntValue;
					case ParameterType.Float:
						return mFloatValue;
					case ParameterType.Bool:
						return mBoolValue;
				}

				return null;
			}
		}

#pragma warning disable 169
		// exists only for ease of serialization
		// otherwise, the editor script would somehow have to find
		// its way back to the script asset using just the typename.
		[SerializeField] private TextAsset mTextAsset;
#pragma warning restore 169

#pragma warning disable 649
		// Collection is updated through the inspector
		[SerializeField] private List<EventParameter> mParameters;
#pragma warning restore 649

		[SerializeField] private string mTypeName;
		[SerializeField] private string mMethodName;

		public void Invoke()
		{
			if (string.IsNullOrEmpty(mTypeName) || string.IsNullOrEmpty(mMethodName))
				return;

			Type t = Type.GetType(mTypeName);
			if (t == null)
				throw new InvalidOperationException("Cannot access type: " + mTypeName);

			MethodInfo m = t.GetMethod(mMethodName, BindingFlags.Public | BindingFlags.Static);
			if (m == null)
				throw new InvalidOperationException("Cannot access method \"" + mMethodName + "\" on type " + mTypeName);

			if (mParameters.Count == 0)
				m.Invoke(null, null);
			else
				InvokeWithParameters(m);
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

		private void InvokeWithParameters(MethodInfo methodInfo)
		{
			var finalParams = mParameters.Select(x => x.GetValue()).ToArray();
			methodInfo.Invoke(null, finalParams);
		}
	}
}
