using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000089 RID: 137
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Impact Audio")]
public class SECTR_ImpactAudio : MonoBehaviour
{
	// Token: 0x060002E3 RID: 739 RVA: 0x0001261C File Offset: 0x0001081C
	private void OnEnable()
	{
		int count = this.SurfaceImpacts.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_ImpactAudio.ImpactSound impactSound = this.SurfaceImpacts[i];
			if (impactSound.SurfaceMaterial != null)
			{
				if (this.surfaceTable == null)
				{
					this.surfaceTable = new Dictionary<PhysicMaterial, SECTR_ImpactAudio.ImpactSound>();
				}
				this.surfaceTable[impactSound.SurfaceMaterial] = impactSound;
			}
		}
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00012681 File Offset: 0x00010881
	private void OnDisable()
	{
		this.surfaceTable = null;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0001268C File Offset: 0x0001088C
	public void OnCollisionStay(Collision collision)
	{
		if (Time.time >= this.nextImpactTime && collision != null && collision.contacts.Length != 0 && collision.relativeVelocity.sqrMagnitude >= this.MinImpactSpeed * this.MinImpactSpeed)
		{
			SECTR_ImpactAudio.ImpactSound defaultSound;
			if (collision.collider.sharedMaterial == null || this.surfaceTable == null || !this.surfaceTable.TryGetValue(collision.collider.sharedMaterial, out defaultSound))
			{
				defaultSound = this.DefaultSound;
			}
			SECTR_AudioSystem.Play(defaultSound.ImpactCue, collision.contacts[0].point, false);
			this.nextImpactTime = Time.time + this.MinImpactInterval;
		}
	}

	// Token: 0x04000305 RID: 773
	private float nextImpactTime;

	// Token: 0x04000306 RID: 774
	private Dictionary<PhysicMaterial, SECTR_ImpactAudio.ImpactSound> surfaceTable;

	// Token: 0x04000307 RID: 775
	[SECTR_ToolTip("Default sound to play on impact.")]
	public SECTR_ImpactAudio.ImpactSound DefaultSound;

	// Token: 0x04000308 RID: 776
	[SECTR_ToolTip("Surface specific impact sounds.")]
	public List<SECTR_ImpactAudio.ImpactSound> SurfaceImpacts = new List<SECTR_ImpactAudio.ImpactSound>();

	// Token: 0x04000309 RID: 777
	[SECTR_ToolTip("The minimum relative speed at the time of impact required to trigger this cue.")]
	public float MinImpactSpeed = 0.01f;

	// Token: 0x0400030A RID: 778
	[SECTR_ToolTip("The minimum amount of time between playback of this sound.")]
	public float MinImpactInterval = 0.5f;

	// Token: 0x0200008A RID: 138
	[Serializable]
	public class ImpactSound
	{
		// Token: 0x0400030B RID: 779
		public PhysicMaterial SurfaceMaterial;

		// Token: 0x0400030C RID: 780
		public SECTR_AudioCue ImpactCue;
	}
}
