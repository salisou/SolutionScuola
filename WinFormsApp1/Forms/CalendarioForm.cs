using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class CalendarioForm : Panel
    {
        private static readonly Color Primary  = Color.FromArgb(25, 55, 109);
        private static readonly Color Accent   = Color.FromArgb(56, 182, 255);
        private static readonly Color Success  = Color.FromArgb(52, 199, 148);
        private static readonly Color Warning  = Color.FromArgb(255, 182, 57);
        private static readonly Color Danger   = Color.FromArgb(255, 90, 95);
        private static readonly Color Purple   = Color.FromArgb(160, 100, 240);
        private static readonly Color BgLight  = Color.FromArgb(245, 248, 255);
        private static readonly Color CardBg   = Color.White;
        private static readonly Color TextDark = Color.FromArgb(22, 33, 55);
        private static readonly Color TextMid  = Color.FromArgb(90, 105, 135);
        private static readonly Color Border   = Color.FromArgb(225, 232, 248);

        private DateTime currentWeekStart;
        private Panel pnlWeek;
        private Label lblWeekTitle;

        // (day 0=Mon, hour, title, col, duration slots)
        private readonly (int day, int hour, string title, string sub, Color col, int slots)[] events =
        {
            (0, 8,  "Matematica 3A",     "Aula 301", Color.FromArgb(56,182,255),  2),
            (0, 11, "Consiglio Classe",  "Sala Riunioni", Warning,               1),
            (1, 9,  "Italiano 2B",       "Aula 202", Success,                    2),
            (1, 14, "Riunione Genitori", "Aula Magna", Danger,                   2),
            (2, 8,  "Fisica 5C",         "Lab. Sci.", Purple,                    2),
            (2, 10, "Inglese 2A",        "Aula 201", Accent,                     1),
            (2, 15, "Verifica Fisica",   "Aula 502", Danger,                     1),
            (3, 9,  "Storia 1A",         "Aula 101", Warning,                    2),
            (3, 11, "Arte 1B",           "Aula Arte", Color.FromArgb(255,150,80),1),
            (4, 8,  "Ed. Fisica 2A",     "Palestra",  Success,                   2),
            (4, 10, "Matematica 4B",     "Aula 402", Accent,                     2),
            (4, 14, "Colloquio",         "Ufficio",   Primary,                   1),
        };

        public CalendarioForm()
        {
            Dock = DockStyle.Fill;
            BackColor = BgLight;
            Padding = new Padding(24, 20, 24, 24);
            AutoScroll = true;
            currentWeekStart = GetMonday(DateTime.Today);
            Build();
        }

        private DateTime GetMonday(DateTime d)
        {
            int diff = ((int)d.DayOfWeek - 1 + 7) % 7;
            return d.AddDays(-diff).Date;
        }

        private void Build()
        {
            int y = 0;

            // ── Header bar ──────────────────────────────────────
            var header = new Panel { Location = new Point(0, y), Height = 52, BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            header.Width = ClientSize.Width - 48;
            Resize += (s, e) => header.Width = ClientSize.Width - 48;

            var btnPrev = CreateNavBtn("◀", 0, 8);
            lblWeekTitle = new Label
            {
                AutoSize  = false,
                Width     = 280,
                Height    = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = TextDark,
                Location  = new Point(46, 8),
            };
            var btnNext = CreateNavBtn("▶", 334, 8);
            var btnToday = CreateButton("Oggi", 360, 11, Accent);
            btnToday.Width = 70;

            btnPrev.Click  += (s, e) => { currentWeekStart = currentWeekStart.AddDays(-7); RefreshWeek(); };
            btnNext.Click  += (s, e) => { currentWeekStart = currentWeekStart.AddDays(7);  RefreshWeek(); };
            btnToday.Click += (s, e) => { currentWeekStart = GetMonday(DateTime.Today);    RefreshWeek(); };

            header.Controls.AddRange(new Control[] { btnPrev, lblWeekTitle, btnNext, btnToday });

            // legend
            (string lbl, Color col)[] legend =
            {
                ("Lezioni", Accent), ("Verifiche", Danger), ("Riunioni", Warning), ("Eventi", Success)
            };
            int lx = 450;
            foreach (var (lbl, col) in legend)
            {
                var leg = new Label
                {
                    Text = "● " + lbl, AutoSize = true,
                    Font = new Font("Segoe UI", 8.5f), ForeColor = col,
                    Location = new Point(lx, 16),
                };
                header.Controls.Add(leg);
                lx += 80;
            }

            Controls.Add(header);
            y += 68;

            // ── Weekly grid ─────────────────────────────────────
            pnlWeek = new Panel
            {
                Location  = new Point(0, y),
                BackColor = CardBg,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            pnlWeek.Width  = ClientSize.Width - 48;
            pnlWeek.Height = 560;
            Resize += (s, e) => { pnlWeek.Width = ClientSize.Width - 48; pnlWeek.Invalidate(); };
            pnlWeek.Paint += DrawWeek;
            Controls.Add(pnlWeek);

            RefreshWeek();
        }

        private void RefreshWeek()
        {
            DateTime end = currentWeekStart.AddDays(4);
            lblWeekTitle.Text = $"{currentWeekStart:d MMMM} – {end:d MMMM yyyy}";
            pnlWeek?.Invalidate();
        }

        private void DrawWeek(object s, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int timeW  = 56;
            int colW   = (pnlWeek.Width - timeW) / 5;
            int hourH  = 48;
            int startH = 8;
            int hours  = 10; // 8-17
            int headerH = 44;

            // rounded card
            using var path = RoundedRect(new Rectangle(0, 0, pnlWeek.Width - 1, pnlWeek.Height - 1), 14);
            using var bgBr = new SolidBrush(CardBg); g.FillPath(bgBr, path);
            using var pen2 = new Pen(Border); g.DrawPath(pen2, path);

            string[] days = { "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì" };
            using var pen = new Pen(Color.FromArgb(235, 240, 252));

            // day headers
            for (int d2 = 0; d2 < 5; d2++)
            {
                int x  = timeW + d2 * colW;
                DateTime day = currentWeekStart.AddDays(d2);
                bool isToday = day.Date == DateTime.Today;

                if (isToday)
                {
                    using var todayBr = new SolidBrush(Color.FromArgb(20, Accent.R, Accent.G, Accent.B));
                    g.FillRectangle(todayBr, x, 0, colW, pnlWeek.Height);
                }

                // day name
                using var fdn = new Font("Segoe UI", 9f, isToday ? FontStyle.Bold : FontStyle.Regular);
                using var bdn = new SolidBrush(isToday ? Primary : TextMid);
                g.DrawString(days[d2], fdn, bdn, x + 6, 8);

                // date number
                using var fdd = new Font("Segoe UI", 14f, FontStyle.Bold);
                using var bdd = new SolidBrush(isToday ? Accent : TextDark);
                g.DrawString(day.Day.ToString(), fdd, bdd, x + 6, 22);

                if (isToday)
                {
                    using var dotBr = new SolidBrush(Accent);
                    g.FillEllipse(dotBr, x + 8, headerH - 8, 6, 6);
                }
            }

            g.DrawLine(new Pen(Border, 1.5f), 0, headerH, pnlWeek.Width, headerH);

            // hour labels + grid lines
            for (int h = 0; h < hours; h++)
            {
                int hy = headerH + h * hourH;
                using var fh = new Font("Segoe UI", 8f);
                using var bh = new SolidBrush(TextMid);
                g.DrawString($"{startH + h}:00", fh, bh, 6, hy + 6);
                g.DrawLine(pen, timeW, hy, pnlWeek.Width, hy);
            }
            // vertical lines
            for (int d2 = 1; d2 < 5; d2++)
                g.DrawLine(pen, timeW + d2 * colW, headerH, timeW + d2 * colW, pnlWeek.Height);

            // events
            foreach (var ev in events)
            {
                int x   = timeW + ev.day * colW + 4;
                int y2  = headerH + (ev.hour - startH) * hourH + 3;
                int w   = colW - 8;
                int h   = ev.slots * hourH - 6;

                using var evPath = RoundedRect(new Rectangle(x, y2, w, h), 8);
                using var evBr   = new SolidBrush(Color.FromArgb(30, ev.col.R, ev.col.G, ev.col.B));
                g.FillPath(evBr, evPath);
                using var evPen  = new Pen(Color.FromArgb(100, ev.col.R, ev.col.G, ev.col.B), 1.5f);
                g.DrawPath(evPen, evPath);
                // left bar
                using var barBr = new SolidBrush(ev.col);
                g.FillRectangle(barBr, x, y2 + 4, 4, h - 8);

                using var ft = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bt = new SolidBrush(ev.col);
                g.DrawString(ev.title, ft, bt, x + 8, y2 + 4);
                if (h > 26)
                {
                    using var fs = new Font("Segoe UI", 7.5f);
                    using var bs = new SolidBrush(TextMid);
                    g.DrawString(ev.sub, fs, bs, x + 8, y2 + 19);
                }
            }

            // current time line
            if (currentWeekStart <= DateTime.Today && DateTime.Today < currentWeekStart.AddDays(5))
            {
                int todayCol  = (int)(DateTime.Today - currentWeekStart).TotalDays;
                int tx        = timeW + todayCol * colW;
                float fracH   = headerH + (DateTime.Now.Hour - startH) * hourH + DateTime.Now.Minute * hourH / 60f;
                using var timePen = new Pen(Danger, 2f);
                g.DrawLine(timePen, tx, (int)fracH, tx + colW, (int)fracH);
                using var timeBr = new SolidBrush(Danger);
                g.FillEllipse(timeBr, tx - 4, (int)fracH - 4, 8, 8);
            }
        }

        private Button CreateNavBtn(string text, int x, int y)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(36, 36),
                FlatStyle = FlatStyle.Flat, BackColor = CardBg, ForeColor = TextDark,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold), Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderColor = Border;
            btn.FlatAppearance.BorderSize  = 1;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 36, 36), 8));
            return btn;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(90, 30),
                FlatStyle = FlatStyle.Flat, BackColor = col, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 90, 30), 8));
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
