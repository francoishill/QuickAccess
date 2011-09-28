﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using SectionDetails = QuickAccess.NSISclass.SectionGroupClass.SectionClass;
using ShortcutDetails = QuickAccess.NSISclass.SectionGroupClass.SectionClass.ShortcutDetails;
using FileToAddTextblock = QuickAccess.NSISclass.SectionGroupClass.SectionClass.FileToAddTextblock;

namespace QuickAccess
{
	public class NSISclass
	{
		private static string Spacer = "  ";
		public string Empty = "";
		public string ProductName;
		public string ProductVersion;
		public string ProductPublisher;
		public string ProductWebsite;
		public string ProductExeName;

		public Compressor CompressorUsed;
		public string SetupFileName;
		public LanguagesEnum SetupLanguage;

		public Boolean UseUninstaller;
		public string InstallerIconPath;
		public string UninstallerIconPath;
		public Boolean ShowWelcomePage;

		public LicensePageDetails LicenseDialogDetailsUsed;
		public Boolean ShowComponentsPage;
		public Boolean ShowDirectoryPage;
		public Boolean UserMayChangeStartMenuName;

		public string FilePathToRunOnFinish;

		public List<string> InstTypes;

		public Boolean InstallForAllUsers;

		public NSISclass() { }

		public NSISclass(
				string ProductNameIn,
				string ProductVersionIn,
				string ProductPublisherIn,
				string ProductWebsiteIn,
				string ProductExeNameIn,
				Compressor CompressorUsedIn,
				string SetupFileNameIn,
				LanguagesEnum SetupLanguageIn,
			//LanguagesEnum LanguagesIn,
				Boolean UseUninstallerIn,
				Boolean ShowWelcomePageIn,
				LicensePageDetails LicenseDialogDetailsUsedIn,
				Boolean ShowComponentsPageIn,
				Boolean ShowDirectoryPageIn,
				Boolean UserMayChangeStartMenuNameIn,
				string FilePathToRunOnFinishIn,
				List<string> InstTypesIn,
				string InstallerIconPathIn = @"${NSISDIR}\Contrib\Graphics\Icons\modern-install-blue-full.ico",
				string UninstallerIconPathIn = @"${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall-blue-full.ico",
				Boolean InstallForAllUsersIn = true
				)
		{
			ProductName = ProductNameIn;
			ProductVersion = ProductVersionIn;
			ProductPublisher = ProductPublisherIn;
			ProductWebsite = ProductWebsiteIn;
			ProductExeName = ProductExeNameIn;
			CompressorUsed = CompressorUsedIn;
			SetupFileName = SetupFileNameIn;

			int CountSetupLanguages = 0; foreach (LanguagesEnum testLanguageFound in Enum.GetValues(typeof(LanguagesEnum))) if (SetupLanguage.HasFlag(testLanguageFound)) CountSetupLanguages++;
			if (CountSetupLanguages > 1) MessageBox.Show("More than one setup language not allowed, first chosen (alphabetically) will be used", "More than one setup language", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			SetupLanguage = SetupLanguageIn;

			//Languages = LanguagesIn;
			UseUninstaller = UseUninstallerIn;
			InstallerIconPath = InstallerIconPathIn;
			UninstallerIconPath = UninstallerIconPathIn;

			ShowWelcomePage = ShowWelcomePageIn;
			LicenseDialogDetailsUsed = LicenseDialogDetailsUsedIn;

			ShowComponentsPage = ShowComponentsPageIn;
			ShowDirectoryPage = ShowDirectoryPageIn;
			UserMayChangeStartMenuName = UserMayChangeStartMenuNameIn;

			FilePathToRunOnFinish = FilePathToRunOnFinishIn;
			InstTypes = InstTypesIn;

			InstallForAllUsers = InstallForAllUsersIn;
		}

		private enum SetShellVarContext { all, current };

		public enum LanguagesEnum : long
		{
			Afrikaans = 0x00000001,
			Albanian = 0x00000002,
			Arabic = 0x00000004,
			Basque = 0x00000008,
			Belarusian = 0x00000010,
			Bosnian = 0x00000020,
			Breton = 0x00000040,
			Bulgarian = 0x00000080,
			Catalan = 0x00000100,
			Croatian = 0x00000200,
			Czech = 0x00000400,
			Danish = 0x00000800,
			Dutch = 0x00001000,
			English = 0x00002000,
			Esperanto = 0x00004000,
			Estonian = 0x00008000,
			Farsi = 0x00010000,
			Finnish = 0x00020000,
			French = 0x00040000,
			Galician = 0x00080000,
			German = 0x00100000,
			Greek = 0x00200000,
			Hebrew = 0x00400000,
			Hungarian = 0x00800000,
			Icelandic = 0x01000000,
			Indonesian = 0x02000000,
			Irish = 0x04000000,
			Italian = 0x08000000,
			Japanese = 0x10000000,
			Korean = 0x20000000,
			Kurdish = 0x40000000,
			Latvian = 0x80000000,
			Lithuanian = 0x100000000,
			Luxembourgish = 0x200000000,
			Macedonian = 0x400000000,
			Malay = 0x800000000,
			Mongolian = 0x1000000000,
			Norwegian = 0x2000000000,
			NorwegianNynorsk = 0x4000000000,
			Polish = 0x8000000000,
			Portuguese = 0x10000000000,
			PortugueseBR = 0x20000000000,
			Romanian = 0x40000000000,
			Russian = 0x80000000000,
			Serbian = 0x100000000000,
			SerbianLatin = 0x200000000000,
			SimpChinese = 0x400000000000,
			Slovak = 0x800000000000,
			Slovenian = 0x1000000000000,
			Spanish = 0x2000000000000,
			SpanishInternational = 0x4000000000000,
			Swedish = 0x8000000000000,
			Thai = 0x10000000000000,
			TradChinese = 0x20000000000000,
			Turkish = 0x40000000000000,
			Ukrainian = 0x80000000000000,
			Uzbek = 0x100000000000000,
			Welsh = 0x200000000000000,
		}

		public class Compressor
		{
			public enum CompressionModeEnum { zlib, bzip2, lzma };

			public CompressionModeEnum CompressionMode;
			public Boolean Final;
			public Boolean Solid;

			public Compressor()
			{
				CompressionMode = CompressionModeEnum.bzip2;
				Final = false;
				Solid = false;
			}

			public Compressor(CompressionModeEnum CompressionModeIn, Boolean FinalIn, Boolean SolidIn)
			{
				CompressionMode = CompressionModeIn;
				Final = FinalIn;
				Solid = SolidIn;
			}
		}

		public class LicensePageDetails
		{
			public enum AcceptWith { Checkbox, Radiobuttons, Classic };

			public Boolean ShowLicensePage;
			public string LicenseFilePath;
			public AcceptWith acceptWith;

			public LicensePageDetails(Boolean ShowLicensePageIn, string LicenseFilePathIn = "", AcceptWith acceptWithIn = AcceptWith.Radiobuttons)
			{
				if (ShowLicensePageIn)
				{
					ShowLicensePage = ShowLicensePageIn;
					LicenseFilePath = LicenseFilePathIn;
					acceptWith = acceptWithIn;
				}
			}
		}

		public List<string> GetAllLinesForNSISfile(List<string> AllSectionGroupLines, List<string> AllSectionAndGroupDescriptions)
		{
			List<string> tmpList = new List<string>();

			tmpList.Add(@"; Script generated by the HM NIS Edit Script Wizard.");
			tmpList.Add("");
			tmpList.Add(@"; HM NIS Edit Wizard helper defines");
			tmpList.Add(@"!define PRODUCT_NAME """ + ProductName + @"""");
			tmpList.Add(@"!define PRODUCT_VERSION """ + ProductVersion + @"""");
			tmpList.Add(@"!define PRODUCT_PUBLISHER """ + ProductPublisher + @"""");
			tmpList.Add(@"!define PRODUCT_WEB_SITE """ + ProductWebsite + @"""");
			tmpList.Add(@"!define PRODUCT_DIR_REGKEY """ + @"Software\Microsoft\Windows\CurrentVersion\App Paths\" + ProductExeName + (ProductExeName.ToUpper().EndsWith(".EXE") ? "" : ".exe") + @"""");
			tmpList.Add(@"!define PRODUCT_EXE_NAME " + @"""" + ProductExeName + @"""");

			if (UseUninstaller)
			{
				tmpList.Add(@"!define PRODUCT_UNINST_KEY ""Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}""");
				tmpList.Add(@"!define PRODUCT_UNINST_ROOT_KEY ""HKLM""");
			}

			tmpList.Add(@"!define PRODUCT_STARTMENU_REGVAL ""NSIS:StartMenuDir""");
			tmpList.Add("");

			tmpList.Add(@";SetCompressor ""/SOLID"" lzma ;Seems to be using more space..");
			tmpList.Add(@"SetCompressor " + (CompressorUsed.Solid ? "/SOLID " : "") + (CompressorUsed.Final ? "/FINAL " : "") + CompressorUsed.CompressionMode.ToString());
			tmpList.Add("");
			tmpList.Add(@"; MUI 1.67 compatible ------");
			tmpList.Add(@"!include ""MUI.nsh""");
			tmpList.Add("");

			tmpList.Add(@"; MUI Settings");
			tmpList.Add(@"!define MUI_ABORTWARNING");
			tmpList.Add(@"!define MUI_ICON """ + InstallerIconPath + @"""");
			if (UseUninstaller) tmpList.Add(@"!define MUI_UNICON """ + UninstallerIconPath + @"""");
			tmpList.Add("");

			if (ShowWelcomePage)
			{
				tmpList.Add(@"; Welcome page");
				tmpList.Add(@"!insertmacro MUI_PAGE_WELCOME");
			}

			if (LicenseDialogDetailsUsed != null && LicenseDialogDetailsUsed.ShowLicensePage)
			{
				tmpList.Add(@"; License page");
				if (LicenseDialogDetailsUsed.acceptWith == LicensePageDetails.AcceptWith.Checkbox)
					tmpList.Add(@"!define MUI_LICENSEPAGE_CHECKBOX");
				else if (LicenseDialogDetailsUsed.acceptWith == LicensePageDetails.AcceptWith.Radiobuttons)
					tmpList.Add(@"!define MUI_LICENSEPAGE_RADIOBUTTONS");
				tmpList.Add(@"!insertmacro MUI_PAGE_LICENSE """ + LicenseDialogDetailsUsed.LicenseFilePath + @"""");
			}

			if (ShowComponentsPage)
			{
				tmpList.Add(@"; Components page");
				tmpList.Add(@"!insertmacro MUI_PAGE_COMPONENTS");
			}

			if (ShowDirectoryPage)
			{
				tmpList.Add(@"; Directory page");
				tmpList.Add(@"!insertmacro MUI_PAGE_DIRECTORY");
			}

			if (UserMayChangeStartMenuName)
			{
				tmpList.Add(@"; Start menu page");
				tmpList.Add(@"var ICONS_GROUP");
				tmpList.Add(@"!define MUI_STARTMENUPAGE_NODISABLE");
				tmpList.Add(@"!define MUI_STARTMENUPAGE_DEFAULTFOLDER ""${PRODUCT_NAME}""");
				tmpList.Add(@"!define MUI_STARTMENUPAGE_REGISTRY_ROOT ""${PRODUCT_UNINST_ROOT_KEY}""");
				tmpList.Add(@"!define MUI_STARTMENUPAGE_REGISTRY_KEY ""${PRODUCT_UNINST_KEY}""");
				tmpList.Add(@"!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME ""${PRODUCT_STARTMENU_REGVAL}""");
				tmpList.Add(@"!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP");
			}

