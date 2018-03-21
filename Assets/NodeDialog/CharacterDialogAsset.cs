using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeDialog
{
	[CreateAssetMenu(fileName = "DialogAsset", menuName = "Node Dialog/Dialog Asset File")]
	public class CharacterDialogAsset : ScriptableObject
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
			OnValidate();
		}

		public void RemoveNode_Editor(BaseDialogNode node)
		{
			if (mNodes == null)
				mNodes = new List<BaseDialogNode>();

			mNodes.Remove(node);
			OnValidate();
		}

		private void OnValidate()
		{
			Debug.Log("VALIDATED");

			string path = UnityEditor.AssetDatabase.GetAssetPath(this);
			var subassets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);

			foreach (BaseDialogNode node in mNodes)
			{
				if (subassets.Contains(node))
					continue;

				UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
				node.hideFlags = HideFlags.HideInHierarchy;
			}

			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
	}
#endif
}
