using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class ComunicazioniForm : Panel
    {
        private static readonly Color Primary  = Color.FromArgb(25, 55, 109);
        private static readonly Color Accent   = Color.FromArgb(56, 182, 255);
        private static readonly Color Success  = Color.FromArgb(52, 199, 148);
        private static readonly Color Warning  = Color.FromArgb(255, 182, 57);
        private static readonly Color Danger   = Color.FromArgb(255, 90, 95);
        private static readonly Color BgLight  = Color.FromArgb(245, 248, 255);
        private static readonly Color CardBg   = Color.White;
        private static readonly Color TextDark = Color.FromArgb(22, 33, 55);
        private static readonly Color TextMid  = Color.FromArgb(90, 105, 135);
        private static readonly Color Border   = Color.FromArgb(225, 232, 248);
        private static readonly Color MsgSent  = Color.FromArgb(25, 55, 109);
        private static readonly Color MsgRecv  = Color.FromArgb(240, 244, 255);

        private Panel pnlConvList, pnlChat;
        private int selectedConv = 0;

        private readonly (string name, string avatar, string last, string time, int unread, Color col, bool online)[] convs =
        {
            ("Genitori Rossi",    "GR", "Riguardo l'assenza di Marco...",    "09:42", 2, Color.FromArgb(56,182,255), true),
            ("Prof. Esposito",    "LE", "La verifica è confermata per...",   "09:15", 0, Color.FromArgb(52,199,148), true),
            ("Segreteria",        "SE", "Circolare n.47 - Si comunica...",   "Ieri",  1, Color.FromArgb(255,182,57), false),
            ("Genitori Ferrari",  "GF", "Grazie per la disponibilità!",      "Ieri",  0, Color.FromArgb(180,100,255), false),
            ("Preside",           "PR", "Riunione convocata per Lunedì...",  "Lun",   3, Danger, false),
            ("Classe 3A",         "3A", "Promemoria: domani si porta...",    "Dom",   0, Color.FromArgb(255,120,80), false),
        };

        private readonly (bool sent, string text, string time)[][] messages =
        {
            new[] {
                (false, "Buongiorno Professore, volevo segnalare che Marco non è riuscito a completare i compiti perché non stava bene.", "08:30"),
                (true,  "Buongiorno, capisco. Può farmi avere la documentazione medica?", "08:45"),
                (false, "Certamente, gliela mando subito per email.", "08:47"),
                (true,  "Perfetto, grazie. Come sta Marco adesso?", "09:00"),
                (false, "Sta meglio, sarà presente domani.", "09:42"),
            },
            new[] {
                (true,  "Buongiorno Laura, la verifica di matematica è confermata per venerdì?", "08:50"),
                (false, "Sì, venerdì prossimo. Gli argomenti sono: equazioni di 2° grado e sistemi.", "09:00"),
                (true,  "Perfetto, lo comunico alla classe.", "09:05"),
                (false, "La verifica è confermata per venerdì mattina.", "09:15"),
            },
        };

        public ComunicazioniForm()
        {
            Dock = DockStyle.Fill;
            BackColor = BgLight;
            Padding = new Padding(24, 20, 24, 24);
            Build();
        }

        private void Build()
        {
            // ── Outer wrapper ────────────────────────────────────
            var wrapper = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = CardBg,
            };
            wrapper.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, wrapper.Width - 1, wrapper.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);
            };
            Controls.Add(wrapper);

            // ── Left: conversation list ──────────────────────────
            pnlConvList = new Panel
            {
                Location  = new Point(0, 0),
                Width     = 280,
                BackColor = Color.FromArgb(248, 250, 255),
                Dock      = DockStyle.Left,
                BorderStyle = BorderStyle.None,
            };
            pnlConvList.Paint += DrawConvListBg;

            // Search
            var searchBox = new Panel { Location = new Point(12, 12), Size = new Size(256, 36), BackColor = CardBg };
            searchBox.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, searchBox.Width - 1, searchBox.Height - 1), 10);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);
                using var fi = new Font("Segoe UI Emoji", 11f);
                using var bc = new SolidBrush(TextMid);
                g.DrawString("🔍", fi, bc, 8, 7);
                using var ft = new Font("Segoe UI", 9f);
                g.DrawString("Cerca conversazione…", ft, bc, 34, 10);
            };
            pnlConvList.Controls.Add(searchBox);

            // New message button
            var btnNew = CreateIconBtn("✏️", 256 - 0, 12);
            pnlConvList.Controls.Add(btnNew);

            // Conversations
            for (int i = 0; i < convs.Length; i++)
            {
                int idx = i;
                var item = BuildConvItem(i);
                item.Location = new Point(0, 60 + i * 72);
                item.MouseClick += (s, e) => { selectedConv = idx; RefreshChat(); HighlightConv(idx); };
                pnlConvList.Controls.Add(item);
            }
            wrapper.Controls.Add(pnlConvList);

            // ── Right: chat area ─────────────────────────────────
            pnlChat = new Panel { BackColor = BgLight, Dock = DockStyle.Fill };
            wrapper.Controls.Add(pnlChat);

            RefreshChat();
            HighlightConv(0);
        }

        private void DrawConvListBg(object s, PaintEventArgs e)
        {
            var g   = e.Graphics;
            var pnl = (Panel)s;
            using var pen = new Pen(Border);
            g.DrawLine(pen, pnl.Width - 1, 0, pnl.Width - 1, pnl.Height);
        }

        private Panel BuildConvItem(int i)
        {
            var cv  = convs[i];
            var pnl = new Panel { Size = new Size(280, 70), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool sel = (selectedConv == i);
                if (sel)
                {
                    using var selBr = new SolidBrush(Color.FromArgb(235, 245, 255));
                    g.FillRectangle(selBr, 0, 0, pnl.Width, pnl.Height);
                    using var bar = new SolidBrush(Accent);
                    g.FillRectangle(bar, 0, 12, 4, 46);
                }

                // avatar
                using var avBr = new SolidBrush(cv.col);
                g.FillEllipse(avBr, 12, 16, 40, 40);
                using var fw = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bw = new SolidBrush(Color.White);
                g.DrawString(cv.avatar, fw, bw, 16, 24);

                if (cv.online)
                {
                    using var dotBr = new SolidBrush(Success);
                    g.FillEllipse(dotBr, 42, 48, 10, 10);
                    using var dotPen = new Pen(Color.FromArgb(248, 250, 255), 2);
                    g.DrawEllipse(dotPen, 42, 48, 10, 10);
                }

                using var fn = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString(cv.name, fn, bd, 60, 16);
                using var ft = new Font("Segoe UI", 9f);
                using var bm = new SolidBrush(TextMid);
                string last = cv.last.Length > 28 ? cv.last[..26] + "…" : cv.last;
                g.DrawString(last, ft, bm, 60, 34);
                using var fti = new Font("Segoe UI", 7.5f);
                g.DrawString(cv.time, fti, bm, pnl.Width - 46, 17);

                if (cv.unread > 0)
                {
                    using var badgeBr = new SolidBrush(Accent);
                    g.FillEllipse(badgeBr, pnl.Width - 32, 36, 20, 20);
                    using var fbd = new Font("Segoe UI", 8f, FontStyle.Bold);
                    g.DrawString(cv.unread.ToString(), fbd, bw, pnl.Width - 28, 40);
                }

                using var sep = new Pen(Border);
                g.DrawLine(sep, 12, pnl.Height - 1, pnl.Width - 12, pnl.Height - 1);
            };
            pnl.MouseEnter += (s, e) => { if (selectedConv != i) pnl.BackColor = Color.FromArgb(242, 246, 255); };
            pnl.MouseLeave += (s, e) => { if (selectedConv != i) pnl.BackColor = Color.Transparent; };
            return pnl;
        }

        private void HighlightConv(int idx)
        {
            foreach (Control c in pnlConvList.Controls)
                if (c is Panel p && p.Height == 70) { p.BackColor = Color.Transparent; p.Invalidate(); }
        }

        private void RefreshChat()
        {
            pnlChat.Controls.Clear();
            var cv = convs[selectedConv];

            // Chat header
            var header = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = CardBg };
            header.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var avBr = new SolidBrush(cv.col);
                g.FillEllipse(avBr, 16, 12, 40, 40);
                using var fw = new Font("Segoe UI", 11f, FontStyle.Bold);
                using var bw = new SolidBrush(Color.White);
                g.DrawString(cv.avatar, fw, bw, 20, 20);
                using var fn = new Font("Segoe UI", 11f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString(cv.name, fn, bd, 66, 16);
                using var fs = new Font("Segoe UI", 8.5f);
                using var bs = new SolidBrush(cv.online ? Success : TextMid);
                g.DrawString(cv.online ? "● Online" : "● Offline", fs, bs, 68, 36);
                using var pen = new Pen(Border);
                g.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };
            pnlChat.Controls.Add(header);

            // Messages scroll area
            var msgArea = new Panel { BackColor = BgLight, AutoScroll = true, Padding = new Padding(16, 12, 16, 12) };
            msgArea.Dock = DockStyle.Fill;
            pnlChat.Controls.Add(msgArea);
            msgArea.BringToFront();

            var msgs = selectedConv < messages.Length ? messages[selectedConv] : Array.Empty<(bool, string, string)>();
            int my = 8;
            foreach (var (sent, text, time) in msgs)
            {
                var bubble = BuildBubble(text, time, sent, msgArea.ClientSize.Width - 32);
                bubble.Location = new Point(sent ? 120 : 8, my);
                msgArea.Controls.Add(bubble);
                my += bubble.Height + 8;
            }

            // Input bar
            var inputBar = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = CardBg };
            inputBar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using var pen = new Pen(Border);
                g.DrawLine(pen, 0, 0, inputBar.Width, 0);
            };
            var txtInput = new TextBox
            {
                Location = new Point(16, 18), Width = inputBar.Width - 130,
                Height = 34, BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10f), BackColor = BgLight,
                ForeColor = TextDark, PlaceholderText = "Scrivi un messaggio…",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            inputBar.Controls.Add(txtInput);
            var btnSend = CreateButton("Invia ➤", inputBar.Width - 100, 16, Accent);
            btnSend.Width = 86; btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            inputBar.Controls.Add(btnSend);
            pnlChat.Controls.Add(inputBar);

            header.BringToFront();
            inputBar.BringToFront();
        }

        private Panel BuildBubble(string text, string time, bool sent, int maxW)
        {
            int bubW  = Math.Min(maxW, 420);
            var size  = TextRenderer.MeasureText(text, new Font("Segoe UI", 9.5f),
                                                  new Size(bubW - 32, 1000), TextFormatFlags.WordBreak);
            int h     = size.Height + 44;
            var pnl   = new Panel { Size = new Size(maxW, h), BackColor = Color.Transparent };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                int bx = sent ? (maxW - bubW - 8) : 8;
                using var bubPath = RoundedRect(new Rectangle(bx, 0, bubW, h - 10), 14);
                Color bg   = sent ? MsgSent : MsgRecv;
                Color textC = sent ? Color.White : TextDark;
                using var bubBr = new SolidBrush(bg); g.FillPath(bubBr, bubPath);
                using var ft = new Font("Segoe UI", 9.5f);
                using var bt = new SolidBrush(textC);
                g.DrawString(text, ft, bt, new Rectangle(bx + 12, 10, bubW - 24, h - 20));
                using var fti = new Font("Segoe UI", 7.5f);
                using var bti = new SolidBrush(sent ? Color.FromArgb(180, 220, 255) : TextMid);
                g.DrawString(time, fti, bti, bx + bubW - 46, h - 22);
            };
            return pnl;
        }

        private Panel CreateIconBtn(string icon, int x, int y)
        {
            var pnl = new Panel { Location = new Point(x, y), Size = new Size(38, 36), Cursor = Cursors.Hand, BackColor = Color.Transparent };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, 37, 35), 8);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);
                using var fi = new Font("Segoe UI Emoji", 14f);
                using var bi = new SolidBrush(TextMid);
                g.DrawString(icon, fi, bi, 5, 5);
            };
            return pnl;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(110, 36),
                FlatStyle = FlatStyle.Flat, BackColor = col, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 110, 36), 10));
            return btn;
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
