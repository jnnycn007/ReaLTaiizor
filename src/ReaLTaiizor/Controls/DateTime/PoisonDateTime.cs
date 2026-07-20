#region Imports

using ReaLTaiizor.Drawing.Poison;
using ReaLTaiizor.Enum.Poison;
using ReaLTaiizor.Extension.Poison;
using ReaLTaiizor.Interface.Poison;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace ReaLTaiizor.Controls
{
    #region PoisonDateTime

    [ToolboxBitmap(typeof(DateTimePicker))]
    public class PoisonDateTime : DateTimePicker, IPoisonControl
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
        public ColorStyle Style
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
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool UseCustomForeColor { get; set; } = false;
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool UseStyleColors { get; set; } = false;

        [Browsable(false)]
        [Category(PoisonDefaults.PropertyCategory.Behaviour)]
        [DefaultValue(true)]
        public bool UseSelectable
        {
            get => GetStyle(ControlStyles.Selectable);
            set => SetStyle(ControlStyles.Selectable, value);
        }

        #endregion

        #region Fields

        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool DisplayFocus { get; set; } = false;

        [field: DefaultValue(PoisonDateTimeSize.Medium)]
        [field: Category(PoisonDefaults.PropertyCategory.Appearance)]
        [DefaultValue(false)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public bool UseCustomFont
        {
            get;
            set
            {
                field = value;
                Refresh();
            }
        } = false;

        public PoisonDateTimeSize FontSize { get; set; } = PoisonDateTimeSize.Medium;
        [DefaultValue(PoisonDateTimeWeight.Regular)]
        [Category(PoisonDefaults.PropertyCategory.Appearance)]
        public PoisonDateTimeWeight FontWeight { get; set; } = PoisonDateTimeWeight.Regular;

        [DefaultValue(false)]
        [Browsable(false)]
        public new bool ShowUpDown
        {
            get => base.ShowUpDown;
            set => base.ShowUpDown = false;
        }

        private bool isHovered = false;
        private bool isPressed = false;
        private bool isFocused = false;
        private int selectedFieldIndex = 0;

        #endregion

        #region Selection Helpers

        private List<(int Start, int Length)> GetDateTextParts()
        {
            string text = Text;
            List<(int Start, int Length)> parts = new();
            int i = 0;

            while (i < text.Length)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    int start = i;
                    while (i < text.Length && char.IsLetterOrDigit(text[i]))
                    {
                        i++;
                    }

                    parts.Add((start, i - start));
                }
                else
                {
                    i++;
                }
            }

            return parts;
        }

        private RectangleF GetTextPartBounds(Graphics g, int start, int length, RectangleF layoutRect)
        {
            string text = Text;

            if (string.IsNullOrEmpty(text) || start < 0 || start + length > text.Length)
            {
                return RectangleF.Empty;
            }

            TextFormatFlags drawFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            TextFormatFlags measureFlags = drawFlags | TextFormatFlags.NoPadding;
            Size proposedSize = new(int.MaxValue, (int)layoutRect.Height);

            int withPad = TextRenderer.MeasureText(g, text, Font, proposedSize, drawFlags).Width;
            int noPad = TextRenderer.MeasureText(g, text, Font, proposedSize, measureFlags).Width;
            int leftPad = (withPad - noPad) / 2;

            int xBefore = start > 0
                ? TextRenderer.MeasureText(g, text.Substring(0, start), Font, proposedSize, measureFlags).Width
                : 0;
            int xThrough = TextRenderer.MeasureText(g, text.Substring(0, start + length), Font, proposedSize, measureFlags).Width;

            return new RectangleF(
                layoutRect.X + leftPad + xBefore,
                layoutRect.Y,
                xThrough - xBefore,
                layoutRect.Height
            );
        }

        #endregion

        #region Routing Fields

        public override Font Font
        {
            get
            {
                if (UseCustomFont)
                {
                    return base.Font;
                }
                else
                {
                    return PoisonFonts.DateTime(FontSize, FontWeight);
                }
            }
            set
            {
                base.Font = value;
                Refresh();
            }
        }

        #endregion

        #region Constructor
        public PoisonDateTime()
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
                    backColor = PoisonPaint.BackColor.Form(Theme);
                }

                if (backColor.A == 255 && BackgroundImage == null)
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
            MinimumSize = new(0, GetPreferredSize(Size.Empty).Height);

            Color borderColor, foreColor;

            if (isHovered && !isPressed && Enabled)
            {
                foreColor = PoisonPaint.ForeColor.ComboBox.Hover(Theme);
                borderColor = PoisonPaint.GetStyleColor(Style);
            }
            else if (isHovered && isPressed && Enabled)
            {
                foreColor = PoisonPaint.ForeColor.ComboBox.Press(Theme);
                borderColor = PoisonPaint.GetStyleColor(Style);
            }
            else if (!Enabled)
            {
                foreColor = PoisonPaint.ForeColor.ComboBox.Disabled(Theme);
                borderColor = PoisonPaint.BorderColor.ComboBox.Disabled(Theme);
            }
            else
            {
                foreColor = PoisonPaint.ForeColor.ComboBox.Normal(Theme);
                borderColor = PoisonPaint.BorderColor.ComboBox.Normal(Theme);
            }

            using (Pen p = new(borderColor))
            {
                Rectangle boxRect = new(0, 0, Width - 1, Height - 1);
                e.Graphics.DrawRectangle(p, boxRect);
            }

            using (SolidBrush b = new(foreColor))
            {
                e.Graphics.FillPolygon(b, new Point[] { new(Width - 20, (Height / 2) - 2), new(Width - 9, (Height / 2) - 2), new(Width - 15, (Height / 2) + 4) });
                //e.Graphics.FillPolygon(b, new Point[] { new Point(Width - 15, (Height / 2) - 5), new Point(Width - 21, (Height / 2) + 2), new Point(Width - 9, (Height / 2) + 2) });
            }

            int _check = 0;

            if (ShowCheckBox)
            {
                _check = 15;
                using (Pen p = new(borderColor))
                {
                    Rectangle boxRect = new(3, (Height / 2) - 6, 12, 12);
                    e.Graphics.DrawRectangle(p, boxRect);
                }

                if (Checked)
                {

                    Color fillColor = PoisonPaint.GetStyleColor(Style);

                    using SolidBrush b = new(fillColor);
                    Rectangle boxRect = new(5, (Height / 2) - 4, 9, 9);
                    e.Graphics.FillRectangle(b, boxRect);
                }
                else
                {
                    foreColor = PoisonPaint.ForeColor.ComboBox.Disabled(Theme);
                }
            }

            Rectangle textRect = new(2 + _check, 2, Width - 20, Height - 4);


            if (isFocused && Enabled)
            {
                List<(int Start, int Length)> parts = GetDateTextParts();

                if (parts.Count > 0)
                {
                    int idx = Math.Min(selectedFieldIndex, parts.Count - 1);
                    RectangleF layoutRect = new(textRect.X, textRect.Y, textRect.Width, textRect.Height);
                    RectangleF selBounds = GetTextPartBounds(e.Graphics, parts[idx].Start, parts[idx].Length, layoutRect);

                    if (selBounds.Width > 0)
                    {
                        Color highlightColor = PoisonPaint.GetStyleColor(Style);
                        Rectangle highlightRect = new(
                            (int)Math.Floor(selBounds.X) - 1,
                            textRect.Y + 1,
                            (int)Math.Ceiling(selBounds.Width) + 2,
                            textRect.Height - 2
                        );

                        using (SolidBrush hb = new(highlightColor))
                        {
                            e.Graphics.FillRectangle(hb, highlightRect);
                        }

                        if (!string.IsNullOrEmpty(Text)) {
                            string beforeSel = Text.Substring(0, parts[idx].Start);
                            string selected = Text.Substring(parts[idx].Start, parts[idx].Length);
                            string afterSel = Text.Substring(parts[idx].Start + parts[idx].Length);

                            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

                            int getLeftPad(Graphics g, string text) {
                                int withPad = TextRenderer.MeasureText(g, text, Font, Size.Empty, TextFormatFlags.Left | TextFormatFlags.VerticalCenter).Width;
                                int noPad = TextRenderer.MeasureText(g, text, Font, Size.Empty, flags).Width;
                                return (withPad - noPad) / 2;
                            }

                            int x = textRect.X;
                            int y = textRect.Y;

                            //绘制在选中文本前的文本
                            //Draw the text that appears before the selected text
                            if (!string.IsNullOrEmpty(beforeSel)) {
                                x += getLeftPad(e.Graphics, beforeSel);

                                Size size = TextRenderer.MeasureText(e.Graphics, beforeSel, Font, Size.Empty, flags);
                                TextRenderer.DrawText(e.Graphics, beforeSel, Font, 
                                    new Rectangle(x, y, size.Width, textRect.Height), foreColor, flags);

                                x += size.Width;
                            }

                            //绘制选中的文本
                            //Draw the selected text
                            if (!string.IsNullOrEmpty(selected)) {
                                //如果开头是被选中的文本，则在左侧添加内边距以保持文本位置正确
                                //If the selected text is at the beginning, add left padding to keep the text position correct
                                if (string.IsNullOrEmpty(beforeSel))
                                    x += getLeftPad(e.Graphics, selected);

                                Size size = TextRenderer.MeasureText(e.Graphics, selected, Font, Size.Empty, flags);
                                TextRenderer.DrawText(e.Graphics, selected, Font,
                                    new Rectangle(x, y, size.Width, textRect.Height), Color.White, flags);

                                x += size.Width;
                            }

                            //绘制在选中文本后的文本
                            //Draw the text that appears after the selected text
                            if (!string.IsNullOrEmpty(afterSel)) {
                                Size size = TextRenderer.MeasureText(e.Graphics, afterSel, Font, Size.Empty, flags);
                                TextRenderer.DrawText(e.Graphics, afterSel, Font,
                                    new Rectangle(x, y, Math.Min(size.Width, textRect.Right - x), textRect.Height), foreColor, flags);
                            }
                        }
                    }
                    else {
                        TextRenderer.DrawText(e.Graphics, Text, Font, textRect, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    }
                }
                else {
                    TextRenderer.DrawText(e.Graphics, Text, Font, textRect, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
            else {
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            OnCustomPaintForeground(new PoisonPaintEventArgs(Color.Empty, foreColor, e.Graphics));

            if (DisplayFocus && isFocused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
            }
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            base.OnValueChanged(eventargs);
            Invalidate();
        }

        #endregion

        #region Focus Methods

        protected override void OnGotFocus(EventArgs e)
        {
            isFocused = true;
            isHovered = true;
            selectedFieldIndex = 0;
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            isHovered = true;
            selectedFieldIndex = 0;
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Keyboard Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (selectedFieldIndex > 0)
                {
                    selectedFieldIndex--;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                List<(int Start, int Length)> parts = GetDateTextParts();
                if (selectedFieldIndex < parts.Count - 1)
                {
                    selectedFieldIndex++;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                isHovered = true;
                isPressed = true;
                Invalidate();
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            //isHovered = false;
            //isPressed = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        #endregion

        #region Mouse Methods

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;

                int _check = ShowCheckBox ? 15 : 0;

                if (e.X >= 2 + _check && e.X < Width - 20)
                {
                    List<(int Start, int Length)> parts = GetDateTextParts();

                    if (parts.Count > 0)
                    {
                        RectangleF layoutRect = new(2 + _check, 2, Width - 20, Height - 4);

                        using Graphics g = CreateGraphics();
                        int bestIndex = 0;
                        float bestDistance = float.MaxValue;

                        for (int i = 0; i < parts.Count; i++)
                        {
                            RectangleF bounds = GetTextPartBounds(g, parts[i].Start, parts[i].Length, layoutRect);

                            if (e.X >= bounds.Left && e.X <= bounds.Right)
                            {
                                bestIndex = i;
                                bestDistance = 0;
                                break;
                            }

                            float dist = Math.Min(Math.Abs(e.X - bounds.Left), Math.Abs(e.X - bounds.Right));

                            if (dist < bestDistance)
                            {
                                bestDistance = dist;
                                bestIndex = i;
                            }
                        }

                        selectedFieldIndex = bestIndex;
                    }
                }

                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!isFocused)
            {
                isHovered = false;
            }

            Invalidate();

            base.OnMouseLeave(e);
        }

        #endregion

        #region Overridden Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize;
            base.GetPreferredSize(proposedSize);

            using (Graphics g = CreateGraphics())
            {
                string measureText = Text.Length > 0 ? Text : "MeasureText";
                proposedSize = new(int.MaxValue, int.MaxValue);
                preferredSize = TextRenderer.MeasureText(g, measureText, Font, proposedSize, TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.VerticalCenter);
                preferredSize.Height += 10;
            }

            return preferredSize;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        #endregion
    }

    #endregion
}