#region Imports

using ReaLTaiizor.Enum.Material;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static ReaLTaiizor.Util.MaterialAnimations;

#endregion

namespace ReaLTaiizor.Controls
{
    #region MaterialDialog

    public class MaterialDialog : MaterialForm
    {
        private const int LEFT_RIGHT_PADDING = 24;
        private const int BUTTON_PADDING = 8;
        private const int BUTTON_HEIGHT = 36;
        private const int TEXT_TOP_PADDING = 17;
        private const int TEXT_BOTTOM_PADDING = 28;
        private const int MSG_BOX_WIDTH = 560;
        private int FONT_HEIGHT = 19; // Need to be changed with Font..
        private int _header_Height = 40;

        private MaterialButton _validationButton = new();
        private MaterialButton _cancelButton = new();
        private AnimationManager _AnimationManager;
        private bool CloseAnimation = false;
        private Form _formOverlay;
        private string _text;
        private string _title;

        //public ObservableCollection<MaterialButton> Buttons { get; set; }


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

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText, bool ShowCancelButton, string CancelButtonText, bool UseAccentColor)
        {
            _formOverlay = new Form
            {
                BackColor = Color.Black,
                Opacity = 0.5,
                MinimizeBox = false,
                MaximizeBox = false,
                Text = "",
                ShowIcon = false,
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.None,
                Size = new Size(ParentForm.Width, ParentForm.Height),
                ShowInTaskbar = false,
                Owner = ParentForm,
                Visible = true,
                Location = new Point(ParentForm.Location.X, ParentForm.Location.Y),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            };

            _title = Title;
            if (Title.Length == 0)
            {
                _header_Height = 0;
            }
            else
            {
                _header_Height = 40;
            }

            _text = Text;
            ShowInTaskbar = false;
            Sizable = false;

            BackColor = SkinManager.BackgroundColor;
            FormStyle = FormStyles.StatusAndActionBar_None;

            _AnimationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.03
            };
            _AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;

            _validationButton = new MaterialButton
            {
                AutoSize = false,
                DialogResult = DialogResult.OK,
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Text = ValidationButtonText
            };
            _cancelButton = new MaterialButton
            {
                AutoSize = false,
                DialogResult = DialogResult.Cancel,
                DrawShadows = false,
                Type = MaterialButton.MaterialButtonType.Text,
                UseAccentColor = UseAccentColor,
                Visible = ShowCancelButton,
                Text = CancelButtonText
            };

            this.AcceptButton = _validationButton;
            this.CancelButton = _cancelButton;

            if (!Controls.Contains(_validationButton))
            {
                Controls.Add(_validationButton);
            }

            if (!Controls.Contains(_cancelButton))
            {
                Controls.Add(_cancelButton);
            }

