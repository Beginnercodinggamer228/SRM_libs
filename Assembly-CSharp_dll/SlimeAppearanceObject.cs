using System;
using UnityEngine;

// Token: 0x02000452 RID: 1106
[Serializable]
public class SlimeAppearanceObject : MonoBehaviour
{
	// Token: 0x04001610 RID: 5648
	public SlimeAppearance.SlimeBone ParentBone;

	// Token: 0x04001611 RID: 5649
	public SlimeAppearance.SlimeBone RootBone;

	// Token: 0x04001612 RID: 5650
	public SlimeAppearance.SlimeBone[] AttachedBones;

	// Token: 0x04001613 RID: 5651
	public bool IgnoreLODIndex;

	// Token: 0x04001614 RID: 5652
	public int LODIndex;

	// Token: 0x04001615 RID: 5653
	[Tooltip("Indicates that this object should be referenced by the slime's rubber bone effect. Only the highest quality LOD body of the slime should check this.")]
	public bool AttachRubberBoneEffect;

	// Token: 0x04001616 RID: 5654
	[Tooltip("If this object is attached to the rubber bone effect of the slime, use this rubber type. Should generally be Slime or SlimeTarr")]
	public RubberBoneEffect.RubberType RubberType = RubberBoneEffect.RubberType.Slime;
}
