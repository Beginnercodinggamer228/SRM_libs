using System;

namespace Noise
{
	// Token: 0x02000A13 RID: 2579
	public class NoiseGen
	{
		// Token: 0x1700035D RID: 861
		// (set) Token: 0x0600457C RID: 17788 RVA: 0x000CB89C File Offset: 0x000C9A9C
		public double Scale
		{
			set
			{
				this.XScale = value;
				this.YScale = value;
			}
		}

		// Token: 0x0600457D RID: 17789 RVA: 0x000CB8AC File Offset: 0x000C9AAC
		public NoiseGen()
		{
		}

		// Token: 0x0600457E RID: 17790 RVA: 0x000CB8E8 File Offset: 0x000C9AE8
		public NoiseGen(double pScale, byte pOctaves)
		{
			this.XScale = pScale;
			this.YScale = pScale;
			this.Octaves = pOctaves;
		}

		// Token: 0x0600457F RID: 17791 RVA: 0x000CB944 File Offset: 0x000C9B44
		public NoiseGen(double pXScale, double pYScale, byte pOctaves)
		{
			this.XScale = pXScale;
			this.YScale = pYScale;
			this.Octaves = pOctaves;
		}

		// Token: 0x06004580 RID: 17792 RVA: 0x000CB9A0 File Offset: 0x000C9BA0
		public float GetNoise(double x, double y, double z)
		{
			if (this.Octaves > 1)
			{
				return Noise.GetOctaveNoise(x * this.XScale, y * this.YScale, z * this.ZScale, (int)this.Octaves);
			}
			return Noise.GetNoise(x * this.XScale, y * this.YScale, z * this.ZScale);
		}

		// Token: 0x0400335D RID: 13149
		public double XScale = 0.02;

		// Token: 0x0400335E RID: 13150
		public double YScale = 0.02;

		// Token: 0x0400335F RID: 13151
		public double ZScale = 1.0;

		// Token: 0x04003360 RID: 13152
		public byte Octaves = 1;
	}
}