			tmpList.Add(@"; Instfiles page");
			tmpList.Add(@"!insertmacro MUI_PAGE_INSTFILES");


			tmpList.Add(@"; Finish page");
			if (FilePathToRunOnFinish.Length > 0) tmpList.Add(@"!define MUI_FINISHPAGE_RUN """ + FilePathToRunOnFinish + @"""");
			tmpList.Add(@"!insertmacro MUI_PAGE_FINISH"); tmpList.Add("");

			if (UseUninstaller)
			{
				tmpList.Add(@"; Uninstaller pages");
				tmpList.Add(@"!insertmacro MUI_UNPAGE_INSTFILES"); tmpList.Add("");
			}

			String UsedLangueString = "";
			//String tmpStr = "";
			//foreach (String lang in Languages) tmpStr += @" """ + lang + @"""";
			tmpList.Add(@"; Language files");
			//tmpList.Add(@"!insertmacro MUI_LANGUAGE " + tmpStr);
			foreach (LanguagesEnum testLanguageFound in Enum.GetValues(typeof(LanguagesEnum)))
				if (SetupLanguage.HasFlag(testLanguageFound))
				{ tmpList.Add(@"!insertmacro MUI_LANGUAGE " + @"""" + testLanguageFound.ToString() + @""""); UsedLangueString = testLanguageFound.ToString(); break; }
			tmpList.Add("");
			tmpList.Add(@"; MUI end ------");
			tmpList.Add("");

			tmpList.Add(@"Name ""${PRODUCT_NAME} ${PRODUCT_VERSION}""");
			tmpList.Add(@"OutFile """ + SetupFileName + (SetupFileName.ToUpper().EndsWith(".EXE") ? "" : ".exe") + @"""");

			//foreach (LanguagesEnum testLanguageFound in Enum.GetValues(typeof(LanguagesEnum)))
			//    if (Languages.HasFlag(testLanguageFound) && testLanguageFound.ToString().ToUpper() != UsedLangueString.ToUpper())
			//        tmpList.Add(@"LoadLanguageFile """ + @"${NSISDIR}\Contrib\Language files\" + testLanguageFound.ToString() + @".nlf""");

			tmpList.Add(@"InstallDir ""$PROGRAMFILES\${PRODUCT_NAME}""");
			tmpList.Add(@"InstallDirRegKey HKLM ""${PRODUCT_DIR_REGKEY}"" """"");
			tmpList.Add(@"ShowInstDetails show");
			if (UseUninstaller) tmpList.Add(@"ShowUnInstDetails show"); tmpList.Add("");

			if (InstTypes != null)
				foreach (String instType in InstTypes)
					tmpList.Add(@"InstType """ + instType + @""""); tmpList.Add("");

			if (AllSectionGroupLines != null)
				foreach (string line in AllSectionGroupLines) tmpList.Add(line); tmpList.Add("");

			string SectionInAllInstTypes = "SectionIn";
			if (InstTypes != null)
				for (int i = 1; i <= InstTypes.Count; i++)
					SectionInAllInstTypes += " " + i.ToString();

			tmpList.Add(@"Section -AdditionalIcons");
			if (InstTypes != null && InstTypes.Count > 0) tmpList.Add(SectionInAllInstTypes);
			//tmpList.Add(Spacer + @"SetShellVarContext " + (InstallForAllUsers ? "all" : "current"));
			tmpList.Add(Spacer + @"SetShellVarContext " + "current");
			tmpList.Add(Spacer + @"!insertmacro MUI_STARTMENU_WRITE_BEGIN Application");
			if (ProductExeName.Length > 0) tmpList.Add(Spacer + string.Format(@"CreateShortCut ""$SMPROGRAMS\$ICONS_GROUP\{0}.lnk"" ""$INSTDIR\{0}.exe""", ProductExeName.ToUpper().EndsWith(".EXE") ? ProductExeName.Substring(0, ProductExeName.Length - 4) : ProductExeName));
			tmpList.Add(Spacer + @"WriteIniStr ""$INSTDIR\${PRODUCT_NAME}.url"" ""InternetShortcut"" ""URL"" ""${PRODUCT_WEB_SITE}""");
			if (ProductWebsite != null && ProductWebsite.Length > 0) tmpList.Add(Spacer + @"CreateShortCut ""$SMPROGRAMS\$ICONS_GROUP\Website.lnk"" ""$INSTDIR\${PRODUCT_NAME}.url""");
			tmpList.Add(Spacer + @"CreateShortCut ""$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk"" ""$INSTDIR\uninst.exe""");
			tmpList.Add(Spacer + @"!insertmacro MUI_STARTMENU_WRITE_END");
			tmpList.Add(@"SectionEnd"); tmpList.Add("");

