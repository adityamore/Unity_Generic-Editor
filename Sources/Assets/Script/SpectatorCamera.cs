using UnityEngine;
using System.Collections;

public class SpectatorCamera : MonoBehaviour
{
	public float mfhorizontalSpeed = 2.0F;
    public float mfverticalSpeed = 2.0F;	

	public float mfhorizontalSpeedPos = 0.50F;
    public float mfverticalSpeedPos = 0.50F;	

 	public float mfzoomSpeed = 20;
	public Camera gizmocam;
	public GameObject mgPivotH;
	public GameObject mgPivotV;
	
	void Update()
	{
		if (Interface.Instance.save_load.activeSelf == false)
		{
			float fscroll = Input.GetAxis("Mouse ScrollWheel");
			
			if (fscroll != 0.0f)
			{
				camera.fieldOfView -= fscroll * mfzoomSpeed;
				gizmocam.fieldOfView = camera.fieldOfView;
			}
			if(Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
			{
		        float fh = mfhorizontalSpeed * Input.GetAxis("Mouse X");
		        float fv = -mfverticalSpeed * Input.GetAxis("Mouse Y");
				
				mgPivotH.transform.Rotate(0,fh,0);
				mgPivotV.transform.Rotate(fv,0,0);
			}
			
			if(Input.GetMouseButton(1))
			{
		        float fh = mfhorizontalSpeedPos * Input.GetAxis("Mouse X");
		        float fv = mfverticalSpeedPos * Input.GetAxis("Mouse Y");
				
				
				Vector3 mouse = new Vector3(fh, 0, fv);
				mouse = mgPivotH.transform.TransformDirection(mouse);
				mgPivotH.transform.position += mouse;
			}
		}
    }
}