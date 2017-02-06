using BackendlessAPI;
using License;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Register
{
    /// <summary>
    /// Interaction logic for AlreadyRegisteredUI.xaml
    /// </summary>
    internal partial class AlreadyRegisteredUI : Window
    {
        internal AlreadyRegisteredUI()
        {
            InitializeComponent();
            GetUserInfo.ConnectApp();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (GetUserInfo.IsConnectedToInternet())
            {
                try
                {
                    bool emailHasAt = txtEmail.Text.Contains("@");
                    bool emailHasDot = txtEmail.Text.Contains(".");

                    if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !string.IsNullOrWhiteSpace(txtPass.Password)
                        && emailHasAt && emailHasDot)
                    {
                        //Online Login
                        Backendless.UserService.Login(txtEmail.Text, txtPass.Password);

                        string licenseFile = RoofGenerator.RoofGenerator.RoofStorage.licenseFile;
                        UserInfo currentUserInfo = new UserInfo(txtEmail.Text);

                        if (File.Exists(licenseFile))
                        {
                            File.Delete(licenseFile);
                        }

                        if (!File.Exists(licenseFile))
                        {
                            GetUserInfo.RegisterPc();
                            GetUserInfo.UserOffLineLogin();
                        }

                        //This commented code avoids saving the same computer twice on the database, but i removed it since its a good way to keep track of the instalations even on the same pc
                        //IList<UserInfo> allComputers = BackendlessAPI.Backendless.Data.Of<UserInfo>().Find().Data;
                        //bool hasCurrentComputerRegistered = false;

                        //foreach (UserInfo currentComputerInDataBaseInfo in allComputers)
                        //{
                        //    bool isDbVersionEqual = currentComputerInDataBaseInfo.AppVersion == currentUserInfo.AppVersion ? true : false;
                        //    bool isDbProcessorEqual = currentComputerInDataBaseInfo.ProcessorId == currentUserInfo.ProcessorId ? true : false;
                        //    bool isDbMotherBEqual = currentComputerInDataBaseInfo.MotherboardId == currentUserInfo.MotherboardId ? true : false;
                        //    bool isDbOSEqual = currentComputerInDataBaseInfo.OS == currentUserInfo.OS ? true : false;

                        //    if (isDbVersionEqual && isDbProcessorEqual && isDbMotherBEqual && isDbOSEqual)
                        //    {
                        //        hasCurrentComputerRegistered = true;
                        //    }
                        //}

                        //if (!hasCurrentComputerRegistered)
                        //{
                            Backendless.Persistence.Save(currentUserInfo);
                        //}

                        Backendless.UserService.Logout();

                        if (RoofGenerator.RoofGenerator.RoofStorage.isRegister)
                        {
                            this.DialogResult = true;
                            MessageBox.Show("Thank you for using OnBIM e ONBOX apps.", "Sucesso!", MessageBoxButton.OK, MessageBoxImage.Information);
                            AboutUI aboutUI = new AboutUI();
                            aboutUI.ShowDialog();
                        }
                    }
                    else
                    {
                        string errors = "Some errors occurred:";
                        if (string.IsNullOrWhiteSpace(txtEmail.Text))
                        {
                            errors += "\n - E-mail must me filled.";
                        }
                        if (!emailHasAt || !emailHasDot)
                        {
                            errors += "\n - Invalid E-mail.";
                        }
                        if (string.IsNullOrWhiteSpace(txtPass.Password))
                        {
                            errors += "\n - Password must be filled.";
                        }

                        MessageBox.Show(errors);
                    }

                }
                catch (Exception excep)
                {
                    if (excep is BackendlessAPI.Exception.BackendlessException)
                    {
                        var backExcep = excep as BackendlessAPI.Exception.BackendlessException;
                        string customInfo = "";
                        //bool isJustWarn = true;

                        switch (backExcep.FaultCode)
                        {
                            case "2002":
                                customInfo = "This app version is disabled, please contact support or update this version.";
                                break;
                            case "3000":
                                customInfo = "This user is unable to use this app.";
                                break;
                            case "3001":
                                customInfo = "Missing information.";
                                break;
                            case "3002":
                                customInfo = "It is not possible to activate multiple licenses at the same time.";
                                break;
                            case "3003":
                                customInfo = "Invalid E-mail and password.";
                                break;
                            case "3006":
                                customInfo = "E-mail and password must be filled.";
                                break;
                            case "3007":
                                customInfo = "Invalid app and version.";
                                break;
                            case "3023":
                                customInfo = "Error on the application data, but the user was registered.";
                                break;
                            case "3034":
                                customInfo = "User register is disabled for this app version.";
                                break;
                            case "3036":
                                customInfo = "Multiple failed access. Account disabled.";
                                break;
                            case "3038":
                                customInfo = "Error occurred while trying to communicate with the server.";
                                //isJustWarn = false;
                                break;
                            case "3044":
                                customInfo = "Multiple register for the same user.";
                                break;
                            case "3087":
                                customInfo = "E-mail confirmation required. Please confirm your E-mail: " + txtEmail.Text + " and try to login again.";
                                break;
                            case "8000":
                                customInfo = "Value exceeds the limit.";
                                break;
                            default:
                                customInfo = "Server error. Please try again later.";
                                break;
                        }

                        customInfo += " please, if you have any questions, contact onbim@onboxdesign.com.br for more information.";

                        MessageBox.Show(excep + "\n" + customInfo + "\n" , "Register error.");
                    }
                    else
                    {
                        MessageBox.Show(excep + "\n", "Register error.");
                    }
                }
        }
            else
            {
                MessageBox.Show("Please, connect to the internet.", "No connection", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}

        private void lblNewRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LicenseUI currentLicenseUI = new LicenseUI();
            this.DialogResult = true;
            currentLicenseUI.ShowDialog();
        }

        private void lblRecoverPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool emailHasAt = txtEmail.Text.Contains("@");
                bool emailHasDot = txtEmail.Text.Contains(".");

                if (!string.IsNullOrWhiteSpace(txtEmail.Text) && emailHasAt && emailHasDot)
                {
                    Backendless.UserService.RestorePassword(txtEmail.Text);
                }
                else
                {
                    string errors = "Following errors were found:";
                    if (string.IsNullOrWhiteSpace(txtEmail.Text))
                    {
                        errors += "\n - Email must be filled to recover password.";
                    }
                    if (!emailHasAt || !emailHasDot)
                    {
                        errors += "\n - Invalid E-mail.";
                    }

                    MessageBox.Show(errors);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server or E-mail error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFacebook_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

    }
}
