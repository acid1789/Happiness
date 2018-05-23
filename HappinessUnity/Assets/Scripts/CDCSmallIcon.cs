using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CDCSmallIcon : MonoBehaviour, IPointerClickHandler
{
	public CellDialogCell CDC;
	public int Index;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		CDC.SelectSmallIcon(this);
	}
}
