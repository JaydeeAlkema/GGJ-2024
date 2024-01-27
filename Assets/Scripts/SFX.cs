using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SFX : MonoBehaviour
{
	[SerializeField] private List<AudioClip> sfxList = new List<AudioClip>();

	[SerializeField] private AudioSource audioSource = null;

	public void PlaySFX(int index)
	{
		if (audioSource == null)
		{
			Debug.Log("NO AUDIO SOURCE");
		}
		else
		{
			audioSource.PlayOneShot(sfxList[index]);
		}
	}

}
