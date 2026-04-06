#region Imports

using ReaLTaiizor.Enum.Cyber;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static ReaLTaiizor.Util.CyberLibrary;
using Timer = System.Windows.Forms.Timer;

#endregion

namespace ReaLTaiizor.Controls
{
    #region CyberComboBox

    [ToolboxBitmap(typeof(ComboBox))]
    [Description("Allows the user to select an item from a drop-down list.")]
    public class CyberComboBox : ComboBox
    {
        #region Variables

        private float h = 0;

        #endregion

        #region Native Methods

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #endregion

        #region Property Region

        [Category("Cyber")]
        [Description("RGB On/Off")]
        public bool RGB
        {
            get;
            set
            {
                field = value;

                if (field == true)
                {
                    timer_rgb.Stop();
                    if (!DrawEngine.GlobalRGB.Enabled)
                    {
                        timer_rgb.Tick += (Sender, EventArgs) =>
                        {
                            h += 4;
                            if (h >= 360)
                            {
                                h = 0;
                            }

                            Invalidate();
                        };
                        timer_rgb.Start();
                    }
                }
                else
                {
                    timer_rgb.Stop();
                    Invalidate();
                }
            }
        }

        [Category("Cyber")]
        [Description("On/Off Rounded")]
        [DefaultValue(true)]
        public bool Rounding
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Percentage rounding")]
        [DefaultValue(30)]
        public int RoundingInt
        {
            get;
            set
            {
                if (value is >= 0 and <= 100)
                {
                    field = value;
                    Invalidate();
                }
            }
        }

