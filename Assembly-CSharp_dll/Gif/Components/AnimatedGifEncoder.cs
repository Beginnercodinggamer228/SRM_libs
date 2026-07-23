using System;
using System.IO;
using UnityEngine;

namespace Gif.Components
{
	// Token: 0x02000A16 RID: 2582
	public class AnimatedGifEncoder
	{
		// Token: 0x06004588 RID: 17800 RVA: 0x000CC187 File Offset: 0x000CA387
		public void SetDelay(int ms)
		{
			this.delay = (int)Math.Round((double)((float)ms / 10f));
		}

		// Token: 0x06004589 RID: 17801 RVA: 0x000CC19E File Offset: 0x000CA39E
		public void SetDispose(int code)
		{
			if (code >= 0)
			{
				this.dispose = code;
			}
		}

		// Token: 0x0600458A RID: 17802 RVA: 0x000CC1AB File Offset: 0x000CA3AB
		public void SetRepeat(int iter)
		{
			if (iter >= 0)
			{
				this.repeat = iter;
			}
		}

		// Token: 0x0600458B RID: 17803 RVA: 0x000CC1B8 File Offset: 0x000CA3B8
		public void SetTransparent(Color32 c)
		{
			this.transparent = c;
		}

		// Token: 0x0600458C RID: 17804 RVA: 0x000CC1C4 File Offset: 0x000CA3C4
		public bool AddFrame(Color32[] pixels, int imgWidth, int imgHeight)
		{
			if (pixels == null || !this.started)
			{
				return false;
			}
			bool result = true;
			try
			{
				if (!this.sizeSet)
				{
					this.SetSize(imgWidth, imgHeight);
				}
				this.pixels = this.ConvertToBytePixels(pixels);
				this.AnalyzePixels();
				if (this.firstFrame)
				{
					this.WriteLSD();
					this.WritePalette();
					if (this.repeat >= 0)
					{
						this.WriteNetscapeExt();
					}
				}
				this.WriteGraphicCtrlExt();
				this.WriteImageDesc();
				if (!this.firstFrame)
				{
					this.WritePalette();
				}
				this.WritePixels();
				this.firstFrame = false;
			}
			catch (IOException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600458D RID: 17805 RVA: 0x000CC268 File Offset: 0x000CA468
		private byte[] ConvertToBytePixels(Color32[] intPixels)
		{
			byte[] array = new byte[intPixels.Length * 3];
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					int num = (j + i * this.width) * 3;
					int num2 = j + (this.height - i - 1) * this.width;
					array[num] = intPixels[num2].r;
					array[num + 1] = intPixels[num2].g;
					array[num + 2] = intPixels[num2].b;
				}
			}
			return array;
		}

		// Token: 0x0600458E RID: 17806 RVA: 0x000CC2FC File Offset: 0x000CA4FC
		public bool Finish()
		{
			if (!this.started)
			{
				return false;
			}
			bool result = true;
			this.started = false;
			try
			{
				this.fs.WriteByte(59);
				this.fs.Flush();
				if (this.closeStream)
				{
					this.fs.Close();
				}
			}
			catch (IOException)
			{
				result = false;
			}
			this.transIndex = 0;
			this.fs = null;
			this.pixels = null;
			this.indexedPixels = null;
			this.colorTab = null;
			this.closeStream = false;
			this.firstFrame = true;
			return result;
		}

		// Token: 0x0600458F RID: 17807 RVA: 0x000CC390 File Offset: 0x000CA590
		public void SetFrameRate(float fps)
		{
			if (fps != 0f)
			{
				this.delay = (int)Math.Round((double)(100f / fps));
			}
		}

		// Token: 0x06004590 RID: 17808 RVA: 0x000CC3AE File Offset: 0x000CA5AE
		public void SetQuality(int quality)
		{
			if (quality < 1)
			{
				quality = 1;
			}
			this.sample = quality;
		}