			tmpList.Add(@"Section -Post");
			if (InstTypes != null && InstTypes.Count > 0) tmpList.Add(SectionInAllInstTypes);
			tmpList.Add(Spacer + @"SetShellVarContext " + (InstallForAllUsers ? "all" : "current"));
			tmpList.Add(Spacer + @"WriteUninstaller ""$INSTDIR\uninst.exe""");
			tmpList.Add(Spacer + @"WriteRegStr HKLM ""${PRODUCT_DIR_REGKEY}"" """" ""$INSTDIR\${PRODUCT_EXE_NAME}""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""DisplayName"" ""$(^Name)""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""UninstallString"" ""$INSTDIR\uninst.exe""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""DisplayIcon"" ""$INSTDIR\${PRODUCT_EXE_NAME}""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""DisplayVersion"" ""${PRODUCT_VERSION}""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""URLInfoAbout"" ""${PRODUCT_WEB_SITE}""");
			tmpList.Add(Spacer + @"WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}"" ""Publisher"" ""${PRODUCT_PUBLISHER}""");
			tmpList.Add(@"SectionEnd"); tmpList.Add("");

			if (AllSectionAndGroupDescriptions != null && AllSectionAndGroupDescriptions.Count > 0)
			{
				tmpList.Add(@"; Section descriptions");
				tmpList.Add(@"!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN");
				foreach (string line in AllSectionAndGroupDescriptions) tmpList.Add(Spacer + line);
				tmpList.Add(@"!insertmacro MUI_FUNCTION_DESCRIPTION_END"); tmpList.Add("");
			}

			tmpList.Add(@"Function un.onUninstSuccess");
			tmpList.Add(Spacer + @"HideWindow");
			tmpList.Add(Spacer + @"MessageBox MB_ICONINFORMATION|MB_OK ""$(^Name) was successfully removed from your computer.""");
			tmpList.Add(@"FunctionEnd"); tmpList.Add("");

			tmpList.Add(@"Function un.onInit");
			tmpList.Add(Spacer + @"MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 ""Are you sure you want to completely remove $(^Name) and all of its components?"" IDYES +2");
			tmpList.Add(Spacer + @"Abort");
			tmpList.Add(@"FunctionEnd"); tmpList.Add("");

			tmpList.Add(@"Section Uninstall");
			//tmpList.Add(Spacer + @"Delete ""$INSTDIR\${PRODUCT_NAME}.url""");
			//tmpList.Add(Spacer + @"Delete ""$INSTDIR\uninst.exe""");
			//tmpList.Add(Spacer + @"Delete ""$INSTDIR\${PRODUCT_EXE_NAME}"""); tmpList.Add("");
			tmpList.Add(Spacer + @"Delete ""$INSTDIR\*.*"""); tmpList.Add("");

			tmpList.Add(Spacer + @"SetShellVarContext all");
			tmpList.Add(Spacer + @"!insertmacro MUI_STARTMENU_GETFOLDER ""Application"" $ICONS_GROUP");
			tmpList.Add(Spacer + @"Delete ""$SMPROGRAMS\$ICONS_GROUP\*.*""");
			tmpList.Add(Spacer + @"Delete ""$DESKTOP\${PRODUCT_NAME}.lnk""");
			tmpList.Add(Spacer + @"RMDir ""$SMPROGRAMS\$ICONS_GROUP"""); tmpList.Add("");

			tmpList.Add(Spacer + @"SetShellVarContext current");
			tmpList.Add(Spacer + @"!insertmacro MUI_STARTMENU_GETFOLDER ""Application"" $ICONS_GROUP");
			tmpList.Add(Spacer + @"Delete ""$SMPROGRAMS\$ICONS_GROUP\*.*""");
			tmpList.Add(Spacer + @"Delete ""$DESKTOP\${PRODUCT_NAME}.lnk""");
			tmpList.Add(Spacer + @"RMDir ""$SMPROGRAMS\$ICONS_GROUP"""); tmpList.Add("");

