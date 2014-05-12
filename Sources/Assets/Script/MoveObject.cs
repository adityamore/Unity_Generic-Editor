using UnityEngine;
using System.Collections;

public class MoveObject: MonoBehaviour 
{
	public GameObject Gizmo;
	public GameObject selectObject;
	public GUISkin m_window;
	public Texture2D mt_rotation;
	public Texture2D mt_move;
	// script
	
	private bool b_arrow;
	private bool b_wheel;
	private bool ctrl;
	private bool shift;
	private static MoveObject mInstance;
	
	private Material[] save;
	private GameObject current;
	private Interface m_interface;
	private gizmo_state state;
	private Vector3 gizmo_pos;
	private Vector3 mouse_pos;
	private Vector3 arrow_pos;
	private Vector3 direction;
	private Material[] texture_gizmo;
	private GameObject selectObject_gizmo;
	
	private int layermask;
	private const float MOVE_FACTOR = 1.0f;
	private const float SPACE_MAX = 1.5f;
	private enum gizmo_state
	{
		defaut = 0,
		arrowX = 1,
		arrowY = 2,
		arrowZ = 3,
		wheelX = 4,
		wheelY = 5,
		wheelZ = 6,
	}
	
	void Awake()
	{
		mInstance = this;		
	}
	
	void Start () 
	{
		Gizmo.SetActive(false);
		state = gizmo_state.defaut;
		m_interface = Interface.Instance;
		selectObject = null;
		current = null;
		ctrl = false;
		b_arrow = false;
		b_wheel = false;
		layermask = 1 << LayerMask.NameToLayer("Snap");
		layermask = ~layermask;
	}

	// change the texture
	void switch_texture(out Material[] saveT, GameObject selectO)
	{
		// save texture to re-apply it when another object is selected
		saveT = selectO.renderer.materials;
		
		// copy the material array with length + 1 for "grid" texture
		Material[] mat = new Material[selectO.renderer.materials.Length + 1];
		int i = -1;
		while (++i < selectO.renderer.materials.Length)
			mat[i] = selectO.renderer.materials[i];

		mat[i] = Resources.Load("Grille", typeof(Material)) as Material;
		
		// apply the new texture with the grid
		selectO.renderer.materials = mat;		
	}
	
	// Give a value to gizmo state
	void affect_state(GameObject current)
	{
		switch (current.tag)
		{
		case "ArrowX":
			state = gizmo_state.arrowX;
			break;
		case "ArrowY":
			state = gizmo_state.arrowY;
			break;
		case "ArrowZ":
			state = gizmo_state.arrowZ;
			break;
		case "WheelX":
			state = gizmo_state.wheelX;
			break;
		case "WheelY":
			state = gizmo_state.wheelY;
			break;
		case "WheelZ":
			state = gizmo_state.wheelZ;
			break;
		default:
			state = gizmo_state.defaut;
			break;
		}		
	}
	
