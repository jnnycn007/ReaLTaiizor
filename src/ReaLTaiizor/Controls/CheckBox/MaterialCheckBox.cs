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
using static ReaLTaiizor.Util.MaterialAnimations;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MaterialCheckBox

    public class MaterialCheckBox : System.Windows.Forms.CheckBox, MaterialControlI
    {
        #region Public properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialMouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category("Material")]
        public bool UseAccentColor
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public bool Ripple
        {
            get;
            set
            {
                field = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    Margin = new Padding(0);
                }

                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool ReadOnly { get; set; }
        #endregion

        #region Private fields
        private readonly AnimationManager _checkAM;
        private readonly AnimationManager _rippleAM;
        private readonly AnimationManager _hoverAM;
        private const int HEIGHT_RIPPLE = 37;
        private const int HEIGHT_NO_RIPPLE = 40;
        private const int TEXT_OFFSET = 26;
        private const int CHECKBOX_SIZE = 18;
        private const int CHECKBOX_SIZE_HALF = CHECKBOX_SIZE / 2;
        private int _boxOffset;
        private static Point[] CheckmarkLine;
        private bool hovered = false;
        private CheckState _oldCheckState;
        #endregion

        #region Constructor
        public MaterialCheckBox()
        {
            _checkAM = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            _hoverAM = new AnimationManager(true)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10
            };
            _rippleAM = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };
            CheckedChanged += (sender, args) =>
            {
                if (Ripple)
                {
                    _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
                }
            };
            _checkAM.OnAnimationProgress += sender => Invalidate();
            _hoverAM.OnAnimationProgress += sender => Invalidate();
            _rippleAM.OnAnimationProgress += sender => Invalidate();

            Ripple = true;
            Height = (int)(HEIGHT_RIPPLE * GetDeviceScaleFactor());
            MouseLocation = new Point(-1, -1);
        }
        #endregion

        #region Overridden events
        private bool _resizing = false;
        protected override void OnSizeChanged(EventArgs e)
        {
            if (_resizing)
            {
                base.OnSizeChanged(e);
                return;
            }

            _resizing = true;
            Width = (int)(Width * GetDeviceScaleFactor());
            base.OnSizeChanged(e);
            
            _boxOffset = ((int)(HEIGHT_RIPPLE * GetDeviceScaleFactor()) / 2) - (int)(9 * GetDeviceScaleFactor());
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
                PerformLayout();
                Invalidate();
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
                PerformLayout();
                Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size strSize;

            using (MaterialNativeTextRenderer NativeText = new(CreateGraphics()))
            {
                strSize = NativeText.MeasureLogString(Text, SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Body1));
            }

            float scale = GetDeviceScaleFactor();
            int boxOffsetLocal = ((int)(HEIGHT_RIPPLE * scale) / 2) - (int)(9 * scale);

            int w = boxOffsetLocal + (int)(TEXT_OFFSET * scale) + strSize.Width;
            int h = Ripple ? (int)(HEIGHT_RIPPLE * scale) : (int)(HEIGHT_NO_RIPPLE * scale);
            return new Size(w, h);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // clear the control
            g.Clear(Parent.BackColor == Color.Transparent ? ((Parent.Parent == null || (Parent.Parent != null && Parent.Parent.BackColor == Color.Transparent)) ? SkinManager.BackgroundColor : Parent.Parent.BackColor) : Parent.BackColor);

            int CHECKBOX_CENTER = _boxOffset + (int)(CHECKBOX_SIZE_HALF * GetDeviceScaleFactor()) - 1;
            Point animationSource = new(CHECKBOX_CENTER, CHECKBOX_CENTER);
            double animationProgress = _checkAM.GetProgress();

            int colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.CheckBoxOffDisabledColor.A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.CheckboxOffColor.A * (1.0 - animationProgress)) : SkinManager.CheckBoxOffDisabledColor.A;
            int rippleHeight = ((int)(HEIGHT_RIPPLE * GetDeviceScaleFactor()) % 2 == 0) ? (int)(HEIGHT_RIPPLE * GetDeviceScaleFactor()) - 3 : (int)(HEIGHT_RIPPLE * GetDeviceScaleFactor()) - 2;

            SolidBrush brush = new(Color.FromArgb(colorAlpha, Enabled ? UseAccentColor ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor : SkinManager.CheckBoxOffDisabledColor));
            Pen pen = new(brush.Color, 2);

            // draw hover animation
            if (Ripple)
            {
                double animationValue = _hoverAM.IsAnimating() ? _hoverAM.GetProgress() : hovered ? 1 : 0;
                int rippleSize = (int)(rippleHeight * (0.7 + (0.3 * animationValue)));

                using SolidBrush rippleBrush = new(Color.FromArgb((int)(40 * animationValue),
                    !Checked ? (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White) : brush.Color)); // no animation
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - (rippleSize / 2), animationSource.Y - (rippleSize / 2), rippleSize, rippleSize));
            }

            // draw ripple animation
            if (Ripple && _rippleAM.IsAnimating())
            {
                for (int i = 0; i < _rippleAM.GetAnimationCount(); i++)
                {
                    double animationValue = _rippleAM.GetProgress(i);
                    int rippleSize = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.7 + (0.3 * animationValue))) : rippleHeight;

                    using SolidBrush rippleBrush = new(Color.FromArgb((int)(animationValue * 40), !Checked ? (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White) : brush.Color));
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - (rippleSize / 2), animationSource.Y - (rippleSize / 2), rippleSize, rippleSize));
                }
            }

            Rectangle checkMarkLineFill = new(_boxOffset, _boxOffset, (int)((int)(CHECKBOX_SIZE * GetDeviceScaleFactor()) * animationProgress), (int)(CHECKBOX_SIZE * GetDeviceScaleFactor()));
            using (GraphicsPath checkmarkPath = CreateRoundRect(_boxOffset - 0.5f, _boxOffset - 0.5f, (int)(CHECKBOX_SIZE * GetDeviceScaleFactor()), (int)(CHECKBOX_SIZE * GetDeviceScaleFactor()), 1))
            {
                if (Enabled)
                {
                    using (Pen pen2 = new(BlendColor(Parent.BackColor, Enabled ? SkinManager.CheckboxOffColor : SkinManager.CheckBoxOffDisabledColor, backgroundAlpha), 2))
                    {
                        g.DrawPath(pen2, checkmarkPath);
                    }

                    g.DrawPath(pen, checkmarkPath);
                    g.FillPath(brush, checkmarkPath);
                }
                else
                {
                    if (Checked)
                    {
                        g.FillPath(brush, checkmarkPath);
                    }
                    else
                    {
                        g.DrawPath(pen, checkmarkPath);
                    }
                }

                g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
            }

            // draw checkbox text
            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                Rectangle textLocation = new(_boxOffset + (int)(TEXT_OFFSET * GetDeviceScaleFactor()), (int)(GetDeviceScaleFactor()), Width - (_boxOffset + (int)(TEXT_OFFSET * GetDeviceScaleFactor())), (int)(HEIGHT_RIPPLE * GetDeviceScaleFactor()));
                NativeText.DrawTransparentText(Text, SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Body1, GetDeviceScaleFactor()),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    textLocation.Location,
                    textLocation.Size,
                    MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }

            // dispose used paint objects
            pen.Dispose();
            brush.Dispose();
        }

        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    Size = GetPreferredSize(Size.Empty);
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (DesignMode)
            {
                return;
            }

            MouseState = MaterialMouseState.OUT;

            GotFocus += (sender, AddingNewEventArgs) =>
            {
                if (Ripple && !hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                    hovered = true;
                }
            };

            LostFocus += (sender, args) =>
            {
                if (Ripple && hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
            };

            MouseEnter += (sender, args) =>
            {
                MouseState = MaterialMouseState.HOVER;
                //if (Ripple && !hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                //    hovered = true;
                //}
                _oldCheckState = CheckState;
            };

            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MaterialMouseState.OUT;
                //if (Ripple && hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                //    hovered = false;
                //}
            };

            MouseDown += (sender, args) =>
            {
                MouseState = MaterialMouseState.DOWN;
                if (Ripple)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                }
                if (ReadOnly)
                {
                    CheckState = _oldCheckState;
                }
            };

            KeyDown += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space) && _rippleAM.GetAnimationCount() == 0)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                }
                if (ReadOnly)
                {
                    CheckState = _oldCheckState;
                }
            };

            MouseUp += (sender, args) =>
            {
                if (Ripple)
                {
                    MouseState = MaterialMouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
                if (ReadOnly)
                {
                    CheckState = _oldCheckState;
                }
            };

            KeyUp += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space))
                {
                    MouseState = MaterialMouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                }
                if (ReadOnly)
                {
                    CheckState = _oldCheckState;
                }
            };

            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }
        #endregion

        #region Private events and methods
        private float GetDeviceScaleFactor()
        {
            // 96 is Windows default scaling DPI（100% scale）
            float scalingFactor = (float)this.DeviceDpi / 96f;
            // Since a 'replace' to code will lead to operate scaling twice, this method provide the sqrt value of a factor.
            // Buttons are not included in sqrt controls.
            return scalingFactor;
        }

        private float GetDeviceScaleFactorSqrt()
        {
            // 96 is Windows default scaling DPI（100% scale）
            float scalingFactor = (float)Math.Sqrt((float)this.DeviceDpi / 96f);
            // Since a 'replace' to code will lead to operate scaling twice, this method provide the sqrt value of a factor.
            return scalingFactor;
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap checkMark = new((int)(CHECKBOX_SIZE * GetDeviceScaleFactor()), (int)(CHECKBOX_SIZE * GetDeviceScaleFactor()));
            Graphics g = Graphics.FromImage(checkMark);

            // clear everything, transparent
            g.Clear(Color.Transparent);

            CheckmarkLine = [new((int)(3 * GetDeviceScaleFactor()), (int)(8 * GetDeviceScaleFactor())), new((int)(7 * GetDeviceScaleFactor()), (int)(12 * GetDeviceScaleFactor())), new((int)(14 * GetDeviceScaleFactor()), (int)(5 * GetDeviceScaleFactor()))];

            // draw the checkmark lines
            using (Pen pen = new(Parent.BackColor, 2))
            {
                g.DrawLines(pen, CheckmarkLine);
            }

            return checkMark;
        }

        private bool IsMouseInCheckArea()
        {
            return ClientRectangle.Contains(MouseLocation);
        }
        #endregion
    }

    #endregion
}