#region Imports

using ReaLTaiizor.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#endregion

namespace ReaLTaiizor.Controls
{
    #region ThunderControlBox

    public class ThunderControlBox : Control
    {
        #region Properties

        public bool DefaultLocation
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        } = true;

        public bool EnableMinimizeButton
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        } = true;

        public bool EnableMaximizeButton
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        } = true;

        public bool EnableCloseButton
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        } = true;

        #endregion

        private MouseStateThunder State = MouseStateThunder.None;
        private Rectangle MinBtn = new(0, 0, 20, 20);
        private Rectangle MaxBtn = new(25, 0, 20, 20);
        private Rectangle ClsBtn = new(50, 0, 20, 20);
        private int x = 0;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            try
            {
                if (DefaultLocation)
                {
                    Location = new(Parent.Width - Width - 3, 3);
                }
            }
            catch (Exception)
            {
                //
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            x = e.Location.X;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Location.X is > 0 and < 20 && EnableMinimizeButton)
            {
                FindForm().WindowState = FormWindowState.Minimized;
            }
            else if (e.Location.X is > 25 and < 45 && EnableMaximizeButton)
            {
                if (FindForm().WindowState == FormWindowState.Normal)
                {
                    FindForm().WindowState = FormWindowState.Maximized;
                }
                else
                {
                    FindForm().WindowState = FormWindowState.Normal;
                }
            }
            else if (e.Location.X is > 50 and < 70 && EnableCloseButton)
            {
                FindForm().Close();
            }

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = MouseStateThunder.Over;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = MouseStateThunder.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = MouseStateThunder.None;
            Invalidate();
        }

        public ThunderControlBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(174, 195, 30);
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new(Width, Height);
            Graphics G = Graphics.FromImage(B);
            base.OnPaint(e);
            G.Clear(BackColor);
            G.SmoothingMode = SmoothingMode.HighQuality;
            Font mf = new("Marlett", 9);
            SolidBrush mfb = new(ForeColor);
            SolidBrush mfbDisabled = new(Color.FromArgb(80, ForeColor));
            SolidBrush minBrush = EnableMinimizeButton ? mfb : mfbDisabled;
            SolidBrush maxBrush = EnableMaximizeButton ? mfb : mfbDisabled;
            SolidBrush clsBrush = EnableCloseButton ? mfb : mfbDisabled;
            Pen P1 = new(Color.FromArgb(21, 21, 21), 1);
            Color C1 = Color.FromArgb(66, 67, 70);
            Color C2 = Color.FromArgb(43, 44, 48);
            GraphicsPath GP1 = DrawThunder.RoundRect(MinBtn, 4);
            GraphicsPath GP2 = DrawThunder.RoundRect(MaxBtn, 4);
            GraphicsPath GP3 = DrawThunder.RoundRect(ClsBtn, 4);

            LinearGradientBrush mlgb;
            switch (State)
            {
                case MouseStateThunder.None:
                    mlgb = new(MinBtn, C1, C2, 90);
                    G.FillPath(mlgb, GP1);
                    G.DrawPath(P1, GP1);
                    G.DrawString("0", mf, minBrush, 4, 4);

                    if (FindForm().WindowState == FormWindowState.Normal)
                    {
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("1", mf, maxBrush, 28, 4);
                    }
                    else
                    {
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("2", mf, maxBrush, 28, 4);
                    }

                    G.FillPath(mlgb, GP3);
                    G.DrawPath(P1, GP3);
                    G.DrawString("r", mf, clsBrush, 52, 4);
                    break;
                case MouseStateThunder.Over:
                    if (x is > 0 and < 20)
                    {
                        if (EnableMinimizeButton)
                        {
                            mlgb = new(MinBtn, Color.FromArgb(100, C1), Color.FromArgb(100, C2), 90);
                        }
                        else
                        {
                            mlgb = new(MinBtn, C1, C2, 90);
                        }
                        G.FillPath(mlgb, GP1);
                        G.DrawPath(P1, GP1);
                        G.DrawString("0", mf, minBrush, 4, 4);

                        if (FindForm().WindowState == FormWindowState.Normal)
                        {
                            mlgb = new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, DrawThunder.RoundRect(MaxBtn, 4));
                            G.DrawPath(P1, GP2);
                            G.DrawString("1", mf, maxBrush, 28, 4);
                        }
                        else
                        {
                            mlgb = new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, DrawThunder.RoundRect(MaxBtn, 4));
                            G.DrawPath(P1, GP2);
                            G.DrawString("2", mf, maxBrush, 28, 4);
                        }

                        mlgb = new(ClsBtn, C1, C2, 90);
                        G.FillPath(mlgb, DrawThunder.RoundRect(ClsBtn, 4));
                        G.DrawPath(P1, GP3);
                        G.DrawString("r", mf, clsBrush, 52, 4);

                        Cursor = EnableMinimizeButton ? Cursors.Hand : Cursors.Default;
                    }
                    else if (x is > 25 and < 45)
                    {
                        mlgb = new(MinBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP1);
                        G.DrawPath(P1, GP1);
                        G.DrawString("0", mf, minBrush, 4, 4);

                        if (FindForm().WindowState == FormWindowState.Normal)
                        {
                            mlgb = EnableMaximizeButton
                                ? new(MaxBtn, Color.FromArgb(100, C1), Color.FromArgb(100, C2), 90)
                                : new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("1", mf, maxBrush, 28, 4);
                        }
                        else
                        {
                            mlgb = EnableMaximizeButton
                                ? new(MaxBtn, Color.FromArgb(100, C1), Color.FromArgb(100, C2), 90)
                                : new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("2", mf, maxBrush, 28, 4);
                        }

                        mlgb = new(ClsBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP3);
                        G.DrawPath(P1, GP3);
                        G.DrawString("r", mf, clsBrush, 52, 4);

                        Cursor = EnableMaximizeButton ? Cursors.Hand : Cursors.Default;
                    }
                    else if (x is > 50 and < 70)
                    {
                        mlgb = new(MinBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP1);
                        G.DrawPath(P1, GP1);
                        G.DrawString("0", mf, minBrush, 4, 4);

                        if (FindForm().WindowState == FormWindowState.Normal)
                        {
                            mlgb = new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("1", mf, maxBrush, 28, 4);
                        }
                        else
                        {
                            mlgb = new(MaxBtn, C1, C2, 90);
                            G.FillPath(mlgb, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("2", mf, maxBrush, 28, 4);
                        }

                        mlgb = EnableCloseButton
                            ? new(ClsBtn, Color.FromArgb(100, C1), Color.FromArgb(100, C2), 90)
                            : new(ClsBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP3);
                        G.DrawPath(P1, GP3);
                        G.DrawString("r", mf, clsBrush, 52, 4);

                        Cursor = EnableCloseButton ? Cursors.Hand : Cursors.Default;
                    }
                    else
                    {
                        mlgb = new(MinBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP1);
                        G.DrawPath(P1, GP1);
                        G.DrawString("0", mf, minBrush, 4, 4);

                        if (FindForm().WindowState == FormWindowState.Normal)
                        {
                            LinearGradientBrush lgb1 = new(MaxBtn, C1, C2, 90);
                            G.FillPath(lgb1, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("1", mf, maxBrush, 28, 4);
                        }
                        else
                        {
                            LinearGradientBrush lgb1 = new(MaxBtn, C1, C2, 90);
                            G.FillPath(lgb1, GP2);
                            G.DrawPath(P1, GP2);
                            G.DrawString("2", mf, maxBrush, 28, 4);
                        }

                        LinearGradientBrush lgb2 = new(ClsBtn, C1, C2, 90);
                        G.FillPath(lgb2, GP3);
                        G.DrawPath(P1, GP3);
                        G.DrawString("r", mf, clsBrush, 52, 4);

                        Cursor = Cursors.Default;
                    }
                    break;
                case MouseStateThunder.Down:
                    mlgb = new(MinBtn, C1, C2, 90);
                    G.FillPath(mlgb, GP1);
                    G.DrawPath(P1, GP1);
                    G.DrawString("0", mf, minBrush, 4, 4);

                    if (FindForm().WindowState == FormWindowState.Normal)
                    {
                        mlgb = new(MaxBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("1", mf, maxBrush, 28, 4);
                    }
                    else
                    {
                        mlgb = new(MaxBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("2", mf, maxBrush, 28, 4);
                    }

                    mlgb = new(ClsBtn, C1, C2, 90);
                    G.FillPath(mlgb, GP3);
                    G.DrawPath(P1, GP3);
                    G.DrawString("r", mf, clsBrush, 52, 4);

                    Cursor = Cursors.Default;
                    break;
                default:
                    mlgb = new(MinBtn, C1, C2, 90);
                    G.FillPath(mlgb, GP1);
                    G.DrawPath(P1, GP1);
                    G.DrawString("0", mf, minBrush, 4, 4);

                    if (FindForm().WindowState == FormWindowState.Normal)
                    {
                        mlgb = new(MaxBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("1", mf, maxBrush, 28, 4);
                    }
                    else
                    {
                        mlgb = new(MaxBtn, C1, C2, 90);
                        G.FillPath(mlgb, GP2);
                        G.DrawPath(P1, GP2);
                        G.DrawString("2", mf, maxBrush, 28, 4);
                    }

                    mlgb = new(ClsBtn, C1, C2, 90);
                    G.FillPath(mlgb, GP3);
                    G.DrawPath(P1, GP3);
                    G.DrawString("r", mf, clsBrush, 52, 4);

                    Cursor = Cursors.Default;
                    break;
            }
            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
}