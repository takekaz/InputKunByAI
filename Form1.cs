using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InputKunByAI
{
    public partial class Form1 : Form
    {
        private const int PanelCount = 10;

        private readonly List<PanelRow> _rows = new();
        private readonly string _historyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HistoryCombo.txt");
        private readonly string _bikouPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BikouCombo.txt");
        private readonly string _orderNoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OrderNoCombo.txt");
        private readonly string _codeNoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CodeNoCombo.txt");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Defaults
            txtEndTime.Text = "17:30";

            EnsureDataFiles();

            BuildRows();
            LoadComboData();
        }

        private void BuildRows()
        {
            flpPanels.SuspendLayout();
            flpPanels.Controls.Clear();
            _rows.Clear();

            for (int i = 0; i < PanelCount; i++)
            {
                var row = new PanelRow(i + 1, ctxTime);
                row.BigCombo.SelectedIndexChanged += (s, e) => ApplyHistoryToRow(row);
                _rows.Add(row);
                flpPanels.Controls.Add(row.Container);
            }

            // Panel1 start time default
            _rows[0].StartText.Text = "08:30";

            // Assign context menu also to the end time
            txtEndTime.ContextMenuStrip = ctxTime;

            flpPanels.ResumeLayout(true);
            flpPanels.PerformLayout();
        }

        private void LoadComboData()
        {
            var orderItems = SafeReadAllLines(_orderNoPath);
            var codeItems = SafeReadAllLines(_codeNoPath);
            var historyItems = SafeReadAllLines(_historyPath);
            var bikouItems = SafeReadAllLines(_bikouPath);

            foreach (var row in _rows)
            {
                row.BigCombo.DropDownStyle = ComboBoxStyle.DropDownList;
                row.BigCombo.Items.Clear();
                row.BigCombo.Items.AddRange(historyItems);

                // row2 - free input allowed
                SetupEditableCombo(row.SmallCombo1, orderItems);
                SetupEditableCombo(row.SmallCombo2, codeItems);
                SetupEditableCombo(row.SmallCombo3, bikouItems);
            }
        }

        private static void SetupEditableCombo(ComboBox combo, string[] items)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDown;
            combo.Items.Clear();
            combo.Items.AddRange(items);
            combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            combo.AutoCompleteSource = AutoCompleteSource.ListItems;
            combo.IntegralHeight = false;
        }

        private void ApplyHistoryToRow(PanelRow row)
        {
            if (row.BigCombo.SelectedItem is string s)
            {
                var parts = s.Split('\t');
                if (parts.Length >= 3)
                {
                    row.SmallCombo1.Text = parts[0];
                    row.SmallCombo2.Text = parts[1];
                    row.SmallCombo3.Text = parts[2];
                }
            }
        }

        private void EnsureDataFiles()
        {
            // Create if missing
            if (!File.Exists(_historyPath)) File.WriteAllText(_historyPath, string.Empty, Encoding.UTF8);
            if (!File.Exists(_bikouPath)) File.WriteAllText(_bikouPath, string.Empty, Encoding.UTF8);
            if (!File.Exists(_orderNoPath))
            {
                File.WriteAllText(_orderNoPath,
                    "A000001：作業A000001" + Environment.NewLine +
                    "B000002：作業B000002" + Environment.NewLine +
                    "C000003：作業C000003",
                    Encoding.UTF8);
            }
            if (!File.Exists(_codeNoPath))
            {
                File.WriteAllText(_codeNoPath,
                    "020：コード020" + Environment.NewLine +
                    "031：コード031" + Environment.NewLine +
                    "032：コード032",
                    Encoding.UTF8);
            }

            // Trim to 500 lines for history files only at startup
            TrimFile(_historyPath, 500);
            TrimFile(_bikouPath, 500);
        }

        private static void TrimFile(string path, int maxLines)
        {
            try
            {
                var lines = SafeReadAllLines(path).Take(maxLines).ToArray();
                File.WriteAllLines(path, lines, Encoding.UTF8);
            }
            catch { /* ignore */ }
        }

        private static string[] SafeReadAllLines(string path)
        {
            try { return File.ReadAllLines(path, Encoding.UTF8); }
            catch { return Array.Empty<string>(); }
        }

        private void pbDrag_Paint(object sender, PaintEventArgs e)
        {
            // Draw simple target icon
            var g = e.Graphics;
            var r = pbDrag.ClientRectangle;
            using var p = new System.Drawing.Pen(System.Drawing.Color.DodgerBlue, 2);
            g.DrawEllipse(p, r.Left + 6, r.Top + 6, r.Width - 12, r.Height - 12);
            g.DrawLine(p, r.Left + r.Width / 2, r.Top + 4, r.Left + r.Width / 2, r.Bottom - 4);
            g.DrawLine(p, r.Left + 4, r.Top + r.Height / 2, r.Right - 4, r.Top + r.Height / 2);
        }

        private async void pbDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            // Begin drag capture: hide main window during drag
            var shouldRestoreTopMost = this.TopMost;
            try
            {
                // change system cursor to crosshair during drag
                SetGlobalCursorCrosshair();

                this.TopMost = true; // keep above until we hide
                this.Hide();

                await Task.Run(() => WaitForMouseUp());

                // Get cursor position and target window
                if (!GetCursorPos(out POINT pt)) return;
                var hWnd = WindowFromPoint(pt);
                if (hWnd == IntPtr.Zero) return;

                // Find parent of control if any
                var parent = GetParent(hWnd);
                if (parent == IntPtr.Zero) parent = hWnd;

                // Activate and click to focus at current cursor position
                FocusWindow(parent);
                ClickAtCursor();

                // Execute send sequence
                await Task.Delay(120);
                ExecuteSendSequence(parent);

                // Update history files after send
                UpdateHistoryFiles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                // restore cursors and window
                RestoreSystemCursors();

                this.Show();
                this.Activate();
                this.TopMost = shouldRestoreTopMost;
            }
        }

        private static void WaitForMouseUp()
        {
            // Simple polling loop
            while ((GetAsyncKeyState(0x01) & 0x8000) != 0)
            {
                Thread.Sleep(10);
            }
        }

        private void ExecuteSendSequence(IntPtr targetParent)
        {
            var endTime = txtEndTime.Text?.Trim() ?? string.Empty;

            // Prepare valid rows based on non-empty start time
            var validRows = new List<(PanelRow row, int index)>();
            for (int i = 0; i < _rows.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(_rows[i].StartText.Text)) break;
                validRows.Add((_rows[i], i));
            }

            if (validRows.Count == 0)
            {
                // No start times, just send end time
                SendText(targetParent, endTime);
                return;
            }

            // 1) End time
            SendText(targetParent, endTime);
            // 18 TABs
            SendTabs(targetParent, 18);
            Thread.Sleep(30);

            // Panel1: combos then start
            var first = validRows[0].row;
            SendComboTexts(targetParent, first);
            // After third combo, one TAB to move to start
            SendTab(targetParent);
            Thread.Sleep(20);
            // Start then 2 TABs
            SendText(targetParent, first.StartText.Text.Trim());
            SendTabs(targetParent, 2);
            Thread.Sleep(20);

            // If there is no next panel (panel2 start empty), send end time here and finish
            if (validRows.Count == 1)
            {
                SendText(targetParent, endTime);
                return;
            }

            // Other panels
            for (int i = 1; i < validRows.Count; i++)
            {
                var row = validRows[i].row;
                var isLast = i == validRows.Count - 1;

                // Send start time then 2 TABs to move to combos area
                SendText(targetParent, row.StartText.Text.Trim());
                SendTabs(targetParent, 2);
                Thread.Sleep(20);

                // Send three combos (with TAB between each) but not after third
                SendComboTexts(targetParent, row);

                // Send start time again then 2 TABs
                SendTab(targetParent); // move focus back to start field after combos
                Thread.Sleep(20);
                SendText(targetParent, row.StartText.Text.Trim());
                SendTabs(targetParent, 2);
                Thread.Sleep(20);

                if (isLast)
                {
                    // Send end time instead of next panel start
                    SendText(targetParent, endTime);
                    break;
                }
            }
        }

        private void SendComboTexts(IntPtr targetParent, PanelRow row)
        {
            // combo1 and combo2: send text before colon (both ASCII ':' and fullwidth '：')
            var c1 = row.SmallCombo1.Text ?? string.Empty;
            var c2 = row.SmallCombo2.Text ?? string.Empty;
            var c3 = row.SmallCombo3.Text ?? string.Empty;

            SendText(targetParent, TakeBeforeColon(c1));
            SendTab(targetParent);
            Thread.Sleep(10);
            SendText(targetParent, TakeBeforeColon(c2));
            SendTab(targetParent);
            Thread.Sleep(10);
            SendText(targetParent, c3);
        }

        private static string TakeBeforeColon(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            int idx = s.IndexOf('：');
            if (idx < 0) idx = s.IndexOf(':');
            return idx >= 0 ? s[..idx] : s;
        }

        private void UpdateHistoryFiles()
        {
            try
            {
                // Build history entries for valid rows
                var validRows = _rows
                    .TakeWhile(r => !string.IsNullOrWhiteSpace(r.StartText.Text))
                    .ToList();

                // Update HistoryCombo: TAB-joined three combos for each valid row
                var historyLines = SafeReadAllLines(_historyPath).ToList();
                foreach (var r in validRows)
                {
                    var hist = string.Join('\t', new[] { r.SmallCombo1.Text ?? "", r.SmallCombo2.Text ?? "", r.SmallCombo3.Text ?? "" });
                    if (!string.IsNullOrWhiteSpace(hist))
                    {
                        // remove duplicates
                        historyLines.RemoveAll(l => string.Equals(l, hist, StringComparison.Ordinal));
                        historyLines.Insert(0, hist);
                    }
                }
                // Trim to 500
                if (historyLines.Count > 500) historyLines = historyLines.Take(500).ToList();
                File.WriteAllLines(_historyPath, historyLines, Encoding.UTF8);

                // Update Bikou: gather third combo of valid rows
                var bikouLines = SafeReadAllLines(_bikouPath).ToList();
                foreach (var r in validRows)
                {
                    var b = r.SmallCombo3.Text?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(b))
                    {
                        bikouLines.RemoveAll(l => string.Equals(l, b, StringComparison.Ordinal));
                        bikouLines.Insert(0, b);
                    }
                }
                if (bikouLines.Count > 500) bikouLines = bikouLines.Take(500).ToList();
                File.WriteAllLines(_bikouPath, bikouLines, Encoding.UTF8);

                // Reload combos to reflect latest history/bikou
                LoadComboData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #region Context menu for time selection
        private void ctxTime_Opening(object sender, CancelEventArgs e)
        {
            var cms = (ContextMenuStrip)sender;
            cms.Items.Clear();

            // Build submenu by hour to simulate 4 columns
            for (int hour = 0; hour <= 23; hour++)
            {
                var hourMenu = new ToolStripMenuItem(hour.ToString("D2") + ":00 - " + hour.ToString("D2") + ":45");
                for (int min = 0; min < 60; min += 15)
                {
                    var text = $"{hour:D2}:{min:D2}";

                    // Add separators before specific times if present within this hour
                    if (text == "08:30" || text == "12:00" || text == "13:00" || text == "17:45")
                    {
                        if (hourMenu.DropDownItems.Count > 0)
                            hourMenu.DropDownItems.Add(new ToolStripSeparator());
                    }

                    var item = new ToolStripMenuItem(text);
                    item.Click += (s2, e2) =>
                    {
                        if (cms.SourceControl is TextBox tb)
                        {
                            tb.Text = text;
                        }
                    };
                    hourMenu.DropDownItems.Add(item);
                }
                cms.Items.Add(hourMenu);
            }
        }
        #endregion

        #region Win32 interop and input helpers
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr PostMessageA(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        private static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint WM_CHAR = 0x0102;
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const ushort VK_TAB = 0x09;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint OCR_NORMAL = 32512; // arrow
        private const int IDC_CROSS = 32515;
        private const uint SPI_SETCURSORS = 0x0057;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public INPUTUNION U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTUNION
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private void FocusWindow(IntPtr hWnd)
        {
            try
            {
                SetForegroundWindow(hWnd);
                SetFocus(hWnd);
            }
            catch { }
        }

        private void ClickAtCursor()
        {
            // Issue a left-click at current cursor position
            var inputs = new List<INPUT>
            {
                new INPUT { type = 0, U = new INPUTUNION { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTDOWN } } },
                new INPUT { type = 0, U = new INPUTUNION { mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTUP } } }
            };
            _ = SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf<INPUT>());
        }

        private void SendTab(IntPtr target)
        {
            var down = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new INPUTUNION { ki = new KEYBDINPUT { wVk = VK_TAB, dwFlags = 0 } }
            };
            var up = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new INPUTUNION { ki = new KEYBDINPUT { wVk = VK_TAB, dwFlags = KEYEVENTF_KEYUP } }
            };
            SendInput(2, new[] { down, up }, Marshal.SizeOf<INPUT>());
            Thread.Sleep(10);
        }

        private void SendTabs(IntPtr target, int count)
        {
            for (int i = 0; i < count; i++) SendTab(target);
        }

        private IntPtr GetFocusedHandleForWindow(IntPtr hwndCandidate)
        {
            try
            {
                uint procId;
                var targetTid = GetWindowThreadProcessId(hwndCandidate, out procId);
                var thisTid = GetCurrentThreadId();
                if (targetTid != 0)
                {
                    AttachThreadInput(thisTid, targetTid, true);
                    var focused = GetFocus();
                    AttachThreadInput(thisTid, targetTid, false);
                    if (focused != IntPtr.Zero) return focused;
                }
            }
            catch { }
            return hwndCandidate;
        }

        private void SendText(IntPtr target, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Obtain the actual focused child to receive WM_CHAR for DBCS (CP932)
            var wmCharTarget = GetFocusedHandleForWindow(target);

            // Always use SendInput Unicode path as requested
            foreach (var ch in text)
            {
                SendUnicodeChar(ch);
                Thread.Sleep(2);
            }
        }

        private void SendUnicodeChar(char ch)
        {
            var down = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new INPUTUNION { ki = new KEYBDINPUT { wVk = 0, wScan = ch, dwFlags = KEYEVENTF_UNICODE } }
            };
            var up = new INPUT
            {
                type = INPUT_KEYBOARD,
                U = new INPUTUNION { ki = new KEYBDINPUT { wVk = 0, wScan = ch, dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP } }
            };
            SendInput(2, new[] { down, up }, Marshal.SizeOf<INPUT>());
        }

        private void SetGlobalCursorCrosshair()
        {
            try
            {
                var hCur = LoadCursor(IntPtr.Zero, IDC_CROSS);
                if (hCur != IntPtr.Zero)
                {
                    SetSystemCursor(hCur, OCR_NORMAL);
                }
            }
            catch { }
        }

        private void RestoreSystemCursors()
        {
            try
            {
                SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, 0);
            }
            catch { }
        }
        #endregion

        private sealed class PanelRow
        {
            public Panel Container { get; }
            public ComboBox BigCombo { get; }
            public ComboBox SmallCombo1 { get; }
            public ComboBox SmallCombo2 { get; }
            public ComboBox SmallCombo3 { get; }
            public TextBox StartText { get; }

            public PanelRow(int index, ContextMenuStrip timeMenu)
            {
                Container = new Panel
                {
                    Width = 720,
                    Height = 58,
                    Margin = new Padding(2),
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(3)
                };

                BigCombo = new ComboBox { Left = 3, Top = 3, Width = 710, Height = 24, DropDownStyle = ComboBoxStyle.DropDownList, IntegralHeight = false };
                Container.Controls.Add(BigCombo);

                var tlp = new TableLayoutPanel
                {
                    Left = 3,
                    Top = 29,
                    Width = 710,
                    Height = 24,
                    ColumnCount = 4,
                    RowCount = 1,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };
                // 作業番号 30%, 作業区分 15%, 備考 40%, 開始 15%
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

                SmallCombo1 = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, IntegralHeight = false, Margin = new Padding(0) };
                SmallCombo2 = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, IntegralHeight = false, Margin = new Padding(0) };
                SmallCombo3 = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDown, IntegralHeight = false, Margin = new Padding(0) };
                StartText = new TextBox { Dock = DockStyle.Fill, ContextMenuStrip = timeMenu, Margin = new Padding(0) };

                tlp.Controls.Add(SmallCombo1, 0, 0);
                tlp.Controls.Add(SmallCombo2, 1, 0);
                tlp.Controls.Add(SmallCombo3, 2, 0);
                tlp.Controls.Add(StartText, 3, 0);
                Container.Controls.Add(tlp);
            }
        }
    }
}
