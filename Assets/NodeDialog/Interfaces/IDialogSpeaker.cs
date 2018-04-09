using System;
using System.Collections.Generic;
using NodeDialog.Graph;

namespace NodeDialog.Interfaces
{
	public interface IDialogSpeaker
	{
		CharacterDialogAsset dialogAsset { get; }

		void HandleStatement(IDictionary<string, string> userVars, string statementToken);

		void HandlePlayerChoice(IDictionary<string, string> userVars, string statementToken, IList<BaseConnection> choices, Action<BaseConnection> onChoiceCompleteCallback);
	}
}
