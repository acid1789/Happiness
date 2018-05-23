using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LogicMatrix;

public class HorizontalCluePanel : MonoBehaviour {

	public GameObject HCPrefab;
	public Sprite NextTo;
	public Sprite NotOverlay;
	public Sprite Span;

	// Use this for initialization
	void Start () {
		HappinessGameInfo.PuzzleInit();

		foreach (Clue c in HappinessGameInfo.Puzzle.HorizontalClues)
		{
			GameObject hc = Instantiate(HCPrefab, transform);

			GameObject left = hc.transform.Find("Left").gameObject;
			GameObject center = hc.transform.Find("Center").gameObject;
			GameObject right = hc.transform.Find("Right").gameObject;

			int[] iIcons = new int[3];
			int[] iRows = c.GetRows();
			int iNumIcons = c.GetIcons(HappinessGameInfo.Puzzle, iIcons);

			left.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[0], iIcons[0]);

			if (c.m_HorizontalType == eHorizontalType.LeftOf || c.m_HorizontalType == eHorizontalType.NotLeftOf)
			{
				center.GetComponent<Image>().sprite = NextTo;
				right.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[1], iIcons[1]);

				if (c.m_HorizontalType == eHorizontalType.NotLeftOf)
					CreateNotOverlay(center, hc.transform);
			}
			else
			{
				center.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[1], iIcons[1]);
				if (iNumIcons == 3)
				{
					right.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[2], iIcons[2]);

					switch (c.m_HorizontalType)
					{
						case eHorizontalType.NextTo:
						case eHorizontalType.LeftOf:
						case eHorizontalType.NotLeftOf:
							break;
						case eHorizontalType.NotNextTo:
							CreateNotOverlay(center, hc.transform);
							break;
						case eHorizontalType.Span:
							CreateSpanOverlay(hc.transform);
							break;
						case eHorizontalType.SpanNotLeft:
							CreateSpanOverlay(hc.transform);
							CreateNotOverlay(left, hc.transform);
							break;
						case eHorizontalType.SpanNotMid:
							CreateSpanOverlay(hc.transform);
							CreateNotOverlay(center, hc.transform);
							break;
						case eHorizontalType.SpanNotRight:
							CreateSpanOverlay(hc.transform);
							CreateNotOverlay(right, hc.transform);
							break;
					}
				}
				else
					right.gameObject.SetActive(false);
			}
		}
	}

	void CreateNotOverlay(GameObject clone, Transform parent)
	{
		GameObject overlay = Instantiate(clone, parent);
		Image img = overlay.GetComponent<Image>();
		img.sprite = NotOverlay;
		overlay.name = "Not Overlay";
		overlay.transform.Rotate(0, 0, 45);
	}

	void CreateSpanOverlay(Transform parent)
	{
		GameObject overlay = new GameObject("Span", typeof(Image));
		overlay.transform.SetParent(parent, false);
		Image img = overlay.GetComponent<Image>();
		img.sprite = Span;
		img.raycastTarget = false;
		img.rectTransform.SetAnchorAndPivot(AnchorPresets.StretchAll, PivotPresets.MiddleCenter);
		img.rectTransform.offsetMin = img.rectTransform.offsetMax = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
