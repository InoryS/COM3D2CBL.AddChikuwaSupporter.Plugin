using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x02000006 RID: 6
	public abstract class Container : Element, IEnumerable<Element>, IEnumerable
	{
		// Token: 0x060000BD RID: 189 RVA: 0x0000C6E4 File Offset: 0x0000B6E4
		public static Element Find(Container parent, string s)
		{
			return Container.Find<Element>(parent, s);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000C700 File Offset: 0x0000B700
		public static T Find<T>(Container parent, string s) where T : Element
		{
			T result;
			if (parent == null)
			{
				result = default(T);
			}
			else
			{
				foreach (Element element in parent)
				{
					if (element is T && element.Name == s)
					{
						return element as T;
					}
					if (element is Container)
					{
						T t = Container.Find<T>(element as Container, s);
						if (t != null)
						{
							return t;
						}
					}
				}
				result = default(T);
			}
			return result;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000C7DC File Offset: 0x0000B7DC
		public int ChildCount
		{
			get
			{
				return this.children.Count;
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000C7F9 File Offset: 0x0000B7F9
		public Container(string name, Rect rect) : base(name, rect)
		{
		}

		// Token: 0x1700000D RID: 13
		public Element this[string s]
		{
			get
			{
				return this.GetChild<Element>(s);
			}
			set
			{
				if (value != null)
				{
					this.AddChild(value);
				}
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000C850 File Offset: 0x0000B850
		public Element AddChild(Element child)
		{
			return this.AddChild<Element>(child);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000C86C File Offset: 0x0000B86C
		public T AddChild<T>(T child) where T : Element
		{
			T result;
			if (child != null && !this.children.Contains(child))
			{
				child.Parent = this;
				child.NotifyParent += new EventHandler<ElementEventArgs>(this.onChildChenged);
				this.children.Add(child);
				this.Resize();
				result = child;
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000C8EC File Offset: 0x0000B8EC
		public Element GetChild(string s)
		{
			return this.GetChild<Element>(s);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000C908 File Offset: 0x0000B908
		public T GetChild<T>() where T : Element
		{
			return this.GetChild<T>("");
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000C974 File Offset: 0x0000B974
		public T GetChild<T>(string s) where T : Element
		{
			return this.children.FirstOrDefault((Element e) => e is T && (s == "" || e.Name == s)) as T;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000C9E0 File Offset: 0x0000B9E0
		public void RemoveChild(string s)
		{
			Element element = this.children.FirstOrDefault((Element e) => e.Name == s);
			if (element != null)
			{
				element.Parent = null;
				element.NotifyParent -= new EventHandler<ElementEventArgs>(this.onChildChenged);
				this.children.Remove(element);
				this.Resize();
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000CA50 File Offset: 0x0000BA50
		public void RemoveChildren()
		{
			foreach (Element element in this.children)
			{
				element.Parent = null;
				element.NotifyParent -= new EventHandler<ElementEventArgs>(this.onChildChenged);
			}
			this.children.Clear();
			this.Resize();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000CAD4 File Offset: 0x0000BAD4
		public virtual void onChildChenged(object sender, EventArgs e)
		{
			this.Resize();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000CAE0 File Offset: 0x0000BAE0
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000CAF8 File Offset: 0x0000BAF8
		public IEnumerator<Element> GetEnumerator()
		{
			return this.children.GetEnumerator();
		}

		// Token: 0x040000A7 RID: 167
		protected List<Element> children = new List<Element>();
	}
}