        [Category("BorderStyle")]
        [Description("On/Off Border")]
        [DefaultValue(true)]
        public bool BackgroundPen
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("BorderStyle")]
        [Description("Border size")]
        [DefaultValue(2F)]
        public float Background_WidthPen
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("BorderStyle")]
        [Description("Border color")]
        public Color ColorBackground_Pen
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Background color")]
        public Color ColorBackground
        {
            get;
            set
            {
                field = value;
                BackColor = field;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Arrow color")]
        public Color ColorArrow
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Dropdown item hover color")]
        public Color ColorItemHover
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        private readonly Timer timer_rgb = new() { Interval = 300 };
        [Category("Timers")]
        [Description("RGB mode refresh rate (redrawing in effect)")]
        public int Timer_RGB
        {
            get => timer_rgb.Interval;
            set => timer_rgb.Interval = value;
        }

        [Category("LinearGradient")]
        [Description("On/Off border gradient")]
        [DefaultValue(false)]
        public bool LinearGradientPen
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("LinearGradient")]
        [Description("Color #1 for border gradient")]
        public Color ColorPen_1
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("LinearGradient")]
        [Description("Color #2 for border gradient")]
        public Color ColorPen_2
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Mode <graphics.SmoothingMode>")]
        [DefaultValue(SmoothingMode.HighQuality)]
        public SmoothingMode SmoothingMode
        {
            get;
            set
            {
                if (value != SmoothingMode.Invalid)
                {
                    field = value;
                }

                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("Mode <graphics.TextRenderingHint>")]
        [DefaultValue(TextRenderingHint.ClearTypeGridFit)]
        public TextRenderingHint TextRenderingHint
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

        [Category("Cyber")]
        [Description("ComboBox style")]
        public StateStyle CyberComboBoxStyle
        {
            get;
            set
            {
                field = value;
                switch (field)
                {
                    case StateStyle.Default:
                        BackColor = Color.FromArgb(37, 52, 68);
                        ForeColor = Color.FromArgb(245, 245, 245);

                        RGB = false;
                        Rounding = true;
                        RoundingInt = 30;
                        BackgroundPen = true;
                        Background_WidthPen = 2F;
                        ColorBackground_Pen = Color.FromArgb(29, 200, 238);
                        ColorBackground = Color.FromArgb(37, 52, 68);
                        ColorArrow = Color.FromArgb(29, 200, 238);
                        ColorItemHover = Color.FromArgb(50, 70, 90);
                        Timer_RGB = 300;
                        LinearGradientPen = false;
                        ColorPen_1 = Color.FromArgb(29, 200, 238);
                        ColorPen_2 = Color.FromArgb(37, 52, 68);
                        SmoothingMode = SmoothingMode.HighQuality;
                        TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        Font = HelpEngine.GetDefaultFont();
                        break;
                    case StateStyle.Custom:
                        break;
                    case StateStyle.Random:
                        HelpEngine.GetRandom random = new();
                        ColorBackground = random.ColorArgb();
                        Rounding = random.Bool();

                        if (Rounding)
                        {
                            RoundingInt = random.Int(5, 90);
                        }

                        BackgroundPen = random.Bool();

                        if (BackgroundPen)
                        {
                            Background_WidthPen = random.Float(1, 3);
                            ColorBackground_Pen = random.ColorArgb(random.Int(0, 255));
                        }

                        LinearGradientPen = random.Bool();
                        if (LinearGradientPen)
                        {
                            ColorPen_1 = random.ColorArgb();
                            ColorPen_2 = random.ColorArgb();
                        }
                        break;
                }
                Invalidate();
            }
        } = StateStyle.Default;

        #endregion

        #region Constructor Region

        public CyberComboBox()
        {
            DrawItem += CyberComboBox_DrawItem;
            FlatStyle = FlatStyle.Flat;
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;

            Cursor = Cursors.Hand;
            ItemHeight = 28;

            CyberComboBoxStyle = StateStyle.Default;
            CyberComboBoxStyle = StateStyle.Custom;
        }

        #endregion

        #region Event Region

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg is 0x000F or 0x0133)
            {
                IntPtr hDC = GetWindowDC(m.HWnd);
                if (hDC == IntPtr.Zero)
                {
                    return;
                }

                using Graphics g = Graphics.FromHdc(hDC);

                g.SmoothingMode = SmoothingMode;
                g.TextRenderingHint = TextRenderingHint;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Background
                g.Clear(ColorBackground);

                Rectangle borderRect = new(1, 1, Width - 3, Height - 3);

                // Rounded background
                float roundingValue = Rounding && RoundingInt > 0
                    ? Height / 100F * RoundingInt
                    : 0.1F;

                using GraphicsPath bgPath = DrawEngine.RoundedRectangle(borderRect, roundingValue);
                using (SolidBrush bgBrush = new(ColorBackground))
                {
                    g.FillPath(bgBrush, bgPath);
                }

                // Border
                if (BackgroundPen && Background_WidthPen > 0)
                {
                    Pen borderPen;
                    if (LinearGradientPen)
                    {
                        borderPen = new Pen(new LinearGradientBrush(borderRect, ColorPen_1, ColorPen_2, 360), Background_WidthPen);
                    }
                    else
                    {
                        borderPen = new Pen(RGB ? DrawEngine.HSV_To_RGB(h, 1f, 1f) : ColorBackground_Pen, Background_WidthPen);
                    }

                    borderPen.LineJoin = LineJoin.Round;
                    g.DrawPath(borderPen, bgPath);
                    borderPen.Dispose();
                }

                // Selected text
                string text = SelectedItem != null ? GetItemText(SelectedItem) : string.Empty;
                if (!string.IsNullOrEmpty(text))
                {
                    using SolidBrush textBrush = new(ForeColor);
                    Rectangle textRect = new(8, 0, Width - 36, Height);
                    using StringFormat sf = new() { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(text, Font, textBrush, textRect, sf);
                }

                // Arrow
                Color arrowColor = RGB ? DrawEngine.HSV_To_RGB(h, 1f, 1f) : ColorArrow;
                int arrowX = Width - 18;
                int arrowY = Height / 2;
                Point[] arrowPoints =
                [
                    new(arrowX - 5, arrowY - 3),
                    new(arrowX + 5, arrowY - 3),
                    new(arrowX, arrowY + 4)
                ];
                using (SolidBrush arrowBrush = new(arrowColor))
                {
                    g.FillPolygon(arrowBrush, arrowPoints);
                }

                _ = ReleaseDC(m.HWnd, hDC);
            }
        }

        private void CyberComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode;
            e.Graphics.TextRenderingHint = TextRenderingHint;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using SolidBrush bgBrush = new(isSelected ? ColorItemHover : ColorBackground);
            e.Graphics.FillRectangle(bgBrush, e.Bounds);

            using SolidBrush textBrush = new(ForeColor);
            using StringFormat sf = new() { LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(
                GetItemText(Items[e.Index]),
                Font,
                textBrush,
                new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height),
                sf);
        }

        #endregion
    }

    #endregion
}
