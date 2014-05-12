using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Load : MonoBehaviour {

	private List<GameObject> ml_Object_map;
	private List<string> ml_tmp_file;

	private DirectoryInfo m_directoryInfo;
	
	void Start () 
	{
		ml_Object_map = null;
		ml_tmp_file = null;
	}
	
	public void load(string name, string s_location)
	{
		XmlReaderSettings settings = new XmlReaderSettings();
		settings.IgnoreWhitespace = true; 
		settings.IgnoreComments = true;
		settings.IgnoreProcessingInstructions = true;
		
		if (ml_Object_map.Count > 0)
		{
			foreach (GameObject game in ml_Object_map)
				Destroy(game);
		}
		
		ml_Object_map.Clear();
		m_directoryInfo = new DirectoryInfo(s_location);
		
		ml_tmp_file.Clear();

		FileInfo[] file;
		file = m_directoryInfo.GetFiles();
		
		foreach (FileInfo info in file)
			ml_tmp_file.Add(info.Name);
		
		if (m_directoryInfo.Exists && ml_tmp_file.Contains(name) == true)
		{
			using (XmlReader xml_reader = XmlReader.Create(s_location + "\\" + name, settings))
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
					ml_Object_map.Add(obj);				
				}
				xml_reader.ReadEndElement();
			}
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
