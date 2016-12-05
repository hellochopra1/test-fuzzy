using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDde;
using NDde.Client;

namespace AutoCompleteApp
{
    public partial class ChangeMac : Form
    {
        public ChangeMac()
        {
            InitializeComponent();
        }
        public System.Diagnostics.Process p = new System.Diagnostics.Process();

        //private void richTextBox1_LinkClicked(object sender,
        //System.Windows.Forms.LinkClickedEventArgs e)
        //{
        //    // Call Process.Start method to open a browser
        //    // with link text as URL.

        //}

     
        private void button1_Click(object sender, EventArgs e)
        {

             p = System.Diagnostics.Process.Start(@"safari.exe", "-private");
            //p = System.Diagnostics.Process.Start(@"chrome.exe", "--incognito");
            //Thread.Sleep(5000);
            //SendKeys.Send("^{T}");
            ////Thread.Sleep(10000);
            ////char[] buffer = new char[100];
            ////int checkLocation = 0;
            ////bool done = false;
            ////bool checkbacn = false;
            //foreach (Char c in "divorce in bc by reliable divorce")
            //{

            //    Random rnd = new Random();
            //    System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
            //    SendKeys.Send(c.ToString());
            //    //if (checkLocation > 12 && !checkbacn)
            //    //{
            //    //    Char y = (Char)(Convert.ToUInt16(c) + rnd.Next(1, 3));
            //    //    SendKeys.Send(y.ToString());
            //    //}
            //    //else
            //    //{
            //    //    SendKeys.Send(c.ToString());
            //    //}
            //    //int checkback = 0;
            //    //int loop = 0;
            //    //if (checkLocation > 15 && !done)
            //    //{
            //    //    done = true;
            //    //    rnd = new Random();
            //    //    for (int i = 0; i < rnd.Next(8, 14); i++)
            //    //    {
            //    //        checkback++;
            //    //        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
            //    //        SendKeys.Send(OpenQA.Selenium.Keys.Backspace);
            //    //    }
            //    //    loop = checkback;
            //    //    for (int j = 0; j < checkback; j++)
            //    //    {
            //    //        loop--;
            //    //        System.Threading.Thread.Sleep(100 + rnd.Next(60, 100));
            //    //        SendKeys.Send(buffer[checkLocation - loop].ToString());
            //    //    }
            //    //    checkbacn = true;
            //    //}


            //}
            ////Random rnd1 = new Random();
            ////System.Threading.Thread.Sleep(100 + rnd1.Next(60, 100));

            ////SendKeys.Send("{ENTER}");
            ////Thread.Sleep(5000);
            ////GetBrowserURL("firefox");
            //////WebClient wc = new WebClient();
            //////string strIP = wc.DownloadString("http://checkip.dyndns.org");
            //////strIP = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(strIP).Value;
            //////wc.Dispose();

            //////const int MIN_MAC_ADDR_LENGTH = 12;
            //////string macAddress = string.Empty;
            //////long maxSpeed = -1;

            //////foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            //////{


            //////    string tempMac = nic.GetPhysicalAddress().ToString();
            //////    if (nic.Speed > maxSpeed &&
            //////        !string.IsNullOrEmpty(tempMac) &&
            //////        tempMac.Length >= MIN_MAC_ADDR_LENGTH)
            //////    {

            //////        maxSpeed = nic.Speed;
            //////        macAddress = tempMac;
            //////    }
            //////}
            //////string speed = maxSpeed.ToString();
            //////string sads = macAddress;

        }

        private string GetBrowserURL(string browser)
        {
            try
            {
                DdeClient dde = new DdeClient(browser, "WWW_GetWindowInfo");
                dde.Connect();
                string url = dde.Request("URL", int.MaxValue);
                string[] text = url.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                dde.Disconnect();
                return text[0].Substring(1);
            }
            catch
            {
                return null;
            }
        }

        static IPAddress getInternetIPAddress()
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                IPAddress gateway = IPAddress.Parse(getInternetGateway());
                return findMatch(addresses, gateway);
            }
            catch (FormatException e) { return null; }
        }

        static string getInternetGateway()
        {
            using (Process tracert = new Process())
            {
                ProcessStartInfo startInfo = tracert.StartInfo;
                startInfo.FileName = "tracert.exe";
                startInfo.Arguments = "-h 1 208.77.188.166"; // www.example.com
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                tracert.Start();

                using (StreamReader reader = tracert.StandardOutput)
                {
                    string line = "";
                    for (int i = 0; i < 9; ++i)
                        line = reader.ReadLine();
                    line = line.Trim();
                    return line.Substring(line.LastIndexOf(' ') + 1);
                }
            }
        }

        static IPAddress findMatch(IPAddress[] addresses, IPAddress gateway)
        {
            byte[] gatewayBytes = gateway.GetAddressBytes();
            foreach (IPAddress ip in addresses)
            {
                byte[] ipBytes = ip.GetAddressBytes();
                if (ipBytes[0] == gatewayBytes[0]
                    && ipBytes[1] == gatewayBytes[1]
                    && ipBytes[2] == gatewayBytes[2])
                {
                    return ip;
                }
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //HtmlWeb hw = new HtmlWeb();
            //HtmlDocument doc = hw.Load(/* url */);
            //foreach (HtmlNode link in doc.DocumentElement.SelectNodes("//a[@href]"))
            //{

            //}
        }
    }
}
