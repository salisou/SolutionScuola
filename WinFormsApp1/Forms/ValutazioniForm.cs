using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class ValutazioniForm : Panel
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

        private readonly (string student, string classe, string materia, float[] voti, float media)[] grades =
        {
            ("Marco Rossi",      "3A", "Matematica", new[] {7f, 8f, 6.5f, 7.5f, 8f},   7.4f),
            ("Giulia Ferrari",   "2B", "Italiano",   new[] {9f, 9.5f, 8.5f, 9f, 9.5f}, 9.1f),
            ("Luca Bianchi",     "5C", "Fisica",     new[] {6f, 7f, 7.5f, 6.5f, 8f},   7.0f),
            ("Alessia Ricci",    "1A", "Inglese",    new[] {9f, 10f, 9f, 9.5f, 9.5f},  9.4f),
            ("Davide Conti",     "4B", "Storia",     new[] {6f, 5.5f, 7f, 6f, 7f},     6.3f),
            ("Sofia Marino",     "3A", "Scienze",    new[] {8f, 8.5f, 9f, 8f, 9f},     8.5f),
            ("Andrea Greco",     "2B", "Arte",       new[] {8f, 7.5f, 8.5f, 8f, 9f},   8.2f),
            ("Chiara Lombardi",  "5C", "Italiano",   new[] {9.5f, 9f, 9.5f, 10f, 9f},  9.4f),
        };

        private readonly string[] materie = { "Matematica", "Italiano", "Inglese", "Fisica", "Storia", "Scienze", "Arte" };
        private readonly float[]  mediaMaterie = { 7.8f, 8.4f, 8.2f, 7.5f, 7.1f, 8.3f, 8.0f };

        public ValutazioniForm()
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

            // ── Filters ──────────────────────────────────────────
            var filterBar = new Panel { Location = new Point(0, y), Height = 50, BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            filterBar.Width = ClientSize.Width - 48;
            Resize += (s, e) => filterBar.Width = ClientSize.Width - 48;

            string[] cls = { "Tutte le Classi", "1A", "2B", "3A", "4B", "5C" };
            int fx = 0;
            foreach (var c in cls)
            {
                int w2 = TextRenderer.MeasureText(c, new Font("Segoe UI", 9f)).Width + 20;
                var pill = CreatePill(c, fx, 10, c == "Tutte le Classi");
                filterBar.Controls.Add(pill);
                fx += w2 + 8;
            }
            var btnNuova = CreateButton("＋  Nuova Valutazione", filterBar.Width - 175, 7, Success);
            btnNuova.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            filterBar.Controls.Add(btnNuova);
            Controls.Add(filterBar);
            y += 66;

            // ── Two-column layout ────────────────────────────────
            var left  = new Panel { Location = new Point(0, y), BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            var right = new Panel { Location = new Point(0, y), BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            Action layout = () =>
            {
                int total = ClientSize.Width - 48;
                int lw    = (int)(total * 0.62) - 8;
                int rw    = total - lw - 16;
                left.Width  = lw;
                right.Width = rw;
                right.Location = new Point(lw + 16, y);
            };
            Resize += (s, e) => layout();
            layout();

            // ── Grade table (left) ───────────────────────────────
            left.Height = grades.Length * 60 + 52;
            left.Paint += (s, e) => DrawGradeTable(e.Graphics, left);
            Controls.Add(left);

            // ── Right column ─────────────────────────────────────
            right.Height = 400;

            // Chart card
            var chartCard = new Panel { Location = new Point(0, 0), Height = 200, BackColor = CardBg,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            chartCard.Width = right.Width;
            right.Resize += (s, e) => { chartCard.Width = right.Width; chartCard.Invalidate(); };
            chartCard.Paint += DrawMatChart;
            right.Controls.Add(chartCard);

            // Distribution card
            var distCard = new Panel { Location = new Point(0, 216), Height = 200, BackColor = CardBg,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            distCard.Width = right.Width;
            right.Resize += (s, e) => { distCard.Width = right.Width; distCard.Invalidate(); };
            distCard.Paint += DrawDistribution;
            right.Controls.Add(distCard);

            Controls.Add(right);
        }

        private void DrawGradeTable(Graphics g, Panel pnl)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
            using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
            using var pen  = new Pen(Border); g.DrawPath(pen, path);

            string[] cols = { "STUDENTE", "CL.", "MATERIA", "VOTI", "MEDIA", "TREND" };
            int[] xs = { 16, 170, 220, 320, 530, 590 };
            using var fh = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var bh = new SolidBrush(TextMid);
            for (int i = 0; i < cols.Length; i++) g.DrawString(cols[i], fh, bh, xs[i], 16);

            using var sep = new Pen(Color.FromArgb(235, 240, 252));
            g.DrawLine(sep, 12, 38, pnl.Width - 12, 38);

            for (int i = 0; i < grades.Length; i++)
            {
                var (student, classe, materia, voti, media) = grades[i];
                int ry = 48 + i * 60;
                if (i % 2 == 0) { using var stripe = new SolidBrush(Color.FromArgb(249, 251, 255)); g.FillRectangle(stripe, 4, ry, pnl.Width - 8, 58); }

                // avatar
                using var avBr = new SolidBrush(Color.FromArgb(30, Accent.R, Accent.G, Accent.B));
                g.FillEllipse(avBr, xs[0], ry + 14, 30, 30);
                using var fa = new Font("Segoe UI", 9f, FontStyle.Bold);
                using var ba = new SolidBrush(Primary);
                g.DrawString(student[0].ToString(), fa, ba, xs[0] + 9, ry + 19);
                using var fn = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString(student, fn, bd, xs[0] + 36, ry + 18);

                // classe
                using var fc = new Font("Segoe UI", 9f, FontStyle.Bold);
                using var bc = new SolidBrush(Primary);
                g.DrawString(classe, fc, bc, xs[1], ry + 18);

                // materia
                using var fm = new Font("Segoe UI", 9f);
                using var bm = new SolidBrush(TextMid);
                g.DrawString(materia, fm, bm, xs[2], ry + 18);

                // voti bubbles
                for (int v = 0; v < voti.Length && v < 5; v++)
                {
                    float voto = voti[v];
                    Color vc = voto >= 8.5f ? Success : voto >= 6.5f ? Accent : Danger;
                    int bx = xs[3] + v * 42;
                    using var vBr = new SolidBrush(Color.FromArgb(30, vc.R, vc.G, vc.B));
                    g.FillEllipse(vBr, bx, ry + 14, 32, 30);
                    using var fv = new Font("Segoe UI", 9f, FontStyle.Bold);
                    using var bv = new SolidBrush(vc);
                    string vs = voto % 1 == 0 ? ((int)voto).ToString() : voto.ToString("F1");
                    g.DrawString(vs, fv, bv, bx + (vs.Length > 2 ? 3 : 8), ry + 20);
                }

                // media
                Color mCol = media >= 8.5f ? Success : media >= 6.5f ? Accent : Danger;
                using var fmed = new Font("Segoe UI", 13f, FontStyle.Bold);
                using var bmed = new SolidBrush(mCol);
                g.DrawString(media.ToString("F1"), fmed, bmed, xs[4] + 6, ry + 16);

                // trend (last 2 voti)
                if (voti.Length >= 2)
                {
                    string trend = voti[voti.Length - 1] >= voti[voti.Length - 2] ? "↑" : "↓";
                    Color  tc    = trend == "↑" ? Success : Danger;
                    using var ft2 = new Font("Segoe UI", 14f, FontStyle.Bold);
                    using var bt2 = new SolidBrush(tc);
                    g.DrawString(trend, ft2, bt2, xs[5], ry + 14);
                }

                if (i < grades.Length - 1) g.DrawLine(sep, 12, ry + 58, pnl.Width - 12, ry + 58);
            }
        }

        private void DrawMatChart(object s, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            var pnl = (Panel)s;
            using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
            using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
            using var pen  = new Pen(Border); g.DrawPath(pen, path);

            using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var bd = new SolidBrush(TextDark);
            g.DrawString("Media per Materia", ft, bd, 16, 14);

            int bx = 14, by = 40, bw = pnl.Width - 28, bh = 130;
            int barW = bw / materie.Length - 6;

            Color[] cols = { Accent, Success, Warning, Color.FromArgb(160,100,240), Danger, Color.FromArgb(255,150,80), Color.FromArgb(100,200,180) };

            for (int i = 0; i < materie.Length; i++)
            {
                float pct  = (mediaMaterie[i] - 5f) / 5f;
                int   barH = (int)(bh * pct);
                int   x    = bx + i * (barW + 6);
                Color col  = cols[i % cols.Length];

                // bg bar
                using var bgBr = new SolidBrush(Color.FromArgb(235, 240, 252));
                g.FillRectangle(bgBr, x, by, barW, bh);
                // fill bar
                using var fgBr = new LinearGradientBrush(new Rectangle(x, by + bh - barH, barW, barH), col, Color.FromArgb(150, col.R, col.G, col.B), 90f);
                g.FillRectangle(fgBr, x, by + bh - barH, barW, barH);
                // value
                using var fv = new Font("Segoe UI", 8f, FontStyle.Bold);
                using var bv = new SolidBrush(col);
                g.DrawString(mediaMaterie[i].ToString("F1"), fv, bv, x + (barW > 30 ? 4 : 2), by + bh - barH - 16);
                // label
                using var fl = new Font("Segoe UI", 7f);
                using var bl = new SolidBrush(TextMid);
                var lbl2 = materie[i].Length > 7 ? materie[i][..5] + "." : materie[i];
                g.DrawString(lbl2, fl, bl, x, by + bh + 4);
            }
        }

        private void DrawDistribution(object s, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            var pnl = (Panel)s;
            using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
            using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
            using var pen  = new Pen(Border); g.DrawPath(pen, path);

            using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var bd = new SolidBrush(TextDark);
            g.DrawString("Distribuzione Voti", ft, bd, 16, 14);

            (string lbl, int pct, Color col)[] dist =
            {
                ("Ottimo (9-10)",   28, Success),
                ("Buono (7-8.9)",   42, Accent),
                ("Suff. (6-6.9)",   20, Warning),
                ("Insuff. (<6)",    10, Danger),
            };

            int ry = 42;
            foreach (var (lbl, pct, col) in dist)
            {
                using var fl = new Font("Segoe UI", 9f);
                using var bl = new SolidBrush(TextDark);
                g.DrawString(lbl, fl, bl, 16, ry + 2);
                using var fp = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bp = new SolidBrush(col);
                g.DrawString($"{pct}%", fp, bp, pnl.Width - 46, ry + 2);

                int barW = pnl.Width - 100;
                using var bgBr = new SolidBrush(Color.FromArgb(235, 240, 252));
                g.FillRectangle(bgBr, 16, ry + 20, barW, 10);
                using var fgBr = new SolidBrush(col);
                g.FillRectangle(fgBr, 16, ry + 20, (int)(barW * pct / 100f), 10);
                ry += 40;
            }
        }

        private Panel CreatePill(string text, int x, int y, bool active)
        {
            int w = TextRenderer.MeasureText(text, new Font("Segoe UI", 9f)).Width + 20;
            var pnl = new Panel { Location = new Point(x, y), Size = new Size(w, 28), Cursor = Cursors.Hand };
            pnl.Tag = active ? "active" : null;
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool act = pnl.Tag as string == "active";
                using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
                using var br   = new SolidBrush(act ? Primary : CardBg); g.FillPath(br, path);
                using var pen  = new Pen(act ? Primary : Border); g.DrawPath(pen, path);
                using var ft   = new Font("Segoe UI", 9f, act ? FontStyle.Bold : FontStyle.Regular);
                using var bt   = new SolidBrush(act ? Color.White : TextMid);
                g.DrawString(text, ft, bt, 8, 6);
            };
            pnl.Click += (s, e) =>
            {
                foreach (Control c in pnl.Parent.Controls)
                    if (c is Panel pp && pp.Height == 28) pp.Tag = null;
                pnl.Tag = "active"; pnl.Parent.Invalidate(true);
            };
            return pnl;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(168, 36),
                FlatStyle = FlatStyle.Flat, BackColor = col, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 168, 36), 10));
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
