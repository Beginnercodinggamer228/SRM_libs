using System;
using System.Collections.Generic;

// Token: 0x02000530 RID: 1328
public class ToyDirector
{
	// Token: 0x06001BA0 RID: 7072 RVA: 0x00069A1C File Offset: 0x00067C1C
	public void Register(Identifiable.Id id)
	{
		this.registered.RemoveAll((Identifiable.Id it) => it == id);
		this.registered.Add(id);
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00069A5F File Offset: 0x00067C5F
	public IEnumerable<Identifiable.Id> GetPurchaseableToys()
	{
		ProgressDirector progressDirector = SRSingleton<SceneContext>.Instance.ProgressDirector;
		int progress = progressDirector.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER);
		foreach (Identifiable.Id id in ToyDirector.BASE_TOYS)
		{
			yield return id;
		}
		List<Identifiable.Id>.Enumerator enumerator = default(List<Identifiable.Id>.Enumerator);
		if (progress >= 10)
		{
			foreach (Identifiable.Id id2 in ToyDirector.UPGRADED_TOYS)
			{
				yield return id2;
			}
			enumerator = default(List<Identifiable.Id>.Enumerator);
		}
		foreach (Identifiable.Id id3 in this.registered)
		{
			yield return id3;
		}
		enumerator = default(List<Identifiable.Id>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04001AC9 RID: 6857
	private static readonly List<Identifiable.Id> BASE_TOYS = new List<Identifiable.Id>
	{
		Identifiable.Id.BEACH_BALL_TOY,
		Identifiable.Id.BIG_ROCK_TOY,
		Identifiable.Id.YARN_BALL_TOY,
		Identifiable.Id.NIGHT_LIGHT_TOY,
		Identifiable.Id.POWER_CELL_TOY,
		Identifiable.Id.BOMB_BALL_TOY,
		Identifiable.Id.BUZZY_BEE_TOY,
		Identifiable.Id.RUBBER_DUCKY_TOY,
		Identifiable.Id.OCTO_BUDDY_TOY
	};

	// Token: 0x04001ACA RID: 6858
	private const int CORPORATE_LEVEL_UNLOCK = 10;

	// Token: 0x04001ACB RID: 6859
	private static readonly List<Identifiable.Id> UPGRADED_TOYS = new List<Identifiable.Id>
	{
		Identifiable.Id.CRYSTAL_BALL_TOY,
		Identifiable.Id.STUFFED_CHICKEN_TOY,
		Identifiable.Id.PUZZLE_CUBE_TOY,
		Identifiable.Id.DISCO_BALL_TOY,
		Identifiable.Id.GYRO_TOP_TOY,
		Identifiable.Id.CHARCOAL_BRICK_TOY,
		Identifiable.Id.SOL_MATE_TOY,
		Identifiable.Id.STEGO_BUDDY_TOY
	};

	// Token: 0x04001ACC RID: 6860
	private List<Identifiable.Id> registered = new List<Identifiable.Id>();
}
