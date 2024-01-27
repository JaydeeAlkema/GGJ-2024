using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField, BoxGroup("References"), Expandable] private PlayerAction[] actions = null;
		[SerializeField, BoxGroup("References")] private LevelData[] levels = null;
		[SerializeField, BoxGroup("References")] private MoveFrame[] moveFrames = null;
		[SerializeField, BoxGroup("References")] private Ducky ducky = null;
		[SerializeField, BoxGroup("References")] private Animator photoCameraAnimator = null;
		[SerializeField, BoxGroup("References")] private Animator photoCameraFlashAnimator = null;
		[SerializeField, BoxGroup("References")] private GameObject scoreTextAnchor = null;
		[SerializeField, BoxGroup("References")] private TextMeshProUGUI scoreTextElement = null;
		[SerializeField, BoxGroup("References")] private TextMeshProUGUI loseText = null;
		[SerializeField, BoxGroup("References")] private TextMeshProUGUI winText = null;
		[SerializeField, BoxGroup("References")] private TextMeshProUGUI levelTextElement = null;
		[SerializeField, BoxGroup("References")] private GameObject buttonsObjects;
		[SerializeField, BoxGroup("References")] private SFX sfx;
		[Space]
		[SerializeField, BoxGroup("Runtime")] private int currentLevel = 0;
		[SerializeField, BoxGroup("Runtime"), Expandable] private List<PlayerAction> actionsQueue = new();
		[SerializeField, BoxGroup("Runtime")] private int currentActionQueueIndex = 0;
		[SerializeField, BoxGroup("Runtime")] private bool canInputActions = true;
		[SerializeField, BoxGroup("Runtime")] private float currentLevelTimer = 0;
		[SerializeField, BoxGroup("Runtime")] private float currentScoreMultiplier = 60;
		[SerializeField, BoxGroup("Runtime")] private int score = 0;
		[Space]
		[SerializeField, BoxGroup("Settings")] private int maxActionsInQueue = 4;
		[Space]
		[SerializeField, BoxGroup("Debugging")] private bool printCrowdMemberFinalScores = false;

		private GameManager instance = null;
		private event Action OnAction;
		private Controls controls;

		public GameManager Instance { get => instance; }

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}

			controls = new Controls();
			controls.Enable();
		}

		private void Start()
		{
			SetNextMoveFrameAsSelected();
			NextLevel();
		}

		private void Update()
		{
			currentLevelTimer += Time.deltaTime;
			if (currentScoreMultiplier > 0) currentScoreMultiplier -= Time.deltaTime;
		}

		private void OnEnable()
		{
			controls.Player.Action1.performed += ctx => StartCoroutine(AddActionToQueue(actions[0]));
			controls.Player.Action2.performed += ctx => StartCoroutine(AddActionToQueue(actions[1]));
			controls.Player.Action3.performed += ctx => StartCoroutine(AddActionToQueue(actions[2]));
			controls.Player.Action4.performed += ctx => StartCoroutine(AddActionToQueue(actions[3]));
		}

		private void OnDisable()
		{
			controls.Disable();
			controls.Player.Action1.performed -= ctx => StartCoroutine(AddActionToQueue(actions[0]));
			controls.Player.Action2.performed -= ctx => StartCoroutine(AddActionToQueue(actions[1]));
			controls.Player.Action3.performed -= ctx => StartCoroutine(AddActionToQueue(actions[2]));
			controls.Player.Action4.performed -= ctx => StartCoroutine(AddActionToQueue(actions[3]));
		}

		private IEnumerator AddActionToQueue(PlayerAction playerAction)
		{
			if (!canInputActions) yield break;
			canInputActions = false;

			actionsQueue.Add(playerAction);
			ducky.SetAndResetTriggers(playerAction.GetAnimationString());

			AddIconToCurrentIconHolder();
			currentActionQueueIndex++;
			SetNextMoveFrameAsSelected();
			TestSequence();

			if (currentActionQueueIndex >= maxActionsInQueue || AllCrowdMembersAreHappy())
			{
				PrintActionsQueue();
				foreach (CrowdMember crowdmember in levels[currentLevel].crowdMembers)
				{
					crowdmember.CheckFinalScore();
					crowdmember.ResetMoodIndicators();
				}
				yield return new WaitForSeconds(ducky.GetAnimator().GetCurrentAnimatorClipInfo(0).Length + 0.35f);
				if (AllCrowdMembersAreHappy())
				{
					buttonsObjects.SetActive(false);
					scoreTextAnchor.SetActive(true);
					winText.gameObject.SetActive(true);
					score = Mathf.FloorToInt(currentScoreMultiplier / currentLevelTimer * 10);
					scoreTextElement.text = $"Score: {score}";
					photoCameraAnimator.SetTrigger("doSnapCamera");
					photoCameraFlashAnimator.SetTrigger("doFlash");
					yield return new WaitForSeconds(0.5f);
					sfx.PlaySFX(0);
					sfx.PlaySFX(1);
					yield return new WaitForSeconds(photoCameraAnimator.GetCurrentAnimatorClipInfo(0).Length + 3f);
				}
				else
				{
					loseText.gameObject.SetActive(true);
					Debug.Log("Crowd members are not happy", this);
					yield return new WaitForSeconds(3f);
					loseText.gameObject.SetActive(false);
					winText.gameObject.SetActive(false);
				}
				currentActionQueueIndex = 0;
				actionsQueue.Clear();
				if (AllCrowdMembersAreHappy()) NextLevel();
				ResetCrowdMembers();
				ResetIcons();
			}
			else
			{
				yield return new WaitForSeconds(ducky.GetAnimator().GetCurrentAnimatorClipInfo(0).Length);
			}
			canInputActions = true;
		}
		private void AddIconToCurrentIconHolder()
		{
			moveFrames[currentActionQueueIndex].SetIcon(actionsQueue[^1].GetIcon());
		}
		private void SetNextMoveFrameAsSelected()
		{
			foreach (MoveFrame iconHolder in moveFrames)
			{
				iconHolder.SetAsUnselected();
			}
			if (currentActionQueueIndex < maxActionsInQueue)
			{
				if (currentActionQueueIndex == 0) moveFrames[0].SetAsSelected();
				else if (currentActionQueueIndex == maxActionsInQueue - 1) moveFrames[^1].SetAsSelected();
				else moveFrames[currentActionQueueIndex].SetAsSelected();
			}
		}
		private void ResetIcons()
		{
			foreach (MoveFrame iconHolder in moveFrames)
			{
				iconHolder.SetIcon(null);
				iconHolder.SetAsUnselected();
			}
			SetNextMoveFrameAsSelected();
		}
		private void TestSequence()
		{
			SetupCrowdMembers();
			OnAction?.Invoke();
			OnAction = null;
			if (printCrowdMemberFinalScores) GetFinalStatesOfCrowdMembers();
		}
		private void SetupCrowdMembers()
		{
			foreach (PlayerAction playerAction in actionsQueue)
			{
				foreach (CrowdMember crowdMember in levels[currentLevel].crowdMembers)
				{
					OnAction += () => crowdMember.CheckPossitivesAndNegatives(playerAction.PlayerActionTypes);
				}
			}
		}
		private void ResetCrowdMembers()
		{
			if (currentLevel >= levels.Length) return;
			foreach (CrowdMember crowdMember in levels[currentLevel].crowdMembers)
			{
				crowdMember.SetScore(crowdMember.GetDefaultScore());
				crowdMember.SetFacesToCurrentScore();
				crowdMember.ResetMoodIndicators();
				crowdMember.ResetEmoticons();
			}
			Debug.Log("Crowd members reset", this);
		}
		private void GetFinalStatesOfCrowdMembers()
		{
			string text = "";
			foreach (CrowdMember crowdMember in levels[currentLevel].crowdMembers)
			{
				text += $"[ {crowdMember.gameObject.name}: {crowdMember.GetScore()} ]";
			}
			string textColor = AllCrowdMembersAreHappy() ? "green" : "red";
			Debug.Log($"<color={textColor}>Final states of crowd members: {text}</color>", this);
		}
		private bool AllCrowdMembersAreHappy()
		{
			bool allGood = true;
			foreach (CrowdMember crowdMember in levels[currentLevel].crowdMembers)
			{
				if (crowdMember.GetScore() <= 0) allGood = false;
			}
			return allGood;
		}
		private void PrintActionsQueue()
		{
			string text = "";
			foreach (PlayerAction playerAction in actionsQueue)
			{
				text += $"| {playerAction.name} |";
			}
			Debug.Log($"Actions queue: {text}", this);
		}
		private void NextLevel()
		{
			SetNextMoveFrameAsSelected();
			currentLevel++;
			ResetCrowdMembers();
			if (currentLevel >= levels.Length)
			{
				currentLevel--;
				ResetGame();
				return;
			}
			winText.gameObject.SetActive(false);
			loseText.gameObject.SetActive(false);
			scoreTextAnchor.SetActive(false);
			currentLevelTimer = 0;
			currentScoreMultiplier = 60;
			levelTextElement.text = $"Level: {currentLevel + 1}";
			buttonsObjects.SetActive(true);

			for (int i = 0; i < levels.Length; i++)
			{
				LevelData levelData = levels[i];
				if (i == currentLevel) levelData.crowdParent.SetActive(true);
				else levelData.crowdParent.SetActive(false);
			}
			score = 0;
		}
		private void ResetGame()
		{
			actionsQueue.Clear();
			currentActionQueueIndex = 0;
			ResetCrowdMembers();
			ResetIcons();
			currentLevel = -1;
			NextLevel();
		}
	}

	[Serializable]
	public struct LevelData
	{
		public GameObject crowdParent;
		public CrowdMember[] crowdMembers;
		public float timeToCompleet;
	}
}
