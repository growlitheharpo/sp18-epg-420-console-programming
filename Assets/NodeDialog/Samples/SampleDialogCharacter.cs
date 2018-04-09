using System;
using System.Collections.Generic;
using NodeDialog.Graph;
using NodeDialog.Interfaces;
using UnityEngine;

namespace NodeDialog.Samples
{
	/// <summary>
	/// ! SAMPLE CLASS !
	/// A character that speaks through the dialog system by referencing a dialog asset.
	/// </summary>
	public class SampleDialogCharacter : MonoBehaviour, IDialogSpeaker
	{
		[SerializeField] private CharacterDialogAsset mDialogAsset;

		/// <summary>
		/// The dialog asset attached to this character.
		/// </summary>
		public CharacterDialogAsset dialogAsset { get { return mDialogAsset; } }

		public void HandleStatement(IDictionary<string, string> userVars, string statementToken)
		{
			
		}

		public void HandlePlayerChoice(IDictionary<string, string> userVars, string statementToken, IList<BaseConnection> conns, Action<BaseConnection> onChoiceCompleteCallback)
		{
			
		}
	}
}
