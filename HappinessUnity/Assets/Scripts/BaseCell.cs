using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCell : MonoBehaviour {


	protected Image FinalIcon;
	protected List<Image> SmallIcons;
	protected bool GreyOutEliminated;

	protected int Row;
	protected int Col;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected void Create(int row, float xPos, float yPos, float width, float height, float largeSize, float smallSize, int topRowCount, int botRowCount, float topRowLeft, float botRowLeft)
	{
		RectTransform crt = (RectTransform)transform;
		crt.anchoredPosition = new Vector2(xPos, -yPos);
		crt.sizeDelta = new Vector2(width, height);
		crt.localScale = Vector3.one;

		RawImage ri = gameObject.AddComponent<RawImage>();
		ri.color = new Color(0, 0, 0, 0);

		// Add the final icon				
		GameObject finalIcon = new GameObject("Final", typeof(Image));
		finalIcon.transform.SetParent(transform);
		FinalIcon = finalIcon.GetComponent<Image>();
		FinalIcon.raycastTarget = false;
		RectTransform firt = (RectTransform)finalIcon.transform;
		firt.SetAnchorAndPivot(AnchorPresets.MiddleCenter, PivotPresets.MiddleCenter);
		firt.sizeDelta = new Vector2(largeSize, largeSize);
		firt.localScale = Vector3.one;
		finalIcon.SetActive(false);


		// Add the small icons
		SmallIcons = new List<Image>();
		for (int i = 0; i < topRowCount; i++)
		{
			CreateSmallIcon(row, i, smallSize, -topRowLeft + (i * smallSize), smallSize / 2);
		}
		for (int i = 0; i < botRowCount; i++)
		{
			CreateSmallIcon(row, topRowCount + i, smallSize, -botRowLeft + (i * smallSize), -smallSize / 2);
		}
	}

	void CreateSmallIcon(int row, int i, float smallSize, float x, float y)
	{
		GameObject smallIcon = new GameObject("SmallIcon_" + i, typeof(Image));
		smallIcon.transform.SetParent(transform);
		Image ri = smallIcon.GetComponent<Image>();
		//ri.color = new Color(0.125f * i, 0, 0);
		ri.rectTransform.sizeDelta = new Vector2(smallSize, smallSize);
		ri.rectTransform.localScale = Vector3.one;
		ri.rectTransform.anchoredPosition = new Vector2(x, y);
		ri.raycastTarget = false;
		ri.sprite = HappinessGameInfo.GetIcon(row, i);
		ri.raycastTarget = false;
		SmallIcons.Add(ri);
	}

	protected void UpdateIcons(int row, int col)
	{
		Row = row;
		Col = col;
		int iFinal = GetFinalIcon();
		if (iFinal >= 0)
		{
			if (!FinalIcon.gameObject.activeInHierarchy)
			{
				FinalIcon.gameObject.SetActive(true);
				foreach (Image img in SmallIcons)
					img.gameObject.SetActive(false);
			}

			FinalIcon.sprite = HappinessGameInfo.GetIcon(row, iFinal);
		}
		else
		{
			FinalIcon.gameObject.SetActive(false);
			bool[] values = GetSmallIconValues();
			for (int i = 0; i < HappinessGameInfo.PuzzleSize; i++)
			{
				SmallIcons[i].sprite = HappinessGameInfo.GetIcon(Row, i);
				if (GreyOutEliminated)
				{
					SmallIcons[i].gameObject.SetActive(true);
					SmallIcons[i].color = values[i] ? Vector4.one : new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
				}
				else
					SmallIcons[i].gameObject.SetActive(values[i]);
			}
		}
	}

	protected virtual int GetFinalIcon()
	{
		return HappinessGameInfo.Puzzle.m_Rows[Row].m_Cells[Col].m_iFinalIcon;
	}

	protected virtual bool[] GetSmallIconValues()
	{
		return HappinessGameInfo.Puzzle.m_Rows[Row].m_Cells[Col].m_bValues;
	}
}
