using NodeDialog.Graph;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// Node that represents a Dialog "Choice", i.e., something that a dialog character
	/// will say that can then be responded to by the player through its transitions.
	/// </summary>
	[BaseNodeVisual("node0 hex", "node0 hex on")]
	public class DialogChoiceNode : BaseNode
	{
		/// <summary>
		/// The line of dialog (or a token for localization) that this character will say to prompt the player.
		/// </summary>
		[Tooltip("The line of dialog (or a token for localization) that this character will say to prompt the player.")]
		[SerializeField] private string mPrompt;
	}
}
