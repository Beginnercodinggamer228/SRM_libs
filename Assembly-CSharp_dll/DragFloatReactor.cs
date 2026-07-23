using System;
using UnityEngine;

// Token: 0x020006EC RID: 1772
[RequireComponent(typeof(Rigidbody))]
public class DragFloatReactor : SRBehaviour, FloatingReactor
{
	// Token: 0x060024F9 RID: 9465 RVA: 0x0008E17C File Offset: 0x0008C37C
	public void Awake()
	{
		this.body = base.GetComponent<Rigidbody>();
		this.normDrag = this.body.drag;
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x0008E19B File Offset: 0x0008C39B
	public void SetIsFloating(bool isFloating)
	{
		this.body.drag = (isFloating ? this.floatDragMultiplier : 1f) * this.normDrag;
		this.isFloating = isFloating;
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x0008E1C6 File Offset: 0x0008C3C6
	public bool GetIsFloating()
	{
		return this.isFloating;
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x0008E1D0 File Offset: 0x0008C3D0
	public static bool IsFloating(GameObject target)
	{
		DragFloatReactor component = target.GetComponent<DragFloatReactor>();
		return component != null && component.GetIsFloating();
	}

	// Token: 0x040023DB RID: 9179
	[Tooltip("The factor by which to increase the drag while we're floating.")]
	public float floatDragMultiplier = 25f;

	// Token: 0x040023DC RID: 9180
	private float normDrag;

	// Token: 0x040023DD RID: 9181
	private Rigidbody body;

	// Token: 0x040023DE RID: 9182
	private bool isFloating;
}
