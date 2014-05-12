using UnityEngine;
using System.Collections;

public class Snap : MonoBehaviour {

	private MoveObject obj;
	
	void Start ()
	{ 
		obj = MoveObject.Instance;
	}
	
	void OnCollisionStay(Collision theCollision)
	{
		if (obj.selectObject != null && this.transform.parent.position == obj.selectObject.transform.position)
		{
			if (LayerMask.LayerToName(theCollision.gameObject.layer) == "Snap")
			{
				Vector3 direction = theCollision.gameObject.transform.position - this.transform.position;
				obj.selectObject.transform.position += direction;
			}
		}
	}

	void Update () 
	{
	}
}
