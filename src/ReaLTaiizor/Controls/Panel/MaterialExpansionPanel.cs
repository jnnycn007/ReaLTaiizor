#region Imports

using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using static ReaLTaiizor.Helper.MaterialDrawHelper;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MaterialExpansionPanel

    public class MaterialExpansionPanel : System.Windows.Forms.Panel, MaterialControlI
    {
        #region "Private members"

        private MaterialButton _validationButton;
        private MaterialButton _cancelButton;

        private const int _expansionPanelDefaultPadding = 16;
        private const int _leftrightPadding = 24;
        private const int _buttonPadding = 8;
        private const int _expandcollapsbuttonsize = 24;
        private const int _textHeaderHeight = 24;
        private const int _headerHeightCollapse = 48;
        private const int _headerHeightExpand = 64;
        private const int _footerHeight = 68;
        private const int _footerButtonHeight = 36;
        private const int _minHeight = 200;
        private int _headerHeight;
        private int _expandHeight;
        private bool _shadowDrawEventSubscribed = false;
        private Rectangle _headerBounds;
        private Rectangle _expandcollapseBounds;
        private Rectangle _savebuttonBounds;
        private Rectangle _cancelbuttonBounds;

        private bool _needsLayoutRefresh = false;

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

            set => _scaleRatio = value;
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

            set => _scaleRatioSqrt = value;
        }

        private enum ButtonState
        {
            SaveOver,
            CancelOver,
            ColapseExpandOver,
            HeaderOver,
            None
        }

        private ButtonState _buttonState = ButtonState.None;

        #endregion

        #region "Public Properties"

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialMouseState MouseState { get; set; }

        [Category("Material"), DefaultValue(false), DisplayName("Use Accent Color")]
        public bool UseAccentColor
        {
            get;
            set { field = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue(false)]
        [Description("Collapses the control when set to true")]
        [Category("Material")]
        public bool Collapse
        {
            get;
            set
            {
                field = value;
                CollapseOrExpand();
                Invalidate();
            }
        }

        [DefaultValue("Title")]
        [Category("Material"), DisplayName("Title")]
        [Description("Title to show in expansion panel's header")]
        public string Title
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [DefaultValue("Description")]
        [Category("Material"), DisplayName("Description")]
        [Description("Description to show in expansion panel's header")]
        public string Description
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category("Material"), DisplayName("Draw Shadows")]
        [Description("Draw Shadows around control")]
        public bool DrawShadows
        {
            get;
            set { field = value; Invalidate(); }
        }

        [DefaultValue(240)]
        [Category("Material"), DisplayName("Expand Height")]
        [Description("Define control height when expanded")]
        public int ExpandHeight
        {
            get => _expandHeight;
            set { if (value < (int)(_minHeight * ScaleFactor)) { value = (int)(_minHeight * ScaleFactor); } _expandHeight = value; Invalidate(); }
        }

        [DefaultValue(true)]
        [Category("Material"), DisplayName("Show Collapse/Expand")]
        [Description("Show collapse/expand indicator")]
        public bool ShowCollapseExpand
        {
            get;
            set { field = value; Invalidate(); }
        }

        [DefaultValue(true)]
        [Category("Material"), DisplayName("Show Validation Buttons")]
        [Description("Show save/cancel button")]
        public bool ShowValidationButtons
        {
            get;
            set { field = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue("SAVE")]
        [Category("Material"), DisplayName("Validation Button Text")]
        [Description("Set Validation button text")]
        public string ValidationButtonText
        {
            get;
            set { field = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue("CANCEL")]
        [Category("Material"), DisplayName("Cancel Button Text")]
        [Description("Set Cancel button text")]
        public string CancelButtonText
        {
            get;
            set { field = value; UpdateRects(); Invalidate(); }
        }

        [DefaultValue(false)]
        [Category("Material"), DisplayName("Validation Button Enable")]
        [Description("Enable validation button")]
        public bool ValidationButtonEnable
        {
            get;
            set { field = value; UpdateRects(); Invalidate(); }
        }

        #endregion

        #region "Events"

        [Category("Action")]
        [Description("Fires when Save button is clicked")]
        public event EventHandler SaveClick;

        [Category("Action")]
        [Description("Fires when Cancel button is clicked")]
        public event EventHandler CancelClick;

        [Category("Disposition")]
        [Description("Fires when Panel Collapse")]
        public event EventHandler PanelCollapse;

        [Category("Disposition")]
        [Description("Fires when Panel Expand")]
        public event EventHandler PanelExpand;

        #endregion

        public MaterialExpansionPanel()
        {
            ShowValidationButtons = true;
            ValidationButtonEnable = false;
            ValidationButtonText = "SAVE";
            CancelButtonText = "CANCEL";
            ShowCollapseExpand = true;
            Collapse = false;
            Title = "Title";
            Description = "Description";
            DrawShadows = true;
            ExpandHeight = 240;
            AutoScroll = false;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = SkinManager.BackgroundColor;
            ForeColor = SkinManager.TextHighEmphasisColor;

            Padding = new Padding(24, 64, 24, 16);
            Margin = new Padding(3, 16, 3, 16);
            Size = new Size(480, ExpandHeight);

            //CollapseOrExpand();

            _validationButton = new MaterialButton
            {
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Enabled = ValidationButtonEnable,
                Visible = ShowValidationButtons,
                Text = "SAVE"
            };
            _cancelButton = new MaterialButton
            {
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Visible = ShowValidationButtons,
                Text = "CANCEL"
            };

            if (!Controls.Contains(_validationButton))
            {
                Controls.Add(_validationButton);
            }
            if (!Controls.Contains(_cancelButton))
            {
                Controls.Add(_cancelButton);
            }

            _validationButton.Click += _validationButton_Click;
            _cancelButton.Click += _cancelButton_Click;

            UpdateRects();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            CancelClick?.Invoke(this, new EventArgs());
            Collapse = true;
            CollapseOrExpand();
        }

        private void _validationButton_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            SaveClick?.Invoke(this, new EventArgs());
            Collapse = true;
            CollapseOrExpand();
        }

        protected override void OnCreateControl()
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            base.OnCreateControl();
            Font = SkinManager.GetFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor);
        }

        protected override void InitLayout()
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            LocationChanged += (sender, e) => { Parent?.Invalidate(); };
            ForeColor = SkinManager.TextHighEmphasisColor;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

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

        private void drawShadowOnParent(object sender, PaintEventArgs e)
        {
            if (Parent == null)
            {
                RemoveShadowPaintEvent((Control)sender, drawShadowOnParent);
                return;
            }

            if (!DrawShadows || Parent == null)
            {
                return;
            }

            // paint shadow on parent
            Graphics gp = e.Graphics;
            Rectangle rect = new(Location, ClientRectangle.Size);
            gp.SmoothingMode = SmoothingMode.AntiAlias;
            DrawSquareShadow(gp, rect);
        }


        private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
        {
            if (_shadowDrawEventSubscribed)
            {
                return;
            }

            control.Paint += shadowPaintEvent;
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
            control.Invalidate();
            _shadowDrawEventSubscribed = false;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            BackColor = SkinManager.BackgroundColor;
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            base.OnDpiChangedAfterParent(e);
        }

        protected override void OnResize(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            if (!Collapse)
            {
                if (DesignMode)
                {
                    _expandHeight = (int)(Height / ScaleFactor);
                }
                if (Height < (int)(_minHeight * ScaleFactor))
                {
                    Height = (int)(_minHeight * ScaleFactor);
                }
            }
            else
            {
                Height = (int)(_headerHeightCollapse * ScaleFactor);
            }

            base.OnResize(e);

            _headerBounds = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, (int)(_headerHeight * ScaleFactor));
            _expandcollapseBounds = new Rectangle(Width - (int)(_leftrightPadding * ScaleFactor) - (int)(_expandcollapsbuttonsize * ScaleFactor), (int)(((int)(_headerHeight * ScaleFactor) - (int)(_expandcollapsbuttonsize * ScaleFactor)) / 2), (int)(_expandcollapsbuttonsize * ScaleFactor), (int)(_expandcollapsbuttonsize * ScaleFactor));

            UpdateRects();

            if (Parent != null)
            {
                RemoveShadowPaintEvent(Parent, drawShadowOnParent);
                AddShadowPaintEvent(Parent, drawShadowOnParent);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
            {
                return;
            }

            ButtonState oldState = _buttonState;

            if (_savebuttonBounds.Contains(e.Location))
            {
                _buttonState = ButtonState.SaveOver;
            }
            else if (_cancelbuttonBounds.Contains(e.Location))
            {
                _buttonState = ButtonState.CancelOver;
            }
            else if (_expandcollapseBounds.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                _buttonState = ButtonState.ColapseExpandOver;
            }
            else if (_headerBounds.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                _buttonState = ButtonState.HeaderOver;
            }
            else
            {
                Cursor = Cursors.Default;
                _buttonState = ButtonState.None;
            }

            if (oldState != _buttonState)
            {
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Enabled && (_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver))
            {
                Collapse = !Collapse;
                CollapseOrExpand();
            }
            else
            {
                if (DesignMode)
                {
                    return;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode)
            {
                return;
            }

            Cursor = Cursors.Arrow;
            _buttonState = ButtonState.None;
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _needsLayoutRefresh = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_needsLayoutRefresh)
            {
                UpdateRects();
                _needsLayoutRefresh = false;
            }

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(Parent.BackColor == Color.Transparent ? ((Parent.Parent == null || (Parent.Parent != null && Parent.Parent.BackColor == Color.Transparent)) ? SkinManager.BackgroundColor : Parent.Parent.BackColor) : Parent.BackColor);

            // card rectangle path
            RectangleF expansionPanelRectF = new(ClientRectangle.Location, ClientRectangle.Size);
            expansionPanelRectF.X -= 0.5f;
            expansionPanelRectF.Y -= 0.5f;
            GraphicsPath expansionPanelPath = CreateRoundRect(expansionPanelRectF, 2);

            // button shadow (blend with form shadow)
            DrawSquareShadow(g, ClientRectangle);

            // Draw expansion panel
            // Disabled
            if (!Enabled)
            {
                using SolidBrush disabledBrush = new(BlendColor(Parent.BackColor, SkinManager.BackgroundDisabledColor, SkinManager.BackgroundDisabledColor.A));
                g.FillPath(disabledBrush, expansionPanelPath);
            }
            // Mormal
            else
            {
                if ((_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver) && Collapse)
                {
                    RectangleF expansionPanelBorderRectF = new(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);
                    expansionPanelBorderRectF.X -= 0.5f;
                    expansionPanelBorderRectF.Y -= 0.5f;
                    GraphicsPath expansionPanelBoarderPath = CreateRoundRect(expansionPanelBorderRectF, 2);

                    g.FillPath(SkinManager.ExpansionPanelFocusBrush, expansionPanelBoarderPath);
                }
                else
                {
                    using SolidBrush normalBrush = new(SkinManager.BackgroundColor);
                    g.FillPath(normalBrush, expansionPanelPath);
                }
            }

            // Calc text Rect
            Rectangle headerRect = new(
                (int)(_leftrightPadding * ScaleFactor),
                ((int)(_headerHeight * ScaleFactor) - (int)(_textHeaderHeight * ScaleFactor)) / 2,
                TextRenderer.MeasureText(Title, SkinManager.GetFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor)).Width + (int)(_expansionPanelDefaultPadding * ScaleFactor),
                (int)(_textHeaderHeight * ScaleFactor));

            //Draw  headers
            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    Title,
                    SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    headerRect.Location,
                    headerRect.Size,
                    MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }

            if (!string.IsNullOrEmpty(Description))
            {
                //Draw description header text 

                Rectangle headerDescriptionRect = new(
                    headerRect.Right + (int)(_expansionPanelDefaultPadding * ScaleFactor),
                    ((int)(_headerHeight * ScaleFactor) - (int)(_textHeaderHeight * ScaleFactor)) / 2,
                    _expandcollapseBounds.Left - (headerRect.Right + (int)(_expansionPanelDefaultPadding * ScaleFactor)) - (int)(_expansionPanelDefaultPadding * ScaleFactor),
                    (int)(_textHeaderHeight * ScaleFactor));

                using MaterialNativeTextRenderer NativeText = new(g);
                // Draw description header text 
                NativeText.DrawTransparentText(
                Description,
                SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor),
                 SkinManager.TextDisabledOrHintColor,
                headerDescriptionRect.Location,
                headerDescriptionRect.Size,
                MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }

            if (ShowCollapseExpand == true)
            {
                using Pen formButtonsPen = new(UseAccentColor && Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.TextDisabledOrHintColor, 2);
                if (Collapse)
                {
                    //Draw Expand button
                    GraphicsPath pth = new();
                    PointF TopLeft = new(_expandcollapseBounds.X + (6 * ScaleFactor), _expandcollapseBounds.Y + (9 * ScaleFactor));
                    PointF MidBottom = new(_expandcollapseBounds.X + (12 * ScaleFactor), _expandcollapseBounds.Y + (15 * ScaleFactor));
                    PointF TopRight = new(_expandcollapseBounds.X + (18 * ScaleFactor), _expandcollapseBounds.Y + (9 * ScaleFactor));
                    pth.AddLine(TopLeft, MidBottom);
                    pth.AddLine(TopRight, MidBottom);
                    g.DrawPath(formButtonsPen, pth);
                    // Note: the numeric offsets above are magic numbers defining the expand glyph shape relative to _expandcollapseBounds and ScaleFactor.
                }
                else
                {
                    // Draw Collapse button
                    GraphicsPath pth = new();
                    PointF BottomLeft = new(_expandcollapseBounds.X + (6 * ScaleFactor), _expandcollapseBounds.Y + (15 * ScaleFactor));
                    PointF MidTop = new(_expandcollapseBounds.X + (12 * ScaleFactor), _expandcollapseBounds.Y + (9 * ScaleFactor));
                    PointF BottomRight = new(_expandcollapseBounds.X + (18 * ScaleFactor), _expandcollapseBounds.Y + (15 * ScaleFactor));
                    pth.AddLine(BottomLeft, MidTop);
                    pth.AddLine(BottomRight, MidTop);
                    g.DrawPath(formButtonsPen, pth);
                }
            }

            if (!Collapse && ShowValidationButtons)
            {
                //Draw divider
                using Pen dividerPen = new(SkinManager.DividersColor, 1);
                g.DrawLine(dividerPen, new Point(0, Height - (int)(_footerHeight * ScaleFactor)), new Point(Width, Height - (int)(_footerHeight * ScaleFactor)));
            }
        }

        private void CollapseOrExpand()
        {
            //if (!useAnimation)
            //{
            if (Collapse)
            {
                _headerHeight = _headerHeightCollapse;
                this.Height = (int)(_headerHeightCollapse * ScaleFactor);
                Margin = new Padding(16, 1, 16, 0);

                // Is the event registered?
                if (PanelCollapse != null)
                {
                    // Raise the event
                    this.PanelCollapse(this, new EventArgs());
                }
            }
            else
            {
                _headerHeight = _headerHeightExpand;
                this.Height = (int)(_expandHeight * ScaleFactor);
                Margin = new Padding(16, 16, 16, 16);

                // Is the event registered?
                if (PanelExpand != null)
                {
                    // Raise the event
                    this.PanelExpand(this, new EventArgs());
                }
            }

            Refresh();
        }

        private void UpdateRects()
        {
            if (!Collapse && ShowValidationButtons)
            {
                int _buttonWidth = TextRenderer.MeasureText(ValidationButtonText, SkinManager.GetFontByType(MaterialSkinManager.FontType.Button, ScaleFactor)).Width + 32;
                _savebuttonBounds = new Rectangle(Width - (int)(_buttonPadding * ScaleFactor) - _buttonWidth, Height - (int)(_expansionPanelDefaultPadding * ScaleFactor) - (int)(_footerButtonHeight * ScaleFactor), _buttonWidth, (int)(_footerButtonHeight * ScaleFactor));
                _buttonWidth = TextRenderer.MeasureText(CancelButtonText, SkinManager.GetFontByType(MaterialSkinManager.FontType.Button, ScaleFactor)).Width + 32;
                _cancelbuttonBounds = new Rectangle(_savebuttonBounds.Left - (int)(_buttonPadding * ScaleFactor) - _buttonWidth, Height - (int)(_expansionPanelDefaultPadding * ScaleFactor) - (int)(_footerButtonHeight * ScaleFactor), _buttonWidth, (int)(_footerButtonHeight * ScaleFactor));

                if (_validationButton != null)
                {
                    _validationButton.Width = _savebuttonBounds.Width;
                    _validationButton.Left = Width - (int)(_buttonPadding * ScaleFactor) - _validationButton.Width;  //Button minimum width management
                    _validationButton.Top = _savebuttonBounds.Top;
                    _validationButton.Height = _savebuttonBounds.Height;
                    _validationButton.Text = ValidationButtonText;
                    _validationButton.Enabled = ValidationButtonEnable;
                    _validationButton.UseAccentColor = UseAccentColor;
                }
                if (_cancelButton != null)
                {
                    _cancelButton.Width = _cancelbuttonBounds.Width;
                    _cancelButton.Left = _validationButton.Left - (int)(_buttonPadding * ScaleFactor) - _cancelbuttonBounds.Width;  //Button minimum width management
                    _cancelButton.Top = _cancelbuttonBounds.Top;
                    _cancelButton.Height = _cancelbuttonBounds.Height;
                    _cancelButton.Text = CancelButtonText;
                    _cancelButton.UseAccentColor = UseAccentColor;
                }
            }
            if (_validationButton != null)
            {
                _validationButton.Visible = ShowValidationButtons;
            }
            if (_cancelButton != null)
            {
                _cancelButton.Visible = ShowValidationButtons;
            }
        }

    }

    #endregion
}