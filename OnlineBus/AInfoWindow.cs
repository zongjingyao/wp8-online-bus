using Com.AMap.Api.Maps;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace BusTest
{
    /// <summary>
    /// 弹出信息
    /// </summary>
    public sealed class AInfoWindow : UserControl
    {
        /// <summary>
        /// 点击Tip的关闭按钮时触发事件
        /// </summary>
        //public event EventHandler<TappedRoutedEventArgs> TipCloseClick;
        private String title;
        private String contentText;
        private Color titleBackgroundColor;
        private Color contentBackgroundColor;

        /// <summary>
        /// 标记物
        /// </summary>
        public AInfoWindow()
        {
            parent = new Grid();
            _path = new Path();
            path1 = new Path();
            path2 = new Path();
            path3 = new Path();
            path4 = new Path();
            tileGird = new Grid();
            Anchor = new Point(5, 0);

            //  this.Tapped += ATip_Tapped;
            //this.DoubleTapped += ATip_DoubleTapped;
            ContentTextBlock = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, TextWrapping = TextWrapping.Wrap, FontSize = 15, Margin = new Thickness(0, 0, 0, 0) };
            TitleTextBlock = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 22, Margin = new Thickness(0, 9, 0, 0) };
            ContentText = "";
            Title = "";

            ContentPanel = new StackPanel();
            TitlePanel = new StackPanel();

            TitleBackgroundColor = HexToColor("#FF2D2D2D");
            ContentBackgroundColor = HexToColor("#2F3236");
            SetBackgroundColor();
            parent.HorizontalAlignment = HorizontalAlignment.Stretch;
            parent.VerticalAlignment = VerticalAlignment.Stretch;
            parent.Margin = new Thickness(0, 0, 0, 0);
            this.Content = parent;

            parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
            parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
            parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            parent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(59) });

            //左上边

            var pathGeometry = new PathGeometry();
            var figures = pathGeometry.Figures;
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, 40);
            figure.Segments.Add(new LineSegment() { Point = new Point(0, 10) });
            figure.Segments.Add(new LineSegment() { Point = new Point(0, 0) });
            figure.Segments.Add(new LineSegment() { Point = new Point(10, 0) });
            // Can add other types, e.g. PolyLineSegment, PathSegment, ArcSegment, BezierSegment
            //figure.Segments.Add(new QuadraticBezierSegment() { Point1 = new Point(0, 0), Point2 = new Point(8, 0) });
            figure.Segments.Add(new LineSegment() { Point = new Point(10, 0) });
            figure.Segments.Add(new LineSegment() { Point = new Point(10, 40) });
            figure.IsClosed = true;
            figures.Add(figure);
            _path.Data = pathGeometry;
            // _path.Margin = new Thickness(0, 1, 0, -2);
            parent.Children.Add(_path);

            var pathGeometry1 = new PathGeometry();
            var figures1 = pathGeometry1.Figures;
            var figure1 = new PathFigure();
            figure1.StartPoint = new Point(10, 40);
            figure1.Segments.Add(new LineSegment() { Point = new Point(10, 10) });
            figure1.Segments.Add(new LineSegment() { Point = new Point(10, 0) });
            //figure1.Segments.Add(new QuadraticBezierSegment() { Point1 = new Point(10, 0), Point2 = new Point(2, 0) });
            figure1.Segments.Add(new LineSegment() { Point = new Point(0, 0) });
            figure1.Segments.Add(new LineSegment() { Point = new Point(0, 40) });
            figure1.IsClosed = true;
            figures1.Add(figure1);
            path1.Data = pathGeometry1;
            path1.SetValue(Grid.ColumnProperty, 2);
            // path1.Margin = new Thickness(0, 1, 0, -2);
            parent.Children.Add(path1);
            //标题
            TitlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            TitlePanel.VerticalAlignment = VerticalAlignment.Stretch;
            TitlePanel.Children.Add(TitleTextBlock);
            tileGird.HorizontalAlignment = HorizontalAlignment.Stretch;
            tileGird.VerticalAlignment = VerticalAlignment.Stretch;
            tileGird.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            tileGird.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
            tileGird.Children.Add(TitlePanel);
            tileGird.Margin = new Thickness(-1, 0, -1, -1);
            tileGird.SetValue(Grid.ColumnProperty, 1);

            parent.Children.Add(tileGird);

            var pathGeometry2 = new PathGeometry();
            var figures2 = pathGeometry2.Figures;
            var figure2 = new PathFigure();
            figure2.StartPoint = new Point(10, 0);
            figure2.Segments.Add(new LineSegment() { Point = new Point(0, 0) });
            figure2.Segments.Add(new LineSegment() { Point = new Point(0, 50) });
            //figure2.Segments.Add(new QuadraticBezierSegment() { Point1 = new Point(0, 50), Point2 = new Point(8, 50) });
            figure2.Segments.Add(new LineSegment() { Point = new Point(10, 50) });
            figure2.IsClosed = false;
            path2.StrokeThickness = 0;
            figures2.Add(figure2);
            path2.SetValue(Grid.RowProperty, 1);
            path2.Data = pathGeometry2;
            parent.Children.Add(path2);

            var pathGeometry3 = new PathGeometry();
            var figures3 = pathGeometry3.Figures;
            var figure3 = new PathFigure();
            figure3.StartPoint = new Point(0, 0);
            figure3.Segments.Add(new LineSegment() { Point = new Point(10, 0) });
            figure3.Segments.Add(new LineSegment() { Point = new Point(10, 50) });
            //figure3.Segments.Add(new QuadraticBezierSegment() { Point1 = new Point(10, 50), Point2 = new Point(2, 50) });
            figure3.Segments.Add(new LineSegment() { Point = new Point(0, 50) });
            figure3.IsClosed = false;
            figures3.Add(figure3);
            path3.StrokeThickness = 0;
            path3.SetValue(Grid.RowProperty, 1);
            path3.SetValue(Grid.ColumnProperty, 2);
            path3.VerticalAlignment = VerticalAlignment.Top;
            path3.Data = pathGeometry3;
            //path3.Margin = new Thickness(0, 0, 0, 0);
            parent.Children.Add(path3);

            ContentPanel.VerticalAlignment = VerticalAlignment.Center;

            ContentPanel.Margin = new Thickness(-1, 0, -1, 0);
            ContentPanel.Orientation = Orientation.Horizontal;
            ContentPanel.Children.Add(ContentTextBlock);
            ContentPanel.SetValue(Grid.RowProperty, 1);
            ContentPanel.SetValue(Grid.ColumnProperty, 1);
            ContentPanel.Height = 50;
            ContentPanel.VerticalAlignment = VerticalAlignment.Top;

            parent.Children.Add(ContentPanel);

            var pathGeometry4 = new PathGeometry();
            var figures4 = pathGeometry4.Figures;
            var figure4 = new PathFigure();
            figure4.StartPoint = new Point(0, 0);
            figure4.Segments.Add(new LineSegment() { Point = new Point(10, 10) });
            figure4.Segments.Add(new LineSegment() { Point = new Point(20, 0) });
            figure4.IsClosed = false;
            figures4.Add(figure4);
            path4.SetValue(Grid.RowProperty, 1);
            path4.SetValue(Grid.ColumnProperty, 1);
            path4.VerticalAlignment = VerticalAlignment.Bottom;
            path4.HorizontalAlignment = HorizontalAlignment.Center;
            path4.Data = pathGeometry4;
            parent.Children.Add(path4);
            SetBackgroundColor();
            SetCloseImage();
            this.Title = "";
            this.ContentText = "";


        }


        /// <summary>
        /// 信息标题
        /// </summary>
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                TitleTextBlock.Text = value;
            }
        }
        /// <summary>
        /// 锚点值默认值(0.5,0)中上位置  取值0->1 左上为Point(0,0)  右下为中心点为Point(1,1)
        /// </summary>
        public Point Anchor
        {
            get
            {
                Point? p = AMapLayer.GetAnchor(this);
                return p == null ? new Point(0.5, 0) : (Point)p;
            }
            set
            {
                AMapLayer.SetAnchor(this, value);
            }
        }
        /// <summary>
        /// 信息内容
        /// </summary>
        public String ContentText
        {
            get
            {
                return contentText;
            }
            set
            {
                contentText = value;
                ContentTextBlock.Text = value;
            }
        }
        /// <summary>
        /// 内容容器
        /// </summary>
        public StackPanel ContentPanel
        {
            get;
            private set;
        }
        /// <summary>
        /// 标题容器
        /// </summary>
        public StackPanel TitlePanel
        {
            get;
            private set;
        }
        /// <summary>
        /// 标题背景色
        /// </summary>
        public System.Windows.Media.Color TitleBackgroundColor
        {
            get
            {
                return titleBackgroundColor;
            }
            set
            {
                titleBackgroundColor = value;
                SetBackgroundColor();
            }
        }
        /// <summary>
        /// 内容背景色
        /// </summary>
        public Color ContentBackgroundColor
        {
            get
            {
                return contentBackgroundColor;
            }
            set
            {
                contentBackgroundColor = value;
                SetBackgroundColor();
            }
        }
        /// <summary>
        /// 内容TextBlock
        /// </summary>
        public TextBlock ContentTextBlock
        {
            get;
            private set;
        }
        /// <summary>
        /// 标题TextBlock
        /// </summary>
        public TextBlock TitleTextBlock
        {
            get;
            private set;
        }
        private Grid parent;
        //左上边
        private Path _path;
        //右上边
        private Path path1;
        //左下边
        private Path path2;
        //右下边
        private Path path3;
        //下面的尖角
        private Path path4;
        private Grid tileGird;
        internal void SetBackgroundColor()
        {
            _path.Fill = new SolidColorBrush(TitleBackgroundColor);
            path1.Fill = new SolidColorBrush(TitleBackgroundColor);
            tileGird.Background = new SolidColorBrush(TitleBackgroundColor);
            path2.Fill = new SolidColorBrush(ContentBackgroundColor);
            path3.Fill = new SolidColorBrush(ContentBackgroundColor);
            ContentPanel.Background = new SolidColorBrush(ContentBackgroundColor);
            path4.Fill = new SolidColorBrush(ContentBackgroundColor);
        }
        void SetCloseImage()
        {
            Image image = new Image();
            BitmapImage bi = new BitmapImage(new Uri("Assets/close.png", UriKind.RelativeOrAbsolute));
            image.Width = 20;
            image.Source = bi;
            image.SetValue(Grid.ColumnProperty, 1);
            image.HorizontalAlignment = HorizontalAlignment.Right;
            tileGird.Children.Add(image);
            image.Tap += image_Tap;
        }

        void image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
            //if (MapInstance != null)
            //{
            //    MapInstance.CloseInfoWindow();
            //}
        }

        internal AMap MapInstance { get; set; }


        internal static Color HexToColor(string hexValue)
        {
            try
            {
                hexValue = hexValue.Replace("#", "");
                byte position = 0;
                byte alpha = System.Convert.ToByte("ff", 16);

                if (hexValue.Length == 8)
                {
                    // get the alpha channel value
                    alpha = System.Convert.ToByte(hexValue.Substring(position, 2), 16);
                    position = 2;
                }

                // get the red value
                byte red = System.Convert.ToByte(hexValue.Substring(position, 2), 16);
                position += 2;

                // get the green value
                byte green = System.Convert.ToByte(hexValue.Substring(position, 2), 16);
                position += 2;

                // get the blue value
                byte blue = System.Convert.ToByte(hexValue.Substring(position, 2), 16);

                // create the Color object
                Color color = Color.FromArgb(alpha, red, green, blue);

                // create the SolidColorBrush object
                return color;
            }
            catch
            {
                return Color.FromArgb(255, 251, 237, 187);
            }
        }
    }
}
