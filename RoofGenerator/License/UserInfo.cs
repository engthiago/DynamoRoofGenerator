using Autodesk.Revit.UI;
using DRGeneratorBackendless;
using Register;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;

namespace License
{

    class Hash
    {
        public static string ComputeHash(string plainText, byte[] salt)
        {
            int minSaltLength = 4, maxSaltLength = 16;

            byte[] saltBytes = null;
            if (salt != null)
            {
                saltBytes = salt;
            }
            else
            {
                Random r = new Random();
                int SaltLength = r.Next(minSaltLength, maxSaltLength);
                saltBytes = new byte[SaltLength];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetNonZeroBytes(saltBytes);
                rng.Dispose();
            }

            byte[] plainData = ASCIIEncoding.UTF8.GetBytes(plainText);
            byte[] plainDataWithSalt = new byte[plainData.Length + saltBytes.Length];

            for (int x = 0; x < plainData.Length; x++)
                plainDataWithSalt[x] = plainData[x];
            for (int n = 0; n < saltBytes.Length; n++)
                plainDataWithSalt[plainData.Length + n] = saltBytes[n];

            byte[] hashValue = null;

            SHA256Managed sha = new SHA256Managed();
            hashValue = sha.ComputeHash(plainDataWithSalt);
            sha.Dispose();


            byte[] result = new byte[hashValue.Length + saltBytes.Length];
            for (int x = 0; x < hashValue.Length; x++)
                result[x] = hashValue[x];
            for (int n = 0; n < saltBytes.Length; n++)
                result[hashValue.Length + n] = saltBytes[n];

            return Convert.ToBase64String(result);
        }

        public static string ComputeHash(string plainText)
        {
            byte[] plainData = ASCIIEncoding.UTF8.GetBytes(plainText);
            byte[] hashValue = null;

            SHA256Managed sha = new SHA256Managed();
            hashValue = sha.ComputeHash(plainData);
            sha.Dispose();

            byte[] result = new byte[hashValue.Length];
            for (int x = 0; x < hashValue.Length; x++)
                result[x] = hashValue[x];

            return Convert.ToBase64String(result);
        }

    }

    class UserInfo
    {
        public string Machine { get; set; }

        public string User { get; set; }

        public string OS { get; set; }

        public string ProcessorId { get; set; }

        public string ProcessorCount { get; set; }

        public string MotherboardId { get; set; }

        public string PhysicalMemory { get; set; }

        public string UserEmail { get; set; }

        public string RevitVersion { get; set; }

        public string RevitVersionName { get; set; }

        public string RevitVersionBuid { get; set; }

        public string RevitLanguage { get; set; }

        public string AppVersion { get; set; }

        public DateTime created { get; set; }

        public DateTime updated { get; set; }

        private string objectId { get; set; }

        private void GetData()
        {

            Machine = Environment.MachineName;
            User = Environment.UserName;
            OS = new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName;
            ProcessorCount = Environment.ProcessorCount.ToString();
            PhysicalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory.ToString();
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            string sQuery = "SELECT ProcessorId FROM Win32_Processor";
            ManagementObjectSearcher oManagementObjectSearcher = new ManagementObjectSearcher(sQuery);
            ManagementObjectCollection oCollection = oManagementObjectSearcher.Get();
            foreach (ManagementObject oManagementObject in oCollection)
            {
                ProcessorId = (string)oManagementObject["ProcessorId"];
                break;
            }

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            foreach (ManagementObject mo in moc)
            {
                MotherboardId = (string)mo["SerialNumber"];
                break;
            }
        }

        public UserInfo(string targetUser)
        {
            GetData();
            UserEmail = targetUser;

            Autodesk.Revit.ApplicationServices.Application revitApp = RoofGenerator.RoofGenerator.RoofStorage.revitApp;

            RevitVersion = revitApp.VersionNumber;
            RevitVersionName = revitApp.VersionName;
            RevitVersionBuid = revitApp.VersionBuild;
            RevitLanguage = revitApp.Language.ToString();
        }

        public UserInfo()
        {
            GetData();
        }
    }

    #region AdClicks
    //class AdsClicks
    //{
    //    public string User { get; }
    //    public string ProcessorId { get; }
    //    public string MotherboardId { get; }
    //    public string AppVersion { get; }
    //    public string Ad { get; set; }

    //    public AdsClicks()
    //    {
    //        User = Environment.UserName;
    //        AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

