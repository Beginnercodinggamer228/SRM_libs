using System;

namespace UnityEngine.UI
{
	// Token: 0x02000A25 RID: 2597
	public class RadialLayoutGroup : LayoutGroup
	{
		// Token: 0x060045EB RID: 17899 RVA: 0x000CDD7C File Offset: 0x000CBF7C
		public Transform GetChild(float radians)
		{
			Transform result = null;
			if (base.rectChildren.Count > 0)
			{
				Vector2 b = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * this.radius;
				float num = float.MaxValue;
				foreach (Transform transform in base.rectChildren)
				{
					float sqrMagnitude = (transform.localPosition - b).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						result = transform;
					}
				}
			}
			return result;
		}

		// Token: 0x060045EC RID: 17900 RVA: 0x000CDE28 File Offset: 0x000CC028
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			base.SetLayoutInputForAxis(this.childSize.x, this.childSize.x, this.childSize.x, 0);
		}

		// Token: 0x060045ED RID: 17901 RVA: 0x000CDE58 File Offset: 0x000CC058
		public override void CalculateLayoutInputVertical()
		{
			base.SetLayoutInputForAxis(this.childSize.y, this.childSize.y, this.childSize.y, 1);
		}

		// Token: 0x060045EE RID: 17902 RVA: 0x000CDE82 File Offset: 0x000CC082
		public override void SetLayoutHorizontal()
		{
			this.SetLayoutAlongAxis(0);
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x000CDE8B File Offset: 0x000CC08B
		public override void SetLayoutVertical()
		{
			this.SetLayoutAlongAxis(1);
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x000CDE94 File Offset: 0x000CC094
		private void SetLayoutAlongAxis(int axis)
		{
			if (base.rectChildren.Count > 0)
			{
				float num = 6.2831855f / (float)base.rectChildren.Count;
				float num2 = this.offset * 0.017453292f;
				foreach (RectTransform rect in base.rectChildren)
				{
					base.SetChildAlongAxis(rect, axis, ((axis == 0) ? Mathf.Cos(num2) : Mathf.Sin(num2)) * this.radius, base.rectTransform.rect.size[axis]);
					num2 += num;
				}
			}
		}

		// Token: 0x040033CF RID: 13263
		[Tooltip("Width/height of the layout child.")]
		public Vector2 childSize;

		// Token: 0x040033D0 RID: 13264
		[Tooltip("Radius to spread the layout children from the center.")]
		public float radius;

		// Token: 0x040033D1 RID: 13265
		[Tooltip("Angular offset, in degrees.")]
		public float offset;
	}
}
