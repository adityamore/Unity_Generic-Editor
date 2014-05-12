using UnityEngine;
using System.Collections;

public class ShowTransform : MonoBehaviour
{
	private Rect mr_rect;
	public GUISkin m_window;
	private MoveObject m_move_object;
	private string position_x;
	private string position_y;
	private string position_z;
	
	private string rotation_x;
	private string rotation_y;
	private string rotation_z;

	void Start () 
	{
		mr_rect = new Rect(Screen.width/1.22F, Screen.height/10F, Screen.width/7F, Screen.height/7F);	

		position_x = "";
		position_y = "";
		position_z = "";
		
		rotation_x = "";
		rotation_y = "";
		rotation_z = "";
		
		m_move_object = MoveObject.Instance;
	}
	
	void OnGUI()
	{
		if (m_move_object.selectObject != null)
		{
			GUI.enabled = true;
			position_x = m_move_object.selectObject.transform.position.x.ToString();
			position_y = m_move_object.selectObject.transform.position.y.ToString();
			position_z = m_move_object.selectObject.transform.position.z.ToString();

			rotation_x = m_move_object.selectObject.transform.eulerAngles.x.ToString();
			rotation_y = m_move_object.selectObject.transform.eulerAngles.y.ToString();
			rotation_z = m_move_object.selectObject.transform.eulerAngles.z.ToString();
		}
		else
			GUI.enabled = false;
		
		GUILayout.BeginArea (mr_rect, m_window.FindStyle("Box"));
		Vector3 tmp_pos;
		Vector3 tmp_rot;
		
		// Area Name
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
	    GUILayout.Label("Transform");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Position
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
	    GUILayout.Label("Position");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("x");
		position_x = GUILayout.TextField(position_x, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.Label("y");
		position_y = GUILayout.TextField(position_y, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.Label("z");
		position_z = GUILayout.TextField(position_z, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// Rotation
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
	    GUILayout.Label("Rotation");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("x");
		rotation_x = GUILayout.TextField(rotation_x, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.Label("y");
		rotation_y = GUILayout.TextField(rotation_y, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.Label("z");
		rotation_z = GUILayout.TextField(rotation_z, GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		
		float.TryParse(position_x, out tmp_pos.x);
		float.TryParse(position_y, out tmp_pos.y);
		float.TryParse(position_z, out tmp_pos.z);

		float.TryParse(rotation_x, out tmp_rot.x);
		float.TryParse(rotation_y, out tmp_rot.y);
		float.TryParse(rotation_z, out tmp_rot.z);
		
		if (GUI.enabled == true)
		{
			m_move_object.selectObject.transform.position = tmp_pos;
			m_move_object.selectObject.transform.eulerAngles = tmp_rot;
			m_move_object.Gizmo.transform.position = tmp_pos;
			m_move_object.Gizmo.transform.eulerAngles = tmp_rot;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
