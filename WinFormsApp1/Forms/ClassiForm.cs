using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class ClassiForm : Panel
    {
        private static readonly Color Primary = Color.FromArgb(25, 55, 109);
        private static readonly Color Accent = Color.FromArgb(56, 182, 255);
        private static readonly Color Success = Color.FromArgb(52, 199, 148);
        private static readonly Color Warning = Color.FromArgb(255, 182, 57);
        private static readonly Color BgLight = Color.FromArgb(245, 248, 255);
        private static readonly Color CardBg = Color.White;
        private static readonly Color TextDark = Color.FromArgb(22, 33, 55);
        private static readonly Color TextMid = Color.FromArgb(90, 105, 135);
        private static readonly Color Border = Color.FromArgb(225, 232, 248);

        private readonly (string id, string nome, string sezione, int studenti, int max, string docRef, string aula, float media, Color col)[] classi =
        {
            ("1A", "Prima",   "A", 24, 28, "Prof. Romano",    "Aula 101", 8.1f, Color.FromArgb(56,182,255)),
            ("1B", "Prima",   "B", 22, 28, "Prof. Barbieri",  "Aula 102", 7.8f, Color.FromArgb(56,182,255)),
            ("2A", "Seconda", "A", 26, 28, "Prof. Santoro",   "Aula 201", 8.4f, Color.FromArgb(52,199,148)),
            ("2B", "Seconda", "B", 25, 28, "Prof. Colombo",   "Aula 202", 8.0f, Color.FromArgb(52,199,148)),
            ("3A", "Terza",   "A", 27, 28, "Prof. Esposito",  "Aula 301", 8.6f, Color.FromArgb(255,182,57)),
            ("3B", "Terza",   "B", 23, 28, "Prof. Barbieri",  "Aula 302", 7.9f, Color.FromArgb(255,182,57)),
            ("4A", "Quarta",  "A", 20, 25, "Prof. Santoro",   "Aula 401", 8.2f, Color.FromArgb(180,100,255)),
            ("4B", "Quarta",  "B", 22, 25, "Prof. Moretti",   "Aula 402", 7.6f, Color.FromArgb(180,100,255)),
            ("5A", "Quinta",  "A", 18, 25, "Prof. Moretti",   "Aula 501", 8.9f, Color.FromArgb(255,120,80)),
            ("5C", "Quinta",  "C", 19, 25, "Prof. Esposito",  "Aula 502", 8.7f, Color.FromArgb(255,120,80)),
        };

        public ClassiForm()
        {
            Dock = DockStyle.Fill;
            BackColor = BgLight;
            Padding = new Padding(24, 20, 24, 24);
            AutoScroll = true;
            Build();
        }

        private void Build()
        {
            int y = 0;

            // ── Top bar ──────────────────────────────────────────
            var topBar = new Panel
            {
                Location = new Point(0, y),
                Height = 50,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            topBar.Width = ClientSize.Width - 48;
            Resize += (s, e) => topBar.Width = ClientSize.Width - 48;

            var btnAdd = CreateButton("＋  Nuova Classe", topBar.Width - 160, 7, Warning);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            topBar.Controls.Add(btnAdd);

            // year filter tabs
            string[] tabs = { "Tutti", "1° Anno", "2° Anno", "3° Anno", "4° Anno", "5° Anno" };
            int tx = 0;
            foreach (var t in tabs)
            {
                var tab = CreateTab(t, tx, 10, t == "Tutti");
                topBar.Controls.Add(tab);
                tx += tab.Width + 6;
            }
            Controls.Add(topBar);
            y += 66;

            // ── Summary bar ──────────────────────────────────────
            var summBar = new Panel
            {
                Location = new Point(0, y),
                Height = 72,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            summBar.Width = ClientSize.Width - 48;
            Resize += (s, e) => { summBar.Width = ClientSize.Width - 48; summBar.Invalidate(); };
            summBar.Paint += DrawSummaryBar;
            Controls.Add(summBar);
            y += 88;

            // ── Classes grid ─────────────────────────────────────
            var grid = new Panel
            {
                Location = new Point(0, y),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            grid.Width = ClientSize.Width - 48;
            Resize += (s, e) => { grid.Width = ClientSize.Width - 48; LayoutGrid(grid); };
            Controls.Add(grid);

            foreach (var cl in classi)
            {
                var card = BuildClassCard(cl);
                grid.Controls.Add(card);
            }
            LayoutGrid(grid);
        }

        private void DrawSummaryBar(object s, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            var pnl = (Panel)s;
            (string lbl, string val, Color col)[] items =
            {
                ("Classi Totali",   "46",    Primary),
                ("Studenti",        "1.248", Accent),
                ("Media per Classe","27",    Success),
                ("Media Voti",      "8.4",   Warning),
            };
            int w = (pnl.Width - (3 * 14)) / 4;
            for (int i = 0; i < items.Length; i++)
            {
                var (lbl, val, col) = items[i];
                int x = i * (w + 14);
                using var path = RoundedRect(new Rectangle(x, 0, w, 64), 12);
                using var br = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen = new Pen(Border); g.DrawPath(pen, path);
                using var barPath = RoundedRect(new Rectangle(x, 0, w, 4), 3);
                using var bBr = new SolidBrush(col); g.FillPath(bBr, barPath);
                using var fv = new Font("Segoe UI", 16f, FontStyle.Bold);
                using var bv = new SolidBrush(TextDark);
                g.DrawString(val, fv, bv, x + 14, 12);
                using var fl = new Font("Segoe UI", 8.5f);
                using var bl = new SolidBrush(TextMid);
                g.DrawString(lbl, fl, bl, x + 14, 40);
            }
        }

        private Panel BuildClassCard((string id, string nome, string sezione, int studenti, int max, string docRef, string aula, float media, Color col) c)
        {
            var card = new Panel { BackColor = CardBg, Cursor = Cursors.Hand, Size = new Size(200, 175) };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen = new Pen(Border); g.DrawPath(pen, path);

                // top gradient band
                var topRect = new Rectangle(0, 0, card.Width, 64);
                using var gradBr = new LinearGradientBrush(topRect, Color.FromArgb(30, c.col.R, c.col.G, c.col.B),
                                                             Color.FromArgb(5, c.col.R, c.col.G, c.col.B), 90f);
                using var topPath = RoundedRect(new Rectangle(0, 0, card.Width, 64), 14);
                g.FillPath(gradBr, topPath);

                // class ID big
                using var fid = new Font("Segoe UI", 28f, FontStyle.Bold);
                using var bid = new SolidBrush(c.col);
                g.DrawString(c.id, fid, bid, 14, 8);

                // nome + sezione
                using var fn = new Font("Segoe UI", 9f);
                using var bn = new SolidBrush(TextMid);
                g.DrawString($"{c.nome} · Sez. {c.sezione}", fn, bn, 16, 50);

                // fill bar
                float pct = (float)c.studenti / c.max;
                int bw2 = card.Width - 28;
                using var bgBar = new SolidBrush(Color.FromArgb(235, 240, 252));
                g.FillRectangle(bgBar, 14, 78, bw2, 8);
                using var fgBar = new LinearGradientBrush(new Rectangle(14, 78, bw2, 8), c.col, Color.FromArgb(180, c.col.R, c.col.G, c.col.B), 0f);
                g.FillRectangle(fgBar, 14, 78, (int)(bw2 * pct), 8);

                using var fp = new Font("Segoe UI", 8f);
                using var bp = new SolidBrush(TextMid);
                g.DrawString($"{c.studenti}/{c.max} studenti", fp, bp, 14, 90);

                // docente referente
                using var fdr = new Font("Segoe UI", 8.5f);
                g.DrawString("👩‍🏫 " + c.docRef, fdr, bp, 14, 114);
                // aula
                g.DrawString("🚪 " + c.aula, fdr, bp, 14, 134);

                // media badge
                Color mCol = c.media >= 8.5f ? Success : c.media >= 7.5f ? Accent : Warning;
                using var mbPath = RoundedRect(new Rectangle(card.Width - 56, 100, 44, 28), 14);
                using var mbBr = new SolidBrush(Color.FromArgb(30, mCol.R, mCol.G, mCol.B));
                g.FillPath(mbBr, mbPath);
                using var fmed = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bmed = new SolidBrush(mCol);
                g.DrawString(c.media.ToString("F1"), fmed, bmed, card.Width - 52, 106);
            };
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(250, 252, 255); card.Invalidate(); };
            card.MouseLeave += (s, e) => { card.BackColor = CardBg; card.Invalidate(); };
            return card;
        }

        private void LayoutGrid(Panel grid)
        {
            int cols = Math.Max(1, grid.Width / 220);
            int gap = 14;
            int cardW = (grid.Width - (gap * (cols - 1))) / cols;
            int cardH = 175;
            for (int i = 0; i < grid.Controls.Count; i++)
            {
                int col = i % cols, row = i / cols;
                grid.Controls[i].Location = new Point(col * (cardW + gap), row * (cardH + gap));
                grid.Controls[i].Size = new Size(cardW, cardH);
                grid.Controls[i].Invalidate();
            }
            grid.Height = (grid.Controls.Count + cols - 1) / cols * (cardH + gap);
        }

        private Panel CreateTab(string text, int x, int y, bool active)
        {
            int w = TextRenderer.MeasureText(text, new Font("Segoe UI", 9f)).Width + 20;
            var pnl = new Panel { Location = new Point(x, y), Size = new Size(w, 28), Cursor = Cursors.Hand };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool act = (pnl.Tag as string) == "active";
                using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
                using var br = new SolidBrush(act ? Primary : CardBg); g.FillPath(br, path);
                using var pen = new Pen(act ? Primary : Border); g.DrawPath(pen, path);
                using var ft = new Font("Segoe UI", 9f, act ? FontStyle.Bold : FontStyle.Regular);
                using var bt = new SolidBrush(act ? Color.White : TextMid);
                g.DrawString(text, ft, bt, 8, 6);
            };
            if (active) pnl.Tag = "active";
            pnl.Click += (s, e) =>
            {
                foreach (Control c in pnl.Parent.Controls)
                    if (c is Panel pp && pp.Height == 28) pp.Tag = null;
                pnl.Tag = "active";
                pnl.Parent.Invalidate(true);
            };
            return pnl;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(154, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = col,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 154, 36), 10));
            return btn;
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - (radius * 2), r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - (radius * 2), r.Bottom - (radius * 2), radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - (radius * 2), radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
