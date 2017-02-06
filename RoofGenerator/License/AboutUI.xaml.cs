using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Register
{
    /// <summary>
    /// Interaction logic for AboutUI.xaml
    /// </summary>
    internal partial class AboutUI : Window
    {
        internal AboutUI()
        {
            InitializeComponent();
        }

        private void AboutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder stringInfo = new StringBuilder();
            stringInfo.AppendLine("What is onBIM with ONBOX?");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("onBIM is a company whose vision is to be always ahead");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("Our goal is to provide solutions. Be it in projects, in modeling, compatibilization or even in processes.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("ONBOX is a company active in several areas of technology and digital media. We work with virtual reality for Oculus Rift, Samsung VR and application creation, all aimed at architecture, engineering and construction.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("We create products that help professionals to enhance their workflows in a BIM environment in an intuitive, fast and dynamic way, always guided by brazilians regulations.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("Together, onBIM plus ONBOX are developing various solutions, uniting the potential of C # with Dynamo's flexibility.");

            textInformation.Text = stringInfo.ToString();
            string appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Title = "RoofGenerator by Onbim and ONBOX: " + appVersion;
            lblVersionInfo.Text = appVersion;


            if (License.GetUserInfo.CheckLicense() == Autodesk.Revit.UI.Result.Succeeded)
            {
                lblLicenseInfo.Text = "Licensed.";
                lblLicenseInfo.Foreground = new SolidColorBrush(Color.FromRgb(0, 70, 255));
                btnLicense.Content = "Deactivate license";
                btnLicense.Width = 120;
            }
            else
            {
                lblLicenseInfo.Text = "Not registered.";
                lblLicenseInfo.Foreground = new SolidColorBrush(Colors.Red);
                btnLicense.Content = "Activate license";
                btnLicense.Width = 120;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.onboxdesign.com.br/");
        }

        private void btnLicense_Click(object sender, RoutedEventArgs e)
        {
            if (RoofGenerator.RoofGenerator.RoofStorage.isRegister)
            {
                MessageBoxResult warning = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (warning == MessageBoxResult.OK)
                {
                    try
                    {
                        File.Delete(RoofGenerator.RoofGenerator.RoofStorage.licenseFile);
                    }
                    catch (System.Exception)
                    {
                    }
                    RoofGenerator.RoofGenerator.RoofStorage.isRegister = false;
                    this.DialogResult = true;
                }

            }
            else
            {
                AlreadyRegisteredUI currentLicenseUI = new AlreadyRegisteredUI();
                this.DialogResult = true;
                currentLicenseUI.ShowDialog();
            }
        }

        private void imgFacebook_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/onboxdsgn"); 
        }

        private void imgYoutube_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/user/mrthiagokurumada");
        }

        private void imgOnbox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.onboxdesign.com.br/");
        }

        private void imgMail_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.onboxdesign.com.br/contato/");
        }
    }
}
