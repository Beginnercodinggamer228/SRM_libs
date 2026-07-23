using System;
using UnityEngine;

// Token: 0x020004A3 RID: 1187
public class SplatOnImpact : CollidableActorBehaviour, Collidable
{
	// Token: 0x060018C3 RID: 6339 RVA: 0x000602ED File Offset: 0x0005E4ED
	public override void Awake()
	{
		base.Awake();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.slimeFaceAnimator = base.GetComponent<SlimeFaceAnimator>();
		this.appearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.penWallLayer = LayerMask.NameToLayer("Pen Walls");
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x0006032C File Offset: 0x0005E52C
	public void ProcessCollisionEnter(Collision col)
	{
		if (col.rigidbody == null)
		{
			float num = float.NegativeInfinity;
			ContactPoint? contactPoint = null;
			int contacts = col.GetContacts(SplatOnImpact.local_contactResults);
			for (int i = 0; i < contacts; i++)
			{
				ContactPoint value = SplatOnImpact.local_contactResults[i];
				float num2 = Vector3.Dot(value.normal, col.relativeVelocity);
				if (num2 > num)
				{
					num = num2;
					contactPoint = new ContactPoint?(value);
				}
			}
			if (num > 6f)
			{
				bool flag = col.gameObject.layer == this.penWallLayer;
				GameObject gameObject;
				if (flag)
				{
					gameObject = SRBehaviour.SpawnAndPlayFX(this.splatFXPrefab, contactPoint.Value.point, Quaternion.LookRotation(contactPoint.Value.normal));
				}
				else
				{
					gameObject = SRBehaviour.InstantiateDynamic(this.splatPrefab, contactPoint.Value.point, Quaternion.LookRotation(contactPoint.Value.normal), false);
				}
				gameObject.transform.Rotate(Vector3.forward, Randoms.SHARED.GetFloat(360f), Space.Self);
				SlimeAppearance.Palette appearancePalette = this.appearanceApplicator.GetAppearancePalette();
				RecolorSlimeMaterial[] componentsInChildren = gameObject.GetComponentsInChildren<RecolorSlimeMaterial>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].SetColors(appearancePalette.Top, appearancePalette.Middle, appearancePalette.Bottom);
				}
				if (!flag)
				{
					float num3 = Mathf.Min(2.5f, Mathf.Pow(num / 6f, 0.25f));
					float inRange = Randoms.SHARED.GetInRange(0.75f, 2.25f);
					FadeAndDestroySplat component = gameObject.GetComponent<FadeAndDestroySplat>();
					component.SetScale(base.transform.localScale.x * inRange * num3);
					component.SetColors(appearancePalette.Top, appearancePalette.Middle, appearancePalette.Bottom);
				}
				if (this.slimeAudio != null)
				{
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.splatCue);
				}
			}
			if (num > 10f)
			{
				if (this.slimeAudio != null)
				{
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceSplatCue);
				}
				if (this.slimeFaceAnimator != null)
				{
					this.slimeFaceAnimator.SetTrigger("triggerMinorWince");
				}
			}
		}
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x04001864 RID: 6244
	public GameObject splatPrefab;

	// Token: 0x04001865 RID: 6245
	public GameObject splatFXPrefab;

	// Token: 0x04001866 RID: 6246
	private SlimeAppearanceApplicator appearanceApplicator;

	// Token: 0x04001867 RID: 6247
	private SlimeAudio slimeAudio;

	// Token: 0x04001868 RID: 6248
	private SlimeFaceAnimator slimeFaceAnimator;

	// Token: 0x04001869 RID: 6249
	private int penWallLayer;

	// Token: 0x0400186A RID: 6250
	private const float SPLAT_THRESHOLD = 6f;

	// Token: 0x0400186B RID: 6251
	private const float COLLISION_AUDIO_THRESHOLD = 6f;

	// Token: 0x0400186C RID: 6252
	private const float COLLISION_VO_THRESHOLD = 10f;

	// Token: 0x0400186D RID: 6253
	private const float MIN_SCALE_FACTOR = 0.75f;

	// Token: 0x0400186E RID: 6254
	private const float MAX_SCALE_FACTOR = 2.25f;

	// Token: 0x0400186F RID: 6255
	private const float SPEED_SCALE_POW = 0.25f;

	// Token: 0x04001870 RID: 6256
	private const float MAX_SPEED_SCALE = 2.5f;

	// Token: 0x04001871 RID: 6257
	private static ContactPoint[] local_contactResults = new ContactPoint[10];
}
