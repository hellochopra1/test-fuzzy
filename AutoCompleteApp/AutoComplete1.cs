#region Extensions

using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using AutoCompleteApp.autocompleteadminLive;
using AutoCompleteApp.Classes;
using AutoCompleteApp.Properties;
using NLog;
using static System.Convert;

#endregion

namespace AutoCompleteApp
{
    public partial class AutoComplete1 : Form
    {
        #region Start IPVanish

        private static void StartIpVanish()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Environment.Is64BitProcess
                        ? @"C:\Program Files (x86)\IPVanish\VPNClient.exe"
                        : @"C:\Program Files\IPVanish\VPNClient.exe",
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardError = false
                }
            };

            process.Start();
        }

        #endregion

        #region Home Settings

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData,
            int dwExtraInfo);

        private enum MouseEventFlags
        {
            Leftdown = 0x00000002,
            Leftup = 0x00000004,
            Middledown = 0x00000020,
            Middleup = 0x00000040,
            MOVE = 0x00000001,
            Absolute = 0x00008000,
            Rightdown = 0x00000008,
            Rightup = 0x00000010,
            Wheel = 0x00000800,
            Xdown = 0x00000080,
            Xup = 0x00000100
        }


        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public const int InternetOptionSettingsChanged = 39;
        public const int InternetOptionRefresh = 37;
        public const int InternetOptionProxySettingsChanged = 95;
        public const int InternetPerConnProxyServer = 2;

        //AutoService.AutoGetTime dd = new AutoService.AutoGetTime();
        //AutoServiceLive.AutoGetTime dd = new AutoServiceLive.AutoGetTime();
        //LiveWebLocalHost.AutoCompleteOMST _webService = new LiveWebLocalHost.AutoCompleteOMST();
        private readonly totalautocompleteLive.AutoCompleteOMST _webService = new totalautocompleteLive.AutoCompleteOMST();
        //private readonly AutoCompleteOMST _webService = new AutoCompleteOMST();
        //net.azurewebsites.autocomplete.AutoCompleteOMST webService = new net.azurewebsites.autocomplete.AutoCompleteOMST();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        private DataSet _ds = new DataSet();
        private DataSet _dsKeywords = new DataSet();

        public AutoComplete1()
        {
            InitializeComponent();

            _isBingSearch = false;
            //Starting the application
            timer5.Interval = 10000;
            timer5.Start();
        }

        #endregion

        #region Setup and Configure new IP

        private int _initialCheckForBrowser;
        private int _countryId;
        private string _ipForVanish = string.Empty;
        private bool _check;

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        private bool CheckIpChange()
        {
            int desc;
            if (InternetGetConnectedState(out desc, 0))
                try
                {
                    var wc = new WebClient();
                    var strIp = string.Empty;
                    var strLocal = string.Empty;
                    //string strIP = wc.DownloadString("http://checkip.dyndns.org");
                    var ipAddress = new WebClient().DownloadString("http://icanhazip.com");
                    strIp = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Match(ipAddress).Value;
                    wc.Dispose();
                    strLocal = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Match(txtLocalIp.Text).Value;
                    if ((strIp != strLocal) && (strIp != _ipForVanish))
                    {
                        _ipForVanish = strIp;
                        _check = true;
                    }
                    else
                    {
                        Thread.Sleep(20000);
                        CheckIpChange();
                    }
                    return _check;
                }
                catch (Exception ex)
                {
                    Logger.Debug($"Could not get data CheckIP. {ex}");
                    CheckIpChange();
                    return false;
                }
            Logger.Debug("Internet not connected");

            Thread.Sleep(10000);
            CheckIpChange();
            return false;
        }

        #endregion

        #region All Timer Events

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            CheckIpChange();
            if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text)) && chkChrome.Checked)
                ForGoogleChrome();
            else if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 2) && chkMozilla.Checked)
                ForFireFoxBrowser();
            else if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 3) && chkSafari.Checked)
                ForGoogleSafari();
            else if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 4) && chkIE.Checked)
                ForGoogleIe();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text)) && chkChrome.Checked)
            {
                //ForIESecondStep();
                //ForGoogleSafariSecond();
                ForChromeSecondStep();
                //ForFireFoxBrowserSecond();
                if (!chkIE.Checked && !chkSafari.Checked && !chkMozilla.Checked)
                    if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 1) == 0)
                        _initialCheckForBrowser = 0;
            }
            else if (((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 2) && chkMozilla.Checked) ||
                     ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 1) && !chkChrome.Checked))
            {
                ForFireFoxBrowserSecond();
                if (!chkIE.Checked && !chkSafari.Checked && !chkChrome.Checked)
                    if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 1) == 0)
                        _initialCheckForBrowser = 0;
                if (!chkIE.Checked && !chkSafari.Checked)
                    if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 2) == 0)
                        _initialCheckForBrowser = 0;
            }
            else if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 3) && chkSafari.Checked)
            {
                ForGoogleSafariSecond();
                //ForIESecondStep();
                if (!chkIE.Checked)
                    if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 3) == 0)
                        _initialCheckForBrowser = 0;
            }
            else if ((_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 4) && chkIE.Checked)
            {
                ForIeSecondStep();
                if (!chkSafari.Checked)
                    if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 3) == 0)
                    {
                        _initialCheckForBrowser = 0;
                        return;
                    }

                if (_initialCheckForBrowser % (ToInt32(txtBrowser.Text) * 4) == 0)
                    _initialCheckForBrowser = 0;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();
            timer1.Interval = 5000;
            timer1.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            timer4.Stop();
            //StartIPVanish();

            //Thread.Sleep(40000);

            //button1_Click(null, null);

            //IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            //System.Threading.Thread.Sleep(2000);
            //SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);           
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            timer5.Stop();

            #region Tool Settings

            var data = _webService.GetSettingsForGoogleAndBing(1);
            //CheckIpChange();  
            txtBrowser.Text = data.Tables[0].Rows[0]["NumberOfBrowserChange"].ToString();
            txtHomeWait.Text = data.Tables[0].Rows[0]["HomeWait"].ToString();
            chkChrome.Checked = true;
            chkIE.Checked = true;
            chkMozilla.Checked = true;
            chkSafari.Checked = true;
            ddlCountry.Items.Insert(0, "US");
            ddlCountry.Items.Insert(1, "Canada");
            ddlCountry.SelectedIndex = 0;
            var strIp = string.Empty;
            //string strIP = wc.DownloadString("http://checkip.dyndns.org");
            var ipAddress = new WebClient().DownloadString("http://icanhazip.com");
            strIp = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Match(ipAddress).Value;
            txtLocalIp.Text = strIp;
            chkIsTesting.Checked = true;
            
            _searchType = 1;
            _countryId = ddlCountry.SelectedIndex == 0 ? 1 : 2;
            _isTesting = true;
            WindowState = FormWindowState.Minimized;
            notifyIcon2.BalloonTipTitle = @"Minimize to Tray";
            notifyIcon2.BalloonTipText = @"App still running";
            switch (WindowState)
            {
                case FormWindowState.Minimized:
                    notifyIcon2.Visible = true;
                    notifyIcon2.ShowBalloonTip(500);
                    Hide();
                    break;
                case FormWindowState.Normal:
                    notifyIcon2.Visible = false;
                    break;
                case FormWindowState.Maximized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion 

            button1_Click(null, null);
        }

        #endregion

        #region ChromeBrowser

        private void ForGoogleChrome()
        {
            try
            {
                StartBrowserForAll("chrome.exe", "--incognito", "chrome");
            }
            catch (WebException ex)
            {
                // generic error handling
                Logger.Debug($"Could not get data Chrome. {ex}");
            }
            catch (Exception exc)
            {
                Logger.Debug($"Exception details Chrome. {exc}");
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

        #endregion

        #region Firefox Browser

        private void ForFireFoxBrowser()
        {
            try
            {
                StartBrowserForAll("firefox.exe", "", "firefox");
            }
            catch (WebException ex)
            {
                // generic error handling
                Logger.Debug($"Could not get data Firefox. {ex}");
            }
            catch (Exception exc)
            {
                Logger.Debug($"Exception details Firefox. {exc}");
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

        #endregion

        #region Safari Browser

        private void ForGoogleSafari()
        {
            var rnd1 = new Random();
            try
            {
                StartBrowserForAll("safari.exe", "--private");
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

        #region IE

        private void ForGoogleIe()
        {
            try
            {
                StartBrowserForAll("iexplore.exe", "", "ie");
            }
            catch (Exception ex)
            {
            }
        }

        private void ForIeSecondStep()
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

        #region Browser Steps

        private void StartBrowserForAll(string browserNameExe, string modeName, string browserName = "")
        {
            timer1.Stop();
            timer3.Interval = 420000;
            timer3.Start();
            _timerAll = DateTime.Now;
            ServicePointManager.Expect100Continue = false;
            _dsKeywords = new DataSet();
            _dsKeywords = _webService.SelectTopKeyword(_countryId, _isTesting, _isBingSearch);

            if (_dsKeywords.Tables[0].Rows.Count <= 0) return;

            var psi = new ProcessStartInfo(browserNameExe, modeName);

            var windowHandler = new NativeWindowHandler();

            psi.WindowStyle = ProcessWindowStyle.Maximized;
            var process = Process.Start(psi);
            if (process != null) windowHandler.MaximizeWindow(process.Handle.ToInt32());
            var rnd = new Random();
            Thread.Sleep(7000);

            foreach (var c in "google")
            {
                Thread.Sleep(100 + rnd.Next(60, 100));
                SendKeys.Send(c.ToString());
            }

            Thread.Sleep(100 + rnd.Next(60, 100));
            SendKeys.Send("^{ENTER}");

            Thread.Sleep(10000);

            //Thread.Sleep(2000);
            var keywordSplit = _dsKeywords.Tables[0].Rows[0]["Keyword"].ToString().Split(' ');

            for (var co = 0; co < keywordSplit.Count(); co++)
            {
                foreach (var c in keywordSplit[co])
                {
                    rnd = new Random();
                    Thread.Sleep(100 + rnd.Next(60, 100));
                    SendKeys.Send(c.ToString());
                }
                if (co < keywordSplit.Count() - 1)
                    SendKeys.Send(" ");
                Thread.Sleep(200 + rnd.Next(10, 30));
            }
            rnd = new Random();

            Thread.Sleep(1000 + rnd.Next(60, 100));
            SendKeys.Send("{ENTER}");

            rnd = new Random();
            var isPagingSearch = rnd.Next(1, 4);

            var checkCount = 0;
            if (string.IsNullOrEmpty(browserName))
            {
                checkAgainSearchDone:
                if (!GetPixelsWhenSearchDone(browserName) && (checkCount < 3))
                {
                    checkCount++;
                    goto checkAgainSearchDone;
                }
                if (isPagingSearch == 3)
                    GetPagingInProcess(browserName);
                else
                    timer2_Tick(null, null);
            }
            else
            {
                if (isPagingSearch == 3)
                    GetPagingInProcess(browserName);
                else
                {
                    timer2.Interval = 15000;
                    timer2.Start();
                }
            }
        }

        private void BrowserSecondStep(string browserName)
        {
            timer2.Stop();
            _initialCheckForBrowser++;
            ScrollingMouse();
            Thread.Sleep(3000);
            MovemouseAround();
            Getpixels(browserName);
        }

        private void GetPagingInProcess(string browser)
        {
            var numberOfClicks = new Random().Next(3, 5);
            for (var i = 0; i < numberOfClicks && CheckTimeRemains(); i++)
            {
                Thread.Sleep(3000);
                MovemouseAround();
                ScrollingDownMouse();
                //Thread.Sleep(3000);
                Getpixels(browser, true);
            }

            const int count = 0;
            var rnd1 = new Random();
            EndAllBrowsers(browser, rnd1, count);
        }

        #endregion

        #region End All Browsers

        public void EndIe()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("iexplore");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    EndIe();
                }
                Clearchachelocalall();
            }
            catch
            {
            }
        }

        public void EndChrome()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("chrome");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    EndChrome();
                }
            }
            catch
            {
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private IntPtr _handle;

        public bool CheckEndChrome()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("chrome");
                if (procsChrome.Count() > 0)
                {
                    _handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(_handle);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckEndFireFox()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("firefox");
                if (procsChrome.Count() > 0)
                {
                    _handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(_handle);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckEndIe()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("iexplore");
                if (procsChrome.Count() > 0)
                {
                    if (procsChrome.Count() > 1)
                        _handle = procsChrome[1].MainWindowHandle;
                    else
                        _handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(_handle);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckEndSafari()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("safari");
                if (procsChrome.Count() > 0)
                {
                    if (procsChrome.Count() > 1)
                        _handle = procsChrome[1].MainWindowHandle;
                    else
                        _handle = procsChrome[0].MainWindowHandle;
                    SetForegroundWindow(_handle);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Endfirefox()
        {
            try
            {
                var procsChrome = Process.GetProcessesByName("firefox");
                if (procsChrome.Count() > 0)
                {
                    procsChrome[0].Kill();
                    Thread.Sleep(3000);
                    Endfirefox();
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Get screenshot and click on image


        private int _totalSearches;

        private void Getpixels(string browser, bool isPaging = false)
        {
            try
            {
                Thread.Sleep(3000);
                if (isPaging == false)
                {
                    SendKeys.Send(browser == "chrome" ? "{F3}" : "^(f)");

                    Thread.Sleep(2000);
                    var str2 = _dsKeywords.Tables[0].Rows[0]["Link"].ToString() != "Website"
                        ? _dsKeywords.Tables[0].Rows[0]["Link"].ToString()
                        : "Website";
                    SendKeys.Send(str2);
                    Thread.Sleep(5000);
                    //SendKeys.Send("{ENTER}"); 
                }
                // takes a snapshot of the screen
                var bmpScreenshot = Screenshot();
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
                //bmpScreenshot.Save(@"d:\myBitmapgoogle.bmp");
                // find the login button and check if it exists
                Point location;
                var success = false;
                switch (browser)
                {
                    case "chrome":
                        success = FindBitmap(Resources.forchrome, bmpScreenshot, out location) ||
                                  FindBitmap(Resources.chromeSearch, bmpScreenshot, out location) ||
                                  FindBitmap(Resources.chromeNextImage, bmpScreenshot, out location);
                        break;
                    case "firefox":
                        success = FindBitmap(Resources.firefoxAll, bmpScreenshot, out location) ||
                                  FindBitmap(Resources.firefoxSnap, bmpScreenshot, out location);
                        break;
                    case "ie":
                        success = FindBitmap(Resources.iePagesGoogle, bmpScreenshot, out location) ||
                                  FindBitmap(Resources.iePagesGoogle1, bmpScreenshot, out location);
                        break;
                    default:
                        success = FindBitmap(Resources.forSafari, bmpScreenshot, out location);
                        break;
                }

                if (success)
                {
                    var rnd1 = new Random();
                    location.X = isPaging ? location.X : location.X + rnd1.Next(2, 20);
                    switch (browser)
                    {
                        case "chrome":
                            location.Y = isPaging ? location.Y : location.Y - 20;
                            break;
                        case "firefox":
                            location.Y = location.Y - 20;
                            break;
                        case "safari":
                            location.Y = location.Y - 16;
                            break;
                        default:
                            location.Y = location.Y - 20;
                            break;
                    }

                    LinearSmoothMove(location, 50);

                    // click
                    MouseClick();

                    if (isPaging == false)
                    {
                        Thread.Sleep(15000);
                        _totalSearches++;
                        Logger.Debug(_totalSearches.ToString());
                        GetBrowserRunning(browser);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        _totalSearches++;
                        Logger.Debug(_totalSearches.ToString());
                    }
                }
                else
                {
                    const int count = 0;
                    var rnd1 = new Random();
                    EndAllBrowsers(browser, rnd1, count);
                }
                _webService.IncreaseSearchCounter(ToInt32(_dsKeywords.Tables[0].Rows[0]["Id"]));
            }
            catch
            {
                _webService.IncreaseSearchCounter(ToInt32(_dsKeywords.Tables[0].Rows[0]["Id"]));

                var count = 0;
                var rnd1 = new Random();
                switch (browser)
                {
                    case "chrome":
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                        checkChromeEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);

                        //MouseOperations.SetCursorPosition(1364, 2);
                        if (CheckEndChrome() && (count < 5))
                        {
                            count++;
                            goto checkChromeEnds;
                        }
                        count = 0;
                        break;
                    case "ie":
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                        checkIEEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndIe())
                            goto checkIEEnds;
                        break;
                    case "safari":
                        Clearchachelocalall();
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                        checkSafariEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndSafari() && (count < 5))
                        {
                            count++;
                            goto checkSafariEnds;
                        }
                        count = 0;
                        break;
                    default:
                        Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                        checkFireFoxEnds:
                        SendKeys.Send("%{F4}");
                        Thread.Sleep(2000);
                        if (CheckEndFireFox() && (count < 5))
                        {
                            count++;
                            goto checkFireFoxEnds;
                        }
                        count = 0;
                        break;
                }
            }
        }

        private void EndAllBrowsers(string browser, Random rnd1, int count)
        {
            switch (browser)
            {
                case "chrome":
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkChromeEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);

                    //MouseOperations.SetCursorPosition(1364, 2);
                    if (CheckEndChrome() && (count < 5))
                    {
                        count++;
                        goto checkChromeEnds;
                    }
                    count = 0;
                    break;
                case "ie":
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkIEEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndIe())
                        goto checkIEEnds;
                    break;
                case "safari":
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    Clearchachelocalall();
                    checkSafariEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndSafari() && (count < 5))
                    {
                        count++;
                        goto checkSafariEnds;
                    }
                    count = 0;
                    break;
                default:
                    Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                    checkFireFoxEnds:
                    SendKeys.Send("%{F4}");
                    Thread.Sleep(2000);
                    if (CheckEndFireFox() && (count < 5))
                    {
                        count++;
                        goto checkFireFoxEnds;
                    }
                    count = 0;
                    break;
            }
        }

        public void LinearSmoothMove(Point newPosition, int steps)
        {
            var start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            var slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;
            var rnd = new Random();
            // Move the mouse to each iterative point.
            for (var i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                Cursor.Position = Point.Round(iterPoint);
                //SetCursorPosition(Point.Round(iterPoint));
                Thread.Sleep(rnd.Next(2, 10));
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

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private Point GetCursorPosition()
        {
            Cursor = new Cursor(Cursor.Current.Handle);
            var p = new Point(Cursor.Position.X, Cursor.Position.Y);
            return p;
        }

        public void MovemouseAround()
        {
            var rnd = new Random();
            var rect = new Rect();
            GetWindowRect(GetForegroundWindow(), out rect);
            var centerRight = rect.Right / 2;
            var centerBottom = rect.Bottom / 2;
            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);

            var start = GetCursorPosition();
            LinearSmoothMove(new Point(start.X + rnd.Next(100, 200), start.Y + rnd.Next(150, 300)), 50);
            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(start.X - rnd.Next(150, 225), start.Y - rnd.Next(100, 150)), 40);

            Thread.Sleep(1000 + rnd.Next(2000, 5000));

            LinearSmoothMove(new Point(centerRight + rnd.Next(10, 20), centerBottom + rnd.Next(1, 20)), 40);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        private void MouseClick()
        {
            mouse_event((int)MouseEventFlags.Leftdown, 0, 0, 0, 0);
            Thread.Sleep(new Random().Next(20, 30));
            mouse_event((int)MouseEventFlags.Leftup, 0, 0, 0, 0);
        }

        private Bitmap Screenshot()
        {
            // this is where we will store a snapshot of the screen
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);


            // creates a graphics object so we can draw the screen in the bitmap (bmpScreenshot)
            var g = Graphics.FromImage(bmpScreenshot);

            // copy from screen into the bitmap we created
            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);

            // return the screenshot
            return bmpScreenshot;
        }

        private bool FindBitmap(Bitmap bmpNeedle, Bitmap bmpHaystack, out Point location)
        {
            for (var outerX = 0; outerX < bmpHaystack.Width - bmpNeedle.Width; outerX++)
                for (var outerY = 0; outerY < bmpHaystack.Height - bmpNeedle.Height; outerY++)
                {
                    for (var innerX = 0; innerX < bmpNeedle.Width; innerX++)
                        for (var innerY = 0; innerY < bmpNeedle.Height; innerY++)
                        {
                            var cNeedle = bmpNeedle.GetPixel(innerX, innerY);
                            var cHaystack = bmpHaystack.GetPixel(innerX + outerX, innerY + outerY);

                            if ((cNeedle.R != cHaystack.R) || (cNeedle.G != cHaystack.G) || (cNeedle.B != cHaystack.B))
                                goto notFound;
                        }
                    location = new Point(outerX, outerY);
                    return true;
                    notFound:
                    ;
                }
            location = Point.Empty;
            return false;
        }

        private bool GetPixelsWhenSearchDone(string browserName)
        {
            Thread.Sleep(5000);
            // takes a snapshot of the screen
            var bmpScreenshot = Screenshot();
            Point location;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (browserName)
            {
                case "ie":
                    return FindBitmap(Resources.ieSearchDone, bmpScreenshot, out location);
                case "firefox":
                    return FindBitmap(Resources.firefoxSearchDone, bmpScreenshot, out location);
                case "chrome":
                    return FindBitmap(Resources.chromeSearchDone, bmpScreenshot, out location);
            }
            return false;
        }

        private void GetpixelsForInnerPages(string browser, string link)
        {
            ScrollingMouse();
            Thread.Sleep(5000);
            SendKeys.Send(browser == "chrome" ? "{F3}" : "^(f)");

            Thread.Sleep(2000);

            SendKeys.Send(_dsKeywords.Tables[0].Rows[0][link].ToString());
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
            var bmpScreenshot = Screenshot();
            Point location;
            //bmpScreenshot.Save("d:\\myBitmapfirefox.bmp");

            switch (browser)
            {
                case "chrome":

                    #region Chrome Inner

                    if (FindBitmap(Resources.innerPagesChrome, bmpScreenshot, out location))
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
                    break;

                #endregion

                case "firefox":
                    {
                        #region Firefox Inner

                        var success = false;
                        success = FindBitmap(Resources.innerPagesFirefoxDark, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.innerpagesFireFox2, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.innerPagesFirefoxDark3, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.innerPagesFireFoxDark4, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.firefoxInnerWhite, bmpScreenshot, out location);
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
                    break;
                case "ie":
                    {
                        #region IE Inner

                        var success = false;
                        success = FindBitmap(Resources.ie6, bmpScreenshot, out location);
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
                    break;
                default:
                    {
                        #region Safari Inner

                        var success = false;
                        success = FindBitmap(Resources.SafariForInner, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.forSafari2, bmpScreenshot, out location);
                        if (!success)
                            success = FindBitmap(Resources.forSafari3, bmpScreenshot, out location);

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
                            Clearchachelocalall();
                            if (browser == "chrome")
                                SendKeys.Send("{F3}");
                            else
                                SendKeys.Send("^(f)");
                            Thread.Sleep(2000);

                            SendKeys.Send("not found");
                        }

                        #endregion
                    }
                    break;
            }
        }

        #endregion

        #region Scrolling

        private void ScrollingMouse()
        {
            for (var a = 0; a < 1; a++)
            {
                for (var i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.Wheel, 0, 0, -50 + new Random().Next(10, 30), 0);
                    Thread.Sleep(new Random().Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep(new Random().Next(400, 500));
                for (var i = 0; i <= 20; i++)
                {
                    mouse_event((int)MouseEventFlags.Wheel, 0, 0, 50 + new Random().Next(15, 30), 0);
                    Thread.Sleep(new Random().Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep(new Random().Next(400, 700));
            }
        }

        private static void ScrollingDownMouse()
        {
            for (var a = 0; a < 4; a++)
            {
                for (var i = 0; i <= 15; i++)
                {
                    mouse_event((int)MouseEventFlags.Wheel, 0, 0, -50 + new Random().Next(10, 30), 0);
                    Thread.Sleep(new Random().Next(20, 100));
                    //mouse_event((uint)MouseEventFlags.XUP, 0, 0, 50, 0);
                }
                Thread.Sleep(new Random().Next(400, 700));
            }
        }


        private void GetBrowserRunning(string browser)
        {
            MovemouseAround();

            var linksRun = new Random();
            var rnd1 = new Random();
            var run = linksRun.Next(3, 5);
            ///int run = linksRun.Next(1, 2);
            int[] numbers; // declare numbers as an int array of any size
            numbers = new int[run];
            //Thread.Sleep(10000);
            for (var c = 0; c < run; c++)
            {
                startCounting:
                var randomKeyword = new Random();
                var caseSwitch = randomKeyword.Next(1, 6);
                if (numbers.Contains(caseSwitch))
                    goto startCounting;
                numbers[c] = caseSwitch;
                if (CheckTimeRemains())
                    switch (caseSwitch)
                    {
                        case 1:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;

                                case1:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case1;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Contact");
                            break;
                        case 2:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                case2:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case2;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Other");
                            break;
                        case 3:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                case3:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case3;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Link3");
                            break;
                        case 4:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                case4:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case4;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Link4");
                            break;
                        case 5:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                case5:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case5;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Link5");
                            break;
                        case 6:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                case6:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto case6;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Link6");
                            break;
                        default:
                            if (CheckTimeRemains())
                            {
                                timer4.Interval = ToInt32(txtHomeWait.Text) * 1000 + rnd1.Next(2, 10) * 1000;
                                timer4.Start();
                                _timerAllWithin = DateTime.Now;
                                caseD:
                                if (CheckTimeRemainsWithin())
                                {
                                    MovemouseAround();
                                    Thread.Sleep(3000 + rnd1.Next(1000, 4000));
                                    goto caseD;
                                }
                            }
                            GetpixelsForInnerPages(browser, "Contact");
                            break;
                    }
            }

            var checkIe = false;
            var count = 0;
            if (CheckTimeRemains())
                GetpixelsForInnerPages(browser, "Contact");
            if (browser == "chrome")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                checkChromeEnds:
                SendKeys.Send("%{F4}");
                Thread.Sleep(2000);

                //MouseOperations.SetCursorPosition(1364, 2);
                if (CheckEndChrome() && (count < 5))
                {
                    count++;
                    goto checkChromeEnds;
                }
                count = 0;
            }
            else if (browser == "safari")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));

                checkSafariEnds:
                SendKeys.Send("%{F4}");
                Thread.Sleep(2000);
                if (CheckEndSafari() && (count < 5))
                {
                    count++;
                    goto checkSafariEnds;
                }
                count = 0;
            }
            else if (browser == "ie")
            {
                Thread.Sleep(10000 + rnd1.Next(3000, 6000));
                EndIe();
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
                if (CheckEndFireFox() && (count < 5))
                {
                    count++;
                    goto checkFireFoxEnds;
                }
                count = 0;
            }
            Clearchachelocalall();
            Logger.Debug("Search completed with browser : " + browser + " at : " + DateTime.Now);


            //Thread.Sleep(5000);
            //timer3_Tick(null, null);
        }

        #endregion

        #region Check Timers

        private DateTime _timerAll;

        public bool CheckTimeRemains()
        {
            TimeSpan t;
            var check = false;
            t = DateTime.Now - _timerAll;
            if (timer3.Interval - 40000 > t.TotalMilliseconds)
                check = true;
            return check;
        }

        private DateTime _timerAllWithin;

        public bool CheckTimeRemainsWithin()
        {
            TimeSpan t;
            var check = false;
            t = DateTime.Now - _timerAllWithin;
            if (timer4.Interval - 4000 > t.TotalMilliseconds)
                check = true;
            else
                timer4.Stop();
            return check;
        }

        public void ReleaseProxyAndKeyword()
        {
            if (_dsKeywords.Tables.Count > 0)
                _webService.ReleaseKeyword(ToInt32(_dsKeywords.Tables[0].Rows[0]["Id"]));
        }

        #endregion

        #region All Button Events

        private int _searchType;
        private bool _isTesting;
        private readonly bool _isBingSearch;

        private void button1_Click(object sender, EventArgs e)
        {
            StartIpVanish();

            Thread.Sleep(30000);

            timer1_Tick(null, null);
        }

        private void notifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            timer1.Stop();
            timer2.Stop();
            Thread.Sleep(2000);
            if (_initialCheckForBrowser < ToInt32(txtBrowser.Text))
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 2)
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 3)
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }
            else if (_initialCheckForBrowser < ToInt32(txtBrowser.Text) * 4)
            {
                MouseOperations.SetCursorPosition(1364, 2);
                MouseClick();
            }

            MessageBox.Show(@"Process has been stopped!");
        }

        #endregion

        #region Clear all cookies

        private void Clearchachelocalall()
        {
            //string GooglePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Google\Chrome\User Data\Default\";
            //string MozilaPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Mozilla\Firefox\";
            //string Opera1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Opera\Opera";
            //string Opera2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Opera\Opera";
            var safari1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Apple Computer\Safari";
            var safari2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Apple Computer\Safari";
            //string IE1 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Intern~1";
            //string IE2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\History";
            //string IE3 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Local\Microsoft\Windows\Tempor~1";
            //string IE4 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Microsoft\Windows\Cookies";
            //string Flash = Environment.GetEnvironmentVariable("USERPROFILE") + @"\AppData\Roaming\Macromedia\Flashp~1";

            //Call This Method ClearAllSettings and Pass String Array Param
            ClearAllSettings(new[] { safari1, safari2 });
        }

        public void ClearAllSettings(string[] clearPath)
        {
            foreach (var historyPath in clearPath)
                if (Directory.Exists(historyPath))
                    DoDelete(new DirectoryInfo(historyPath));
        }

        private void DoDelete(DirectoryInfo folder)
        {
            try
            {
                foreach (var file in folder.GetFiles())
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                    }
                foreach (var subfolder in folder.GetDirectories())
                    DoDelete(subfolder);
            }
            catch
            {
            }
        }

        #endregion
    }

    #region NativeWindowHandler

    public class NativeWindowHandler
    {
        private const int WmClose = 0x0010;
        private const int SwShownormal = 1;
        private const int SwShowminimized = 2;
        private const int SwShowmaximized = 3;


        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd,
            int msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        [DllImport("user32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);


        public int GetWnd(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                throw new ArgumentException("Process Name " + processName +
                                            " not running!");

            return processes[0].MainWindowHandle.ToInt32();
        }


        public int GetForegroundWindowEx()
        {
            return GetForegroundWindow();
        }


        public void CloseWindow(int handle)
        {
            SendMessage(handle, WmClose, 0, 0);
        }


        public void MinimizeWindow(int handle)
        {
            ShowWindow(handle, SwShowminimized);
        }


        public void MaximizeWindow(int handle)
        {
            ShowWindow(handle, SwShowmaximized);
        }


        public void NormalizeWindow(int handle)
        {
            ShowWindow(handle, SwShownormal);
        }
    }

    #endregion
}