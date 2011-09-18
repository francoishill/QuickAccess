using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace QuickAccess
{
    public partial class Form1 : Form
    {
        private static string ThisAppName = "QuickAccess";
        //private const string ServerAddress = "http://localhost";
        //private const string ServerAddress = "https://fjh.co.za";
        private const string ServerAddress = "http://firepuma.com";
        private const string doWorkAddress = ServerAddress + "/desktopapp";
        private string Username = "f";
        private string Password = "f";
        private const string MySQLdateformat = "yyyy-MM-dd HH:mm:ss";

        private const string AvailableActionList = "Type tasks: todo, run, mail, explore, web, google, kill, etc";

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>The GetForegroundWindow function returns a handle to the foreground window.</summary>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        const int WM_HOTKEY = 786;
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        const uint MOD_ALT = 1;
        const uint MOD_CONTROL = 2;
        const uint MOD_SHIFT = 4;
        const uint MOD_WIN = 8;
        const int Id = 500;

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //// Activate or minimize a window
        //[DllImportAttribute("User32.DLL")]
        //private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //private const int SW_SHOW = 5;
        //private const int SW_MINIMIZE = 6;
        //private const int SW_RESTORE = 9;

        //Win32 API calls necesary to raise an unowned processs main window

        public Form1()
        {
            InitializeComponent();

            if (!System.Diagnostics.Debugger.IsAttached && Environment.GetCommandLineArgs()[0].ToUpper().Contains("Apps\\2.0".ToUpper()))
            {
                Microsoft.Win32.RegistryKey regkeyRUN = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                regkeyRUN.SetValue(ThisAppName, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"", Microsoft.Win32.RegistryValueKind.String);
            }
        }

        private bool IsAltDown()
        {
            return ((ModifierKeys & Keys.Alt) == Keys.Alt);
        }

        private bool IsAltShiftDown()
        {
            return (ModifierKeys & (Keys.Alt | Keys.Shift)) == (Keys.Alt | Keys.Shift);
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int CtrlMoveInterval = 100;
            if (IsAltShiftDown()) CtrlMoveInterval = 10;

            if (e.KeyCode == Keys.Escape)
            {
                if (textBox1.Text.Length > 0) textBox1.Text = "";
                else this.Hide();
            }
            else if (e.KeyCode == Keys.Down && (IsAltDown() || IsAltShiftDown()))
                MoveWindowDownByInterval(CtrlMoveInterval);
            else if (e.KeyCode == Keys.Up && (IsAltDown() || IsAltShiftDown()))
                MoveWindowUpByInterval(CtrlMoveInterval);
            else if (e.KeyCode == Keys.Left && (IsAltDown() || IsAltShiftDown()))
                MoveWindowLeftByInterval(CtrlMoveInterval);
            else if (e.KeyCode == Keys.Right && (IsAltDown() || IsAltShiftDown()))
                MoveWindowRightByInterval(CtrlMoveInterval);
        }

        private void MoveWindowRightByInterval(int CtrlMoveInterval)
        {
            if (this.Location.X + CtrlMoveInterval <= Screen.GetWorkingArea(this.Location).Right - this.Width)
                this.Location = new Point(this.Location.X + CtrlMoveInterval, this.Location.Y);
            else
                this.Location = new Point(Screen.GetWorkingArea(this.Location).Right - this.Width, this.Location.Y);
        }

        private void MoveWindowLeftByInterval(int CtrlMoveInterval)
        {
            if (this.Location.X - CtrlMoveInterval >= 0)
                this.Location = new Point(this.Location.X - CtrlMoveInterval, this.Location.Y);
            else
                this.Location = new Point(0, this.Location.Y);
        }

        private void MoveWindowUpByInterval(int CtrlMoveInterval)
        {
            if (this.Location.Y - CtrlMoveInterval >= 0)
                this.Location = new Point(this.Location.X, this.Location.Y - CtrlMoveInterval);
            else
                this.Location = new Point(this.Location.X, 0);
        }

        private void MoveWindowDownByInterval(int CtrlMoveInterval)
        {
            if (this.Location.Y + CtrlMoveInterval <= Screen.GetWorkingArea(this.Location).Bottom - this.Height)
                this.Location = new Point(this.Location.X, this.Location.Y + CtrlMoveInterval);
            else
                this.Location = new Point(this.Location.X, Screen.GetWorkingArea(this.Location).Bottom - this.Height);
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!RegisterHotKey(this.Handle, Id, MOD_CONTROL, (int)Keys.Q))
                MessageBox.Show("Could not register hotkey");
            label1.Text = AvailableActionList;
            SetAutocompleteActionList();
            InitializeHooks(false, true);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                ToggleWindowActivation();
            }
            base.WndProc(ref m);
        }

        private void ToggleWindowActivation()
        {
            if (GetForegroundWindow() != this.Handle)
            {
                this.Visible = true;
                this.Activate();
                this.WindowState = FormWindowState.Normal;
                //textBox1.Focus();
            }
            else this.Hide();
        }

        void SetAutocompleteActionList()
        {
            textBox1.AutoCompleteCustomSource.Clear();
            textBox1.AutoCompleteCustomSource.AddRange(new string[] {
                "todo ",
                "run ",
                "mail ",
                "explore ",
                "web ",
                "google ",
                "kill "
            });
        }

        void SetAutoCompleteRun()
        {
            textBox1.AutoCompleteCustomSource.Clear();
            textBox1.AutoCompleteCustomSource.AddRange(new string[] {
                "run chrome",
                "run delphi2010",
                "run delphi2007",
                "run phpstorm"
            });
        }

        UserActivityHook actHook;
        private void InitializeHooks(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            actHook = new UserActivityHook(InstallMouseHook, InstallKeyboardHook);
            actHook.OnMouseActivity += new MouseEventHandler(actHook_OnMouseActivity);
            actHook.KeyDown += new KeyEventHandler(actHook_KeyDown);
            actHook.KeyPress += new KeyPressEventHandler(actHook_KeyPress);
            actHook.KeyUp += new KeyEventHandler(actHook_KeyUp);
        }

        void actHook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;
                case MouseButtons.Middle:
                    //else notifyIcon1.ShowBalloonTip(300, "Middle", activeTitle, ToolTipIcon.Info);
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
        }

        void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        void actHook_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        void actHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
            }
        }

        private void updateStatusText(string str)
        {
            label1.Text = str;
        }

        private void appendLogTextbox(string str, bool mustUpdateStatusText = false)
        {
            //label1.Text = str;
            textBox_Messages.Text = str + (textBox_Messages.Text.Length > 0 ? Environment.NewLine : "") + textBox_Messages.Text;
            if (mustUpdateStatusText) updateStatusText(str);
        }

        /// <summary>
        /// Post data to php, maximum length of data is 8Mb
        /// </summary>
        /// <param name="url">The url of the php, do not include the ?</param>
        /// <param name="data">The data, i.e. "name=koos&surname=koekemoer". Note to not include the ?</param>
        /// <returns>Returns the data received from the php (usually the "echo" statements in the php.</returns>
        public string PostPHP(string url, string data)
        {
            string vystup = "";
            try
            {
                data = data.Replace("+", "[|]");
                //Our postvars
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                //Initialisation, we use localhost, change if appliable
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
                //Our method is post, otherwise the buffer (postvars) would be useless
                WebReq.Method = "POST";
                //We use form contentType, for the postvars.
                WebReq.ContentType = "application/x-www-form-urlencoded";
                //The length of the buffer (postvars) is used as contentlength.
                WebReq.ContentLength = buffer.Length;
                //We open a stream for writing the postvars
                Stream PostData = WebReq.GetRequestStream();
                //Now we write, and afterwards, we close. Closing is always important!
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                //Get the response handle, we have no true response yet!
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                //Let's show some information about the response
                //System.Windows.Forms.MessageBox.Show(WebResp.StatusCode.ToString());
                //System.Windows.Forms.MessageBox.Show(WebResp.Server);

                //Now, we read the response (the string), and output it.
                Stream Answer = WebResp.GetResponseStream();
                StreamReader _Answer = new StreamReader(Answer);
                vystup = _Answer.ReadToEnd();

                //Congratulations, you just requested your first POST page, you
                //can now start logging into most login forms, with your application
                //Or other examples.
                string tmpresult = vystup.Trim() + "\n";
            }
            catch (Exception exc)
            {
                if (!exc.Message.ToUpper().StartsWith("The remote name could not be resolved:".ToUpper()))
                    //LoggingClass.AddToLogList(UserMessages.MessageTypes.PostPHP, exc.Message);
                    appendLogTextbox("Post php: " + exc.Message);
                else //LoggingClass.AddToLogList(UserMessages.MessageTypes.PostPHPremotename, exc.Message);
                    appendLogTextbox("Post php remote name: " + exc.Message);
                //SysWinForms.MessageBox.Show("Error (092892): " + Environment.NewLine + exc.Message, "Exception error", SysWinForms.MessageBoxButtons.OK, SysWinForms.MessageBoxIcon.Error);
            }
            return vystup;
        }

        public void PerformVoidFunctionSeperateThread(MethodInvoker method)
        {
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                method.Invoke();
            });
            th.Start();
            //th.Join();
            while (th.IsAlive) { Application.DoEvents(); }
        }

        private string GetPrivateKey()
        {
            try
            {
                //toolStripStatusLabelCurrentStatus.Text = "Obtaining pvt key...";
                string tmpkey = null;

                PerformVoidFunctionSeperateThread(() =>
                {
                    tmpkey = PostPHP(ServerAddress + "/generateprivatekey.php", "username=" + Username + "&password=" + Password);
                });

                string tmpSuccessKeyString = "Success: Key=";
                if (tmpkey != null && tmpkey.Length > 0 && tmpkey.ToUpper().StartsWith(tmpSuccessKeyString.ToUpper()))
                {
                    tmpkey = tmpkey.Substring(tmpSuccessKeyString.Length).Replace("\n", "").Replace("\r", "");
                    //toolStripStatusLabelCurrentStatus.Text = tmpkey;
                }
                return tmpkey;
            }
            catch (Exception exc)
            {
                appendLogTextbox("Obtain private key exception: " + exc.Message);
                return null;
            }
        }

        private bool PerformDesktopAppDoTask(string UsernameIn, string TaskName, List<string> ArgumentList, bool CheckForSpecificResult = false, string SuccessSpecificResult = "", bool MustWriteResultToLogsTextbox = false)
        {
            string result = GetResultOfPerformingDesktopAppDoTask(UsernameIn, TaskName, ArgumentList, MustWriteResultToLogsTextbox);
            if (CheckForSpecificResult && result == SuccessSpecificResult)
                return true;
            return false;
        }

        private string GetResultOfPerformingDesktopAppDoTask(string UsernameIn, string TaskName, List<string> ArgumentList, bool MustWriteResultToLogsTextbox = false)
        {
            string tmpkey = GetPrivateKey();
            appendLogTextbox("Obtained private key");

            if (tmpkey != null)
            {
                HttpWebRequest addrequest = null;
                HttpWebResponse addresponse = null;
                StreamReader input = null;

                try
                {
                    if (UsernameIn != null && UsernameIn.Length > 0
                                           && tmpkey != null && tmpkey.Length > 0)
                    {
                        string encryptedstring;
                        string decryptedstring = "";
                        bool mustreturn = false;
                        PerformVoidFunctionSeperateThread(() =>
                        {
                            string ArgumentListTabSeperated = "";
                            foreach (string s in ArgumentList)
                                ArgumentListTabSeperated += (ArgumentListTabSeperated.Length > 0 ? "\t" : "") + s;

                            string tmpRequest = doWorkAddress + "/dotask/" +
                                PhpEncryption.SimpleTripleDesEncrypt(UsernameIn, "123456789abcdefghijklmno") + "/" +
                                PhpEncryption.SimpleTripleDesEncrypt(TaskName, tmpkey) + "/" +
                                PhpEncryption.SimpleTripleDesEncrypt(ArgumentListTabSeperated, tmpkey);
                            addrequest = (HttpWebRequest)WebRequest.Create(tmpRequest);// + "/");
                            //appendLogTextbox(addrequest.RequestUri.ToString());
                            try
                            {
                                addresponse = (HttpWebResponse)addrequest.GetResponse();
                                input = new StreamReader(addresponse.GetResponseStream());
                                encryptedstring = input.ReadToEnd();
                                //appendLogTextbox("Encrypted response: " + encryptedstring);

                                decryptedstring = PhpEncryption.SimpleTripleDesDecrypt(encryptedstring, tmpkey);
                                //appendLogTextbox("Decrypted response: " + decryptedstring);
                                decryptedstring = decryptedstring.Replace("\0", "").Trim();
                                //MessageBox.Show(this, decryptedstring);
                                if (MustWriteResultToLogsTextbox) appendLogTextbox("Result for " + TaskName + ": " + decryptedstring);
                                mustreturn = true;
                            }
                            catch (Exception exc) { MessageBox.Show(this, "Exception:" + exc.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        });
                        if (mustreturn) return decryptedstring;
                    }
                }
                catch (Exception exc)
                {
                    appendLogTextbox("Obtain php: " + exc.Message);
                }
                finally
                {
                    if (addresponse != null) addresponse.Close();
                    if (input != null) input.Close();
                }
            }
            return null;
        }

        private void AddTodoItemNow(string Category, string Subcat, string Items, string Description, bool Completed, DateTime Due, DateTime Created, int RemindedCount, bool StopSnooze, int AutosnoozeInterval)
        {
            appendLogTextbox("Adding new item, please wait...");
            bool successfulAdd = PerformDesktopAppDoTask(
                Username,
                "addtolist",
                new List<string>()
                {
                    Category,
                    Subcat,
                    Items,
                    Description,
                    Due.ToString(MySQLdateformat),
                    StopSnooze ? "1" : "0",
                    AutosnoozeInterval.ToString()
                },
                true,
                "1");
            if (successfulAdd)
            {
                appendLogTextbox("Successfully added todo item.");
                textBox1.Text = "";
            }
        }

        private void CreateNewOutlookMessage(string To, string Subject, string Body)
        {
            if (Process.GetProcessesByName("Outlook").Length == 0)
            {
                appendLogTextbox("Starting Outlook, please wait...");
                Process p = System.Diagnostics.Process.Start("Outlook");
            }

            // Creates a new Outlook Application Instance
            Outlook.Application objOutlook = new Outlook.Application();
            // Creating a new Outlook Message from the Outlook Application Instance
            Outlook.MailItem mic = (Outlook.MailItem)(objOutlook.CreateItem(Outlook.OlItemType.olMailItem));
            mic.To = To;
            mic.Subject = Subject;
            mic.Body = Body;
            this.TopMost = false;
            mic.Display(true);
            this.TopMost = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.ToUpper().StartsWith("TODO"))
            {
                label1.Text = "todo MinutesFromNow;Autosnooze;Item name;Description";
            }
            else if (textBox1.Text.ToUpper().StartsWith("RUN"))
            {
                label1.Text = "run chrome/canary/delphi2010/delphi2007/phpstorm";
                if (textBox1.Text.ToUpper() == "RUN") SetAutoCompleteRun();
            }
            else if (textBox1.Text.ToUpper().StartsWith("MAIL"))
            {
                label1.Text = "mail to;subject;body";
            }
            else if (textBox1.Text.ToUpper().StartsWith("EXPLORE"))
            {
                label1.Text = "explore franother/prog/docs";
            }
            else if (textBox1.Text.ToUpper().StartsWith("WEB"))
            {
                label1.Text = "web google.com";
            }
            else if (textBox1.Text.ToUpper().StartsWith("GOOGLE"))
            {
                label1.Text = "google search on google";
            }
            else if (textBox1.Text.ToUpper().StartsWith("KILL"))
            {
                label1.Text = "kill processNameToKill";
            }
            else
                label1.Text = AvailableActionList;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, Id);
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            notifyIcon1.Visible = !this.Visible;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text.ToUpper().StartsWith("TODO ") && textBox1.Text.Length >= 12 && textBox1.Text.Split(';').Length == 4)
                {
                    string tmpstr = textBox1.Text.Substring(5);
                    AddTodoItemNow(
                        "QuickAccess",
                        "Quick todo",
                        tmpstr.Split(';')[2],
                        tmpstr.Split(';')[3],
                        false,
                        DateTime.Now.AddMinutes(Convert.ToInt32(tmpstr.Split(';')[0])),
                        DateTime.Now,
                        0,
                        false,
                        Convert.ToInt32(tmpstr.Split(';')[1]));
                }
                else if (textBox1.Text.ToUpper().StartsWith("RUN ") && textBox1.Text.Length >= 5)
                {
                    string appname = textBox1.Text.Substring(4).ToLower();
                    string exepath = "";
                    if (appname == "chrome")
                        exepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                    else if (appname == "canary")
                        exepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome SxS\Application\chrome.exe";
                    else if (appname == "delphi2007")
                        exepath = @"C:\Program Files (x86)\CodeGear\RAD Studio\5.0\bin\bds.exe";
                    else if (appname == "delphi2010")
                        exepath = @"C:\Program Files (x86)\Embarcadero\RAD Studio\7.0\bin\bds.exe";
                    else if (appname == "phpstorm")
                        exepath = @"C:\Program Files (x86)\JetBrains\PhpStorm 2.1.4\bin\PhpStorm.exe";

                    if (File.Exists(exepath))
                        System.Diagnostics.Process.Start(exepath);
                    else
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(appname);
                        }
                        catch (Exception exc) { appendLogTextbox(exc.Message); }
                    }
                }
                else if (textBox1.Text.ToUpper().StartsWith("MAIL ") && textBox1.Text.Length >= 15 && textBox1.Text.Split(';').Length == 3)
                {
                    string tmpstr = textBox1.Text.Substring(5);
                    CreateNewOutlookMessage(
                        tmpstr.Split(';')[0],
                        tmpstr.Split(';')[1],
                        tmpstr.Split(';')[2]);
                }
                else if (textBox1.Text.ToUpper().StartsWith("EXPLORE ") && textBox1.Text.Length >= 9)
                {
                    string explorepath = textBox1.Text.Substring(8).ToLower();
                    if (explorepath == "franother")
                        System.Diagnostics.Process.Start(@"c:\francois\other");
                    else if (explorepath == "prog")
                        System.Diagnostics.Process.Start(@"c:\programming");
                    else if (explorepath == "docs")
                        System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    else if (Directory.Exists(explorepath))
                        System.Diagnostics.Process.Start(explorepath);
                    else
                        appendLogTextbox("Unrecognized command to explore, and directory not found: " + explorepath);
                }
                else if (textBox1.Text.ToUpper().StartsWith("WEB ") && textBox1.Text.Length >= 7)
                {
                    string url = textBox1.Text.Substring(4).ToLower();
                    if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("www."))
                        url = "http://" + url;
                    System.Diagnostics.Process.Start(url);
                }
                else if (textBox1.Text.ToUpper().StartsWith("GOOGLE ") && textBox1.Text.Length >= 8)
                {
                    string searchtxt = textBox1.Text.Substring(7);
                    System.Diagnostics.Process.Start("http://www.google.co.za/search?q=" + searchtxt);
                }
                else if (textBox1.Text.ToUpper().StartsWith("KILL ") && textBox1.Text.Length >= 6)
                {
                    string processName = textBox1.Text.Substring(5);
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length > 1) appendLogTextbox("More than one process found, cannot kill");
                    else if (processes.Length == 0) appendLogTextbox("Cannot find process with name " + processName);
                    else
                    {
                        processes[0].Kill();
                        appendLogTextbox("Process killed: " + processName);
                    }
                }

                textBox1.Select(textBox1.Text.Length, 0);
            }
        }
    }

    public class PhpEncryption
    {
        public static string SimpleTripleDesEncrypt(string Data, string keystring)
        {
            byte[] key = Encoding.ASCII.GetBytes(keystring);
            byte[] iv = Encoding.ASCII.GetBytes("password");
            byte[] data = Encoding.ASCII.GetBytes(Data);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateEncryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return ByteArrayToString(enc);
        }

        public static string SimpleTripleDesDecrypt(string Data, string keystring)
        {
            byte[] key = Encoding.ASCII.GetBytes(keystring);
            byte[] iv = Encoding.ASCII.GetBytes("password");
            byte[] data = StringToByteArray(Data);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateDecryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return Encoding.ASCII.GetString(enc);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static string StringToHex(string stringIn)
        {
            return PhpEncryption.ByteArrayToString(Encoding.Default.GetBytes(stringIn));
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string HexToString(string hexIn)
        {
            return Encoding.Default.GetString(StringToByteArray(hexIn));
        }
    }
}