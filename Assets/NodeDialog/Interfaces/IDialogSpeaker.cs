using System;
using System.Collections.Generic;
using NodeDialog.Graph;

namespace NodeDialog.Interfaces
{
	public interface IDialogSpeaker
	{
		void HandleStatement(IDictionary<string, string> userVars, string statementToken);

		void HandlePlayerChoice(IDictionary<string, string> userVars, string statementToken, Action<BaseConnection> onChoiceCompleteCallback);
	}
}
