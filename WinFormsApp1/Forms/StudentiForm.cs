using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class StudentiForm : Panel
    {
        private static readonly Color Primary   = Color.FromArgb(25, 55, 109);
        private static readonly Color Accent    = Color.FromArgb(56, 182, 255);
        private static readonly Color Success   = Color.FromArgb(52, 199, 148);
        private static readonly Color Warning   = Color.FromArgb(255, 182, 57);
        private static readonly Color Danger    = Color.FromArgb(255, 90, 95);
        private static readonly Color BgLight   = Color.FromArgb(245, 248, 255);
        private static readonly Color CardBg    = Color.White;
        private static readonly Color TextDark  = Color.FromArgb(22, 33, 55);
        private static readonly Color TextMid   = Color.FromArgb(90, 105, 135);
        private static readonly Color Border    = Color.FromArgb(225, 232, 248);

        private Panel pnlList;
        private TextBox txtSearch;
        private int selectedRow = -1;

        private readonly string[][] students = {
            new[] { "Marco",    "Rossi",    "3A", "16", "M", "marco.rossi@scuola.it",    "Presente",  "8.2" },
            new[] { "Giulia",   "Ferrari",  "2B", "15", "F", "g.ferrari@scuola.it",      "Presente",  "9.1" },
            new[] { "Luca",     "Bianchi",  "5C", "18", "M", "l.bianchi@scuola.it",      "Assente",   "7.5" },
            new[] { "Alessia",  "Ricci",    "1A", "14", "F", "a.ricci@scuola.it",        "Presente",  "9.4" },
            new[] { "Davide",   "Conti",    "4B", "17", "M", "d.conti@scuola.it",        "Assente",   "6.8" },
            new[] { "Sofia",    "Marino",   "3A", "16", "F", "s.marino@scuola.it",       "Presente",  "8.7" },
            new[] { "Andrea",   "Greco",    "2B", "15", "M", "a.greco@scuola.it",        "Ritardo",   "7.9" },
            new[] { "Chiara",   "Lombardi", "5C", "18", "F", "c.lombardi@scuola.it",     "Presente",  "9.0" },
            new[] { "Matteo",   "Gallo",    "1A", "14", "M", "m.gallo@scuola.it",        "Presente",  "8.3" },
            new[] { "Valentina","Bruno",    "4B", "17", "F", "v.bruno@scuola.it",        "Assente",   "7.1" },
            new[] { "Federico", "Costa",    "3A", "16", "M", "f.costa@scuola.it",        "Presente",  "8.8" },
            new[] { "Elena",    "Fontana",  "2B", "15", "F", "e.fontana@scuola.it",      "Presente",  "9.2" },
        };

        public StudentiForm()
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

            // ── Top bar ─────────────────────────────────────────
            var topBar = new Panel
            {
                Location  = new Point(0, y),
                Height    = 50,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            topBar.Width = ClientSize.Width - 48;
            Resize += (s, e) => topBar.Width = ClientSize.Width - 48;

            // search
            var searchPanel = new Panel
            {
                Width     = 260,
                Height    = 38,
                Location  = new Point(0, 6),
                BackColor = CardBg,
            };
            searchPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, searchPanel.Width - 1, searchPanel.Height - 1), 10);
                using var br   = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen  = new Pen(Border);
                g.DrawPath(pen, path);
                using var fi = new Font("Segoe UI Emoji", 11f);
                using var bc = new SolidBrush(TextMid);
                g.DrawString("🔍", fi, bc, 8, 7);
            };
            txtSearch = new TextBox
            {
                Location    = new Point(34, 9),
                Width       = 210,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Segoe UI", 10f),
                ForeColor   = TextDark,
                BackColor   = CardBg,
                PlaceholderText = "Cerca studente…",
            };
            txtSearch.TextChanged += (s, e) => RefreshList();
            searchPanel.Controls.Add(txtSearch);
            topBar.Controls.Add(searchPanel);

            // filter pills
            string[] filters = { "Tutti", "3A", "2B", "5C", "1A", "4B" };
            int fx = 280;
            foreach (var f in filters)
            {
                var fil = CreateFilterPill(f, fx, 10, f == "Tutti");
                topBar.Controls.Add(fil);
                fx += fil.Width + 8;
            }

            // Add button
            var btnAdd = CreateButton("＋  Nuovo Studente", topBar.Width - 170, 7, Accent);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            topBar.Controls.Add(btnAdd);

            Controls.Add(topBar);
            y += 66;

            // ── Stats strip ─────────────────────────────────────
            var stats = new Panel
            {
                Location  = new Point(0, y),
                Height    = 80,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            stats.Width = ClientSize.Width - 48;
            Resize += (s, e) => stats.Width = ClientSize.Width - 48;
            stats.Paint += (s, e) => DrawStatsStrip(e.Graphics, stats);
            Controls.Add(stats);
            y += 96;

            // ── Table ────────────────────────────────────────────
            pnlList = new Panel
            {
                Location  = new Point(0, y),
                BackColor = CardBg,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            pnlList.Width  = ClientSize.Width - 48;
            pnlList.Height = students.Length * 50 + 48;
            Resize += (s, e) => { pnlList.Width = ClientSize.Width - 48; pnlList.Invalidate(); };
            pnlList.Paint += (s, e) => DrawTable(e.Graphics, pnlList, students);
            pnlList.MouseClick += OnTableClick;
            Controls.Add(pnlList);
        }

        private void DrawStatsStrip(Graphics g, Panel pnl)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            (string label, string val, Color col)[] stats =
            {
                ("Totale Studenti", "1.248", Primary),
                ("Presenti Oggi",   "1.089", Success),
                ("Assenti Oggi",    "108",   Danger),
                ("Ritardi",         "51",    Warning),
                ("Media Voti",      "8.4",   Accent),
            };
            int w = (pnl.Width - 4 * 12) / stats.Length;
            for (int i = 0; i < stats.Length; i++)
            {
                var (lbl, val, col) = stats[i];
                int x = i * (w + 12);
                using var path = RoundedRect(new Rectangle(x, 0, w, 72), 12);
                using var br   = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen  = new Pen(Border);
                g.DrawPath(pen, path);
                using var barPath = RoundedRect(new Rectangle(x, 0, w, 4), 3);
                using var barBr   = new SolidBrush(col);
                g.FillPath(barBr, barPath);
                using var fv = new Font("Segoe UI", 18f, FontStyle.Bold);
                using var bv = new SolidBrush(TextDark);
                g.DrawString(val, fv, bv, x + 14, 14);
                using var fl = new Font("Segoe UI", 8.5f);
                using var bl = new SolidBrush(TextMid);
                g.DrawString(lbl, fl, bl, x + 14, 44);
            }
        }

        private void DrawTable(Graphics g, Panel pnl, string[][] rows)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
            using var br   = new SolidBrush(CardBg);
            g.FillPath(br, path);
            using var pen  = new Pen(Border);
            g.DrawPath(pen, path);

            // header
            string[] cols = { "STUDENTE", "CLASSE", "ETÀ", "SESSO", "EMAIL", "STATO", "MEDIA" };
            int[] xs = { 16, 230, 310, 370, 430, 590, 690 };
            using var fh = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var bh = new SolidBrush(TextMid);
            for (int i = 0; i < cols.Length; i++) g.DrawString(cols[i], fh, bh, xs[i], 15);
            using var sep = new Pen(Color.FromArgb(235, 240, 252));
            g.DrawLine(sep, 12, 38, pnl.Width - 12, 38);

            for (int i = 0; i < rows.Length; i++)
            {
                var r  = rows[i];
                int ry = 48 + i * 50;
                bool sel = (selectedRow == i);

                if (sel)
                {
                    using var hl = new SolidBrush(Color.FromArgb(235, 245, 255));
                    g.FillRectangle(hl, 4, ry, pnl.Width - 8, 48);
                    using var hlPen = new Pen(Accent, 1.5f);
                    g.DrawRectangle(hlPen, 4, ry, pnl.Width - 9, 47);
                }
                else if (i % 2 == 0)
                {
                    using var stripe = new SolidBrush(Color.FromArgb(249, 251, 255));
                    g.FillRectangle(stripe, 4, ry, pnl.Width - 8, 48);
                }

                // avatar
                Color avCol = r[4] == "F" ? Color.FromArgb(255, 182, 200) : Color.FromArgb(180, 215, 255);
                using var avBr = new SolidBrush(avCol);
                g.FillEllipse(avBr, xs[0], ry + 9, 30, 30);
                using var fa = new Font("Segoe UI", 9f, FontStyle.Bold);
                using var ba = new SolidBrush(Primary);
                g.DrawString(r[0][0].ToString(), fa, ba, xs[0] + 9, ry + 14);

                using var fn = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString($"{r[0]} {r[1]}", fn, bd, xs[0] + 38, ry + 10);
                using var fe = new Font("Segoe UI", 8.5f);
                using var be = new SolidBrush(TextMid);
                g.DrawString(r[5], fe, be, xs[0] + 38, ry + 28);

                using var ft = new Font("Segoe UI", 9.5f);
                // classe badge
                using var clsBr = new SolidBrush(Color.FromArgb(30, Accent.R, Accent.G, Accent.B));
                using var clsPath = RoundedRect(new Rectangle(xs[1], ry + 14, 36, 20), 10);
                g.FillPath(clsBr, clsPath);
                using var clsTxt = new SolidBrush(Primary);
                using var fcls = new Font("Segoe UI", 9f, FontStyle.Bold);
                g.DrawString(r[2], fcls, clsTxt, xs[1] + 6, ry + 16);

                g.DrawString(r[3], ft, bd, xs[2], ry + 16);
                // sesso icon
                string genderIcon = r[4] == "F" ? "♀" : "♂";
                Color  genderCol  = r[4] == "F" ? Color.FromArgb(220, 80, 160) : Color.FromArgb(56, 120, 220);
                using var fg2 = new Font("Segoe UI", 12f, FontStyle.Bold);
                using var bg2 = new SolidBrush(genderCol);
                g.DrawString(genderIcon, fg2, bg2, xs[3], ry + 14);

                using var femailF = new Font("Segoe UI", 8.5f);
                g.DrawString(r[5], femailF, be, xs[4], ry + 16);

                // stato badge
                Color   stCol  = r[6] == "Presente" ? Success : r[6] == "Assente" ? Danger : Warning;
                using var stPath = RoundedRect(new Rectangle(xs[5], ry + 13, 72, 22), 11);
                using var stBr   = new SolidBrush(Color.FromArgb(35, stCol.R, stCol.G, stCol.B));
                g.FillPath(stBr, stPath);
                using var fs = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bs = new SolidBrush(stCol);
                g.DrawString(r[6], fs, bs, xs[5] + 8, ry + 16);

                // media
                float media = float.Parse(r[7]);
                Color mCol  = media >= 9f ? Success : media >= 7f ? Accent : Danger;
                using var fm = new Font("Segoe UI", 11f, FontStyle.Bold);
                using var bm = new SolidBrush(mCol);
                g.DrawString(r[7], fm, bm, xs[6], ry + 14);

                // action buttons
                using var btnPath = RoundedRect(new Rectangle(pnl.Width - 90, ry + 13, 28, 22), 6);
                using var btnBr   = new SolidBrush(Color.FromArgb(240, 244, 255));
                g.FillPath(btnBr, btnPath);
                using var fico = new Font("Segoe UI Emoji", 10f);
                using var bico = new SolidBrush(TextMid);
                g.DrawString("✏️", fico, bico, pnl.Width - 88, ry + 13);

                using var btnPath2 = RoundedRect(new Rectangle(pnl.Width - 56, ry + 13, 28, 22), 6);
                g.FillPath(btnBr, btnPath2);
                g.DrawString("🗑️", fico, bico, pnl.Width - 54, ry + 13);

                if (i < rows.Length - 1)
                    g.DrawLine(sep, 12, ry + 48, pnl.Width - 12, ry + 48);
            }
        }

        private void OnTableClick(object s, MouseEventArgs e)
        {
            int rowH = 50, offsetY = 48;
            int idx = (e.Y - offsetY) / rowH;
            if (idx >= 0 && idx < students.Length)
            {
                selectedRow = (selectedRow == idx) ? -1 : idx;
                pnlList.Invalidate();
            }
        }

        private void RefreshList() => pnlList?.Invalidate();

        private Panel CreateFilterPill(string text, int x, int y, bool active)
        {
            int w = TextRenderer.MeasureText(text, new Font("Segoe UI", 9f)).Width + 20;
            var pnl = new Panel
            {
                Location  = new Point(x, y),
                Size      = new Size(w, 28),
                BackColor = active ? Primary : CardBg,
                Cursor    = Cursors.Hand,
            };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool act = pnl.BackColor == Primary;
                using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 14);
                using var br   = new SolidBrush(act ? Primary : CardBg);
                g.FillPath(br, path);
                using var pen  = new Pen(act ? Primary : Border);
                g.DrawPath(pen, path);
                using var ft = new Font("Segoe UI", 9f, act ? FontStyle.Bold : FontStyle.Regular);
                using var bt = new SolidBrush(act ? Color.White : TextMid);
                g.DrawString(text, ft, bt, 8, 6);
            };
            pnl.Click += (s, e) =>
            {
                foreach (Control c in pnl.Parent.Controls)
                    if (c is Panel pp && pp != pnl && pp.Height == 28)
                        pp.BackColor = CardBg;
                pnl.BackColor = Primary;
                pnl.Invalidate(); pnl.Parent.Invalidate(true);
            };
            return pnl;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(160, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = col,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 160, 36), 10));
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
