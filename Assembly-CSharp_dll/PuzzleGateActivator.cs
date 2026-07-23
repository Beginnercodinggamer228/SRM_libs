using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200075A RID: 1882
public class PuzzleGateActivator : MonoBehaviour
{
	// Token: 0x06002749 RID: 10057 RVA: 0x000954C9 File Offset: 0x000936C9
	public void OnTriggerEnter(Collider col)
	{
		if (PhysicsUtil.IsPlayerMainCollider(col))
		{
			this.playersPresent++;
		}
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000954E1 File Offset: 0x000936E1
	public void OnTriggerExit(Collider col)
	{
		if (PhysicsUtil.IsPlayerMainCollider(col))
		{
			this.playersPresent--;
		}
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000954F9 File Offset: 0x000936F9
	public void Update()
	{
		if (this.playersPresent > 0)
		{
			this.TryToActivate();
		}
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x0009550C File Offset: 0x0009370C
	public void TryToActivate()
	{
		if (this.gateDoor.CurrState == AccessDoor.State.CLOSED)
		{
			this.gateDoor.CurrState = AccessDoor.State.OPEN;
			base.StartCoroutine(this.DoDeactivateSequence());
			AnalyticsUtil.CustomEvent("PuzzleOpened", new Dictionary<string, object>
			{
				{
					"name",
					base.name
				}
			}, true);
		}
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x00095561 File Offset: 0x00093761
	private IEnumerator DoDeactivateSequence()
	{
		foreach (PuzzleGateActivator.SequenceEntry sequenceEntry in this.deactivateSequence)
		{
			SECTR_AudioSystem.Play(sequenceEntry.cue, sequenceEntry.toDeactivate.transform.position, false);
			sequenceEntry.toDeactivate.SetActive(false);
			yield return new WaitForSeconds(this.sequenceStepDelay);
		}
		PuzzleGateActivator.SequenceEntry[] array = null;
		if (this.deactivateAfterSequence != null)
		{
			this.deactivateAfterSequence.SetActive(false);
		}
		if (this.deactivateSequence.Length != 0)
		{
			yield return new WaitForSeconds(this.stingerDelay - this.sequenceStepDelay);
			SECTR_AudioSystem.Play(this.stingerCue, this.deactivateSequence[this.deactivateSequence.Length - 1].toDeactivate.transform.position, false);
		}
		yield return null;
		yield break;
	}

	// Token: 0x0400270E RID: 9998
	public float sequenceStepDelay = 1f;

	// Token: 0x0400270F RID: 9999
	public float stingerDelay = 6f;

	// Token: 0x04002710 RID: 10000
	public PuzzleGateActivator.SequenceEntry[] deactivateSequence;

	// Token: 0x04002711 RID: 10001
	public GameObject deactivateAfterSequence;

	// Token: 0x04002712 RID: 10002
	public AccessDoor gateDoor;

	// Token: 0x04002713 RID: 10003
	[Tooltip("The sound to play after all slot cues are played.")]
	public SECTR_AudioCue stingerCue;

	// Token: 0x04002714 RID: 10004
	private int playersPresent;

	// Token: 0x0200075B RID: 1883
	[Serializable]
	public class SequenceEntry
	{
		// Token: 0x04002715 RID: 10005
		public GameObject toDeactivate;

		// Token: 0x04002716 RID: 10006
		public SECTR_AudioCue cue;
	}
}
