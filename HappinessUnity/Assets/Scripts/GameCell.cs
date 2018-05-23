using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameCell : BaseCell, IPointerClickHandler
{
	public CellDialog CellDialog;

	int X;
	int Y;

	// Use this for initialization
	void Start () {
		
	}

	public void Setup(int x, int y, float cellWidth, float cellHeight, float largeSize, float smallSize, int topRowCount, int botRowCount, float topRowLeft, float botRowLeft)
	{
		X = x;
		Y = y;

		Create(Y, x * cellWidth, y * cellHeight, cellWidth, cellHeight, largeSize, smallSize, topRowCount, botRowCount, topRowLeft, botRowLeft);
		UpdateIcons(Y, X);
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void Refresh()
	{
		UpdateIcons(Y, X);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		CellDialog.Show(X, Y);
	}
}
