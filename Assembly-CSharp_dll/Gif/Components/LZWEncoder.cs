using System;
using System.IO;

namespace Gif.Components
{
	// Token: 0x02000A17 RID: 2583
	public class LZWEncoder
	{
		// Token: 0x0600459F RID: 17823 RVA: 0x000CC99C File Offset: 0x000CAB9C
		public LZWEncoder(int width, int height, byte[] pixels, int color_depth)
		{
			this.imgW = width;
			this.imgH = height;
			this.pixAry = pixels;
			this.initCodeSize = Math.Max(2, color_depth);
		}

		// Token: 0x060045A0 RID: 17824 RVA: 0x000CCA40 File Offset: 0x000CAC40
		private void Add(byte c, Stream outs)
		{
			byte[] array = this.accum;
			int num = this.a_count;
			this.a_count = num + 1;
			array[num] = c;
			if (this.a_count >= 254)
			{
				this.Flush(outs);
			}
		}

		// Token: 0x060045A1 RID: 17825 RVA: 0x000CCA7A File Offset: 0x000CAC7A
		private void ClearTable(Stream outs)
		{
			this.ResetCodeTable(this.hsize);
			this.free_ent = this.ClearCode + 2;
			this.clear_flg = true;
			this.Output(this.ClearCode, outs);
		}

		// Token: 0x060045A2 RID: 17826 RVA: 0x000CCAAC File Offset: 0x000CACAC
		private void ResetCodeTable(int hsize)
		{
			for (int i = 0; i < hsize; i++)
			{
				this.htab[i] = -1;
			}
		}

		// Token: 0x060045A3 RID: 17827 RVA: 0x000CCAD0 File Offset: 0x000CACD0
		private void Compress(int init_bits, Stream outs)
		{
			this.g_init_bits = init_bits;
			this.clear_flg = false;
			this.n_bits = this.g_init_bits;
			this.maxcode = this.MaxCode(this.n_bits);
			this.ClearCode = 1 << init_bits - 1;
			this.EOFCode = this.ClearCode + 1;
			this.free_ent = this.ClearCode + 2;
			this.a_count = 0;
			int num = this.NextPixel();
			int num2 = 0;
			for (int i = this.hsize; i < 65536; i *= 2)
			{
				num2++;
			}
			num2 = 8 - num2;
			int num3 = this.hsize;
			this.ResetCodeTable(num3);
			this.Output(this.ClearCode, outs);
			int num4;
			while ((num4 = this.NextPixel()) != LZWEncoder.EOF)
			{
				int i = (num4 << this.maxbits) + num;
				int num5 = num4 << num2 ^ num;
				if (this.htab[num5] == i)
				{
					num = this.codetab[num5];
				}
				else
				{
					if (this.htab[num5] >= 0)
					{
						int num6 = num3 - num5;
						if (num5 == 0)
						{
							num6 = 1;
						}
						for (;;)
						{
							if ((num5 -= num6) < 0)
							{
								num5 += num3;
							}
							if (this.htab[num5] == i)
							{
								break;
							}
							if (this.htab[num5] < 0)
							{
								goto IL_121;
							}
						}
						num = this.codetab[num5];
						continue;
					}
					IL_121:
					this.Output(num, outs);
					num = num4;
					if (this.free_ent < this.maxmaxcode)
					{
						int[] array = this.codetab;
						int num7 = num5;
						int num8 = this.free_ent;
						this.free_ent = num8 + 1;
						array[num7] = num8;
						this.htab[num5] = i;
					}
					else
					{
						this.ClearTable(outs);
					}
				}
			}
			this.Output(num, outs);
			this.Output(this.EOFCode, outs);
		}

		// Token: 0x060045A4 RID: 17828 RVA: 0x000CCC6C File Offset: 0x000CAE6C
		public void Encode(Stream os)
		{
			os.WriteByte(Convert.ToByte(this.initCodeSize));
			this.remaining = this.imgW * this.imgH;
			this.curPixel = 0;
			this.Compress(this.initCodeSize + 1, os);
			os.WriteByte(0);
		}

		// Token: 0x060045A5 RID: 17829 RVA: 0x000CCCBA File Offset: 0x000CAEBA
		private void Flush(Stream outs)
		{
			if (this.a_count > 0)
			{
				outs.WriteByte(Convert.ToByte(this.a_count));
				outs.Write(this.accum, 0, this.a_count);
				this.a_count = 0;
			}
		}

