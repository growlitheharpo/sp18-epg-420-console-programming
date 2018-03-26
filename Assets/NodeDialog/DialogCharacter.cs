using UnityEngine;

namespace NodeDialog
{
	/// <summary>
	/// ! SAMPLE CLASS !
	/// A character that speaks through the dialog system by referencing a dialog asset.
	/// </summary>
	public class DialogCharacter : MonoBehaviour
	{
		[SerializeField] private CharacterDialogAsset mDialogAsset;

		/// <summary>
		/// The dialog asset attached to this character.
		/// </summary>
		public CharacterDialogAsset dialogAsset { get { return mDialogAsset; } }
	}
}
