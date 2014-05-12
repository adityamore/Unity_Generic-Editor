using UnityEngine;
using System.Collections;

public class Focus : MonoBehaviour {
	
	
	public GameObject pivotH;
	public Camera gizmocam;

	private GameObject selectObject;
	private bool move_pivot;
	private float progression;
	private Vector3 save_vector;
	private float save_fov;
	private MoveObject my_move_object;
	
	void Start ()
	{
		my_move_object = GetComponent<MoveObject>();
		move_pivot = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Interface.Instance.save_load.activeSelf == false)
		{
			if (Input.GetKeyDown(KeyCode.F) && my_move_object.selectObject != null)
			{
				move_pivot = true;
				save_vector = pivotH.transform.position;
				save_fov = camera.fieldOfView;
				progression = 0.0f;
			}
			if (move_pivot == true)
			{
				pivotH.transform.position = Vector3.Lerp(save_vector, my_move_object.selectObject.transform.position, progression);
				camera.fieldOfView = Mathf.Lerp(save_fov, 15.0f, progression);
	
				gizmocam.fieldOfView = camera.fieldOfView;
				progression += Time.deltaTime;
				
				if (progression >= 1)
					move_pivot = false;
			}
		}
	}
}