    //        string sQuery = "SELECT ProcessorId FROM Win32_Processor";
    //        ManagementObjectSearcher oManagementObjectSearcher = new ManagementObjectSearcher(sQuery);
    //        ManagementObjectCollection oCollection = oManagementObjectSearcher.Get();
    //        foreach (ManagementObject oManagementObject in oCollection)
    //        {
    //            ProcessorId = (string)oManagementObject["ProcessorId"];
    //            break;
    //        }
    //        ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
    //        ManagementObjectCollection moc = mos.Get();
    //        foreach (ManagementObject mo in moc)
    //        {
    //            MotherboardId = (string)mo["SerialNumber"];
    //            break;
    //        }
    //    }
    //} 



    //class ButtonInfo
    //{
    //    private string name;
    //    public string Name
    //    {
    //        get { return name; }
    //        set { name = value; }
    //    }

    //    private string toolTip;
    //    public string ToolTip
    //    {
    //        get { return toolTip; }
    //        set { toolTip = value; }
    //    }

    //    private string longDescription;
    //    public string LongDescription
    //    {
    //        get { return longDescription; }
    //        set { longDescription = value; }
    //    }

    //    private BitmapImage newImage;
    //    public BitmapImage NewImage
    //    {
    //        get { return newImage; }
    //        set { newImage = value; }
    //    }

    //    private BitmapImage largeImage;
    //    public BitmapImage LargeImage
    //    {
    //        get { return largeImage; }
    //        set { largeImage = value; }
    //    }

    //    private BitmapImage image;
    //    public BitmapImage Image
    //    {
    //        get { return image; }
    //        set { image = value; }
    //    }

    //    private string link;
    //    public string Link
    //    {
    //        get { return link; }
    //        set { link = value; }
    //    }

    //    public ButtonInfo()
    //    {
    //        Name = "";
    //        ToolTip = "";
    //        LongDescription = "";
    //        NewImage = null;
    //        LargeImage = null;
    //        Image = null;
    //        Link = "";
    //    }
    //}
    #endregion


    static internal class GetUserInfo
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        static internal Result CheckLicense()
        {
            if (RoofGenerator.RoofGenerator.RoofStorage.isRegister)
            {
                return Result.Succeeded;
            }
            else
            {
                UserOffLineLogin();
                if (RoofGenerator.RoofGenerator.RoofStorage.isRegister)
                {
                    return Result.Succeeded;
                }
                else
                {
                    AlreadyRegisteredUI loginUI = new AlreadyRegisteredUI();
                    loginUI.ShowDialog();

                    if (RoofGenerator.RoofGenerator.RoofStorage.isRegister)
                    {
                        return Result.Succeeded;
                    }
                    else
                    {
                        return Result.Failed;
                    }
                }
            }
        }

        static internal void UserOffLineLogin()
        {
            License.UserInfo currentUserInfo = new License.UserInfo();

            if (File.Exists(RoofGenerator.RoofGenerator.RoofStorage.licenseFile))
            {
                using (StreamReader reader = new StreamReader(RoofGenerator.RoofGenerator.RoofStorage.licenseFile))
                {
                    string licenseInfo = reader.ReadLine();
                    string currentInfo = License.Hash.ComputeHash(currentUserInfo.ProcessorId + currentUserInfo.MotherboardId + RoofGenerator.RoofGenerator.RoofStorage.roofGeneratorVersion);

                    if (licenseInfo == currentInfo)
                    {
                        RoofGenerator.RoofGenerator.RoofStorage.isRegister = true;
                    }
                }
            }
        }

