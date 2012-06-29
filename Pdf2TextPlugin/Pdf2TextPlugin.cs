using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using InlineCommandToolkit;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace Pdf2TextPluginCommandPlugin
{
	public class Pdf2TextPluginCommandPlugin : InlineCommands.OverrideToStringClass, IQuickAccessPluginInterface
	{
		public override string CommandName { get { return "Pdf2TextPlugin"; } }
		public override string DisplayName { get { return "PDF 2 Text"; } }
		public override string Description { get { return "A tool to easily extract the text from a PDF file."; } }
        public override string ArgumentsExample { get { return @"C:\Users\francois\Documents\Visual Studio 2010\Projects\QuickAccess\Pdf2TextPlugin\Test pdf.pdf"; } }

		public override bool PreValidateArgument(out string errorMessage, int Index, string argumentValue)
		{
			errorMessage = "";
            if (Index == 0 && (string.IsNullOrWhiteSpace(argumentValue) || !File.Exists(argumentValue)))
                errorMessage = "Argument must be existing filename, \"" + (argumentValue ?? "") + "\" is not a file";
            else return true;
			return false;
		}

		public override bool ValidateArguments(out string errorMessage, params string[] arguments)
		{
			errorMessage = "";
            if (arguments.Length != 1) errorMessage = "Exactly one argument required for Pdf2Text command";
            else if (!PreValidateArgument(out errorMessage, 0, arguments[0]))
                errorMessage = errorMessage + "";
            else return true;
            return false;
		}

        private string getTextFromPdfFile(string filename)
        {
            PDDocument doc = PDDocument.load(filename);
            PDFTextStripper stripper = new PDFTextStripper();
            return stripper.getText(doc);
        }
        public override bool PerformCommand(out string errorMessage, TextFeedbackEventHandler textFeedbackEvent = null, ProgressChangedEventHandler progressChangedEvent = null, params string[] arguments)
		{
			errorMessage = "";
            try
            {
                //TextFeedbackEventArgs.RaiseTextFeedbackEvent_Ifnotnull(this, textFeedbackEvent, "This is where the command is performed", TextFeedbackType.Noteworthy);
                string filename = arguments[0];
                string outputTextfilename = Path.ChangeExtension(filename, ".txt");
                File.WriteAllText(outputTextfilename, getTextFromPdfFile(filename));
                Process.Start("explorer", "/start,\"" + outputTextfilename + "\"");
            }
            catch { return false; }
			return true;
		}

		private CommandArgument[] availableArguments = new CommandArgument[]
		{
			new CommandArgument("", "filename", new ObservableCollection<string>() { @"C:\Users\francois\Documents\Visual Studio 2010\Projects\QuickAccess\Pdf2TextPlugin\Test pdf.pdf" })
		};
		public override CommandArgument[] AvailableArguments
		{
			get { return availableArguments; }
			set { availableArguments = value; }
		}

		private readonly ObservableCollection<string>[] predefinedArgumentsList =
		{
			new ObservableCollection<string>() { @"C:\Users\francois\Documents\Visual Studio 2010\Projects\QuickAccess\Pdf2TextPlugin\Test pdf.pdf" }
		};

		public override System.Collections.ObjectModel.ObservableCollection<string>[] PredefinedArgumentsList { get { return predefinedArgumentsList; } }

		private System.Collections.Generic.Dictionary<string, string>[] argumentsReplaceKeyValuePair =
		{
		};
		public override System.Collections.Generic.Dictionary<string, string>[] ArgumentsReplaceKeyValuePair { get { return argumentsReplaceKeyValuePair; } }

		//This must return the argument count visible to the user (may vary depending what is already in the currentArguments).
		public override int ArgumentCountForCurrentPopulatedArguments { get { return 1; } }
	}
}
