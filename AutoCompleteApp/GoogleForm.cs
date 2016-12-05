#region Extensions

using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using AutoCompleteApp.Classes;
using System.Text.RegularExpressions;

#endregion

namespace AutoCompleteApp
{
    public partial class GoogleForm : Form
    {
        private bool IsBingSearch;
        autocompleteadminLive.AutoCompleteOMST webService = new autocompleteadminLive.AutoCompleteOMST();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public DateTime TimerAll { get; private set; }
        public DataSet dsKeywords { get; private set; }
        public bool IsTesting { get; private set; }

        public GoogleForm()
        {
            InitializeComponent();
            IsBingSearch = false;
            ///Starting the application
            timer5.Interval = 10000;
            timer5.Start();
        }

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

        #region Start IPVanish

        private void StartIPVanish()
        {
            Process ipVanish = new Process();
            if (System.Environment.Is64BitProcess)
                ipVanish.StartInfo.FileName = @"C:\Program Files (x86)\IPVanish\VPNClient.exe";
            else
                ipVanish.StartInfo.FileName = @"C:\Program Files\IPVanish\VPNClient.exe";
            ipVanish.StartInfo.Verb = "runas";
             
            ipVanish.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            ipVanish.StartInfo.UseShellExecute = false;
            ipVanish.StartInfo.RedirectStandardError = false;
            ipVanish.Start();
            ipVanish.WaitForExit();
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
                ForFireFoxBrowser();
                //ForGoogleChrome();
                //ForGoogleIE();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                ForFireFoxBrowser();
            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            { 
                ForGoogleSafari();
            } 
        } 

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (initialCheckForBrowser < Convert.ToInt32(txtBrowser.Text))
            {
                //ForIESecondStep();
                //ForGoogleSafariSecond();
                //ForChromeSecondStep();
                ForFireFoxBrowserSecond();

            }
            else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 2))
            {
                ForFireFoxBrowserSecond();
            }
            //else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 3))
            //{
            //    ForGoogleSafariSecond();
            //    //ForIESecondStep();
            //    initialCheckForBrowser++;
            //    if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 3) == 0)
            //    {
            //        initialCheckForBrowser = 0;
            //    }
            //}
            //else if (initialCheckForBrowser < (Convert.ToInt32(txtBrowser.Text) * 4))
            //{
            //    ForIESecondStep();
            //    //initialCheckForBrowser++;
            //    //if (initialCheckForBrowser % (Convert.ToInt32(txtBrowser.Text) * 4) == 0)
            //    //{
            //    //    initialCheckForBrowser = 0;
            //    //}
            //}
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void timer5_Tick(object sender, EventArgs e)
        {

        }

        #endregion

        private void ForGoogleSafari()
        {
            throw new NotImplementedException();
        }

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
                    chkBing.Checked = false;
                    var rnd = new Random();
                    var psi = new ProcessStartInfo("firefox.exe");
                    var windowHandler = new NativeWindowHandler();
                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    var ieProcess = Process.Start(psi);
                    windowHandler.MaximizeWindow(ieProcess.Handle.ToInt32());

                    Thread.Sleep(10000);
                    foreach (Char c in "google")
                    {
                        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                        SendKeys.Send(c.ToString());
                    }
                    System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
                    SendKeys.Send("^{ENTER}");
                    int checkCount = 0;
                    Thread.Sleep(7000 + rnd.Next(60, 100));

                    checkAgain:
                    if (!HelperMethods.getpixelsFireFox(chkBing.Checked) && checkCount < 3)
                    {
                        checkCount++;
                        goto checkAgain;
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

                    var rnd1 = new Random();
                    Thread.Sleep(1000 + rnd1.Next(60, 100));
                    SendKeys.Send("{ENTER}");
                    Thread.Sleep(3000 + rnd1.Next(60, 100));

                    checkCount = 0;

                    checkAgainSearchDoneFireFox:
                    if (!HelperMethods.getpixelsFireFoxSearchDone(chkBing.Checked) && checkCount < 3)
                    {
                        checkCount++;
                        goto checkAgainSearchDoneFireFox;
                    }
                    else
                    {
                        timer2_Tick(null, null);
                    }

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
        }

        private void ForFireFoxBrowserSecond()
        {
            try
            {
                timer2.Stop();
                HelperMethods.BrowserSecondStep("firefox");
            }
            catch (Exception ex)
            {

            }
        }

        public bool CheckEndFireFox()
        {
            try
            {
                Process[] procsChrome = Process.GetProcessesByName("firefox");
                if (procsChrome.Count() > 0)
                {
                    HelperMethods.handle = procsChrome[0].MainWindowHandle;
                    HelperMethods.SetForegroundWindow(HelperMethods.handle);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        #endregion

        
         
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

        

    }
}
