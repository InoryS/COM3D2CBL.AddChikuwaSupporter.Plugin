using System;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x02000005 RID: 5
	public abstract class Element : IComparable<Element>
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x0000C4A0 File Offset: 0x0000B4A0
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000C4B8 File Offset: 0x0000B4B8
		public virtual Rect Rectangle
		{
			get
			{
				return this.rect;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000C4D0 File Offset: 0x0000B4D0
		public virtual float Left
		{
			get
			{
				return this.rect.x;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000C4F0 File Offset: 0x0000B4F0
		public virtual float Top
		{
			get
			{
				return this.rect.y;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000AD RID: 173 RVA: 0x0000C510 File Offset: 0x0000B510
		public virtual float Width
		{
			get
			{
				return this.rect.width;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000C530 File Offset: 0x0000B530
		public virtual float Height
		{
			get
			{
				return this.rect.height;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000AF RID: 175 RVA: 0x0000C550 File Offset: 0x0000B550
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x0000C568 File Offset: 0x0000B568
		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				if (this.Parent != null)
				{
					this.notifyParent(false, true);
				}
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000B1 RID: 177 RVA: 0x0000C593 File Offset: 0x0000B593
		// (remove) Token: 0x060000B2 RID: 178 RVA: 0x0000C5AC File Offset: 0x0000B5AC
		public event EventHandler<ElementEventArgs> NotifyParent = delegate(object A_0, ElementEventArgs A_1)
		{
		};

		// Token: 0x060000B3 RID: 179 RVA: 0x0000C5C8 File Offset: 0x0000B5C8
		public Element()
		{
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000C604 File Offset: 0x0000B604
		public Element(string name, Rect rect)
		{
			this.id = this.GetHashCode();
			this.name = name;
			this.rect = rect;
			this.visible = true;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000C667 File Offset: 0x0000B667
		public virtual void Draw()
		{
			this.Draw(this.rect);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000C677 File Offset: 0x0000B677
		public virtual void Draw(Rect outRect)
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000C67A File Offset: 0x0000B67A
		public virtual void Resize()
		{
			this.Resize(false);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000C688 File Offset: 0x0000B688
		public virtual void Resize(bool broadCast)
		{
			if (!broadCast)
			{
				this.notifyParent(true, false);
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000C6A4 File Offset: 0x0000B6A4
		public virtual int CompareTo(Element e)
		{
			return this.name.CompareTo(e.Name);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000C6C7 File Offset: 0x0000B6C7
		protected virtual void notifyParent(bool sizeChanged, bool visibleChanged)
		{
			this.NotifyParent(this, new ElementEventArgs(this.name, sizeChanged, visibleChanged));
		}

		// Token: 0x0400009F RID: 159
		protected readonly int id;

		// Token: 0x040000A0 RID: 160
		protected string name;

		// Token: 0x040000A1 RID: 161
		protected Rect rect;

		// Token: 0x040000A2 RID: 162
		protected bool visible;

		// Token: 0x040000A3 RID: 163
		public Container Parent = null;
	}
}
