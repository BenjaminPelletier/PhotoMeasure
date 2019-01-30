using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PhotoMeasure.UI
{
    public partial class ImageExplorer : UserControl
    {
        public class CoordinateConverter
        {
            private SizeF _Viewport;
            private RectangleF _ViewRegion;

            public CoordinateConverter(SizeF viewport, RectangleF viewRegion)
            {
                _Viewport = viewport;
                _ViewRegion = viewRegion;
            }

            #region Conversions from Image coordinates to Viewport coordinates
            public float VpX(float xImg) { return (xImg - _ViewRegion.X) / _ViewRegion.Width * _Viewport.Width; }
            public float VpY(float yImg) { return (yImg - _ViewRegion.Y) / _ViewRegion.Height * _Viewport.Height; }
            public float VpW(float wImg) { return wImg * _Viewport.Width / _ViewRegion.Width; }
            public float VpH(float hImg) { return hImg * _Viewport.Height / _ViewRegion.Height; }
            public PointF Vp(PointF pImg) { return new PointF(VpX(pImg.X), VpY(pImg.Y)); }
            public Point Vp(Point pImg) { return new Point((int)Math.Round(VpX(pImg.X)), (int)Math.Round(VpY(pImg.Y))); }
            public IEnumerable<PointF> Vp(IEnumerable<PointF> ptsImg) { return ptsImg.Select(p => Vp(p)); }
            public IEnumerable<Point> Vp(IEnumerable<Point> ptsImg) { return ptsImg.Select(p => Vp(p)); }
            #endregion

            #region Conversions from Viewport coordinates to Image coordinates
            public float ImgX(float xVp) { return _ViewRegion.X + xVp / _Viewport.Width * _ViewRegion.Width; }
            public float ImgY(float yVp) { return _ViewRegion.Y + yVp / _Viewport.Height * _ViewRegion.Height; }
            public float ImgW(float wVp) { return wVp * _ViewRegion.Width / _Viewport.Width; }
            public float ImgH(float hVp) { return hVp * _ViewRegion.Height / _Viewport.Height; }
            public PointF Img(PointF pVp) { return new PointF(ImgX(pVp.X), ImgY(pVp.Y)); }
            public Point Img(Point pVp) { return new Point((int)Math.Round(ImgX(pVp.X)), (int)Math.Round(ImgY(pVp.Y))); }
            public IEnumerable<PointF> Img(IEnumerable<PointF> ptsVp) { return ptsVp.Select(p => Img(p)); }
            public IEnumerable<Point> Img(IEnumerable<Point> ptsVp) { return ptsVp.Select(p => Img(p)); }
            #endregion
        }

        private Bitmap _Canvas;
        private Graphics _CanvasGraphics;

        private RectangleF _ViewRegion;

        private Bitmap _Image;

        private const float MOUSEWHEEL_ZOOM = 0.35f;

        public event EventHandler<PaintEventArgs> PaintImage;

        public ImageExplorer()
        {
            InitializeComponent();
            this.MouseWheel += ImageExplorer_MouseWheel;
        }

        public CoordinateConverter Converter { get { return new CoordinateConverter(this.ClientSize, _ViewRegion); } }

        public Bitmap Image
        {
            set
            {
                _Image = value;
                FitImage();
            }
        }

        public void FitImage()
        {
            if (_Image == null) { return; }

            ViewRegion = new RectangleF(0, 0, _Image.Width, _Image.Height);
            this.Invalidate();
        }

        public void CenterOn(PointF pt)
        {
            RectangleF view = _ViewRegion;
            var center = new PointF(view.X + view.Width / 2, view.Y + view.Height / 2);
            view.X += pt.X - center.X;
            view.Y += pt.Y - center.Y;
            this.ViewRegion = view;
        }

        private RectangleF ViewRegion
        {
            get
            {
                return _ViewRegion;
            }
            set
            {
                if (_Image == null) { return; }

                RectangleF region = value;
                var viewport = this.ClientSize;

                // Adjust region aspect ratio to match viewport
                if (region.Width / region.Height > viewport.Width / viewport.Height)
                {
                    // Region has wider aspect ratio than viewport
                    float dy = region.Width * viewport.Height / viewport.Width - region.Height;
                    region.Y -= dy / 2;
                    region.Height += dy;
                }
                else
                {
                    // Region has taller aspect ratio than viewport
                    float dx = region.Height * viewport.Width / viewport.Height - region.Width;
                    region.X -= dx / 2;
                    region.Width += dx;
                }

                // If region is bigger than image in both dimensions, shrink it back down
                float zx = _Image.Width / region.Width;
                float zy = _Image.Height / region.Height;
                if (zx < 1 && zy < 1)
                {
                    float zoom = Math.Max(zx, zy);
                    region = IncrementalZoom(region, zoom - 1);

                    // If region is now out of view, put it back in view
                    if (region.X < 0) { region.X = 0; }
                    if (region.Y < 0) { region.Y = 0; }
                    if (region.Right > _Image.Width) { region.X -= (region.Right - _Image.Width); }
                    if (region.Bottom > _Image.Height) { region.Y -= (region.Bottom - _Image.Height); }
                }

                _ViewRegion = region;
            }
        }

        private static RectangleF IncrementalZoom(RectangleF region, float dz)
        {
            float dWidth = region.Width * dz;
            float dHeight = region.Height * dz;
            region.X -= dWidth / 2;
            region.Y -= dHeight / 2;
            region.Width += dWidth;
            region.Height += dHeight;
            return region;
        }

        private void ImageExplorer_Paint(object sender, PaintEventArgs e)
        {
            if (_CanvasGraphics == null) { return; }

            if (_Image != null)
            {
                _CanvasGraphics.Clear(this.BackColor);
                _CanvasGraphics.DrawImage(_Image, new RectangleF(0, 0, this.ClientSize.Width, this.ClientSize.Height), _ViewRegion, GraphicsUnit.Pixel);
                PaintImage?.Invoke(this, new PaintEventArgs(_CanvasGraphics, e.ClipRectangle));
            }

            e.Graphics.DrawImage(_Canvas, 0, 0);
        }

        private void ImageExplorer_Resize(object sender, EventArgs e)
        {
            if (_CanvasGraphics != null) { _CanvasGraphics.Dispose(); }
            if (_Canvas != null) { _Canvas.Dispose(); }
            _Canvas = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            _CanvasGraphics = Graphics.FromImage(_Canvas);

            if (_Image != null)
            {
                ViewRegion = _ViewRegion;
                this.Invalidate();
            }
        }
        
        private void ImageExplorer_MouseWheel(object sender, MouseEventArgs e)
        {
            // Record point in image under cursor
            float xImage = (float)e.X / this.ClientSize.Width * _ViewRegion.Width + _ViewRegion.X;
            float yImage = (float)e.Y / this.ClientSize.Height * _ViewRegion.Height + _ViewRegion.Y;

            // Adjust zoom
            float fraction = MOUSEWHEEL_ZOOM * e.Delta / SystemInformation.MouseWheelScrollDelta;
            RectangleF region = IncrementalZoom(ViewRegion, fraction);

            // Attempt to center point in image originally under cursor
            float xCenter = region.X + region.Width / 2;
            float yCenter = region.Y + region.Height / 2;
            float dx = xImage - xCenter;
            float dy = yImage - yCenter;
            region.X += xImage - xCenter;
            region.Y += yImage - yCenter;
            this.ViewRegion = region;

            // Move mouse cursor over point in image originally under cursor
            float dxCursor = (xImage - _ViewRegion.X) / _ViewRegion.Width * this.ClientSize.Width - e.X;
            float dYCursor = (yImage - _ViewRegion.Y) / _ViewRegion.Height * this.ClientSize.Height - e.Y;
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Position = new Point(Cursor.Position.X + (int)dxCursor, Cursor.Position.Y + (int)dYCursor);

            this.Invalidate();
        }

        private void ImageExplorer_MouseEnter(object sender, EventArgs e)
        {
            if (_Image != null)
            {
                this.Focus();
            }
        }
    }
}
