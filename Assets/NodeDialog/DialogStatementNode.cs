using System.Collections.Generic;
using NodeDialog.Graph;
using NodeDialog.Interfaces;
using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// Node that represents a Dialog "Statement", i.e. something that a dialog character
	/// will say directly to the player without any need for a response.
	/// </summary>
	[BaseNodeVisual("node0", "node0 on")]
	public class DialogStatementNode : DialogBaseNode
	{
		/// <summary>
		/// The line of dialog (or a token for localization) that this character will say.
		/// </summary>
		[Tooltip("The line of dialog (or a token for localization) that this character will say.")] 
		[SerializeField] 
		private string mStatement;

		/// <summary>
		/// The line of dialog (or a token for localization) that this character will say.
		/// </summary>
		public string statement { get { return mStatement; } }

		/// <summary>
		/// Get the statement for this node pre-localized.
		/// </summary>
		/// <param name="manager">The manager that handles localization.</param>
		public string GetLocalizedStatement(IDialogManager manager)
		{
			return manager.LocalizeToken(statement);
		}
	}
}
