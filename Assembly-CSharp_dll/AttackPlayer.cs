using System;
using UnityEngine;

// Token: 0x02000392 RID: 914
public class AttackPlayer : CollidableActorBehaviour, ControllerCollisionListener, Collidable, RegistryUpdateable
{
	// Token: 0x0600130B RID: 4875 RVA: 0x0004A889 File Offset: 0x00048A89
	public override void Awake()
	{
		this.vacuumable = base.GetComponent<Vacuumable>();
		this.chomper = base.GetComponent<Chomper>();
		this.faceAnim = base.GetComponent<SlimeFaceAnimator>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x0004A8BB File Offset: 0x00048ABB
	public void OnControllerCollision(GameObject obj)
	{
		this.MaybeSpinAndChomp(obj, false);
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x0004A8C6 File Offset: 0x00048AC6
	public void ProcessCollisionEnter(Collision col)
	{
		this.MaybeSpinAndChomp(col.gameObject, false);
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x0004A8D8 File Offset: 0x00048AD8
	public bool MaybeSpinAndChomp(GameObject obj, bool ignoreEmotions)
	{
		if (this.shouldAttackPlayer && this.chomper.CanChomp())
		{
			Identifiable.Id id = this.ExtractOtherId(obj);
			if (id == Identifiable.Id.PLAYER)
			{
				base.transform.LookAt(obj.transform);
				this.chomper.StartChomp(obj, id, false, true, null, new Chomper.OnChompCompleteDelegate(this.FinishChomp));
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x0004A937 File Offset: 0x00048B37
	public bool DoesAttack(GameObject other)
	{
		return this.ExtractOtherId(other) == Identifiable.Id.PLAYER;
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x0004A944 File Offset: 0x00048B44
	public bool MaybeChomp(GameObject obj)
	{
		if (this.shouldAttackPlayer && this.chomper.CanChomp())
		{
			Identifiable.Id id = this.ExtractOtherId(obj);
			if (id == Identifiable.Id.PLAYER)
			{
				this.chomper.StartChomp(obj, id, false, false, null, new Chomper.OnChompCompleteDelegate(this.FinishChomp));
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x0004A994 File Offset: 0x00048B94
	private Identifiable.Id ExtractOtherId(GameObject other)
	{
		int instanceID = other.GetInstanceID();
		Identifiable.Id result;
		if (AttackPlayer.recentIds.contains(instanceID))
		{
			Identifiable identifiable = AttackPlayer.recentIds.get(instanceID);
			result = ((identifiable == null) ? Identifiable.Id.NONE : identifiable.id);
		}
		else
		{
			Identifiable component = other.GetComponent<Identifiable>();
			AttackPlayer.recentIds.put(instanceID, component);
			result = ((component == null) ? Identifiable.Id.NONE : component.id);
		}
		return result;
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x0004AA00 File Offset: 0x00048C00
	public void RegistryUpdate()
	{
		if (this.shouldAttackPlayer && this.vacuumable != null && this.vacuumable.isHeld() && this.chomper.CanChomp())
		{
			GameObject player = SRSingleton<SceneContext>.Instance.Player;
			this.chomper.StartChomp(player, Identifiable.Id.PLAYER, true, false, null, new Chomper.OnChompCompleteDelegate(this.FinishChomp));
		}
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x0004AA65 File Offset: 0x00048C65
	public void CancelChomp(GameObject obj)
	{
		this.chomper.CancelChomp(obj);
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x0004AA74 File Offset: 0x00048C74
	private void FinishChomp(GameObject chomping, Identifiable.Id chompingId, bool whileHeld, bool wasLaunched)
	{
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.attackCue);
		if (whileHeld)
		{
			SRSingleton<Overlay>.Instance.PlayChomp();
		}
		if (chomping == null)
		{
			return;
		}
		this.faceAnim.SetTrigger("triggerChompClosed");
		this.DoDamage(chomping, false);
		if (this.onFinishChompSuccess != null)
		{
			this.onFinishChompSuccess(chomping);
		}
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0004AAE4 File Offset: 0x00048CE4
	private bool DoDamage(GameObject other, bool immediateMode)
	{
		if (other == null)
		{
			return true;
		}
		if (!immediateMode)
		{
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.gulpCue);
		}
		if (other.GetInterfaceComponent<Damageable>().Damage(this.damagePerAttack, base.gameObject))
		{
			DeathHandler.Kill(other, DeathHandler.Source.SLIME_ATTACK_PLAYER, base.gameObject, "AttackPlayer.DoDamage");
			if (!immediateMode)
			{
				this.PlayOnDeathAudio(other);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x0004AB54 File Offset: 0x00048D54
	private void PlayOnDeathAudio(GameObject other)
	{
		SlimeAudio componentInChildren = other.GetComponentInChildren<SlimeAudio>();
		if (componentInChildren != null && componentInChildren.slimeSounds.voiceDamageCue != null)
		{
			SECTR_AudioSystem.Play(componentInChildren.slimeSounds.voiceDamageCue, other.transform.position, false);
		}
	}

	// Token: 0x040011E5 RID: 4581
	public AttackPlayer.OnFinishChompSuccessDelegate onFinishChompSuccess;

	// Token: 0x040011E6 RID: 4582
	public bool shouldAttackPlayer;

	// Token: 0x040011E7 RID: 4583
	public int damagePerAttack = 20;

	// Token: 0x040011E8 RID: 4584
	private Chomper chomper;

	// Token: 0x040011E9 RID: 4585
	private SlimeFaceAnimator faceAnim;

	// Token: 0x040011EA RID: 4586
	private Vacuumable vacuumable;

	// Token: 0x040011EB RID: 4587
	private SlimeAudio slimeAudio;

	// Token: 0x040011EC RID: 4588
	private static LRUCache<int, Identifiable> recentIds = new LRUCache<int, Identifiable>(200);

	// Token: 0x02000393 RID: 915
	// (Invoke) Token: 0x0600131B RID: 4891
	public delegate void OnFinishChompSuccessDelegate(GameObject gameObject);
}
