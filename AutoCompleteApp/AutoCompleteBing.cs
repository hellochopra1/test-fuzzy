#region Extensions

using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Data.OleDb;
using System.IO;
using AutoCompleteApp.Classes;
using System.Text.RegularExpressions;
using NDde.Client;
using System.Windows.Automation;

#endregion


namespace AutoCompleteApp
{

    public partial class AutoCompleteBing : Form
    {

        #region Home Settings

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData,
          int dwExtraInfo);

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const int INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95;
        public const int INTERNET_PER_CONN_PROXY_SERVER = 2;
        bool settingsReturn, refreshReturn;
        //AutoService.AutoGetTime dd = new AutoService.AutoGetTime();
        //AutoServiceLive.AutoGetTime dd = new AutoServiceLive.AutoGetTime();
        //LiveWebLocalHost.AutoCompleteOMST webService = new LiveWebLocalHost.AutoCompleteOMST();
        //totalautocompleteLive.AutoCompleteOMST webService = new totalautocompleteLive.AutoCompleteOMST();
        autocompleteadminLive.AutoCompleteOMST webService = new autocompleteadminLive.AutoCompleteOMST();
        //net.azurewebsites.autocomplete.AutoCompleteOMST webService = new net.azurewebsites.autocomplete.AutoCompleteOMST();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        DataSet ds = new DataSet();
        DataSet dsKeywords = new DataSet();
        bool checkStartUp = false;
        public AutoCompleteBing()
        {
            InitializeComponent();
            IsBingSearch = true;
            ///Starting the application
            timer5.Interval = 10000;
            timer5.Start();

        }

        #endregion

        #region Start IPVanish

        private void StartIPVanish()
        {
            Process inkscape = new Process();
            if (System.Environment.Is64BitProcess)
                inkscape.StartInfo.FileName = @"C:\Program Files (x86)\IPVanish\VPNClient.exe";
            else
                inkscape.StartInfo.FileName = @"C:\Program Files\IPVanish\VPNClient.exe";
            inkscape.StartInfo.Verb = "runas";

            //inkscape.StartInfo.EnvironmentVariables["WINDOWSFONTDIR"].Replace("WINDOWSFONTDIR", "C:\\Windows\\Fonts\\");// = "D:\\Windows\\Fonts\\";
            inkscape.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //inkscape.StartInfo.Arguments =
            //String.Format("--file \"{0}\" --export-{1} \"{2}\" --export-width {3} --export-height {4}",
            //                settings.SvgFilePath, settings.Extension, settings.OutputFilePath, settings.Width, settings.Height);
            inkscape.StartInfo.UseShellExecute = false;
            inkscape.StartInfo.RedirectStandardError = false;
            inkscape.Start();
            //inkscape.WaitForExit();

        }

