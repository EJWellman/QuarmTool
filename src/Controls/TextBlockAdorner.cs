using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace EQTool.Controls
{
	public class TextBlockAdorner : Adorner
	{
		private double angle = 0.0;
		private Point transformOrigin = new Point(0, 0);
		private Canvas childElement;
		public VisualCollection _visualChildren;
		private Thumb leftTop, rightTop, leftBottom, rightBottom;

		public TextBlockAdorner(UIElement element) : base(element)
		{
			_visualChildren = new VisualCollection(this);
			childElement = element as Canvas;
			CreateThumbPart(ref leftTop);
			leftTop.DragDelta += (sender, e) =>
			{
				double hor = e.HorizontalChange;
				double vert = e.VerticalChange;
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					double _max = hor > vert ? hor : vert;
					hor = _max;
					vert = _max;
				}
				ResizeX(hor);
				ResizeY(vert);
				e.Handled = true;
			};
			CreateThumbPart(ref rightTop);
			rightTop.DragDelta += (sender, e) =>
			{
				double hor = e.HorizontalChange;
				double vert = e.VerticalChange;
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					// THIS: NO WORKED
					double _max = Math.Abs(hor) > Math.Abs(vert) ? Math.Abs(hor) : Math.Abs(vert);
					if (hor >= 0 && vert <= 0)
					{
						hor = _max;
						vert = -_max;
					}
					else
					{
						hor = -_max;
						vert = _max;
					}
				}
				ResizeWidth(hor);
				ResizeY(vert);
				e.Handled = true;
			};
			CreateThumbPart(ref leftBottom);
			leftBottom.DragDelta += (sender, e) =>
			{
				double hor = e.HorizontalChange;
				double vert = e.VerticalChange;
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					// THIS: NO WORKED
					double _max = Math.Abs(hor) > Math.Abs(vert) ? Math.Abs(hor) : Math.Abs(vert);
					if (hor <= 0 && vert >= 0)
					{
						hor = -_max;
						vert = _max;
					}
					else
					{
						hor = _max;
						vert = -_max;
					}
				}
				ResizeX(hor);
				ResizeHeight(vert);
				e.Handled = true;
			};
			CreateThumbPart(ref rightBottom);
			rightBottom.DragDelta += (sender, e) =>
			{
				double hor = e.HorizontalChange;
				double vert = e.VerticalChange;
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					double _max = hor > vert ? hor : vert;
					hor = _max;
					vert = _max;
				}
				ResizeWidth(hor);
				ResizeHeight(vert);
				e.Handled = true;
			};
		}
		private void ResizeWidth(double e)
		{
			double deltaHorizontal = Math.Min(-e, childElement.ActualWidth - childElement.MinWidth);
			if (childElement.Width - deltaHorizontal < 10)
			{
				return;
			}
			Canvas.SetTop(childElement, Canvas.GetTop(childElement) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
			Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + (deltaHorizontal * transformOrigin.X * (1 - Math.Cos(angle))));
			childElement.Width -= deltaHorizontal;
		}
		private void ResizeX(double e)
		{
			double deltaHorizontal = Math.Min(e, childElement.ActualWidth - childElement.MinWidth);
			if (childElement.Width - deltaHorizontal < 10)
			{
				return;
			}
			Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaHorizontal * Math.Sin(angle) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
			Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaHorizontal * Math.Cos(angle) + (transformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle))));
			childElement.Width -= deltaHorizontal;
		}
		private void ResizeHeight(double e)
		{
			double deltaVertical = Math.Min(-e, childElement.ActualHeight - childElement.MinHeight);
			if(childElement.Height - deltaVertical < 10)
			{
				return;
			}
			Canvas.SetTop(childElement, Canvas.GetTop(childElement) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
			Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) - deltaVertical * transformOrigin.Y * Math.Sin(-angle));
			childElement.Height -= deltaVertical;
		}
		private void ResizeY(double e)
		{
			double deltaVertical = Math.Min(e, childElement.ActualHeight - childElement.MinHeight);
			if (childElement.Height - deltaVertical < 10)
			{
				return;
			}
			Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaVertical * Math.Cos(-angle) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
			Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaVertical * Math.Sin(-angle) - (transformOrigin.Y * deltaVertical * Math.Sin(-angle)));
			childElement.Height -= deltaVertical;
		}
		public void CreateThumbPart(ref Thumb cornerThumb)
		{
			cornerThumb = new Thumb { Width = 8, Height = 8, Background = Brushes.Red };
			_visualChildren.Add(cornerThumb);
		}
		public void EnforceSize(FrameworkElement element)
		{
			if (element.Width.Equals(Double.NaN))
				element.Width = element.DesiredSize.Width;
			if (element.Height.Equals(Double.NaN))
				element.Height = element.DesiredSize.Height;
			FrameworkElement parent = element.Parent as FrameworkElement;
			if (parent != null)
			{
				element.MaxHeight = parent.ActualHeight;
				element.MaxWidth = parent.ActualWidth;
			}
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			base.ArrangeOverride(finalSize);
			double desireWidth = AdornedElement.DesiredSize.Width;
			double desireHeight = AdornedElement.DesiredSize.Height;
			double adornerWidth = this.DesiredSize.Width;
			double adornerHeight = this.DesiredSize.Height;
			leftTop.Arrange(new Rect(-adornerWidth / 2 - 15, -adornerHeight / 2 - 15, adornerWidth, adornerHeight));
			rightTop.Arrange(new Rect(desireWidth - adornerWidth / 2 + 15, -adornerHeight / 2 - 15, adornerWidth, adornerHeight));
			leftBottom.Arrange(new Rect(-adornerWidth / 2 - 15, desireHeight - adornerHeight / 2 + 15, adornerWidth, adornerHeight));
			rightBottom.Arrange(new Rect(desireWidth - adornerWidth / 2 + 15, desireHeight - adornerHeight / 2 + 15, adornerWidth, adornerHeight));
			return finalSize;
		}
		protected override int VisualChildrenCount => _visualChildren.Count;
		protected override Visual GetVisualChild(int index) => _visualChildren[index];
		protected override void OnRender(DrawingContext drawingContext) => base.OnRender(drawingContext);
	}
}
