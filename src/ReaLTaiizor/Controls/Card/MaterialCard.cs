#region Imports

using ReaLTaiizor.Manager;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static ReaLTaiizor.Helper.MaterialDrawHelper;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MaterialCard

    public class MaterialCard : System.Windows.Forms.Panel, MaterialControlI
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialMouseState MouseState { get; set; }


        private float? _scaleRatio; // Cache
        private float ScaleFactor
        {
            get
            {
                if (!_scaleRatio.HasValue)
                {
                    _scaleRatio = SkinManager.GetDeviceScaleFactor(this);
                }
                return _scaleRatio.Value;
            }
            set
            {
                _scaleRatio = value;
            }
        }
        private float? _scaleRatioSqrt; // Cache
        private float ScaleFactorSqrt
        {
            get
            {
                if (!_scaleRatioSqrt.HasValue)
                {
                    _scaleRatioSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);
                }
                return _scaleRatioSqrt.Value;
            }
            set
            {
                _scaleRatioSqrt = value;
            }
        }

        public MaterialCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Paint += new PaintEventHandler(paintControl);
            BackColor = SkinManager.BackgroundColor;
            ForeColor = SkinManager.TextHighEmphasisColor;
            Margin = new Padding((int)(SkinManager.FORM_PADDING * ScaleFactor));
            Padding = new Padding((int)(SkinManager.FORM_PADDING * ScaleFactor));
        }

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            if (Parent == null)
            {
                RemoveShadowPaintEvent((Control)sender, drawShadowOnParent);
                return;
            }

            // paint shadow on parent
            Graphics gp = e.Graphics;
            Rectangle rect = new(Location, ClientRectangle.Size);
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawSquareShadow(gp, rect);
        }

        protected override void InitLayout()
        {
            LocationChanged += (sender, e) => 
            {
                ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
                ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this); 
                Parent?.Invalidate(); 
            };
            ForeColor = SkinManager.TextHighEmphasisColor;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent != null)
            {
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            }

            if (_oldParent != null)
            {
                RemoveShadowPaintEvent(_oldParent, drawShadowOnParent);
            }

            _oldParent = Parent;
        }

        private Control _oldParent;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Parent == null)
            {
                return;
            }

            if (Visible)
            {
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            }
            else
            {
                RemoveShadowPaintEvent(Parent, drawShadowOnParent);
            }
        }

        private bool _shadowDrawEventSubscribed = false;

        private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (_shadowDrawEventSubscribed)
            {
                return;
            }

            control.Paint += shadowPaintEvent;
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);
            control.Invalidate();
            _shadowDrawEventSubscribed = true;
        }

        private void RemoveShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (!_shadowDrawEventSubscribed)
            {
                return;
            }

            control.Paint -= shadowPaintEvent;
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);
            control.Invalidate();
            _shadowDrawEventSubscribed = false;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackColor = SkinManager.BackgroundColor;
        }

        private void paintControl(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.Clear(Parent.BackColor == Color.Transparent ? ((Parent.Parent == null || (Parent.Parent != null && Parent.Parent.BackColor == Color.Transparent)) ? SkinManager.BackgroundColor : Parent.Parent.BackColor) : Parent.BackColor);

            // card rectangle path
            RectangleF cardRectF = new(ClientRectangle.Location, ClientRectangle.Size);
            cardRectF.X -= 0.5f;
            cardRectF.Y -= 0.5f;
            GraphicsPath cardPath = CreateRoundRect(cardRectF, 4);

            // button shadow (blend with form shadow)
            DrawSquareShadow(g, ClientRectangle);

            // Draw card
            using SolidBrush normalBrush = new(BackColor);
            g.FillPath(normalBrush, cardPath);
        }
    }

    #endregion
}