        private bool getIpVanishClose()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.IpVanishLogon, bmpScreenshot, out location))
            {
                Cursor.Position = location;
                // click
                MouseClick();
            }
            else
            {
                FindBitmap(Properties.Resources.IPVanishClose, bmpScreenshot, out location);
                Cursor.Position = location;

                // click
                MouseClick();
            }
            return success;
        }

        private bool getIpVanishConnect()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.IpVanishConnect, bmpScreenshot, out location))
            {
                Cursor.Position = location;

                // click
                MouseClick();
            }
            return success;
        }

        #endregion

        #region Setup and Configure new IP

        int initialCheckForBrowser = 0;
        int checkKeywordLocation = 0;
        int CountryId = 0;
        string IPForVanish = string.Empty;
        bool check = false;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        private bool CheckIpChange()
        {
            int desc;
            if (InternetGetConnectedState(out desc, 0))
            {
                try
                {

                    WebClient wc = new WebClient();
                    string strIP = string.Empty;
                    string strLocal = string.Empty;
                    //string strIP = wc.DownloadString("http://checkip.dyndns.org");
                    string ipAddress = new WebClient().DownloadString("http://icanhazip.com");
                    strIP = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(ipAddress).Value;
                    wc.Dispose();
                    strLocal = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(txtLocalIp.Text).Value;
                    if (strIP != strLocal && strIP != IPForVanish)
                    {
                        IPForVanish = strIP;
                        check = true;
                    }
                    else
                    {
                        Thread.Sleep(20000);
                        CheckIpChange();
                    }
                    return check;
                }
                catch (Exception ex)
                {
                    Logger.Debug(string.Format("Could not get data CheckIP. {0}", ex));
                    CheckIpChange();
                    return false;
                }
            }
            else
            {
                Logger.Debug("Internet not connected");

                Thread.Sleep(10000);
                CheckIpChange();
                return false;
            }
        }

        #endregion

        #region All Timer Events

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            CheckIpChange();
            if (initialCheckForBrowser < Convert.ToInt32(txtBrowser.Text))
            {
                //ForGoogleSafari();
                //ForFireFoxBrowser();
                ForGoogleChrome();
                //ForGoogleIE();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                ForFireFoxBrowser();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            {
                //ForGoogleIE();

                ForGoogleSafari();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 4))
            {
                ForGoogleIE();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (initialCheckForBrowser < Convert.ToInt32(txtBrowser.Text))
            {
                //ForIESecondStep();
                //ForGoogleSafariSecond();
                ForChromeSecondStep();
                //ForFireFoxBrowserSecond();

            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                ForFireFoxBrowserSecond();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            {
                ForGoogleSafariSecond();
                //ForIESecondStep();
                initialCheckForBrowser++;
                if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 3) == 0)
                {
                    initialCheckForBrowser = 0;
                }
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 4))
            {
                ForIESecondStep();
                //initialCheckForBrowser++;
                //if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 4) == 0)
                //{
                //    initialCheckForBrowser = 0;
                //}
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();
            if (initialCheckForBrowser < Convert.ToInt32(txtBrowser.Text))
            {
                //checkChromeEnds:
                //    SendKeys.Send("%{F4}");
                //    Thread.Sleep(2000);

                //    //MouseOperations.SetCursorPosition(1364, 2);
                //    if (CheckEndChrome())
                //        goto checkChromeEnds;
                //clearchachelocalall();
                initialCheckForBrowser++;

                timer1.Interval = 5000;
                timer1.Start();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                //checkFireFoxEnds:
                //    SendKeys.Send("%{F4}");
                //Thread.Sleep(2000);
                //if (CheckEndFireFox())
                //    goto checkFireFoxEnds;
                //Endfirefox();
                //clearchachelocalall();
                initialCheckForBrowser++;
                //if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 2) == 0)
                //{
                //    initialCheckForBrowser = 0;
                //}

                timer1.Interval = 5000;
                timer1.Start();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            {
                ///For IE
                // MouseOperations.SetCursorPosition(1364, 2);
                initialCheckForBrowser++;
                if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 3) == 0)
                {
                    initialCheckForBrowser = 0;
                }
                timer1.Interval = 5000;
                timer1.Start();

                /////For Safari
                //MouseOperations.SetCursorPosition(1364, 2);
                //MouseClick();
                //clearchachelocalall();
                //timer1.Interval = 5000;
                //timer1.Start();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 4))
            {
                ///For IE
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
                timer1.Interval = 5000;
                timer1.Start();
            }

        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            timer4.Stop();
            //StartIPVanish();

            //Thread.Sleep(40000);

            button1_Click(null, null);

            //IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            //System.Threading.Thread.Sleep(2000);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);           
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            timer5.Stop();
            //CheckIpChange();
            txtDrive.Text = "C";
            txtProxy.Text = "5";
            txtUrl.Text = "https://www.google.com";
            txtBrowser.Text = "1";
            txtHomeWait.Text = "20";
            txtXAxis.Text = "1000";
            txtYAxis.Text = "800";
            WebClient wc = new WebClient();
            string strIP = string.Empty;
            //string strIP = wc.DownloadString("http://checkip.dyndns.org");
            string ipAddress = new WebClient().DownloadString("http://icanhazip.com");
            strIP = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(ipAddress).Value;
            txtLocalIp.Text = strIP;
            chkIsTesting.Checked = true;

            ddlCountry.Items.Insert(0, "US");
            ddlCountry.Items.Insert(1, "Canada");
            ddlCountry.SelectedIndex = 0;

            ddlSearchType.Items.Insert(0, "Google");
            ddlSearchType.Items.Insert(1, "YouTube");
            ddlSearchType.SelectedIndex = 0;
            chkBing.Checked = true;

            Thread.Sleep(10000);

            StartIPVanish();


            //IsTesting = true;
            //CountryId = 1;
            //this.WindowState = FormWindowState.Minimized;
            //clearchachelocalall();
            //Thread.Sleep(5000);
            //timer1.Interval = 10000;
            //timer1.Start();
            //button1_Click(null, null);

            //StartIPVanish();
            timer4.Interval = 3000;
            timer4.Start();
        }

        #endregion

        #region IE
        DateTime TimerIE;
        private void ForGoogleIE()
        {
            try
            {
                timer1.Stop();
                timer3.Interval = 420000;
                timer3.Start();
                TimerAll = DateTime.Now;
                System.Net.ServicePointManager.Expect100Continue = false;
                dsKeywords = new DataSet();
                dsKeywords = webService.SelectTopKeyword(CountryId, IsTesting, IsBingSearch);

                if (dsKeywords.Tables[0].Rows.Count > 0)
                {
                    ProcessStartInfo psi = new ProcessStartInfo("iexplore.exe");

                    NativeWindowHandler windowHandler = new NativeWindowHandler();

                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    Process ieProcess = Process.Start(psi);
                    windowHandler.MaximizeWindow(ieProcess.Handle.ToInt32());
                    Random rnd = new Random();
                    Thread.Sleep(5000);

                    foreach (Char c in chkBing.Checked ? "bing" : "google")
                    {
                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send(c.ToString());
                    }

                    System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                    SendKeys.Send("^{ENTER}");
                    int checkCount = 0;
                    Thread.Sleep(7000 + rnd.Next(60, 100));

                    //Thread.Sleep(5000);

                    if (!chkBing.Checked)
                    {
                    checkAgain:

                        if (!getpixelsIE() && checkCount < 3)
                        {
                            checkCount++;
                            goto checkAgain;
                        }
                        else
                        {

                            //MouseOperations.SetCursorPosition(500 + (new Random()).Next(20, 30), 410);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        }
                    }

                    Thread.Sleep(2000);
                    foreach (Char c in dsKeywords.Tables[0].Rows[0]["Keyword"].ToString())
                    {
                        rnd = new Random();
                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send(c.ToString());
                    }
                    Random rnd1 = new Random();

                    System.Threading.Thread.Sleep(1000 + rnd1.Next(60, 100));
                    SendKeys.Send("{ENTER}");
                    System.Threading.Thread.Sleep(3000 + rnd1.Next(60, 100));

                    checkCount = 0;
                    if (!chkBing.Checked)
                    {
                    checkAgainIESearchDone:

                        if (!getpixelsIESearchDone() && checkCount < 3)
                        {
                            checkCount++;
                            goto checkAgainIESearchDone;
                        }
                        else
                        {
                            timer2_Tick(null, null);
                            //MouseOperations.SetCursorPosition(500 + (new Random()).Next(20, 30), 410);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        }
                    }
                }
                else
                {
                    timer3.Stop();
                    timer1.Start();
                    timer1.Interval = 100000;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ForIESecondStep()
        {
            try
            {
                BrowserSecondStep("ie"); 
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region ChromeBrowser
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        private void ForGoogleChrome()
        {
            bool ifFailes = false;
            try
            {
                timer1.Stop();
                timer3.Interval = 420000;
                timer3.Start();
                TimerAll = DateTime.Now;
                System.Net.ServicePointManager.Expect100Continue = false;
                dsKeywords = new DataSet();

                dsKeywords = webService.SelectTopKeyword(CountryId, IsTesting, IsBingSearch);


                if (dsKeywords.Tables[0].Rows.Count > 0)
                {
                    Random rnd = new Random();
                    chkBing.Checked = true;
                    ProcessStartInfo psi = new ProcessStartInfo("chrome.exe", "-incognito");
                    NativeWindowHandler windowHandler = new NativeWindowHandler();
                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    Process ieProcess = Process.Start(psi);

                    int randoms = 1;// rnd.Next(1, 4);
                    //if (randoms == 3)
                    //    chkBing.Checked = true;
                    chkBing.Checked = true;

                    windowHandler.MaximizeWindow(ieProcess.Handle.ToInt32());
                    //else
                    //    MoveWindow(ieProcess.MainWindowHandle, 10 + rnd.Next(1, 20), 30 + rnd.Next(5, 20), 1000 + rnd.Next(30, 100), 650 + rnd.Next(30, 100), true);
                    Thread.Sleep(10000);
                    int checkCount = 0;
                     
                        foreach (Char c in chkBing.Checked ? "bing" : "google")
                        {
                            System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                            SendKeys.Send(c.ToString());
                        }

                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send("^{ENTER}");

                        Thread.Sleep(7000 + rnd.Next(60, 100));

                    checkAgain:
                        if (!getpixels(chkBing.Checked) && checkCount < 3)
                        {
                            checkCount++;
                            goto checkAgain;
                        }
                        else
                        {
                            //LinearSmoothMove(new Point(500 + (new Random()).Next(20, 30), 390), 50);
                            ////MouseOperations.SetCursorPosition(500 + (new Random()).Next(20, 30), 390);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                            //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        }
                    

                    string[] keywordSplit = dsKeywords.Tables[0].Rows[0]["Keyword"].ToString().Split(' ');

                    for (int co = 0; co < keywordSplit.Count(); co++)
                    {
                        foreach (Char c in keywordSplit[co].ToString())
                        {
                            rnd = new Random();
                            System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                            SendKeys.Send(c.ToString());
                        }
                        if ((co < keywordSplit.Count() - 1))
                            SendKeys.Send(" ");
                        Thread.Sleep(300 + rnd.Next(300, 500));
                    }


                    Random rnd1 = new Random();

                    System.Threading.Thread.Sleep(1000 + rnd1.Next(600, 800));
                    SendKeys.Send("{ENTER}");
                    System.Threading.Thread.Sleep(3000 + rnd1.Next(600, 800));

                    checkCount = 0;

                checkAgainSearchDoneChrome:
                    if (!getpixelsChromeSearchDone(chkBing.Checked) && checkCount < 3)
                    {
                        checkCount++;
                        goto checkAgainSearchDoneChrome;
                    }
                    else
                    {
                        timer2_Tick(null, null);
                    }

                }
                //}
                else
                {
                    //checkKeywordLocation++;
                    //timer3.Stop();
                    //timer1.Start();
                    //timer1.Interval = 100000;
                }
            }
            catch (WebException ex)
            {
                ifFailes = true;
                // generic error handling
                Logger.Debug(string.Format("Could not get data Chrome. {0}", ex));
            }
            catch (Exception exc)
            {
                Logger.Debug(string.Format("Exception details Chrome. {0}", exc));
            }
            if (ifFailes)
            {
                //timer3.Stop();
                // ForGoogleChrome();
            }

        }

        private void ForChromeSecondStep()
        {
            try
            {
                BrowserSecondStep("chrome"); 
            }
            catch (Exception ex)
            {

            }
        }

        public void EndIE()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("iexplore");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    EndIE();
                }
                clearchachelocalall();
            }
            catch { }
        }

        public void EndChrome()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("chrome");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    EndChrome();
                }
            }
            catch { }
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        private IntPtr handle;
        public bool CheckEndChrome()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("chrome");
                if (procsChrome.Count() > 0)
                {
                    handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(handle);

                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public bool CheckEndFireFox()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("firefox");
                if (procsChrome.Count() > 0)
                {
                    handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(handle);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public bool CheckEndIE()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("iexplore");
                if (procsChrome.Count() > 0)
                {
                    if (procsChrome.Count() > 1)
                        handle = procsChrome[1].MainWindowHandle;
                    else
                        handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(handle);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public bool CheckEndSafari()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("safari");
                if (procsChrome.Count() > 0)
                {
                    if (procsChrome.Count() > 1)
                        handle = procsChrome[1].MainWindowHandle;
                    else
                        handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(handle);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public void Endfirefox()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("firefox");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    Endfirefox();
                }
            }
            catch { }
        }
        #endregion

        #region Firefox Browser

        private void ForFireFoxBrowser()
        {
            bool ifFailes = false;
            try
            {
                timer1.Stop();
                timer3.Interval = 420000;
                timer3.Start();
                TimerAll = DateTime.Now;
                System.Net.ServicePointManager.Expect100Continue = false;
                dsKeywords = new DataSet();
                dsKeywords = webService.SelectTopKeyword(CountryId, IsTesting, IsBingSearch);

                if (dsKeywords.Tables[0].Rows.Count > 0)
                {
                    chkBing.Checked = true;
                    Random rnd = new Random();
                    ProcessStartInfo psi = new ProcessStartInfo("firefox.exe");
                    NativeWindowHandler windowHandler = new NativeWindowHandler();
                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    Process ieProcess = Process.Start(psi);
                    windowHandler.MaximizeWindow(ieProcess.Handle.ToInt32());
                    int randoms = 1;// rnd.Next(1, 4);
                    //chkBing.Checked = true;
                    //if (randoms == 3)
                    chkBing.Checked = true;
                    
                     

                    Thread.Sleep(10000);
                    foreach (Char c in chkBing.Checked ? "bing" : "google")
                    {
                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send(c.ToString());
                    }
                    System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                    SendKeys.Send("^{ENTER}");
                    int checkCount = 0;
                    Thread.Sleep(7000 + rnd.Next(60, 100));

                checkAgain:
                    if (!getpixelsFireFox(chkBing.Checked) && checkCount < 3)
                    {
                        checkCount++;
                        goto checkAgain;
                    }
                    else
                    {
                        //MessageBox.Show(checkCount.ToString());
                        //MessageBox.Show("Found");
                        //return;
                        //Thread.Sleep(2000);
                        //MouseOperations.SetCursorPosition(500 + (new Random()).Next(20, 30), 420);
                        //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                        //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                    }

                    string[] keywordSplit = dsKeywords.Tables[0].Rows[0]["Keyword"].ToString().Split(' ');

                    for (int co = 0; co < keywordSplit.Count(); co++)
                    {
                        foreach (Char c in keywordSplit[co].ToString())
                        {
                            rnd = new Random();
                            System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                            SendKeys.Send(c.ToString());
                        }
                        if ((co < keywordSplit.Count() - 1))
                            SendKeys.Send(" ");
                        Thread.Sleep(300 + rnd.Next(300, 500));
                    }

                    Random rnd1 = new Random();
                    System.Threading.Thread.Sleep(1000 + rnd1.Next(60, 100));
                    SendKeys.Send("{ENTER}");
                    System.Threading.Thread.Sleep(3000 + rnd1.Next(60, 100));

                    checkCount = 0;

                checkAgainSearchDoneFireFox:
                    if (!getpixelsFireFoxSearchDone(chkBing.Checked) && checkCount < 3)
                    {
                        checkCount++;
                        goto checkAgainSearchDoneFireFox;
                    }
                    else
                    {
                        timer2_Tick(null, null);
                    }


                    //Bitmap bmpScreenshot = Screenshot();
                    //Thread.Sleep(2000);
                    //bmpScreenshot.Save("d:\\myBitmapfirefox.bmp");

                }
                else
                {
                    //checkKeywordLocation++;
                    //timer3.Stop();
                    //timer1.Start();
                    //timer1.Interval = 100000;
                }
            }
            catch (WebException ex)
            {
                ifFailes = true;
                // generic error handling
                Logger.Debug(string.Format("Could not get data Firefox. {0}", ex));
            }
            catch (Exception exc)
            {
                Logger.Debug(string.Format("Exception details Firefox. {0}", exc));
            }
            if (ifFailes)
            {
                //timer3.Stop();
                //ForFireFoxBrowser();
            }
        }

        private void ForFireFoxBrowserSecond()
        {
            try
            {
                BrowserSecondStep("firefox");
            }
            catch (Exception ex)
            {

            }
        }

        private void BrowserSecondStep(string browserName)
        {
            timer2.Stop();
            scrollingMouse();
            Thread.Sleep(3000);
            movemouseAround();
            getpixels(browserName);
        }

        #endregion

        #region Get screenshot and click on image
        int totalSearches = 0;
        private void getpixels(string browser)
        {
            try
            {
                Thread.Sleep(5000);

                if (browser == "chrome")
                    SendKeys.Send("{F3}");
                else
                    SendKeys.Send("^(f)");

                Thread.Sleep(2000);
                string str2 = string.Empty;
                if (dsKeywords.Tables[0].Rows[0]["Link"].ToString() != "Website")
                {

                    str2 = dsKeywords.Tables[0].Rows[0]["Link"].ToString();

                    //string[] main = dsKeywords.Tables[0].Rows[0]["Link"].ToString().Split('.');
                    //if (main.Contains("www."))
                    //{
                    //    string[] main2 = main[2].Split('/');
                    //    str2 = main[1].ToString() + "." + main2[0].ToString();
                    //}
                    //else
                    //{
                    //    string[] main3 = main[1].Split('/');
                    //    str2 = main[0].ToString() + "." + main3[0].ToString();
                    //}
                }
                else
                    str2 = "Website";
                SendKeys.Send(str2);
                Thread.Sleep(5000);
                //SendKeys.Send("{ENTER}"); 

                // takes a snapshot of the screen
                Bitmap bmpScreenshot = Screenshot();
                Thread.Sleep(2000);

                //if (browser != "ie" && browser != "safari")
                //{
                //    SendKeys.Send("^(a)");
                //    Thread.Sleep(500);
                //    SendKeys.Send("{BACKSPACE}");
                //    Thread.Sleep(500);
                //    SendKeys.Send("{ESC}");
                //}
                // makes the background of the form a screenshot of the screen
                //this.BackgroundImage = bmpScreenshot; 
                //bmpScreenshot.Save("d:\\myBitmapgoogle.bmp");
                // find the login button and check if it exists
                Point location;
                bool success = false;
                if (browser == "chrome")
                    success = chkBing.Checked ? FindBitmap(Properties.Resources.chromeBing2, bmpScreenshot, out location) : FindBitmap(Properties.Resources.forchrome, bmpScreenshot, out location);
                else if (browser == "firefox")
                    success = FindBitmap(Properties.Resources.firefoxAll, bmpScreenshot, out location);
                else if (browser == "ie")
                    success = FindBitmap(Properties.Resources.iePagesGoogle, bmpScreenshot, out location) || FindBitmap(Properties.Resources.iePagesGoogle1, bmpScreenshot, out location);
                else
                    success = FindBitmap(Properties.Resources.forSafari, bmpScreenshot, out location);

                //FindBitmap(Properties.Resources.forfirefox, bmpScreenshot, out location) || FindBitmap(Properties.Resources.forFireFox1, bmpScreenshot, out location) || 
                //check if it found the bitmap
                //if (success == false)
                //{
                //    MessageBox.Show("Couldn't find the login button");
                //    return;
                //}

                if (success)
                {
                     
                    Random rnd1 = new Random();
                    location.X = location.X + rnd1.Next(2, 20);
                    if (browser == "chrome")
                        location.Y = location.Y - 20;
                    else if (browser == "firefox")
                        location.Y = location.Y - 20;
                    else if (browser == "safari")
                        location.Y = location.Y - 16;
                    else
                        location.Y = location.Y - 20;

                    LinearSmoothMove(location, 50);


                    // click
                    MouseClick();

                    Thread.Sleep(15000);
                    //string url = string.Empty;
                    ////Thread.Sleep(10000); 
                    //if (browser == "chrome")
                    //    url = GetBrowserURL("Chrome");
                    //else
                    //    url = GetBrowserURL("firefox");

                    //if (!url.Contains("google"))
                    //{
                    //    //Thread.Sleep(Convert.ToInt32(txtHomeWait.Text) * 1000);
                    //    GetBrowserRunning(browser);
                    //    // webService.IncreaseSearchCounter(Convert.ToInt32(dsKeywords.Tables[0].Rows[0]["Id"]));
                    //}
                    webService.IncreaseSearchCounter(Convert.ToInt32(dsKeywords.Tables[0].Rows[0]["Id"]));
                    totalSearches++;
                    Logger.Debug(totalSearches.ToString());
                    GetBrowserRunning(browser);
                }
                else
                {
                    int count = 0;
                    Random rnd1 = new Random();
                    if (browser == "chrome")
                    {
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkChromeEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);

                        //MouseOperations.SetCursorPosition(1364, 2);
                        if (CheckEndChrome() && count < 5)
                        {
                            count++;
                            goto checkChromeEnds;
                        }
                        else
                            count = 0;
                    }
                    else if (browser == "ie")
                    {
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkIEEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndIE())
                            goto checkIEEnds;
                    }
                    else if (browser == "safari")
                    {
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkSafariEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndSafari() && count < 5)
                        {
                            count++;
                            goto checkSafariEnds;
                        }
                        else
                            count = 0;
                    }
                    else
                    {
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkFireFoxEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndFireFox() && count < 5)
                        {
                            count++;
                            goto checkFireFoxEnds;
                        }
                        else
                            count = 0;
                    }
                }

                /*
                 *     [x] Snapshot of the whole screen
                 *     [x] Find the login button and check if it exists
                 *     [x] Move the mouse to login button
                 *     [ ] Click the login button
                 */
            }
            catch
            {
                int count = 0;
                Random rnd1 = new Random();
                if (browser == "chrome")
                {
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                checkChromeEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);

                    //MouseOperations.SetCursorPosition(1364, 2);
                    if (CheckEndChrome() && count < 5)
                    {
                        count++;
                        goto checkChromeEnds;
                    }
                    else
                        count = 0;
                }
                else if (browser == "ie")
                {
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                checkIEEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndIE())
                        goto checkIEEnds;
                }
                else if (browser == "safari")
                {
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                checkSafariEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndSafari() && count < 5)
                    {
                        count++;
                        goto checkSafariEnds;
                    }
                    else
                        count = 0;
                }
                else
                {
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                checkFireFoxEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndFireFox() && count < 5)
                    {
                        count++;
                        goto checkFireFoxEnds;
                    }
                    else
                        count = 0;
                }
            }
        }

        public void LinearSmoothMove(Point newPosition, int steps)
        {
            Point start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;
            Random rnd = new Random();
            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                Cursor.Position = Point.Round(iterPoint);
                //SetCursorPosition(Point.Round(iterPoint));
                Thread.Sleep(rnd.Next(3, 10));
            }
            //for (int i = 0; i < steps; i++)
            //{
            //    iterPoint = new PointF(iterPoint.X - slope.X, iterPoint.Y - slope.Y);
            //    Cursor.Position = Point.Round(iterPoint);
            //    //SetCursorPosition(Point.Round(iterPoint));
            //    Thread.Sleep(rnd.Next(5, 20));
            //}

            // Move the mouse to the final destination.
            //Cursor.Position = newPosition;
        }

        public void LinearSmoothMoveReverse(Point newPosition, int steps)
        {
            Point start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X + start.X, newPosition.Y + start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;
            Random rnd = new Random();
            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X - slope.X, iterPoint.Y - slope.Y);
                Cursor.Position = Point.Round(iterPoint);
                //SetCursorPosition(Point.Round(iterPoint));
                Thread.Sleep(rnd.Next(5, 20));
            }
            //for (int i = 0; i < steps; i++)
            //{
            //    iterPoint = new PointF(iterPoint.X - slope.X, iterPoint.Y - slope.Y);
            //    Cursor.Position = Point.Round(iterPoint);
            //    //SetCursorPosition(Point.Round(iterPoint));
            //    Thread.Sleep(rnd.Next(5, 20));
            //}

            // Move the mouse to the final destination.
            //Cursor.Position = newPosition;
        }

        //[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        //public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);




        private Point GetCursorPosition()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = new Point(Cursor.Position.X, Cursor.Position.Y);
            return p;
        }

        public void movemouseAround()
        {
            Random rnd = new Random();
            RECT rect = new RECT();
            GetWindowRect(GetForegroundWindow(), out rect);
            int centerRight = rect.Right / 2;
            int centerBottom = rect.Bottom / 2;
            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);

            Point start = GetCursorPosition();
            LinearSmoothMove(new Point(start.X + rnd.Next(100, 200), start.Y + rnd.Next(150, 300)), 50);
            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(start.X - rnd.Next(150, 225), start.Y - rnd.Next(100, 150)), 40);

            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);
            scrollingMouse();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        /// <summary>
        /// Simulates a mouse click
        /// </summary>
        private void MouseClick()
        {
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep((new Random()).Next(20, 30));
            mouse_event((int)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Takes a snapshot of the screen
        /// </summary>
        /// <returns>A snapshot of the screen</returns>
        private Bitmap Screenshot()
        {
            // this is where we will store a snapshot of the screen
            Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);


            // creates a graphics object so we can draw the screen in the bitmap (bmpScreenshot)
            Graphics g = Graphics.FromImage(bmpScreenshot);

            // copy from screen into the bitmap we created
            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);

            // return the screenshot
            return bmpScreenshot;
        }

        /// <summary>
        /// Find the location of a bitmap within another bitmap and return if it was successfully found
        /// </summary>
        /// <param name="bmpNeedle">The image we want to find</param>
        /// <param name="bmpHaystack">Where we want to search for the image</param>
        /// <param name="location">Where we found the image</param>
        /// <returns>If the bmpNeedle was found successfully</returns>
        private bool FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack, out Point location)
        {
            for (int outerX = 0; outerX < bmpHaystack.Width - bmpNeedle.Width; outerX++)
            {
                for (int outerY = 0; outerY < bmpHaystack.Height - bmpNeedle.Height; outerY++)
                {
                    for (int innerX = 0; innerX < bmpNeedle.Width; innerX++)
                    {
                        for (int innerY = 0; innerY < bmpNeedle.Height; innerY++)
                        {
                            Color cNeedle = bmpNeedle.GetPixel(innerX, innerY);
                            Color cHaystack = bmpHaystack.GetPixel(innerX + outerX, innerY + outerY);

                            if (cNeedle.R != cHaystack.R || cNeedle.G != cHaystack.G || cNeedle.B != cHaystack.B)
                            {
                                goto notFound;
                            }
                        }
                    }
                    location = new Point(outerX, outerY);
                    return true;
                notFound:
                    continue;
                }
            }
            location = Point.Empty;
            return false;
        }

        private string GetBrowserURL(string browser)
        {
            try
            {
                string ut = string.Empty;
                if (browser == "Chrome")
                {
                    Process[] procsChrome = Process.GetProcessesByName("chrome");
                    foreach (Process chrome in procsChrome)
                    {

                        // the chrome process must have a window
                        if (chrome.MainWindowHandle == IntPtr.Zero)
                        {
                            continue;
                        }

                        // find the automation element
                        AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);

                        AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
                          new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

                        // if it can be found, get the value from the URL bar
                        if (elmUrlBar != null)
                        {
                            AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                            if (patterns.Length > 0)
                            {
                                ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
                                ut = val.Current.Value;
                            }
                        }
                    }
                    return ut;
                }
                else
                {
                    DdeClient dde = new DdeClient(browser, "WWW_GetWindowInfo");
                    dde.Connect();
                    string url = dde.Request("URL", int.MaxValue);
                    string[] text = url.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                    dde.Disconnect();
                    return text[0].Substring(1);
                }
            }
            catch
            {
                return null;
            }
        }

        private bool getpixelsIE()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.ieClose, bmpScreenshot, out location))
            {
                success = true;
            }
            return success;

        }

        private bool getpixelsIESearchDone()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.ieSearchDone, bmpScreenshot, out location))
            {
                success = true;
            }
            return success;

        }

        private bool CloseChrome()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.CloseChrome, bmpScreenshot, out location))
            {
                success = true;
                Cursor.Position = location;
                // click
                MouseClick();
            }
            return success;

        }

        private bool CloseFirefox()
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            bool success = false;

            if (FindBitmap(Properties.Resources.CloseFirefox, bmpScreenshot, out location))
            {
                success = true;
                Cursor.Position = location;
                // click
                MouseClick();
            }
            return success;

        }


        private bool getpixels(bool isBing)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.chromeBing, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.chromeGoogle, bmpScreenshot, out location);
        }

        private bool getpixelsChromeSearchDone(bool isBing)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.bingSearchDone, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.chromeSearchDone, bmpScreenshot, out location);

        }

        private bool getpixelsFireFoxSearchDone(bool isBing)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.bingFireFoxSearchDone, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.firefoxSearchDone, bmpScreenshot, out location);
        }

        private bool getpixelsFireFox(bool isBing)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            if (isBing)
                return FindBitmap(Properties.Resources.firefoxBing, bmpScreenshot, out location);
            else
                return FindBitmap(Properties.Resources.firefoxclose, bmpScreenshot, out location);
        }

        public static Cursor Current { get; set; }
        public bool IsBingSearch { get; private set; }

        private void getpixelsForInnerPages(string browser, string link)
        {
            scrollingMouse();
            Thread.Sleep(5000);
            if (browser == "chrome")
                SendKeys.Send("{F3}");
            else
                SendKeys.Send("^(f)");

            Thread.Sleep(2000);

            SendKeys.Send(dsKeywords.Tables[0].Rows[0][link].ToString());
            Thread.Sleep(2000);
            if (browser == "ie")
            {
                LinearSmoothMove(new Point(0, 120), 50);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                Thread.Sleep(2000);
            }
            //notFound:
            //SendKeys.Send("{ENTER}");
            // takes a snapshot of the screen
            Bitmap bmpScreenshot = Screenshot();
            Point location;
            //bmpScreenshot.Save("d:\\myBitmapfirefox.bmp");

            if (browser == "chrome")
            {
                #region Chrome Inner
                if (FindBitmap(Properties.Resources.innerPagesChrome, bmpScreenshot, out location))
                {
                    location.X = location.X + 10;
                    location.Y = location.Y + 5;
                    LinearSmoothMove(location, 50);


                    //if (IsHandCursor())
                    //{
                    SendKeys.Send("^(a)");
                    Thread.Sleep(500);
                    SendKeys.Send("{BACKSPACE}");
                    Thread.Sleep(500);
                    SendKeys.Send("{ESC}");
                    // click
                    MouseClick();
                    location.X = location.X + 50;
                    location.Y = location.Y + 10;
                    Thread.Sleep(3000);
                    LinearSmoothMove(location, 50);
                    //}
                    //else
                    //{
                    //    SendKeys.Send("{ENTER}");
                    //    Thread.Sleep(2000);
                    //    goto notFound;
                    //}
                }
                else
                {
                    if (browser == "chrome")
                        SendKeys.Send("{F3}");
                    else
                        SendKeys.Send("^(f)");
                    Thread.Sleep(2000);

                    SendKeys.Send("not found");
                }
                #endregion
            }
            else if (browser == "firefox")
            {
                #region Firefox Inner

                bool success = false;
                success = FindBitmap(Properties.Resources.innerPagesFirefoxDark, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.innerpagesFireFox2, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.innerPagesFirefoxDark3, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.innerPagesFireFoxDark4, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.firefoxInnerWhite, bmpScreenshot, out location);
                if (success)
                {
                    location.X = location.X + 10;
                    location.Y = location.Y + 5;
                    LinearSmoothMove(location, 50);

                    SendKeys.Send("^(a)");
                    Thread.Sleep(500);
                    SendKeys.Send("{BACKSPACE}");
                    Thread.Sleep(500);
                    SendKeys.Send("{ESC}");
                    // click
                    MouseClick();
                    location.X = location.X + 50;
                    location.Y = location.Y + 10;
                    Thread.Sleep(3000);
                    LinearSmoothMove(location, 50);

                }
                else
                {
                    if (browser == "chrome")
                        SendKeys.Send("{F3}");
                    else
                        SendKeys.Send("^(f)");
                    Thread.Sleep(2000);

                    SendKeys.Send("not found");
                }

                #endregion
            }
            else if (browser == "ie")
            {
                #region IE Inner

                bool success = false;
                success = FindBitmap(Properties.Resources.ie6, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie2, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie3, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie4, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie5, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie1, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie7, bmpScreenshot, out location);
                //if (!success)
                //    success = FindBitmap(Properties.Resources.ie8, bmpScreenshot, out location);
                if (success)
                {
                    location.X = location.X + 10;
                    location.Y = location.Y + 5;
                    LinearSmoothMove(location, 50);

                    //SendKeys.Send("^(a)");
                    //Thread.Sleep(500);
                    //SendKeys.Send("{BACKSPACE}");
                    //Thread.Sleep(500);
                    //SendKeys.Send("{ESC}");
                    // click
                    MouseClick();
                    location.X = location.X + 50;
                    location.Y = location.Y + 10;
                    Thread.Sleep(3000);
                    LinearSmoothMove(location, 50);

                }
                else
                {
                    if (browser == "chrome")
                        SendKeys.Send("{F3}");
                    else
                        SendKeys.Send("^(f)");
                    Thread.Sleep(2000);

                    SendKeys.Send("not found");
                    Thread.Sleep(2000);
                    SendKeys.Send("^(a)");
                    Thread.Sleep(500);
                    SendKeys.Send("{BACKSPACE}");
                    Thread.Sleep(500);
                    SendKeys.Send("{ESC}");

                }
                #endregion
            }
            else
            {
                #region Safari Inner
                bool success = false;
                success = FindBitmap(Properties.Resources.SafariForInner, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.forSafari2, bmpScreenshot, out location);
                if (!success)
                    success = FindBitmap(Properties.Resources.forSafari3, bmpScreenshot, out location);

                if (success)
                {
                    location.X = location.X + 10;
                    location.Y = location.Y + 10;
                    Cursor.Position = location;

                    // click
                    MouseClick();
                    location.X = location.X + 50;
                    location.Y = location.Y + 10;
                    Thread.Sleep(3000);
                    LinearSmoothMove(location, 50);

                }
                else
                {
                    if (browser == "chrome")
                        SendKeys.Send("{F3}");
                    else
                        SendKeys.Send("^(f)");
                    Thread.Sleep(2000);

                    SendKeys.Send("not found");
                }
                #endregion
            }
        }

        private static bool IsHandCursor()
        {
            var h = Cursors.Hand.Handle;

            CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(out pci);

            return pci.hCursor == h;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
            // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
            //    0             The cursor is hidden.
            //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;          // Handle to the cursor. 
            public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        //public void LinearSmoothMove(Point newPosition, TimeSpan duration)
        //{
        //    Point start = GetCursorPosition();

        //    // Find the vector between start and newPosition
        //    double deltaX = newPosition.X - start.X;
        //    double deltaY = newPosition.Y - start.Y;

        //    // start a timer
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();

        //    double timeFraction = 0.0;

        //    do
        //    {
        //        timeFraction = (double)stopwatch.Elapsed.Ticks / duration.Ticks;
        //        if (timeFraction > 1.0)
        //            timeFraction = 1.0;

        //        PointF curPoint = new PointF(start.X + timeFraction * deltaX,
        //                                     start.Y + timeFraction * deltaY);
        //        SetCursorPosition(Point.Round(curPoint));
        //        Thread.Sleep(20);
        //    } while (timeFraction < 1.0);
        //}

        #endregion

        #region Scrolling

        private void scrollingMouse()
        {
            for (int a = 0; a <= 1; a++)
            {
                for (int i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.WHEEL, 0, 0, -50 + (new Random()).Next(10, 30), 0);
                    Thread.Sleep((new Random()).Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep((new Random()).Next(400, 500));
                for (int i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.WHEEL, 0, 0, 50 + (new Random()).Next(15, 30), 0);
                    Thread.Sleep((new Random()).Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep((new Random()).Next(400, 700));                
            }
        }


        private void GetBrowserRunning(string browser)
        {
            movemouseAround();

            Random linksRun = new Random();
            Random rnd1 = new Random();
            int run = linksRun.Next(3, 5);
            ///int run = linksRun.Next(1, 2);
            int[] numbers; // declare numbers as an int array of any size
            numbers = new int[run];
            //Thread.Sleep(10000);
            for (int c = 0; c < run; c++)
            {
            startCounting:
                Random randomKeyword = new Random();
                int caseSwitch = randomKeyword.Next(1, 6);
                if (numbers.Contains(caseSwitch))
                    goto startCounting;
                numbers[c] = caseSwitch;
                if (checkTimeRemains())
                {
                    switch (caseSwitch)
                    {
                        case 1:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;

                            case1:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case1;
                                }

                            }
                            getpixelsForInnerPages(browser, "Contact");
                            break;
                        case 2:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            case2:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case2;
                                }
                            }
                            getpixelsForInnerPages(browser, "Other");
                            break;
                        case 3:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            case3:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case3;
                                }
                            }
                            getpixelsForInnerPages(browser, "Link3");
                            break;
                        case 4:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            case4:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case4;
                                }
                            }
                            getpixelsForInnerPages(browser, "Link4");
                            break;
                        case 5:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            case5:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case5;
                                }
                            }
                            getpixelsForInnerPages(browser, "Link5");
                            break;
                        case 6:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            case6:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case6;
                                }
                            }
                            getpixelsForInnerPages(browser, "Link6");
                            break;
                        default:
                            if (checkTimeRemains())
                            {
                                timer4.Interval = Convert.ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(10, 30) * 1000;
                                timer4.Start();
                                TimerAllWithin = DateTime.Now;
                            caseD:
                                if (checkTimeRemainsWithin())
                                {
                                    movemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto caseD;
                                }
                            }
                            getpixelsForInnerPages(browser, "Contact");
                            break;
                    }
                }
            }

            bool checkIE = false;
            int count = 0;
            if (checkTimeRemains())
            {
                getpixelsForInnerPages(browser, "Contact");
            }
            if (browser == "chrome")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
            checkChromeEnds:
                SendKeys.Send("%{F4}");
                Thread.Sleep(2000);

                //MouseOperations.SetCursorPosition(1364, 2);
                if (CheckEndChrome() && count < 5)
                {
                    count++;
                    goto checkChromeEnds;
                }
                else
                    count = 0;
            }
            else if (browser == "safari")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
            checkSafariEnds:
                SendKeys.Send("%{F4}");
                Thread.Sleep(2000);
                if (CheckEndSafari() && count < 5)
                {
                    count++;
                    goto checkSafariEnds;
                }
                else
                    count = 0;
            }
            else if (browser == "ie")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                EndIE();
                //checkIEEnds:
                //    SendKeys.Send("%{F4}");
                //    Thread.Sleep(2000);
                //    if (CheckEndIE())
                //        goto checkIEEnds;
            }
            else
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
            checkFireFoxEnds:
                SendKeys.Send("%{F4}");
                Thread.Sleep(2000);
                if (CheckEndFireFox() && count < 5)
                {
                    count++;
                    goto checkFireFoxEnds;
                }
                else
                    count = 0;
            }

            Logger.Debug("Search completed with browser : " + browser + " at : " + DateTime.Now);


            //Thread.Sleep(5000);
            //timer3_Tick(null, null);
        }

        #endregion

        #region UnUsed Code

        DateTime TimerAll;
        public bool checkTimeRemains()
        {
            TimeSpan t;
            bool check = false;
            t = DateTime.Now - TimerAll;
            if (timer3.Interval - 40000 > t.TotalMilliseconds)
            {
                check = true;
            }
            return check;
        }

        DateTime TimerAllWithin;
        public bool checkTimeRemainsWithin()
        {
            TimeSpan t;
            bool check = false;
            t = DateTime.Now - TimerAllWithin;
            if (timer4.Interval - 4000 > t.TotalMilliseconds)
            {
                check = true;
            }
            else
            {
                timer4.Stop();
            }
            return check;
        }

        public void ReleaseProxyAndKeyword()
        {
            if (dsKeywords.Tables.Count > 0)
            {
                //MessageBox.Show("");
                webService.ReleaseKeyword(Convert.ToInt32(dsKeywords.Tables[0].Rows[0]["Id"]));
                //webService.ReleaseProxy(Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]));
            }
        }

        #region Safari Browser

        private void ForGoogleSafari()
        {
            bool ifFailes = false;
            try
            {
                timer1.Stop();
                timer3.Interval = 420000;
                timer3.Start();
                TimerAll = DateTime.Now;
                System.Net.ServicePointManager.Expect100Continue = false;
                dsKeywords = new DataSet();
                dsKeywords = webService.SelectTopKeyword(CountryId, IsTesting, IsBingSearch);

                if (dsKeywords.Tables[0].Rows.Count > 0)
                {
                    ProcessStartInfo psi = new ProcessStartInfo("safari.exe", "-private");

                    NativeWindowHandler windowHandler = new NativeWindowHandler();

                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    Process ieProcess = Process.Start(psi);
                    windowHandler.MaximizeWindow(ieProcess.Handle.ToInt32());
                    Random rnd = new Random();
                    Thread.Sleep(5000);

                    foreach (Char c in "bing")
                    {
                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send(c.ToString());
                    }

                    System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                    SendKeys.Send("^{ENTER}");

                    Thread.Sleep(10000);



                    //MouseOperations.SetCursorPosition(500 + (new Random()).Next(20, 30), 305);
                    //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    //MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);


                    //Thread.Sleep(2000);
                    string[] keywordSplit = dsKeywords.Tables[0].Rows[0]["Keyword"].ToString().Split(' ');

                    for (int co = 0; co < keywordSplit.Count(); co++)
                    {
                        foreach (Char c in keywordSplit[co].ToString())
                        {
                            rnd = new Random();
                            System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                            SendKeys.Send(c.ToString());
                        }
                        if ((co < keywordSplit.Count() - 1))
                            SendKeys.Send(" ");
                        Thread.Sleep(300 + rnd.Next(300, 500));
                    }
                    Random rnd1 = new Random();

                    System.Threading.Thread.Sleep(1000 + rnd1.Next(60, 100));
                    SendKeys.Send("{ENTER}");
                    rnd1 = new Random();
                    timer2.Interval = 15000 + rnd1.Next(10, 50);
                    timer2.Start();
                }
                else
                {
                    timer3.Stop();
                    timer1.Start();
                    timer1.Interval = 100000;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ForGoogleSafariSecond()
        {
            try
            {
                BrowserSecondStep("safari"); 
            }
            catch (Exception ex)
            {


            }
        }
        

        #endregion

        #endregion

        #region Set Proxy

        int checkIpLocation = 0;
        public void SetProxies()
        {
            //if (checkKeywordLocation % Convert.ToInt32(txtProxy.Text) == 0)
            //{
            //va loans
            //va loans federal savings bank

            //ohio divorce forms smartdivorce.com


            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            ds = webService.SelectTopProxy(CountryId);
            webService.LockDownProxy(Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]));
            string ip = ds.Tables[0].Rows[0]["ProxyIP"].ToString();
            RegKey.SetValue("ProxyServer", "" + ip + "");
            RegKey.SetValue("ProxyEnable", 1);
            //MessageBox.Show(ip);
            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_PER_CONN_PROXY_SERVER, IntPtr.Zero, 0);

            checkIpLocation++;
            //StoredValues.CheckIpCount = checkIpLocation;
            if (checkIpLocation >= ds.Tables[0].Rows.Count)
            {
                checkIpLocation = 0;
            }
            //this.Close();

            //}
        }

        #endregion

        #region Load Keywords

        OleDbConnection oledbConn;

        private DataSet GetProxyData()
        {

            //string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            //OleDbConnection MyConn = new OleDbConnection(ConnStr);
            //string strAccessSelect = "SELECT * FROM tblIpName";
            //OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            //OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            //MyConn.Open();

            //myDataAdapter.Fill(ds, "Proxies");


            var direct = txtProxyName.Text;// System.IO.Path.GetFullPath("ProxyAddress.xlsx");//"E:\\Dheeraj\\ProxyAddress.xlsx");
            //Directory.GetDirectoryRoot("E:\\Dheeraj\\ProxyAddress.xlsx");
            // string path = System.IO.Path.GetFullPath(Server.MapPath("~/Temp/eagleridge.xls"));
            if (Path.GetExtension(direct) == ".xls")
            {
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + direct + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");

                // oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");
            }
            else if (Path.GetExtension(direct) == ".xlsx")
            {
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + direct + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            }
            oledbConn.Open();
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter oleda = new OleDbDataAdapter();


            cmd.Connection = oledbConn;

            cmd.CommandText = "SELECT * FROM [Sheet1$]";

            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(ds);

            return ds;
        }

        private void UpdateProxy()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "update tblIpName set checked = 1 where id = (select top 1 id from tblIpName where Checked = 0)";
            //            string strAccessSelect = "SELECT top 1 * FROM tblKeyword where Checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            ds = new DataSet();
            MyConn.Open();
            myDataAdapter.Fill(ds, "Proxies");
        }

        private void SelectTopProxy()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "SELECT top 1 * FROM tblIpName where Checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            ds = new DataSet();
            MyConn.Open();
            myDataAdapter.Fill(ds, "Proxies");
        }

        private void ResetAllProxy()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "update tblIpName set checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            ds = new DataSet();
            MyConn.Open();
            myDataAdapter.Fill(ds, "Proxies");
        }

        private DataSet GetKeywordData()
        {

            //string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            //OleDbConnection MyConn = new OleDbConnection(ConnStr);
            //string strAccessSelect = "SELECT * FROM tblKeyword";
            //OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            //OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            //MyConn.Open();
            //myDataAdapter.Fill(dsKeywords, "Categories");


            var direct = txtKeyword.Text;// System.IO.Path.GetFullPath("KeyWordDetails.xlsx");//"E:\\Dheeraj\\ProxyAddress.xlsx");
            //Directory.GetDirectoryRoot("E:\\Dheeraj\\ProxyAddress.xlsx");
            // string path = System.IO.Path.GetFullPath(Server.MapPath("~/Temp/eagleridge.xls"));
            if (Path.GetExtension(direct) == ".xls")
            {
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + direct + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");

                // oledbConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"");
            }
            else if (Path.GetExtension(direct) == ".xlsx")
            {
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + direct + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
            }
            oledbConn.Open();
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter oleda = new OleDbDataAdapter();


            cmd.Connection = oledbConn;

            cmd.CommandText = "SELECT * FROM [Sheet1$]";

            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(dsKeywords);

            return dsKeywords;
        }

        private void UpdateKeyWord()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "update tblKeyword set checked = 1 where id = (select top 1 id from tblKeyword where Checked = 0)";
            //            string strAccessSelect = "SELECT top 1 * FROM tblKeyword where Checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            MyConn.Open();
            myDataAdapter.Fill(dsKeywords, "Categories");
        }

        private void SelectTopKeyWord()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "SELECT top 1 * FROM tblKeyword where Checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            dsKeywords = new DataSet();
            MyConn.Open();
            myDataAdapter.Fill(dsKeywords, "Categories");
        }

        private void ResetAllKeyWord()
        {
            string ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\OMST\Documents\Database1.accdb;";
            OleDbConnection MyConn = new OleDbConnection(ConnStr);
            string strAccessSelect = "update tblKeyword set checked = 0";
            OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, MyConn);
            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
            dsKeywords = new DataSet();
            MyConn.Open();
            myDataAdapter.Fill(dsKeywords, "Categories");
        }

        #endregion

        #region All Button Events

        int SearchType = 0;
        bool IsTesting = false;
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Process has been Started, Click ok to continue!");
            // this.WindowState = FormWindowState.Minimized;
            //GetKeywordData();
            //GetProxyData();

            if (ddlSearchType.SelectedIndex == 0)
                SearchType = 1;
            else
                SearchType = 2;

            if (ddlCountry.SelectedIndex == 0)
                CountryId = 1;
            else
                CountryId = 2;
            IsTesting = chkIsTesting.Checked;
            this.WindowState = FormWindowState.Minimized;
            // notifyIcon2.Icon = Properties.Resources.MinimizeIcon;
            notifyIcon2.BalloonTipTitle = "Minimize to Tray";
            notifyIcon2.BalloonTipText = "App still running";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon2.Visible = true;
                notifyIcon2.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon2.Visible = false;
            }


            timer1_Tick(null, null);

            //timer1.Interval = 1000;
            //timer1.Start();
        }

        private void notifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            timer1.Stop();
            timer2.Stop();
            Thread.Sleep(2000);
            if (initialCheckForBrowser < Convert.ToInt32(txtBrowser.Text))
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 4))
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }

            MessageBox.Show("Process has been stopped!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtProxyName.Text = openFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtKeyword.Text = openFileDialog2.FileName;
            }
        }

        #endregion

        #region Clear all cookies

        private void clearchachelocalall()
        {
            //string GooglePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Google\Chrome\User Data\Default\";
            //string MozilaPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Mozilla\Firefox\";
            //string Opera1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Opera\Opera";
            //string Opera2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Opera\Opera";
            //string Safari1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Apple Computer\Safari";
            //string Safari2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Apple Computer\Safari";
            string IE1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Intern~1";
            string IE2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\History";
            string IE3 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\Tempor~1";
            string IE4 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Microsoft\Windows\Cookies";
            //string Flash = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Macromedia\Flashp~1";

            //Call This Method ClearAllSettings and Pass String Array Param
            ClearAllSettings(new string[] { IE1, IE2, IE3, IE4 });

        }
        public void ClearAllSettings(string[] ClearPath)
        {
            foreach (string HistoryPath in ClearPath)
            {
                if (Directory.Exists(HistoryPath))
                {
                    DoDelete(new DirectoryInfo(HistoryPath));
                }
            }
        }
        void DoDelete(DirectoryInfo folder)
        {
            try
            {

                foreach (FileInfo file in folder.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    { }
                }
                foreach (DirectoryInfo subfolder in folder.GetDirectories())
                {
                    DoDelete(subfolder);
                }
            }
            catch
            {

            }
        }

        #endregion

    }

     
}
