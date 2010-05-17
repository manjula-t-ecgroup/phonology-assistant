// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.   
// <copyright from='2005' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: Pa.cs
// Responsibility: David O
// 
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using SIL.FieldWorks.Common.UIAdapters;
using SIL.Localization;
using SIL.Pa.Model;
using SIL.Pa.PhoneticSearching;
using SIL.Pa.Processing;
using SIL.Pa.Properties;
using SIL.Pa.ResourceStuff;
using SilUtils;
using SilUtils.Controls;

namespace SIL.Pa
{
	#region ITabView interface
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface ITabView
	{
		void SetViewActive(bool makeActive, bool isDocked);
		bool ActiveView { get; }
		Form OwningForm { get;}
		ITMAdapter TMAdapter { get;}
	}

	#endregion

	#region IUndockedViewWnd interface
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface IUndockedViewWnd
	{
		ToolStripProgressBar ProgressBar { get;}
		ToolStripStatusLabel ProgressBarLabel { get;}
		ToolStripStatusLabel StatusBarLabel { get;}
		StatusStrip StatusBar { get; }
	}

	#endregion

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ------------------------------------------------------------------------------------
	public static class App
	{
		public enum FeatureType
		{
			Articulatory,
			Binary
		}

		#region Constants
		public static char[] kTieBars = new[] { '\u0361', '\u035C' };
		public const string kTopTieBar = "\u0361";
		public const string kBottomTieBar = "\u035C";
		public const char kTopTieBarC = '\u0361';
		public const char kBottomTieBarC = '\u035C';
		public const string kDottedCircle = "\u25CC";
		public const char kDottedCircleC = '\u25CC';
		public const char kOrc = '\uFFFC';
		public const string kDiacriticPlaceholder = "[" + kDottedCircle + "]";
		public const string kSearchPatternDiamond = "\u25CA";
		public const string kEmptyDiamondPattern = kSearchPatternDiamond + "/" + kSearchPatternDiamond + "_" + kSearchPatternDiamond;

		public const string kHelpFileName = "Phonology_Assistant_Help.chm";
		public const string kHelpSubFolder = "Helps";
		public const string kTrainingSubFolder = "Training";
		public const string kPaRegKeyName = @"Software\SIL\Phonology Assistant";
		public const string kAppSettingsName = "application";

		public const string kLocalizationGroupTMItems = "Toolbar and Menu Items";
		public const string kLocalizationGroupUICtrls = "User Interface Controls";
		public const string kLocalizationGroupDialogs = "Dialog Boxes";
		public const string kLocalizationGroupInfoMsg = "Information Messages";
		public const string kLocalizationGroupMisc = "Miscellaneous Strings";

		//public static string kOpenClassBracket;
		//public static string kCloseClassBracket;
		//public static string kstidFileTypeAllExe;
		//public static string kstidFileTypeAllFiles;
		//public static string kstidFileTypeHTML;
		//public static string kstidFileTypeWordXml;
		//public static string kstidFileTypePAXML;
		//public static string kstidFileTypePAProject;
		//public static string kstidFiletypeRTF;
		//public static string kstidFiletypeSASoundMP3;
		//public static string kstidFiletypeSASoundWave;
		//public static string kstidFiletypeSASoundWMA;
		//public static string kstidFileTypeToolboxDB;
		//public static string kstidFileTypeToolboxITX;
		//public static string kstidFileTypeXML;
		//public static string kstidFileTypeXSLT;
		//public static string kstidQuerySearchingMsg;
		//public static string kstidSaveChangesMsg;
		//public static string kstidSaveFileDialogGenericCaption;

		#endregion

