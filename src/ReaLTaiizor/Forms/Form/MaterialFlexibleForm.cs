#region Imports

using ReaLTaiizor.Controls;
using ReaLTaiizor.Manager;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static ReaLTaiizor.Helper.MaterialDrawHelper;

#endregion

namespace ReaLTaiizor.Forms
{
    #region MaterialFlexibleForm

    public class MaterialFlexibleForm : MaterialForm, MaterialControlI
    {
        private readonly MaterialSkinManager materialManager;

        public static Font FONT;

        public static double MAX_WIDTH_FACTOR = 0.7;

        public static double MAX_HEIGHT_FACTOR = 0.9;

        private const int STATU_BAR_PADDING = 24;
        private const int ACTION_BAR_PADDING = 40;
        private const int CONTENT_VERTICAL_PADDING = 20;
        private const int BUTTON_BAR_PADDING = 10;

        private MaterialMultiLineTextBoxEdit richTextBoxMessage;
        private MaterialLabel materialLabel1;
        private MaterialButton leftButton;
        private MaterialButton middleButton;
        private MaterialButton rightButton;

        [Browsable(false)]
        public enum ButtonsPosition
        {
            Fill,
            Left,
            Right,
            Center
        }

        public ButtonsPosition ButtonsPositionEnum { get; set; } = ButtonsPosition.Right;

        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.MaterialFlexibleFormBindingSource = new BindingSource(this.components);
            this.messageContainer = new System.Windows.Forms.Panel();
            this.materialLabel1 = new MaterialLabel();
            this.pictureBoxForIcon = new PictureBox();
            this.richTextBoxMessage = new MaterialMultiLineTextBoxEdit();
            this.leftButton = new MaterialButton();
            this.middleButton = new MaterialButton();
            this.rightButton = new MaterialButton();
            ((ISupportInitialize)this.MaterialFlexibleFormBindingSource).BeginInit();
            this.messageContainer.SuspendLayout();
            ((ISupportInitialize)this.pictureBoxForIcon).BeginInit();
            this.SuspendLayout();
            // 
            // messageContainer
            // 
            this.messageContainer.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom
            | AnchorStyles.Left
            | AnchorStyles.Right);
            this.messageContainer.BackColor = Color.White;
            this.messageContainer.Controls.Add(this.materialLabel1);
            this.messageContainer.Controls.Add(this.pictureBoxForIcon);
            this.messageContainer.Controls.Add(this.richTextBoxMessage);
            this.messageContainer.Location = new Point(1, (int)((STATU_BAR_PADDING + ACTION_BAR_PADDING + 10) * ScaleFactor));
            this.messageContainer.Name = "messageContainer";
            this.messageContainer.Size = new System.Drawing.Size(445, 135);
            this.messageContainer.TabIndex = 1;
            // 
            // materialLabel1
            // 
            this.materialLabel1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            this.materialLabel1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MaterialFlexibleFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(56, 12);
            this.materialLabel1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(377, 111);
            this.materialLabel1.TabIndex = 9;
            this.materialLabel1.Text = "<Message>";
            this.materialLabel1.Visible = false;
            // 
            // pictureBoxForIcon
            // 
            this.pictureBoxForIcon.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxForIcon.Location = new System.Drawing.Point((int)(12 * ScaleFactor), 12);
            this.pictureBoxForIcon.Name = "pictureBoxForIcon";
            this.pictureBoxForIcon.Size = new System.Drawing.Size((int)(32 * ScaleFactor), (int)(32 * ScaleFactor));
            this.pictureBoxForIcon.TabIndex = 8;
            this.pictureBoxForIcon.TabStop = false;
            // 
            // richTextBoxMessage
            // 
            this.richTextBoxMessage.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            this.richTextBoxMessage.AnimateReadOnly = false;
            this.richTextBoxMessage.BackColor = System.Drawing.Color.White;
            this.richTextBoxMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.richTextBoxMessage.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.richTextBoxMessage.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.richTextBoxMessage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MaterialFlexibleFormBindingSource, "MessageText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.richTextBoxMessage.Depth = 0;
            this.richTextBoxMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.richTextBoxMessage.HideSelection = true;
            this.richTextBoxMessage.Location = new System.Drawing.Point((int)(56 * ScaleFactor), 12);
            this.richTextBoxMessage.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxMessage.MaxLength = 32767;
            this.richTextBoxMessage.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            this.richTextBoxMessage.Name = "richTextBoxMessage";
            this.richTextBoxMessage.PasswordChar = '\0';
            this.richTextBoxMessage.ReadOnly = true;
            this.richTextBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.richTextBoxMessage.SelectedText = "";
            this.richTextBoxMessage.SelectionLength = 0;
            this.richTextBoxMessage.SelectionStart = 0;
            this.richTextBoxMessage.ShortcutsEnabled = true;
            this.richTextBoxMessage.Size = new System.Drawing.Size(377, 111);
            this.richTextBoxMessage.TabIndex = 0;
            this.richTextBoxMessage.TabStop = false;
            this.richTextBoxMessage.Text = "<Message>";
            this.richTextBoxMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.richTextBoxMessage.UseSystemPasswordChar = false;
            // 
            // leftButton
            // 
            this.leftButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.leftButton.AutoSize = false;
            this.leftButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftButton.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.leftButton.Depth = 0;
            this.leftButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.leftButton.HighEmphasis = false;
            this.leftButton.Icon = null;
            this.leftButton.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            this.leftButton.Location = new System.Drawing.Point(63, 209);
            this.leftButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.leftButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.leftButton.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.leftButton.Name = "leftButton";
            this.leftButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.leftButton.Size = new System.Drawing.Size(108, 36);
            this.leftButton.TabIndex = 14;
            this.leftButton.Text = "OK";
            this.leftButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Text;
            this.leftButton.UseAccentColor = false;
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Visible = false;
            // 
            // middleButton
            // 
            this.middleButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.middleButton.AutoSize = false;
            this.middleButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.middleButton.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.middleButton.Depth = 0;
            this.middleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.middleButton.HighEmphasis = true;
            this.middleButton.Icon = null;
            this.middleButton.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            this.middleButton.Location = new System.Drawing.Point(179, 209);
            this.middleButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.middleButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.middleButton.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.middleButton.Name = "middleButton";
            this.middleButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.middleButton.Size = new System.Drawing.Size(102, 36);
            this.middleButton.TabIndex = 15;
            this.middleButton.Text = "OK";
            this.middleButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Text;
            this.middleButton.UseAccentColor = false;
            this.middleButton.UseVisualStyleBackColor = true;
            this.middleButton.Visible = false;
            // 
            // rightButton
            // 
            this.rightButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.rightButton.AutoSize = false;
            this.rightButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightButton.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.rightButton.Depth = 0;
            this.rightButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rightButton.HighEmphasis = true;
            this.rightButton.Icon = null;
            this.rightButton.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            this.rightButton.Location = new System.Drawing.Point(289, 209);
            this.rightButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.rightButton.MinimumSize = new System.Drawing.Size(0, 24);
            this.rightButton.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            this.rightButton.Name = "rightButton";
            this.rightButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.rightButton.Size = new System.Drawing.Size(106, 36);
            this.rightButton.TabIndex = 13;
            this.rightButton.Text = "OK";
            this.rightButton.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            this.rightButton.UseAccentColor = false;
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Visible = false;
            // 
            // MaterialFlexibleForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(447, 254);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.middleButton);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.messageContainer);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MaterialFlexibleFormBindingSource, "CaptionText", true));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(276, 140);
            this.Name = "MaterialFlexibleForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Caption>";
            this.Load += new System.EventHandler(this.MaterialFlexibleForm_Load);
            this.Shown += new System.EventHandler(this.MaterialFlexibleForm_Shown);
            ((ISupportInitialize)this.MaterialFlexibleFormBindingSource).EndInit();
            this.messageContainer.ResumeLayout(false);
            ((ISupportInitialize)this.pictureBoxForIcon).EndInit();
            this.ResumeLayout(false);

        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            ScaleFactor = SkinManager.GetDeviceScaleFactor(this);
            ScaleFactorSqrt = SkinManager.GetDeviceScaleFactorSqrt(this);
            base.OnDpiChanged(e);
        }

        private System.Windows.Forms.BindingSource MaterialFlexibleFormBindingSource;

        private System.Windows.Forms.Panel messageContainer;

        private System.Windows.Forms.PictureBox pictureBoxForIcon;

        //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
        private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";

        private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

        //These are the possible buttons (in a standard MessageBox)
        private enum ButtonID
        {
            /// <summary>
            /// Defines the OK
            /// </summary>
            OK = 0,

            /// <summary>
            /// Defines the CANCEL
            /// </summary>
            CANCEL,

            /// <summary>
            /// Defines the YES
            /// </summary>
            YES,

            /// <summary>
            /// Defines the NO
            /// </summary>
            NO,

            /// <summary>
            /// Defines the ABORT
            /// </summary>
            ABORT,

            /// <summary>
            /// Defines the RETRY
            /// </summary>
            RETRY,

            /// <summary>
            /// Defines the IGNORE
            /// </summary>
            IGNORE
        };

        //These are the buttons texts for different languages.
        //If you want to add a new language, add it here and in the GetButtonText-Function
        private enum TwoLetterISOLanguageID
        {
            /// <summary>
            /// Defines the en
            /// </summary>
            en,

            /// <summary>
            /// Defines the de
            /// </summary>
            de,

            /// <summary>
            /// Defines the es
            /// </summary>
            es,

            /// <summary>
            /// Defines the it
            /// </summary>
            it,

            /// <summary>
            /// Defines the fr
            /// </summary>
            fr,

            /// <summary>
            /// Defines the ro
            /// </summary>
            ro,

            /// <summary>
            /// Defines the pl
            /// </summary>
            pl,

            /// <summary>
            /// Defines the tr
            /// </summary>
            tr,

            /// <summary>
            /// Defines the zh
            /// </summary>
            zh
        };

        private static readonly string[] BUTTON_TEXTS_ENGLISH_EN = { "OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore" }; //Note: This is also the fallback language

        private static readonly string[] BUTTON_TEXTS_GERMAN_DE = { "OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren" };

        private static readonly string[] BUTTON_TEXTS_SPANISH_ES = { "Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar" };

        private static readonly string[] BUTTON_TEXTS_ITALIAN_IT = { "OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora" };

        private static readonly string[] BUTTON_TEXTS_FRENCH_FR = { "OK", "Annuler", "&Oui", "&Non", "&Interrompre", "&Recommencer", "&Ignorer" };

        private static readonly string[] BUTTON_TEXTS_ROMANIAN_RO = { "Acceptă", "Anulează", "&Da", "&Nu", "&Întrerupe", "&Reîncearcă", "&Ignoră" };

        private static readonly string[] BUTTON_TEXTS_POLISH_PL = { "OK", "Anuluj", "Tak", "Nie", "Opuść", "Powtórz", "Ignoruj" };

        private static readonly string[] BUTTON_TEXTS_TURKISH_TR = { "Tamam", "İptal", "&Evet", "&Hayır", "&Sonlandır", "&Yeniden Dene", "&Yoksay" }; //Abort: &Durdur

        private static readonly string[] BUTTON_TEXTS_SIMPLIFIED_CHINESE_ZH = { "确定", "取消", "&是", "&否", "&中止", "&重试", "&忽略" };

        private MessageBoxDefaultButton defaultButton;

        private int visibleButtonsCount;

        private TwoLetterISOLanguageID languageID = TwoLetterISOLanguageID.en;

        private MaterialFlexibleForm()
        {
            InitializeComponent();

            //Try to evaluate the language. If this fails, the fallback language English will be used
            System.Enum.TryParse<TwoLetterISOLanguageID>(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out this.languageID);

            this.KeyPreview = true;
            this.KeyUp += MaterialFlexibleForm_KeyUp;

            materialManager = MaterialSkinManager.Instance;
            materialManager.AddFormToManage(this);
            FONT = materialManager.GetFontByType(MaterialSkinManager.FontType.Body1, ScaleFactor);
            messageContainer.BackColor = this.BackColor;
        }

        private static string[] GetStringRows(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }

            string[] messageRows = message.Split(new char[] { '\n' }, StringSplitOptions.None);
            return messageRows;
        }

        private string GetButtonText(ButtonID buttonID)
        {
            int buttonTextArrayIndex = Convert.ToInt32(buttonID);

            return this.languageID switch
            {
                TwoLetterISOLanguageID.de => BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex],
                TwoLetterISOLanguageID.es => BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex],
                TwoLetterISOLanguageID.it => BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex],
                TwoLetterISOLanguageID.fr => BUTTON_TEXTS_FRENCH_FR[buttonTextArrayIndex],
                TwoLetterISOLanguageID.ro => BUTTON_TEXTS_ROMANIAN_RO[buttonTextArrayIndex],
                TwoLetterISOLanguageID.pl => BUTTON_TEXTS_POLISH_PL[buttonTextArrayIndex],
                TwoLetterISOLanguageID.tr => BUTTON_TEXTS_TURKISH_TR[buttonTextArrayIndex],
                TwoLetterISOLanguageID.zh => BUTTON_TEXTS_SIMPLIFIED_CHINESE_ZH[buttonTextArrayIndex],
                _ => BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex],
            };
        }

        private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
        {
            const double MIN_FACTOR = 0.2;
            const double MAX_FACTOR = 1.0;

            if (workingAreaFactor < MIN_FACTOR)
            {
                return MIN_FACTOR;
            }

            if (workingAreaFactor > MAX_FACTOR)
            {
                return MAX_FACTOR;
            }

            return workingAreaFactor;
        }

        private static void SetDialogStartPosition(MaterialFlexibleForm MaterialFlexibleForm, IWin32Window owner)
        {
            //If no owner given: Center on current screen
            if (owner == null)
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                MaterialFlexibleForm.StartPosition = FormStartPosition.Manual;
                MaterialFlexibleForm.Left = screen.Bounds.Left + (screen.Bounds.Width / 2) - (MaterialFlexibleForm.Width / 2);
                MaterialFlexibleForm.Top = screen.Bounds.Top + (screen.Bounds.Height / 2) - (MaterialFlexibleForm.Height / 2);
            }
        }

        private static void SetDialogSizes(MaterialFlexibleForm MaterialFlexibleForm, string text, string caption)
        {
            //First set the bounds for the maximum dialog size
            MaterialFlexibleForm.MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * MaterialFlexibleForm.GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
                                                          Convert.ToInt32(SystemInformation.WorkingArea.Height * MaterialFlexibleForm.GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

            //Get rows. Exit if there are no rows to render...
            string[] stringRows = GetStringRows(text);
            if (stringRows == null)
            {
                return;
            }

            //Calculate whole text height
            int textHeight = Math.Min(TextRenderer.MeasureText(text, FONT).Height, 600);

            //Calculate width for longest text line
            const int SCROLLBAR_WIDTH_OFFSET = 15;
            int longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
            int captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
            int textWidth = Math.Max(longestTextRowWidth + (int)(SCROLLBAR_WIDTH_OFFSET * MaterialFlexibleForm.ScaleFactor), captionWidth);

            //Calculate margins
            int marginWidth = MaterialFlexibleForm.Width - MaterialFlexibleForm.richTextBoxMessage.Width;
            int marginHeight = MaterialFlexibleForm.Height - MaterialFlexibleForm.richTextBoxMessage.Height;


            int minimumHeight = MaterialFlexibleForm.messageContainer.Top + MaterialFlexibleForm.pictureBoxForIcon.Height + (int)(2 * 8 * MaterialFlexibleForm.ScaleFactor) + (int)(54 * MaterialFlexibleForm.ScaleFactor);
            if (marginHeight < minimumHeight)
            {
                marginHeight = minimumHeight;
            }

            //Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
            MaterialFlexibleForm.Size = new Size(textWidth + marginWidth,
                                                   textHeight + marginHeight);
            MaterialFlexibleForm.messageContainer.Height = MaterialFlexibleForm.Height - (int)((ACTION_BAR_PADDING + STATU_BAR_PADDING + BUTTON_BAR_PADDING) * MaterialFlexibleForm.ScaleFactor) - MaterialFlexibleForm.rightButton.Height;
        }

        private static void SetDialogIcon(MaterialFlexibleForm MaterialFlexibleForm, MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    MaterialFlexibleForm.pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case MessageBoxIcon.Warning:
                    MaterialFlexibleForm.pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;

                case MessageBoxIcon.Error:
                    MaterialFlexibleForm.pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case MessageBoxIcon.Question:
                    MaterialFlexibleForm.pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                default:
                    //When no icon is used: Correct placement and width of rich text box.
                    MaterialFlexibleForm.pictureBoxForIcon.Visible = false;
                    MaterialFlexibleForm.richTextBoxMessage.Left -= MaterialFlexibleForm.pictureBoxForIcon.Width;
                    MaterialFlexibleForm.richTextBoxMessage.Width += MaterialFlexibleForm.pictureBoxForIcon.Width;
                    break;
            }
        }

        private static void SetDialogButtons(MaterialFlexibleForm MaterialFlexibleForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, ButtonsPosition buttonsPosition)
        {
            //Set the buttons visibilities and texts
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    MaterialFlexibleForm.visibleButtonsCount = 3;

                    MaterialFlexibleForm.leftButton.Visible = true;
                    MaterialFlexibleForm.leftButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.ABORT);
                    MaterialFlexibleForm.leftButton.DialogResult = DialogResult.Abort;

                    MaterialFlexibleForm.middleButton.Visible = true;
                    MaterialFlexibleForm.middleButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.RETRY);
                    MaterialFlexibleForm.middleButton.DialogResult = DialogResult.Retry;

                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.IGNORE);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.Ignore;

                    MaterialFlexibleForm.ControlBox = false;
                    break;

                case MessageBoxButtons.OKCancel:
                    MaterialFlexibleForm.visibleButtonsCount = 2;

                    MaterialFlexibleForm.middleButton.Visible = true;
                    MaterialFlexibleForm.middleButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.CANCEL);
                    MaterialFlexibleForm.middleButton.DialogResult = DialogResult.Cancel;

                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.OK);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.OK;

                    MaterialFlexibleForm.CancelButton = MaterialFlexibleForm.middleButton;
                    break;

                case MessageBoxButtons.RetryCancel:
                    MaterialFlexibleForm.visibleButtonsCount = 2;

                    MaterialFlexibleForm.middleButton.Visible = true;
                    MaterialFlexibleForm.middleButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.CANCEL);
                    MaterialFlexibleForm.middleButton.DialogResult = DialogResult.Cancel;

                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.RETRY);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.Retry;

                    MaterialFlexibleForm.CancelButton = MaterialFlexibleForm.middleButton;
                    break;

                case MessageBoxButtons.YesNo:
                    MaterialFlexibleForm.visibleButtonsCount = 2;

                    MaterialFlexibleForm.middleButton.Visible = true;
                    MaterialFlexibleForm.middleButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.NO);
                    MaterialFlexibleForm.middleButton.DialogResult = DialogResult.No;

                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.YES);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.Yes;

                    //MaterialFlexibleForm.ControlBox = false;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    MaterialFlexibleForm.visibleButtonsCount = 3;

                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.YES);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.Yes;

                    MaterialFlexibleForm.middleButton.Visible = true;
                    MaterialFlexibleForm.middleButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.NO);
                    MaterialFlexibleForm.middleButton.DialogResult = DialogResult.No;

                    MaterialFlexibleForm.leftButton.Visible = true;
                    MaterialFlexibleForm.leftButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.CANCEL);
                    MaterialFlexibleForm.leftButton.DialogResult = DialogResult.Cancel;

                    MaterialFlexibleForm.CancelButton = MaterialFlexibleForm.leftButton;
                    break;

                case MessageBoxButtons.OK:
                default:
                    MaterialFlexibleForm.visibleButtonsCount = 1;
                    MaterialFlexibleForm.rightButton.Visible = true;
                    MaterialFlexibleForm.rightButton.Text = MaterialFlexibleForm.GetButtonText(ButtonID.OK);
                    MaterialFlexibleForm.rightButton.DialogResult = DialogResult.OK;

                    MaterialFlexibleForm.CancelButton = MaterialFlexibleForm.rightButton;
                    break;
            }

            //Set default button (used in MaterialFlexibleForm_Shown)
            MaterialFlexibleForm.defaultButton = defaultButton;

            SetButtonsPosition(MaterialFlexibleForm, buttonsPosition);
        }

        private void MaterialFlexibleForm_Shown(object sender, EventArgs e)
        {
            int buttonIndexToFocus = 1;
            System.Windows.Forms.Button buttonToFocus;

            //Set the default button...
            buttonIndexToFocus = this.defaultButton switch
            {
                MessageBoxDefaultButton.Button2 => 2,
                MessageBoxDefaultButton.Button3 => 3,
                _ => 1,
            };
            if (buttonIndexToFocus > this.visibleButtonsCount)
            {
                buttonIndexToFocus = this.visibleButtonsCount;
            }

            if (buttonIndexToFocus == 3)
            {
                buttonToFocus = this.rightButton;
            }
            else if (buttonIndexToFocus == 2)
            {
                buttonToFocus = this.middleButton;
            }
            else
            {
                buttonToFocus = this.leftButton;
            }

            buttonToFocus.Focus();
        }

        private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Process.Start(e.LinkText);
            }
            catch (Exception)
            {
                //Let the caller of MaterialFlexibleForm decide what to do with this exception...
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        internal void MaterialFlexibleForm_KeyUp(object sender, KeyEventArgs e)
        {
            //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
            {
                string buttonsTextLine = (this.leftButton.Visible ? this.leftButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                    + (this.middleButton.Visible ? this.middleButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                    + (this.rightButton.Visible ? this.rightButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

                //Build same clipboard text like the standard .Net MessageBox
                string textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + this.Text + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + this.richTextBoxMessage.Text + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                     + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                     + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

                //Set text in clipboard
                Clipboard.SetText(textForClipboard);
            }
        }

        public string CaptionText { get; set; }

        public string MessageText { get; set; }


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

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        {
            //Create a new instance of the FlexibleMessageBox form
            MaterialFlexibleForm MaterialFlexibleForm = new()
            {
                ShowInTaskbar = false,
                Sizable = false,

                //Bind the caption and the message text
                CaptionText = caption,
                MessageText = text
            };

            MaterialFlexibleForm.Width = (int)(MaterialFlexibleForm.Width * MaterialFlexibleForm.ScaleFactor);
            MaterialFlexibleForm.Height = (int)(MaterialFlexibleForm.Height * MaterialFlexibleForm.ScaleFactor);
            MaterialFlexibleForm.MaterialFlexibleFormBindingSource.DataSource = MaterialFlexibleForm;


            //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
            SetDialogIcon(MaterialFlexibleForm, icon);

            //Set the font for all controls
            MaterialFlexibleForm.Font = FONT;
            MaterialFlexibleForm.richTextBoxMessage.Font = FONT;
            MaterialFlexibleForm.richTextBoxMessage.Visible = UseRichTextBox;
            MaterialFlexibleForm.materialLabel1.Font = FONT;
            MaterialFlexibleForm.materialLabel1.Visible = !UseRichTextBox;

            //Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
            SetDialogSizes(MaterialFlexibleForm, text, caption);

            //Set the dialogs start position when given. Otherwise center the dialog on the current screen.
            SetDialogStartPosition(MaterialFlexibleForm, owner);


            //Set the buttons visibilities and texts. Also set a default button.
            //Moved after SetDialogSizes() because it needs Dialog.Width property set.
            SetDialogButtons(MaterialFlexibleForm, buttons, defaultButton, buttonsPosition);
            //Show the dialog
            return MaterialFlexibleForm.ShowDialog(owner);
        }

        private void MaterialFlexibleForm_Load(object sender, EventArgs e)
        {
        }

        private static void SetButtonsPosition(MaterialFlexibleForm fMF, ButtonsPosition buttonsPosition)
        {
            const int padding = 10;
            int visibleButtonsWidth = 0;
            switch (buttonsPosition)
            {
                case ButtonsPosition.Center:
                    switch (fMF.visibleButtonsCount)
                    {
                        case 3:
                            fMF.middleButton.Left = (fMF.Width / 2) - (fMF.middleButton.Width / 2);
                            fMF.leftButton.Left = fMF.middleButton.Left - fMF.leftButton.Width - ((int)(padding * fMF.ScaleFactor) * 2);
                            fMF.rightButton.Left = fMF.middleButton.Right + ((int)(padding * fMF.ScaleFactor) * 2);
                            visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 6);
                            break;
                        case 2:
                            fMF.middleButton.Left = (fMF.Width / 2) - fMF.middleButton.Width - (int)(padding * fMF.ScaleFactor);
                            fMF.rightButton.Left = (fMF.Width / 2) + (int)(padding * fMF.ScaleFactor);
                            visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 4);
                            break;
                        case 1:
                            fMF.rightButton.Left = (fMF.Width / 2) - (fMF.rightButton.Width / 2);
                            visibleButtonsWidth = fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 2);
                            break;
                        default:
                            break;
                    }
                    break;
                case ButtonsPosition.Left:
                    switch (fMF.visibleButtonsCount)
                    {
                        case 3:
                            fMF.leftButton.Left = (int)(padding * fMF.ScaleFactor);
                            fMF.middleButton.Left = fMF.leftButton.Right + ((int)(padding * fMF.ScaleFactor) * 2);
                            fMF.rightButton.Left = fMF.middleButton.Right + ((int)(padding * fMF.ScaleFactor) * 2);
                            visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 6);
                            break;
                        case 2:
                            fMF.middleButton.Left = (int)(padding * fMF.ScaleFactor);
                            fMF.rightButton.Left = fMF.middleButton.Right + ((int)(padding * fMF.ScaleFactor) * 2);
                            visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 4);
                            break;
                        case 1:
                            fMF.rightButton.Left = (int)(padding * fMF.ScaleFactor);
                            visibleButtonsWidth = fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 2);
                            break;
                        default:
                            break;
                    }
                    break;
                case ButtonsPosition.Right:
                    // This alignment is simplest, in this alignment doesn't care how many buttons are visible.
                    // Always the buttons visibility order is right, right + middle, right + middle + left
                    fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - (int)(padding * fMF.ScaleFactor);
                    fMF.middleButton.Left = fMF.rightButton.Left - fMF.middleButton.Width - ((int)(padding * fMF.ScaleFactor) * 2);
                    fMF.leftButton.Left = fMF.middleButton.Left - fMF.leftButton.Width - ((int)(padding * fMF.ScaleFactor) * 2);
                    switch (fMF.visibleButtonsCount)
                    {
                        case 3:
                            visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 6);
                            break;
                        case 2:
                            visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 4);
                            break;
                        case 1:
                            visibleButtonsWidth = fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 2);
                            break;
                        default:
                            break;
                    }
                    break;
                case ButtonsPosition.Fill:
                    switch (fMF.visibleButtonsCount)
                    {
                        case 3:
                            fMF.leftButton.Left = (int)(padding * fMF.ScaleFactor);
                            fMF.middleButton.Left = (fMF.Width / 2) - (fMF.middleButton.Width / 2);
                            fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - ((int)(padding * fMF.ScaleFactor) * 2);
                            visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 6);
                            break;
                        case 2:
                            fMF.middleButton.Left = (int)(padding * fMF.ScaleFactor);
                            fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - ((int)(padding * fMF.ScaleFactor) * 2);
                            visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 4);
                            break;
                        case 1:
                            fMF.rightButton.Left = (fMF.Width / 2) - (fMF.middleButton.Width / 2);
                            visibleButtonsWidth = fMF.rightButton.Width + ((int)(padding * fMF.ScaleFactor) * 2);
                            break;
                        default:
                            break;
                    }
                    break;
            }
            fMF.Width = Math.Max(fMF.Width, visibleButtonsWidth);
        }
    }

    #endregion
}