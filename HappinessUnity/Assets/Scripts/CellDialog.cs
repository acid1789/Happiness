using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellDialog : MonoBehaviour {

	public CellDialogCell Cell;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init()
	{
		Cell.Init();
	}

	public void Show(int x, int y)
	{
		gameObject.SetActive(true);
		Cell.SetCell(x, y);
	}
}
