using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer
{
    internal class Canvas : PictureBox
    {

        public static readonly float[] ZoomModes = [0.5f, 1f, 2f, 3f, 4f, 5f];
        public const int DEFAULT_ZOOM_MODE = 1;

        public event Action<int>? ZoomChanged;

        private int _zoomMode = DEFAULT_ZOOM_MODE;
        private Point? lastDragPosition;
        private Point offset = Point.Empty;

        public int ZoomMode
        {
            get => _zoomMode;
            set => SetZoom(value);
        }

        public Canvas()
        {
            BackColor = Color.Black;
            DoubleBuffered = true;
            //SetStyle(ControlStyles.Selectable, true);
            //UpdateStyles();
        }

        public void ResetView()
        {
            _zoomMode = DEFAULT_ZOOM_MODE;
            ZoomChanged?.Invoke(_zoomMode);
            offset = Point.Empty;
            Invalidate();
        }

        private void SetZoom(int newZoomMode)
        {
            if (newZoomMode != _zoomMode)
            {
                _zoomMode = newZoomMode;
                ZoomChanged?.Invoke(newZoomMode);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) != 0)
            {
                if (lastDragPosition != null)
                {
                    int x = lastDragPosition.Value.X - e.X;
                    int y = lastDragPosition.Value.Y - e.Y;

                    offset = new Point(offset.X - x, offset.Y - y);

                    Invalidate();
                }

                lastDragPosition = e.Location;
            }
            else
            {
                lastDragPosition = null;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ResetView();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int newZoomMode = Math.Clamp(_zoomMode + Math.Sign(e.Delta), 0, ZoomModes.Length - 1);
            SetZoom(newZoomMode);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            float zoomFactor = ZoomModes[_zoomMode];

            pe.Graphics.TranslateTransform(offset.X, offset.Y);
            pe.Graphics.ScaleTransform(zoomFactor, zoomFactor);
            base.OnPaint(pe);
        }

    }
}
