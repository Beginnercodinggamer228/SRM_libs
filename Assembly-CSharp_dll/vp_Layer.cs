using System;
using UnityEngine;

// Token: 0x0200083B RID: 2107
public sealed class vp_Layer
{
	// Token: 0x06002C33 RID: 11315 RVA: 0x000A6DA1 File Offset: 0x000A4FA1
	static vp_Layer()
	{
		Physics.IgnoreLayerCollision(8, 29);
		Physics.IgnoreLayerCollision(8, 11);
		Physics.IgnoreLayerCollision(8, 16);
		Physics.IgnoreLayerCollision(8, 14);
		Physics.IgnoreLayerCollision(29, 29);
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x000053FC File Offset: 0x000035FC
	private vp_Layer()
	{
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x000A6DD8 File Offset: 0x000A4FD8
	public static void Set(GameObject obj, int layer, bool recursive = false)
	{
		if (layer < 0 || layer > 31)
		{
			Debug.LogError("vp_Layer: Attempted to set layer id out of range [0-31].");
			return;
		}
		obj.layer = layer;
		if (recursive)
		{
			foreach (object obj2 in obj.transform)
			{
				vp_Layer.Set(((Transform)obj2).gameObject, layer, true);
			}
		}
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x000A6E54 File Offset: 0x000A5054
	public static bool IsInMask(int layer, int layerMask)
	{
		return (layerMask & 1 << layer) == 0;
	}

	// Token: 0x04002A52 RID: 10834
	public static readonly vp_Layer instance = new vp_Layer();

	// Token: 0x04002A53 RID: 10835
	public const int Default = 0;

	// Token: 0x04002A54 RID: 10836
	public const int TransparentFX = 1;

	// Token: 0x04002A55 RID: 10837
	public const int IgnoreRaycast = 2;

	// Token: 0x04002A56 RID: 10838
	public const int Water = 4;

	// Token: 0x04002A57 RID: 10839
	public const int MovableObject = 21;

	// Token: 0x04002A58 RID: 10840
	public const int Ragdoll = 22;

	// Token: 0x04002A59 RID: 10841
	public const int IgnoreBullets = 24;

	// Token: 0x04002A5A RID: 10842
	public const int Enemy = 25;

	// Token: 0x04002A5B RID: 10843
	public const int Pickup = 26;

	// Token: 0x04002A5C RID: 10844
	public const int Trigger = 27;

	// Token: 0x04002A5D RID: 10845
	public const int MovingPlatform = 28;

	// Token: 0x04002A5E RID: 10846
	public const int Weapon = 31;

	// Token: 0x04002A5F RID: 10847
	public const int Player = 8;

	// Token: 0x04002A60 RID: 10848
	public const int Launched = 9;

	// Token: 0x04002A61 RID: 10849
	public const int ActorIgnorePlayer = 11;

	// Token: 0x04002A62 RID: 10850
	public const int Mountains = 12;

	// Token: 0x04002A63 RID: 10851
	public const int Held = 13;

	// Token: 0x04002A64 RID: 10852
	public const int RaycastOnly = 14;

	// Token: 0x04002A65 RID: 10853
	public const int Actor = 15;

	// Token: 0x04002A66 RID: 10854
	public const int ActorEchoes = 16;

	// Token: 0x04002A67 RID: 10855
	public const int Beatrix = 17;

	// Token: 0x04002A68 RID: 10856
	public const int VacCone = 18;

	// Token: 0x04002A69 RID: 10857
	public const int Interactable = 19;

	// Token: 0x04002A6A RID: 10858
	public const int Drone = 20;

	// Token: 0x04002A6B RID: 10859
	public const int ActorStatic = 21;

	// Token: 0x04002A6C RID: 10860
	public const int ActorTrigger = 22;

	// Token: 0x04002A6D RID: 10861
	public const int PenWalls = 29;

	// Token: 0x0200083C RID: 2108
	public static class Mask
	{
		// Token: 0x04002A6E RID: 10862
		public const int BulletBlockers = -754974997;

		// Token: 0x04002A6F RID: 10863
		public const int ExternalBlockers = -675375893;

		// Token: 0x04002A70 RID: 10864
		public const int PhysicsBlockers = 270532864;

		// Token: 0x04002A71 RID: 10865
		public const int IgnoreWalkThru = -755066901;

		// Token: 0x04002A72 RID: 10866
		public const int AnyActor = -2206209;
	}
}
