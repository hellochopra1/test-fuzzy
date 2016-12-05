using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Collections.ObjectModel;
using System.Net;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Data.OleDb;
using System.IO;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.IE;
namespace AutoCompleteApp.Classes
{
    public static class StoredValues
    {
        public static ChromeDriver Driver { get; set; }
        public static IWebDriver DriverNew { get; set; }
        public static SafariDriver DriverSafari { get; set; }
        public static InternetExplorerDriver DriverInternetExplorer { get; set; }
        public static int CheckIpCount { get; set; }
    }
}
