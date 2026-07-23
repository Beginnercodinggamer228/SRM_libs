using System;

namespace Gif.Components
{
	// Token: 0x02000A18 RID: 2584
	public class NeuQuant
	{
		// Token: 0x060045AA RID: 17834 RVA: 0x000CCEE4 File Offset: 0x000CB0E4
		public NeuQuant(byte[] thepic, int len, int sample)
		{
			this.thepicture = thepic;
			this.lengthcount = len;
			this.samplefac = sample;
			this.network = new int[NeuQuant.netsize][];
			for (int i = 0; i < NeuQuant.netsize; i++)
			{
				this.network[i] = new int[4];
				int[] array = this.network[i];
				array[0] = (array[1] = (array[2] = (i << NeuQuant.netbiasshift + 8) / NeuQuant.netsize));
				this.freq[i] = NeuQuant.intbias / NeuQuant.netsize;
				this.bias[i] = 0;
			}
		}

		// Token: 0x060045AB RID: 17835 RVA: 0x000CCFC0 File Offset: 0x000CB1C0
		public byte[] ColorMap()
		{
			byte[] array = new byte[3 * NeuQuant.netsize];
			int[] array2 = new int[NeuQuant.netsize];
			for (int i = 0; i < NeuQuant.netsize; i++)
			{
				array2[this.network[i][3]] = i;
			}
			int num = 0;
			for (int j = 0; j < NeuQuant.netsize; j++)
			{
				int num2 = array2[j];
				array[num++] = (byte)this.network[num2][0];
				array[num++] = (byte)this.network[num2][1];
				array[num++] = (byte)this.network[num2][2];
			}
			return array;
		}