		// Token: 0x06004591 RID: 17809 RVA: 0x000CC3C0 File Offset: 0x000CA5C0
		public void SetSize(int w, int h)
		{
			if (this.started && !this.firstFrame)
			{
				return;
			}
			this.width = w;
			this.height = h;
			if (this.width < 1)
			{
				this.width = 320;
			}
			if (this.height < 1)
			{
				this.height = 240;
			}
			this.sizeSet = true;
		}

		// Token: 0x06004592 RID: 17810 RVA: 0x000CC41C File Offset: 0x000CA61C
		public bool Start(FileStream os)
		{
			if (os == null)
			{
				return false;
			}
			bool flag = true;
			this.closeStream = false;
			this.fs = os;
			try
			{
				this.WriteString("GIF89a");
			}
			catch (IOException)
			{
				flag = false;
			}
			return this.started = flag;
		}

		// Token: 0x06004593 RID: 17811 RVA: 0x000CC46C File Offset: 0x000CA66C
		public bool Start(string file)
		{
			bool flag = true;
			try
			{
				this.fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				flag = this.Start(this.fs);
				this.closeStream = true;
			}
			catch (IOException)
			{
				flag = false;
			}
			return this.started = flag;
		}

		// Token: 0x06004594 RID: 17812 RVA: 0x000CC4C0 File Offset: 0x000CA6C0
		protected void AnalyzePixels()
		{
			int num = this.pixels.Length;
			int num2 = num / 3;
			this.indexedPixels = new byte[num2];
			NeuQuant neuQuant = new NeuQuant(this.pixels, num, this.sample);
			this.colorTab = neuQuant.Process();
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num4 = neuQuant.Map((int)(this.pixels[num3++] & byte.MaxValue), (int)(this.pixels[num3++] & byte.MaxValue), (int)(this.pixels[num3++] & byte.MaxValue));
				this.usedEntry[num4] = true;
				this.indexedPixels[i] = (byte)num4;
			}
			this.pixels = null;
			this.colorDepth = 8;
			this.palSize = 7;
			if (this.transparent != Color.clear)
			{
				this.transIndex = this.FindClosest(this.transparent);
			}
		}

		// Token: 0x06004595 RID: 17813 RVA: 0x000CC5AC File Offset: 0x000CA7AC
		protected int FindClosest(Color32 c)
		{
			if (this.colorTab == null)
			{
				return -1;
			}
			int r = (int)c.r;
			int g = (int)c.g;
			int b = (int)c.b;
			int result = 0;
			int num = 16777216;
			int num2 = this.colorTab.Length;
			for (int i = 0; i < num2; i++)
			{
				int num3 = r - (int)(this.colorTab[i++] & byte.MaxValue);
				int num4 = g - (int)(this.colorTab[i++] & byte.MaxValue);
				int num5 = b - (int)(this.colorTab[i] & byte.MaxValue);
				int num6 = num3 * num3 + num4 * num4 + num5 * num5;
				int num7 = i / 3;
				if (this.usedEntry[num7] && num6 < num)
				{
					num = num6;
					result = num7;
				}
			}
			return result;
		}

		// Token: 0x06004596 RID: 17814 RVA: 0x000CC66C File Offset: 0x000CA86C
		protected void WriteGraphicCtrlExt()
		{
			this.fs.WriteByte(33);
			this.fs.WriteByte(249);
			this.fs.WriteByte(4);
			int num;
			int num2;
			if (this.transparent == Color.clear)
			{
				num = 0;
				num2 = 0;
			}
			else
			{
				num = 1;
				num2 = 2;
			}
			if (this.dispose >= 0)
			{
				num2 = (this.dispose & 7);
			}
			num2 <<= 2;
			this.fs.WriteByte(Convert.ToByte(0 | num2 | 0 | num));
			this.WriteShort(this.delay);
			this.fs.WriteByte(Convert.ToByte(this.transIndex));
			this.fs.WriteByte(0);
		}

