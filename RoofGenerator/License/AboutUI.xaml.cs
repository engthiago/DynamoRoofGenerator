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
            stringInfo.AppendLine("O que é a ONBOX?");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("A ONBOX é uma empresa atuante em diversas áreas da tecnologia e mídias digitais. Trabalhamos com realidade virtual para Oculus Rift, Samsung VR e criação de aplicativos, todos voltados para arquitetura, engenharia e construção civil.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("Criamos produtos que auxiliam os profissionais a potencializarem seus fluxos de trabalho em ambiente BIM de uma forma intuitiva, rápida e dinâmica, guiadas sempre pelas normativas ABNT – NBR.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("Desenvolvemos também soluções exclusivas que promovem o marketing de seu empreendimento com potencialidade de vendas pois as interações e nível de realismo que alcançamos impressionam e agradam o usuário final.");
            stringInfo.AppendLine("");
            stringInfo.AppendLine("Fundada à partir do sonho do empreendedor, graduado em Design de Produto e em Engenharia Civil, em desvendar e buscar incessantemente novas tecnologias além de acompanhar e promover avanços na sociedade geek.");
          
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
