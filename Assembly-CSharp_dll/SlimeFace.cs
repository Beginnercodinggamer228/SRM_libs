using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200047D RID: 1149
[CreateAssetMenu(menuName = "Slimes/Slime Face")]
public class SlimeFace : ScriptableObject
{
	// Token: 0x060017D5 RID: 6101 RVA: 0x0005C8C6 File Offset: 0x0005AAC6
	public SlimeExpressionFace GetExpressionFace(SlimeFace.SlimeExpression expression)
	{
		return this._expressionToFaceLookup[expression];
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x0005C8D4 File Offset: 0x0005AAD4
	public void OnEnable()
	{
		this._expressionToFaceLookup.Clear();
		for (int i = 0; i < this.ExpressionFaces.Length; i++)
		{
			SlimeExpressionFace slimeExpressionFace = this.ExpressionFaces[i];
			this._expressionToFaceLookup.Add(slimeExpressionFace.SlimeExpression, slimeExpressionFace);
		}
	}

	// Token: 0x0400170D RID: 5901
	public SlimeExpressionFace[] ExpressionFaces;

	// Token: 0x0400170E RID: 5902
	private Dictionary<SlimeFace.SlimeExpression, SlimeExpressionFace> _expressionToFaceLookup = new Dictionary<SlimeFace.SlimeExpression, SlimeExpressionFace>(SlimeFace.DefaultSlimeExpressionComparer);

	// Token: 0x0400170F RID: 5903
	public static SlimeFace.SlimeExpressionComparer DefaultSlimeExpressionComparer = new SlimeFace.SlimeExpressionComparer();

	// Token: 0x0200047E RID: 1150
	public enum SlimeExpression
	{
		// Token: 0x04001711 RID: 5905
		None,
		// Token: 0x04001712 RID: 5906
		Alarm,
		// Token: 0x04001713 RID: 5907
		Angry,
		// Token: 0x04001714 RID: 5908
		AttackTelegraph,
		// Token: 0x04001715 RID: 5909
		Awe,
		// Token: 0x04001716 RID: 5910
		Blink,
		// Token: 0x04001717 RID: 5911
		Blush,
		// Token: 0x04001718 RID: 5912
		BlushBlink,
		// Token: 0x04001719 RID: 5913
		ChompClosed,
		// Token: 0x0400171A RID: 5914
		ChompOpen,
		// Token: 0x0400171B RID: 5915
		Elated,
		// Token: 0x0400171C RID: 5916
		Feral,
		// Token: 0x0400171D RID: 5917
		Fried,
		// Token: 0x0400171E RID: 5918
		Glitch,
		// Token: 0x0400171F RID: 5919
		Grimace,
		// Token: 0x04001720 RID: 5920
		Happy,
		// Token: 0x04001721 RID: 5921
		Hungry,
		// Token: 0x04001722 RID: 5922
		Invoke,
		// Token: 0x04001723 RID: 5923
		Scared,
		// Token: 0x04001724 RID: 5924
		Starving,
		// Token: 0x04001725 RID: 5925
		Wince
	}

	// Token: 0x0200047F RID: 1151
	public class SlimeExpressionComparer : IEqualityComparer<SlimeFace.SlimeExpression>
	{
		// Token: 0x060017D9 RID: 6105 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(SlimeFace.SlimeExpression slimeExpr1, SlimeFace.SlimeExpression slimeExpr2)
		{
			return slimeExpr1 == slimeExpr2;
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(SlimeFace.SlimeExpression slimeExpr)
		{
			return (int)slimeExpr;
		}
	}
}
