using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x0200000F RID: 15
	public class Window : Container
	{
		// Token: 0x06000111 RID: 273 RVA: 0x0000E834 File Offset: 0x0000D834
		public Window(Rect ratio, string header, string title) : this(title, ratio, header, title, null)
		{
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000E844 File Offset: 0x0000D844
		public Window(string name, Rect ratio, string header, string title) : this(name, ratio, header, title, null)
		{
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000E858 File Offset: 0x0000D858
		public Window(string name, Rect ratio, string header, string title, List<Element> children) : base(name, PixelValuesCM3D2.PropScreenMH(ratio))
		{
			this.sizeRatio = ratio;
			this.HeaderText = header;
			this.TitleText = title;
			this.TitleHeight = (float)PixelValuesCM3D2.Line("C1");
			if (children != null && children.Count > 0)
			{
				this.children = new List<Element>(children);
				foreach (Element element in children)
				{
					element.Parent = this;
					element.NotifyParent += new EventHandler<ElementEventArgs>(this.onChildChenged);
				}
				this.Resize();
			}
			this.lastScreenSize = new Vector2((float)Screen.width, (float)Screen.height);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000E990 File Offset: 0x0000D990
		public override void Draw(Rect outRect)
		{
			if (this.propScreen())
			{
				this.resizeAllChildren(this);
				this.Resize();
				outRect = this.rect;
			}
			this.WindowStyle.fontSize = PixelValuesCM3D2.Font("C2");
			this.WindowStyle.alignment = 2;
			this.rect = GUI.Window(this.id, outRect, new GUI.WindowFunction(this.drawWindow), this.HeaderText, this.WindowStyle);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000EA11 File Offset: 0x0000DA11
		public override void Resize()
		{
			this.calcAutoSize();
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000EA1C File Offset: 0x0000DA1C
		public Element AddHorizontalSpacer()
		{
			return this.AddHorizontalSpacer((float)PixelValuesCM3D2.Margin);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000EA3C File Offset: 0x0000DA3C
		public Element AddHorizontalSpacer(float height)
		{
			return base.AddChild<Window.HorizontalSpacer>(new Window.HorizontalSpacer(height));
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000EA5C File Offset: 0x0000DA5C
		private void drawWindow(int id)
		{
			this.TitleHeight = (float)PixelValuesCM3D2.Line("C1");
			this.TitleFontSize = PixelValuesCM3D2.Font("C2");
			this.LabelStyle.fontSize = this.TitleFontSize;
			this.LabelStyle.alignment = 0;
			GUI.Label(this.titleRect, this.TitleText, this.LabelStyle);
			GUI.BeginGroup(this.contentRect);
			Rect outRect;
			outRect..ctor(0f, 0f, 0f, 0f);
			foreach (Element element in this.children)
			{
				if (element.Visible)
				{
					if (element.Left >= 0f || element.Top >= 0f)
					{
						Rect outRect2;
						outRect2..ctor((element.Left >= 0f) ? element.Left : outRect.x, (element.Top >= 0f) ? element.Top : outRect.y, (element.Width > 0f) ? element.Width : this.autoSize.x, (element.Height > 0f) ? element.Height : this.autoSize.y);
						element.Draw(outRect2);
					}
					else
					{
						outRect.width = ((element.Width > 0f) ? element.Width : this.autoSize.x);
						outRect.height = ((element.Height > 0f) ? element.Height : this.autoSize.y);
						element.Draw(outRect);
						outRect.y += outRect.height;
					}
				}
			}
			GUI.EndGroup();
			GUI.DragWindow();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000EC88 File Offset: 0x0000DC88
		private bool propScreen()
		{
			Vector2 vector;
			vector..ctor((float)Screen.width, (float)Screen.height);
			bool result;
			if (this.lastScreenSize != vector)
			{
				this.rect = PixelValuesCM3D2.PropScreenMH(this.rect.x, this.rect.y, this.sizeRatio.width, this.sizeRatio.height, this.lastScreenSize);
				this.lastScreenSize = vector;
				this.calcRectSize();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000ED14 File Offset: 0x0000DD14
		private void calcRectSize()
		{
			this.baseRect = PixelValuesCM3D2.InsideRect(this.rect);
			this.titleRect = new Rect((float)PixelValuesCM3D2.Margin, 0f, this.baseRect.width, this.TitleHeight);
			this.contentRect = new Rect(this.baseRect.x, this.baseRect.y + this.titleRect.height, this.baseRect.width, this.baseRect.height - this.titleRect.height);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000EDAC File Offset: 0x0000DDAC
		public void calcAutoSize()
		{
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			foreach (Element element in this.children)
			{
				if (element.Visible)
				{
					if (element.Left <= 0f && element.Top <= 0f && element.Width > 0f)
					{
						zero.x += element.Width;
					}
					else
					{
						zero2.x += 1f;
					}
					if (element.Left <= 0f && element.Top <= 0f && element.Height > 0f)
					{
						zero.y += element.Height;
					}
					else
					{
						zero2.y += 1f;
					}
				}
			}
			bool flag = false;
			if ((this.scroll & Window.Scroll.HScroll) == Window.Scroll.None)
			{
				if (this.contentRect.width < zero.x || (this.contentRect.width > zero.x && zero2.x == 0f))
				{
					this.rect.width = zero.x + (float)(PixelValuesCM3D2.Margin * 2);
					flag = true;
				}
			}
			if ((this.scroll & Window.Scroll.VScroll) == Window.Scroll.None)
			{
				if (this.contentRect.height < zero.y || (this.contentRect.height > zero.y && zero2.y == 0f))
				{
					this.rect.height = zero.y + this.titleRect.height + (float)(PixelValuesCM3D2.Margin * 3);
					flag = true;
				}
			}
			if (flag)
			{
				this.calcRectSize();
			}
			this.autoSize.x = ((zero2.x > 0f) ? ((this.contentRect.width - zero.x) / (float)this.colums) : this.contentRect.width);
			this.autoSize.y = ((zero2.y > 0f) ? ((this.contentRect.height - zero.y) / (float)Math.Ceiling((double)(zero2.y / (float)this.colums))) : this.contentRect.height);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000F088 File Offset: 0x0000E088
		private void resizeAllChildren(Container parent)
		{
			if (parent != null)
			{
				foreach (Element element in parent)
				{
					if (element is Container)
					{
						this.resizeAllChildren(element as Container);
					}
					else
					{
						element.Resize(true);
					}
				}
			}
		}

		// Token: 0x040000F2 RID: 242
		public const float AutoLayout = -1f;

		// Token: 0x040000F3 RID: 243
		private Rect sizeRatio;

		// Token: 0x040000F4 RID: 244
		private Rect baseRect;

		// Token: 0x040000F5 RID: 245
		private Rect titleRect;

		// Token: 0x040000F6 RID: 246
		private Rect contentRect;

		// Token: 0x040000F7 RID: 247
		private Vector2 autoSize = Vector2.zero;

		// Token: 0x040000F8 RID: 248
		private Vector2 hScrollViewPos = Vector2.zero;

		// Token: 0x040000F9 RID: 249
		private Vector2 vScrollViewPos = Vector2.zero;

		// Token: 0x040000FA RID: 250
		private Vector2 lastScreenSize;

		// Token: 0x040000FB RID: 251
		private int colums = 1;

		// Token: 0x040000FC RID: 252
		public GUIStyle WindowStyle = "window";

		// Token: 0x040000FD RID: 253
		public GUIStyle LabelStyle = "label";

		// Token: 0x040000FE RID: 254
		public string HeaderText;

		// Token: 0x040000FF RID: 255
		public int HeaderFontSize;

		// Token: 0x04000100 RID: 256
		public string TitleText;

		// Token: 0x04000101 RID: 257
		public float TitleHeight;

		// Token: 0x04000102 RID: 258
		public int TitleFontSize;

		// Token: 0x04000103 RID: 259
		public Window.Scroll scroll = Window.Scroll.None;

		// Token: 0x02000010 RID: 16
		[Flags]
		public enum Scroll
		{
			// Token: 0x04000105 RID: 261
			None = 0,
			// Token: 0x04000106 RID: 262
			HScroll = 1,
			// Token: 0x04000107 RID: 263
			VScroll = 2
		}

		// Token: 0x02000011 RID: 17
		private class HorizontalSpacer : Element
		{
			// Token: 0x0600011D RID: 285 RVA: 0x0000F10C File Offset: 0x0000E10C
			public HorizontalSpacer(float height) : base("Spacer:", new Rect(-1f, -1f, -1f, height))
			{
				this.name += this.id;
			}
		}
	}
}
