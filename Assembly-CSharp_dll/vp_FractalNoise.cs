using System;
using UnityEngine;

// Token: 0x02000831 RID: 2097
public class vp_FractalNoise
{
	// Token: 0x06002BF8 RID: 11256 RVA: 0x000A5CA2 File Offset: 0x000A3EA2
	public vp_FractalNoise(float inH, float inLacunarity, float inOctaves) : this(inH, inLacunarity, inOctaves, null)
	{
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000A5CB0 File Offset: 0x000A3EB0
	public vp_FractalNoise(float inH, float inLacunarity, float inOctaves, vp_Perlin noise)
	{
		this.m_Lacunarity = inLacunarity;
		this.m_Octaves = inOctaves;
		this.m_IntOctaves = (int)inOctaves;
		this.m_Exponent = new float[this.m_IntOctaves + 1];
		float num = 1f;
		for (int i = 0; i < this.m_IntOctaves + 1; i++)
		{
			this.m_Exponent[i] = (float)Math.Pow((double)this.m_Lacunarity, (double)(-(double)inH));
			num *= this.m_Lacunarity;
		}
		if (noise == null)
		{
			this.m_Noise = new vp_Perlin();
			return;
		}
		this.m_Noise = noise;
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x000A5D40 File Offset: 0x000A3F40
	public float HybridMultifractal(float x, float y, float offset)
	{
		float num = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[0];
		float num2 = num;
		x *= this.m_Lacunarity;
		y *= this.m_Lacunarity;
		int i;
		for (i = 1; i < this.m_IntOctaves; i++)
		{
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			float num3 = (this.m_Noise.Noise(x, y) + offset) * this.m_Exponent[i];
			num += num2 * num3;
			num2 *= num3;
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
		}
		float num4 = this.m_Octaves - (float)this.m_IntOctaves;
		return num + num4 * this.m_Noise.Noise(x, y) * this.m_Exponent[i];
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x000A5E04 File Offset: 0x000A4004
	public float RidgedMultifractal(float x, float y, float offset, float gain)
	{
		float num = Mathf.Abs(this.m_Noise.Noise(x, y));
		num = offset - num;
		num *= num;
		float num2 = num;
		for (int i = 1; i < this.m_IntOctaves; i++)
		{
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
			float num3 = num * gain;
			num3 = Mathf.Clamp01(num3);
			num = Mathf.Abs(this.m_Noise.Noise(x, y));
			num = offset - num;
			num *= num;
			num *= num3;
			num2 += num * this.m_Exponent[i];
		}
		return num2;
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000A5E94 File Offset: 0x000A4094
	public float BrownianMotion(float x, float y)
	{
		float num = 0f;
		long num2;
		for (num2 = 0L; num2 < (long)this.m_IntOctaves; num2 += 1L)
		{
			num = this.m_Noise.Noise(x, y) * this.m_Exponent[(int)(checked((IntPtr)num2))];
			x *= this.m_Lacunarity;
			y *= this.m_Lacunarity;
		}
		float num3 = this.m_Octaves - (float)this.m_IntOctaves;
		return num + num3 * this.m_Noise.Noise(x, y) * this.m_Exponent[(int)(checked((IntPtr)num2))];
	}

	// Token: 0x04002A2A RID: 10794
	private vp_Perlin m_Noise;

	// Token: 0x04002A2B RID: 10795
	private float[] m_Exponent;

	// Token: 0x04002A2C RID: 10796
	private int m_IntOctaves;

	// Token: 0x04002A2D RID: 10797
	private float m_Octaves;

	// Token: 0x04002A2E RID: 10798
	private float m_Lacunarity;
}