		// Token: 0x060045AC RID: 17836 RVA: 0x000CD058 File Offset: 0x000CB258
		public void Inxbuild()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < NeuQuant.netsize; i++)
			{
				int[] array = this.network[i];
				int num3 = i;
				int num4 = array[1];
				int[] array2;
				for (int j = i + 1; j < NeuQuant.netsize; j++)
				{
					array2 = this.network[j];
					if (array2[1] < num4)
					{
						num3 = j;
						num4 = array2[1];
					}
				}
				array2 = this.network[num3];
				if (i != num3)
				{
					int j = array2[0];
					array2[0] = array[0];
					array[0] = j;
					j = array2[1];
					array2[1] = array[1];
					array[1] = j;
					j = array2[2];
					array2[2] = array[2];
					array[2] = j;
					j = array2[3];
					array2[3] = array[3];
					array[3] = j;
				}
				if (num4 != num)
				{
					this.netindex[num] = num2 + i >> 1;
					for (int j = num + 1; j < num4; j++)
					{
						this.netindex[j] = i;
					}
					num = num4;
					num2 = i;
				}
			}
			this.netindex[num] = num2 + NeuQuant.maxnetpos >> 1;
			for (int j = num + 1; j < 256; j++)
			{
				this.netindex[j] = NeuQuant.maxnetpos;
			}
		}

		// Token: 0x060045AD RID: 17837 RVA: 0x000CD178 File Offset: 0x000CB378
		public void Learn()
		{
			if (this.lengthcount < NeuQuant.minpicturebytes)
			{
				this.samplefac = 1;
			}
			this.alphadec = 30 + (this.samplefac - 1) / 3;
			byte[] array = this.thepicture;
			int num = 0;
			int num2 = this.lengthcount;
			int num3 = this.lengthcount / (3 * this.samplefac);
			int num4 = num3 / NeuQuant.ncycles;
			int num5 = NeuQuant.initalpha;
			int num6 = NeuQuant.initradius;
			int num7 = num6 >> NeuQuant.radiusbiasshift;
			if (num7 <= 1)
			{
				num7 = 0;
			}
			int i;
			for (i = 0; i < num7; i++)
			{
				this.radpower[i] = num5 * ((num7 * num7 - i * i) * NeuQuant.radbias / (num7 * num7));
			}
			int num8;
			if (this.lengthcount < NeuQuant.minpicturebytes)
			{
				num8 = 3;
			}
			else if (this.lengthcount % NeuQuant.prime1 != 0)
			{
				num8 = 3 * NeuQuant.prime1;
			}
			else if (this.lengthcount % NeuQuant.prime2 != 0)
			{
				num8 = 3 * NeuQuant.prime2;
			}
			else if (this.lengthcount % NeuQuant.prime3 != 0)
			{
				num8 = 3 * NeuQuant.prime3;
			}
			else
			{
				num8 = 3 * NeuQuant.prime4;
			}
			i = 0;
			while (i < num3)
			{
				int b = (int)(array[num] & byte.MaxValue) << NeuQuant.netbiasshift;
				int g = (int)(array[num + 1] & byte.MaxValue) << NeuQuant.netbiasshift;
				int r = (int)(array[num + 2] & byte.MaxValue) << NeuQuant.netbiasshift;
				int j = this.Contest(b, g, r);
				this.Altersingle(num5, j, b, g, r);
				if (num7 != 0)
				{
					this.Alterneigh(num7, j, b, g, r);
				}
				num += num8;
				if (num >= num2)
				{
					num -= this.lengthcount;
				}
				i++;
				if (num4 == 0)
				{
					num4 = 1;
				}
				if (i % num4 == 0)
				{
					num5 -= num5 / this.alphadec;
					num6 -= num6 / NeuQuant.radiusdec;
					num7 = num6 >> NeuQuant.radiusbiasshift;
					if (num7 <= 1)
					{
						num7 = 0;
					}
					for (j = 0; j < num7; j++)
					{
						this.radpower[j] = num5 * ((num7 * num7 - j * j) * NeuQuant.radbias / (num7 * num7));
					}
				}
			}
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x000CD394 File Offset: 0x000CB594
		public int Map(int b, int g, int r)
		{
			int num = 1000;
			int result = -1;
			int num2 = this.netindex[g];
			int num3 = num2 - 1;
			while (num2 < NeuQuant.netsize || num3 >= 0)
			{
				if (num2 < NeuQuant.netsize)
				{
					int[] array = this.network[num2];
					int num4 = array[1] - g;
					if (num4 >= num)
					{
						num2 = NeuQuant.netsize;
					}
					else
					{
						num2++;
						if (num4 < 0)
						{
							num4 = -num4;
						}
						int num5 = array[0] - b;
						if (num5 < 0)
						{
							num5 = -num5;
						}
						num4 += num5;
						if (num4 < num)
						{
							num5 = array[2] - r;
							if (num5 < 0)
							{
								num5 = -num5;
							}
							num4 += num5;
							if (num4 < num)
							{
								num = num4;
								result = array[3];
							}
						}
					}
				}
				if (num3 >= 0)
				{
					int[] array = this.network[num3];
					int num4 = g - array[1];
					if (num4 >= num)
					{
						num3 = -1;
					}
					else
					{
						num3--;
						if (num4 < 0)
						{
							num4 = -num4;
						}
						int num5 = array[0] - b;
						if (num5 < 0)
						{
							num5 = -num5;
						}
						num4 += num5;
						if (num4 < num)
						{
							num5 = array[2] - r;
							if (num5 < 0)
							{
								num5 = -num5;
							}
							num4 += num5;
							if (num4 < num)
							{
								num = num4;
								result = array[3];
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x000CD499 File Offset: 0x000CB699
		public byte[] Process()
		{
			this.Learn();
			this.Unbiasnet();
			this.Inxbuild();
			return this.ColorMap();
		}

		// Token: 0x060045B0 RID: 17840 RVA: 0x000CD4B4 File Offset: 0x000CB6B4
		public void Unbiasnet()
		{
			for (int i = 0; i < NeuQuant.netsize; i++)
			{
				this.network[i][0] >>= NeuQuant.netbiasshift;
				this.network[i][1] >>= NeuQuant.netbiasshift;
				this.network[i][2] >>= NeuQuant.netbiasshift;
				this.network[i][3] = i;
			}
		}

		// Token: 0x060045B1 RID: 17841 RVA: 0x000CD52C File Offset: 0x000CB72C
		protected void Alterneigh(int rad, int i, int b, int g, int r)
		{
			int num = i - rad;
			if (num < -1)
			{
				num = -1;
			}
			int num2 = i + rad;
			if (num2 > NeuQuant.netsize)
			{
				num2 = NeuQuant.netsize;
			}
			int num3 = i + 1;
			int num4 = i - 1;
			int num5 = 1;
			while (num3 < num2 || num4 > num)
			{
				int num6 = this.radpower[num5++];
				if (num3 < num2)
				{
					int[] array = this.network[num3++];
					try
					{
						array[0] -= num6 * (array[0] - b) / NeuQuant.alpharadbias;
						array[1] -= num6 * (array[1] - g) / NeuQuant.alpharadbias;
						array[2] -= num6 * (array[2] - r) / NeuQuant.alpharadbias;
					}
					catch (Exception)
					{
					}
				}
				if (num4 > num)
				{
					int[] array = this.network[num4--];
					try
					{
						array[0] -= num6 * (array[0] - b) / NeuQuant.alpharadbias;
						array[1] -= num6 * (array[1] - g) / NeuQuant.alpharadbias;
						array[2] -= num6 * (array[2] - r) / NeuQuant.alpharadbias;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x060045B2 RID: 17842 RVA: 0x000CD674 File Offset: 0x000CB874
		protected void Altersingle(int alpha, int i, int b, int g, int r)
		{
			int[] array = this.network[i];
			array[0] -= alpha * (array[0] - b) / NeuQuant.initalpha;
			array[1] -= alpha * (array[1] - g) / NeuQuant.initalpha;
			array[2] -= alpha * (array[2] - r) / NeuQuant.initalpha;
		}

		// Token: 0x060045B3 RID: 17843 RVA: 0x000CD6D4 File Offset: 0x000CB8D4
		protected int Contest(int b, int g, int r)
		{
			int num = int.MaxValue;
			int num2 = num;
			int num3 = -1;
			int result = num3;
			for (int i = 0; i < NeuQuant.netsize; i++)
			{
				int[] array = this.network[i];
				int num4 = array[0] - b;
				if (num4 < 0)
				{
					num4 = -num4;
				}
				int num5 = array[1] - g;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				num5 = array[2] - r;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				if (num4 < num)
				{
					num = num4;
					num3 = i;
				}
				int num6 = num4 - (this.bias[i] >> NeuQuant.intbiasshift - NeuQuant.netbiasshift);
				if (num6 < num2)
				{
					num2 = num6;
					result = i;
				}
				int num7 = this.freq[i] >> NeuQuant.betashift;
				this.freq[i] -= num7;
				this.bias[i] += num7 << NeuQuant.gammashift;
			}
			this.freq[num3] += NeuQuant.beta;
			this.bias[num3] -= NeuQuant.betagamma;
			return result;
		}

		// Token: 0x04003398 RID: 13208
		protected static readonly int netsize = 128;

		// Token: 0x04003399 RID: 13209
		protected static readonly int prime1 = 499;

		// Token: 0x0400339A RID: 13210
		protected static readonly int prime2 = 491;

		// Token: 0x0400339B RID: 13211
		protected static readonly int prime3 = 487;

		// Token: 0x0400339C RID: 13212
		protected static readonly int prime4 = 503;

		// Token: 0x0400339D RID: 13213
		protected static readonly int minpicturebytes = 3 * NeuQuant.prime4;

		// Token: 0x0400339E RID: 13214
		protected static readonly int maxnetpos = NeuQuant.netsize - 1;

		// Token: 0x0400339F RID: 13215
		protected static readonly int netbiasshift = 4;

		// Token: 0x040033A0 RID: 13216
		protected static readonly int ncycles = 100;

		// Token: 0x040033A1 RID: 13217
		protected static readonly int intbiasshift = 16;

		// Token: 0x040033A2 RID: 13218
		protected static readonly int intbias = 1 << NeuQuant.intbiasshift;

		// Token: 0x040033A3 RID: 13219
		protected static readonly int gammashift = 10;

		// Token: 0x040033A4 RID: 13220
		protected static readonly int gamma = 1 << NeuQuant.gammashift;

		// Token: 0x040033A5 RID: 13221
		protected static readonly int betashift = 10;

		// Token: 0x040033A6 RID: 13222
		protected static readonly int beta = NeuQuant.intbias >> NeuQuant.betashift;

		// Token: 0x040033A7 RID: 13223
		protected static readonly int betagamma = NeuQuant.intbias << NeuQuant.gammashift - NeuQuant.betashift;

		// Token: 0x040033A8 RID: 13224
		protected static readonly int initrad = NeuQuant.netsize >> 3;

		// Token: 0x040033A9 RID: 13225
		protected static readonly int radiusbiasshift = 6;

		// Token: 0x040033AA RID: 13226
		protected static readonly int radiusbias = 1 << NeuQuant.radiusbiasshift;

		// Token: 0x040033AB RID: 13227
		protected static readonly int initradius = NeuQuant.initrad * NeuQuant.radiusbias;

		// Token: 0x040033AC RID: 13228
		protected static readonly int radiusdec = 30;

		// Token: 0x040033AD RID: 13229
		protected static readonly int alphabiasshift = 10;

		// Token: 0x040033AE RID: 13230
		protected static readonly int initalpha = 1 << NeuQuant.alphabiasshift;

		// Token: 0x040033AF RID: 13231
		protected int alphadec;

		// Token: 0x040033B0 RID: 13232
		protected static readonly int radbiasshift = 8;

		// Token: 0x040033B1 RID: 13233
		protected static readonly int radbias = 1 << NeuQuant.radbiasshift;

		// Token: 0x040033B2 RID: 13234
		protected static readonly int alpharadbshift = NeuQuant.alphabiasshift + NeuQuant.radbiasshift;

		// Token: 0x040033B3 RID: 13235
		protected static readonly int alpharadbias = 1 << NeuQuant.alpharadbshift;

		// Token: 0x040033B4 RID: 13236
		protected byte[] thepicture;

		// Token: 0x040033B5 RID: 13237
		protected int lengthcount;

		// Token: 0x040033B6 RID: 13238
		protected int samplefac;

		// Token: 0x040033B7 RID: 13239
		protected int[][] network;

		// Token: 0x040033B8 RID: 13240
		protected int[] netindex = new int[256];

		// Token: 0x040033B9 RID: 13241
		protected int[] bias = new int[NeuQuant.netsize];

		// Token: 0x040033BA RID: 13242
		protected int[] freq = new int[NeuQuant.netsize];

		// Token: 0x040033BB RID: 13243
		protected int[] radpower = new int[NeuQuant.initrad];
	}
}
