using System.Collections.Generic;
using UnityEngine;

namespace NodeDialog
{
    public class DialogCharacter : MonoBehaviour
	{
		[SerializeField] private List<BaseDialogNode> mNodes;

#if UNITY_EDITOR
		public List<BaseDialogNode> GetNodes_Editor()
		{
			return mNodes ?? (mNodes = new List<BaseDialogNode>());
		}

		public void AddNode_Editor(BaseDialogNode newNode)
		{
			if (mNodes == null)
				mNodes = new List<BaseDialogNode>();

			mNodes.Add(newNode);
		}

		public void RemoveNode_Editor(BaseDialogNode node)
		{
			if (mNodes == null)
				mNodes = new List<BaseDialogNode>();

			mNodes.Remove(node);
		}
#endif
	}
}
