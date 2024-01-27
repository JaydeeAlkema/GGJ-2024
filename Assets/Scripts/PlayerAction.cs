using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public enum PlayerActionType
	{
		Cool,
		Clumsy,
		Funny,
		Weird
	}

	[CreateAssetMenu(fileName = "PlayerAction", menuName = "ScriptableObjects/PlayerAction", order = 1)]
	public class PlayerAction : ScriptableObject
	{
		[SerializeField, BoxGroup("References")] private string animationString = null;
		[SerializeField, BoxGroup("References")] private Sprite icon = null;
		[Space]
		[SerializeField, BoxGroup("Settings")] private List<PlayerActionType> playerActionTypes = new();

		public List<PlayerActionType> PlayerActionTypes { get => playerActionTypes; set => playerActionTypes = value; }

		public string GetAnimationString() => animationString;
		public Sprite GetIcon() => icon;
	}
}