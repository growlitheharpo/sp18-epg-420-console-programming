using System.Collections.Generic;
using NodeDialog.Events;
using NodeDialog.Graph;
using NodeDialog.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeDialog.Samples
{
	/// <summary>
	/// ! SAMPLE CLASS !
	/// A character that speaks through the dialog system by referencing a dialog asset.
	/// </summary>
	public class SampleDialogManager : MonoBehaviour, IDialogManager
	{
		private Dictionary<IDialogSpeaker, DialogBaseNode> mNodeMap;

		private void Start()
		{
			mNodeMap = new Dictionary<IDialogSpeaker, DialogBaseNode>();
		}

		public void ExecuteToPause(IDialogSpeaker speaker)
		{
            // Find if this character has a return point saved.
            // Note: this currently does not support saving/loading, but can be adjusted to add it!
			DialogBaseNode currentNode;
			if (!mNodeMap.TryGetValue(speaker, out currentNode))
				currentNode = speaker.dialogAsset.rootNode;

			ExecutePreEvents(currentNode);
			
			// Check if this is a statement node first
			DialogStatementNode statement = currentNode as DialogStatementNode;
			if (statement != null)
			{

				// Execute it if it is
				speaker.HandleStatement(statement.userVariableDictionary, statement.GetLocalizedStatement(this));

				// Save our return point as the next node, or clear our save if there isn't one (we return to the start)
				if (statement.outConnections.Count > 0)
					mNodeMap[speaker] = statement.outConnections[0].outNode as DialogBaseNode;
				else
					mNodeMap.Remove(speaker);
			}
			else
			{
				// Not a statement, check if this is a choice node.
				DialogChoiceNode choice = currentNode as DialogChoiceNode;
				if (choice != null)
				{
					// Execute if it is
					speaker.HandlePlayerChoice(choice.userVariableDictionary, choice.GetLocalizedPrompt(this), choice.outConnections, a =>
					{
						mNodeMap[speaker] = a.outNode as DialogBaseNode;
					});
				}
			}

			ExecutePostEvents(statement);
		}

		private void ExecutePreEvents(DialogBaseNode node)
		{
			foreach (NodeEvent e in node.onStartEvents)
				e.Invoke();
		}

		private void ExecutePostEvents(DialogBaseNode node)
		{
			foreach (NodeEvent e in node.onCompleteEvents)
				e.Invoke();
        }

        public string LocalizeToken(string token)
		{
			return token;
		}
	}
}