		private static string s_helpFilePath;
		private static ToolStripStatusLabel s_statusBarLabel;
		private static ToolStripProgressBar s_progressBar;
		private static ToolStripStatusLabel s_progressBarLabel;
		private static ToolStripProgressBar s_activeProgressBar;
		private static ToolStripStatusLabel s_activeProgBarLabel;
		private static PaFieldInfoList s_fieldInfo;
		private static List<ITMAdapter> s_defaultMenuAdapters;
		private static readonly Dictionary<Type, Form> s_openForms = new Dictionary<Type, Form>();
		private static readonly List<IxCoreColleague> s_colleagueList = new List<IxCoreColleague>();

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// --------------------------------------------------------------------------------
		static App()
		{
			if (DesignMode)
				return;
			
			InitializePaRegKey();
			SettingsFile = Path.Combine(DefaultProjectFolder, "pa.xml");
			SettingsHandler = new PaSettingsHandler(SettingsFile);
			MsgMediator = new Mediator();

			PortableSettingsProvider.SettingsFileFolder = DefaultProjectFolder;
			
			ProcessHelper.CopyFilesForPrettyHTMLExports();

			LocalizationManager.Initialize(Path.Combine(DefaultProjectFolder, "Localizations"));
			LocalizationManager.DefaultStringGroup = kLocalizationGroupMisc;
			SetUILanguage();

			// Create the master set of PA fields. When a project is opened, any
			// custom fields belonging to the project will be added to this list.
			s_fieldInfo = PaFieldInfoList.DefaultFieldInfoList;

			MinimumViewWindowSize = Settings.Default.MinimumViewWindowSize;
			FwDBUtils.ShowMsgWhenGatheringFWInfo = Settings.Default.ShowMsgWhenGatheringFwInfo;

			var chrs = Settings.Default.UncertainGroupAbsentPhoneChars;
			if (!string.IsNullOrEmpty(chrs))
				IPASymbolCache.UncertainGroupAbsentPhoneChars = chrs;

			chrs = Settings.Default.UncertainGroupAbsentPhoneChar;
			if (!string.IsNullOrEmpty(chrs))
				IPASymbolCache.UncertainGroupAbsentPhoneChar = chrs;

			ReadAddOns();

			LocalizeItemDlg.SetDialogSplitterPosition += LocalizeItemDlg_SetDialogSplitterPosition;
			LocalizeItemDlg.SaveDialogSplitterPosition += LocalizeItemDlg_SaveDialogSplitterPosition;
			LocalizeItemDlg.SetDialogBounds += LocalizeItemDlg_SetDialogBounds;
			LocalizeItemDlg.SaveDialogBounds += LocalizeItemDlg_SaveDialogBounds;

			// Load the cache of IPA symbols, articulatory and binary features.
			InventoryHelper.Load();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void SetUILanguage()
		{
			string langId = Settings.Default.UserInterfaceLanguage;

			// Specifying the UI language on the command-line trumps the one in
			// the settings file (i.e. the one set in the options dialog box).
			foreach (var arg in Environment.GetCommandLineArgs())
			{
				if (arg.ToLower().StartsWith("/uilang:") || arg.ToLower().StartsWith("-uilang:"))
				{
					langId = arg.Substring(8);
					break;
				}
			}

			LocalizationManager.UILangId = (string.IsNullOrEmpty(langId) ?
				LocalizationManager.kDefaultLang : langId);
		}

		#region Misc. localized global strings
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kOpenClassBracket
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"OpenClassSymbol", "<",
					"Character used to delineate the opening of a phonetic search class.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kCloseClassBracket
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"CloseClassSymbol", ">",
					"Character used to delineate the closing of a phonetic search class.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeAllExe
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.ExecutableFileTypes",
					"All Executables (*.exe;*.com;*.pif;*.bat;*.cmd)|*.exe;*.com;*.pif;*.bat;*.cmd",
					"File types for executable files.", kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeAllFiles
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.AllFileTypes",
					"All Files (*.*)|*.*", "Used in open/save file dialogs as the type for all files.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeHTML
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"FileTypes.HTMLFileType", "HTML Files (*.html)|*.html");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeWordXml
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"FileTypes.Word2003XmlFileType", "Word 2003 XML Files (*.xml)|*.xml");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypePAXML
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.PaXMLFileType",
					"{0} XML Files (*.paxml)|*.paxml", "Parameter is the application name.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypePAProject
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.PaProjectFileType",
					"{0} Projects (*.pap)|*.pap",
					"File type for Phonology Assistant projects. The parameter is the application name.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFiletypeRTF
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.RTFFileType",
					"Rich Text Format (*.rtf)|*.rtf", "File type for rich text format output.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFiletypeSASoundMP3
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"FileTypes.Mp3FileType", "Speech Analyzer MP3 Files (*.mp3)|*.mp3");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFiletypeSASoundWave
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"FileTypes.WaveFileType", "Speech Analyzer Wave Files (*.wav)|*.wav");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFiletypeSASoundWMA
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"FileTypes.WindowsMediaAudioFileType", "Speech Analyzer WMA Files (*.wma)|*.wma");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeToolboxDB
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.ToolboxFileType",
					"Toolbox Files (*.db)|*.db");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeToolboxITX
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.ToolboxInterlinearFileType",
					"Interlinear Toolbox Files (*.itx)|*.itx");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeXML
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.XmlFileType",
					"XML Files (*.xml)|*.xml");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeXSLT
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.XsltTFileType",
					"XSLT Files (*.xslt)|*.xslt");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidFileTypeZip
		{
			get
			{
				return LocalizationManager.LocalizeString("FileTypes.ZipFileType",
					"Zip Files (*.zip)|*.zip");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidQuerySearchingMsg
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"PhoneticSearchingInProgressMessage", "Searching...",
					"Message displayed in status bar next to the progress bar when doing a query searches.",
					kLocalizationGroupMisc);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidSaveChangesMsg
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"GenericSaveChangesQuestion", "Would you like to save your changes?");
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string kstidSaveFileDialogGenericCaption
		{
			get
			{
				return LocalizationManager.LocalizeString(
					"GenericSaveFileDialogCaption", "Save File");
			}
		}

		#endregion

		#region event handlers for saving and restoring localization dialog settings
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Set the localization dialog's size and location.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static void LocalizeItemDlg_SetDialogBounds(LocalizeItemDlg dlg)
		{
			SettingsHandler.LoadFormProperties(dlg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves localization dialog size and location.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static void LocalizeItemDlg_SaveDialogBounds(LocalizeItemDlg dlg)
		{
			SettingsHandler.SaveFormProperties(dlg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the saved splitter distance value for Localizing dialog box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static int LocalizeItemDlg_SetDialogSplitterPosition()
		{
			return SettingsHandler.GetIntSettingsValue("LocalizeDlg", "splitdistance", 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the splitter distance value for Localizing dialog box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static void LocalizeItemDlg_SaveDialogSplitterPosition(int pos)
		{
			SettingsHandler.SaveSettingsValue("LocalizeDlg", "splitdistance", pos);
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void ShowSplashScreen()
		{
			if (Settings.Default.ShowSplashScreen)
			{
				SplashScreen = new SplashScreen(true, VersionType.Alpha);
				SplashScreen.Show();
				SplashScreen.Message = Properties.Resources.kstidSplashScreenLoadingMsg;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Add Ons are undocumented and it's assumed that each add-on assembly contains at
		/// least a class called "PaAddOnManager". If a class by that name is not found in
		/// an assembly in the AddOns folder, then it's not considered to be an AddOn
		/// assembly for PA. It's up to the PaAddOnManager class in each add-on to do all
		/// the proper initialization it needs. There's nothing in the PA code that recognizes
		/// AddOns. It's all up to the Add On to reference the PA code, not the other way
		/// around. So, if an Add On needs to add a menu to the main menu, it's up to the
		/// add on to do it.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void ReadAddOns()
		{
			if (DesignMode)
				return;

			string[] addOnAssemblyFiles;

			try
			{
				var addOnPath = Path.Combine(AssemblyPath, "AddOns");
				addOnAssemblyFiles = Directory.GetFiles(addOnPath, "*.dll");
			}
			catch
			{
				return;
			}

			if (addOnAssemblyFiles == null || addOnAssemblyFiles.Length == 0)
				return;

			foreach (string filename in addOnAssemblyFiles)
			{
				try
				{
					Assembly assembly = ReflectionHelper.LoadAssembly(filename);
					if (assembly != null)
					{
						object instance =
							ReflectionHelper.CreateClassInstance(assembly, "PaAddOnManager");

						if (instance != null)
						{
							if (AddOnAssemblys == null)
								AddOnAssemblys = new List<Assembly>();

							if (AddOnManagers == null)
								AddOnManagers = new List<object>();

							AddOnAssemblys.Add(assembly);
							AddOnManagers.Add(instance);
						}
					}
				}
				catch { }
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void InitializePaRegKey()
		{
			string projPath = GetDefaultProjectFolder();

			// Check if an entry in the registry specifies the default project path.
			RegistryKey key = Registry.CurrentUser.CreateSubKey(kPaRegKeyName);

			if (key != null)
			{
				string tmpProjPath = key.GetValue("DefaultProjectsLocation") as string;

				// If the registry value was not found, then create it. Otherwise, use
				// the path found in the registry and not the one constructed above.
				if (string.IsNullOrEmpty(tmpProjPath))
					key.SetValue("DefaultProjectsLocation", projPath);
				else
					projPath = tmpProjPath;

				key.Close();
			}

			DefaultProjectFolder = projPath;

			// Create the folder if it doesn't exist.
			if (!Directory.Exists(projPath))
				Directory.CreateDirectory(projPath);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Construct the default project path.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string GetDefaultProjectFolder()
		{
			// Construct the default project path.
			string projPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			// I found that in limited user mode on Vista, Environment.SpecialFolder.MyDocuments
			// returns an empty string. Argh!!! Therefore, I have to make sure there is
			// a valid and full path. Do that by getting the user's desktop folder and
			// chopping off everything that follows the last backslash. If getting the user's
			// desktop folder fails, then fall back to the program's folder, which is
			// probably not right, but we'll have to assume it will never happen. :o)
			if (string.IsNullOrEmpty(projPath))
			{
				projPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				if (string.IsNullOrEmpty(projPath) || !Directory.Exists(projPath))
					return AssemblyPath;

				projPath = projPath.TrimEnd('\\');
				int i = projPath.LastIndexOf('\\');
				projPath = projPath.Substring(0, i);
			}

			return Path.Combine(projPath, Properties.Resources.kstidDefaultProjFileFolderName);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The normal DesignMode property doesn't work when derived classes are loaded in
		/// designer.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool DesignMode
		{
			get { return Process.GetCurrentProcess().ProcessName == "devenv"; }
		}

		#region Cache related properties and methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the IPA symbols cache.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static IPASymbolCache IPASymbolCache
		{
			get { return InventoryHelper.IPASymbolCache; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache of articulatory features.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static AFeatureCache AFeatureCache
		{
			get { return InventoryHelper.AFeatureCache; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache of binary features.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static BFeatureCache BFeatureCache
		{
			get { return InventoryHelper.BFeatureCache; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static RecordCache RecordCache { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static WordCache WordCache { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache of phones in the current project, without respect to current filter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static PhoneCache UnfilteredPhoneCache { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the cache of phones in the current project, with respect to current filter.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static PhoneCache PhoneCache { get; set; }
		
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void AddMediatorColleague(IxCoreColleague colleague)
		{
			if (colleague != null && !DesignMode)
			{
				MsgMediator.AddColleague(colleague);

				if (!s_colleagueList.Contains(colleague))
					s_colleagueList.Add(colleague);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void RemoveMediatorColleague(IxCoreColleague colleague)
		{
			if (colleague != null && !DesignMode)
			{
				try
				{
					MsgMediator.RemoveColleague(colleague);
				}
				catch { }

				if (s_colleagueList.Contains(colleague))
					s_colleagueList.Remove(colleague);
			}
		}

		#region Misc. Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string BreakChars
		{
			get
			{
				return (string.IsNullOrEmpty(Settings.Default.WordBreakCharacters) ?
					" " : Settings.Default.WordBreakCharacters);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the XCore message mediator for the application.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Mediator MsgMediator { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the minimum allowed size of a view's window, including the main window that
		/// can hold all the views.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Size MinimumViewWindowSize { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the default location for PA projects.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string DefaultProjectFolder { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the application's splash screen.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ISplashScreen SplashScreen { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// When running normally, this should be the same as Application.StartupPath. When
		/// running tests, this will be the folder that contains the assembly in which this
		/// class is found.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string AssemblyPath
		{
			get
			{
				// CodeBase prepends "file:/", which must be removed.
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the path where the application's factory configuration files are stored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string ConfigFolder
		{
			get { return Path.Combine(AssemblyPath, "Configuration"); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the path where the application's processing files (i.e. xslt) are stored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string ProcessingFolder
		{
			get { return Path.Combine(AssemblyPath, "Processing"); }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the currently opened PA project.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static PaProject Project { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether or not a project is being loaded.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool ProjectLoadInProcess { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static PaFieldInfoList FieldInfo
		{
			get
			{
				if (s_fieldInfo == null)
					s_fieldInfo = PaFieldInfoList.DefaultFieldInfoList;

				return s_fieldInfo;
			}
			set { s_fieldInfo = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the toolbar menu adapter PaMainWnd. This value should only be set
		/// by the PaMainWnd class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ITMAdapter TMAdapter { get; set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full path and filename of the XML file that stores the application's
		/// settings.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static string SettingsFile { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static PaSettingsHandler SettingsHandler { get; private set; }

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full path and filename of the application's help file.
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static string HelpFile
		{
			get { return null; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the application's main form.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Form MainForm { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the application's current view form. When the view is docked, then
		/// this form will not be visible.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ITabView CurrentView { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the application's current view type.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Type CurrentViewType { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the main status bar label on PaMainWnd.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ToolStripStatusLabel StatusBarLabel
		{
			get
			{
				IUndockedViewWnd udvwnd = (CurrentView != null && CurrentView.ActiveView ?
					CurrentView.OwningForm : MainForm) as IUndockedViewWnd;

				return (udvwnd != null ? udvwnd.StatusBarLabel : s_statusBarLabel);
			}
			set { s_statusBarLabel = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the progress bar on the PaMainWnd.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ToolStripProgressBar ProgressBar
		{
			get
			{
				IUndockedViewWnd udvwnd = (CurrentView != null && CurrentView.ActiveView ?
					CurrentView.OwningForm : MainForm) as IUndockedViewWnd;

				return (udvwnd != null ? udvwnd.ProgressBar : s_progressBar);
			}
			set { s_progressBar = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the progress bar's label on the PaMainWnd.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ToolStripStatusLabel ProgressBarLabel
		{
			get
			{
				IUndockedViewWnd udvwnd = (CurrentView != null && CurrentView.ActiveView ?
					CurrentView.OwningForm : MainForm) as IUndockedViewWnd;

				return (udvwnd != null ? udvwnd.ProgressBarLabel : s_progressBarLabel);
			}
			set { s_progressBarLabel = value; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// PA add-on DLL provides undocumented features, if it exists in the pa.exe folder.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static List<Assembly> AddOnAssemblys { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// PA add-on manager provides undocumented features, if it exists.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static List<object> AddOnManagers { get; private set; }

		#endregion

		#region Options Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color WordListGridColor
		{
			get
			{
				var clr = Settings.Default.WordListGridColor;
				return (clr != Color.Transparent && clr != Color.Empty ? clr :
					ColorHelper.CalculateColor(SystemColors.WindowText, SystemColors.Window, 25));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedFocusedWordListRowBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"selectedfocusedwordlistrowcolors", "background", ColorHelper.LightHighlight);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedfocusedwordlistrowcolors", "background", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedFocusedWordListRowForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"selectedfocusedwordlistrowcolors", "foreground", SystemColors.WindowText);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedfocusedwordlistrowcolors", "foreground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedUnFocusedWordListRowBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"selectedunfocusedwordlistrowcolors", "background", SystemColors.Control);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedunfocusedwordlistrowcolors", "background", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedUnFocusedWordListRowForeColor
		{
			get
			{
				// It turns out the control color for the silver Windows XP theme is very
				// close to the default color calculated for selected rows in PA word lists.
				// Therefore, when a word list grid looses focus and a selected row's
				// background color gets changed to the control color, it's very hard to
				// tell the difference between a selected row in a focused grid from that
				// of a non focused grid. So, when the theme is the silver (i.e. Metallic)
				// then also make the text gray for selected rows in non focused grid's.
				if (PaintingHelper.CanPaintVisualStyle() &&
					System.Windows.Forms.VisualStyles.VisualStyleInformation.DisplayName == "Windows XP style" &&
					System.Windows.Forms.VisualStyles.VisualStyleInformation.ColorScheme == "Metallic")
				{
					return SettingsHandler.GetColorSettingsValue(
						"selectedunfocusedwordlistrowcolors", "foreground", SystemColors.GrayText);
				}

				return SettingsHandler.GetColorSettingsValue(
					"selectedunfocusedwordlistrowcolors", "foreground", SystemColors.ControlText);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedunfocusedwordlistrowcolors", "foreground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedWordListCellBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"selectedwordlistcellcolors", "background", ColorHelper.LightLightHighlight);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedwordlistcellcolors", "background", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color SelectedWordListCellForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"selectedwordlistcellcolors", "foreground", SystemColors.WindowText);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"selectedwordlistcellcolors", "foreground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color UncertainPhoneForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"uncertainphonecolors", "foreground", Color.RoyalBlue);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"uncertainphonecolors", "foreground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color QuerySearchItemForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"searchresultcolors", "srchitemforeground", Color.Black);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"searchresultcolors", "srchitemforeground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color QuerySearchItemBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"searchresultcolors", "srchitembackground", Color.Yellow);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"searchresultcolors", "srchitembackground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the background color for result cells in an XY grid whose value is
		/// zero.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color XYChartZeroBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"xychartcolors", "zerobackground", Color.PaleGoldenrod);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"xychartcolors", "zerobackground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the foreground color for result cells in an XY grid whose value is
		/// zero.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color XYChartZeroForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"xychartcolors", "zeroforeground", Color.Black);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"xychartcolors", "zeroforeground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the background color for result cells in an XY grid whose value is
		/// greater than zero.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color XYChartNonZeroBackColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"xychartcolors", "nonzerobackground", Color.LightSteelBlue);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"xychartcolors", "nonzerobackground", value);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the foreground color for result cells in an XY grid whose value is
		/// greater than zero.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Color XYChartNonZeroForeColor
		{
			get
			{
				return SettingsHandler.GetColorSettingsValue(
					"xychartcolors", "nonzeroforeground", Color.Black);
			}
			set
			{
				SettingsHandler.SaveSettingsValue(
					"xychartcolors", "nonzeroforeground", value);
			}
		}

		#endregion

		#region Misc. methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static FormSettings InitializeForm(Form frm, FormSettings settings)
		{
			if (settings != null)
				settings.InitializeForm(frm);
			else
				settings = FormSettings.Create(frm);

			return settings;
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the default phonology assistant menu in the specified container control.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ITMAdapter LoadDefaultMenu(Control menuContainer)
		{
			ITMAdapter adapter = AdapterHelper.CreateTMAdapter();

			if (adapter != null)
			{
				PrepareAdapterForLocalizationSupport(adapter);

				string[] defs = new string[1];
				defs[0] = Path.Combine(ConfigFolder, "PaTMDefinition.xml");
				adapter.Initialize(menuContainer, MsgMediator, ApplicationRegKeyPath, defs);
				adapter.AllowUpdates = true;
				adapter.RecentFilesList = (MruProjects.Paths ?? new string[] { });
//				adapter.RecentFilesList = RecentlyUsedProjectList;
				adapter.RecentlyUsedItemChosen += (filename => MsgMediator.SendMessage("RecentlyUsedProjectChosen", filename));
			}

			if (s_defaultMenuAdapters == null)
				s_defaultMenuAdapters = new List<ITMAdapter>();

			s_defaultMenuAdapters.Add(adapter);
			return adapter;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Prepares the adapter for localization support.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void PrepareAdapterForLocalizationSupport(ITMAdapter adapter)
		{
			adapter.LocalizeItem += HandleLocalizingTMItem;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Reverses what the method PrepareAdapterForLocalizationSupport does to prepare
		/// the specified adapter for localization.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void UnPrepareAdapterForLocalizationSupport(ITMAdapter adapter)
		{
			adapter.LocalizeItem -= HandleLocalizingTMItem;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handles localizing a toolbar/menu item.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		static void HandleLocalizingTMItem(object item, string id, TMItemProperties itemProps)
		{
			LocalizationManager.LocalizeObject(item, id, itemProps.Text, itemProps.Tooltip,
				ShortcutKeysEditor.KeysToString(itemProps.ShortcutKey), "Toolbar or Menu item",
				kLocalizationGroupTMItems, LocalizationPriority.High);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void UnloadDefaultMenu(ITMAdapter adapter)
		{
			if (s_defaultMenuAdapters != null && s_defaultMenuAdapters.Contains(adapter))
				s_defaultMenuAdapters.Remove(adapter);

			if (adapter != null)
			{
				adapter.Dispose();
				adapter = null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified file to the recently used projects list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void AddProjectToRecentlyUsedProjectsList(string filename)
		{
			AddProjectToRecentlyUsedProjectsList(filename, false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified file to the recently used projects list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void AddProjectToRecentlyUsedProjectsList(string filename, bool addToEnd)
		{
			MruProjects.AddNewPath(filename, addToEnd);
			MruProjects.Save();

			if (s_defaultMenuAdapters != null)
			{
				foreach (ITMAdapter adapter in s_defaultMenuAdapters)
					adapter.RecentFilesList = (MruProjects.Paths ?? new string[] {});
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the top-level registry key path to the application's registry entry.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string ApplicationRegKeyPath
		{
			get { return @"Software\SIL\Phonology Assistant"; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Enables or disables a TM item based on whether or not there is project loaded.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void EnableWhenProjectOpen(TMItemProperties itemProps)
		{
			bool enable = (Project != null);

			if (itemProps != null && itemProps.Enabled != enable)
			{
				itemProps.Visible = true;
				itemProps.Enabled = enable;
				itemProps.Update = true;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Enables or disables a TM item based on whether or not there is a project loaded
		/// and the specified view type is current.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool DetermineMenuStateBasedOnViewType(TMItemProperties itemProps, Type viewType)
		{
			bool enable = (Project != null && CurrentViewType != viewType);

			if (itemProps != null && itemProps.Enabled != enable)
			{
				itemProps.Visible = true;
				itemProps.Enabled = enable;
				itemProps.Update = true;
			}

			return (itemProps != null && CurrentViewType == viewType);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the progress bar, assuming the max. value will be the count of items
		/// in the current project's word cache.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ToolStripProgressBar InitializeProgressBar(string text)
		{
			return InitializeProgressBar(text, WordCache.Count);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static ToolStripProgressBar InitializeProgressBar(string text, int maxValue)
		{
			IUndockedViewWnd udvwnd = (CurrentView != null && CurrentView.ActiveView ?
				CurrentView.OwningForm : MainForm) as IUndockedViewWnd;

			ToolStripProgressBar bar =
				(udvwnd != null ? udvwnd.ProgressBar : s_progressBar);

			ToolStripStatusLabel lbl =
				(udvwnd != null ? udvwnd.ProgressBarLabel : s_progressBarLabel);

			if (bar != null)
			{
				bar.Maximum = maxValue;
				bar.Value = 0;
				lbl.Text = text;
				lbl.Visible = bar.Visible = true;
				s_activeProgBarLabel = lbl;
				s_activeProgressBar = bar;
				Utils.WaitCursors(true);
			}

			return s_progressBar;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the progress bar for the specified view.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void InitializeProgressBarForLoadingView(string msg, int maxValue)
		{
			InitializeProgressBar(msg, maxValue);

			if (SplashScreen != null && SplashScreen.StillAlive)
				SplashScreen.Message = msg;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void UninitializeProgressBar()
		{
			ToolStripProgressBar bar = (s_activeProgressBar ?? s_progressBar);
			ToolStripStatusLabel lbl = (s_activeProgBarLabel ?? s_progressBarLabel);

			if (bar != null)
				bar.Visible = false;

			if (lbl != null)
				lbl.Visible = false;

			s_activeProgBarLabel = null;
			s_activeProgressBar = null;
			Utils.WaitCursors(false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Increments the progress bar by one.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void IncProgressBar()
		{
			IncProgressBar(1);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Increments the progress bar by the specified amount. Passing -1 to this method
		/// will cause the progress bar to go to it's max. value.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void IncProgressBar(int amount)
		{
			ToolStripProgressBar bar = (s_activeProgressBar ?? s_progressBar);

			if (bar != null)
			{
				if (amount != -1 && (bar.Value + amount) <= bar.Maximum)
					bar.Value += amount;
				else
					bar.Value = bar.Maximum;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the progress bar label.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void UpdateProgressBarLabel(string label)
		{
			ToolStripStatusLabel lbl = (s_activeProgBarLabel ?? s_progressBarLabel);

			if (lbl != null)
			{
				if (SplashScreen != null && SplashScreen.StillAlive)
					SplashScreen.Message = label;

				lbl.Text = label;
				Application.DoEvents();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the splash screen if it's showing.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void CloseSplashScreen()
		{
			if (SplashScreen != null && SplashScreen.StillAlive)
			{
				Application.DoEvents();
				if (MainForm != null)
					MainForm.Activate();

				SplashScreen.Close();
			}

			SplashScreen = null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string OpenFileDialog(string defaultFileType, string filter, string dlgTitle)
		{
			int filterIndex = 0;
			return OpenFileDialog(defaultFileType, filter, ref filterIndex, dlgTitle);
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static string OpenFileDialog(string defaultFileType, string filter,
			ref int filterIndex, string dlgTitle)
		{
			string[] filenames = OpenFileDialog(defaultFileType, filter, ref filterIndex,
				dlgTitle, false, null);

			return (filenames == null || filenames.Length == 0 ? null : filenames[0]);
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static string[] OpenFileDialog(string defaultFileType, string filter,
			ref int filterIndex, string dlgTitle, bool multiSelect)
		{
			return OpenFileDialog(defaultFileType, filter, ref filterIndex,
				dlgTitle, multiSelect, null);
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// --------------------------------------------------------------------------------
		public static string[] OpenFileDialog(string defaultFileType, string filter,
			ref int filterIndex, string dlgTitle, bool multiSelect, string initialDirectory)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.CheckFileExists = true;
			dlg.CheckPathExists = true;
			dlg.DefaultExt = defaultFileType;
			dlg.Filter = filter;
			dlg.Multiselect = multiSelect;
			dlg.ShowReadOnly = false;
			dlg.ShowHelp = false;
			dlg.Title = dlgTitle;
			dlg.RestoreDirectory = false;
			dlg.InitialDirectory = (initialDirectory ?? Environment.CurrentDirectory);

			if (filterIndex > 0)
				dlg.FilterIndex = filterIndex;

			dlg.ShowDialog();

			filterIndex = dlg.FilterIndex;
			return dlg.FileNames;
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Open the Save File dialog.
		/// </summary>
		/// <param name="defaultFileType">Default file type to save</param>
		/// <param name="filter">The parent's saved filter</param>
		/// <param name="filterIndex">The new filter index</param>
		/// <param name="dlgTitle">Title of the Save File dialog</param>
		/// <param name="initialDir">Directory where the dialog starts</param>
		/// --------------------------------------------------------------------------------
		public static string SaveFileDialog(string defaultFileType, string filter,
			ref int filterIndex, string dlgTitle, string initialDir)
		{
			return SaveFileDialog(defaultFileType, filter, ref filterIndex, dlgTitle,
				null, initialDir);
		}

		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Open the Save File dialog.
		/// </summary>
		/// <param name="defaultFileType">Default file type to save</param>
		/// <param name="filter">The parent's saved filter</param>
		/// <param name="filterIndex">The new filter index</param>
		/// <param name="initialFileName">The default filename</param>
		/// <param name="initialDir">Directory where the dialog starts</param>
		/// <param name="dlgTitle">Title of the Save File dialog</param>
		/// --------------------------------------------------------------------------------
		public static string SaveFileDialog(string defaultFileType, string filter,
			ref int filterIndex, string dlgTitle, string initialFileName, string initialDir)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.AddExtension = true;
			dlg.DefaultExt = defaultFileType;
			dlg.OverwritePrompt = true;
			dlg.Filter = filter;
			dlg.Title = dlgTitle;
			dlg.RestoreDirectory = false;

			if (!string.IsNullOrEmpty(initialFileName))
				dlg.FileName = initialFileName;

			if (!string.IsNullOrEmpty(initialDir))
				dlg.InitialDirectory = initialDir;

			if (filterIndex > 0)
				dlg.FilterIndex = filterIndex;

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				filterIndex = dlg.FilterIndex;
				return dlg.FileName;
			}

			return string.Empty;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether or not the specified view type or form is the active.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool IsViewOrFormActive(Type viewType, Form frm)
		{
			return (viewType == CurrentViewType && frm != null && frm.ContainsFocus);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether or not the specified form is the active form.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool IsFormActive(Form frm)
		{
			if (frm == null)
				return false;

			if (frm.ContainsFocus || frm.GetType() == CurrentViewType)
				return true;

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Closes all MDI child forms.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void CloseAllForms()
		{
			s_openForms.Clear();

			if (MainForm == null)
				return;

			// There may be some child forms not in the s_openForms collection. If that's
			// the case, then close them this way.
			for (int i = MainForm.MdiChildren.Length - 1; i >= 0; i--)
				MainForm.MdiChildren[i].Close();
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and loads a result cache for the specified search query.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static WordListCache Search(SearchQuery query)
		{
			int resultCount;
			return Search(query, true, false, 1, out resultCount);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and loads a result cache for the specified search query.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static WordListCache Search(SearchQuery query, int incAmount)
		{
			int resultCount;
			return Search(query, true, false, incAmount, out resultCount);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and loads a result cache for the specified search query.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static WordListCache Search(SearchQuery query, bool incProgressBar,
			bool returnCountOnly, int incAmount, out int resultCount)
		{
			return Search(query, incProgressBar, returnCountOnly,
				true, incAmount, out resultCount);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and loads a result cache for the specified search query.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static WordListCache Search(SearchQuery query, bool incProgressBar,
			bool returnCountOnly, bool showErrMsg, int incAmount, out int resultCount)
		{
			resultCount = 0;
			bool patternContainsWordBoundaries = (query.Pattern.IndexOf('#') >= 0);
			int incCounter = 0;

			query.ErrorMessages.Clear();
			SearchQuery modifiedQuery;
			if (!ConvertClassesToPatterns(query, out modifiedQuery, showErrMsg))
				return null;

			if (Project != null)
				SearchEngine.IgnoreUndefinedCharacters = Project.IgnoreUndefinedCharsInSearches;

			SearchEngine.ConvertPatternWithTranscriptionChanges =
				Settings.Default.ConvertPatternsWithTranscriptionChanges;

			SearchEngine engine = new SearchEngine(modifiedQuery, PhoneCache);

			string msg = engine.GetCombinedErrorMessages();
			if (!string.IsNullOrEmpty(msg))
			{
				if (showErrMsg)
					Utils.MsgBox(msg);

				query.ErrorMessages.AddRange(modifiedQuery.ErrorMessages);
				resultCount = -1;
				return null;
			}

			if (!VerifyMiscPatternConditions(engine, showErrMsg))
			{
				query.ErrorMessages.AddRange(modifiedQuery.ErrorMessages);
				resultCount = -1;
				return null;
			}

			WordListCache resultCache = (returnCountOnly ? null : new WordListCache());

			foreach (WordCacheEntry wordEntry in WordCache)
			{
				if (incProgressBar && (incCounter++) == incAmount)
				{
					IncProgressBar(incAmount);
					incCounter = 0;
				}

				string[][] eticWords = new string[1][];

				if (query.IncludeAllUncertainPossibilities && wordEntry.ContiansUncertainties)
				{
					// Get a list of all the words (each word being in the form of
					// an array of phones) that can be derived from all the primary
					// and non primary uncertainties.
					eticWords = wordEntry.GetAllPossibleUncertainWords(false);

					if (eticWords == null)
						continue;
				}
				else
				{
					// Not all uncertain possibilities should be included in the search, so
					// just load up the phones that only include the primary uncertain Phone(s).
					eticWords[0] = wordEntry.Phones;
					if (eticWords[0] == null)
						continue;
				}

				// If eticWords.Length = 1 then either the word we're searching doesn't contain
				// uncertain phones or it does but they are only primary uncertain phones. When
				// eticWords.Length > 1, we know the uncertain phones in the first word are only
				// primary uncertainities while at least one phone in the remaining words is a
				// non primary uncertainy.
				for (int i = 0; i < eticWords.Length; i++)
				{
					// If the search pattern contains the word breaking character,
					// then add a space at the beginning and end of the array of
					// phones so the word breaking character has something to
					// match at the extremes of the phonetic values.
					if (patternContainsWordBoundaries)
					{
						List<string> tmpChars = new List<string>(eticWords[i]);
						tmpChars.Insert(0, " ");
						tmpChars.Add(" ");
						eticWords[i] = tmpChars.ToArray();
					}

					int[] result;
					bool matchFound = engine.SearchWord(eticWords[i], out result);
					while (matchFound)
					{
						if (returnCountOnly)
							resultCount++;
						else
						{
							// The last parameter sent to Add is true when the word contains
							// non primary phones, which will be the case when the eticWords
							// collection contains more than one word (I use the term 'word'
							// loosely since it's really just a transcription that may contain
							// spaces) and we're searching any word other than the first in
							// the collection.
							resultCache.Add(wordEntry, eticWords[i], result[0], result[1], i > 0);
						}

						matchFound = engine.SearchWord(out result);
					}
				}
			}

			if (incProgressBar)
				IncProgressBar(-1);

			return resultCache;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the specified query's pattern for search class specifications and replaces
		/// them with the pattern the classes represent.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool ConvertClassesToPatterns(SearchQuery query,
			out SearchQuery modifiedQuery, bool showMsgOnErr)
		{
			string msg;
			return ConvertClassesToPatterns(query, out modifiedQuery, showMsgOnErr, out msg);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the specified query's pattern for search class specifications and replaces
		/// them with the pattern the classes represent.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool ConvertClassesToPatterns(SearchQuery query,
			out SearchQuery modifiedQuery, bool showMsgOnErr, out string errorMsg)
		{
			Debug.Assert(query != null);
			Debug.Assert(query.Pattern != null);
			errorMsg = null;

			modifiedQuery = query.Clone();

			// Get the index of the first opening bracket and check if we need go further.
			int i = query.Pattern.IndexOf(kOpenClassBracket);
			if (Project == null || Project.SearchClasses.Count == 0 || i < 0)
				return true;

			while (i >= 0)
			{
				// Save the offset of the open bracket and find
				// its corresponding closing bracket.
				int start = i;
				i = modifiedQuery.Pattern.IndexOf(kCloseClassBracket, i);

				if (i > start)
				{
					// Extract the class name from the query's pattern and
					// find the SearchClass object having that class name.
					string className = modifiedQuery.Pattern.Substring(start, i - start + 1);
					SearchClass srchClass = Project.SearchClasses.GetSearchClass(className, true);
					if (srchClass != null)
						modifiedQuery.Pattern = modifiedQuery.Pattern.Replace(className, srchClass.Pattern);
					else
					{
						errorMsg = Properties.Resources.kstidMissingClassMsg;
						errorMsg = string.Format(errorMsg, className);
						modifiedQuery.ErrorMessages.Add(errorMsg);
						query.ErrorMessages.Add(errorMsg);

						if (showMsgOnErr)
						{
							Utils.MsgBox(errorMsg, MessageBoxButtons.OK,
								   MessageBoxIcon.Exclamation);
						}

						return false;
					}
				}

				// Get the next open class bracket.
				i = modifiedQuery.Pattern.IndexOf(kOpenClassBracket);
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static bool VerifyMiscPatternConditions(SearchEngine engine, bool showErrMsg)
		{
			if (engine == null)
				return false;

			string msg = null;

			if (engine.GetWordBoundaryCondition() != SearchEngine.WordBoundaryCondition.NoCondition)
				msg = SearchQueryException.WordBoundaryError;
			else if (engine.GetZeroOrMoreCondition() != SearchEngine.ZeroOrMoreCondition.NoCondition)
				msg = SearchQueryException.ZeroOrMoreError;
			else if (engine.GetOneOrMoreCondition() != SearchEngine.OneOrMoreCondition.NoCondition)
				msg = SearchQueryException.OneOrMoreError;

			if (msg == null)
				msg = engine.GetCombinedErrorMessages();

			if (msg != null && showErrMsg)
				Utils.MsgBox(msg);

			return (msg == null);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void DrawWatermarkImage(string imageId, Graphics g, Rectangle clientRectangle)
		{
			Image watermark = Properties.Resources.ResourceManager.GetObject(imageId) as Image;
			if (watermark == null)
				return;

			Rectangle rc = new Rectangle();
			rc.Size = watermark.Size;
			rc.X = clientRectangle.Right - rc.Width - 10;
			rc.Y = clientRectangle.Bottom - rc.Height - 10;
			g.DrawImage(watermark, rc);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string HelpFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(s_helpFilePath))
				{
					s_helpFilePath = AssemblyPath;
					//s_helpFilePath = Path.Combine(s_helpFilePath, kHelpSubFolder);
					s_helpFilePath = Path.Combine(s_helpFilePath, kHelpFileName);
				}

				return s_helpFilePath;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void ShowHelpTopic(Control ctrl)
		{
			ShowHelpTopic("hid" + ctrl.Name);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void ShowHelpTopic(string hid)
		{
			if (File.Exists(HelpFilePath))
				Help.ShowHelp(new Label(), HelpFilePath, HelpTopicPaths.ResourceManager.GetString(hid));
			else
			{
				string msg = string.Format(Properties.Resources.kstidHelpFileMissingMsg,
					Utils.PrepFilePathForMsgBox(s_helpFilePath));

				Utils.MsgBox(msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Builds a manner of articulation sort key for the specified phone.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string GetMOAKey(string phone)
		{
			// TODO: When chow characters are supported, figure out how to deal with them.

			if (string.IsNullOrEmpty(phone))
				return null;

			StringBuilder keybldr = new StringBuilder(6);
			foreach (char c in phone)
			{
				IPASymbol info = IPASymbolCache[c];
				keybldr.Append(info == null ? "000" :
					string.Format("{0:X3}", info.MOArticulation));
			}

			return keybldr.ToString();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Builds a place of articulation sort key for the specified phone.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string GetPOAKey(string phone)
		{
			// TODO: When chou characters are supported, figure out how to deal with them.

			if (string.IsNullOrEmpty(phone))
				return null;

			StringBuilder keybldr = new StringBuilder(6);
			foreach (char c in phone)
			{
				IPASymbol info = IPASymbolCache[c];
				keybldr.Append(info == null ? "000" :
					string.Format("{0:X3}", info.POArticulation));
			}

			return keybldr.ToString();
		}

		#region Method to migrate previous versions of a file to current.
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool MigrateToLatestVersion(string filename, Assembly assembly,
			string transformNamespace, string errMsg)
		{
			var oldFile = Path.ChangeExtension(filename, "old");

			using (var stream = assembly.GetManifestResourceStream(transformNamespace))
			{
				var updatedFile = XmlHelper.TransformFile(filename, stream);
				if (updatedFile == null)
					return false;

				try
				{
					if (File.Exists(oldFile))
						File.Delete(oldFile);

					File.Move(filename, oldFile);
					File.Move(updatedFile, filename);
					return true;
				}
				catch (Exception e1)
				{
					try
					{
						errMsg = string.Format(errMsg, e1.Message, oldFile);
					}
					catch { }

					Utils.MsgBox(errMsg);

					try
					{
						if (!File.Exists(oldFile))
							File.Move(filename, oldFile);
					}
					catch (Exception e2)
					{
						Utils.MsgBox(e2.Message);
					}
				}
			}

			return false;
		}

		#endregion
	}
}
