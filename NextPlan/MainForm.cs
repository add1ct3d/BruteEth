using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace NextPlan
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern uint PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, int hWnd2, string lpsz1, string lpsz2);

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0X0204;
        private const int WM_RBUTTONUP = 0x0205;


        private ChromiumWebBrowser mBrowser;
        private CefSettings mSettings;
        private IntPtr mHandle;

        private Random r = new Random();
        private string EthAddress = "f18e249d106c437cb6b4299f9a64d3a86cb25a6abf80ef5745b612153835f92d";


        private int Step = 0;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mSettings = new CefSettings();
            Cef.Initialize(mSettings);

            mBrowser = new ChromiumWebBrowser("https://www.myetherwallet.com/#view-wallet-info");
            mBrowser.Dock = DockStyle.None;
            mBrowser.Size = new Size(1920, 1080);
            mBrowser.Location = new Point(0, 0);

            Controls.Add(mBrowser);
            
            
        }

        private int LParam(int LoWord, int HiWord)
        {
            HiWord -= 23;
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }

        private void LClick(int x, int y, int next)
        {
            PostMessage(mHandle, WM_LBUTTONDOWN, 1, LParam(x, y));
            PostMessage(mHandle, WM_LBUTTONUP, 1, LParam(x, y));

            mTimer.Interval = next;
        }
        private void RClick(int x, int y, int next)
        {
            PostMessage(mHandle, WM_RBUTTONDOWN, 1, LParam(x, y));
            PostMessage(mHandle, WM_RBUTTONUP, 1, LParam(x, y));

            mTimer.Interval = next;
        }

        private void GetHandle()
        {
            mHandle = IntPtr.Zero;
            mHandle = FindWindowEx(Handle, 0, "WindowsForms10.Window.8.app.0.141b42a_r10_ad1", null);
            mHandle = FindWindowEx(mHandle, 0, "CefBrowserWindow", null);
            mHandle = FindWindowEx(mHandle, 0, "Chrome_WidgetWin_0", null);
            mHandle = FindWindowEx(mHandle, 0, "Chrome_RenderWidgetHostHWND", null);
            
            Text = mHandle == IntPtr.Zero ? "Not Found" : "Handle: " + mHandle;
        }

        private void GetRandomKey()
        {
            mTimer.Enabled = false;

            EthAddress = string.Empty;
            int tmp = 0;

            for (int i = 0; i < 64; i++)
            {
                tmp = r.Next(0, 16);
                switch (tmp)
                {
                    case 10:
                        EthAddress += "a";
                        break;
                    case 11:
                        EthAddress += "b";
                        break;
                    case 12:
                        EthAddress += "c";
                        break;
                    case 13:
                        EthAddress += "d";
                        break;
                    case 14:
                        EthAddress += "e";
                        break;
                    case 15:
                        EthAddress += "f";
                        break;
                    default:
                        EthAddress += "" + tmp;
                        break;
                }

            }
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, (Object)EthAddress);

            mTimer.Interval = 100;
            mTimer.Enabled = true;
        }

        private void mTimer_Tick(object sender, EventArgs e)
        {
            switch (Step)
            {
                case 1:
                    LClick(30, 30, 500);
                    break;
                case 2:
                    LClick(185, 642, 100);
                    break;
                case 3:
                    LClick(736, 616, 100);
                    break;
                case 4:
                    GetRandomKey();
                    break;
                case 5:
                    RClick(736, 616, 250);
                    break;
                case 6:
                    LClick(785, 750, 100);
                    break;
                case 7:
                    LClick(765, 726, 4000);
                    break;
                case 8:
                    mTimer.Enabled = false;
                    GetSourceTestAsync();
                    break;
                case 9:
                    LClick(109, 231,250);
                    break;
                case 10:
                    RClick(736, 616, 250);
                    break;
                case 11:
                    LClick(777, 777, 100);
                    break;
                case 12:
                    Step = 3;
                    break;
            }


            Step += 1;
        }

        private async Task GetSourceTestAsync()
        {
            List<string> FrameList = new List<string>();
            FrameList = mBrowser.GetBrowser().GetFrameNames();
            //Text = "out: " + FrameList.Count;


            //mBrowser.GetBrowser().GetFrame(FrameList[0]).GetSource();
            //MessageBox.Show(str.ToString());
            string source = await mBrowser.GetMainFrame().GetSourceAsync();
            if (source.IndexOf("Account Balance") > 0)
            {
                source = source.Substring(source.IndexOf("Account Balance") + 130);
                source = source.Substring(0, source.IndexOf("</span>"));
                //Text = source;
                /*
                if (double.Parse(source) > 0)
                {
                    
                }
                */
                listBox1.Items.Add(EthAddress + " - " + source + " ETH");
            }


            mTimer.Interval = 100;
            mTimer.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            GetHandle();

            Step = 1;
            mTimer.Interval = 200;
            mTimer.Enabled = true;
        }
    }
}
