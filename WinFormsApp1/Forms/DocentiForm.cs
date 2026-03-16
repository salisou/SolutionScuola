using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchoolCRM
{
    public class DocentiForm : Panel
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

        private readonly (string nome, string cognome, string materia, string classi, string email, string stato, int ore, Color col)[] docenti =
        {
            ("Laura",    "Esposito",  "Matematica",  "3A, 3B, 5C", "l.esposito@scuola.it",  "Attivo",     18, Color.FromArgb(56,182,255)),
            ("Marco",    "Santoro",   "Italiano",    "1A, 2B, 4A", "m.santoro@scuola.it",   "Attivo",     20, Color.FromArgb(52,199,148)),
            ("Francesca","Colombo",   "Inglese",     "2A, 2B, 3C", "f.colombo@scuola.it",   "Attivo",     16, Color.FromArgb(255,182,57)),
            ("Roberto",  "Moretti",   "Fisica",      "4B, 5A, 5C", "r.moretti@scuola.it",   "Congedo",    0,  Color.FromArgb(255,90,95)),
            ("Silvia",   "Romano",    "Storia",      "1A, 1B, 2A", "s.romano@scuola.it",    "Attivo",     14, Color.FromArgb(180,100,255)),
            ("Giovanni", "Barbieri",  "Scienze",     "3A, 4B, 4C", "g.barbieri@scuola.it",  "Attivo",     18, Color.FromArgb(56,182,255)),
            ("Marta",    "Fumagalli", "Arte",        "1A, 1B, 1C", "m.fumagalli@scuola.it", "Part-time",  10, Color.FromArgb(255,150,80)),
            ("Claudio",  "De Luca",   "Ed. Fisica",  "2A, 3B, 4A", "c.deluca@scuola.it",    "Attivo",     12, Color.FromArgb(52,199,148)),
        };

        public DocentiForm()
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

            // ── Action bar ──────────────────────────────────────
            var topBar = new Panel
            {
                Location  = new Point(0, y),
                Height    = 50,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            topBar.Width = ClientSize.Width - 48;
            Resize += (s, e) => topBar.Width = ClientSize.Width - 48;

            var searchPanel = CreateSearchBox(0, 6, 240);
            topBar.Controls.Add(searchPanel);

            var btnAdd = CreateButton("＋  Nuovo Docente", topBar.Width - 160, 7, Success);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            topBar.Controls.Add(btnAdd);
            Controls.Add(topBar);
            y += 66;

            // ── Cards grid ──────────────────────────────────────
            var grid = new Panel
            {
                Location  = new Point(0, y),
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            };
            grid.Width  = ClientSize.Width - 48;
            grid.Height = (int)Math.Ceiling(docenti.Length / 2.0) * 148;
            Resize += (s, e) => { grid.Width = ClientSize.Width - 48; LayoutCards(grid); };
            Controls.Add(grid);

            for (int i = 0; i < docenti.Length; i++)
            {
                var card = BuildDocCard(docenti[i]);
                card.Tag = i;
                grid.Controls.Add(card);
            }
            LayoutCards(grid);
        }

        private void LayoutCards(Panel grid)
        {
            int gap = 16;
            int cols = grid.Width >= 700 ? 2 : 1;
            int cardW = (grid.Width - gap * (cols - 1)) / cols;
            int cardH = 138;
            for (int i = 0; i < grid.Controls.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                grid.Controls[i].Location = new Point(col * (cardW + gap), row * (cardH + gap));
                grid.Controls[i].Size     = new Size(cardW, cardH);
                grid.Controls[i].Invalidate();
            }
            grid.Height = ((grid.Controls.Count + cols - 1) / cols) * (cardH + gap);
        }

        private Panel BuildDocCard((string nome, string cognome, string materia, string classi, string email, string stato, int ore, Color col) d)
        {
            var card = new Panel { BackColor = CardBg, Cursor = Cursors.Hand };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 14);
                using var br   = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen  = new Pen(Border);
                g.DrawPath(pen, path);
                // left color strip
                using var strip = new SolidBrush(d.col);
                using var sPath = RoundedRect(new Rectangle(0, 0, 6, card.Height), 3);
                g.FillPath(strip, sPath);

                // avatar
                using var avBr = new SolidBrush(Color.FromArgb(30, d.col.R, d.col.G, d.col.B));
                g.FillEllipse(avBr, 18, 24, 52, 52);
                using var fa = new Font("Segoe UI", 15f, FontStyle.Bold);
                using var ba = new SolidBrush(d.col);
                string initials = $"{d.nome[0]}{d.cognome[0]}";
                g.DrawString(initials, fa, ba, 22, 32);

                // name
                using var fn = new Font("Segoe UI", 11f, FontStyle.Bold);
                using var bd = new SolidBrush(TextDark);
                g.DrawString($"{d.nome} {d.cognome}", fn, bd, 80, 20);

                // materia badge
                using var mbPath = RoundedRect(new Rectangle(80, 44, TextRenderer.MeasureText(d.materia, new Font("Segoe UI", 8.5f)).Width + 16, 22), 11);
                using var mbBr   = new SolidBrush(Color.FromArgb(30, d.col.R, d.col.G, d.col.B));
                g.FillPath(mbBr, mbPath);
                using var fm = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                using var bm = new SolidBrush(d.col);
                g.DrawString(d.materia, fm, bm, 88, 47);

                // info row
                using var fi = new Font("Segoe UI", 8.5f);
                using var bi = new SolidBrush(TextMid);
                g.DrawString("📧 " + d.email, fi, bi, 18, 84);
                g.DrawString("📚 " + d.classi, fi, bi, 18, 104);

                // ore circle
                int cx = card.Width - 52, cy2 = 28;
                using var oBr = new SolidBrush(Color.FromArgb(20, d.col.R, d.col.G, d.col.B));
                g.FillEllipse(oBr, cx, cy2, 40, 40);
                using var fo = new Font("Segoe UI", 9f, FontStyle.Bold);
                g.DrawString($"{d.ore}h", fo, bm, cx + (d.ore < 10 ? 12 : 8), cy2 + 12);

                // stato badge
                Color stCol = d.stato == "Attivo" ? Success : d.stato == "Part-time" ? Warning : Danger;
                int sw = TextRenderer.MeasureText(d.stato, new Font("Segoe UI", 8.5f)).Width + 14;
                using var stPath = RoundedRect(new Rectangle(card.Width - sw - 12, card.Height - 32, sw, 20), 10);
                using var stBr   = new SolidBrush(Color.FromArgb(35, stCol.R, stCol.G, stCol.B));
                g.FillPath(stBr, stPath);
                using var fs = new Font("Segoe UI", 8f, FontStyle.Bold);
                using var bs = new SolidBrush(stCol);
                g.DrawString(d.stato, fs, bs, card.Width - sw - 6, card.Height - 29);
            };

            // hover
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(250, 252, 255); card.Invalidate(); };
            card.MouseLeave += (s, e) => { card.BackColor = CardBg; card.Invalidate(); };
            return card;
        }

        private Panel CreateSearchBox(int x, int y, int w)
        {
            var pnl = new Panel { Location = new Point(x, y), Size = new Size(w, 38), BackColor = CardBg };
            pnl.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = RoundedRect(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 10);
                using var br   = new SolidBrush(CardBg);
                g.FillPath(br, path);
                using var pen  = new Pen(Border);
                g.DrawPath(pen, path);
                using var fi = new Font("Segoe UI Emoji", 11f);
                using var bc = new SolidBrush(TextMid);
                g.DrawString("🔍", fi, bc, 8, 7);
            };
            var txt = new TextBox
            {
                Location = new Point(34, 9), Width = w - 44,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10f),
                ForeColor = TextDark, BackColor = CardBg,
                PlaceholderText = "Cerca docente…",
            };
            pnl.Controls.Add(txt);
            return pnl;
        }

        private Button CreateButton(string text, int x, int y, Color col)
        {
            var btn = new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(154, 36),
                FlatStyle = FlatStyle.Flat, BackColor = col, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, 154, 36), 10));
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
