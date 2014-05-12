using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

public class SaveLoad : MonoBehaviour 
{	
	public Texture2D mt_icon;
	public Texture2D mt_cross;
	public GUIStyle m_mTestStyle;
	public GUISkin m_window;

#region
		
	private string ms_location;
	private string[] msa_GridDirectory;
	private string ms_file_name;
	private string ms_tmp_file_name;
	private string root;	
	private string pattern;

	private int mi_selGridDir = -1;
	private int mi_save_file;
	private int mi_selected_button;
	private int mi_index_file;

	private List<string> ml_tmp_dir;
	private List<string> ml_tmp_file;

	private DirectoryInfo m_directorySelection;
	private DirectoryInfo m_directoryInfo;
	
	private Vector2 mv_SbarValue;
	
	private Interface m_interface;
	
	#endregion // private variable	
	
	void Start () 
	{
		ms_location = ".";
		ms_file_name = "new_file";
		
		mi_save_file = -1;
		mi_selected_button = -1;
		mi_index_file = -1;

		ml_tmp_dir = new List<string>();
		ml_tmp_file = new List<string>();
		
		m_directorySelection = new DirectoryInfo(ms_location);
		ms_location = m_directorySelection.FullName;
		root = ms_location.Split('\\')[0];		
		mv_SbarValue = new Vector2();
		
		m_interface = Interface.Instance;
		pattern = @"^[A-Z]:\\.*$";
	}
	
	#region
	
	void GetDir()
	{
		ml_tmp_dir.Clear();
		
		m_directoryInfo = new DirectoryInfo(ms_location);
		
		DirectoryInfo[] dir;
		
		try
		{
			dir = m_directoryInfo.GetDirectories();
		}
		catch
		{
			return ;
		}
		
		aff_dir(dir);
	}
	
	void aff_dir(DirectoryInfo[] dir)
	{
		
		foreach (DirectoryInfo name in dir)
			ml_tmp_dir.Add(name.Name);
		
		if (ml_tmp_dir.Count > 0)
		{
			msa_GridDirectory = new string[ml_tmp_dir.Count];
			
			for (int i = 0; i < ml_tmp_dir.Count; i++)
				msa_GridDirectory[i] = ml_tmp_dir[i];
			
			mi_selGridDir = GUILayout.SelectionGrid(mi_selGridDir, msa_GridDirectory, 1);		
		}
	}
	
	void GetFile()
	{
		ml_tmp_file.Clear();

		FileInfo[] file;
		try
		{
			file = m_directoryInfo.GetFiles();
		}
		catch
		{
			return ;
		}
		
		aff_file(file);
	}
	
	void aff_file(FileInfo[] file)
	{
		foreach (FileInfo name in file)
			ml_tmp_file.Add(name.Name);
		
		if (ml_tmp_file.Count > 0)
		{
			for (int i = 0; i < ml_tmp_file.Count; i++)
			{
				if (ml_tmp_file[i].Contains(".xml") == false)
				{
					GUI.enabled = false;
					GUILayout.Toggle(false, ml_tmp_file[i], m_mTestStyle);
					GUI.enabled = true;
				}
				else
				{
					if ((GUILayout.Toggle(mi_selected_button == i, ml_tmp_file[i], m_mTestStyle) && !(mi_selected_button == i)))
					{
						mi_selected_button = i;
						mi_index_file = i;
					}
				}
			}
		}
	}
	
	void FileBrowser()
	{
		m_directorySelection = new DirectoryInfo(ms_location);
		ms_tmp_file_name = "";

		if ((m_directorySelection.Attributes & FileAttributes.Directory) == FileAttributes.Directory && m_directorySelection.Exists)
	    {
			GetDir();
			GetFile();
			
			// if user change directory
			if (mi_selGridDir > -1)
			{
				ms_location = ms_location + "\\" + msa_GridDirectory[mi_selGridDir] + "\\";
				mi_index_file = -1;
				mi_save_file = -1;
				mi_selected_button = -1;
			}
			
			// if user clic on file
			if (mi_index_file > -1)
			{
				ms_tmp_file_name = ml_tmp_file[mi_index_file];
			}
	    }
	}
	
	#endregion // File Browser
	
	#region 
	
	void GoBack()
	{
		if ((ms_location.Length > 4 && ms_location != "." && Regex.IsMatch(ms_location,pattern) == true) && GUILayout.Button(mt_icon, GUILayout.Width(40)))
	    {
	        m_directoryInfo = m_directoryInfo.Parent;
	        ms_location = m_directoryInfo.FullName;
			mi_index_file = -1;
			mi_save_file = -1;
			mi_selected_button = -1;
	    }
	}
	
	void Leave()
	{
		GUILayout.Space(710);
		if (GUILayout.Button(mt_cross, GUILayout.Width(40)))
		{
			this.gameObject.SetActive(false);
		}
	}

