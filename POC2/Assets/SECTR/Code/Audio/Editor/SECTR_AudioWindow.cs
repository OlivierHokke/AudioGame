// Copyright (c) 2014 Make Code Now! LLC
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
#define UNITY_4
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SECTR_AudioWindow : SECTR_Window 
{
	#region Private Details
	private class TreeItem
	{
		#region Private Details
		private string path;
		private SECTR_AudioBus bus;
		private SECTR_AudioCue cue;
		private AudioClip clip;
		private AudioImporter importer;
		#endregion

		#region Public Interface
		public Rect ScrollRect;
		public Rect WindowRect;
		public bool Expanded = true;
		public bool Rename = false;
		public string Name;

		public TreeItem(SECTR_AudioWindow window, SECTR_AudioCue cue, string path)
		{
			this.cue = cue;
			this.path = path;
			this.Name = cue.name;
			window.hierarchyItems.Add(AsObject, this);
		}

		public TreeItem(SECTR_AudioWindow window, SECTR_AudioBus bus, string path)
		{
			this.bus = bus;
			this.path = path;
			this.Name = bus.name;
			this.Expanded = EditorPrefs.GetBool(expandedPrefPrefix + this.path, true);
			window.hierarchyItems.Add(AsObject, this);
		}

		public TreeItem(AudioImporter importer, string path, string name)
		{
			this.importer = importer;
			this.path = path;
			this.Name = name;
		}

		public string Path 				{ get { return path; } }
		public SECTR_AudioBus Bus 		{ get { return bus; } }
		public SECTR_AudioCue Cue 		{ get { return cue; } }
		public AudioImporter Importer 	{ get { return importer; } }
		public AudioClip Clip 			
		{
			get 
			{
				if(importer && !clip)
				{
					clip = (AudioClip)AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip));
				}
				return clip;
			}
		}

		public string DefaultName
		{
			get
			{
				if(bus) return bus.name;
				if(cue) return cue.name;
				if(importer) return Clip.name;
				return "";
			}
		}

		public Object AsObject
		{
			get
			{
				if(bus) return bus;
				if(cue) return cue;
				if(importer) return importer;
				return null;
			}
		}
		#endregion
	}

	private class AudioClipFolder
	{
		public List<TreeItem> items = new List<TreeItem>(32);
		public bool expanded = true;
	}

	private class Splitter
	{
		public int thickness = 5;
		public int pos = 0;
		public bool vertical = true;
		public Rect rect;
		public bool dragging = false;

		public Splitter(int pos, bool vertical)
		{
			this.pos = pos;
			this.vertical = vertical;
		}

		public void Draw(SECTR_AudioWindow window)
		{
			if(vertical)
			{
				rect = new Rect(pos, 0f, thickness, window.showClipList ? window.bottomSplitter.pos : Screen.height);
			}
			else
			{
				rect = new Rect(0f, pos, Screen.width, thickness);
			}

			GUI.Box(rect, GUIContent.none);
		}

		public bool HandleEvents(SECTR_AudioWindow parent)
		{
			bool handledEvent = false;
			switch(Event.current.type)
			{
			case EventType.MouseDown:
				if(rect.Contains(Event.current.mousePosition)) 
				{
					dragging = true;
					handledEvent = true;
				}
				break;
			case EventType.MouseDrag:
				if(dragging)
				{
					pos += vertical ? (int)Event.current.delta.x :  (int)Event.current.delta.y;
					handledEvent = true;
				}
				break;
			case EventType.MouseUp:
				dragging = false;
				break;
			}

			if(dragging || rect.Contains(Event.current.mousePosition))
			{
				EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition.x - 16, Event.current.mousePosition.y - 16, 32, 32), 
				                               vertical ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
			}

			return handledEvent;
		}
	}

	private Vector2 treeScrollPos;
	private Vector2 propertyScrollPos;
	private Vector2 clipScrollPos;
	private Vector2 busScrollPos;

	private string treeSearchString = "";
	private string clipSearchString = "";
	private string busSearchString = "";

	private Splitter leftSplitter = new Splitter(-1, true);
	private Splitter bottomSplitter = new Splitter(-1, false);

	private int lastWidth = 0;
	private int lastHeight = 0;
	private int indent = 0;

	private TreeItem selectedTreeItem = null;
	private List<TreeItem> selectedTreeItems = new List<TreeItem>();
	private List<TreeItem> displayedTreeItems = new List<TreeItem>();
	private TreeItem selectedClipItem = null;
	private List<TreeItem> selectedClipItems = new List<TreeItem>();
	private List<TreeItem> displayedClipItems = new List<TreeItem>();
	private TreeItem dragHoverItem = null;
	private TreeItem dragTreeItem = null;
	private TreeItem dragClipItem = null;
	private bool lastSelectedTree = true;
	private Rect clipScrollRect;
	private SECTR_Editor propertyEditor = null;

	// 
	private Dictionary<string, AudioClipFolder> clipItems = new Dictionary<string, AudioClipFolder>(256);
	private Dictionary<Object, TreeItem> hierarchyItems = new Dictionary<Object, TreeItem>(256);
	private List<TreeItem> deadItems = new List<TreeItem>();
	private List<SECTR_AudioBus> newBuses = new List<SECTR_AudioBus>();
	private List<SECTR_AudioCue> newCues = new List<SECTR_AudioCue>();

	// editor state vars
	private bool changingBitrate = false;
	private bool confirmBitrate = false;
	private bool showFullClipDetails = false;
	private bool showProperties = true;
	private bool showHierarchy = true;
	private bool showClipList = true;
	private string audioRootPath = null;

	// Styles and Icons
	private GUIStyle busSliderStyle = null;
	private GUIStyle busFieldStyle = null;
	private Texture2D playIcon;
	private Texture2D cueIcon;
	private Texture2D busIcon;
	private Texture2D expandedIcon;
	private Texture2D collapsedIcon;
	private Texture2D folderIcon;
	private Texture2D soloOnIcon;
	private Texture2D soloOffIcon;
	private Texture2D muteOnIcon;
	private Texture2D muteOffIcon;

	// baking
	SECTR_ComputeRMS bakeMaster = null;
	bool wasBaking = false;

	private const string rootPrefPrefix = "SECTR_Audio_Root_";
	private const string expandedPrefPrefix = "SECTR_Audio_Expanded_";
	private const string showPrefPrefix = "SECTR_Audio_Show_";
	private const string fullDetailsPref = "SECTR_Audio_FullClip";
	#endregion

	#region Public Interface
	public static Texture2D LoadIcon(string iconName)
	{
		// Look for each icon first in the default path, only do a full search if we don't find it there.
		// Full search would be required if someone imports the library into a non-standard place.
		Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/SECTR/Code/Audio/Editor/Icons/" + iconName, typeof(Texture2D));
		if(!icon)
		{
			icon = SECTR_Asset.Find<Texture2D>(iconName);
		}
		return icon;
	}
	#endregion

	#region Unity Interface
	void OnEnable()
	{
		playIcon = LoadIcon("PlayIcon.psd");
		cueIcon = LoadIcon("CueIcon.psd");
		busIcon = LoadIcon("BusIcon.psd");
		expandedIcon = LoadIcon("ExpandedIcon.psd");
		collapsedIcon = LoadIcon("CollapsedIcon.psd");
		folderIcon = LoadIcon("FolderIcon.psd");
		soloOnIcon = LoadIcon("SoloOnIcon.psd");
		soloOffIcon = LoadIcon("SoloOffIcon.psd");
		muteOnIcon = LoadIcon("MuteOnIcon.psd");
		muteOffIcon = LoadIcon("MuteOffIcon.psd");

		showFullClipDetails = UnityEditor.EditorPrefs.GetBool(fullDetailsPref, false);
		showHierarchy = UnityEditor.EditorPrefs.GetBool(showPrefPrefix + "Hierarchy", true);
		showProperties = UnityEditor.EditorPrefs.GetBool(showPrefPrefix + "Properties", true);
		showClipList = UnityEditor.EditorPrefs.GetBool(showPrefPrefix + "ClipList", true);

		audioRootPath = UnityEditor.EditorPrefs.GetString(rootPrefPrefix + SECTR_Asset.GetProjectName(), "");
		if(string.IsNullOrEmpty(audioRootPath))
		{
			if(EditorUtility.DisplayDialog("Welcome to SECTR AUDIO!", 
			                               "Do you store all of your audio files beneath a single folder? If so, please tell us which one. If not, don't worry, we can search your whole project quickly. You can always set a new root audio folder in the Audio Window right click menu.",
			                               "Yes",
			                               "No"))
			{
				_SelectAudioRoot();
			}

			if(string.IsNullOrEmpty(audioRootPath))			
			{
				audioRootPath = Application.dataPath;
				UnityEditor.EditorPrefs.SetString(rootPrefPrefix + SECTR_Asset.GetProjectName(), audioRootPath);
			}
		}


		if(hierarchyItems.Count == 0)
		{
			_RefreshAssets();
		}
	}

	void OnDestroy()
	{
		hierarchyItems.Clear();
		clipItems.Clear();
		SECTR_AudioSystem.Solo(null);
	}

	void Update()
	{
		int numDeadItems = deadItems.Count;
		for(int itemIndex = 0; itemIndex < numDeadItems; ++itemIndex)
		{
			hierarchyItems.Remove(deadItems[itemIndex].AsObject);
		}
		
		int numNewBuses = newBuses.Count;
		for(int busIndex = 0; busIndex < numNewBuses; ++busIndex)
		{
			SECTR_AudioBus bus = newBuses[busIndex];
			hierarchyItems[bus] = new TreeItem(this, bus, AssetDatabase.GetAssetPath(bus)); 
		}
		
		int numNewCues = newCues.Count;
		for(int cueIndex = 0; cueIndex < numNewCues; ++cueIndex)
		{
			SECTR_AudioCue cue = newCues[cueIndex];
			hierarchyItems[cue] = new TreeItem(this, cue, AssetDatabase.GetAssetPath(cue)); 
		}
		
		if(numDeadItems > 0 || numNewBuses > 0 || numNewCues > 0)
		{
			Repaint();
		}

		deadItems.Clear();
		newBuses.Clear();
		newCues.Clear();

		if(bakeMaster)
		{
			Repaint();
		}
	}

	protected override void OnGUI()
	{
		base.OnGUI();

		if(busSliderStyle == null)
		{
			busSliderStyle = new GUIStyle(GUI.skin.verticalSlider);
			busSliderStyle.alignment = TextAnchor.MiddleCenter;
		}

		if(busFieldStyle == null)
		{
			busFieldStyle = new GUIStyle(EditorStyles.textField);
			busFieldStyle.alignment = TextAnchor.MiddleCenter;
		}

		if(leftSplitter.pos == -1)
		{
			leftSplitter.pos = (int)(position.width * 0.3f);
		}
		if(bottomSplitter.pos == -1)
		{
			bottomSplitter.pos = (int)(position.height * 0.6f);
		}

		if(lastWidth == 0 && lastHeight == 0)
		{
			lastWidth = (int)position.width;
			lastHeight = (int)position.height;
		}
		else if(position.width != lastWidth || position.height != lastHeight)
		{
			float leftFrac = leftSplitter.pos / (float)lastWidth;
			float bottomFrac = bottomSplitter.pos / (float)lastHeight;
			leftSplitter.pos = (int)(position.width * leftFrac);
			bottomSplitter.pos = (int)(position.height * bottomFrac);
			lastWidth = (int)position.width;
			lastHeight = (int)position.height;
		}

		displayedTreeItems.Clear();
		displayedClipItems.Clear();

		if(showHierarchy)
		{
			_DrawHierarchy();
			if(showProperties)
			{
				leftSplitter.Draw(this);
			}
		}
		if(showProperties)
		{
			_DrawProperties();
		}
		if(showClipList)
		{
			if(showProperties || showHierarchy)
			{
				bottomSplitter.Draw(this);
			}
			_DrawClipList();
		}

		if(bakeMaster)
		{
			float progress = bakeMaster.Progress;
			EditorUtility.DisplayProgressBar("Baking HDR Audio Data", "Please don't leave the Editor window during HDR baking.", progress);
			wasBaking = true;
		}
		else if(wasBaking)
		{
			EditorUtility.ClearProgressBar();
			wasBaking = false;
		}

		_HandleEvents();
	}
	#endregion

	#region Private Methods
	private void _RefreshAssets()
	{
		clipItems.Clear();
		hierarchyItems.Clear();

		List<string> assetExtensions = new List<string>()	{ ".asset", };
		List<string> clipExtensions = new List<string>()	{ ".wav", ".aif", ".aiff", ".ogg", ".mp3", ".xm", ".mod", ".it", ".xm", };
		List<string> paths = new List<string>(128);

		// Add all Buses
		List<SECTR_AudioBus> buses = SECTR_Asset.GetAll<SECTR_AudioBus>(audioRootPath, assetExtensions, ref paths, false);
		for(int busIndex = 0; busIndex < buses.Count; ++busIndex)
		{
			SECTR_AudioBus bus = buses[busIndex];
			if(bus != null)
			{
				new TreeItem(this, bus, paths[busIndex]);
			}
		}

		// Add all Cues
		List<SECTR_AudioCue> cues = SECTR_Asset.GetAll<SECTR_AudioCue>(audioRootPath, assetExtensions, ref paths, false);
		for(int cueIndex = 0; cueIndex < cues.Count; ++cueIndex)
		{
			SECTR_AudioCue cue = cues[cueIndex];
			if(cue != null)
			{
				new TreeItem(this, cue, paths[cueIndex]);
			}
		}

		// Build the list of AudioClips
		SECTR_Asset.GetAll<AudioClip>(audioRootPath, clipExtensions, ref paths, true);
		for(int pathIndex = 0; pathIndex < paths.Count; ++pathIndex)
		{
			string path = paths[pathIndex];
			if(!string.IsNullOrEmpty(path))
			{
				string dirPath = "";
				string fileName = "";
				SECTR_Asset.SplitPath(path, out dirPath, out fileName);

				TreeItem item = new TreeItem((AudioImporter)AssetImporter.GetAtPath(path), path, fileName);

				AudioClipFolder folder;
				if(!clipItems.TryGetValue(dirPath, out folder))
				{
					folder = new AudioClipFolder();
					bool userExpanded = EditorPrefs.GetBool(expandedPrefPrefix + dirPath, false);
					folder.expanded = userExpanded;
					clipItems.Add(dirPath, folder);
				}
				folder.items.Add(item);
			}
		}
	}
	
	private void _DrawHierarchy()
	{
		GUILayout.BeginArea(new Rect(0f,
		                             0f,
		                             showProperties ? leftSplitter.pos : Screen.width,
		                             showClipList ? bottomSplitter.pos : Screen.height));

		Rect headerRect = DrawHeader("HIERARCHY", ref treeSearchString, leftSplitter.pos / 2, false);

		treeScrollPos = EditorGUILayout.BeginScrollView(treeScrollPos);

		// Back up initial indent b/c DrawTreeItem will start by indenting.
		string searchString = treeSearchString.ToLower();
		indent = 0;

		foreach(TreeItem item in hierarchyItems.Values)
		{
			if(item.Cue != null || item.Bus != null)
			{
				if((item.Cue && item.Cue.Bus == null) || (item.Bus && item.Bus.Parent == null))
				{
					_DrawTreeItem(item, headerRect.height, searchString);
				}
			}
			else
			{
				deadItems.Add(item);
			}
		}

		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void _DrawTreeItem(TreeItem item, float initialOffset, string searchString)
	{
		if(item != null && (item.Bus || item.Cue) && _ChildVisible(item, searchString))
		{
			item.ScrollRect = EditorGUILayout.BeginHorizontal();
			bool selected = selectedTreeItems.Contains(item);
			if(selected)
			{
				Rect selectionRect = item.ScrollRect;
				selectionRect.y += 3;
				GUI.Box(selectionRect, "", selectionBoxStyle);
			}
			item.WindowRect = item.ScrollRect;
			item.WindowRect.y += initialOffset;
			item.WindowRect.y -= treeScrollPos.y;
			GUILayout.Label(GUIContent.none, GUILayout.Width(indent), GUILayout.MaxWidth(indent), GUILayout.MinWidth(indent), GUILayout.ExpandWidth(false));

			elementStyle.alignment = TextAnchor.MiddleLeft;
			if(item == dragHoverItem || selected)
			{
				elementStyle.normal.textColor = Color.white;
			}
			else
			{
				if(item.Cue && item.Cue.IsTemplate)
				{
					elementStyle.normal.textColor = Color.blue;
				}
				else if(item.Cue && item.Cue.Template != null)
				{
					elementStyle.normal.textColor = Color.yellow;
				}
				else
				{
					elementStyle.normal.textColor = UnselectedItemColor;
				}
			}

			int iconSize = lineHeight;
			if(item.Bus && (item.Bus.Children.Count > 0 || item.Bus.Cues.Count > 0))
			{
				if(GUILayout.Button(item.Expanded ? expandedIcon : collapsedIcon, elementStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
				{
					item.Expanded = !item.Expanded;
					EditorPrefs.SetBool(expandedPrefPrefix + item.Path, item.Expanded);
				}
			}
			else
			{
				GUILayout.Button(GUIContent.none, elementStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
			}

			bool wasEnabled = GUI.enabled;
			GUI.enabled &= SECTR_VC.IsEditable(item.Path);

			Texture typeIcon = null;
			if(item.Bus != null) typeIcon = busIcon;
			if(item.Cue != null) typeIcon = cueIcon;
			if(item.Importer != null) typeIcon = playIcon;
			if(typeIcon)
			{
				GUILayout.Label(typeIcon, elementStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
			}
			if(item.Rename)
			{
				string focusName = "RenamingItem";
				GUI.SetNextControlName(focusName);
				item.Name = GUILayout.TextField(item.Name);
				GUI.FocusControl(focusName);
			}
			else
			{
				GUILayout.Label(item.Name, elementStyle);
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = wasEnabled;

			displayedTreeItems.Add(item);

			if(item.Expanded && item.Bus)
			{
				indent += iconSize / 2;
				foreach(SECTR_AudioBus bus in item.Bus.Children)
				{
					if(bus != null)
					{
						if(hierarchyItems.ContainsKey(bus))
						{
							_DrawTreeItem(hierarchyItems[bus], initialOffset, searchString);
						}
						else if(!newBuses.Contains(bus))
						{
							newBuses.Add(bus);
						}
					}
				}
				foreach(SECTR_AudioCue cue in item.Bus.Cues)
				{
					if(cue != null)
					{
						if(hierarchyItems.ContainsKey(cue))
						{
							_DrawTreeItem(hierarchyItems[cue], initialOffset, searchString);
						}
						else if(!newCues.Contains(cue))
						{
							newCues.Add(cue);
						}
					}
				}
				indent -= iconSize / 2;
			}
		}
	}

	private bool _ChildVisible(TreeItem item, string searchString)
	{
		if(item == null)
		{
			return false;
		}
		else if(string.IsNullOrEmpty(treeSearchString) || item.Name.ToLower().Contains(searchString))
		{
			return true;
		}
		else if(item.Expanded && item.Bus)
		{
			foreach(SECTR_AudioBus bus in item.Bus.Children)
			{
				if(_ChildVisible(hierarchyItems[bus], searchString))
				{
					return true;
				}
			}
			foreach(SECTR_AudioCue cue in item.Bus.Cues)
			{
				if(_ChildVisible(hierarchyItems[cue], searchString))
				{
					return true;
				}
			}
		}

		return false;
	}

	private void _DrawProperties()
	{
		int width = showHierarchy ? Screen.width - leftSplitter.pos - leftSplitter.thickness : Screen.width;
		GUILayout.BeginArea(new Rect(showHierarchy ? leftSplitter.pos + leftSplitter.thickness : 0f,
		                             0f,
		                             width,
		                             showClipList ? bottomSplitter.pos : Screen.height));

		bool allCues = selectedTreeItems.Count > 0;
		bool allBuses = selectedTreeItems.Count > 0;
		foreach(TreeItem item in selectedTreeItems)
		{
			allCues &= item.Cue != null;
			allBuses &= item.Bus != null;
		}

		if(allBuses)
		{
			DrawHeader("BUSES", ref busSearchString, (Screen.width - leftSplitter.pos) / 4, true);
			EditorGUILayout.Space();
			busScrollPos = EditorGUILayout.BeginScrollView(busScrollPos);
			EditorGUILayout.BeginHorizontal();
			string searchString = busSearchString.ToLower();

			List<TreeItem> drawnBuses = new List<TreeItem>(selectedTreeItems.Count);
			foreach(TreeItem item in hierarchyItems.Values)
			{
				if(selectedTreeItems.Contains(item) && !drawnBuses.Contains(item))
				{
					_DrawBus(item, searchString, drawnBuses);
				}
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndScrollView();
		}
		else if(allCues)
		{
			string nullSearch = null;
			if(selectedTreeItems.Count > 1)
			{
				DrawHeader("PROPERTIES (" + selectedTreeItems.Count + " SELECTED)", ref nullSearch, 0, true);
			}
			else
			{
				DrawHeader("PROPERTIES (" + selectedTreeItem.Cue.name + ")", ref nullSearch, 0, true);
			}

			propertyScrollPos = EditorGUILayout.BeginScrollView(propertyScrollPos);
			bool wasEnabled = GUI.enabled;
			GUI.enabled &= SECTR_VC.IsEditable(selectedTreeItem.Path);
			if(selectedTreeItem.Cue && (propertyEditor == null || propertyEditor.target != selectedTreeItem.Cue))
			{
				int numSelected = selectedTreeItems.Count;
				SECTR_AudioCueEditor cueEditor = null;
				if(numSelected > 1)
				{
					SECTR_AudioCue[] cues = new SECTR_AudioCue[selectedTreeItems.Count];
					for(int selectedIndex = 0; selectedIndex < numSelected; ++selectedIndex)
					{
						cues[selectedIndex] = selectedTreeItems[selectedIndex].Cue;
					}
					cueEditor = (SECTR_AudioCueEditor)Editor.CreateEditor(cues);
				}
				else
				{
					cueEditor = (SECTR_AudioCueEditor)Editor.CreateEditor(selectedTreeItem.Cue);
				}
				cueEditor.DrawBus = false;
				propertyEditor = cueEditor;
			}
			else if(selectedTreeItem.Bus && (propertyEditor == null || propertyEditor.target != selectedTreeItem.Bus))
			{
				propertyEditor = (SECTR_Editor)Editor.CreateEditor(selectedTreeItem.Bus);
			}
			if(propertyEditor)
			{
				propertyEditor.WidthOverride = width;
				propertyEditor.OnInspectorGUI();
			}
			GUI.enabled = wasEnabled;
			EditorGUILayout.EndScrollView();
		}
		else if(selectedTreeItems.Count > 0)
		{
			string nullSearch = null;
			DrawHeader("MIXED SELECTION", ref nullSearch, 0, true);
			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;
			GUILayout.Button("Cannot display Buses and Cues simultaneously.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUI.enabled = wasEnabled;
		}
		else
		{
			string nullSearch = null;
			DrawHeader("NO SELECTION", ref nullSearch, 0, true);
			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;
			GUILayout.Button("Nothing Selected in Hierarchy View.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUI.enabled = wasEnabled;
		}
		GUILayout.EndArea();
	}
	
	private void _DrawBus(TreeItem item, string searchString, List<TreeItem> drawnBuses)
	{
		if(item != null && item.Bus)
		{
			string name = item.Bus.name;
			SECTR_AudioBus parent = item.Bus.Parent;
			while(parent != null)
			{
				name = parent.name + "/" + name;
				parent = parent.Parent;
			}

			if(string.IsNullOrEmpty(busSearchString) || name.ToLower().Contains(searchString))
			{
				bool wasEnabled = GUI.enabled;
				GUI.enabled &= SECTR_VC.IsEditable(item.Path);

				SECTR_AudioBusEditor.DrawBusControls(name, 100, item.Bus, muteOnIcon, muteOffIcon, soloOnIcon, soloOffIcon, elementStyle, busSliderStyle, busFieldStyle);
				GUILayout.Box(GUIContent.none, GUILayout.Width(5), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));

				GUI.enabled = wasEnabled;
				drawnBuses.Add(item);
			}

			foreach(SECTR_AudioBus bus in item.Bus.Children)
			{
				if(bus != null)
				{
					_DrawBus(hierarchyItems[bus], searchString, drawnBuses);
				}
			}
		}
	}
	
	private void _DrawClipList()
	{
		int minWidth = showFullClipDetails ? 750 : 600;
		int screenWidth = Mathf.Max(Screen.width, minWidth);
		int iconSize = lineHeight;

		GUILayout.BeginArea(new Rect(0f,
		                             (showHierarchy || showProperties) ? bottomSplitter.pos + bottomSplitter.thickness : 0f, 
		                             Screen.width, 
		                             (showHierarchy || showProperties) ? Screen.height - bottomSplitter.pos - bottomSplitter.thickness - lineHeight * 2 : Screen.height));

		// Header
		Rect headerRect = DrawHeader("AUDIO CLIPS", ref clipSearchString, Screen.width / 4, true);

		// Column labels
		Rect columnRect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(screenWidth), GUILayout.Height(headerHeight));
		headerStyle.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label(GUIContent.none, GUILayout.Width(iconSize * 2), GUILayout.MaxWidth(iconSize * 2), GUILayout.MinWidth(iconSize * 2), GUILayout.ExpandWidth(false), GUILayout.Height(headerHeight));
		string[] categories = {
#if UNITY_4
			"NAME",
			"3D",
			"LENGTH",
			"SIZE",
			"CHANNELS",
			"MONO",
			"HARDWARE",
			"STREAM",
			"LOOP",
			"COMPRESSED",
			"BITRATE",

#else
			"NAME",
			"LENGTH",
			"SIZE",
			"CHANNELS",
			"MONO",
			"STREAM",
			"COMPRESSED",
			// TODO: Implement remaining Unity 5 options.
			//"OPT RATE",
			//"SAMPLERATE",
#endif
		};
		float[] widthScales = {
#if UNITY_4
			2.5f,
			0.5f,
			0.75f,
			0.75f,
			1.0f,
			0.6f,
			1.0f,
			0.75f,
			0.6f,
			1.2f,
			0.9f,
#else
			2.5f,
			0.75f,
			0.75f,
			1.0f,
			0.6f,
			0.75f,
			1.2f,
			// TODO: Implement remaining Unity 5 options.
			//0.6f,
			//0.9f,
#endif
		};
		int[] widths = new int[categories.Length];
#if UNITY_4
		int advancedStart = 5;
#else
		int advancedStart = 4;
#endif

		int baseColumnWidth;
		if(showFullClipDetails)
		{
			baseColumnWidth = screenWidth / categories.Length;
		}
		else
		{
			baseColumnWidth = screenWidth / advancedStart;
		}

		int columnSum = 0;
		for(int catIndex = 0; catIndex < (showFullClipDetails ? categories.Length : advancedStart); ++catIndex)
		{
			int width = showFullClipDetails ? (int)(widthScales[catIndex] * baseColumnWidth) : baseColumnWidth;
			GUI.Label(new Rect(columnRect.x + columnSum, columnRect.y, width, columnRect.height), categories[catIndex], headerStyle);
			columnSum += width;
			widths[catIndex] = width;
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		// Audio Clips
		clipScrollPos = EditorGUILayout.BeginScrollView(clipScrollPos);

		string searchString = clipSearchString.ToLower();
		foreach(string folderName in clipItems.Keys)
		{
			AudioClipFolder folder = clipItems[folderName];

			Rect folderRect = EditorGUILayout.BeginHorizontal();
			elementStyle.normal.textColor = UnselectedItemColor;
			elementStyle.alignment = TextAnchor.MiddleLeft;
			if(GUILayout.Button(folder.expanded ? expandedIcon : collapsedIcon, elementStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
			{
				folder.expanded = !folder.expanded;
				EditorPrefs.SetBool(expandedPrefPrefix + folderName, folder.expanded);
			}
			GUILayout.Label(folderIcon, elementStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
			GUILayout.Label(folderName, elementStyle);
			EditorGUILayout.EndHorizontal();

			if(folder.expanded)
			{
				deadItems.Clear();
				foreach(TreeItem item in folder.items)
				{
					if(item.Importer == null || string.IsNullOrEmpty(item.Path))
					{
						deadItems.Add(item);
					}
					else if((string.IsNullOrEmpty(clipSearchString) || item.Name.ToLower().Contains(searchString)))
					{
						AudioImporter importer = item.Importer;
						if(importer == null)
						{
							deadItems.Add(item);
							continue;
						}

						item.ScrollRect = EditorGUILayout.BeginHorizontal();
						item.WindowRect = item.ScrollRect;
						item.WindowRect.y += bottomSplitter.pos;
						item.WindowRect.y += headerRect.height;
						item.WindowRect.y += columnRect.height;
						item.WindowRect.y += folderRect.height;
						item.WindowRect.y -= clipScrollPos.y;

						bool selected = selectedClipItems.Contains(item);

						if(selected && selectionBoxStyle != null)
						{
							Rect selectionRect = item.ScrollRect;
							selectionRect.y += 1;
							selectionRect.height += 1;
							GUI.Box(selectionRect, "", selectionBoxStyle);
						}

						elementStyle.normal.textColor = selected ? Color.white : UnselectedItemColor;

						// Indent for folder
						GUILayout.Label(GUIContent.none, GUILayout.Width(iconSize), GUILayout.MaxWidth(iconSize), GUILayout.MinWidth(iconSize), GUILayout.ExpandWidth(false));
						// Audition button

						if(GUILayout.Button(new GUIContent(playIcon, "Plays a preview of this clip."), iconButtonStyle, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
						{
							SECTR_AudioSystem.Audition(item.Clip);
							selectedClipItem = item;
						}

						// Now for all the info
						bool wasEnabled = GUI.enabled;
						GUI.enabled &= SECTR_VC.IsEditable(item.Path);

						int checkSize = 20;
						int columnIndex = 0;
						int columnWidth = 0;
						float rowY = item.ScrollRect.y + 1;
						columnSum = (int)item.ScrollRect.x;

						// Editable properties
#if UNITY_4
						bool new3D = importer.threeD;
						bool newHardware = importer.hardware;
						bool newLoop = importer.loopable;
						bool newStream = importer.loadType == AudioImporterLoadType.StreamFromDisc;
						bool compressed = importer.format == AudioImporterFormat.Compressed;
						int bitrate = importer.compressionBitrate;
#else
						bool newStream = importer.loadType == AudioClipLoadType.Streaming;
						bool compressed = importer.format == AudioClipFormat.Compressed;
						//bool optimizeRate = importer.optimizeSampleRate;
						//float sampleRate = importer.sampleRate;
#endif
						bool newMono = importer.forceToMono;

						// Name
						columnWidth = widths[columnIndex++];
						elementStyle.alignment = TextAnchor.MiddleLeft;
						float shift = iconSize * 2.5f;
						GUI.Label(new Rect(columnSum + shift, rowY, columnWidth - shift, item.ScrollRect.height), item.Name, elementStyle);
						elementStyle.alignment = TextAnchor.UpperCenter;
						columnSum += columnWidth;

#if UNITY_4
						// 3D
						columnWidth = widths[columnIndex++];
						new3D = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), new3D);
						columnSum += columnWidth;
#endif
						
						// Length
						columnWidth = widths[columnIndex++];
						float length = item.Clip.length;
						string label = "s";
						if(length > 60f)
						{
							length /= 60f;
							label = "m";
						}
						EditorGUI.LabelField(new Rect(columnSum, rowY, columnWidth, item.ScrollRect.height), length.ToString("N2") + " " + label, elementStyle);
						columnSum += columnWidth;

						// Size
						int sizeKB = 0;
#if UNITY_4
						if(importer.format == AudioImporterFormat.Compressed)
#else
						if(importer.format == AudioClipFormat.Compressed)
#endif
						{
#if UNITY_4
							int effectiveBitrate = bitrate > 0 ? bitrate : 156000;
							sizeKB = (int)(item.Clip.length * effectiveBitrate / 8);
#endif
							// TODO: Compute size for 5.x
						}
						else
						{
							sizeKB = (int)(item.Clip.length * item.Clip.frequency * item.Clip.channels * 2); // 2 assumes 16 bits per sample.
						}
						sizeKB /= 1024;
						string size = "~";
						if(sizeKB >= 1000)
						{
							size += (sizeKB / 1024f).ToString("N2") + " MB";
						}
						else
						{
							size += sizeKB + " KB";
						}
						columnWidth = widths[columnIndex++];
						EditorGUI.LabelField(new Rect(columnSum, rowY, columnWidth, item.ScrollRect.height), size, elementStyle);
						columnSum += columnWidth;

						// Channels
						string channels = item.Clip.channels + "ch";
						channels += " @ " + (item.Clip.frequency / 1000f) + "k";
						columnWidth = widths[columnIndex++];
						EditorGUI.LabelField(new Rect(columnSum, rowY, columnWidth, item.ScrollRect.height), channels, elementStyle);
						columnSum += columnWidth;

						// Advanced Stuff
						if(showFullClipDetails)
						{
							// Force Mono
							columnWidth = widths[columnIndex++];
							newMono = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), newMono);
							columnSum += columnWidth;

#if UNITY_4
							// Hardware
							columnWidth = widths[columnIndex++];
							newHardware = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), newHardware);
							columnSum += columnWidth;
#endif

							// Stream vs In Memory
							columnWidth = widths[columnIndex++];
							newStream = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), newStream);
							columnSum += columnWidth;

#if UNITY_4
							// Loop
							columnWidth = widths[columnIndex++];
							newLoop = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), newLoop);
							columnSum += columnWidth;
#endif

							// Compressed
							columnWidth = widths[columnIndex++];
							compressed = EditorGUI.Toggle(new Rect(columnSum - checkSize / 2 + columnWidth / 2, rowY, checkSize, item.ScrollRect.height), compressed);
							columnSum += columnWidth;

#if UNITY_4
							// Bitrate
							int labelWidth = 40;
							GUI.enabled &= compressed;
							columnWidth = widths[columnIndex++];
							if(bitrate > 0 )
							{
								bitrate =  EditorGUI.IntField(new Rect(columnSum - labelWidth / 2 + columnWidth / 2, rowY, labelWidth, item.ScrollRect.height), bitrate / 1000, busFieldStyle) * 1000;

							}
							else
							{
								int userBitrate = EditorGUI.IntField(new Rect(columnSum - labelWidth / 2 + columnWidth / 2, rowY, labelWidth, item.ScrollRect.height), 156, busFieldStyle) * 1000;
								if(userBitrate != 156000)
								{
									bitrate = userBitrate;
								}
							}
							columnSum += columnWidth; 
#endif
						}

						if((newMono != importer.forceToMono) || 
#if UNITY_4
						   (new3D != importer.threeD) ||
						   (newHardware != importer.hardware) ||
						   (newLoop != importer.loopable) ||
						   (newStream != (importer.loadType == AudioImporterLoadType.StreamFromDisc)) ||
						   (compressed != (importer.format == AudioImporterFormat.Compressed)) ||
#else
						   (newStream != (importer.loadType == AudioClipLoadType.Streaming)) ||
						   (compressed != (importer.format == AudioClipFormat.Compressed)) ||
#endif
						   (confirmBitrate))
						{
							importer.forceToMono = newMono;
#if UNITY_4
							importer.threeD = new3D;
							importer.hardware = newHardware;
							importer.loopable = newLoop;
							importer.loadType = newStream ? AudioImporterLoadType.StreamFromDisc : AudioImporterLoadType.CompressedInMemory;
							importer.format = compressed ? AudioImporterFormat.Compressed : AudioImporterFormat.Native;
							importer.compressionBitrate = bitrate;
#else
							importer.loadType = newStream ? AudioClipLoadType.Streaming : AudioClipLoadType.CompressedInMemory;
							// TODO: Expose PCM vs ADPCM
							importer.format = compressed ? AudioClipFormat.Compressed : AudioClipFormat.PCM;
#endif
							confirmBitrate = false;
							EditorUtility.SetDirty(importer);
							AssetDatabase.WriteImportSettingsIfDirty(item.Path);
							AssetDatabase.Refresh();
						}

						GUI.enabled = wasEnabled;
						EditorGUILayout.EndHorizontal();

						displayedClipItems.Add(item);
					}
				}
				int numDeadItems = deadItems.Count;
				for(int itemIndex = 0; itemIndex < numDeadItems; ++itemIndex)
				{
					folder.items.Remove(deadItems[itemIndex]);
				}
			}
		}
		
		EditorGUILayout.EndScrollView();
		if(Event.current.type == EventType.Repaint)
		{
			clipScrollRect = GUILayoutUtility.GetLastRect();
		}
		GUILayout.EndArea();
	}

	private void _HandleEvents()
	{
		if(Event.current != null)
		{
			if(!string.IsNullOrEmpty(Event.current.commandName) && Event.current.commandName == "UndoRedoPerformed")
			{
				Repaint();
				return;
			}

			if((showHierarchy && showProperties && leftSplitter.HandleEvents(this)) || (showClipList && (showHierarchy || showProperties) && bottomSplitter.HandleEvents(this)))
			{
				Repaint();
				return;
			}

#if UNITY_EDITOR_OSX
			bool heldControl = (Event.current.modifiers & EventModifiers.Command) != 0;
#else
			bool heldControl = (Event.current.modifiers & EventModifiers.Control) != 0;
#endif
			bool heldShift = (Event.current.modifiers & EventModifiers.Shift) != 0;

			switch(Event.current.type)
			{
			case EventType.MouseDown:
				if(Event.current.button == 0)
				{
					if(Event.current.mousePosition.x < leftSplitter.pos && Event.current.mousePosition.y < bottomSplitter.pos)
					{
						foreach(TreeItem item in displayedTreeItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								if(Event.current.clickCount > 1)
								{
									if(SECTR_VC.IsEditable(item.Path))
									{									
										selectedTreeItem = item;
										selectedTreeItems.Clear();
										selectedTreeItems.Add(item);
										selectedTreeItem.Rename = true;
										Repaint();
									}
								}
								else
								{
									dragTreeItem = item;
								}
								break;
							}
						}
					}
					else if(Event.current.mousePosition.y > bottomSplitter.pos)
					{
						foreach(TreeItem item in displayedClipItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								dragClipItem = item;
								break;
							}
						}
					}
				}
				break;
			case EventType.MouseUp:
				dragTreeItem = null;
				dragClipItem = null;
				dragHoverItem = null;

				if(selectedTreeItem != null && selectedTreeItem.Rename && !selectedTreeItem.WindowRect.Contains(Event.current.mousePosition))
				{
					selectedTreeItem.Name = selectedTreeItem.DefaultName;
					selectedTreeItem.Rename = false;
					Repaint();
				}
				else if(Event.current.mousePosition.x < leftSplitter.pos && Event.current.mousePosition.y < bottomSplitter.pos)
				{
					lastSelectedTree = true;
					TreeItem newSelection = Event.current.button == 0 ? null : selectedTreeItem;
					foreach(TreeItem item in displayedTreeItems)
					{
						if(item.WindowRect.Contains(Event.current.mousePosition))
						{
							newSelection = item;
							if(Event.current.clickCount > 1 && SECTR_VC.IsEditable(item.Path))
							{
								newSelection.Rename = true;
								Repaint();
							}
							break;
						}
					}

					if(newSelection != selectedTreeItem || heldControl || heldShift)
					{
						if(newSelection == null)
						{
							selectedTreeItem = null;
							selectedTreeItems.Clear();
						}
						else if(heldControl)
						{
							if(selectedTreeItems.Contains(newSelection))
							{
								selectedTreeItems.Remove(newSelection);
								if(selectedTreeItems.Count > 0)
								{
									selectedTreeItem = selectedTreeItems[0];
								}
								else
								{
									selectedTreeItem = null;
								}
							}
							else
							{
								selectedTreeItems.Add(newSelection);
								selectedTreeItem = newSelection;
							}
						}
						else if(heldShift && selectedTreeItem != null)
						{
							foreach(TreeItem item in displayedTreeItems)
							{
								if((item.WindowRect.y >= selectedTreeItem.WindowRect.y && item.WindowRect.y <= newSelection.WindowRect.y) ||
								   (item.WindowRect.y <= selectedTreeItem.WindowRect.y && item.WindowRect.y >= newSelection.WindowRect.y))
								{
									if(!selectedTreeItems.Contains(item))
									{
										selectedTreeItems.Add(item);
									}
								}
								else
								{
									selectedTreeItems.Remove(item);
								}
							}
							selectedTreeItem = newSelection;
						}
						else
						{
							selectedTreeItem = newSelection;
							selectedTreeItems.Clear();
							selectedTreeItems.Add(selectedTreeItem);
						}
						propertyEditor = null;
						GUI.FocusControl(null);
						Repaint();
					}
				}
				else if(Event.current.mousePosition.y > bottomSplitter.pos)
				{
					lastSelectedTree = false;
					TreeItem newSelection = Event.current.button == 0 ? null : selectedClipItem;
					foreach(TreeItem item in displayedClipItems)
					{
						if(item.WindowRect.Contains(Event.current.mousePosition))
						{
							newSelection = item;
							break;
						}
					}

					if(newSelection != selectedClipItem || heldControl || heldShift)
					{
						if(newSelection == null)
						{
							selectedClipItem = null;
							selectedClipItems.Clear();
						}
						else if(heldControl)
						{
							if(selectedClipItems.Contains(newSelection))
							{
								selectedClipItems.Remove(newSelection);
								if(selectedClipItems.Count > 0)
								{
									selectedClipItem = selectedClipItems[0];
								}
								else
								{
									selectedClipItem = null;
								}
							}
							else
							{
								selectedClipItems.Add(newSelection);
								selectedClipItem = newSelection;
							}
						}
						else if(heldShift && selectedClipItem != null)
						{
							foreach(TreeItem item in displayedClipItems)
							{
								if((item.WindowRect.y >= selectedClipItem.WindowRect.y && item.WindowRect.y <= newSelection.WindowRect.y) ||
								   (item.WindowRect.y <= selectedClipItem.WindowRect.y && item.WindowRect.y >= newSelection.WindowRect.y))
								{
									if(!selectedClipItems.Contains(item))
									{
										selectedClipItems.Add(item);
									}
								}
								else
								{
									selectedClipItems.Remove(item);
								}
							}
							selectedClipItem = newSelection;
						}
						else
						{
							selectedClipItem = newSelection;
							selectedClipItems.Clear();
							selectedClipItems.Add(selectedClipItem);
						}

						Repaint();
					}
				}
				break;
			case EventType.MouseDrag:
				if(Event.current.mousePosition.y > bottomSplitter.pos && dragClipItem != null)
				{
					if(!selectedClipItems.Contains(dragClipItem))
					{
						selectedClipItem = dragClipItem;
						selectedClipItems.Clear();
						selectedClipItems.Add(selectedClipItem);
					}
					DragAndDrop.PrepareStartDrag();
					Object[] objects = new Object[1];
					objects[0] = dragClipItem.Clip;
					DragAndDrop.objectReferences = objects;
					DragAndDrop.StartDrag("Dragging " + dragClipItem.Name + " (AudioClip)");
					Event.current.Use();
				}
				else if(Event.current.mousePosition.x < leftSplitter.pos && dragTreeItem != null)
				{
					if(!selectedTreeItems.Contains(dragTreeItem))
					{
						selectedTreeItem = dragTreeItem;
						selectedTreeItems.Clear();
						selectedTreeItems.Add(selectedTreeItem);
					}
					DragAndDrop.PrepareStartDrag();
					Object[] objects = new Object[1];
					objects[0] = dragTreeItem.AsObject;
					DragAndDrop.objectReferences = objects;
					DragAndDrop.StartDrag("Dragging " + dragTreeItem.Name + " (" + objects[0].GetType() + ")");
					Event.current.Use();
				}
				break;
			case EventType.DragPerform:
			case EventType.DragUpdated:
				if(Event.current.mousePosition.x < leftSplitter.pos && Event.current.mousePosition.y < bottomSplitter.pos)
				{
					TreeItem hoverItem = null;
					Object dragObject = DragAndDrop.objectReferences[0];
					if(dragObject && dragObject.GetType() == typeof(AudioClip))
					{
						foreach(TreeItem item in displayedTreeItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								if(Event.current.type == EventType.DragPerform)
								{
									if(item.Cue != null && SECTR_VC.IsEditable(item.Path))
									{
										SECTR_Undo.Record(item.Cue, "Add Clip");
										foreach(TreeItem selectedItem in selectedClipItems)
										{
											if(selectedItem.Importer != null)
											{
												item.Cue.AddClip(selectedItem.Clip, false);
											}
										}
										selectedTreeItem = item;
										selectedTreeItems.Clear();
										selectedTreeItems.Add(selectedTreeItem);
										DragAndDrop.AcceptDrag();
										Repaint();
									}
									else if(item.Bus != null)
									{
										foreach(TreeItem selectedItem in selectedClipItems)
										{
											if(selectedItem.Importer != null)
											{
												TreeItem newItem = _CreateTreeItem(item, false, selectedItem.Clip.name);
												if(newItem != null)
												{
													SECTR_AudioCue cue = newItem.Cue;
#if UNITY_4
													AudioImporter importer = selectedItem.Importer;
													cue.Loops = importer.loopable;
													cue.Spatialization = importer.threeD ? SECTR_AudioCue.Spatializations.Local3D : SECTR_AudioCue.Spatializations.Simple2D;
#endif
													cue.AddClip(selectedItem.Clip, false);
													newItem.Rename = selectedClipItems.Count == 1;
												}
											}
										}
										AssetDatabase.SaveAssets();
										AssetDatabase.Refresh();
										DragAndDrop.AcceptDrag();
										Repaint();
									}
								}
								else
								{
									hoverItem = item;
									DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
								}
								break;
							}
						}
					}
					else if(dragObject && dragObject.GetType() == typeof(SECTR_AudioBus))
					{
						SECTR_AudioBus bus = ((SECTR_AudioBus)dragObject);
						foreach(TreeItem item in displayedTreeItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								if(item.Bus != null && bus != item.Bus && bus.Parent != item.Bus && !bus.IsAncestorOf(item.Bus) && 
									SECTR_VC.IsEditable(item.Path))
								{
									if(Event.current.type == EventType.DragPerform)
									{
										foreach(TreeItem selectedItem in selectedTreeItems)
										{
											if(selectedItem.Bus != null && selectedItem.Bus.Parent != item.Bus && !selectedItem.Bus.IsAncestorOf(item.Bus) &&
											   SECTR_VC.IsEditable(selectedItem.Path))
											{
												selectedItem.Bus.Parent = item.Bus;
												EditorUtility.SetDirty(selectedItem.Bus);
											}
										}
										DragAndDrop.AcceptDrag();
										Repaint();
									}
									else
									{
										hoverItem = item;
										DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
									}
								}
								break;
							}
						}
					}
					else if(dragObject && dragObject.GetType() == typeof(SECTR_AudioCue))
					{
						SECTR_AudioCue cue = ((SECTR_AudioCue)dragObject);
						foreach(TreeItem item in displayedTreeItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								if(item.Bus != null && item.Bus != cue.Bus && 
								   SECTR_VC.IsEditable(item.Path))
								{
									if(Event.current.type == EventType.DragPerform)
									{
										foreach(TreeItem selectedItem in selectedTreeItems)
										{
											if(selectedItem.Cue && selectedItem.Cue.Bus != item.Bus &&
											   SECTR_VC.IsEditable(selectedItem.Path))
											{
												selectedItem.Cue.Bus = item.Bus;
												EditorUtility.SetDirty(selectedItem.Cue);
											}
										}
										DragAndDrop.AcceptDrag();
										Repaint();
									}
									else
									{
										hoverItem = item;
										DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
									}
								}
								break;
							}
						}
					}

					if(dragHoverItem != hoverItem)
					{
						dragHoverItem = hoverItem;
						Repaint();
					}
				}
				break;
			case EventType.KeyUp:
				if(selectedTreeItem != null && selectedTreeItem.Rename)
				{
					if(Event.current.keyCode == KeyCode.Escape)
					{
						selectedTreeItem.Name = selectedTreeItem.DefaultName;
						selectedTreeItem.Rename = false;
						Repaint();
					}
					else if(Event.current.keyCode == KeyCode.Return)
					{
						string newPath = selectedTreeItem.Path.Replace(selectedTreeItem.DefaultName + ".asset", selectedTreeItem.Name + ".asset");
						bool bus = selectedTreeItem.Bus != null;
						bool cue = selectedTreeItem.Cue != null;
						bool importer = selectedTreeItem.Importer != null;
						AssetDatabase.RenameAsset(selectedTreeItem.Path, selectedTreeItem.Name);
						SECTR_VC.WaitForVC();

						_RemoveTreeItem(selectedTreeItem);

						TreeItem renamedItem = null;
						if(bus)
						{
							SECTR_AudioBus newBus = (SECTR_AudioBus)AssetDatabase.LoadAssetAtPath(newPath, typeof(SECTR_AudioBus));
							if(newBus)
							{
								renamedItem = new TreeItem(this, newBus, newPath);
							}
						}
						else if(cue)
						{
							SECTR_AudioCue newCue = (SECTR_AudioCue)AssetDatabase.LoadAssetAtPath(newPath, typeof(SECTR_AudioCue));
							if(newCue)
							{
								renamedItem = new TreeItem(this, newCue, newPath);
							}
						}
						else if(importer)
						{
							AudioImporter newImporter = (AudioImporter)AssetImporter.GetAtPath(newPath);
							if(newImporter)
							{
								renamedItem = new TreeItem(newImporter, newPath, selectedTreeItem.Name);
							}
						}
						if(renamedItem != null)
						{
							selectedTreeItem = renamedItem;
							selectedTreeItems.Clear();
							selectedTreeItems.Add(selectedTreeItem);
						}
						else
						{
							Debug.LogWarning("Unable to rename asset. Name may already be in use.");
							_RefreshAssets();
						}
						GUI.FocusControl(null);
						Repaint();
					}
				}
				else if(changingBitrate)
				{
					if(Event.current.keyCode == KeyCode.Return)
					{
						changingBitrate = false;
						confirmBitrate = true;
						GUI.FocusControl(null);
						Repaint();
					}
				}
				else
				{
					switch(Event.current.keyCode)
					{
					case KeyCode.Return:
						if(selectedTreeItem != null && SECTR_VC.IsEditable(selectedTreeItem.Path))
						{
							selectedTreeItem.Rename = true;
							Repaint();
						}
						break;
					case KeyCode.D:
						if(selectedTreeItem != null &&
					#if UNITY_EDITOR_OSX
						Event.current.command
					#else
						Event.current.control
					#endif
						)
						{
							_DuplicateSelectedHierarchyItem();
						}
						break;
					case KeyCode.Delete:
					case KeyCode.Backspace:
						if(selectedTreeItem != null && SECTR_VC.IsEditable(selectedTreeItem.Path))
						{
							_DeleteSelectedHierarchyItem();
						}
						break;
					case KeyCode.Escape:
						SECTR_AudioSystem.StopAudition();
						break;
					case KeyCode.Space:
						if(lastSelectedTree && selectedTreeItem != null && selectedTreeItem.Cue != null)
						{
							SECTR_AudioSystem.Audition(selectedTreeItem.Cue);
						}
						else if(!lastSelectedTree && selectedClipItem != null)
						{
							SECTR_AudioSystem.Audition(selectedClipItem.Clip);
						}
						break;
					case KeyCode.UpArrow:
					case KeyCode.DownArrow:
					case KeyCode.LeftArrow:
					case KeyCode.RightArrow:
						if(lastSelectedTree && selectedTreeItem != null)
						{
							int numDisplayed = displayedTreeItems.Count;
							for(int treeIndex = 0; treeIndex < numDisplayed; ++treeIndex)
							{
								if(displayedTreeItems[treeIndex] == selectedTreeItem)
								{
									if(Event.current.keyCode == KeyCode.UpArrow && treeIndex > 0)
									{
										selectedTreeItem = displayedTreeItems[treeIndex - 1];
										if(!Event.current.shift)
										{
											selectedTreeItems.Clear();
										}
										if(!selectedTreeItems.Contains(selectedTreeItem))
										{
											selectedTreeItems.Add(selectedTreeItem);
										}
									}
									else if(Event.current.keyCode == KeyCode.DownArrow && treeIndex < numDisplayed - 1)
									{
										selectedTreeItem = displayedTreeItems[treeIndex + 1];
										if(!Event.current.shift)
										{
											selectedTreeItems.Clear();
										}
										if(!selectedTreeItems.Contains(selectedTreeItem))
										{
											selectedTreeItems.Add(selectedTreeItem);
										}
									}
									else if(Event.current.keyCode == KeyCode.RightArrow && selectedTreeItem.Bus != null)
									{
										selectedTreeItem.Expanded = true;
									}
									else if(Event.current.keyCode == KeyCode.LeftArrow && selectedTreeItem.Bus != null)
									{
										selectedTreeItem.Expanded = false;
									}
									Repaint();
									break;
								}
							}
						}
						else if(!lastSelectedTree && selectedClipItem != null)
						{
							int numDisplayed = displayedClipItems.Count;
							for(int treeIndex = 0; treeIndex < numDisplayed; ++treeIndex)
							{
								if(displayedClipItems[treeIndex] == selectedClipItem)
								{
									if(Event.current.keyCode == KeyCode.UpArrow && treeIndex > 0)
									{
										selectedClipItem = displayedClipItems[treeIndex - 1];
										selectedClipItems.Clear();
										selectedClipItems.Add(selectedClipItem);
										if(selectedClipItem.ScrollRect.y < clipScrollPos.y)
										{
											clipScrollPos.y = selectedClipItem.ScrollRect.y;
										}
									}
									else if(Event.current.keyCode == KeyCode.DownArrow && treeIndex < numDisplayed - 1)
									{
										selectedClipItem = displayedClipItems[treeIndex + 1];
										selectedClipItems.Clear();
										selectedClipItems.Add(selectedClipItem);
										if(selectedClipItem.ScrollRect.y > clipScrollPos.y + clipScrollRect.height)
										{
											clipScrollPos.y = selectedClipItem.ScrollRect.y;
										}
									}
									else if(Event.current.keyCode == KeyCode.RightArrow && selectedClipItem != null)
									{
										selectedClipItem.Expanded = true;
									}
									else if(Event.current.keyCode == KeyCode.LeftArrow && selectedClipItem != null)
									{
										selectedClipItem.Expanded = false;
									}
									Repaint();
									break;
								}
							}
						}

						break;
					}
				}
				break;
			case EventType.ContextClick:

				GenericMenu menu = new GenericMenu();
				
				menu.AddItem(new GUIContent("Show Hierarchy"), showHierarchy, delegate() 
				{
					showHierarchy = !showHierarchy;
					UnityEditor.EditorPrefs.SetBool(showPrefPrefix + "Hierarchy", showHierarchy);
				});
				menu.AddItem(new GUIContent("Show Properties"), showProperties, delegate() 
				{
					showProperties = !showProperties;
					UnityEditor.EditorPrefs.SetBool(showPrefPrefix + "Properties", showProperties);
				});
				menu.AddItem(new GUIContent("Show Audio Clips"), showClipList, delegate() 
				{
					showClipList = !showClipList;
					UnityEditor.EditorPrefs.SetBool(showPrefPrefix + "ClipList", showClipList);
				});

				if(Event.current.mousePosition.x < leftSplitter.pos || Event.current.mousePosition.y > bottomSplitter.pos)
				{
					menu.AddSeparator(null);
					bool hasVC = SECTR_VC.HasVC();
					if(Event.current.mousePosition.x < leftSplitter.pos && Event.current.mousePosition.y < bottomSplitter.pos)
					{
						TreeItem cloneItem = null;
						foreach(TreeItem item in displayedTreeItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								cloneItem = item;
								bool editable = !hasVC || SECTR_VC.IsEditable(item.Path);

								if(hasVC)
								{
									if(!editable)
									{
										menu.AddItem(new GUIContent("Check Out"), false, delegate() 
										{
											foreach(TreeItem selectedItem in selectedTreeItems)
											{
												SECTR_VC.CheckOut(selectedItem.Path);
											}
										});
									}
									else
									{
										menu.AddItem(new GUIContent("Revert"), false, delegate()
										{
											if(EditorUtility.DisplayDialog("Are you sure?", "Reverting will discard all changes to " + item.Name + ". This cannot be Undone." , "Ok", "Cancel") )
											{
												foreach(TreeItem selectedItem in selectedTreeItems)
												{
													SECTR_VC.Revert(selectedItem.Path);
												}
												_RefreshAssets();
											}
										});
									}
								}
								
								menu.AddItem(new GUIContent("Show In Project"), false, delegate()
								{
									if(item.Cue != null) Selection.activeObject = item.Cue;
									if(item.Bus != null) Selection.activeObject = item.Bus;
									EditorUtility.FocusProjectWindow();
								});
								menu.AddSeparator("");

								menu.AddItem(new GUIContent("Duplicate"), false, _DuplicateSelectedHierarchyItem);

								if(editable)
								{
									menu.AddItem(new GUIContent("Rename"), false, delegate() 
									{
										selectedTreeItem = item;
										selectedTreeItems.Clear();
										selectedTreeItems.Add(selectedTreeItem);
										selectedTreeItem.Rename = true;
										propertyEditor = null;
										Repaint();
									});
								}
								else
								{
									menu.AddSeparator("Rename");
								}

								if(editable)
								{
									menu.AddItem(new GUIContent("Delete"), false, _DeleteSelectedHierarchyItem);
								}
								else
								{
									menu.AddSeparator("Delete");
								}

								menu.AddSeparator(null);
								break;
							}
						}

						menu.AddItem(new GUIContent("Create New Bus"), false, delegate() 
						{
							_CreateTreeItem(cloneItem, true, null);
							Repaint();
						});
						menu.AddItem(new GUIContent("Create New Cue"), false, delegate() 
						{
							_CreateTreeItem(cloneItem, false, null);
							Repaint();
						});
					}
					else if(Event.current.mousePosition.y > bottomSplitter.pos)
					{
						foreach(TreeItem item in displayedClipItems)
						{
							if(item.WindowRect.Contains(Event.current.mousePosition))
							{
								// Project Items
								if(SECTR_VC.HasVC())
								{
									if(!SECTR_VC.IsEditable(item.Path))
									{
										menu.AddItem(new GUIContent("Check Out"), false, delegate()
										{
											foreach(TreeItem selectedItem in selectedClipItems)
											{
												SECTR_VC.CheckOut(selectedItem.Path);
											}
										});
									}
									else
									{
										menu.AddItem(new GUIContent("Revert"), false, delegate()
										{
											if(EditorUtility.DisplayDialog("Are you sure?", "Reverting will discard all changes to selected clips. This cannot be Undone." , "Ok", "Cancel") )
											{
												foreach(TreeItem selectedItem in selectedClipItems)
												{
													SECTR_VC.Revert(selectedItem.Path);
												}
												_RefreshAssets();
											}
										});
									}
								}
								menu.AddItem(new GUIContent("Show In Project"), false, delegate()
								{
									Selection.activeObject = item.Clip;
									EditorUtility.FocusProjectWindow();
								});
								menu.AddSeparator("");

								// Creation Items
								if(selectedTreeItem != null && selectedTreeItem.Cue != null && 
								   !selectedTreeItem.Cue.HasClip(item.Clip))
								{
									menu.AddItem(new GUIContent("Add Selected to Cue"), false, delegate()
									{
										SECTR_Undo.Record(selectedTreeItem.Cue, "Add Clips");
										foreach(TreeItem selectedItem in selectedClipItems)
										{
											selectedTreeItem.Cue.AddClip(selectedItem.Clip, false);
										}
										Repaint();
									});
								}
								else
								{
									menu.AddSeparator("Add Clip to Selected Cue");
								}
								menu.AddItem(new GUIContent("Create Cues from Selected"), false, delegate() 
								{
									foreach(TreeItem selectedItem in selectedClipItems)
									{
										TreeItem newItem = _CreateTreeItem(selectedTreeItem, false, selectedItem.Clip.name);
										if(newItem != null)
										{
											newItem.Rename = selectedClipItems.Count == 1;
											SECTR_AudioCue cue = newItem.Cue;
#if UNITY_4_0
											AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(selectedItem.Path);
											cue.Spatialization = importer.threeD ? SECTR_AudioCue.Spatializations.Local3D : SECTR_AudioCue.Spatializations.Simple2D;
											cue.Loops = importer.loopable;
#endif
											cue.AddClip(selectedItem.Clip, false);
										}
									}
									Repaint();
								});
								menu.AddSeparator("");
								break;
							}
						}

						menu.AddItem(new GUIContent("Show Advanced Properties"), showFullClipDetails, delegate()
						{
							showFullClipDetails = !showFullClipDetails;
							UnityEditor.EditorPrefs.SetBool(fullDetailsPref, showFullClipDetails);
							Repaint();
						});

						menu.AddItem(new GUIContent("Close All Folders"), false, delegate()
						{
							foreach(string path in clipItems.Keys)
							{
								clipItems[path].expanded = false;
								UnityEditor.EditorPrefs.SetBool(expandedPrefPrefix + path, false);
							}
							Repaint();
						});
					}
				}

				// Items in all menus.
				menu.AddSeparator(null);
				menu.AddItem(new GUIContent("Bake HDR Keys"), false, delegate() 
				{
					List<string> paths = new List<string>();
					List<string> extensions = new List<string>()
					{
						".asset",
					};
					bakeMaster = SECTR_ComputeRMS.BakeList(SECTR_Asset.GetAll<SECTR_AudioCue>(audioRootPath, extensions, ref paths, false));
				});
				menu.AddSeparator(null);
				menu.AddItem(new GUIContent("Refresh Assets"), false, delegate() 
				{
					_RefreshAssets();
				});
				menu.AddSeparator(null);
				menu.AddItem(new GUIContent("Change Audio Root"), false, delegate() 
				{
					_SelectAudioRoot();
					_RefreshAssets();
				});
				menu.ShowAsContext();
				break;
			}

		}
	}

	private TreeItem _CreateTreeItem(TreeItem selectedItem, bool createBus, string name)
	{
		SECTR_AudioBus parentBus = null;
		string dirPath = audioRootPath;
		string fileName = "";
		string typeName = createBus ? "Bus" : "Cue";
		if(string.IsNullOrEmpty(name))
		{
			name = "New" + typeName;
		}
		if(selectedItem != null)
		{
			if(selectedItem.Bus)
			{
				parentBus = selectedItem.Bus;
			}
			else if(selectedItem.Cue)
			{
				parentBus = selectedItem.Cue.Bus;
			}
			SECTR_Asset.SplitPath(selectedItem.Path, out dirPath, out fileName);
		}
		else 
		{
			foreach(TreeItem item in hierarchyItems.Values)
			{
				if((item.Bus && item.Bus.Parent == null) || (item.Cue && item.Cue.Bus == null))
				{
					SECTR_Asset.SplitPath(item.Path, out dirPath, out fileName);
					break;
				}
			}
			
			dirPath = EditorUtility.SaveFolderPanel("Chose " + typeName + " Location", dirPath, "");
			if(string.IsNullOrEmpty(dirPath))
			{
				return null;
			}
			dirPath = dirPath.Replace(Application.dataPath, "Assets");
		}
		
		string assetPath = "";
		TreeItem newItem;
		if(createBus)
		{
			SECTR_AudioBus bus = SECTR_Asset.Create<SECTR_AudioBus>(dirPath, name, ref assetPath);
			bus.Parent = parentBus;
			newItem = new TreeItem(this, bus, assetPath);
		}
		else
		{
			SECTR_AudioCue cue = SECTR_Asset.Create<SECTR_AudioCue>(dirPath, name, ref assetPath);
			cue.Bus = parentBus;
			newItem = new TreeItem(this, cue, assetPath);
		}

		newItem.Rename = true;
		selectedTreeItem = newItem;
		selectedTreeItems.Clear();
		selectedTreeItems.Add(selectedTreeItem);
		propertyEditor = null;
		return newItem;
	}

	private void _RemoveTreeItem(TreeItem item)
	{
		if(item.Bus)
		{
			EditorPrefs.DeleteKey(expandedPrefPrefix + item.Path);
		}
		if(item == selectedTreeItem)
		{
			selectedTreeItem = null;
			selectedTreeItems.Clear();
		}
		if(item == selectedClipItem)
		{
			selectedClipItem = null;
		}
		hierarchyItems.Remove(item.AsObject);
	}

	private void _SelectAudioRoot()
	{
		audioRootPath = EditorUtility.OpenFolderPanel("Choose AUDIO Root", audioRootPath, "");
		if(!string.IsNullOrEmpty(audioRootPath))
		{
			UnityEditor.EditorPrefs.SetString(rootPrefPrefix + SECTR_Asset.GetProjectName(), audioRootPath);
		}
	}

	private void _DeleteSelectedHierarchyItem()
	{
		if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete the selected clips? This cannot be Undone." , "Ok", "Cancel") )
		{
			List<TreeItem> oldSelection = new List<TreeItem>(selectedTreeItems);
			foreach(TreeItem selectedItem in oldSelection)
			{
				if(selectedItem.Bus != null && (selectedItem.Bus.Children.Count > 0 || selectedItem.Bus.Cues.Count > 0))
				{
					EditorUtility.DisplayDialog("Cannot Delete", selectedItem.Name + " cannot be deleted while it has children.", "Ok"); 
				}
				else
				{
					_RemoveTreeItem(selectedItem);
					ScriptableObject.DestroyImmediate(selectedItem.AsObject, true);
					AssetDatabase.DeleteAsset(selectedItem.Path);
				}
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Resources.UnloadUnusedAssets();
			selectedTreeItem = null;
			selectedTreeItems.Clear();
			Repaint();
		}
	}

	private void _DuplicateSelectedHierarchyItem()
	{
		List<TreeItem> newItems = new List<TreeItem>(selectedTreeItems.Count);
		foreach(TreeItem selectedItem in selectedTreeItems)
		{
			TreeItem newItem = null;
			string dirPath = "";
			string fileName = "";
			SECTR_Asset.SplitPath(selectedItem.Path, out dirPath, out fileName);
			
			string newPath = dirPath + selectedItem.Name + " Copy.asset";
			if(AssetDatabase.CopyAsset(selectedItem.Path, newPath))
			{
				SECTR_VC.WaitForVC();
				if(selectedItem.Bus)
				{
					SECTR_AudioBus bus = SECTR_Asset.Load<SECTR_AudioBus>(newPath);
					newItem = new TreeItem(this, bus, newPath);
				}
				else if(selectedItem.Cue)
				{
					SECTR_AudioCue cue = SECTR_Asset.Load<SECTR_AudioCue>(newPath);
					newItem = new TreeItem(this, cue, newPath);
				}
				
				if(newItem != null)
				{
					newItems.Add(newItem);
				}
			}
		}
		selectedTreeItems = newItems;
		selectedTreeItem = selectedTreeItems.Count > 0 ? selectedTreeItems[0] : null;
		propertyEditor = null;
		Repaint();
	}
	#endregion
}
