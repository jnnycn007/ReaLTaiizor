#region Imports

using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static ReaLTaiizor.Helper.MaterialDrawHelper;
using static ReaLTaiizor.Util.MaterialAnimations;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MaterialComboBox

    public class MaterialComboBox : ComboBox, MaterialControlI
    {
        // For some reason, even when overriding the AutoSize property, it doesn't appear on the properties panel, so we have to create a new one.
        [field: Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Layout")]
        public bool AutoResize
        {
            get;
            set
            {
                field = value;
                recalculateAutoSize();
            }
        }

        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialMouseState MouseState { get; set; }

        [Category("Material"), DefaultValue(true), Description("Using a larger size enables the hint to always be visible")]
        public bool UseTallSize
        {
            get;
            set
            {
                field = value;
                setHeightVars();
                Invalidate();
            }
        }

        [Category("Material"), DefaultValue(true)]
        public bool UseAccent { get; set; }

        [Category("Material"), DefaultValue(""), Localizable(true)]
        public string Hint
        {
            get;
            set
            {
                field = value;
                hasHint = !string.IsNullOrEmpty(Hint);
                Invalidate();
            }
        } = string.Empty;

        public int StartIndex
        {
            get;
            set
            {
                field = value;
                try
                {
                    if (base.Items.Count > 0)
                    {
                        base.SelectedIndex = value;
                    }
                }
                catch
                {
                }
                Invalidate();
            }
        }

        private const int TEXT_SMALL_SIZE = 18;
        private const int TEXT_SMALL_Y = 4;
        private const int BOTTOM_PADDING = 3;
        private const int DEFAULT_HEIGHT = 50; // DO NOT REPLACE WITH RATIO FORMULA
        private const int HEIGHT_NOT_TALL = 36; // DO NOT REPLACE WITH RATIO FORMULA
        private const int PADDING_CONST = 7;
        private const int DROPDOWN_PADDING = 2;
        private const int HINTRECT_PADDING = 2;
        private const int INDICATOR_HEIGHT = 2;
        private const int CLIENT_PADDING = 8;
        private int HEIGHT = 50;
        private int LINE_Y;

        int hintTextSize = 16;

        private bool hasHint;


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

        private readonly AnimationManager _animationManager;

        public MaterialComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            // Material Properties
            Hint = "";
            UseAccent = true;
            UseTallSize = true;
            MaxDropDownItems = 4;

            Font = SkinManager.GetFontByType(MaterialSkinManager.FontType.Subtitle2, ScaleFactor);
            BackColor = SkinManager.BackgroundColor;
            ForeColor = SkinManager.TextHighEmphasisColor;
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DropDownWidth = Width;

            // Animations
            _animationManager = new AnimationManager(true)
            {
                Increment = 0.08,
                AnimationType = AnimationType.EaseInOut
            };
            _animationManager.OnAnimationProgress += sender => Invalidate();
            _animationManager.OnAnimationFinished += sender => _animationManager.SetProgress(0);
            DropDownClosed += (sender, args) =>
            {
                MouseState = MaterialMouseState.OUT;
                if (SelectedIndex < 0 && !Focused)
                {
                    _animationManager.StartNewAnimation(AnimationDirection.Out);
                }
            };
            LostFocus += (sender, args) =>
            {
                MouseState = MaterialMouseState.OUT;
                if (SelectedIndex < 0)
                {
                    _animationManager.StartNewAnimation(AnimationDirection.Out);
                }
            };
            DropDown += (sender, args) =>
            {
                _animationManager.StartNewAnimation(AnimationDirection.In);
            };
            GotFocus += (sender, args) =>
            {
                _animationManager.StartNewAnimation(AnimationDirection.In);
                Invalidate();
            };
            MouseEnter += (sender, args) =>
            {
                MouseState = MaterialMouseState.HOVER;
                Invalidate();
            };
            MouseLeave += (sender, args) =>
            {
                MouseState = MaterialMouseState.OUT;
                Invalidate();
            };
            SelectedIndexChanged += (sender, args) =>
            {
                Invalidate();
            };
            KeyUp += (sender, args) =>
            {
                if (Enabled && DropDownStyle == ComboBoxStyle.DropDownList && (args.KeyCode == Keys.Delete || args.KeyCode == Keys.Back))
                {
                    SelectedIndex = -1;
                    Invalidate();
                }
            };
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            base.OnDpiChangedAfterParent(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            g.Clear(Parent.BackColor == Color.Transparent ? ((Parent.Parent == null || (Parent.Parent != null && Parent.Parent.BackColor == Color.Transparent)) ? SkinManager.BackgroundColor : Parent.Parent.BackColor) : Parent.BackColor);
            g.FillRectangle(Enabled ? Focused ?
                SkinManager.BackgroundFocusBrush : // Focused
                MouseState == MaterialMouseState.HOVER ?
                SkinManager.BackgroundHoverBrush : // Hover
                SkinManager.BackgroundAlternativeBrush : // normal
                SkinManager.BackgroundDisabledBrush // Disabled
                , ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, LINE_Y);

            //Set color and brush
            Color SelectedColor = new();
            if (UseAccent)
            {
                SelectedColor = SkinManager.ColorScheme.AccentColor;
            }
            else
            {
                SelectedColor = SkinManager.ColorScheme.PrimaryColor;
            }

            SolidBrush SelectedBrush = new(SelectedColor);

            // Create and Draw the arrow
            GraphicsPath pth = new();
            PointF TopRight = new(this.Width - 0.5f * ScaleFactor - SkinManager.FORM_PADDING, (this.Height >> 1) - 2.5f * ScaleFactor);
            PointF MidBottom = new(this.Width - 4.5f * ScaleFactor - SkinManager.FORM_PADDING, (this.Height >> 1) + 2.5f * ScaleFactor);
            PointF TopLeft = new(this.Width - 8.5f * ScaleFactor - SkinManager.FORM_PADDING, (this.Height >> 1) - 2.5f * ScaleFactor);
            // Magic numbers warning
#warning Magic Numbers
            pth.AddLine(TopLeft, TopRight);
            pth.AddLine(TopRight, MidBottom);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath((SolidBrush)(Enabled ? DroppedDown || Focused ?
                SelectedBrush : //DroppedDown or Focused
                SkinManager.TextHighEmphasisBrush : //Not DroppedDown and not Focused
                new SolidBrush(BlendColor(SkinManager.TextHighEmphasisColor, SkinManager.SwitchOffDisabledThumbColor, 197))  //Disabled
                ), pth);
            g.SmoothingMode = SmoothingMode.None;

            // HintText
            bool userTextPresent = SelectedIndex >= 0;
            Rectangle hintRect = new(SkinManager.FORM_PADDING, ClientRectangle.Y, Width, LINE_Y);

            // bottom line base
            g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, LINE_Y, Width, (int)(INDICATOR_HEIGHT) / 2);

            if (!_animationManager.IsAnimating())
            {
                // No animation
                if (hasHint && UseTallSize && (DroppedDown || Focused || SelectedIndex >= 0))
                {
                    // hint text
                    hintRect = new Rectangle(SkinManager.FORM_PADDING, (int)(TEXT_SMALL_Y * ScaleFactor), Width, (int)(TEXT_SMALL_SIZE * ScaleFactor));
                    hintTextSize = 12;
                }

                // bottom line
                if (DroppedDown || Focused)
                {
                    g.FillRectangle(SelectedBrush, 0, LINE_Y, Width, (int)(INDICATOR_HEIGHT * ScaleFactor));
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // hint Animation
                if (hasHint && UseTallSize)
                {
                    hintRect = new Rectangle(
                        SkinManager.FORM_PADDING,
                        userTextPresent && !_animationManager.IsAnimating() ? (int)(TEXT_SMALL_Y * ScaleFactor) : ClientRectangle.Y + (int)(((int)(TEXT_SMALL_Y * ScaleFactor) - ClientRectangle.Y) * animationProgress),
                        Width,
                        userTextPresent && !_animationManager.IsAnimating() ? (int)(TEXT_SMALL_SIZE * ScaleFactor) : (int)(LINE_Y + (((int)(TEXT_SMALL_SIZE * ScaleFactor) - LINE_Y) * animationProgress)));
                    hintTextSize = userTextPresent && !_animationManager.IsAnimating() ? 12 : (int)(16 + ((12 - 16) * animationProgress));
                }

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(SelectedBrush, LineAnimationX, LINE_Y, LineAnimationWidth, (int)(INDICATOR_HEIGHT * ScaleFactor));
            }

            // Calc text Rect
            Rectangle textRect = new(
                SkinManager.FORM_PADDING,
                hasHint && UseTallSize ? hintRect.Y + hintRect.Height - (int)(HINTRECT_PADDING * ScaleFactor) : ClientRectangle.Y,
                ClientRectangle.Width - (SkinManager.FORM_PADDING * 3) - (int)((int)(CLIENT_PADDING * ScaleFactor) * ScaleFactor),
                hasHint && UseTallSize ? LINE_Y - (hintRect.Y + hintRect.Height) : LINE_Y);

            g.Clip = new Region(textRect);

            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                // Draw user text
                NativeText.DrawTransparentText(
                    Text,
                    SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Subtitle1, ScaleFactor),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    textRect.Location,
                    textRect.Size,
                    MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }

            g.ResetClip();

            // Draw hint text
            if (hasHint && (UseTallSize || string.IsNullOrEmpty(Text)))
            {
                using MaterialNativeTextRenderer NativeText = new(g);
                NativeText.DrawTransparentText(
                Hint,
                SkinManager.GetTextBoxFontBySize(hintTextSize, ScaleFactor),
                Enabled ? DroppedDown || Focused ?
                SelectedColor : // Focus 
                SkinManager.TextMediumEmphasisColor : // not focused
                SkinManager.TextDisabledOrHintColor, // Disabled
                hintRect.Location,
                hintRect.Size,
                MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }
        }

        private void CustomMeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)(HEIGHT * ScaleFactor) - (int)(PADDING_CONST * ScaleFactor);
        }

        private void CustomDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index > Items.Count || !Focused)
            {
                return;
            }

            Graphics g = e.Graphics;

            // Draw the background of the item.
            g.FillRectangle(SkinManager.BackgroundBrush, e.Bounds);

            // Hover
            if (e.State.HasFlag(DrawItemState.Focus)) // Focus == hover
            {
                g.FillRectangle(SkinManager.BackgroundHoverBrush, e.Bounds);
            }

            string Text = "";
            if (!string.IsNullOrWhiteSpace(DisplayMember))
            {
                if (!Items[e.Index].GetType().Equals(typeof(DataRowView)))
                {
                    object item = Items[e.Index].GetType().GetProperty(DisplayMember).GetValue(Items[e.Index]);
                    Text = item.ToString();
                }
                else
                {
                    DataTable table = ((DataRow)Items[e.Index].GetType().GetProperty("Row").GetValue(Items[e.Index])).Table;
                    Text = table.Rows[e.Index][DisplayMember].ToString();
                }
            }
            else
            {
                Text = Items[e.Index].ToString();
            }

            using MaterialNativeTextRenderer NativeText = new(g);
            NativeText.DrawTransparentText(
            Text,
            SkinManager.GetFontByType(MaterialSkinManager.FontType.Subtitle1, ScaleFactor),
            SkinManager.TextHighEmphasisNoAlphaColor,
            new Point(e.Bounds.Location.X + SkinManager.FORM_PADDING, e.Bounds.Location.Y),
            new Size(e.Bounds.Size.Width - (SkinManager.FORM_PADDING * 2), e.Bounds.Size.Height),
            MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle); ;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            MouseState = MaterialMouseState.OUT;
            MeasureItem += CustomMeasureItem;
            DrawItem += CustomDrawItem;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawVariable;
            recalculateAutoSize();
            setHeightVars();
        }

        protected override void OnResize(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            base.OnResize(e);
            recalculateAutoSize();
            setHeightVars();
        }

        private void setHeightVars()
        {
            HEIGHT = UseTallSize ? DEFAULT_HEIGHT : HEIGHT_NOT_TALL;
            Size = new Size(Size.Width, (int)(HEIGHT * ScaleFactor));
            LINE_Y = Height - INDICATOR_HEIGHT;
            ItemHeight = (int)(HEIGHT * ScaleFactor) - (int)(PADDING_CONST * ScaleFactor);
            DropDownHeight = (ItemHeight * MaxDropDownItems) + (int)(DROPDOWN_PADDING * ScaleFactor);
        }

        public void recalculateAutoSize()
        {
            if (!AutoResize)
            {
                return;
            }

            int w = DropDownWidth;
            int padding = SkinManager.FORM_PADDING * 3;
            int vertScrollBarWidth = (Items.Count > MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

            Graphics g = CreateGraphics();
            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                System.Collections.Generic.IEnumerable<string> itemsList = this.Items.Cast<object>().Select(item => item.ToString());
                foreach (string s in itemsList)
                {
                    int newWidth = NativeText.MeasureLogString(s, SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Subtitle1, ScaleFactor)).Width + vertScrollBarWidth + padding;
                    if (w < newWidth)
                    {
                        w = newWidth;
                    }
                }
            }

            if (Width != w)
            {
                DropDownWidth = w;
                Width = w;
            }
        }
    }

    #endregion
}