	#endregion // Go Back and Leave	
	
	#region
	
	void Address()
	{
		epur_string();
		
		m_directorySelection = new DirectoryInfo(ms_location);

		ms_location = GUILayout.TextArea(ms_location);

		epur_string();
		
		m_directorySelection = new DirectoryInfo(ms_location);
	}
	
	void epur_string()
	{
		if (ms_location == "" || (ms_location.Length <= 4 && ms_location != "." && ms_location.Contains(root + "\\") == false))
		{
			if (ms_location.Length >= 4)
			{
				ms_location = ms_location.Remove(0,3);
				ms_location = ms_location.Insert(0, root+"\\");
			}
			else
				ms_location = root+"\\";
		}
		ms_location = ms_location.Replace("\n", string.Empty);
	}

	#endregion // Address
	
	void OnGUI()
	{	
		GUILayout.BeginArea(new Rect( Screen.width /4, Screen.height/4f, Screen.width / 2, Screen.height/1.5f), m_window.FindStyle("Box"));
		mi_selGridDir = -1;
		
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		GUILayout.Space(28);
		GUILayout.BeginHorizontal();

		GoBack();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		Leave();
		GUILayout.EndHorizontal();
		Address();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		mv_SbarValue = GUILayout.BeginScrollView(mv_SbarValue);
		GUILayout.BeginVertical();
		FileBrowser();
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		
		if (mi_index_file != mi_save_file)
		{
			ms_file_name = GUILayout.TextField(ms_tmp_file_name);
			mi_save_file = mi_index_file;
		}
		else
			ms_file_name = GUILayout.TextField(ms_file_name);
		
		if (ms_file_name.Contains(".xml") == false)
			ms_file_name += ".xml";
		
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();
		GUILayout.Space(5);
		if (GUILayout.Button("Save"))
			save(ms_file_name, ms_location);
		if (GUILayout.Button("Load"))
			load (ms_file_name, ms_location);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}
	
	public void save(string name, string ms_location)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		
		if (m_directorySelection.Exists)
		{
			using (XmlWriter writer = XmlWriter.Create(ms_location + "\\" + name, settings)) 
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("GameObject");
				for (int i = 0; i < m_interface.ml_Object_map.Count; i++)
				{
					writer.WriteStartElement("Object");
					writer.WriteAttributeString("Name", m_interface.ml_Object_map[i].name);
					writer.WriteStartElement("Position_X");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.position.x);
					writer.WriteEndElement();
					writer.WriteStartElement("Position_Y");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.position.y);
					writer.WriteEndElement();
					writer.WriteStartElement("Position_Z");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.position.z);
					writer.WriteEndElement();
					writer.WriteStartElement("Rotation_X");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.eulerAngles.x);
					writer.WriteEndElement();
					writer.WriteStartElement("Rotation_Y");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.eulerAngles.y);
					writer.WriteEndElement();
					writer.WriteStartElement("Rotation_Z");
					writer.WriteValue(m_interface.ml_Object_map[i].transform.eulerAngles.z);
					writer.WriteEndElement();
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}
	}	
	
	public void load(string name, string ms_location)
	{
		XmlReaderSettings settings = new XmlReaderSettings();
		settings.IgnoreWhitespace = true; 
		settings.IgnoreComments = true;
		settings.IgnoreProcessingInstructions = true;
		
		if (m_interface.ml_Object_map.Count > 0)
		{
			foreach (GameObject game in m_interface.ml_Object_map)
				Destroy(game);
		}
		m_interface.ml_Object_map.Clear();
		if (m_directorySelection.Exists && ml_tmp_file.Contains(name) == true)
		{
			using (XmlReader xml_reader = XmlReader.Create(ms_location + "\\" + name, settings))
			{
				xml_reader.ReadStartElement("GameObject");
				while (xml_reader.EOF != true && xml_reader.Name != "GameObject")
				{
					Vector3 tmp_pos = new Vector3();
					Vector3 tmp_rot = new Vector3();
					string tmp_name;
					
					tmp_name = xml_reader.GetAttribute("Name");
					xml_reader.ReadStartElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_pos.x);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_pos.y);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_pos.z);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_rot.x);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_rot.y);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadStartElement();
					float.TryParse(xml_reader.ReadString(), out tmp_rot.z);
					xml_reader.ReadEndElement();
					
					xml_reader.ReadEndElement();
	
					GameObject obj = Instantiate(Resources.Load(tmp_name)) as GameObject;
					obj.name = tmp_name;
					obj.transform.eulerAngles = tmp_rot;
					obj.transform.position = tmp_pos;
					m_interface.ml_Object_map.Add(obj);				
				}
				xml_reader.ReadEndElement();
			}
		}
	}
}
