using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200059B RID: 1435
[RequireComponent(typeof(Image))]
public class ImageCycler : MonoBehaviour
{
	// Token: 0x06001DD1 RID: 7633 RVA: 0x000719E3 File Offset: 0x0006FBE3
	public void Awake()
	{
		this.img = base.GetComponent<Image>();
		if (this.sprites != null)
		{
			this.SetSprites(this.sprites);
		}
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x00071A05 File Offset: 0x0006FC05
	public void OnEnable()
	{
		this.flipTime = Time.time;
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x00071A12 File Offset: 0x0006FC12
	public void SetSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		if (sprites.Length != 0)
		{
			this.idx = 0;
			this.img.sprite = sprites[this.idx];
		}
		this.flipTime = Time.time + this.timePerFrame;
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x00071A4C File Offset: 0x0006FC4C
	public void Update()
	{
		if (this.sprites.Length >= 2 && Time.time > this.flipTime)
		{
			this.idx = (this.idx + 1) % this.sprites.Length;
			this.img.sprite = this.sprites[this.idx];
			this.flipTime += this.timePerFrame;
		}
	}

	// Token: 0x04001CFB RID: 7419
	public Sprite[] sprites;

	// Token: 0x04001CFC RID: 7420
	[Tooltip("Seconds between frames.")]
	public float timePerFrame = 1f;

	// Token: 0x04001CFD RID: 7421
	private Image img;

	// Token: 0x04001CFE RID: 7422
	private float flipTime;

	// Token: 0x04001CFF RID: 7423
	private int idx;
}
