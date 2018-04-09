using System;
using System.Collections.Generic;
using NodeDialog.Graph;

namespace NodeDialog.Interfaces
{
	public interface IDialogManager
	{
		void ExecuteToPause(IDialogSpeaker speaker);

		string LocalizeToken(string token);
	}
}
