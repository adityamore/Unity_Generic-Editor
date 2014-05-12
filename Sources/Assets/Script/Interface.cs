using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

public class Interface : MonoBehaviour 
{
	public 	TextAsset mta_xmlfile;
	public 	GameObject mg_defaut;
	public 	string s_text_area;
	public GameObject save_load;
	
	public List<GameObject> ml_Object_map;
	public GUISkin m_window;
	#region

	private List<InterfaceElement> ml_list_gui;
	private static Interface m_instance;
	private Map m_map;
	private enum me_button_type
	{
		undefined = 0,
		button = 1,
		toggle = 2,
		textarea = 3
	};

	private struct InterfaceElement
	{
		public me_button_type mse_type;
		public string s_content;
		public bool b_toggle_type;
		public string s_text_area;
		public List<Element> l_element;
	}
	
	private int mi_selected_button = -1;
	private int mi_selected_group = -1;
	private Element mc_save;
	private Rect mr_rect;
	private Vector2 mv_SbarValue;

	private MoveObject m_obj;
	
	#endregion // private variable
	
	// Read and extract into InterfaceRight class each attribut of node name "group"
	InterfaceElement check_attribute_group(XmlReader xml_reader)
	{
		InterfaceElement s_InterfaceElement = new InterfaceElement();
		s_InterfaceElement.mse_type = 0;
		s_InterfaceElement.b_toggle_type = false;
		s_InterfaceElement.s_content = "undefined";
		s_InterfaceElement.s_text_area = "Choose a title";

		if (xml_reader.HasAttributes)
		{
			while (xml_reader.MoveToNextAttribute() != false)
			{
				bool valide_type = true;
				string save_type = "";
				string save_readstring = "";
				string save_content = "";
				
				// if type exist, else, a button will be created					
				if ((save_type = xml_reader.GetAttribute("type")) != null)
				{
					if (Enum.IsDefined(typeof(me_button_type),save_type) == true)
						s_InterfaceElement.mse_type = (me_button_type)Enum.Parse(typeof(me_button_type), save_type);
					else
						valide_type = false;
				}
				
				// if content as attribute or string exist, else, "undefined" has been saved		
				if ((save_content = xml_reader.GetAttribute("content")) != null || (save_readstring = xml_reader.ReadString()) != null)
				{
					s_InterfaceElement.s_content = save_content != null ? save_content : save_readstring;
					if (valide_type == false)
						s_InterfaceElement.s_content += " (Coming Soon)";
				}
			}
		}
		return s_InterfaceElement;
	}

	// Read and extract into Element class each attribut of node name "element"
	Element check_attribute_element(XmlReader xml_reader, Element element)
	{
		if (xml_reader.HasAttributes)
		{
			while (xml_reader.MoveToNextAttribute() != false)
			{
				string prefab_name = "";
				
				if ((prefab_name = xml_reader.GetAttribute("prefab")) != null)
				{
					if (Resources.Load(prefab_name) != null)
						element.prefab = Resources.Load(prefab_name) as GameObject;
				}
			}
		}
		return element;
	}
	
	void Awake()
	{
		m_instance = this;
	}
	
	// Parse the XML file
	void Start()
	{	
		// Initialisation of interface list
		ml_Object_map = new List<GameObject>();
		ml_list_gui = new List<InterfaceElement>();
		
		s_text_area = "Enter a title";
		save_load.SetActive(false);
		mc_save = null;
		mr_rect = new Rect(Screen.width/1.20F, Screen.height/4F, Screen.width/9F, Screen.height/3F);
		
		m_obj = MoveObject.Instance;
		m_map = Map.Instance;

		// Options for XmlReader, ignore whitespace, comments and continue to read
		XmlReaderSettings settings = new XmlReaderSettings();
		settings.IgnoreWhitespace = true; 
		settings.IgnoreComments = true;
		
		using (XmlReader xml_reader = XmlReader.Create(new StringReader(mta_xmlfile.text),settings))
		{
			xml_reader.ReadStartElement("menu");
			
			// while node is an element and its name is "group"
			while (xml_reader.NodeType == XmlNodeType.Element && xml_reader.Name == "group") 
			{
				// Extract attributs
				InterfaceElement s_InterfaceElement;
				s_InterfaceElement = check_attribute_group(xml_reader);
				s_InterfaceElement.l_element = new List<Element>();

				// read in the node
				xml_reader.ReadStartElement("group");
				
				// while node is an element and its name is "element"
				while (xml_reader.NodeType == XmlNodeType.Element && xml_reader.Name == "element")
				{
					Element element = new Element(mg_defaut);
					element = check_attribute_element(xml_reader, element);
					xml_reader.ReadStartElement("element");
					element.content = xml_reader.ReadString();
					s_InterfaceElement.l_element.Add(element);

					// element
					xml_reader.ReadEndElement();
				}
				
				// group
				xml_reader.ReadEndElement();
				ml_list_gui.Add(s_InterfaceElement);
			}
			
			// menu
			xml_reader.ReadEndElement();
		}
	}
	
	// show group toggle and elements
	void aff_toggle(int i_i)
	{
		InterfaceElement temp =  ml_list_gui[i_i];
		temp.b_toggle_type = GUILayout.Toggle(temp.b_toggle_type, ml_list_gui[i_i].s_content);
		ml_list_gui[i_i] = temp;
		if (ml_list_gui[i_i].b_toggle_type == true)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20F);
			GUILayout.BeginVertical();
					