            Width = (int)(MSG_BOX_WIDTH * ScaleFactor);
            int TextWidth = TextRenderer.MeasureText(_text, SkinManager.GetFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor)).Width;
            int RectWidth = Width - (2 * (int)(LEFT_RIGHT_PADDING * ScaleFactor)) - (int)(BUTTON_PADDING * ScaleFactor);
            int RectHeight = ((TextWidth / RectWidth) + 1) * (int)(FONT_HEIGHT * ScaleFactor);
            Rectangle textRect = new(
                (int)(LEFT_RIGHT_PADDING * ScaleFactor),
                (int)(_header_Height * ScaleFactor) + (int)(TEXT_TOP_PADDING * ScaleFactor),
                RectWidth,
                RectHeight + 9);

            Height = (int)(_header_Height * ScaleFactor) + (int)(TEXT_TOP_PADDING * ScaleFactor) + textRect.Height + (int)(TEXT_BOTTOM_PADDING * ScaleFactor) + 52; //560;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            int _buttonWidth = TextRenderer.MeasureText(ValidationButtonText, SkinManager.GetFontByType(MaterialSkinManager.FontType.Button, ScaleFactor)).Width + 32;
            Rectangle _validationbuttonBounds = new(Width - (int)(BUTTON_PADDING * ScaleFactor) - _buttonWidth, Height - (int)(BUTTON_PADDING * ScaleFactor) - (int)(BUTTON_HEIGHT * ScaleFactor), _buttonWidth, (int)(BUTTON_HEIGHT * ScaleFactor));
            _validationButton.Width = _validationbuttonBounds.Width;
            _validationButton.Height = _validationbuttonBounds.Height;
            _validationButton.Top = _validationbuttonBounds.Top;
            _validationButton.Left = _validationbuttonBounds.Left;  //Button minimum width management
            _validationButton.Visible = true;

            _buttonWidth = TextRenderer.MeasureText(CancelButtonText, SkinManager.GetFontByType(MaterialSkinManager.FontType.Button, ScaleFactor)).Width + 32;
            Rectangle _cancelbuttonBounds = new(_validationbuttonBounds.Left - (int)(BUTTON_PADDING * ScaleFactor) - _buttonWidth, Height - (int)(BUTTON_PADDING * ScaleFactor) - (int)(BUTTON_HEIGHT * ScaleFactor), _buttonWidth, (int)(BUTTON_HEIGHT * ScaleFactor));
            _cancelButton.Width = _cancelbuttonBounds.Width;
            _cancelButton.Height = _cancelbuttonBounds.Height;
            _cancelButton.Top = _cancelbuttonBounds.Top;
            _cancelButton.Left = _cancelbuttonBounds.Left;  //Button minimum width management

            //this.ShowDialog();
            //this Dispose();
            //return materialDialogResult;
        }

        public MaterialDialog(Form ParentForm) : this(ParentForm, "Title", "Dialog box", "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Text) : this(ParentForm, "Title", Text, "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text) : this(ParentForm, Title, Text, "OK", false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText) : this(ParentForm, Title, Text, ValidationButtonText, false, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, bool ShowCancelButton) : this(ParentForm, Title, Text, "OK", ShowCancelButton, "Cancel", false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, bool ShowCancelButton, string CancelButtonText) : this(ParentForm, Title, Text, "OK", ShowCancelButton, CancelButtonText, false)
        {
        }

        public MaterialDialog(Form ParentForm, string Title, string Text, string ValidationButtonText, bool ShowCancelButton, string CancelButtonText) : this(ParentForm, Title, Text, ValidationButtonText, ShowCancelButton, CancelButtonText, false)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            base.OnLoad(e);

            Location = new Point(Convert.ToInt32(Owner.Location.X + (Owner.Width / 2) - (Width / 2)), Convert.ToInt32(Owner.Location.Y + (Owner.Height / 2) - (Height / 2)));
            _AnimationManager.StartNewAnimation(AnimationDirection.In);
        }

        void _AnimationManager_OnAnimationProgress(object sender)
        {
            if (CloseAnimation)
            {
                Opacity = _AnimationManager.GetProgress();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            e.Graphics.Clear(BackColor);

            // Calc title Rect
            Rectangle titleRect = new(
                (int)(LEFT_RIGHT_PADDING * ScaleFactor),
                0,
                Width - (2 * (int)(LEFT_RIGHT_PADDING * ScaleFactor)),
                (int)(_header_Height * ScaleFactor));

            //Draw title
            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    _title,
                    SkinManager.GetLogFontByType(MaterialSkinManager.FontType.H6, ScaleFactor),
                    SkinManager.TextHighEmphasisColor,
                    titleRect.Location,
                    titleRect.Size,
                    MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Bottom);
            }

            // Calc text Rect

            int TextWidth = TextRenderer.MeasureText(_text, SkinManager.GetFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor)).Width;
            int RectWidth = Width - (2 * (int)(LEFT_RIGHT_PADDING * ScaleFactor)) - (int)(BUTTON_PADDING * ScaleFactor);
            int RectHeight = ((TextWidth / RectWidth) + 1) * (int)(FONT_HEIGHT * ScaleFactor);

            Rectangle textRect = new(
                (int)(LEFT_RIGHT_PADDING * ScaleFactor),
                (int)(_header_Height * ScaleFactor) + (int)(TEXT_TOP_PADDING * ScaleFactor),
                RectWidth,
                RectHeight + (int)(FONT_HEIGHT * ScaleFactor));

            //Draw  Text
            using (MaterialNativeTextRenderer NativeText = new(g))
            {
                // Draw header text
                NativeText.DrawMultilineTransparentText(
                    _text,
                    SkinManager.GetLogFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor),
                    SkinManager.TextHighEmphasisColor,
                    textRect.Location,
                    textRect.Size,
                    MaterialNativeTextRenderer.TextAlignFlags.Left | MaterialNativeTextRenderer.TextAlignFlags.Middle);
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _formOverlay.Visible = false;
            _formOverlay.Close();
            _formOverlay.Dispose();

            DialogResult res = this.DialogResult;

            base.OnFormClosing(e);
        }

        void _AnimationManager_OnAnimationFinished(object sender)
        {
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void InitializeComponent()
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);

            this.SuspendLayout();
            this.ClientSize = new Size((int)(MSG_BOX_WIDTH * ScaleFactor), 182);
            this.Name = "Dialog";
            this.ResumeLayout(false);

        }

        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        return;
                    }

                    break;
            }

            base.WndProc(ref message);
        }

    }

    #endregion
}