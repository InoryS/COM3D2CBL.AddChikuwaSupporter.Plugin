using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x0200001A RID: 26
	public static class PixelValuesCM3D2
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600013B RID: 315 RVA: 0x0000F560 File Offset: 0x0000E560
		// (set) Token: 0x0600013C RID: 316 RVA: 0x0000F57C File Offset: 0x0000E57C
		public static int Margin
		{
			get
			{
				return PixelValuesCM3D2.PropPx(PixelValuesCM3D2.margin);
			}
			set
			{
				PixelValuesCM3D2.margin = value;
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000F588 File Offset: 0x0000E588
		static PixelValuesCM3D2()
		{
			PixelValuesCM3D2.font["C1"] = 12;
			PixelValuesCM3D2.font["C2"] = 11;
			PixelValuesCM3D2.font["H1"] = 20;
			PixelValuesCM3D2.font["H2"] = 16;
			PixelValuesCM3D2.font["H3"] = 14;
			PixelValuesCM3D2.line["C1"] = 18;
			PixelValuesCM3D2.line["C2"] = 14;
			PixelValuesCM3D2.line["H1"] = 30;
			PixelValuesCM3D2.line["H2"] = 24;
			PixelValuesCM3D2.line["H3"] = 22;
			PixelValuesCM3D2.sys["Menu.Height"] = 45;
			PixelValuesCM3D2.sys["OkButton.Height"] = 95;
			PixelValuesCM3D2.sys["HScrollBar.Width"] = 15;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000F6BC File Offset: 0x0000E6BC
		public static int Font(string key)
		{
			return PixelValuesCM3D2.PropPx(PixelValuesCM3D2.font[key]);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000F6E0 File Offset: 0x0000E6E0
		public static int Line(string key)
		{
			return PixelValuesCM3D2.PropPx(PixelValuesCM3D2.line[key]);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000F704 File Offset: 0x0000E704
		public static int Sys(string key)
		{
			return PixelValuesCM3D2.PropPx(PixelValuesCM3D2.sys[key]);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000F728 File Offset: 0x0000E728
		public static int Font_(string key)
		{
			return PixelValuesCM3D2.font[key];
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000F748 File Offset: 0x0000E748
		public static int Line_(string key)
		{
			return PixelValuesCM3D2.line[key];
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000F768 File Offset: 0x0000E768
		public static int Sys_(string key)
		{
			return PixelValuesCM3D2.sys[key];
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000F788 File Offset: 0x0000E788
		public static Rect PropScreen(Rect ratio)
		{
			return new Rect((float)(Screen.width - PixelValuesCM3D2.Margin * 2) * ratio.x + (float)PixelValuesCM3D2.Margin, (float)(Screen.height - PixelValuesCM3D2.Margin * 2) * ratio.y + (float)PixelValuesCM3D2.Margin, (float)(Screen.width - PixelValuesCM3D2.Margin * 2) * ratio.width, (float)(Screen.height - PixelValuesCM3D2.Margin * 2) * ratio.height);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000F808 File Offset: 0x0000E808
		public static Rect PropScreenMH(Rect ratio)
		{
			Rect result = PixelValuesCM3D2.PropScreen(ratio);
			result.y += (float)PixelValuesCM3D2.Sys("Menu.Height");
			result.height -= (float)(PixelValuesCM3D2.Sys("Menu.Height") + PixelValuesCM3D2.Sys("OkButton.Height"));
			return result;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000F864 File Offset: 0x0000E864
		public static Rect PropScreenMH(float left, float top, float width, float height, Vector2 last)
		{
			Rect result = PixelValuesCM3D2.PropScreen(new Rect(left / (last.x - (float)(PixelValuesCM3D2.Margin * 2)), top / (last.y - (float)(PixelValuesCM3D2.Margin * 2)), width, height));
			result.height -= (float)(PixelValuesCM3D2.Sys("Menu.Height") + PixelValuesCM3D2.Sys("OkButton.Height"));
			return result;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000F8D0 File Offset: 0x0000E8D0
		public static Rect InsideRect(Rect rect)
		{
			return new Rect((float)PixelValuesCM3D2.Margin, (float)PixelValuesCM3D2.Margin, rect.width - (float)(PixelValuesCM3D2.Margin * 2), rect.height - (float)(PixelValuesCM3D2.Margin * 2));
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000F914 File Offset: 0x0000E914
		public static Rect InsideRect(Rect rect, int height)
		{
			return new Rect((float)PixelValuesCM3D2.Margin, (float)PixelValuesCM3D2.Margin, rect.width - (float)(PixelValuesCM3D2.Margin * 2), (float)height);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000F94C File Offset: 0x0000E94C
		public static Rect InsideRect(Rect rect, Rect padding)
		{
			return new Rect(rect.x + padding.x, rect.y + padding.x, rect.width - padding.width * 2f, rect.height - padding.height * 2f);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000F9AC File Offset: 0x0000E9AC
		public static int PropPx(int px)
		{
			return (int)((float)px * (1f + ((float)Screen.width / PixelValuesCM3D2.BaseWidth - 1f) * PixelValuesCM3D2.PropRatio));
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000F9E0 File Offset: 0x0000E9E0
		public static Rect PropRect(int px)
		{
			return new Rect((float)PixelValuesCM3D2.PropPx(px), (float)PixelValuesCM3D2.PropPx(px), (float)PixelValuesCM3D2.PropPx(px), (float)PixelValuesCM3D2.PropPx(px));
		}

		// Token: 0x04000121 RID: 289
		private static int margin = 10;

		// Token: 0x04000122 RID: 290
		private static Dictionary<string, int> font = new Dictionary<string, int>();

		// Token: 0x04000123 RID: 291
		private static Dictionary<string, int> line = new Dictionary<string, int>();

		// Token: 0x04000124 RID: 292
		private static Dictionary<string, int> sys = new Dictionary<string, int>();

		// Token: 0x04000125 RID: 293
		public static float BaseWidth = 1280f;

		// Token: 0x04000126 RID: 294
		public static float PropRatio = 0.6f;
	}
}