			// Show each toggle for the toggle group
			for (int i = 0; i < ml_list_gui[i_i].l_element.Count; i++)
			{
				if (m_map.start == true && ml_list_gui[i_i].l_element[i].content == "Start")
					GUI.enabled = false;
				if (m_map.end == true && ml_list_gui[i_i].l_element[i].content == "Finish")				
					GUI.enabled = false;
					
				if ((ml_list_gui[i_i].l_element[i].state = 
					GUILayout.Toggle((mi_selected_group == i_i && mi_selected_button == i), ml_list_gui[i_i].l_element[i].content) && !(mi_selected_group == i_i && mi_selected_button == i)))
				{
					mi_selected_group = i_i;
					mi_selected_button = i;
					mc_save = ml_list_gui[i_i].l_element[i];
				}
				GUI.enabled = true;
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}
	
	// set _save_load and reset some variables
	void active_save_load(int i_i)
	{
		if (GUILayout.Button(ml_list_gui[i_i].s_content))
		{
			if (save_load.activeSelf == true)
			{
				save_load.SetActive(false);

			}
			else if (save_load.activeSelf == false)
			{
				save_load.SetActive(true);
				if (mi_selected_group > -1 && mi_selected_button > -1)
				{
					ml_list_gui[mi_selected_group].l_element[mi_selected_button].state = false;
					mi_selected_button = -1;
					mc_save = null;
				}
			}
		}	
	}
	
	void OnGUI()
	{
		if (save_load.activeSelf == false)
			GUI.enabled = true;
		else
			GUI.enabled = false;
		
		GUI.depth = -1;
		GUILayout.BeginArea (mr_rect, m_window.FindStyle("Box"));
		mv_SbarValue = GUILayout.BeginScrollView(mv_SbarValue);
		for (int i_i = 0; i_i < ml_list_gui.Count ; i_i++)
		{
			// test the type of GUI
			switch (ml_list_gui[i_i].mse_type)
			{
			case me_button_type.button:
//				if (m_map.complete == false)
//					GUI.enabled = false;
				if (ml_list_gui[i_i].s_content == "Save/Load")
				{
					active_save_load(i_i);
					GUI.enabled = true;
				}
				else
				{
					GUI.enabled = true;
					GUILayout.Button(ml_list_gui[i_i].s_content);
				}
				break;
				
			case me_button_type.toggle:
				aff_toggle(i_i);
				break;
				
			case me_button_type.textarea:
				s_text_area = GUILayout.TextArea(s_text_area, 100);
				break;
				
			default:
				GUILayout.Button(ml_list_gui[i_i].s_content);
				break;
			}
		}
		
		if (m_obj.selectObject != null)
		{
			if (GUILayout.Button("Erase Object") == true)
			{
				if (m_obj.selectObject.name == "Start")
					m_map.start = false;
				if (m_obj.selectObject.name == "Finish")
					m_map.end = false;
				foreach (Block block in m_map.m_piste)
				{
					if (block.me == m_obj.selectObject)
					{
						Destroy(block.me);
						m_map.m_piste.Remove(block);
						m_obj.Gizmo.SetActive(false);
					}
				}
				ml_Object_map.Remove(m_obj.selectObject);
				Destroy(m_obj.selectObject);
				m_obj.Gizmo.SetActive(false);
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
    }
	
	void Update ()
	{
		// stop the toggle selection
		if (Input.GetKey(KeyCode.Escape))
		{
			if (mc_save != null)
			{
				mc_save.state = false;
				mi_selected_button = -1;
				mc_save = null;				
			}
		}

		Vector2 vMouse = Input.mousePosition;
		
		// There is a problem with Y between screen and MousePosition. 0,0 MousePosition : top left. 0,0 Screen : bottom left.
		vMouse.y = Screen.height - vMouse.y;
		if (!(mr_rect.Contains(vMouse)))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
			{
				if (Physics.Raycast (ray, 100) == false && mc_save != null && mc_save.prefab != null)
				{
					float distance;
					Plane drawPlane = new Plane(Vector3.up, Vector3.zero);
					if (drawPlane.Raycast(ray, out distance) == true)
					{
						GameObject obj = Instantiate(mc_save.prefab, ray.origin + ray.direction * distance, Quaternion.identity) as GameObject;

						obj.name = mc_save.prefab.name;

						if (obj.name == "Start" || obj.name == "Finish")
						{
							if (obj.name == "Finish")
								m_map.end = true;
							else
								m_map.start = true;

							mc_save.state = false;
							mi_selected_button = -1;
							mc_save = null;
						}
						Block block = new Block(obj);
						m_map.m_piste.Add(block);
						ml_Object_map.Add((GameObject)obj);
					}
				}
			}
		}
	}
	
	public static Interface Instance
	{
		get{return m_instance;}
	}
	
	public Rect rect
	{
		get{return mr_rect;}
	}
}

public class Block
{
	private GameObject g_me;
	private GameObject g_prev;
	private GameObject g_next;

	public Block(GameObject mg_defaut)
	{
		g_me = mg_defaut;
		g_prev = g_next = null;
	}	
#region
	public GameObject me
	{
		get {return g_me;}
		set {g_me = value;}
	}
	
	public GameObject prev
	{
		get {return g_prev;}
		set {g_prev = value;}
	}
	
	public GameObject next
	{
		get {return g_next;}
		set {g_next = value;}
	}
	
	#endregion // get/set
}

public class Element
{
	private string s_content;
	private GameObject g_prefab;
	private bool b_state;
	
	public Element(GameObject mg_defaut)
	{
		s_content = "undefined";
		g_prefab = mg_defaut;
	}
	
#region
	
	public string content
	{
		get {return s_content;}
		set {s_content = value;}
	}
	
	public GameObject prefab
	{
		get {return g_prefab;}
		set {g_prefab = value;}
	}
	
	public bool state
	{
		get {return b_state;}
		set {b_state = value;}
	}
	
	#endregion // get/set
}