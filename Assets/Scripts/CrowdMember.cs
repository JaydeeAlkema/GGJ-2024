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
		[SerializeField, BoxGroup("References")] private SpriteRenderer spriteRenderer = null;
		[SerializeField, BoxGroup("References")] private List<Sprite> positiveFaces = new();
		[SerializeField, BoxGroup("References")] private List<Sprite> negativeFaces = new();
		[SerializeField, BoxGroup("References")] private List<Sprite> neutralFaces = new();
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

			if (score > 0) spriteRenderer.sprite = positiveFaces[UnityEngine.Random.Range(0, positiveFaces.Count)];
			else if (score < 0) spriteRenderer.sprite = negativeFaces[UnityEngine.Random.Range(0, negativeFaces.Count)];
			else spriteRenderer.sprite = neutralFaces[UnityEngine.Random.Range(0, neutralFaces.Count)];
		}

		public void ResetFaceToNeutral()
		{
			spriteRenderer.sprite = neutralFaces[UnityEngine.Random.Range(0, neutralFaces.Count)];
		}
	}
}