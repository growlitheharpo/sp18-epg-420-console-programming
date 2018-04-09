using NodeDialog.Graph;
using NodeDialog.Interfaces;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// Node that represents a Dialog "Choice", i.e., something that a dialog character
	/// will say that can then be responded to by the player through its transitions.
	/// </summary>
	[BaseNodeVisual("node0 hex", "node0 hex on")]
	public class DialogChoiceNode : DialogBaseNode
	{
		/// <summary>
		/// The line of dialog (or a token for localization) that this character will say to prompt the player.
		/// </summary>
		[Tooltip("The line of dialog (or a token for localization) that this character will say to prompt the player.")] [SerializeField]
		private string mPrompt;

		/// <summary>
		/// The line of dialog (or a token for localization) that this character will say to prompt the player.
		/// </summary>
		public string prompt { get { return mPrompt; } }

		/// <summary>
		/// Get the statement for this node pre-localized.
		/// </summary>
		/// <param name="manager">The manager that handles localization.</param>
		public string GetLocalizedPrompt(IDialogManager manager)
		{
			return manager.LocalizeToken(prompt);
		}
	}
}
