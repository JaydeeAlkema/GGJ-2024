using NaughtyAttributes;
using System;
using UnityEngine;

namespace Assets.Scripts
{
	public class ActionsQueue : MonoBehaviour
	{
		[SerializeField, BoxGroup("References"), Expandable] private PlayerAction[] actions = null;
		[SerializeField, BoxGroup("References")] private CrowdMember[] crowdMembers = null;

		private ActionsQueue instance = null;
		private event Action OnAction;

		public ActionsQueue Instance { get => instance; }

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
		}

		[Button]
		private void TestSequence()
		{
			SetupCrowdMembers();
			OnAction?.Invoke();
			OnAction = null;
		}

		private void SetupCrowdMembers()
		{
			foreach (PlayerAction playerAction in actions)
			{
				foreach (CrowdMember crowdMember in crowdMembers)
				{
					OnAction += () => crowdMember.CheckPossitivesAndNegatives(playerAction.PlayerActionTypes);
				}
			}
		}

		private void ResetCrowdMembers()
		{
			foreach (CrowdMember crowdMember in crowdMembers)
			{
				crowdMember.Score = 0;
			}
		}

		private void GetFinalStatesOfCrowdMembers()
		{
			foreach (CrowdMember crowdMember in crowdMembers)
			{
				Debug.Log($"Final score of {crowdMember.name}: {crowdMember.Score}", this);
			}
		}
	}
}