		// Token: 0x060045A6 RID: 17830 RVA: 0x000CCCF0 File Offset: 0x000CAEF0
		private int MaxCode(int n_bits)
		{
			return (1 << n_bits) - 1;
		}

		// Token: 0x060045A7 RID: 17831 RVA: 0x000CCCFC File Offset: 0x000CAEFC
		private int NextPixel()
		{
			if (this.remaining == 0)
			{
				return LZWEncoder.EOF;
			}
			this.remaining--;
			if (this.curPixel + 1 < this.pixAry.GetUpperBound(0))
			{
				byte[] array = this.pixAry;
				int num = this.curPixel;
				this.curPixel = num + 1;
				return array[num] & 255;
			}
			return 255;
		}

		// Token: 0x060045A8 RID: 17832 RVA: 0x000CCD60 File Offset: 0x000CAF60
		private void Output(int code, Stream outs)
		{
			this.cur_accum &= this.masks[this.cur_bits];
			if (this.cur_bits > 0)
			{
				this.cur_accum |= code << this.cur_bits;
			}
			else
			{
				this.cur_accum = code;
			}
			this.cur_bits += this.n_bits;
			while (this.cur_bits >= 8)
			{
				this.Add((byte)(this.cur_accum & 255), outs);
				this.cur_accum >>= 8;
				this.cur_bits -= 8;
			}
			if (this.free_ent > this.maxcode || this.clear_flg)
			{
				if (this.clear_flg)
				{
					this.maxcode = this.MaxCode(this.n_bits = this.g_init_bits);
					this.clear_flg = false;
				}
				else
				{
					this.n_bits++;
					if (this.n_bits == this.maxbits)
					{
						this.maxcode = this.maxmaxcode;
					}
					else
					{
						this.maxcode = this.MaxCode(this.n_bits);
					}
				}
			}
			if (code == this.EOFCode)
			{
				while (this.cur_bits > 0)
				{
					this.Add((byte)(this.cur_accum & 255), outs);
					this.cur_accum >>= 8;
					this.cur_bits -= 8;
				}
				this.Flush(outs);
			}
		}

		// Token: 0x0400337E RID: 13182
		private static readonly int EOF = -1;

		// Token: 0x0400337F RID: 13183
		private int imgW;

		// Token: 0x04003380 RID: 13184
		private int imgH;

		// Token: 0x04003381 RID: 13185
		private byte[] pixAry;

		// Token: 0x04003382 RID: 13186
		private int initCodeSize;

		// Token: 0x04003383 RID: 13187
		private int remaining;

		// Token: 0x04003384 RID: 13188
		private int curPixel;

		// Token: 0x04003385 RID: 13189
		private static readonly int BITS = 12;

		// Token: 0x04003386 RID: 13190
		private static readonly int HSIZE = 5003;

		// Token: 0x04003387 RID: 13191
		private int n_bits;

		// Token: 0x04003388 RID: 13192
		private int maxbits = LZWEncoder.BITS;

		// Token: 0x04003389 RID: 13193
		private int maxcode;

		// Token: 0x0400338A RID: 13194
		private int maxmaxcode = 1 << LZWEncoder.BITS;

		// Token: 0x0400338B RID: 13195
		private int[] htab = new int[LZWEncoder.HSIZE];

		// Token: 0x0400338C RID: 13196
		private int[] codetab = new int[LZWEncoder.HSIZE];

		// Token: 0x0400338D RID: 13197
		private int hsize = LZWEncoder.HSIZE;

		// Token: 0x0400338E RID: 13198
		private int free_ent;

		// Token: 0x0400338F RID: 13199
		private bool clear_flg;

		// Token: 0x04003390 RID: 13200
		private int g_init_bits;

		// Token: 0x04003391 RID: 13201
		private int ClearCode;

		// Token: 0x04003392 RID: 13202
		private int EOFCode;

		// Token: 0x04003393 RID: 13203
		private int cur_accum;

		// Token: 0x04003394 RID: 13204
		private int cur_bits;

		// Token: 0x04003395 RID: 13205
		private int[] masks = new int[]
		{
			0,
			1,
			3,
			7,
			15,
			31,
			63,
			127,
			255,
			511,
			1023,
			2047,
			4095,
			8191,
			16383,
			32767,
			65535
		};

		// Token: 0x04003396 RID: 13206
		private int a_count;

		// Token: 0x04003397 RID: 13207
		private byte[] accum = new byte[256];
	}
}
