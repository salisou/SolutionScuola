using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class ImpostazioniForm : Panel
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

        private Panel pnlContent;
        private int   activeSection = 0;

        private readonly string[] sections =
        {
            "👤 Profilo",
            "🔔 Notifiche",
            "🎨 Aspetto",
            "🔒 Sicurezza",
            "🏫 Scuola",
            "📤 Esporta Dati",
        };

        public ImpostazioniForm()
        {
            Dock = DockStyle.Fill;
            BackColor = BgLight;
            Padding = new Padding(24, 20, 24, 24);
            Build();
        }

        private void Build()
        {
            // ── Two-col layout ───────────────────────────────────
            var sideNav = new Panel
            {
                Location  = new Point(0, 0),
                Width     = 220,
                BackColor = CardBg,
                Anchor    = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            };
            sideNav.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, sideNav.Width - 1, sideNav.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);
            };
            Resize += (s, e) => sideNav.Height = ClientSize.Height - 48;
            sideNav.Height = ClientSize.Height - 48;

            // section header
            var sideTitle = new Label
            {
                Text = "Impostazioni", AutoSize = false, Height = 44, Width = 220,
                Location = new Point(0, 0), TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = TextDark,
                Padding = new Padding(16, 0, 0, 0),
            };
            sideNav.Controls.Add(sideTitle);

            for (int i = 0; i < sections.Length; i++)
            {
                var item = BuildNavItem(i);
                item.Location = new Point(0, 48 + i * 44);
                sideNav.Controls.Add(item);
            }
            Controls.Add(sideNav);

            // ── Content panel ────────────────────────────────────
            pnlContent = new Panel
            {
                Location  = new Point(236, 0),
                BackColor = Color.Transparent,
                AutoScroll = true,
                Anchor    = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            };
            Resize += (s, e) =>
            {
                pnlContent.Width  = ClientSize.Width - 48 - 236;
                pnlContent.Height = ClientSize.Height - 48;
            };
            pnlContent.Width  = ClientSize.Width - 48 - 236;
            pnlContent.Height = ClientSize.Height - 48;
            Controls.Add(pnlContent);

            ShowSection(0);
        }

        private Panel BuildNavItem(int i)
        {
            var pnl = new Panel { Size = new Size(220, 42), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                bool active = activeSection == i;
                if (active)
                {
                    using var hl  = new SolidBrush(Color.FromArgb(235, 245, 255));
                    g.FillRectangle(hl, 8, 4, pnl.Width - 16, pnl.Height - 8);
                    using var bar = new SolidBrush(Accent);
                    g.FillRectangle(bar, 8, 4, 4, pnl.Height - 8);
                }
                using var ft = new Font("Segoe UI", 10f, active ? FontStyle.Bold : FontStyle.Regular);
                using var bt = new SolidBrush(active ? Primary : TextMid);
                g.DrawString(sections[i], ft, bt, 22, 11);
            };
            pnl.MouseEnter += (s, e) => { if (activeSection != i) { pnl.BackColor = Color.FromArgb(248, 250, 255); pnl.Invalidate(); } };
            pnl.MouseLeave += (s, e) => { if (activeSection != i) { pnl.BackColor = Color.Transparent; pnl.Invalidate(); } };
            pnl.Click += (s, e) => { activeSection = i; ShowSection(i); RefreshNavItems(pnl.Parent); };
            return pnl;
        }

        private void RefreshNavItems(Control parent)
        {
            foreach (Control c in parent.Controls)
                if (c is Panel p && p.Height == 42) p.Invalidate();
        }

        private void ShowSection(int idx)
        {
            pnlContent.Controls.Clear();
            switch (idx)
            {
                case 0: BuildProfiloSection(); break;
                case 1: BuildNotificheSection(); break;
                case 2: BuildAspettoSection(); break;
                case 3: BuildSicurezzaSection(); break;
                case 4: BuildScuolaSection(); break;
                case 5: BuildEsportaSection(); break;
            }
        }

        // ── PROFILO ──────────────────────────────────────────────
        private void BuildProfiloSection()
        {
            int y = 0;
            AddSectionTitle("Profilo Utente", "Gestisci le tue informazioni personali", ref y);

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg };
            card.Width = pnlContent.ClientSize.Width;
            pnlContent.Resize += (s, e) => card.Width = pnlContent.ClientSize.Width;

            // avatar area
            card.Height = 340;
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                // avatar
                using var avBr = new SolidBrush(Primary);
                g.FillEllipse(avBr, 24, 20, 80, 80);
                using var fw = new Font("Segoe UI", 22f, FontStyle.Bold);
                using var bw = new SolidBrush(Color.White);
                g.DrawString("DS", fw, bw, 30, 34);

                // online dot
                using var dotBr = new SolidBrush(Success);
                g.FillEllipse(dotBr, 86, 84, 16, 16);

                using var fn = new Font("Segoe UI", 13f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Prof. Di Stefano", fn, bd, 118, 28);
                using var fr = new Font("Segoe UI", 9.5f);
                using var bm = new SolidBrush(TextMid);
                g.DrawString("Amministratore · Anno scolastico 2024/25", fr, bm, 120, 52);
                // badge
                using var badgePath = RoundedRect(new Rectangle(120, 70, 100, 22), 11);
                using var badgeBr   = new SolidBrush(Color.FromArgb(30, Accent.R, Accent.G, Accent.B));
                g.FillPath(badgeBr, badgePath);
                using var fb = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bb = new SolidBrush(Accent);
                g.DrawString("✔ Account Verificato", fb, bb, 128, 73);

                // divider
                using var sep = new Pen(Border);
                g.DrawLine(sep, 16, 112, card.Width - 16, 112);
            };

            // Form fields
            string[][] fields = {
                new[] { "Nome",          "Antonio" },
                new[] { "Cognome",       "Di Stefano" },
                new[] { "Email",         "a.distefano@scuola.it" },
                new[] { "Telefono",      "+39 333 123 4567" },
                new[] { "Ruolo",         "Docente / Amministratore" },
            };
            int fy = 126;
            foreach (var f in fields)
            {
                AddFormField(card, f[0], f[1], ref fy);
            }

            var btnSave = CreateButton("💾  Salva Modifiche", 16, card.Height - 52, Success);
            card.Controls.Add(btnSave);
            pnlContent.Controls.Add(card);
        }

        // ── NOTIFICHE ─────────────────────────────────────────────
        private void BuildNotificheSection()
        {
            int y = 0;
            AddSectionTitle("Notifiche", "Configura quando e come vuoi ricevere gli avvisi", ref y);

            (string title, string desc, bool on, Color col)[] toggles =
            {
                ("Assenze studenti",    "Avviso immediato quando uno studente è assente",  true,  Accent),
                ("Nuovi messaggi",      "Notifica per ogni nuovo messaggio ricevuto",       true,  Success),
                ("Verifiche in arrivo", "Promemoria 24h prima di una verifica",             true,  Warning),
                ("Riunioni e eventi",   "Avviso per eventi nel calendario",                 false, Primary),
                ("Report settimanale",  "Riepilogo automatico ogni lunedì mattina",         true,  Accent),
                ("Allarmi voti bassi",  "Notifica quando uno studente scende sotto il 6",   false, Danger),
            };

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg };
            card.Width  = pnlContent.ClientSize.Width;
            card.Height = toggles.Length * 68 + 24;
            pnlContent.Resize += (s, e) => { card.Width = pnlContent.ClientSize.Width; card.Invalidate(); };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                for (int i = 0; i < toggles.Length; i++)
                {
                    var (title, desc, on, col) = toggles[i];
                    int ty = 16 + i * 68;
                    // icon circle
                    using var icBr = new SolidBrush(Color.FromArgb(25, col.R, col.G, col.B));
                    g.FillEllipse(icBr, 16, ty + 10, 38, 38);
                    // toggle indicator circle
                    Color tColor = on ? col : TextMid;
                    using var dotBr = new SolidBrush(tColor);
                    g.FillEllipse(dotBr, 22, ty + 16, 26, 26);

                    using var fn = new Font("Segoe UI", 10f, FontStyle.Bold);
                    using var bd = new SolidBrush(TextDark);
                    g.DrawString(title, fn, bd, 66, ty + 12);
                    using var fd = new Font("Segoe UI", 8.5f);
                    using var bm = new SolidBrush(TextMid);
                    g.DrawString(desc, fd, bm, 66, ty + 30);

                    // toggle switch
                    int tx  = card.Width - 76;
                    Color  bgCol = on ? Color.FromArgb(30, col.R, col.G, col.B) : Color.FromArgb(235, 240, 252);
                    using var tgBr  = new SolidBrush(bgCol);
                    using var tgPath = RoundedRect(new Rectangle(tx, ty + 18, 52, 24), 12);
                    g.FillPath(tgBr, tgPath);
                    using var knobBr = new SolidBrush(on ? col : Color.FromArgb(200, 210, 230));
                    int kx = on ? tx + 28 : tx + 4;
                    g.FillEllipse(knobBr, kx, ty + 22, 16, 16);

                    if (i < toggles.Length - 1)
                    {
                        using var sep = new Pen(Color.FromArgb(235, 240, 252));
                        g.DrawLine(sep, 16, ty + 60, card.Width - 16, ty + 60);
                    }
                }
            };
            pnlContent.Controls.Add(card);
        }

        // ── ASPETTO ──────────────────────────────────────────────
        private void BuildAspettoSection()
        {
            int y = 0;
            AddSectionTitle("Aspetto", "Personalizza il tema e il layout dell'applicazione", ref y);

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg, Height = 280 };
            card.Width = pnlContent.ClientSize.Width;
            pnlContent.Resize += (s, e) => { card.Width = pnlContent.ClientSize.Width; card.Invalidate(); };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Tema Colore", ft, bd, 16, 16);

                // color swatches
                Color[] swatches = { Color.FromArgb(25,55,109), Color.FromArgb(20,100,80), Color.FromArgb(120,40,160),
                                     Color.FromArgb(180,50,50), Color.FromArgb(180,110,20) };
                for (int i = 0; i < swatches.Length; i++)
                {
                    using var swBr = new SolidBrush(swatches[i]);
                    g.FillEllipse(swBr, 16 + i * 48, 44, 36, 36);
                    if (i == 0)
                    {
                        using var selPen = new Pen(swatches[i], 3f);
                        g.DrawEllipse(selPen, 12 + i * 48, 40, 44, 44);
                        using var checkBr = new SolidBrush(Color.White);
                        using var checkF  = new Font("Segoe UI", 10f, FontStyle.Bold);
                        g.DrawString("✓", checkF, checkBr, 21, 50);
                    }
                }

                // font size
                using var ft2 = new Font("Segoe UI", 10f, FontStyle.Bold);
                g.DrawString("Dimensione Testo", ft2, bd, 16, 102);
                string[] sizes = { "Piccolo", "Normale", "Grande" };
                for (int i = 0; i < sizes.Length; i++)
                {
                    int sx  = 16 + i * 110;
                    bool sel = i == 1;
                    using var sBr  = new SolidBrush(sel ? Color.FromArgb(235,245,255) : BgLight);
                    using var sPth = RoundedRect(new Rectangle(sx, 126, 100, 34), 8);
                    g.FillPath(sBr, sPth);
                    using var sPen = new Pen(sel ? Accent : Border);
                    g.DrawPath(sPen, sPth);
                    using var fs2  = new Font("Segoe UI", 9f, sel ? FontStyle.Bold : FontStyle.Regular);
                    using var bs2  = new SolidBrush(sel ? Primary : TextMid);
                    g.DrawString(sizes[i], fs2, bs2, sx + 22, 134);
                }

                // sidebar style
                g.DrawString("Stile Sidebar", ft2, bd, 16, 178);
                string[] styles = { "Compatta", "Standard", "Espansa" };
                for (int i = 0; i < styles.Length; i++)
                {
                    int sx  = 16 + i * 110;
                    bool sel = i == 1;
                    using var sBr  = new SolidBrush(sel ? Color.FromArgb(235,245,255) : BgLight);
                    using var sPth = RoundedRect(new Rectangle(sx, 202, 100, 34), 8);
                    g.FillPath(sBr, sPth);
                    using var sPen = new Pen(sel ? Accent : Border);
                    g.DrawPath(sPen, sPth);
                    using var fs2  = new Font("Segoe UI", 9f, sel ? FontStyle.Bold : FontStyle.Regular);
                    using var bs2  = new SolidBrush(sel ? Primary : TextMid);
                    g.DrawString(styles[i], fs2, bs2, sx + 22, 210);
                }
            };
            pnlContent.Controls.Add(card);
        }

        // ── SICUREZZA ─────────────────────────────────────────────
        private void BuildSicurezzaSection()
        {
            int y = 0;
            AddSectionTitle("Sicurezza", "Gestisci password, accessi e sessioni attive", ref y);

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg, Height = 320 };
            card.Width = pnlContent.ClientSize.Width;
            pnlContent.Resize += (s, e) => { card.Width = pnlContent.ClientSize.Width; card.Invalidate(); };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                // security score
                using var ft = new Font("Segoe UI", 10f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Sicurezza Account", ft, bd, 16, 16);

                int score = 75;
                using var bgBr = new SolidBrush(Color.FromArgb(235,240,252));
                g.FillRectangle(bgBr, 16, 40, card.Width - 32, 14);
                using var fgBr = new LinearGradientBrush(new Rectangle(16, 40, (int)((card.Width - 32) * score / 100f), 14),
                                                          Warning, Success, 0f);
                g.FillRectangle(fgBr, 16, 40, (int)((card.Width - 32) * score / 100f), 14);
                using var fm = new Font("Segoe UI", 8.5f);
                using var bm = new SolidBrush(TextMid);
                g.DrawString($"Punteggio sicurezza: {score}/100 – Buono", fm, bm, 16, 60);

                // items
                (string icon, string label, string value, bool ok)[] items =
                {
                    ("🔑", "Password",           "Aggiornata 15 giorni fa",        true),
                    ("📱", "Autenticazione 2FA",  "Non configurata",                false),
                    ("💻", "Sessioni attive",     "2 dispositivi connessi",         true),
                    ("📋", "Log accessi",         "Ultimo accesso: oggi 08:30",     true),
                };
                int iy = 88;
                foreach (var (icon, label, value, ok) in items)
                {
                    using var fi = new Font("Segoe UI Emoji", 14f);
                    g.DrawString(icon, fi, bd, 16, iy + 6);
                    using var fn = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    g.DrawString(label, fn, bd, 46, iy + 8);
                    using var fv = new Font("Segoe UI", 8.5f);
                    using var bv = new SolidBrush(ok ? TextMid : Danger);
                    g.DrawString(value, fv, bv, 46, iy + 26);
                    using var statusBr = new SolidBrush(ok ? Success : Danger);
                    g.FillEllipse(statusBr, card.Width - 28, iy + 16, 12, 12);
                    using var sep = new Pen(Color.FromArgb(235,240,252));
                    g.DrawLine(sep, 16, iy + 50, card.Width - 16, iy + 50);
                    iy += 54;
                }
            };
            var btnPwd = CreateButton("🔑  Cambia Password", 16, 260, Warning);
            card.Controls.Add(btnPwd);
            pnlContent.Controls.Add(card);
        }

        // ── SCUOLA ───────────────────────────────────────────────
        private void BuildScuolaSection()
        {
            int y = 0;
            AddSectionTitle("Informazioni Scuola", "Dati dell'istituto scolastico", ref y);

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg, Height = 300 };
            card.Width = pnlContent.ClientSize.Width;
            pnlContent.Resize += (s, e) => { card.Width = pnlContent.ClientSize.Width; card.Invalidate(); };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                // school logo placeholder
                using var logoBr = new LinearGradientBrush(new Rectangle(16, 16, 60, 60), Primary, Accent, 45f);
                g.FillEllipse(logoBr, 16, 16, 60, 60);
                using var fw = new Font("Segoe UI", 18f, FontStyle.Bold);
                g.DrawString("🏫", fw, new SolidBrush(Color.White), 22, 22);

                using var fn = new Font("Segoe UI", 13f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString("Istituto Comprensivo G. Garibaldi", fn, bd, 90, 20);
                using var fa = new Font("Segoe UI", 9f);
                using var bm = new SolidBrush(TextMid);
                g.DrawString("Via Roma 12, Bologna (BO) 40121", fa, bm, 90, 44);
                g.DrawString("Cod. Meccanografico: BOIC884001", fa, bm, 90, 62);
            };

            int fy = 92;
            string[][] schoolFields = {
                new[] { "Nome Istituto",        "I.C. G. Garibaldi" },
                new[] { "Anno Scolastico",       "2024/2025" },
                new[] { "Dirigente Scolastico",  "Dott.ssa Giovanna Manzoni" },
                new[] { "Email Istituto",        "info@icgaribaldiBO.edu.it" },
                new[] { "Sito Web",              "www.icgaribaldiBO.edu.it" },
            };
            foreach (var f in schoolFields) AddFormField(card, f[0], f[1], ref fy);
            card.Height = fy + 20;
            pnlContent.Controls.Add(card);
        }

        // ── ESPORTA ──────────────────────────────────────────────
        private void BuildEsportaSection()
        {
            int y = 0;
            AddSectionTitle("Esporta Dati", "Scarica i dati del sistema in vari formati", ref y);

            var card = new Panel { Location = new Point(0, y), BackColor = CardBg, Height = 280 };
            card.Width = pnlContent.ClientSize.Width;
            pnlContent.Resize += (s, e) => { card.Width = pnlContent.ClientSize.Width; card.Invalidate(); };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg); g.FillPath(br, path);
                using var pen  = new Pen(Border); g.DrawPath(pen, path);

                (string icon, string title, string desc, Color col)[] exports =
                {
                    ("📊", "Registro Voti",    "Excel con tutti i voti dell'anno",         Success),
                    ("👥", "Lista Studenti",   "CSV con dati anagrafici degli studenti",    Accent),
                    ("📅", "Calendario",       "iCal con tutti gli eventi del calendario",  Warning),
                    ("📋", "Report Presenze",  "PDF mensile delle presenze per classe",     Danger),
                };

                for (int i = 0; i < exports.Length; i++)
                {
                    var (icon, title, desc, col) = exports[i];
                    int ey = 16 + i * 62;
                    using var icBr = new SolidBrush(Color.FromArgb(25, col.R, col.G, col.B));
                    g.FillEllipse(icBr, 16, ey + 10, 40, 40);
                    using var fi = new Font("Segoe UI Emoji", 16f);
                    g.DrawString(icon, fi, new SolidBrush(col), 18, ey + 13);
                    using var fn = new Font("Segoe UI", 10f, FontStyle.Bold);
                    using var bd = new SolidBrush(TextDark);
                    g.DrawString(title, fn, bd, 66, ey + 12);
                    using var fd = new Font("Segoe UI", 8.5f);
                    using var bm = new SolidBrush(TextMid);
                    g.DrawString(desc, fd, bm, 66, ey + 30);
                    using var sep = new Pen(Color.FromArgb(235, 240, 252));
                    g.DrawLine(sep, 16, ey + 56, card.Width - 16, ey + 56);
                }
            };

            // export buttons
            (string lbl, int x, Color col)[] btns =
            {
                ("📊 Esporta Voti",    16,  Success),
                ("👥 Esporta Studenti", 190, Accent),
                ("📅 Esporta Cal.",    364,  Warning),
                ("📋 Esporta Presenze",538,  Danger),
            };
            foreach (var (lbl, bx, col) in btns)
            {
                var btn = CreateButton(lbl, bx, 234, col);
                btn.Width = 164;
                card.Controls.Add(btn);
            }
            pnlContent.Controls.Add(card);
        }

        // ── HELPERS ──────────────────────────────────────────────
        private void AddSectionTitle(string title, string subtitle, ref int y)
        {
            var lbl = new Label
            {
                Text = title, AutoSize = true,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = TextDark, Location = new Point(0, y),
            };
            var sub = new Label
            {
                Text = subtitle, AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = TextMid, Location = new Point(0, y + 26),
            };
            pnlContent.Controls.Add(lbl);
            pnlContent.Controls.Add(sub);
            y += 56;
        }

        private void AddFormField(Panel parent, string label, string value, ref int y)
        {
            var lbl = new Label
            {
                Text = label, AutoSize = false, Height = 18, Width = parent.Width - 32,
                Location = new Point(16, y),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = TextMid,
            };
            var txt = new TextBox
            {
                Text = value, Location = new Point(16, y + 20),
                Width = parent.Width - 32, Height = 30,
                BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10f),
                ForeColor = TextDark, BackColor = BgLight,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            y += 62;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(160, 36),
                FlatStyle = FlatStyle.Flat, BackColor = col, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand,
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
