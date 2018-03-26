using System;

namespace NodeDialog
{
	/// <summary>
	/// Attribute for describing the assets to use when creating a node.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class BaseNodeVisualAttribute : Attribute
	{
		public const string DEFAULT_NORMAL_IMAGE = "node0 hex";
		public const string DEFAULT_SELECTED_IMAGE = "node0 hex on";

		/// <summary>
		/// The filename of the "normal" image for this node.
		/// </summary>
		public string normalImage { get; private set; }

		/// <summary>
		/// The filename of the "selected" image for this node.
		/// </summary>
		public string selectedImage { get; private set; }

		/// <summary>
		/// Attribute for describing the assets to use when creating a node.
		/// </summary>
		/// <param name="normalImage">The filename of the "normal" image for this node.</param>
		/// <param name="selectedImage">The filename of the "selected" image for this node.</param>
		public BaseNodeVisualAttribute(string normalImage, string selectedImage)
		{
			this.normalImage = normalImage;
			this.selectedImage = selectedImage;
		}
	}
}
