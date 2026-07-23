using System;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class Incinerate : SRBehaviour
{
	// Token: 0x060010CF RID: 4303 RVA: 0x00043600 File Offset: 0x00041800
	public void Awake()
	{
		this.incinerateAudio = base.GetComponent<SECTR_AudioSource>();
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x00043610 File Offset: 0x00041810
	private void OnCollisionEnter(Collision col)
	{
		Identifiable component = col.gameObject.GetComponent<Identifiable>();
		if (component == null || !this.CanBeIncinerated(component))
		{
			return;
		}
		if (component.id == Identifiable.Id.ELDER_HEN || component.id == Identifiable.Id.ELDER_ROOSTER)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.INCINERATED_ELDER_CHICKENS, 1);
		}
		else if (component.id == Identifiable.Id.CHICK || component.id == Identifiable.Id.BRIAR_CHICK || component.id == Identifiable.Id.STONY_CHICK || component.id == Identifiable.Id.PAINTED_CHICK)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.INCINERATED_CHICKS, 1);
		}
		this.ProcessIncinerateResults(component.id, 1, col.gameObject.transform.position, col.gameObject.transform.rotation);
		Destroyer.DestroyActor(col.gameObject, "Incinerate.OnCollisionEnter", false);
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x000436DC File Offset: 0x000418DC
	public void ProcessIncinerateResults(Identifiable.Id id, int amount, Vector3 position, Quaternion rotation)
	{
		SRBehaviour.SpawnAndPlayFX(this.ExplosionFX, position, rotation);
		Vacuumable component = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id).GetComponent<Vacuumable>();
		if (component == null || component.size == Vacuumable.Size.NORMAL)
		{
			this.incinerateAudio.Cue = this.smallCue;
			this.incinerateAudio.Play();
		}
		else
		{
			this.incinerateAudio.Cue = this.largeCue;
			this.incinerateAudio.Play();
		}
		if (this.ProcessIncinerateResults(id, amount))
		{
			SRBehaviour.SpawnAndPlayFX(this.ashFX, position, rotation);
		}
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x00043772 File Offset: 0x00041972
	public bool ProcessIncinerateResults(Identifiable.Id id, int amount)
	{
		if (this.ashTrough != null && this.ashTrough.isActiveAndEnabled && Identifiable.IsFood(id))
		{
			this.ashTrough.AddAsh((float)amount * this.ashPerIncineration);
			return true;
		}
		return false;
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x000437AE File Offset: 0x000419AE
	public int GetAshSpace()
	{
		if (this.ashTrough != null && this.ashTrough.isActiveAndEnabled)
		{
			return Mathf.CeilToInt(this.ashTrough.GetAshSpace() / this.ashPerIncineration);
		}
		return 0;
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x000437E4 File Offset: 0x000419E4
	private bool CanBeIncinerated(Identifiable ident)
	{
		return ident.id != Identifiable.Id.FIRE_PLORT && ident.id != Identifiable.Id.FIRE_SLIME && ident.id != Identifiable.Id.CHARCOAL_BRICK_TOY;
	}

	// Token: 0x04000FB8 RID: 4024
	public GameObject ExplosionFX;

	// Token: 0x04000FB9 RID: 4025
	public GameObject ashFX;

	// Token: 0x04000FBA RID: 4026
	public SECTR_AudioCue smallCue;

	// Token: 0x04000FBB RID: 4027
	public SECTR_AudioCue largeCue;

	// Token: 0x04000FBC RID: 4028
	public FillableAshSource ashTrough;

	// Token: 0x04000FBD RID: 4029
	public float ashPerIncineration;

	// Token: 0x04000FBE RID: 4030
	private SECTR_AudioSource incinerateAudio;
}
