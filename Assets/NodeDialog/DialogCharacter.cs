using System.Collections.Generic;
using UnityEngine;

namespace NodeDialog
{
    public class DialogCharacter : MonoBehaviour
    {
	    [SerializeField] private CharacterDialogAsset mDialogAsset;

		public CharacterDialogAsset dialogAsset {get { return mDialogAsset; }}
    }
}
