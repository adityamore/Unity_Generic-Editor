using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	private bool m_start;
	private bool m_end;
	private bool m_complete;
	
	public List<Block> m_piste;
	public List<GameObject> m_other;
	
	public static Map map_instance;
	
	void Awake()
	{
		map_instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		start = end = complete = false;

		m_piste = new List<Block>();
		m_other = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		complete = true;

		if (m_start == false || m_end == false)
		{
			Debug.LogError("Please insert a Start and/or End block");
			complete = false;
		}
		foreach (Block block in m_piste)
		{
			Debug.Log(block.me.name);
			if (block.me.name != "Finish" && block.next == null)
			{
				Debug.LogError("Please link a next block to " + block.me.name);
				complete = false;
			}
			if (block.me.name != "Start" && block.prev == null)
			{
				Debug.LogError("Please link a previous block to " + block.me.name);
				complete = false;
			}
		}
	}
	
	public static Map Instance
	{
		get {return map_instance;}
	}
	
	public bool start
	{
		get {return m_start;}
		set {m_start = value;}
	}
	
	public bool end
	{
		get {return m_end;}
		set {m_end = value;}
	}
	public bool complete
	{
		get {return m_complete;}
		set {m_complete = value;}
	}
}
