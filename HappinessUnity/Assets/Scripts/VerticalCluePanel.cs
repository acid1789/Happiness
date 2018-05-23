using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LogicMatrix;

public class VerticalCluePanel : MonoBehaviour {

	public GameObject VCPrefab;
	public Sprite NotOverlay;
	public Sprite EitherOrOverlay;

	// Use this for initialization
	void Start () {

		HappinessGameInfo.PuzzleInit();

		foreach (Clue c in HappinessGameInfo.Puzzle.VerticalClues)
		{
			GameObject vc = Instantiate(VCPrefab, transform);

			GameObject top = vc.transform.Find("Top").gameObject;
			GameObject center = vc.transform.Find("Center").gameObject;
			GameObject bottom = vc.transform.Find("Bottom").gameObject;

			int[] iIcons = new int[3];
			int[] iRows = c.GetRows();
			int numIcons = c.GetIcons(HappinessGameInfo.Puzzle, iIcons);

			top.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[0], iIcons[0]);
			center.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[1], iIcons[1]);
			if (numIcons == 3)
				bottom.GetComponent<Image>().sprite = HappinessGameInfo.GetIcon(iRows[2], iIcons[2]);
			else
				bottom.gameObject.SetActive(false);

			switch (c.m_VerticalType)
			{
				case eVerticalType.Two:
				case eVerticalType.Three:
					break;
				case eVerticalType.EitherOr:
					CreateEitherOrOverlay(vc.transform);
					break;
				case eVerticalType.TwoNot:
					Image not = CreateNotOverlay(center, vc.transform);
					not.rectTransform.anchoredPosition = new Vector2(0, 30);
					break;
				case eVerticalType.ThreeTopNot:
					CreateNotOverlay(top, vc.transform);
					break;
				case eVerticalType.ThreeMidNot:
					CreateNotOverlay(center, vc.transform);
					break;
				case eVerticalType.ThreeBotNot:
					CreateNotOverlay(bottom, vc.transform);
					break;
			}
		}

	}

	Image CreateNotOverlay(GameObject template, Transform parent)
	{
		GameObject overlay = Instantiate(template, parent);
		Image img = overlay.GetComponent<Image>();
		img.sprite = NotOverlay;
		overlay.name = "Not Overlay";
		overlay.transform.Rotate(0, 0, 45);
		return img;
	}

	void CreateEitherOrOverlay(Transform parent)
	{
		GameObject overlay = new GameObject("EitherOr", typeof(Image));
		overlay.transform.SetParent(parent);
		Image img = overlay.GetComponent<Image>();
		img.sprite = EitherOrOverlay;
		img.rectTransform.anchoredPosition = new Vector2(0, -30);
		img.rectTransform.sizeDelta = new Vector2(60, 60);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
