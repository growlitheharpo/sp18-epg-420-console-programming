using System;

namespace NodeDialog
{
	[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Struct)]
	public class BaseNodeVisualAttribute : Attribute
	{
		public const string DEFAULT_NORMAL_IMAGE = "node0 hex";
		public const string DEFAULT_SELECTED_IMAGE = "node0 hex on";

		public string normalImage { get; private set; }
		public string selectedImage { get; private set; }

		public BaseNodeVisualAttribute(string normalImage, string selectedImage)
		{
			this.normalImage = normalImage;
			this.selectedImage = selectedImage;
		}
	}
}
