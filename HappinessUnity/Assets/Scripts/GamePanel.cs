using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LogicMatrix;

public class GamePanel : MonoBehaviour {

	public GameObject FramePrefab;
	public CellDialog CellDialog;

	GameCell[,] Cells;

	void Start ()
	{
		HappinessGameInfo.PuzzleInit();

		RectTransform rt = (RectTransform)transform;

		float cellWidth = rt.rect.width / HappinessGameInfo.PuzzleSize;
		float cellHeight = rt.rect.height / HappinessGameInfo.PuzzleSize;
		float largeSize = Mathf.Min(cellHeight, cellWidth);

		int topRowCount = HappinessGameInfo.PuzzleSize >> 1;
		int botRowCount = HappinessGameInfo.PuzzleSize - topRowCount;

		// Make sure bottom row is the smaller of the two
		while (topRowCount < botRowCount)
		{
			topRowCount++;
			botRowCount--;
		}
		float smallSize = Mathf.Min(cellWidth / topRowCount, cellHeight / 2);
		int halfTopRow = topRowCount >> 1;
		float topRowLeft = (smallSize * halfTopRow);
		if ((topRowCount & 1) == 0)
			topRowLeft -= (smallSize / 2);
		int halfBotRow = botRowCount >> 1;
		float botRowLeft = (smallSize * halfBotRow);
		if ((botRowCount & 1) == 0)
			botRowLeft -= (smallSize / 2);


		Cells = new GameCell[HappinessGameInfo.PuzzleSize, HappinessGameInfo.PuzzleSize];
		for (int y = 0; y < HappinessGameInfo.PuzzleSize; y++)
		{
			for (int x = 0; x < HappinessGameInfo.PuzzleSize; x++)
			{
				// Create the cell
				string cellName = string.Format("Cell_{0}_{1}", x, y);
				GameCell gc = new GameObject(cellName, typeof(RectTransform), typeof(GameCell)).GetComponent<GameCell>();
				gc.transform.SetParent(transform);
				gc.GetComponent<RectTransform>().SetAnchorAndPivot(AnchorPresets.TopLeft, PivotPresets.TopLeft);
				gc.Setup(x, y, cellWidth, cellHeight, largeSize, smallSize, topRowCount, botRowCount, topRowLeft, botRowLeft);

				// Add the frame
				Instantiate(FramePrefab, gc.transform, false);
				Cells[x, y] = gc;
				gc.CellDialog = CellDialog;
			}
		}

		CellDialog.Init();
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void RefreshRow(int row)
	{
		for (int i = 0; i < HappinessGameInfo.PuzzleSize; i++)
		{
			Cells[i, row].Refresh();
		}
	}
}