        static internal void RegisterPc()
        {
            string licenseFile = RoofGenerator.RoofGenerator.RoofStorage.licenseFile;
            UserInfo currentUserInfo = new UserInfo();

            string info = Hash.ComputeHash(currentUserInfo.ProcessorId + currentUserInfo.MotherboardId + RoofGenerator.RoofGenerator.RoofStorage.roofGeneratorVersion);
            string path = RoofGenerator.RoofGenerator.RoofStorage.licensePath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter writer = new StreamWriter(licenseFile))
            {
                writer.Write(info);
                writer.Close();
            }
            File.SetAttributes(path, File.GetAttributes(licenseFile) | FileAttributes.Hidden);
        }

        static internal bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        static internal void ConnectApp()
        {
            string appId = "6947CC83-EF90-E9F3-FF92-5D32E497A000";
            string secretKey = "FD83FEF2-147C-A978-FFD9-3D75599E5300";
            Backendless.InitApp(appId, secretKey);
        }

        #region UpdateAndAds commented
        //static internal void UpdateAndAds(out ButtonInfo updateButtonInfo, out IList<ButtonInfo> adsButtonsInfo)
        //{

        //    updateButtonInfo = new ButtonInfo();
        //    adsButtonsInfo = new List<ButtonInfo>();

        //    if (IsConnectedToInternet())
        //    {
        //        try
        //        {
        //            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //            double appVersionNumber = 0;
        //            XmlDocument onlineAppInfo = new XmlDocument();
        //            onlineAppInfo.Load("http://www.onboxdesign.com.br/OnboxApp/OnboxAppInfo.xml");
        //            XmlNode updateInfo = onlineAppInfo.SelectSingleNode("SoftwareInfo/Update");
        //            string appOnlineVersion = updateInfo.SelectSingleNode("Version").InnerText;
        //            double appOnlineVersionNumber = 0;

        //            if (appVersion != "" && appOnlineVersion != "")
        //            {
        //                appVersion = appVersion.Replace(".", "");
        //                appOnlineVersion = appOnlineVersion.Replace(".", "");
        //            }

        //            if (double.TryParse(appVersion, out appVersionNumber) && (double.TryParse(appOnlineVersion, out appOnlineVersionNumber))
        //                && updateInfo != null)
        //            {
        //                if (appOnlineVersionNumber > appVersionNumber)
        //                {
        //                    string buttonName = "  " + updateInfo.SelectSingleNode("Name").InnerText + "  ";
        //                    updateButtonInfo.Name = buttonName;

        //                    XmlNode nodeToolTip = updateInfo.SelectSingleNode("ToolTip");
        //                    if (nodeToolTip != null)
        //                    {
        //                        string toolTip = nodeToolTip.InnerText;
        //                        updateButtonInfo.ToolTip = toolTip;
        //                    }

        //                    XmlNode nodeDescription = updateInfo.SelectSingleNode("LongDescription");
        //                    if (nodeDescription != null)
        //                    {
        //                        string description = nodeDescription.InnerText;
        //                        updateButtonInfo.LongDescription = description;
        //                    }

        //                    XmlNode nodeLink = updateInfo.SelectSingleNode("Link");
        //                    if (nodeLink != null)
        //                    {
        //                        string Link = nodeLink.InnerText;
        //                        if (Link != "")
        //                            updateButtonInfo.Link = Link;
        //                        else
        //                            updateButtonInfo.Link = "http://www.onboxdesign.com.br/onbox-app/";
        //                    }
        //                    else
        //                        updateButtonInfo.Link = "http://www.onboxdesign.com.br/onbox-app/";

        //                    XmlNode nodeLinkNew32 = updateInfo.SelectSingleNode("ImageLinkNew32");
        //                    if (nodeLinkNew32 != null)
        //                    {
        //                        string imageLinkNew32 = nodeLinkNew32.InnerText;
        //                        if (imageLinkNew32 != "")
        //                        {
        //                            updateButtonInfo.NewImage = new BitmapImage(new Uri(imageLinkNew32));
        //                        }
        //                        else
        //                        {
        //                            updateButtonInfo.NewImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox32.png", UriKind.Absolute));
        //                        }
        //                    }
        //                    else
        //                        updateButtonInfo.NewImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox32.png", UriKind.Absolute));

        //                    XmlNode nodeLink32 = updateInfo.SelectSingleNode("ImageLink32");
        //                    if (nodeLink32 != null)
        //                    {
        //                        string imageLink32 = nodeLink32.InnerText;
        //                        if (imageLink32 != "")
        //                            updateButtonInfo.LargeImage = new BitmapImage(new Uri(imageLink32));
        //                        else
        //                            updateButtonInfo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox32.png", UriKind.Absolute));
        //                    }
        //                    else
        //                        updateButtonInfo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox32.png", UriKind.Absolute));

        //                    XmlNode nodeLink16 = updateInfo.SelectSingleNode("ImageLink16");
        //                    if (nodeLink16 != null)
        //                    {
        //                        string imageLink16 = nodeLink16.InnerText;
        //                        if (imageLink16 != "")
        //                            updateButtonInfo.Image = new BitmapImage(new Uri(imageLink16));
        //                        else
        //                            updateButtonInfo.Image = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox16.png", UriKind.Absolute));
        //                    }
        //                    else
        //                        updateButtonInfo.Image = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/UpdateOnbox16.png", UriKind.Absolute));
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        try
        //        {
        //            XmlDocument onlineAppInfo = new XmlDocument();
        //            onlineAppInfo.Load("http://www.onboxdesign.com.br/OnboxApp/OnboxAppInfo.xml");
        //            var Ads = onlineAppInfo.SelectNodes("SoftwareInfo/Ad");

        //            if (Ads != null)
        //            {
        //                foreach (XmlNode currentAdNode in Ads)
        //                {
        //                    if (currentAdNode != null)
        //                    {
        //                        ButtonInfo currentButtonInfo = new ButtonInfo();
        //                        currentButtonInfo.Name = "  " + currentAdNode.SelectSingleNode("Name").InnerText + "  ";
        //                        currentButtonInfo.ToolTip = currentAdNode.SelectSingleNode("ToolTip").InnerText;
        //                        currentButtonInfo.LongDescription = currentAdNode.SelectSingleNode("LongDescription").InnerText;
        //                        currentButtonInfo.Link = currentAdNode.SelectSingleNode("Link").InnerText;
        //                        WebClient webClient = new WebClient();

        //                        if (!Directory.Exists(ONBOXApp.licensePath + "Images"))
        //                        {
        //                            Directory.CreateDirectory(ONBOXApp.licensePath + "Images");
        //                        }


        //                        if (currentAdNode.SelectSingleNode("ImageLink16") != null)
        //                        {
        //                            string currentImageLink = currentAdNode.SelectSingleNode("ImageLink16").InnerText;
        //                            if (currentImageLink != "")
        //                            {
        //                                string currentImageFileName = Path.GetFileName(currentAdNode.SelectSingleNode("ImageLink16").InnerText);
        //                                string currentImageFileLocal = ONBOXApp.licensePath + "Images\\" + currentImageFileName;
        //                                if (File.Exists(currentImageFileLocal))
        //                                {
        //                                    currentButtonInfo.Image = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                                else
        //                                {
        //                                    webClient.DownloadFile(currentImageLink, currentImageFileLocal);
        //                                    currentButtonInfo.Image = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                            }
        //                            else
        //                            {
        //                                currentButtonInfo.Image = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder16.png", UriKind.Absolute));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            currentButtonInfo.Image = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder16.png", UriKind.Absolute));
        //                        }

        //                        if (currentAdNode.SelectSingleNode("ImageLink32") != null)
        //                        {
        //                            string currentImageLink = currentAdNode.SelectSingleNode("ImageLink32").InnerText;
        //                            if (currentImageLink != "")
        //                            {
        //                                string currentImageFileName = Path.GetFileName(currentAdNode.SelectSingleNode("ImageLink32").InnerText);
        //                                string currentImageFileLocal = ONBOXApp.licensePath + "\\Images\\" + currentImageFileName;
        //                                if (File.Exists(currentImageFileLocal))
        //                                {
        //                                    currentButtonInfo.LargeImage = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                                else
        //                                {
        //                                    webClient.DownloadFile(currentImageLink, currentImageFileLocal);
        //                                    currentButtonInfo.LargeImage = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                            }
        //                            else
        //                            {
        //                                currentButtonInfo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder32.png", UriKind.Absolute));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            currentButtonInfo.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder32.png", UriKind.Absolute));
        //                        }

        //                        if (currentAdNode.SelectSingleNode("ImageLinkNew32") != null)
        //                        {
        //                            string currentImageLink = currentAdNode.SelectSingleNode("ImageLinkNew32").InnerText;
        //                            if (currentImageLink != "")
        //                            {
        //                                string currentImageFileName = Path.GetFileName(currentAdNode.SelectSingleNode("ImageLinkNew32").InnerText);
        //                                string currentImageFileLocal = ONBOXApp.licensePath + "\\Images\\" + currentImageFileName;
        //                                if (File.Exists(currentImageFileLocal))
        //                                {
        //                                    currentButtonInfo.NewImage = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                                else
        //                                {
        //                                    webClient.DownloadFile(currentImageLink, currentImageFileLocal);
        //                                    currentButtonInfo.NewImage = new BitmapImage(new Uri(currentImageFileLocal));
        //                                }
        //                            }
        //                            else
        //                            {
        //                                currentButtonInfo.NewImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder32.png", UriKind.Absolute));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            currentButtonInfo.NewImage = new BitmapImage(new Uri("pack://application:,,,/ONBOXApp;component/Resources/PlaceHolder32.png", UriKind.Absolute));
        //                        }

        //                        adsButtonsInfo.Add(currentButtonInfo);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }
        //} 
        #endregion
    }
}
