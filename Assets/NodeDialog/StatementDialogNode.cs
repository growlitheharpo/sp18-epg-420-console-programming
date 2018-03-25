using UnityEngine;

namespace NodeDialog
{
	[BaseNodeVisual("node0", "node0 on")]
	public class StatementDialogNode : BaseDialogNode
	{
		[SerializeField] private string mStatement;
	}
}
