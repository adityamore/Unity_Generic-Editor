using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {

	public List<Grid> grille;
	
	void Awake()
	{
		
	}
	
	void Start () 
	{

	}
	
	void Update () 
	{
	
	}
}

public class Grid
{
	public List<GameObject> cube;
}