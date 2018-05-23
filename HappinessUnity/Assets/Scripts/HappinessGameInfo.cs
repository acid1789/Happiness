using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HappinessGameInfo
{
	public static int PuzzleSize;
	public static LogicMatrix.Puzzle Puzzle;
	public static List<string> IconSets;
	public static ActionHistory History;

	static Sprite[][] PuzzleIcons;

	public static void InitIconSets()
	{
		if( IconSets == null )
		{
			IconSets = new List<string>();

			IconSets.Add("Icons/Balls");
			IconSets.Add("Icons/Deserts");
			IconSets.Add("Icons/Flowers");
			IconSets.Add("Icons/Fruit");
			IconSets.Add("Icons/Hats");
			IconSets.Add("Icons/MInstruments");
			IconSets.Add("Icons/Toys");
			IconSets.Add("Icons/Weapons");
		}
	}

	public static void PuzzleInit(int size = 3, int puzzleNumber = 0)
	{
		InitIconSets();
		if (Puzzle == null)
		{
			History = new ActionHistory();
			PuzzleSize = size;
			Puzzle = new LogicMatrix.Puzzle(puzzleNumber, PuzzleSize, 1);
			
			// Choose icon sets
			string[] puzzleIconSets = new string[PuzzleSize];
			List<string> availableSets = new List<string>(IconSets);
			for (int i = 0; i < PuzzleSize; i++)
			{
				int selection = Random.Range(0, availableSets.Count);
				puzzleIconSets[i] = availableSets[selection];
				availableSets.RemoveAt(selection);
			}

			// Load icon sets
			PuzzleIcons = new Sprite[PuzzleSize][];
			for (int i = 0; i < PuzzleSize; i++)
			{
				PuzzleIcons[i] = Resources.LoadAll<Sprite>(puzzleIconSets[i]);
			}
		}
	}

	public static Sprite GetIcon(int set, int icon)
	{
		return PuzzleIcons[set][icon];
	}
}