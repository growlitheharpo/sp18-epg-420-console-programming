using NodeDialog.Graph;
using System;
using System.Collections.Generic;
using NodeDialog.Events;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// Base class that contains functionality common to all *dialog* nodes
	/// </summary>
	public class DialogBaseNode : BaseNode
	{
		[Serializable]
		public struct UserVariable
		{
			[Tooltip("The name of this variable.")]
			[SerializeField]
			private string mName;

			[Tooltip("The value of this variable.")]
			[SerializeField]
			private string mValue;

			public string name { get { return mName; } }

			public string value { get { return mValue; } }

			public KeyValuePair<string, string> ToKeyValue()
			{
				return new KeyValuePair<string, string>(mName, mValue);
			}
		}

		[SerializeField] private List<UserVariable> mUserVariables;
		private Dictionary<string, string> mCachedDictionaryCopy;

		[SerializeField] private List<NodeEvent> mOnStartEvents;
		[SerializeField] private List<NodeEvent> mOnCompleteEvents;

		public List<UserVariable> userVariableList
		{
			get { return mUserVariables; }
		}

		public Dictionary<string, string> userVariableDictionary
		{
			get
			{
				if (mCachedDictionaryCopy != null)
					return mCachedDictionaryCopy;
				
				mCachedDictionaryCopy = new Dictionary<string, string>();
				foreach (UserVariable v in mUserVariables)
					mCachedDictionaryCopy.Add(v.name, v.value);

				return mCachedDictionaryCopy;
			}
		}

		public List<NodeEvent> onStartEvents { get { return mOnStartEvents; } }

		public List<NodeEvent> onCompleteEvents { get { return mOnCompleteEvents; } }
	}
}
