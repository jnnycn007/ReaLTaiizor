#region Imports

using ReaLTaiizor.Design.Poison;
using ReaLTaiizor.Drawing.Poison;
using ReaLTaiizor.Enum.Poison;
using ReaLTaiizor.Extension.Poison;
using ReaLTaiizor.Interface.Poison;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace ReaLTaiizor.Controls
{
    #region PoisonProgressBar

    [Designer(typeof(PoisonProgressBarDesigner))]
    [ToolboxBitmap(typeof(ProgressBar))]
    public class PoisonProgressBar : ProgressBar, IPoisonControl
    {
        #region Interface

        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public event EventHandler<PoisonPaintEventArgs> CustomPaintBackground;
        protected virtual void OnCustomPaintBackground(PoisonPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintBackground != null)
            {
                CustomPaintBackground(this, e);
            }
        }

        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public event EventHandler<PoisonPaintEventArgs> CustomPaint;
        protected virtual void OnCustomPaint(PoisonPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaint != null)
            {
                CustomPaint(this, e);
            }
        }

        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public event EventHandler<PoisonPaintEventArgs> CustomPaintForeground;
        protected virtual void OnCustomPaintForeground(PoisonPaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint) && CustomPaintForeground != null)
            {
                CustomPaintForeground(this, e);
            }
        }

        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        [DefaultValue(ColorStyle.Default)]
        public new ColorStyle Style
        {
            get
            {
                if (DesignMode || field != ColorStyle.Default)
                {
                    return field;
                }

                if (StyleManager != null && field == ColorStyle.Default)
                {
                    return StyleManager.Style;
                }

                if (StyleManager == null && field == ColorStyle.Default)
                {
                    return PoisonDefaults.Style;
                }

                return field;
            }
            set;
        } = ColorStyle.Default;

        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        [DefaultValue(ThemeStyle.Default)]
        public ThemeStyle Theme
        {
            get
            {
                if (DesignMode || field != ThemeStyle.Default)
                {
                    return field;
                }

                if (StyleManager != null && field == ThemeStyle.Default)
                {
                    return StyleManager.Theme;
                }

                if (StyleManager == null && field == ThemeStyle.Default)
                {
                    return PoisonDefaults.Theme;
                }

                return field;
            }
            set;
        } = ThemeStyle.Default;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PoisonStyleManager StyleManager { get; set; } = null;
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool UseCustomBackColor { get; set; } = false;
        [Browsable(false)]
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseCustomForeColor { get; set; } = false;
        [Browsable(false)]
        [DefaultValue(true)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseStyleColors { get; set; } = true;

        [Browsable(false)]
        [Category(PoisonDefaults.PropertyCategory.Behaviour)]
        [DefaultValue(false)]
        public bool UseSelectable
        {
            get => GetStyle(ControlStyles.Selectable);
            set => SetStyle(ControlStyles.Selectable, value);
        }

        #endregion

        #region Fields

        [DefaultValue(PoisonProgressBarSize.Medium)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public PoisonProgressBarSize FontSize { get; set; } = PoisonProgressBarSize.Medium;
        [DefaultValue(PoisonProgressBarWeight.Light)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public PoisonProgressBarWeight FontWeight { get; set; } = PoisonProgressBarWeight.Light;
        [DefaultValue(ContentAlignment.MiddleRight)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleRight;
        [DefaultValue(true)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool HideProgressText { get; set; } = true;
        [DefaultValue(ProgressBarStyle.Continuous)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public ProgressBarStyle ProgressBarStyle { get; set; } = ProgressBarStyle.Continuous;

        public new int Value
        {
            get => base.Value;
            set { if (value > Maximum) { return; } base.Value = value; Invalidate(); }
        }

        [Browsable(false)]
        public double ProgressTotalPercent => (1 - ((double)(Maximum - Value) / Maximum)) * 100;

        [Browsable(false)]
        public double ProgressTotalValue => 1 - ((double)(Maximum - Value) / Maximum);

        [Browsable(false)]
        public string ProgressPercentText => string.Format("{0}%", Math.Round(ProgressTotalPercent));

        private double ProgressBarWidth => (double)Value / Maximum * ClientRectangle.Width;

        /// <summary>
        /// Set to 0 if you want to apply default width.
        /// </summary>
        [DefaultValue(0)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public int ProgressBarMarqueeWidth
        {
            get => field == 0 ? ClientRectangle.Width / 3 : field;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("ProgressBarMarqueeWidth must be a number more than zero.", (Exception)null);
                field = value;
            }
        } = 0;

        [DefaultValue(100)]
        public int MarqueeFPS 
        { 
            get; 
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MarqueeFPS must be a number more than zero.", (Exception)null);
                field = value;
                if (marqueeTimer != null)
                {
                    marqueeTimer.Interval = 1000 / value;
                }
            }
        } = 100;

        /// <summary>
        /// Same usage as Winforms originally included.
        /// </summary>
        [DefaultValue(100)]
        public new int MarqueeAnimationSpeed 
        { 
            get; 
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MarqueeAnimationSpeed value must be a number more than zero.", (Exception)null);
                field = value;
            }
        } = 100;

        /// <summary>
        /// If enabled, the marquee will change its speed like a material design ProgressBar like.
        /// </summary>
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool EnableMaterialMarqueeStyleSpeed { get; set; } = false;

        /// <summary>
        /// Indicates the ratio material design ProgressBar will speed up.
        /// </summary>
        [DefaultValue(3)]
        public int MaterialStyleMarqueeSpeedRatio
        {
            get;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MaterialStyleSpeedRatio value must be a number more than zero.", (Exception)null);
                field = value;
            }
        } = 3;
        #endregion

        #region Constructor

        public PoisonProgressBar()
        {
            SetStyle
            (
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                    true
            );
        }

        #endregion

        #region Paint Methods

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color backColor = BackColor;

                if (!UseCustomBackColor)
                {
                    if (!Enabled)
                    {
                        backColor = PoisonPaint.BackColor.ProgressBar.Bar.Disabled(Theme);
                    }
                    else
                    {
                        backColor = PoisonPaint.BackColor.ProgressBar.Bar.Normal(Theme);
                    }
                }

                if (backColor.A == 255)
                {
                    e.Graphics.Clear(backColor);
                    return;
                }

                base.OnPaintBackground(e);

                OnCustomPaintBackground(new PoisonPaintEventArgs(backColor, Color.Empty, e.Graphics));
            }
            catch
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }

                OnCustomPaint(new PoisonPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
                OnPaintForeground(e);
            }
            catch
            {
                Invalidate();
            }
        }

        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            if (ProgressBarStyle == ProgressBarStyle.Continuous)
            {
                if (!DesignMode)
                {
                    StopTimer();
                }

                DrawProgressContinuous(e.Graphics);
            }
            else if (ProgressBarStyle == ProgressBarStyle.Blocks)
            {
                if (!DesignMode)
                {
                    StopTimer();
                }

                DrawProgressContinuous(e.Graphics);
            }
            else if (ProgressBarStyle == ProgressBarStyle.Marquee)
            {
                if (!DesignMode && Enabled)
                {
                    StartTimer();
                }

                if (!Enabled)
                {
                    StopTimer();
                }

                if (Value == Maximum)
                {
                    StopTimer();
                    DrawProgressContinuous(e.Graphics);
                }
                else
                {
                    DrawProgressMarquee(e.Graphics);
                }
            }

            DrawProgressText(e.Graphics);

            using (Pen p = new(PoisonPaint.BorderColor.ProgressBar.Normal(Theme)))
            {
                Rectangle borderRect = new(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawRectangle(p, borderRect);
            }

            OnCustomPaintForeground(new PoisonPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
        }

        private void DrawProgressContinuous(Graphics graphics)
        {
            graphics.FillRectangle(PoisonPaint.GetStyleBrush(Style), 0, 0, (int)ProgressBarWidth, ClientRectangle.Height);
        }

        private float marqueeX = 0;

        private void DrawProgressMarquee(Graphics graphics)
        {
            graphics.FillRectangle(PoisonPaint.GetStyleBrush(Style), marqueeX, 0, ProgressBarMarqueeWidth, ClientRectangle.Height);
        }

        private void DrawProgressText(Graphics graphics)
        {
            if (HideProgressText)
            {
                return;
            }

            Color foreColor;

            if (!Enabled)
            {
                foreColor = PoisonPaint.ForeColor.ProgressBar.Disabled(Theme);
            }
            else
            {
                foreColor = PoisonPaint.ForeColor.ProgressBar.Normal(Theme);
            }

            TextRenderer.DrawText(graphics, ProgressPercentText, PoisonFonts.ProgressBar(FontSize, FontWeight), ClientRectangle, foreColor, PoisonPaint.GetTextFormatFlags(TextAlign));
        }

        #endregion

        #region Overridden Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize;
            base.GetPreferredSize(proposedSize);

            using (Graphics g = CreateGraphics())
            {
                proposedSize = new(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, ProgressPercentText, PoisonFonts.ProgressBar(FontSize, FontWeight), proposedSize, PoisonPaint.GetTextFormatFlags(TextAlign));
            }

            return preferredSize;
        }

        #endregion

        #region Private Methods

        private Timer marqueeTimer;
        private bool marqueeTimerEnabled => marqueeTimer != null && marqueeTimer.Enabled;

        private void StartTimer()
        {
            if (marqueeTimerEnabled)
            {
                return;
            }

            if (marqueeTimer == null)
            {
                marqueeTimer = new Timer { Interval = 1000 / MarqueeFPS };
                marqueeTimer.Tick += marqueeTimer_Tick;
            }

            marqueeX = -ProgressBarMarqueeWidth;

            marqueeTimer.Stop();
            marqueeTimer.Start();

            marqueeTimer.Enabled = true;

            Invalidate();
        }

        private void StopTimer()
        {
            if (marqueeTimer == null)
            {
                return;
            }

            marqueeTimer.Stop();

            Invalidate();
        }

        private bool materialEffectFlag = true;
        private void marqueeTimer_Tick(object sender, EventArgs e)
        {

            marqueeX += (ClientRectangle.Width + ProgressBarMarqueeWidth) / (MarqueeAnimationSpeed * MarqueeFPS / 100F); 
            // Here 100F is 1000/10, 'cause we need to support original anime users we keep a *10 ratio here.

            if (marqueeX > ClientRectangle.Width)
            {
                marqueeX = -ProgressBarMarqueeWidth;
                if (EnableMaterialMarqueeStyleSpeed)// Once fast, once slow
                {
                    if (materialEffectFlag)
                    {
                        MarqueeAnimationSpeed *= MaterialStyleMarqueeSpeedRatio;
                    }
                    else
                    {
                        MarqueeAnimationSpeed /= MaterialStyleMarqueeSpeedRatio;
                    }
                    materialEffectFlag = !materialEffectFlag;
                }
            }

            Invalidate();
        }

        #endregion
    }

    #endregion
}