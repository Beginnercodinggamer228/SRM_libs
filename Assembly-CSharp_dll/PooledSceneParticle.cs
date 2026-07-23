using System;
using UnityEngine;

// Token: 0x0200074F RID: 1871
public class PooledSceneParticle : SRBehaviour
{
	// Token: 0x0600270D RID: 9997 RVA: 0x000946E6 File Offset: 0x000928E6
	public void Awake()
	{
		SRSingleton<SceneContext>.Instance.SceneParticleDirector.AddSecondFrameListener(this);
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000946F8 File Offset: 0x000928F8
	public void OnEnable()
	{
		if (this.initialized)
		{
			this.InitParticle();
		}
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x00094708 File Offset: 0x00092908
	public void OnSecondFrame()
	{
		this.initialized = true;
		if (base.isActiveAndEnabled)
		{
			this.InitParticle();
		}
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x00094720 File Offset: 0x00092920
	protected virtual void InitParticle()
	{
		if (this.particlePrefab != null && this.particle == null)
		{
			this.particle = SRBehaviour.SpawnAndPlayFX(this.particlePrefab, base.gameObject);
			Animator[] array = this.animsToRebind;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Rebind();
			}
		}
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x0009477D File Offset: 0x0009297D
	public void OnApplicationQuit()
	{
		this.isShuttingDown = true;
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x00094788 File Offset: 0x00092988
	public void OnDisable()
	{
		if (this.particle != null && !this.isShuttingDown)
		{
			if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.fxPool != null)
			{
				SRSingleton<SceneContext>.Instance.fxPool.RecycleAfterFrame(this.particle);
				this.particle = null;
			}
			Animator[] array = this.animsToRebind;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Rebind();
			}
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000947FD File Offset: 0x000929FD
	public ParticleSystem GetParticleSystem()
	{
		if (!(this.particle != null))
		{
			return null;
		}
		return this.particle.GetComponent<ParticleSystem>();
	}

	// Token: 0x040026B1 RID: 9905
	public GameObject particlePrefab;

	// Token: 0x040026B2 RID: 9906
	[Tooltip("Any animators we need to inform that we've messed with the hierarchy")]
	public Animator[] animsToRebind;

	// Token: 0x040026B3 RID: 9907
	protected GameObject particle;

	// Token: 0x040026B4 RID: 9908
	private bool initialized;

	// Token: 0x040026B5 RID: 9909
	private bool isShuttingDown;
}
