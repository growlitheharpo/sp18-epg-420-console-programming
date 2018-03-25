using UnityEngine;

namespace NodeDialog
{
	[BaseNodeVisual("node0 hex", "node0 hex on")]
	public class ChoiceDialogNode : BaseDialogNode
	{
		[SerializeField] private string mPrompt;
	}
}
