using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class MainForm : Form
    {
        // ── colori brand ──────────────────────────────────────────
        private static readonly Color Primary    = Color.FromArgb(25,  55, 109);   // blu scuro
        private static readonly Color Accent     = Color.FromArgb(56, 182, 255);   // celeste vivace
        private static readonly Color Success    = Color.FromArgb(52, 199, 148);   // verde acqua
        private static readonly Color Warning    = Color.FromArgb(255, 182,  57);  // giallo ambra
        private static readonly Color Danger     = Color.FromArgb(255,  90,  95);  // rosso corallo
        private static readonly Color BgLight    = Color.FromArgb(245, 248, 255);
        private static readonly Color CardBg     = Color.White;
        private static readonly Color TextDark   = Color.FromArgb(22,  33,  55);
        private static readonly Color TextMid    = Color.FromArgb(90, 105, 135);
        private static readonly Color Sidebar    = Color.FromArgb(18,  42,  89);
        private static readonly Color SideHover  = Color.FromArgb(30,  65, 130);

        // ── pannelli principali ───────────────────────────────────
        private Panel pnlSidebar, pnlHeader, pnlContent;
        private Panel activeSideItem = null;

        public MainForm()
        {
            InitializeForm();
            BuildSidebar();
            BuildHeader();
            BuildDashboard();
        }

        // ─────────────────────────────────────────────────────────
        //  FORM
        // ─────────────────────────────────────────────────────────
        private void InitializeForm()
        {
            Text            = "ScuolaCRM – Gestione Scolastica";
            Size            = new Size(1280, 800);
            MinimumSize     = new Size(1100, 700);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = BgLight;
            Font            = new Font("Segoe UI", 9f);
            DoubleBuffered  = true;
        }

        // ─────────────────────────────────────────────────────────
        //  SIDEBAR  (220 px)
        // ─────────────────────────────────────────────────────────
        private void BuildSidebar()
        {
            pnlSidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 220,
                BackColor = Sidebar,
            };
            Controls.Add(pnlSidebar);

            // logo area
            var logo = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.Transparent };
            logo.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var br = new SolidBrush(Accent);
                g.FillEllipse(br, 18, 20, 36, 36);
                using var bw = new SolidBrush(Color.White);
                using var fnt = new Font("Segoe UI", 13f, FontStyle.Bold);
                g.DrawString("S", fnt, bw, 26, 24);
                using var ft = new Font("Segoe UI", 11f, FontStyle.Bold);
                g.DrawString("ScuolaCRM", ft, bw, 62, 22);
                using var ft2 = new Font("Segoe UI", 7.5f);
                using var br2 = new SolidBrush(Color.FromArgb(160, 200, 255));
                g.DrawString("Gestione Scolastica", ft2, br2, 63, 42);
            };
            pnlSidebar.Controls.Add(logo);

            // separator
            var sep = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(50, 255, 255, 255) };
            pnlSidebar.Controls.Add(sep);

            // menu items
            string[][] items =
            {
                new[] { "🏠",  "Dashboard",      "home"     },
                new[] { "🎓",  "Studenti",        "students" },
                new[] { "👩‍🏫",  "Docenti",         "teachers" },
                new[] { "📚",  "Classi",          "classes"  },
                new[] { "📅",  "Calendario",      "calendar" },
                new[] { "📊",  "Valutazioni",     "grades"   },
                new[] { "💬",  "Comunicazioni",   "msgs"     },
                new[] { "⚙️",  "Impostazioni",    "settings" },
            };

            // spacer
            var spacer = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent };
            pnlSidebar.Controls.Add(spacer);

            foreach (var it in items)
            {
                var item  = CreateSidebarItem(it[0], it[1], it[2]);
                pnlSidebar.Controls.Add(item);
                if (it[2] == "home") { SetActiveItem(item); }
            }

            // bottom user card
            var userCard = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 70,
                BackColor = Color.FromArgb(12, 30, 65),
            };
            userCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                // avatar circle
                using var br = new SolidBrush(Accent);
                g.FillEllipse(br, 14, 16, 38, 38);
                using var fw = new Font("Segoe UI", 13f, FontStyle.Bold);
                using var bw = new SolidBrush(Color.White);
                g.DrawString("DS", fw, bw, 16, 20);
                // text
                using var fn = new Font("Segoe UI", 9f, FontStyle.Bold);
                g.DrawString("Prof. Di Stefano", fn, bw, 60, 18);
                using var fn2 = new Font("Segoe UI", 7.5f);
                using var br2 = new SolidBrush(Color.FromArgb(160, 200, 255));
                g.DrawString("Amministratore", fn2, br2, 60, 36);
                // dot verde
                using var dg = new SolidBrush(Success);
                g.FillEllipse(dg, 44, 46, 10, 10);
            };
            pnlSidebar.Controls.Add(userCard);
        }

        private Panel CreateSidebarItem(string icon, string label, string key)
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 46,
                BackColor = Color.Transparent,
                Tag       = key,
                Cursor    = Cursors.Hand,
            };

            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool active = (activeSideItem == pnl);

                if (active)
                {
                    using var br = new SolidBrush(SideHover);
                    g.FillRectangle(br, 0, 0, pnl.Width, pnl.Height);
                    using var bar = new SolidBrush(Accent);
                    g.FillRectangle(bar, 0, 6, 4, pnl.Height - 12);
                }

                using var fi = new Font("Segoe UI Emoji", 13f);
                using var fl = new Font("Segoe UI",       9.5f, active ? FontStyle.Bold : FontStyle.Regular);
                using var bc = new SolidBrush(active ? Color.White : Color.FromArgb(200, 220, 255));

                g.DrawString(icon,  fi, bc, 16, 12);
                g.DrawString(label, fl, bc, 48, 14);
            };

            pnl.MouseEnter += (s, e) => { if (activeSideItem != pnl) { pnl.BackColor = SideHover; } };
            pnl.MouseLeave += (s, e) => { if (activeSideItem != pnl) { pnl.BackColor = Color.Transparent; } };
            pnl.MouseClick += (s, e) =>
            {
                SetActiveItem(pnl);
                HandleNavigation((string)pnl.Tag);
            };

            return pnl;
        }

        private void SetActiveItem(Panel item)
        {
            activeSideItem = item;
            pnlSidebar?.Invalidate(true);
        }

        // ─────────────────────────────────────────────────────────
        //  HEADER  (64 px)
        // ─────────────────────────────────────────────────────────
        private void BuildHeader()
        {
            pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 64,
                BackColor = CardBg,
            };
            pnlHeader.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using var pen = new Pen(Color.FromArgb(220, 228, 245), 1);
                g.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };
            Controls.Add(pnlHeader);

            // title label
            var lbl = new Label
            {
                Text      = "Dashboard",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize  = true,
                Location  = new Point(24, 18),
            };
            pnlHeader.Controls.Add(lbl);

            // search box
            var search = new Panel
            {
                Width     = 240,
                Height    = 36,
                BackColor = BgLight,
                Location  = new Point(pnlHeader.Width - 540, 14),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
            };
            RoundCorners(search, 18);
            search.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, search.Width - 1, search.Height - 1), 18);
                using var br   = new SolidBrush(BgLight);
                g.FillPath(br, path);
                using var pen  = new Pen(Color.FromArgb(210, 220, 240));
                g.DrawPath(pen, path);
                using var fi = new Font("Segoe UI Emoji", 11f);
                using var bc = new SolidBrush(TextMid);
                g.DrawString("🔍", fi, bc, 8, 7);
                using var ft = new Font("Segoe UI", 9f);
                g.DrawString("Cerca studente, classe…", ft, bc, 34, 10);
            };
            pnlHeader.Controls.Add(search);

            // bell icon
            var bell = CreateHeaderIcon("🔔", pnlHeader.Width - 260, 14, "3");
            pnlHeader.Controls.Add(bell);

            // mail icon
            var mail = CreateHeaderIcon("✉️", pnlHeader.Width - 200, 14, "7");
            pnlHeader.Controls.Add(mail);

            // avatar
            var av = new Panel
            {
                Width     = 40,
                Height    = 40,
                Location  = new Point(pnlHeader.Width - 140, 12),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                Cursor    = Cursors.Hand,
            };
            av.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var br = new SolidBrush(Primary);
                g.FillEllipse(br, 0, 0, 40, 40);
                using var fw = new Font("Segoe UI", 11f, FontStyle.Bold);
                using var bw = new SolidBrush(Color.White);
                g.DrawString("DS", fw, bw, 4, 10);
            };
            pnlHeader.Controls.Add(av);

            pnlHeader.Resize += (s, e) =>
            {
                search.Location = new Point(pnlHeader.Width - 540, 14);
                bell.Location   = new Point(pnlHeader.Width - 260, 14);
                mail.Location   = new Point(pnlHeader.Width - 200, 14);
                av.Location     = new Point(pnlHeader.Width - 140, 12);
            };
        }

        private Panel CreateHeaderIcon(string icon, int x, int y, string badge)
        {
            var pnl = new Panel
            {
                Width     = 40,
                Height    = 40,
                Location  = new Point(x, y),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                Cursor    = Cursors.Hand,
                BackColor = Color.Transparent,
            };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, 39, 39), 10);
                using var br   = new SolidBrush(BgLight);
                g.FillPath(br, path);
                using var fi = new Font("Segoe UI Emoji", 14f);
                using var bc = new SolidBrush(TextDark);
                g.DrawString(icon, fi, bc, 5, 5);
                // badge
                if (!string.IsNullOrEmpty(badge))
                {
                    using var br2 = new SolidBrush(Danger);
                    g.FillEllipse(br2, 24, 2, 16, 16);
                    using var fw2 = new Font("Segoe UI", 7f, FontStyle.Bold);
                    using var bw2 = new SolidBrush(Color.White);
                    g.DrawString(badge, fw2, bw2, 27, 5);
                }
            };
            return pnl;
        }

        // ─────────────────────────────────────────────────────────
        //  CONTENT  (scrollable)
        // ─────────────────────────────────────────────────────────
        private void BuildDashboard()
        {
            pnlContent?.Dispose();
            pnlContent = new Panel
            {
                Dock            = DockStyle.Fill,
                BackColor       = BgLight,
                AutoScroll      = true,
                Padding         = new Padding(24, 20, 24, 24),
            };
            Controls.Add(pnlContent);
            pnlContent.BringToFront();
            pnlHeader?.BringToFront();

            int y = 0;

            // ── Benvenuto banner ────────────────────────────────
            var banner = new Panel
            {
                Location  = new Point(0, y),
                Height    = 110,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            banner.Paint += DrawBanner;
            pnlContent.Controls.Add(banner);
            pnlContent.Resize += (s, e) => banner.Width = pnlContent.ClientSize.Width - 48;
            banner.Width = pnlContent.ClientSize.Width - 48;
            y += 126;

            // ── KPI cards ───────────────────────────────────────
            var kpiData = new[]
            {
                ("👨‍🎓", "Studenti Totali",  "1.248",  "+12 questo mese",  Accent),
                ("👩‍🏫", "Docenti",          "84",     "8 materie",        Success),
                ("🏫",  "Classi Attive",    "46",     "3 nuove classi",   Warning),
                ("📝",  "Verifiche Oggi",   "18",     "6 in corso ora",   Danger),
            };

            int kpiW = (pnlContent.ClientSize.Width - 48 - 3 * 16) / 4;
            for (int i = 0; i < kpiData.Length; i++)
            {
                var (ico, title, value, sub, col) = kpiData[i];
                var card = CreateKpiCard(ico, title, value, sub, col);
                card.Location = new Point(i * (kpiW + 16), y);
                card.Width    = kpiW;
                int idx = i;
                pnlContent.Controls.Add(card);
                pnlContent.Resize += (s, e) =>
                {
                    int w = (pnlContent.ClientSize.Width - 48 - 3 * 16) / 4;
                    card.Location = new Point(idx * (w + 16), y);
                    card.Width    = w;
                };
            }
            y += 116;

            // ── sezione titolo "Attività recenti" ───────────────
            y += 20;
            var secLabel = new Label
            {
                Text      = "Attività recenti",
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = TextDark,
                Location  = new Point(0, y),
                AutoSize  = true,
            };
            pnlContent.Controls.Add(secLabel);
            y += 30;

            // ── attività table card ─────────────────────────────
            var tableCard = CreateTableCard(y);
            pnlContent.Controls.Add(tableCard);
            pnlContent.Resize += (s, e) => tableCard.Width = pnlContent.ClientSize.Width - 48;
            tableCard.Width = pnlContent.ClientSize.Width - 48;
            y += tableCard.Height + 20;

            // ── row: grafico + eventi ────────────────────────────
            var chartCard  = CreateChartCard(y);
            var eventsCard = CreateEventsCard(y);

            pnlContent.Controls.Add(chartCard);
            pnlContent.Controls.Add(eventsCard);

            pnlContent.Resize += (s, e) =>
            {
                int total = pnlContent.ClientSize.Width - 48;
                chartCard.Width  = (int)(total * 0.62);
                eventsCard.Width = total - chartCard.Width - 16;
                eventsCard.Location = new Point(chartCard.Width + 16, y);
            };
            int totalW = pnlContent.ClientSize.Width - 48;
            chartCard.Width  = (int)(totalW * 0.62);
            eventsCard.Width = totalW - chartCard.Width - 16;
            eventsCard.Location = new Point(chartCard.Width + 16, y);
        }

        // ── Banner benvenuto ─────────────────────────────────────
        private void DrawBanner(object s, PaintEventArgs e)
        {
            var g   = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            var pnl = (Panel)s;
            var r   = new Rectangle(0, 0, pnl.Width, pnl.Height);

            using var br = new LinearGradientBrush(r, Primary, Color.FromArgb(40, 90, 180), LinearGradientMode.Horizontal);
            using var path = RoundedRect(r, 16);
            g.FillPath(br, path);

            // decorative circles
            using var bc = new SolidBrush(Color.FromArgb(20, 255, 255, 255));
            g.FillEllipse(bc, pnl.Width - 140, -40, 180, 180);
            g.FillEllipse(bc, pnl.Width - 80,   20, 100, 100);

            using var bw = new SolidBrush(Color.White);
            using var ft = new Font("Segoe UI", 16f, FontStyle.Bold);
            g.DrawString("Buongiorno, Prof. Di Stefano! 👋", ft, bw, 24, 18);
            using var ft2 = new Font("Segoe UI", 10f);
            using var bc2 = new SolidBrush(Color.FromArgb(200, 235, 255));
            g.DrawString($"Oggi è {DateTime.Now:dddd d MMMM yyyy}  ·  Anno Scolastico 2024/2025", ft2, bc2, 26, 50);

            using var chip = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
            using var chipPath = RoundedRect(new Rectangle(24, 75, 190, 24), 12);
            g.FillPath(chip, chipPath);
            using var ft3 = new Font("Segoe UI", 8.5f);
            g.DrawString("📋  3 verifiche programmate oggi", ft3, bw, 32, 79);
        }

        // ── KPI card ─────────────────────────────────────────────
        private Panel CreateKpiCard(string icon, string title, string value, string sub, Color accent)
        {
            var card = new Panel { Height = 96, BackColor = CardBg, Cursor = Cursors.Hand };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br = new SolidBrush(CardBg);
                g.FillPath(br, path);
                // shadow-like border
                using var pen = new Pen(Color.FromArgb(225, 232, 248));
                g.DrawPath(pen, path);
                // accent bar top
                using var barPath = RoundedRect(new Rectangle(0, 0, card.Width, 5), 4);
                using var barBr   = new SolidBrush(accent);
                g.FillPath(barBr, barPath);
                // icon circle
                using var ic = new SolidBrush(Color.FromArgb(30, accent.R, accent.G, accent.B));
                g.FillEllipse(ic, 16, 16, 46, 46);
                using var fi = new Font("Segoe UI Emoji", 16f);
                g.DrawString(icon, fi, new SolidBrush(accent), 17, 17);
                // value
                using var fv = new Font("Segoe UI", 18f, FontStyle.Bold);
                using var bv = new SolidBrush(TextDark);
                g.DrawString(value, fv, bv, 70, 14);
                // title
                using var ft = new Font("Segoe UI", 8.5f);
                using var bt = new SolidBrush(TextMid);
                g.DrawString(title, ft, bt, 70, 42);
                // sub
                using var fs = new Font("Segoe UI", 7.5f);
                using var bs = new SolidBrush(accent);
                g.DrawString("↑ " + sub, fs, bs, 70, 62);
            };
            AddHoverEffect(card);
            return card;
        }

        // ── Table card attività ──────────────────────────────────
        private Panel CreateTableCard(int y)
        {
            var rows = new[]
            {
                ("Marco Rossi",       "3A",  "Matematica",  "Assente",    Danger),
                ("Giulia Ferrari",    "2B",  "Italiano",    "Presente",   Success),
                ("Luca Bianchi",      "5C",  "Fisica",      "Ritardo",    Warning),
                ("Alessia Ricci",     "1A",  "Inglese",     "Presente",   Success),
                ("Davide Conti",      "4B",  "Storia",      "Assente",    Danger),
            };

            int rowH  = 46;
            int total = 50 + rows.Length * rowH;
            var card  = new Panel { Location = new Point(0, y), Height = total, BackColor = CardBg };

            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen = new Pen(Color.FromArgb(225, 232, 248));
                g.DrawPath(pen, path);

                // header row
                using var fh = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bh = new SolidBrush(TextMid);
                string[] cols = { "STUDENTE", "CLASSE", "MATERIA", "STATO" };
                int[] xs = { 16, 220, 360, 520 };
                for (int i = 0; i < cols.Length; i++)
                    g.DrawString(cols[i], fh, bh, xs[i], 16);

                using var sep = new Pen(Color.FromArgb(235, 240, 252));
                g.DrawLine(sep, 12, 38, card.Width - 12, 38);

                for (int i = 0; i < rows.Length; i++)
                {
                    var (name, cls, subject, status, col) = rows[i];
                    int ry = 50 + i * rowH;

                    if (i % 2 == 0)
                    {
                        using var stripe = new SolidBrush(Color.FromArgb(248, 250, 255));
                        g.FillRectangle(stripe, 4, ry, card.Width - 8, rowH);
                    }

                    // avatar
                    using var av = new SolidBrush(Color.FromArgb(30, Accent.R, Accent.G, Accent.B));
                    g.FillEllipse(av, xs[0], ry + 8, 28, 28);
                    using var fi = new Font("Segoe UI", 9f, FontStyle.Bold);
                    using var bw = new SolidBrush(Primary);
                    g.DrawString(name[0].ToString(), fi, bw, xs[0] + 8, ry + 12);

                    using var fn = new Font("Segoe UI", 9.5f);
                    using var bd = new SolidBrush(TextDark);
                    g.DrawString(name, fn, bd, xs[0] + 34, ry + 14);
                    g.DrawString(cls, fn, bd, xs[1], ry + 14);
                    g.DrawString(subject, fn, bd, xs[2], ry + 14);

                    // badge stato
                    var badgeR = new Rectangle(xs[3], ry + 11, 76, 22);
                    using var badgePath = RoundedRect(badgeR, 11);
                    using var badgeBr   = new SolidBrush(Color.FromArgb(35, col.R, col.G, col.B));
                    g.FillPath(badgeBr, badgePath);
                    using var fs = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                    using var bc = new SolidBrush(col);
                    g.DrawString(status, fs, bc, xs[3] + 8, ry + 14);

                    // separator
                    if (i < rows.Length - 1)
                        g.DrawLine(sep, 12, ry + rowH, card.Width - 12, ry + rowH);
                }
            };

            return card;
        }

        // ── Chart card (bar chart) ───────────────────────────────
        private Panel CreateChartCard(int y)
        {
            var card = new Panel
            {
                Location  = new Point(0, y),
                Height    = 260,
                BackColor = CardBg,
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen = new Pen(Color.FromArgb(225, 232, 248));
                g.DrawPath(pen, path);

                using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Presenze settimanali", ft, bd, 20, 16);
                using var fs = new Font("Segoe UI", 8f);
                using var bm = new SolidBrush(TextMid);
                g.DrawString("Questa settimana vs settimana scorsa", fs, bm, 22, 36);

                // chart area
                int cx = 40, cy = 60, cw = card.Width - 60, ch = 160;
                float[] curr = { 0.87f, 0.92f, 0.78f, 0.95f, 0.88f };
                float[] prev = { 0.80f, 0.85f, 0.83f, 0.88f, 0.80f };
                string[] days = { "Lun", "Mar", "Mer", "Gio", "Ven" };

                int barW   = cw / (curr.Length * 2 + 1);
                int groupW = barW * 2 + 6;
                int startX = cx + (cw - curr.Length * groupW) / 2;

                // gridlines
                for (int gi = 0; gi <= 4; gi++)
                {
                    int gy = cy + ch - (int)(ch * gi / 4f);
                    using var gpen = new Pen(Color.FromArgb(235, 238, 248));
                    g.DrawLine(gpen, cx, gy, cx + cw, gy);
                    using var fg = new Font("Segoe UI", 7f);
                    g.DrawString($"{gi * 25}%", fg, bm, cx - 34, gy - 7);
                }

                for (int i = 0; i < curr.Length; i++)
                {
                    int bx = startX + i * (groupW + 10);
                    int h1 = (int)(ch * curr[i]);
                    int h2 = (int)(ch * prev[i]);

                    using var b1 = new SolidBrush(Accent);
                    g.FillRectangle(b1, bx,          cy + ch - h1, barW, h1);
                    using var b2 = new SolidBrush(Color.FromArgb(200, 210, 230));
                    g.FillRectangle(b2, bx + barW + 4, cy + ch - h2, barW, h2);

                    using var fd = new Font("Segoe UI", 7.5f);
                    g.DrawString(days[i], fd, bm, bx + 2, cy + ch + 6);
                }

                // legend
                using var fleg = new Font("Segoe UI", 8f);
                using var bl1 = new SolidBrush(Accent);
                using var bl2 = new SolidBrush(Color.FromArgb(200, 210, 230));
                g.FillRectangle(bl1, cx + cw - 180, 18, 12, 12);
                g.DrawString("Questa settimana", fleg, bm, cx + cw - 164, 16);
                g.FillRectangle(bl2, cx + cw -  70, 18, 12, 12);
                g.DrawString("Scorsa", fleg, bm, cx + cw - 54, 16);
            };
            return card;
        }

        // ── Events card ──────────────────────────────────────────
        private Panel CreateEventsCard(int y)
        {
            var events = new[]
            {
                ("09:00", "Consiglio di Classe 3A",  Accent),
                ("11:30", "Riunione con i Genitori",  Warning),
                ("14:00", "Verifica di Fisica 5C",    Danger),
                ("16:30", "Colloquio individuale",    Success),
            };

            var card = new Panel
            {
                Location  = new Point(0, y),
                Height    = 260,
                BackColor = CardBg,
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen = new Pen(Color.FromArgb(225, 232, 248));
                g.DrawPath(pen, path);

                using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Agenda di Oggi", ft, bd, 16, 16);

                for (int i = 0; i < events.Length; i++)
                {
                    var (time, title, col) = events[i];
                    int ey = 50 + i * 52;

                    using var lineBr = new SolidBrush(col);
                    g.FillRectangle(lineBr, 16, ey, 4, 38);

                    using var flt = new Font("Segoe UI", 8f, FontStyle.Bold);
                    using var blt = new SolidBrush(col);
                    g.DrawString(time, flt, blt, 26, ey + 2);

                    using var ftt = new Font("Segoe UI", 9f);
                    using var btt = new SolidBrush(TextDark);
                    g.DrawString(title, ftt, btt, 26, ey + 18);

                    if (i < events.Length - 1)
                    {
                        using var sep = new Pen(Color.FromArgb(235, 240, 252));
                        g.DrawLine(sep, 16, ey + 46, card.Width - 16, ey + 46);
                    }
                }
            };
            return card;
        }

        // ─────────────────────────────────────────────────────────
        //  NAVIGATION
        // ─────────────────────────────────────────────────────────
        private void HandleNavigation(string key)
        {
            // Aggiorna titolo header
            var headerLbl = pnlHeader?.Controls.OfType<Label>().FirstOrDefault();
            if (headerLbl != null)
            {
                headerLbl.Text = key switch
                {
                    "home"     => "Dashboard",
                    "students" => "Studenti",
                    "teachers" => "Docenti",
                    "classes"  => "Classi",
                    "calendar" => "Calendario",
                    "grades"   => "Valutazioni",
                    "msgs"     => "Comunicazioni",
                    "settings" => "Impostazioni",
                    _          => "Dashboard",
                };
            }

            // Rimuove il pannello corrente
            pnlContent?.Dispose();

            switch (key)
            {
                case "home":
                    BuildDashboard();
                    break;
                case "students":
                    pnlContent = new StudentiForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "teachers":
                    pnlContent = new DocentiForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "classes":
                    pnlContent = new ClassiForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "calendar":
                    pnlContent = new CalendarioForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "grades":
                    pnlContent = new ValutazioniForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "msgs":
                    pnlContent = new ComunicazioniForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                case "settings":
                    pnlContent = new ImpostazioniForm();
                    Controls.Add(pnlContent);
                    pnlContent.BringToFront();
                    pnlHeader?.BringToFront();
                    break;
                default:
                    BuildDashboard();
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────────────────
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

        private static void RoundCorners(Control c, int r) =>
            c.Region = new Region(RoundedRect(new Rectangle(0, 0, c.Width, c.Height), r));

        private static void AddHoverEffect(Panel card)
        {
            card.MouseEnter += (s, e) => card.Invalidate();
            card.MouseLeave += (s, e) => card.Invalidate();
        }
    }
}