			tmpList.Add(Spacer + @"RMDir ""$INSTDIR"""); tmpList.Add("");

			tmpList.Add(Spacer + @"DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} ""${PRODUCT_UNINST_KEY}""");
			tmpList.Add(Spacer + @"DeleteRegKey HKLM ""${PRODUCT_DIR_REGKEY}""");
			tmpList.Add(Spacer + @"SetAutoClose true");
			tmpList.Add(@"SectionEnd");

			return tmpList;
		}

		public class SectionGroupClass
		{
			//public List<string> FullTextBlock;
			public string Name;
			public string Description;
			public string IDString = "";
			public Boolean UninstallerGroup;
			public Boolean DisplayInBold;
			public Boolean ExpandedByDefault;

			public SectionGroupClass() { }

			public SectionGroupClass(string NameIn, string DescriptionIn, Boolean UninstallerGroupIn, Boolean DisplayInBoldIn = true, Boolean ExpandedByDefaultIn = true)//, List<SectionGroupDetails> SectionGroupsIn, List<SectionDetails> SectionsIn)
			{
				Name = NameIn;
				Description = DescriptionIn;
				//IDString = IDStringIn;
				UninstallerGroup = UninstallerGroupIn;
				DisplayInBold = DisplayInBoldIn;
				ExpandedByDefault = ExpandedByDefaultIn;
			}

			public class SectionClass
			{
				public enum SetOverwriteEnum { on, off, Try, ifnewer, ifdiff };

				public string SectionName;
				public string SectionDescription;
				public string IDString = "";
				public string InstTypes_CommaSeperated;
				public SetOverwriteEnum SetOverwrite;
				public string SetOutPath;
				public readonly string EndLine = "SectionEnd";
				public Boolean UnselectedByDefault;
				public Boolean HiddenToUser;
				public Boolean SectionForUninstaller;
				public Boolean DisplaySectionWithBoldFont;
				public int ReserveDiskspaceForSection;

				public SectionClass() { }

				public SectionClass(
						string SectionNameIn,//
						string SectionDescriptionIn,
						string InstTypes_CommaSeperatedIn,
						SetOverwriteEnum SetOverwriteIn = SetOverwriteEnum.ifnewer,
						string SetOutPathIn = "$INSTDIR",
						Boolean UnselectedByDefaultIn = false,//
						Boolean HiddenToUserIn = false, Boolean SectionForUninstallerIn = false,//
						Boolean DisplaySectionWithBoldFontIn = false,//
						int ReserveDiskspaceForSectionIn = 0)
				{
					SectionName = SectionNameIn;
					SectionDescription = SectionDescriptionIn;
					InstTypes_CommaSeperated = InstTypes_CommaSeperatedIn;
					SetOverwrite = SetOverwriteIn;
					ReserveDiskspaceForSection = ReserveDiskspaceForSectionIn;

					SetOutPath = SetOutPathIn;
				}

				public string HeaderLine
				{
					get
					{
						return
								"Section" +
									 (UnselectedByDefault ? " /o" : "") + " \"" + (DisplaySectionWithBoldFont ? "!" : "") +
									 (HiddenToUser ? "-" : "") + (SectionForUninstaller ? "un." : "") +
									 SectionName + "\"" +
									 " " + IDString;// +
					}
				}

				private String RemoveQuoteChars(String InputString)
				{
					String tmpStr = InputString;
					if (tmpStr.StartsWith("\"")) tmpStr = tmpStr.Remove(0, 1);
					if (tmpStr.EndsWith("\"")) tmpStr = tmpStr.Substring(0, tmpStr.Length - 1);
					return tmpStr;
				}

				public class FileToAddTextblock
				{
					public enum ExecuteModeEnum { None, NormalNotDelete, NormalDoDelete, QuietNotDelete, QuietDoDelete };

					public string SetOutPath;
					public SetOverwriteEnum SetOverwrite;
					public ExecuteModeEnum ExecuteMode;
					public string FileNameOnly;
					public Boolean HideDetailsPrint;
					public string DetailPrint;

					//public FileToAddTextblock() { }

					/// <summary>
					/// Details for each file (directory) that will be included in the installation package
					/// </summary>
					/// <param name="SetShellVarContextIn">Must the file/dir be installed for all users or only current</param>
					/// <param name="SetOutPathIn">What is the destination directory for the file(s)  (can be built from enum DirectoryVariables)</param>
					/// <param name="SetOverwriteIn">How must overwrite be handled if the file exists in the destination directory</param>
					/// <param name="FileStrIn">The full file string remember /oname=X to specify different output name than original(File /r /x *.svn* /x *.tmp "..\..\..\Binaries\Win32\SRMS\Latest\GLS Shared\*.*")</param>
					/// <param name="ExecuteModeIn">If the file must be runned after installing to destination</param>
					/// <param name="SetDetailsPrintIn">Whether details must be hidden when copying or running the file</param>
					/// <param name="DetailPrintIn">The message to display to the user when copying or running the file</param>
					public FileToAddTextblock(//List<FileToAddLine> FileLineStringListIn,
							string SetOutPathIn = "$INSTDIR", SetOverwriteEnum SetOverwriteIn = SetOverwriteEnum.ifnewer,
							ExecuteModeEnum ExecuteModeIn = ExecuteModeEnum.None, string FileNameOnlyIn = "",
							Boolean HideDetailsPrintIn = false, string DetailPrintIn = "")
					{
						//FileLineStringList = FileLineStringListIn;
						SetOutPath = SetOutPathIn;
						SetOverwrite = SetOverwriteIn;
						ExecuteMode = ExecuteModeIn;
						FileNameOnly = FileNameOnlyIn;
						HideDetailsPrint = HideDetailsPrintIn;
						DetailPrint = DetailPrintIn;

						//TextBlockForFile = new List<string>();
					}

					public class FileOrDirToAddLine
					{
						public FileOrDirToAddLine() { }
						////public string FullLineString = "";
						//public string FilePath;
						//public Boolean PreserverAttributes;
						//public string RenameFileName;
						////public string DirPath;
						////public Boolean Recursive;
						////public string ExclusionList_CommaSeperated;
						////public Boolean FileTrue_DirectoryFalse;

						//public FileOrDirToAddLine() { }

						//public FileOrDirToAddLine(string FilePathIn, Boolean PreserverAttributesIn, string RenameFileNameIn = "")
						//{
						//    if (!File.Exists(FilePathIn)) MessageBox.Show("File does not exist: " + FilePathIn, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						//    FilePath = FilePathIn;
						//    PreserverAttributes = PreserverAttributesIn;
						//    RenameFileName = RenameFileNameIn;
						//    //FileTrue_DirectoryFalse = true;
						//}

						////public FileToAddLine(string DirPathIn, string ExclusionList_CommaSeperatedIn, Boolean RecursiveIn)
						////{
						////    if (!Directory.Exists(DirPathIn)) MessageBox.Show("Directory does not exist: " + DirPathIn, "Dir not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						////    DirPath = DirPathIn;
						////    ExclusionList_CommaSeperated = ExclusionList_CommaSeperatedIn;
						////    Recursive = RecursiveIn;
						////    FileTrue_DirectoryFalse = false;
						////}

						//public string FullLineString
						//{
						//    get
						//    {
						//        //string tmpExclusionString = "";
						//        //if (ExclusionList_CommaSeperated != null && ExclusionList_CommaSeperated.Length > 0) foreach (string s in ExclusionList_CommaSeperated.Split(',')) tmpExclusionString += "/x " + s.Trim() + " ";

						//        return
						//            //FileTrue_DirectoryFalse ?
						//            "File " + (PreserverAttributes ? "/a " : "") + (RenameFileName.Length > 0 ? @"""" + "/oname=" + RenameFileName + @""" " : "") + @"""" + FilePath + @"""";
						//          //  :
						//          //  "File " +
						//          //(Recursive ? "/r " : "") +
						//          //tmpExclusionString +
						//          //@"""" + DirPath + @"""";
						//    }
						//}

						public class FileToAddLine : FileOrDirToAddLine
						{
							//public string FullLineString = "";
							public string FilePath;
							public Boolean PreserverAttributes;
							public string RenameFileName;
							//public string DirPath;
							//public Boolean Recursive;
							//public string ExclusionList_CommaSeperated;
							//public Boolean FileTrue_DirectoryFalse;

							public FileToAddLine() { }

							public FileToAddLine(string FilePathIn, Boolean PreserverAttributesIn = true, string RenameFileNameIn = "")
							{
								if (!File.Exists(FilePathIn)) MessageBox.Show("File does not exist: " + FilePathIn, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								FilePath = FilePathIn;
								PreserverAttributes = PreserverAttributesIn;
								RenameFileName = RenameFileNameIn;
								//FileTrue_DirectoryFalse = true;
							}

							//public FileToAddLine(string DirPathIn, string ExclusionList_CommaSeperatedIn, Boolean RecursiveIn)
							//{
							//    if (!Directory.Exists(DirPathIn)) MessageBox.Show("Directory does not exist: " + DirPathIn, "Dir not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							//    DirPath = DirPathIn;
							//    ExclusionList_CommaSeperated = ExclusionList_CommaSeperatedIn;
							//    Recursive = RecursiveIn;
							//    FileTrue_DirectoryFalse = false;
							//}

							public string FullLineString
							{
								get
								{
									//string tmpExclusionString = "";
									//if (ExclusionList_CommaSeperated != null && ExclusionList_CommaSeperated.Length > 0) foreach (string s in ExclusionList_CommaSeperated.Split(',')) tmpExclusionString += "/x " + s.Trim() + " ";

									return
										//FileTrue_DirectoryFalse ?
											"File " + (PreserverAttributes ? "/a " : "") + (RenameFileName.Length > 0 ? @"""" + "/oname=" + RenameFileName + @""" " : "") + @"""" + FilePath + @"""";
									//  :
									//  "File " +
									//(Recursive ? "/r " : "") +
									//tmpExclusionString +
									//@"""" + DirPath + @"""";
								}
							}
						}

						public class DirToAddLine : FileOrDirToAddLine
						{
							//public string FullLineString = "";
							//public string FilePath;
							//public Boolean PreserverAttributes;
							//public string RenameFileName;
							public string DirPath;
							public Boolean Recursive;
							public string ExclusionList_CommaSeperated;
							//public Boolean FileTrue_DirectoryFalse;

							public DirToAddLine() { }

							//public DirToAddLine(string FilePathIn, Boolean PreserverAttributesIn, string RenameFileNameIn = "")
							//{
							//    if (!File.Exists(FilePathIn)) MessageBox.Show("File does not exist: " + FilePathIn, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							//    FilePath = FilePathIn;
							//    PreserverAttributes = PreserverAttributesIn;
							//    RenameFileName = RenameFileNameIn;
							//    FileTrue_DirectoryFalse = true;
							//}

							public DirToAddLine(string DirPathIn, string ExclusionList_CommaSeperatedIn, Boolean RecursiveIn)
							{
								if (!Directory.Exists(DirPathIn)) MessageBox.Show("Directory does not exist: " + DirPathIn, "Dir not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								DirPath = DirPathIn;
								ExclusionList_CommaSeperated = ExclusionList_CommaSeperatedIn;
								Recursive = RecursiveIn;
								//FileTrue_DirectoryFalse = false;
							}

							public string FullLineString
							{
								get
								{
									string tmpExclusionString = "";
									if (ExclusionList_CommaSeperated != null && ExclusionList_CommaSeperated.Length > 0) foreach (string s in ExclusionList_CommaSeperated.Split(',')) tmpExclusionString += "/x " + s.Trim() + " ";

									return
										//FileTrue_DirectoryFalse ?
										//"File " + (PreserverAttributes ? "/a " : "") + (RenameFileName.Length > 0 ? @"""" + "/oname=" + RenameFileName + @""" " : "") + @"""" + FilePath + @""""
										//:
											"File " +
										(Recursive ? "/r " : "") +
										tmpExclusionString +
										@"""" + DirPath + @"""";
								}
							}
						}
					}

					public override string ToString()
					{
						return SetOutPath;
					}
				}

				public class ShortcutDetails
				{
					public string RelativeShortcutPath;
					public string FullFileLocation;

					public ShortcutDetails() { }

					/// <summary>
					/// Require only the full path of the shortcut .lnk file; and the (installed) file location (can be built from enum DirectoryVariables)
					/// </summary>
					/// <param name="RelativeShortcutPathIn">The full path of the shortcut .lnk file (can be built from enum DirectoryVariables)</param>
					/// <param name="FullFileLocationIn">The file location for which the shorcut is created (can be built from enum DirectoryVariables)</param>
					public ShortcutDetails(string RelativeShortcutPathIn, string FullFileLocationIn)
					{
						RelativeShortcutPath = RelativeShortcutPathIn;
						FullFileLocation = FullFileLocationIn;
					}

					public string FullShortcutLine
					{
						get
						{
							return
									"CreateShortCut " + @"""" + @"$SMPROGRAMS\$ICONS_GROUP\" + RelativeShortcutPath + @"""" + (FullFileLocation.Length > 0 ? @" """ + FullFileLocation + @"""" : "");
						}
					}

					public override string ToString()
					{
						return FullShortcutLine;
					}
				}
			}
		}

		public static List<string> GetDescriptionOfNode(TreeNode NodeIn)
		{
			List<string> tmpList = new List<string>();

			if (NodeIn.Tag is NSISclass.SectionGroupClass)
			{
				tmpList.Add(@"!insertmacro MUI_DESCRIPTION_TEXT ${" + (NodeIn.Tag as NSISclass.SectionGroupClass).IDString + @"} """ + (NodeIn.Tag as NSISclass.SectionGroupClass).Description + @"""");
				foreach (TreeNode subnode in NodeIn.Nodes)
					foreach (string line in GetDescriptionOfNode(subnode))
						tmpList.Add(line);
			}
			if (NodeIn.Tag is NSISclass.SectionGroupClass.SectionClass)
			{
				tmpList.Add(@"!insertmacro MUI_DESCRIPTION_TEXT ${" + (NodeIn.Tag as NSISclass.SectionGroupClass.SectionClass).IDString + @"} """ + (NodeIn.Tag as NSISclass.SectionGroupClass.SectionClass).SectionDescription + @"""");
			}

			return tmpList;
			//tmpList.Add(Spacer + @"!insertmacro MUI_DESCRIPTION_TEXT ${GRP01} ""All components of the program""");
		}

		public static List<string> GetStringListOfNode(TreeNode NodeIn)
		{
			List<string> tmpList = new List<string>();

			string LevelSpaces = "";
			for (int i = 0; i < NodeIn.Level; i++) LevelSpaces += Spacer;
			string HeaderLevelSpaces = LevelSpaces.Length > 0 ? LevelSpaces.Substring(Spacer.Length) : "";

			if (NodeIn.Tag is NSISclass)
			{
				List<string> tmpSectionGroupLines = new List<string>();
				foreach (TreeNode subnode in NodeIn.Nodes)
					foreach (string line in GetStringListOfNode(subnode))
						tmpSectionGroupLines.Add(line);

				List<string> tmpSectionDescriptionLines = new List<string>();
				foreach (TreeNode subnode in NodeIn.Nodes)
					foreach (string line in GetDescriptionOfNode(subnode))
						tmpSectionDescriptionLines.Add(line);

				return ((NSISclass)NodeIn.Tag).GetAllLinesForNSISfile(tmpSectionGroupLines, tmpSectionDescriptionLines);

				//foreach (string line in ((NSISclass)NodeIn.Tag).GetAllLinesForNSISfile(tmpSectionGroupLines, tmpSectionDescriptionLines))
				//    textBox_CURRENTNODETEXTBLOCK.Text += (textBox_CURRENTNODETEXTBLOCK.Text.Length > 0 ? Environment.NewLine : "") + line;
				//listBox1.Items.Add(line);
			}
			else if (NodeIn.Tag is NSISclass.SectionGroupClass)
			{
				NSISclass.SectionGroupClass group = (NSISclass.SectionGroupClass)NodeIn.Tag;
				tmpList.Add(HeaderLevelSpaces + "SectionGroup " + (group.ExpandedByDefault ? "/e " : "") + @"""" + (group.DisplayInBold ? "!" : "") + (group.UninstallerGroup ? "un." : "") + group.Name + @""" " + group.IDString);
				foreach (TreeNode subnode in NodeIn.Nodes) foreach (string subnodeLine in GetStringListOfNode(subnode)) tmpList.Add(LevelSpaces + subnodeLine);
				tmpList.Add(HeaderLevelSpaces + "SectionGroupEnd");
			}
			else if (NodeIn.Tag is NSISclass.SectionGroupClass.SectionClass)
			{
				NSISclass.SectionGroupClass.SectionClass section = (NSISclass.SectionGroupClass.SectionClass)NodeIn.Tag;
				//tmpList.Add(section.HeaderLine);
				//foreach (string line in section.SectionTextBlock) tmpList.Add(line);
				Boolean tmpInstallforAllUsers = true;
				if (GetMainParentOfNode(NodeIn).Tag is NSISclass) tmpInstallforAllUsers = ((NSISclass)GetMainParentOfNode(NodeIn).Tag).InstallForAllUsers;

				string SectionInInstTypes = "";
				if (section.InstTypes_CommaSeperated != null && section.InstTypes_CommaSeperated.Length > 0)
				{
					SectionInInstTypes = "SectionIn";
					foreach (string insttype in section.InstTypes_CommaSeperated.Split(','))
						SectionInInstTypes += " " + ((GetMainParentOfNode(NodeIn).Tag as NSISclass).InstTypes.IndexOf(insttype) + 1);
				}

				tmpList.Add(HeaderLevelSpaces + section.HeaderLine);
				if (section.InstTypes_CommaSeperated != null && section.InstTypes_CommaSeperated.Length > 0)
					tmpList.Add(LevelSpaces + SectionInInstTypes);
				tmpList.Add(LevelSpaces + @"SetShellVarContext " + (tmpInstallforAllUsers ? "all" : "current"));
				tmpList.Add(LevelSpaces + "SetOverwrite " + section.SetOverwrite.ToString().ToLower());
				if (section.SetOutPath != null && section.SetOutPath.Length > 0) tmpList.Add(LevelSpaces + "SetOutPath " + @"""" + section.SetOutPath + @"""");
				else MessageBox.Show("SetOutPath must be set for section: " + section.SectionName, "No SetOutPath", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (section.ReserveDiskspaceForSection > 0) tmpList.Add(LevelSpaces + "AddSize " + section.ReserveDiskspaceForSection.ToString());

				List<TreeNode> FileNodes = new List<TreeNode>();
				List<TreeNode> ShortcutNodes = new List<TreeNode>();
				List<TreeNode> OtherNodes = new List<TreeNode>();
				foreach (TreeNode subnode in NodeIn.Nodes)
					if (subnode.Tag is FileToAddTextblock) FileNodes.Add(subnode);
					else if (subnode.Tag is ShortcutDetails) ShortcutNodes.Add(subnode);
					else OtherNodes.Add(subnode);

				if (FileNodes.Count > 0) tmpList.Add("");
				foreach (TreeNode node in FileNodes)
					foreach (string nodeLine in GetStringListOfNode(node)) tmpList.Add(LevelSpaces + nodeLine);

				if (ShortcutNodes.Count > 0) tmpList.Add("");
				foreach (TreeNode node in ShortcutNodes)
					foreach (string nodeLine in GetStringListOfNode(node)) tmpList.Add(LevelSpaces + nodeLine);

				if (OtherNodes.Count > 0) tmpList.Add("");
				foreach (TreeNode node in OtherNodes)
					foreach (string nodeLine in GetStringListOfNode(node)) tmpList.Add(LevelSpaces + nodeLine);

				if (FileNodes.Count > 0 || ShortcutNodes.Count > 0 || OtherNodes.Count > 0) tmpList.Add("");
				tmpList.Add(HeaderLevelSpaces + "SectionEnd");
			}
			else if (NodeIn.Tag is FileToAddTextblock)
			{
				FileToAddTextblock fileToAdd = NodeIn.Tag as FileToAddTextblock;
				//TextBlockForFile = new List<string>();
				if (fileToAdd.SetOutPath.Length > 0) tmpList.Add(LevelSpaces + "SetOutPath " + @"""" + fileToAdd.SetOutPath + @"""");
				tmpList.Add(LevelSpaces + "SetOverwrite " + fileToAdd.SetOverwrite.ToString());

				if (fileToAdd.DetailPrint.Length > 0) tmpList.Add(LevelSpaces + "DetailPrint '" + fileToAdd.DetailPrint + "'");
				if (fileToAdd.HideDetailsPrint) tmpList.Add(LevelSpaces + "SetDetailsPrint none");

				//foreach (FileToAddLine fileline in fileToAdd.FileLineStringList)
				//    if (fileline.FullLineString.Length > 0) TextBlockForFile.Add(fileline.FullLineString);
				foreach (TreeNode subnode in NodeIn.Nodes) foreach (string subnodeLine in GetStringListOfNode(subnode)) tmpList.Add(LevelSpaces + subnodeLine);
				//foreach (TreeNode subnode in NodeIn.Nodes) GetStringListOfNode(subnode);
				//if (subSubnode.Tag is FileToAddTextblock.FileToAddLine)
				//{
				//    FileToAddTextblock.FileToAddLine fileToAddLine = subSubnode.Tag as FileToAddTextblock.FileToAddLine;
				//    if (fileToAddLine.FullLineString.Length > 0) tmpList.Add(fileToAddLine.FullLineString);
				//}

				if (fileToAdd.ExecuteMode != FileToAddTextblock.ExecuteModeEnum.None)
				{
					if (fileToAdd.FileNameOnly.Length > 0)
					{
						if (fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.NormalDoDelete || fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.NormalNotDelete)
							tmpList.Add(LevelSpaces + "ExecWait '" + fileToAdd.FileNameOnly + "'");
						if (fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.QuietDoDelete || fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.QuietNotDelete)
							tmpList.Add(LevelSpaces + "ExecWait '" + fileToAdd.FileNameOnly + "' /q");
						if (fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.NormalDoDelete || fileToAdd.ExecuteMode == FileToAddTextblock.ExecuteModeEnum.QuietDoDelete)
							tmpList.Add(LevelSpaces + "Delete \"" + fileToAdd.SetOutPath + "\\" + fileToAdd.FileNameOnly + "\"");
					}
					else MessageBox.Show("FileNameOnly must be set to use ExecuteMode", "No FileName", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				if (fileToAdd.HideDetailsPrint) tmpList.Add(LevelSpaces + "SetDetailsPrint both");
			}
			else if (NodeIn.Tag is FileToAddTextblock.FileOrDirToAddLine)
			{
				//FileToAddTextblock.FileOrDirToAddLine fileToAddLine;
				if (NodeIn.Tag is FileToAddTextblock.FileOrDirToAddLine.FileToAddLine)
				{
					//fileToAddLine = NodeIn.Tag as FileToAddTextblock.FileOrDirToAddLine;
					if ((NodeIn.Tag as FileToAddTextblock.FileOrDirToAddLine.FileToAddLine).FullLineString.Length > 0) tmpList.Add(LevelSpaces + (NodeIn.Tag as FileToAddTextblock.FileOrDirToAddLine.FileToAddLine).FullLineString);
				}
				else
				{
					if ((NodeIn.Tag as FileToAddTextblock.FileOrDirToAddLine.DirToAddLine).FullLineString.Length > 0) tmpList.Add(LevelSpaces + (NodeIn.Tag as FileToAddTextblock.FileOrDirToAddLine.DirToAddLine).FullLineString);
				}
			}

			//foreach (FileToAddTextblock fta in FilesToAddTextblockList)
			//    foreach (string line in fta.TextBlockForFile)
			//        SectionTextBlock.Add(line);

			else if (NodeIn.Tag is ShortcutDetails)
			{
				ShortcutDetails shortcut = NodeIn.Tag as ShortcutDetails;
				tmpList.Add(LevelSpaces + shortcut.FullShortcutLine);
			}
			return tmpList;
		}

		public static TreeNode GetMainParentOfNode(TreeNode node)
		{
			if (node.Parent != null) return GetMainParentOfNode(node.Parent);
			else return node;
		}

		public static Boolean CheckNodeAndSubnodesForSectionWithoutInstType(TreeNode node)
		{
			if (node.Tag is NSISclass.SectionGroupClass.SectionClass)
				if ((node.Tag as NSISclass.SectionGroupClass.SectionClass).InstTypes_CommaSeperated.Length == 0)
					return true;
			foreach (TreeNode subnode in node.Nodes)
				if (CheckNodeAndSubnodesForSectionWithoutInstType(subnode))
					return true;
			return false;
		}

		public static Boolean WasInstTypesModified(List<string> OriginalList, List<string> NewList)
		{
			if (OriginalList == null && NewList == null) return false;
			if (OriginalList == null) return true;
			if (NewList == null) return true;
			if (OriginalList.Count != NewList.Count) return true;

			for (int i = 0; i < OriginalList.Count; i++)
				if (OriginalList[i] != NewList[i])
					return true;
			return false;
		}

		public static void ClearAllSectionInstTypesIfWasModified(TreeNode MainNode)
		{
			if (MainNode.Tag is NSISclass.SectionGroupClass.SectionClass)
				(MainNode.Tag as NSISclass.SectionGroupClass.SectionClass).InstTypes_CommaSeperated = "";
			for (int i = 0; i < MainNode.Nodes.Count; i++)
				ClearAllSectionInstTypesIfWasModified(MainNode.Nodes[i]);
		}

		public static class TemplateNSISnodes
		{
			public static TreeNode CSharpProject
			{
				get
				{
					Form tmpChooseAppForm = new Form();
					ListBox listBox1 = new ListBox();
					listBox1.Dock = DockStyle.Fill;
					listBox1.SelectedIndexChanged += delegate
					{
						if (listBox1.SelectedIndex != -1)
							tmpChooseAppForm.DialogResult = DialogResult.OK;
					};
					foreach (string s in GetOpenAppANDextractAppList())
						listBox1.Items.Add(new OwnAppListboxItem(Path.GetDirectoryName(s), s.Split('\\')[s.Split('\\').Length - 1].Replace(".exe", "")));

					tmpChooseAppForm.Name = "Choose app";
					tmpChooseAppForm.Size = new Size(300, 300);
					tmpChooseAppForm.StartPosition = FormStartPosition.CenterScreen;
					tmpChooseAppForm.Text = "Choose app";
					tmpChooseAppForm.TopMost = true;
					tmpChooseAppForm.Controls.Add(listBox1);

					if (tmpChooseAppForm.ShowDialog() == DialogResult.OK)
					{
						string DirectoryString = (listBox1.SelectedItem as OwnAppListboxItem).DirectoryPath;
						tmpChooseAppForm.Close();

						string AppNameIncludingEXEextension = "";
						foreach (string file in Directory.GetFiles(DirectoryString))
							if (file.ToUpper().EndsWith(".EXE"))
								AppNameIncludingEXEextension = file.Split('\\')[file.Split('\\').Length - 1];
						if (AppNameIncludingEXEextension.Length > 0)
						{
							string AppNameOnly = AppNameIncludingEXEextension.Substring(0, AppNameIncludingEXEextension.Length - 4);

							TreeNode subsubsubFileLineNode = new TreeNode(DirectoryString + "\\*.*");
							subsubsubFileLineNode.Tag = new NSISclass.SectionGroupClass.SectionClass.FileToAddTextblock.FileOrDirToAddLine.FileToAddLine(DirectoryString + "\\*.*");

							TreeNode subsubFileTextblockNode = new TreeNode("$INSTDIR");
							subsubFileTextblockNode.Tag = new NSISclass.SectionGroupClass.SectionClass.FileToAddTextblock();
							subsubFileTextblockNode.Nodes.Add(subsubsubFileLineNode);

							//TreeNode subsubShortcutNode = new TreeNode("Shortcut");
							//subsubShortcutNode.Tag = new NSISclass.SectionGroupClass.SectionClass.ShortcutDetails(AppNameOnly + ".lnk", "$INSTDIR\\" + AppNameIncludingEXEextension);

							TreeNode subSectionNode = new TreeNode("Full program");
							subSectionNode.Tag = new NSISclass.SectionGroupClass.SectionClass(
									"Full program",
									"The full package",
									"");
							//subSectionNode.Nodes.Add(subsubShortcutNode);
							subSectionNode.Nodes.Add(subsubFileTextblockNode);

							TreeNode tmpMainNode = new TreeNode(AppNameOnly);
							tmpMainNode.Tag = new NSISclass(
									AppNameOnly,
									"0.0",
									"Francois Hill",
									"www.francoishill.com",
									AppNameIncludingEXEextension,
									new Compressor(),
									AppNameOnly + "_Setup_0.0.exe",//" 1_0_0_0 setup.exe",
									LanguagesEnum.English,
									true,
									true,
									new LicensePageDetails(false),
									true,
									true,
									true,
									AppNameIncludingEXEextension,
									new List<string>() { });
							tmpMainNode.Nodes.Add(subSectionNode);

							return tmpMainNode;
						}
					}

					//if (fbd.ShowDialog(AlwaysOnTopForm) == DialogResult.OK)
					//{
					//    if (fbd.SelectedPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + @"Visual Studio 2010\Projects\"))
					//    {
					//        TreeNode tmpNode = new TreeNode();

					//        AlwaysOnTopForm = null;
					//        return tmpNode;
					//    }
					//}
					return null;
				}
			}

			private class OwnAppListboxItem
			{
				public string DirectoryPath;//like c:\francois
				public string DisplayString;
				public OwnAppListboxItem(string DirectoryPathIn, string DisplayStringIn)
				{
					DirectoryPath = DirectoryPathIn;
					DisplayString = DisplayStringIn;
				}
				public override string ToString()
				{
					return DisplayString;
				}
			}

			private static List<string> GetOpenAppANDextractAppList()
			{
				List<string> tmpList = new List<string>();

				Form AlwaysOnTopForm = new Form();
				AlwaysOnTopForm.TopMost = true;
				string RootPath = null;
				if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Local Settings\Apps\2.0"))
				{
					foreach (String dir1 in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Local Settings\Apps\2.0"))
					{
						String tmpFolderName = dir1.Split('\\')[dir1.Split('\\').Length - 1];
						if (tmpFolderName.Contains('.'))
						{
							foreach (String dir2 in Directory.GetDirectories(dir1))
							{
								tmpFolderName = dir2.Split('\\')[dir2.Split('\\').Length - 1];
								if (tmpFolderName.Contains('.'))
								{
									RootPath = dir2;
									break;
								}
							}
						}
					}
				}

				if (RootPath == null)
				{
					MessageBox.Show(AlwaysOnTopForm, "Root path not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return tmpList;
				}
				else
				{

					//AppFolderFullPathListExtractInstallFiles.Clear();
					//MessageBox.Show(RootPath);
					String[] Dirs = Directory.GetDirectories(RootPath);
					List<String> OnlyFolderNames = new List<string>();
					List<String> OnlyNewestFullPaths = new List<string>();
					List<String> tmpCodeList = new List<string>(); //list of the different codes i.e. the first 27 letters
					foreach (String fulldir in Dirs)
					{
						String foldername = fulldir.Split('\\')[fulldir.Split('\\').Length - 1];
						if (foldername.Length >= 27)
						{
							String tmpCode = foldername.Substring(0, 27);
							if (!tmpCodeList.Contains(tmpCode)) tmpCodeList.Add(tmpCode);
						}
					}
					foreach (String code in tmpCodeList)
					{
						List<String> fullDirsStartingWithCode = new List<string>();
						foreach (String dir in Dirs)
						{
							String tmpFolderName = dir.Split('\\')[dir.Split('\\').Length - 1];
							if (tmpFolderName.StartsWith(code) && !tmpFolderName.Contains("none") && !tmpFolderName.Contains("manifests"))
								fullDirsStartingWithCode.Add(dir);
						}

						DirectoryInfo tmpNewestDir = null;
						if (fullDirsStartingWithCode.Count != 0)
						{
							foreach (String dirStartWithCode in fullDirsStartingWithCode)
							{
								if (tmpNewestDir == null) tmpNewestDir = new DirectoryInfo(dirStartWithCode);
								else
								{
									DirectoryInfo tmpThisDir = new DirectoryInfo(dirStartWithCode);
									DirectoryInfo tmpCurrNewestDir = tmpNewestDir;

									if (DateTime.Compare(tmpThisDir.LastWriteTime, tmpCurrNewestDir.LastWriteTime) > 0) tmpNewestDir = new DirectoryInfo(dirStartWithCode);
								}
							}

							if (tmpNewestDir != null)
							{
								OnlyNewestFullPaths.Add(tmpNewestDir.FullName);
							}
							else MessageBox.Show("Invalid directory error", "Directory error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}

					//ToolStripMenuItem OpenAppItem = new ToolStripMenuItem("Open app");
					foreach (String newestpath in OnlyNewestFullPaths)
					{
						foreach (String file in Directory.GetFiles(newestpath))
						{
							if (file.ToUpper().EndsWith(".EXE"))
							{
								tmpList.Add(file);
							}
						}
					}
				}

				return tmpList;
			}
		}
	}
}