using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellDialogCell : BaseCell {

	public GamePanel GP;
	public GameObject CellDialog;
	public GameObject SelectionHighlight;
	public Button AcceptButton;
	public Button ResetButton;
	public Button EliminateButton;
	public Button ConfirmButton;

	GameObject Highlight;
	CDCSmallIcon CurrentSelection;
	int FinalIconIndex;
	bool[] IconState;

	// Use this for initialization
	void Start() {
	}

	public void Init()
	{		
		float smallSize = 200;
		float largeSize = 400;
		int topRowCount = HappinessGameInfo.PuzzleSize >> 1;
		int botRowCount = HappinessGameInfo.PuzzleSize - topRowCount;

		// Make sure bottom row is the smaller of the two
		while (topRowCount < botRowCount)
		{
			topRowCount++;
			botRowCount--;
		}
		int halfTopRow = topRowCount >> 1;
		float topRowLeft = (smallSize * halfTopRow);
		if ((topRowCount & 1) == 0)
			topRowLeft -= (smallSize / 2);
		int halfBotRow = botRowCount >> 1;
		float botRowLeft = (smallSize * halfBotRow);
		if ((botRowCount & 1) == 0)
			botRowLeft -= (smallSize / 2);

		Create(0, 0, 0, smallSize * topRowCount, largeSize, largeSize, smallSize, topRowCount, botRowCount, topRowLeft, botRowLeft);

		GreyOutEliminated = true;
		for( int i = 0; i < SmallIcons.Count; i++ )
		{
			Image img = SmallIcons[i];
			img.raycastTarget = true;
			CDCSmallIcon si = img.gameObject.AddComponent<CDCSmallIcon>();
			si.CDC = this;
			si.Index = i;
		}
		Highlight = null;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override int GetFinalIcon()
	{
		return FinalIconIndex;
	}

	protected override bool[] GetSmallIconValues()
	{
		return IconState;
	}

	public void SelectSmallIcon(CDCSmallIcon icon)
	{
		CurrentSelection = icon;
		if (icon == null)
		{
			if (Highlight != null)
				Highlight.SetActive(false);
			EliminateButton.GetComponentInChildren<Text>().text = "Eliminate Selection";
		}
		else
		{
			if (Highlight == null)
			{
				Highlight = Instantiate(SelectionHighlight, icon.transform);
			}
			else
				Highlight.transform.SetParent(icon.transform);

			RectTransform rt = Highlight.GetComponent<RectTransform>();
			rt.offsetMin = rt.offsetMax = Vector2.zero;

			Highlight.SetActive(true);
			EliminateButton.GetComponentInChildren<Text>().text = IconState[CurrentSelection.Index] ? "Eliminate Selection" : "Restore Selection";
		}
		SetupButtons();
	}

	public void SetCell(int x, int y)
	{
		FinalIconIndex = HappinessGameInfo.Puzzle.m_Rows[y].m_Cells[x].m_iFinalIcon;
		IconState = new bool[HappinessGameInfo.PuzzleSize];
		for( int i = 0; i < IconState.Length; i++ )
			IconState[i] = HappinessGameInfo.Puzzle.m_Rows[y].m_Cells[x].m_bValues[i];
		UpdateIcons(y, x);
		SelectSmallIcon(null);
	}

	void SetupButtons()
	{
		AcceptButton.interactable = false;
		EliminateButton.interactable = CurrentSelection != null;
		ConfirmButton.interactable = CurrentSelection != null;
		ResetButton.interactable = false;

		for( int i = 0; i < IconState.Length; i++ )
		{
			if (!IconState[i])
				ResetButton.interactable = true;
			if (IconState[i] != HappinessGameInfo.Puzzle.m_Rows[Row].m_Cells[Col].m_bValues[i])
				AcceptButton.interactable = true;
		}
	}

	void Close()
	{
		CellDialog.SetActive(false);
	}

	public void CancelCick()
	{
		Close();
	}

	public void AcceptClick()
	{
		// Commit all changes to the actual puzzle
		for (int i = 0; i < IconState.Length; i++)
		{
			if (HappinessGameInfo.Puzzle.m_Rows[Row].m_Cells[Col].m_bValues[i] != IconState[i])
			{
				if (IconState[i])
					HappinessGameInfo.History.DoAction(eActionType.eAT_RestoreIcon, Row, Col, i);
				else
					HappinessGameInfo.History.DoAction(eActionType.eAT_EliminateIcon, Row, Col, i);
			}
		}
		if (FinalIconIndex >= 0 && FinalIconIndex != HappinessGameInfo.Puzzle.m_Rows[Row].m_Cells[Col].m_iFinalIcon)
			HappinessGameInfo.History.DoAction(eActionType.eAT_SetFinalIcon, Row, Col, FinalIconIndex);
		Close();
		GP.RefreshRow(Row);
	}

	public void EliminateClick()
	{
		IconState[CurrentSelection.Index] = !IconState[CurrentSelection.Index];
		UpdateIcons(Row, Col);
		SelectSmallIcon(CurrentSelection);
	}

	public void ConfirmClick()
	{
		FinalIconIndex = CurrentSelection.Index;
		for (int i = 0; i < IconState.Length; i++)
			IconState[i] = i == CurrentSelection.Index;
		UpdateIcons(Row, Col);
		SelectSmallIcon(null);
	}

	public void ResetClick()
	{
		FinalIconIndex = -1;
		for (int i = 0; i < IconState.Length; i++)
			IconState[i] = true;
		UpdateIcons(Row, Col);
		SelectSmallIcon(null);
	}
}
