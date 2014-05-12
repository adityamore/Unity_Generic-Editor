using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

public class InterfaceRight : MonoBehaviour 
{
	public 	string msstringToEdit = "Map name";
	public 	TextAsset moxmlfile;
	private enum button_type{button = 0, toogle = 1, undefined = 2};
	
#region
	private bool mbelements = false;
	private bool mbelementsmap = false;
	private Dictionary<string, int> tag;
	private IList<Enum> gui_type = new List<Enum>{button_type.button, button_type.toogle, button_type.undefined};

	#endregion // private variable
	
	void Start()
	{
/*		string[] namedConstants = Enum.GetNames(typeof(button_type));
			foreach (string constant in namedConstants)
			Debug.Log("ENUM = " + constant);
*/    			
		tag = new Dictionary<string, int>();
		
		using (XmlReader reader = XmlReader.Create(new StringReader(moxmlfile.text)))
		{
			while (reader.Read()) // Read each node
			{
				if (reader.IsStartElement()) // Read next begin balise, or empty balise
				{
					if (reader.IsEmptyElement)
						Debug.Log("<{0}> " + reader.Name);
					else
					{
						Debug.Log("<{0}> " + reader.Name);
						if (reader.HasAttributes)
						{
							Debug.Log("Attribute of < " + reader.Name + ">");
							while (reader.MoveToNextAttribute())
							{
								Debug.Log("  {0}={1}" + reader.Name + " " + reader.Value);
							}

							if (Enum.IsDefined(typeof(button_type),reader.Value) == true)
								tag.Add(reader.ReadString(), (int)Enum.Parse(typeof(button_type), reader.Value));
							else
								tag.Add("undefined", 2);
							Debug.Log(tag.Values);
						}
//						reader.Read(); // Read the start tag.
//						if (reader.IsStartElement())  // Handle nested elements.
//							Debug.Log("\r\n<{0}> " + reader.Name);

//						Debug.Log(reader.ReadString());  //Read the text content of the element.
					}
				}
			} 
		}
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(Screen.width/1.15F, Screen.height/8F, Screen.width/8F, Screen.height/1.3F), "");
		msstringToEdit = GUI.TextArea (new Rect (Screen.width/1.14F, Screen.height/7.5F, Screen.width/9F, Screen.height/15F), msstringToEdit, 100);

		mbelements = GUI.Toggle(new Rect(Screen.width/1.14F,Screen.height/4.8F,Screen.width/9F,Screen.height/15F), mbelements, "Elements");
		mbelementsmap = GUI.Toggle(new Rect(Screen.width/2F,0,Screen.width/9F,Screen.height/15F), mbelementsmap, "Elements on map");

		if (mbelements == true)
			Debug.Log("Le bouton elements est ouvert");
		
		if (mbelementsmap == true)
			Debug.Log("Le bouton elementsmap est ouvert");

	}
	
	void Update ()
	{
		//			Debug.Log( strValue );
	}
}
