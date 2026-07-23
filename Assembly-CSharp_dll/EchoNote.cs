using System;
using UnityEngine;

// Token: 0x020001D6 RID: 470
public class EchoNote : MonoBehaviour
{
	// Token: 0x060009E3 RID: 2531 RVA: 0x0002BAD4 File Offset: 0x00029CD4
	public void OnTriggerEnter(Collider collider)
	{
		Identifiable.Id id = Identifiable.GetId(collider.gameObject);
		if (PhysicsUtil.IsPlayerMainCollider(collider) || Identifiable.IsSlime(id))
		{
			this.renderer.material.SetFloat("_StartTime", Time.timeSinceLevelLoad);
			SECTR_AudioSystem.Play(SRSingleton<SceneContext>.Instance.InstrumentDirector.currentInstrument.cue, null, base.transform.position, false, new int?(this.clip - 1), id == Identifiable.Id.PLAYER);
		}
	}

	// Token: 0x04000832 RID: 2098
	[Tooltip("Note renderer; used to adjust animation when triggered.")]
	public Renderer renderer;

	// Token: 0x04000833 RID: 2099
	[Tooltip("Clip from the onCollisionCue to play. (1-indexed)")]
	[Range(1f, 13f)]
	public int clip;
}
