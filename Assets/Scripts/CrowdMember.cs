using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class CrowdMember : MonoBehaviour
	{
		[SerializeField, BoxGroup("References")] private List<PlayerActionType> positives = new();
		[SerializeField, BoxGroup("References")] private List<PlayerActionType> negatives = new();
		[Space]
		[SerializeField, BoxGroup("Runtime")] private int score = 0;
		[Space]
		[SerializeField, BoxGroup("Debugging")] private bool printScores = false;

		public int GetScore() => score;
		public void SetScore(int value) => score = value;

		public void CheckPossitivesAndNegatives(List<PlayerActionType> actions)
		{
			foreach (PlayerActionType action in actions)
			{
				if (positives.Contains(action))
				{
					score++;
				}
				else if (negatives.Contains(action))
				{
					score--;
				}
			}
			if (printScores) Debug.Log($"Checking positives and negatives at {gameObject.name} - Score: {score}", this);
		}
	}
}