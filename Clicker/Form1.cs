using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clicker
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static void ClickAt(uint X, uint Y)
        {
            SetCursorPos((int)X, (int)Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);

            //mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            //mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);

            System.Threading.Thread.Sleep(50);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var line in txtLog.Lines)
            {
                var vals = line.Split(' ');
                if (vals.Length == 2)
                {
                    var x = uint.Parse(vals[0]);
                    var y = uint.Parse(vals[1]);
                    ClickAt(x, y);
                }
                else 
                {
                    WriteToActiveWindow(line);
                }
            }
        }

        private void WriteToActiveWindow(string p)
        {
            SendKeys.SendWait(p);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Process p = Process.GetProcessesByName("notepad").FirstOrDefault();
            //if (p != null)
            //{
            //    IntPtr h = p.MainWindowHandle;
            //    SetForegroundWindow(h);
            //    SendKeys.SendWait("^v");
            //}

            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var pos = Cursor.Position;
            button2.Text = string.Format("{0} {1}{2}", pos.X, pos.Y, Environment.NewLine);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam, int flags, int timeout, out IntPtr pdwResult);

        const int SMTO_ABORTIFHUNG = 2;

        bool IsResponding(Process process)
        {
            var handleRef = new HandleRef(process, process.MainWindowHandle);

            int timeout = 100;
            IntPtr lpdwResult;

            IntPtr lResult = SendMessageTimeout(
                handleRef,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                SMTO_ABORTIFHUNG,
                timeout,
                out lpdwResult);

            return lResult != IntPtr.Zero;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process p = Process.GetProcessesByName("Sleeper").FirstOrDefault();
            if (p != null)
            {
                //IntPtr h = p.MainWindowHandle;
                //SetForegroundWindow(h);
                //SendKeys.SendWait("1000");
                
                var resp = IsResponding(p);

                //if (p.Responding)
                if (resp)
                    MessageBox.Show("YES");
                else
                    MessageBox.Show("NO");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process p = Process.GetProcessesByName("Sleeper").FirstOrDefault();
            while(!IsResponding(p))
            { }
            MessageBox.Show("Now ready");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            var active = GetForegroundWindow();

            var r = IsRespondingPtr(active);
            if (r)
                MessageBox.Show("YES");
            else
                MessageBox.Show("NO");
        }

        private bool IsRespondingPtr(IntPtr active)
        {
            var handleRef = new HandleRef(null, active);

            int timeout = 100;
            IntPtr lpdwResult;

            IntPtr lResult = SendMessageTimeout(
                handleRef,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                SMTO_ABORTIFHUNG,
                timeout,
                out lpdwResult);

            return lResult != IntPtr.Zero;
        }
    }
}
