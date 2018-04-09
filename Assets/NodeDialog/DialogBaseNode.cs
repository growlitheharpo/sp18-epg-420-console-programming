using NodeDialog.Graph;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

		[Serializable]
		public class UserVariableList : List<UserVariable>
		{
			private Dictionary<string, string> mDictionaryRepresentation;

			public UserVariableList(IEnumerable<UserVariable> other) : base(other) { }

			private Dictionary<string, string> GenerateDictionary()
			{
				mDictionaryRepresentation = new Dictionary<string, string>();
				foreach (var v in this)
					mDictionaryRepresentation.Add(v.name, v.value);
				return mDictionaryRepresentation;
			}

			public Dictionary<string, string> ToDictionary()
			{
				return mDictionaryRepresentation ?? GenerateDictionary();
			}
		}

		[SerializeField] private List<UserVariable> mUserVariables;
		private UserVariableList mInnerVariableList;

		public UserVariableList userVariables
		{
			get { return mInnerVariableList ?? (mInnerVariableList = new UserVariableList(mUserVariables)); }
		}
	}
}