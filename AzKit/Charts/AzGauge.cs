
/* Copyright (c) 2016 Alaa Azazi

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace AzKit.Charts
{
    public class AzGauge : Canvas
    {
        private const double RADIANS = Math.PI / 180;

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(AzGauge), new PropertyMetadata(50.0, OnSizePropertyChanged));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(AzGauge), new PropertyMetadata(2.0, OnSizePropertyChanged));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Color), typeof(AzGauge), new PropertyMetadata(default(Brush), OnColorPropertyChanged));
        public static readonly DependencyProperty PercentValueProperty = DependencyProperty.Register("PercentValue", typeof(double), typeof(AzGauge), new PropertyMetadata(0.0, OnPercentValuePropertyChanged));
        public static readonly DependencyProperty AngleValueProperty = DependencyProperty.Register("AngleValue", typeof(double), typeof(AzGauge), new PropertyMetadata(0.0, OnAngleValuePropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }
            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Thickness
        {
            get
            {
                return (double)GetValue(ThicknessProperty);
            }
            set
            {
                SetValue(ThicknessProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color Fill
        {
            get
            {
                return (Color)GetValue(FillProperty);
            }
            set
            {
                SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Percent
        {
            get
            {
                return (double)GetValue(PercentValueProperty);
            }
            set
            {
                SetValue(PercentValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Angle
        {
            get
            {
                return (double)GetValue(AngleValueProperty);
            }
            set
            {
                SetValue(AngleValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Point CenterPoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Point StartPoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AzGauge()
        {
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetControlSize();
            Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Draw()
        {
            Children.Clear();

            Path foregroundGauge = GetForegroundGauge();
            Path backgroundGauge = GetBackgroundGauge();
            Grid labelContainer = GetLabelContainer();

            Children.Add(backgroundGauge);
            Children.Add(foregroundGauge);
            Children.Add(labelContainer);

        }

        /// <summary>
        /// 
        /// </summary>
        private void SetControlSize()
        {
            Width = Radius * 2 + Thickness;
            Height = Radius * 2 + Thickness;

            CenterPoint = GetCenterPoint();
            StartPoint = new Point(CenterPoint.X, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Point GetCenterPoint()
        {
            return new Point(Radius, Radius);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Point GetEndPoint()
        {
            double adjustedAngle = (Angle >= 360) ? 359.9 : Angle;
            double x = CenterPoint.X + (Radius * Math.Sin(adjustedAngle * RADIANS));
            double y = CenterPoint.Y - (Radius * Math.Cos(adjustedAngle * RADIANS));

            return new Point(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAngle()
        {
            double angle = Percent / 100 * 360;
            if (angle >= 360)
            {
                angle = 359.999;
            }
            Angle = angle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Path GetForegroundGauge()
        {
            var path = new Path();
            var pathGeometry = new PathGeometry();

            var arcSegment = new ArcSegment
            {
                IsLargeArc = Angle > 180.0,
                Point = GetEndPoint(),
                Size = new Size(Radius, Radius),
                SweepDirection = SweepDirection.Clockwise
            };

            var pathFigure = new PathFigure
            {
                StartPoint = StartPoint,
                IsClosed = false
            };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;
            path.Stroke = new SolidColorBrush(Fill);
            path.StrokeThickness = Thickness;
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Path GetBackgroundGauge()
        {
            var path = new Path();
            var pathGeometry = new PathGeometry();

            var arcSegment = new ArcSegment
            {
                IsLargeArc = true,
                Point = new Point(CenterPoint.X - 0.1, 0),
                Size = new Size(Radius, Radius),
                SweepDirection = SweepDirection.Clockwise
            };

            var pathFigure = new PathFigure
            {
                StartPoint = StartPoint,
                IsClosed = false
            };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;
            path.Stroke = new SolidColorBrush(Colors.DarkGray);
            path.StrokeThickness = Thickness;
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Grid GetLabelContainer()
        {
            Grid labelContainer = new Grid
            {
                Width = Radius * 2,
                Height = Radius * 2
            };

            TextBlock label = new TextBlock
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                FontSize = Radius / 2,
                Foreground = new SolidColorBrush(this.Fill),
                Text = String.Format("{0}%", this.Percent)
                //Text = String.Format("{0}°", this.Angle)
            };

            labelContainer.Children.Add(label);

            return labelContainer;
        }

        #region Property Event Handler
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AzGauge control = d as AzGauge;
            control.SetControlSize();
            control.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPercentValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AzGauge control = d as AzGauge;
            control.SetAngle();
            control.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAngleValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AzGauge control = d as AzGauge;
            control.Draw();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AzGauge control = d as AzGauge;
            control.Draw();
        }
        #endregion
    }

}