	// Show Arrow or Circle of Gizmo
	void aff_wheel_or_arrow(Event e, Rect r, Rect re)
	{
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.W && e.isKey)
		{
			b_arrow = true;
			b_wheel = false;
		}

		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.E && e.isKey)
		{
			b_arrow = false;
			b_wheel = true;
		}
		
		b_arrow = GUI.Toggle(r, b_arrow, mt_move, m_window.FindStyle("Button"));
			
		b_wheel = (b_arrow == false ? true : false);

		b_wheel = GUI.Toggle(re, b_wheel, mt_rotation, m_window.FindStyle("Button"));

		b_arrow = (b_wheel == false ? true : false);
		
		foreach (Transform child in Gizmo.transform)
		{
			if (b_arrow == true)
			{
				if (child.tag.Contains("Arrow"))
					child.gameObject.SetActive(true);
				else
					child.gameObject.SetActive(false);
			}
			else
			{
				if (child.tag.Contains("Wheel"))
					child.gameObject.SetActive(true);
				else
					child.gameObject.SetActive(false);					
			}
		}		
	}
	
	void OnGUI()
	{
		Event e = Event.current;
		GUI.depth = 0;
		Rect r = new Rect(Screen.width /2f, Screen.height/50f, 30,30);
		Rect re = new Rect(Screen.width /1.9f, Screen.height/50f, 30,30);
		Rect rst = new Rect(Screen.width/1.22F, Screen.height/10F, Screen.width/7F, Screen.height/7F);
		Vector2 vMouse = Input.mousePosition;
		
		vMouse.y = Screen.height - vMouse.y;
		
		if (m_interface.save_load.activeSelf == false)
		{
			if (!(r.Contains(vMouse)) && !(re.Contains(vMouse)) && !(rst.Contains(vMouse)))
			{
				if (e.type == EventType.MouseDown && e.button == 0 && e.isMouse && e.keyCode != KeyCode.LeftAlt)
					raycast();
			}
			aff_wheel_or_arrow(e, r, re);
		}
	}
	
	// init some variable if ray touch the gizmo
	void ray_touch_gizmo(GameObject current)
	{
		// apply the original texture
		if (selectObject_gizmo != null)
			selectObject_gizmo.renderer.materials = texture_gizmo;
		selectObject_gizmo = current;
		
		gizmo_pos = Camera.main.WorldToScreenPoint(Gizmo.transform.position);
		affect_state(selectObject_gizmo);
		
		if (state >= gizmo_state.arrowX && state <= gizmo_state.arrowZ)
		{
			arrow_pos = Camera.main.WorldToScreenPoint(selectObject_gizmo.transform.position);
			direction = arrow_pos - gizmo_pos;
			direction /= direction.sqrMagnitude;
		}
		else if (state >= gizmo_state.wheelX && state <= gizmo_state.wheelZ)
		{
			arrow_pos = Camera.main.WorldToScreenPoint(selectObject_gizmo.transform.position);
		}
		
		mouse_pos = Input.mousePosition;
		switch_texture(out texture_gizmo, selectObject_gizmo);
	}
	
	// touch object, show gizmo
	void ray_touch_another_object(GameObject current)
	{
		// apply the original texture
		if (selectObject != null)
			selectObject.renderer.materials = save;
		
		if (shift == true)
		{
			if (selectObject != null)
			{	
				
				Transform child;
				Vector3 angles;
				
				angles.x = 0;
				angles.y = 0;
				angles.z = 0;
				
				child = null;
				
				selectObject.transform.eulerAngles = angles;
				foreach (Transform childtmp in selectObject.transform)
					if(childtmp.name.Contains("Link"))
						child = childtmp;
				if (child != null)
				{
					if (child.name.Contains("9") || child.name.Contains("8"))
					{
						angles.x = selectObject.transform.eulerAngles.x + current.transform.eulerAngles.x;
						if (child.name.Contains("9"))
							angles.y = selectObject.transform.eulerAngles.y + current.transform.eulerAngles.y + 270;
						else
							angles.y = selectObject.transform.eulerAngles.y + current.transform.eulerAngles.y + 90;

						angles.z = selectObject.transform.eulerAngles.z + current.transform.eulerAngles.z;
						
						foreach (Transform c in current.transform)
						{
							if (c.name.Contains("Link") && c.name.Contains("9") && child.name.Contains("9"))
								angles.y = selectObject.transform.eulerAngles.y + current.transform.eulerAngles.y - 90;

							if (c.name.Contains("Link") && c.name.Contains("8") && child.name.Contains("8"))
								angles.y = selectObject.transform.eulerAngles.y + current.transform.eulerAngles.y + 90;
						}
					}
					else
					{
						angles.x = selectObject.transform.eulerAngles.x + current.transform.eulerAngles.x;
						angles.y = selectObject.transform.eulerAngles.y + current.transform.eulerAngles.y;
						angles.z = selectObject.transform.eulerAngles.z + current.transform.eulerAngles.z;						
					}
					selectObject.transform.eulerAngles = angles;

					Vector3 tmp = current.transform.position - child.transform.position;				
					selectObject.transform.position += tmp;
				}
			}
		}
		selectObject = current;
		
		// place the Gizmo in the current object position
		Gizmo.transform.position = current.transform.position;
		Gizmo.transform.rotation = current.transform.rotation;
		if (selectObject_gizmo != null)
			selectObject_gizmo.renderer.materials = texture_gizmo;
		Gizmo.SetActive(true);

		switch_texture(out save, selectObject);
	}
	
	// cast a ray to find gizmo or gameobjet
	void raycast()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Vector2 vMouse = Input.mousePosition;
		vMouse.y = Screen.height - vMouse.y;
		
		// if the ray touch the Gizmo or object
			if (Gizmo.activeSelf == true && Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Gizmo")))
			{
				current = hit.collider.gameObject;
				ray_touch_gizmo(current);
			}
			else if (Physics.Raycast (ray, out hit, 100, layermask))
			{
				current = hit.collider.gameObject;
				if (current != selectObject)
					ray_touch_another_object(current);
			}
			else if (selectObject != null)
			{
				selectObject.renderer.materials = save;
				if (selectObject_gizmo != null)
					selectObject_gizmo.renderer.materials = texture_gizmo;
				Gizmo.SetActive(false);
				selectObject = null;
				selectObject_gizmo = null;
			}
	}
	
	// move object and Gizmo
	void change_object_position()
	{
		Vector3 mouse_movement = Input.mousePosition - mouse_pos;
		float move_value = Vector3.Dot(mouse_movement, direction) * MOVE_FACTOR;
		
		if (ctrl == false)
		{
			Gizmo.transform.position += move_value * (current.transform.position - Gizmo.transform.position);
			selectObject.transform.position += move_value * (current.transform.position - Gizmo.transform.position);
		}
		else
		{
			Gizmo.transform.position += move_value * (current.transform.position - Gizmo.transform.position);
			Vector3 tmp;
			tmp = selectObject.transform.position;
			tmp.x = Mathf.Round(Gizmo.transform.position.x);
			tmp.y = Mathf.Round(Gizmo.transform.position.y);
			tmp.z = Mathf.Round(Gizmo.transform.position.z);
			selectObject.transform.position = tmp;
		}
		mouse_pos = Input.mousePosition;
		
		float distance = Vector3.Distance(Gizmo.transform.position, selectObject.transform.position);
		
		if (distance >= SPACE_MAX)
			selectObject.transform.position = Gizmo.transform.position; 
	}
	
	// rotate object and Gizmo
	void change_object_rotation()
	{
		Vector3 mouse_movement = Input.mousePosition - mouse_pos;
		float move = mouse_movement.x + mouse_movement.y;
		
		if (state == gizmo_state.wheelX)
		{
			Gizmo.transform.Rotate(-move, 0, 0);
			
			if (ctrl == false)
				selectObject.transform.Rotate(-move, 0, 0);
		}
		else if (state == gizmo_state.wheelY)
		{
			Gizmo.transform.Rotate(0, -move, 0);
	
			if (ctrl == false)
				selectObject.transform.Rotate(0, -move, 0);
		}
		else if (state == gizmo_state.wheelZ)
		{
			Gizmo.transform.Rotate(0, 0, -move);
			
			if (ctrl == false)
				selectObject.transform.Rotate(0, 0, -move);
		}	
		
		if (ctrl == true)
		{
			Vector3 tmp;

			tmp = selectObject.transform.eulerAngles;
			
			if (state == gizmo_state.wheelX)
				tmp.x = Mathf.Round(Gizmo.transform.eulerAngles.x);

			if (state == gizmo_state.wheelY)
				tmp.y = Mathf.Round(Gizmo.transform.eulerAngles.y);
				
			if (state == gizmo_state.wheelZ)
				tmp.z = Mathf.Round(Gizmo.transform.eulerAngles.z);		
			
			tmp.x = Mathf.Round(Gizmo.transform.eulerAngles.x / 30.0f ) * 30.0f;
			tmp.y = Mathf.Round(Gizmo.transform.eulerAngles.y / 30.0f ) * 30.0f;
			tmp.z = Mathf.Round(Gizmo.transform.eulerAngles.z / 30.0f ) * 30.0f;

			selectObject.transform.eulerAngles = tmp;
		}
		mouse_pos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		// if button is up, the status of gizmo is "defaut" and position is the position of object
		if (Input.GetMouseButtonUp(0))
		{
			if (selectObject != null)
			{
				Gizmo.transform.position = selectObject.transform.position;
				Gizmo.transform.rotation = selectObject.transform.rotation;
			}
			state = gizmo_state.defaut;		
		}
		
		if (Input.GetKeyDown(KeyCode.LeftControl))
			ctrl = true;
		
		if (Input.GetKeyUp(KeyCode.LeftControl))
			ctrl = false;
		
		if (Input.GetKeyDown(KeyCode.LeftShift))
			shift = true;
		
		if (Input.GetKeyUp(KeyCode.LeftShift))
			shift = false;
		
		
		
		// move object and gizmo
		if (state >= gizmo_state.arrowX && state <= gizmo_state.arrowZ)
			change_object_position();
		else if (state >= gizmo_state.wheelX && state <= gizmo_state.wheelZ)
			change_object_rotation();
	}
	
	// singleton
	public static MoveObject Instance
	{
		get {return mInstance;}	
	}
}