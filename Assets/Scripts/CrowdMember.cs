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
		[SerializeField, BoxGroup("References")] private GameObject positiveArrowGameObject = null;
		[SerializeField, BoxGroup("References")] private GameObject neutralArrowGameObject = null;
		[SerializeField, BoxGroup("References")] private GameObject negativeArrowGameObject = null;
		[Space]
		[SerializeField, BoxGroup("References")] private GameObject happyFaceIcon = null;
		[SerializeField, BoxGroup("References")] private GameObject neutralFaceIcon = null;
		[SerializeField, BoxGroup("References")] private GameObject cringeFaceIcon = null;
		[Space]
		[SerializeField, BoxGroup("Runtime")] private int defaultScore = 0;
		[SerializeField, BoxGroup("Runtime")] private int currentScore = 0;
		[Space]
		[SerializeField, BoxGroup("Debugging")] private bool printScores = false;

		public int GetScore() => currentScore;
		public int GetDefaultScore() => defaultScore;
		public void SetScore(int value) => currentScore = value;

		void OnEnable()
		{
			currentScore = defaultScore;
		}

		public void CheckPossitivesAndNegatives(List<PlayerActionType> actions)
		{
			ResetMoodIndicators();
			ResetEmoticons();
			int oldScore = currentScore;
			foreach (PlayerActionType action in actions)
			{
				if (positives.Contains(action))
				{
					if (currentScore != 1)
					{
						currentScore++;
						positiveArrowGameObject.SetActive(true);
						neutralArrowGameObject.SetActive(false);
						negativeArrowGameObject.SetActive(false);
					}
				}
				else if (negatives.Contains(action))
				{
					if (currentScore != -1)
					{
						currentScore--;
						positiveArrowGameObject.SetActive(false);
						neutralArrowGameObject.SetActive(false);
						negativeArrowGameObject.SetActive(true);
					}
				}
			}

			if (oldScore == currentScore) neutralArrowGameObject.SetActive(true);

			if (printScores) Debug.Log($"Checking positives and negatives at {gameObject.name} - Score: {currentScore}", this);

			SetFacesToCurrentScore();
		}

		public void SetFacesToCurrentScore()
		{
			if (currentScore > 0) spriteRenderer.sprite = positiveFaces[UnityEngine.Random.Range(0, positiveFaces.Count)];
			else if (currentScore < 0) spriteRenderer.sprite = negativeFaces[UnityEngine.Random.Range(0, negativeFaces.Count)];
			else spriteRenderer.sprite = neutralFaces[UnityEngine.Random.Range(0, neutralFaces.Count)];
		}
		public void ResetMoodIndicators()
		{
			positiveArrowGameObject.SetActive(false);
			neutralArrowGameObject.SetActive(false);
			negativeArrowGameObject.SetActive(false);
		}

		public void ResetEmoticons()
		{
			happyFaceIcon.SetActive(false);
			neutralFaceIcon.SetActive(false);
			cringeFaceIcon.SetActive(false);
		}

		public void CheckFinalScore()
		{
			if(currentScore > 0) happyFaceIcon.SetActive(true);
			else if (currentScore < 0) cringeFaceIcon.SetActive(true);
			else neutralFaceIcon.SetActive(true);
		}
	}
}