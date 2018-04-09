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

		[SerializeField] private SampleDialogManager mManager;

		[SerializeField] private UnityEngine.UI.Text mStatementText;
		[SerializeField] private UnityEngine.UI.GridLayoutGroup mChoiceGroup;
		[SerializeField] private UnityEngine.UI.Button mContinueButton;
		[SerializeField] private GameObject mChoicePrefabButton;

		private IDictionary<string, string> mCurrentUserVariables;

		/// <summary>
		/// The dialog asset attached to this character.
		/// </summary>
		public CharacterDialogAsset dialogAsset { get { return mDialogAsset; } }

		private void Start()
		{
			mContinueButton.onClick.AddListener(ContinueDialog);
		}

		public void HandleStatement(IDictionary<string, string> userVars, string statementToken)
		{
			mStatementText.text = statementToken;
			mContinueButton.gameObject.SetActive(true);
			mCurrentUserVariables = userVars;
		}

		public void HandlePlayerChoice(
			IDictionary<string, string> userVars, string choiceText, IList<BaseConnection> conns,
			Action<BaseConnection> onChoiceCompleteCallback)
		{
			mContinueButton.gameObject.SetActive(false);
			foreach (BaseConnection c in conns)
			{
				UnityEngine.UI.Button button = Instantiate(mChoicePrefabButton, mChoiceGroup.transform, false)
					.GetComponent<UnityEngine.UI.Button>();
				
				button.GetComponentInChildren<UnityEngine.UI.Text>().text = c.connectionText;
				
				BaseConnection connection = c;
				button.onClick.AddListener(() => HandleChoice(connection, onChoiceCompleteCallback));
			}

			mStatementText.text = choiceText;
			mCurrentUserVariables = userVars;
		}

		private void HandleChoice(BaseConnection c, Action<BaseConnection> callback)
		{
			mContinueButton.gameObject.SetActive(true);
			foreach (Transform child in mChoiceGroup.transform)
				Destroy(child.gameObject);

			callback(c);
			ContinueDialog();
		}

		private void ContinueDialog()
		{
			mManager.ExecuteToPause(this);
		}

		private void OnGUI()
		{
			GUILayout.Label("Current user variables: ");
			if (mCurrentUserVariables == null)
				GUILayout.Label("None.");
			else
			{
				foreach (var pair in mCurrentUserVariables)
					GUILayout.Label(pair.Key + ": " + pair.Value);
			}
		}
	}
}