		// Token: 0x06004597 RID: 17815 RVA: 0x000CC720 File Offset: 0x000CA920
		protected void WriteImageDesc()
		{
			this.fs.WriteByte(44);
			this.WriteShort(0);
			this.WriteShort(0);
			this.WriteShort(this.width);
			this.WriteShort(this.height);
			if (this.firstFrame)
			{
				this.fs.WriteByte(0);
				return;
			}
			this.fs.WriteByte(Convert.ToByte(128 | this.palSize));
		}

		// Token: 0x06004598 RID: 17816 RVA: 0x000CC794 File Offset: 0x000CA994
		protected void WriteLSD()
		{
			this.WriteShort(this.width);
			this.WriteShort(this.height);
			this.fs.WriteByte(Convert.ToByte(240 | this.palSize));
			this.fs.WriteByte(0);
			this.fs.WriteByte(0);
		}

		// Token: 0x06004599 RID: 17817 RVA: 0x000CC7F0 File Offset: 0x000CA9F0
		protected void WriteNetscapeExt()
		{
			this.fs.WriteByte(33);
			this.fs.WriteByte(byte.MaxValue);
			this.fs.WriteByte(11);
			this.WriteString("NETSCAPE2.0");
			this.fs.WriteByte(3);
			this.fs.WriteByte(1);
			this.WriteShort(this.repeat);
			this.fs.WriteByte(0);
		}

		// Token: 0x0600459A RID: 17818 RVA: 0x000CC864 File Offset: 0x000CAA64
		protected void WritePalette()
		{
			this.fs.Write(this.colorTab, 0, this.colorTab.Length);
			int num = 768 - this.colorTab.Length;
			for (int i = 0; i < num; i++)
			{
				this.fs.WriteByte(0);
			}
		}

		// Token: 0x0600459B RID: 17819 RVA: 0x000CC8B2 File Offset: 0x000CAAB2
		protected void WritePixels()
		{
			new LZWEncoder(this.width, this.height, this.indexedPixels, this.colorDepth).Encode(this.fs);
		}

		// Token: 0x0600459C RID: 17820 RVA: 0x000CC8DC File Offset: 0x000CAADC
		protected void WriteShort(int value)
		{
			this.fs.WriteByte(Convert.ToByte(value & 255));
			this.fs.WriteByte(Convert.ToByte(value >> 8 & 255));
		}

		// Token: 0x0600459D RID: 17821 RVA: 0x000CC910 File Offset: 0x000CAB10
		protected void WriteString(string s)
		{
			char[] array = s.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				this.fs.WriteByte((byte)array[i]);
			}
		}

		// Token: 0x0400336B RID: 13163
		protected int width;

		// Token: 0x0400336C RID: 13164
		protected int height;

		// Token: 0x0400336D RID: 13165
		protected Color32 transparent = new Color32(0, 0, 0, 0);

		// Token: 0x0400336E RID: 13166
		protected int transIndex;

		// Token: 0x0400336F RID: 13167
		protected int repeat = -1;

		// Token: 0x04003370 RID: 13168
		protected int delay;

		// Token: 0x04003371 RID: 13169
		protected bool started;

		// Token: 0x04003372 RID: 13170
		protected FileStream fs;

		// Token: 0x04003373 RID: 13171
		protected byte[] pixels;

		// Token: 0x04003374 RID: 13172
		protected byte[] indexedPixels;

		// Token: 0x04003375 RID: 13173
		protected int colorDepth;

		// Token: 0x04003376 RID: 13174
		protected byte[] colorTab;

		// Token: 0x04003377 RID: 13175
		protected bool[] usedEntry = new bool[256];

		// Token: 0x04003378 RID: 13176
		protected int palSize = 7;

		// Token: 0x04003379 RID: 13177
		protected int dispose = -1;

		// Token: 0x0400337A RID: 13178
		protected bool closeStream;

		// Token: 0x0400337B RID: 13179
		protected bool firstFrame = true;

		// Token: 0x0400337C RID: 13180
		protected bool sizeSet;

		// Token: 0x0400337D RID: 13181
		protected int sample = 10;
	}
}
