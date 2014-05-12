using UnityEngine;
using System.Collections;

public class SpectatorCamera : MonoBehaviour
{
	public float mfhorizontalSpeed = 2.0F;
    public float mfverticalSpeed = 2.0F;	

	public float mfhorizontalSpeedPos = 0.50F;
    public float mfverticalSpeedPos = 0.50F;	

 	public float mfzoomSpeed = 20;
	public GameObject mgPivotH;
	public GameObject mgPivotV;
	
	void Update()
	{
		float fscroll = Input.GetAxis("Mouse ScrollWheel");
		
		if (fscroll != 0.0f)
			camera.fieldOfView -= fscroll * mfzoomSpeed;

		if(Input.GetMouseButton(0))
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
			
			mgPivotH.transform.position -= new Vector3(fh, 0, fv);
		}		
    }
}