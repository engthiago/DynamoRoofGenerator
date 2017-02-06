using BackendlessAPI;
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Security.AccessControl;
using BackendlessAPI.Exception;
using System.Globalization;
using License;

namespace Register
{
    /// <summary>
    /// Interaction logic for LicenseUI.xaml
    /// </summary>
    internal partial class LicenseUI : Window
    {
        
        internal LicenseUI()
        {
            InitializeComponent();
            License.GetUserInfo.ConnectApp();
        }

        private void lblAlreadyRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AlreadyRegisteredUI computerRegisterUI = new AlreadyRegisteredUI();
            this.DialogResult = true;
            computerRegisterUI.ShowDialog();
        }

        private void lblConditions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConditionsUI conditionUI = new ConditionsUI();
            conditionUI.ShowDialog();

            if (conditionUI.DialogResult == true)
            {
                checkAccept.IsChecked = true;
            }else
            {
                checkAccept.IsChecked = false;
            }

        }

        //alternative way to check the connection
        internal static bool ComunicationWithBackEndless()
        {
            System.Uri Url = new System.Uri("https://backendless.com/");

            System.Net.WebRequest WebReq;
            System.Net.WebResponse Resp;
            WebReq = System.Net.WebRequest.Create(Url);

            try
            {
                Resp = WebReq.GetResponse();
                Resp.Close();
                WebReq = null;
                return true;
            }

            catch
            {
                WebReq = null;
                return false;
            }
        }

        private void btnOK_Click_1(object sender, RoutedEventArgs e)
        {
            if (GetUserInfo.IsConnectedToInternet())
            {
                try
                {
                    BackendlessUser user = new BackendlessUser();

                    bool emailHasAt = txtEmail.Text.Contains("@");
                    bool emailHasDot = txtEmail.Text.Contains(".");
                    bool validProfess = comboProfess.SelectedIndex == 0 ? false : true;
                    bool validCountry = comboCountry.SelectedIndex == 0 ? false : true;

                    if (!string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtSurname.Text)
                        && !string.IsNullOrWhiteSpace(txtState.Text) && !string.IsNullOrWhiteSpace(txtCity.Text)
                        && !string.IsNullOrWhiteSpace(txtEmail.Text) && !string.IsNullOrWhiteSpace(txtPass.Password)
                        && txtPass.Password == txtPassConf.Password && txtEmail.Text == txtEmailConf.Text && validProfess
                        && validCountry && emailHasAt && emailHasDot && checkAccept.IsChecked == true)
                    {

                        user.SetProperty("email", txtEmail.Text);
                        user.SetProperty("name", txtName.Text);
                        user.SetProperty("profession", comboProfess.SelectedIndex.ToString());
                        user.SetProperty("surname", txtSurname.Text);
                        user.SetProperty("country", comboCountry.SelectedItem.ToString());
                        user.SetProperty("state", txtState.Text);
                        user.SetProperty("city", txtCity.Text);
                        user.Password = txtPass.Password;

                        Backendless.UserService.Register(user);
                       
                        this.DialogResult = true;

                        MessageBox.Show("Dear Mr/Miss " + txtName.Text + ", thank you for registering" +
                            "\n Please, verify the E-mail: " + txtEmail.Text +
                            "\n to confirm your signup. After that, this computer can be activated.",
                            "Success!"
                            , MessageBoxButton.OK, MessageBoxImage.Information);

                        AlreadyRegisteredUI loginUI = new AlreadyRegisteredUI();
                        loginUI.ShowDialog();
                    }
                    else
                    {
                        string errors = "Following error were found:";
                        if (string.IsNullOrWhiteSpace(txtName.Text))
                        {
                            errors += "\n - Name can not be empty";
                        }
                        if (string.IsNullOrWhiteSpace(txtSurname.Text))
                        {
                            errors += "\n - Sir name can not be empty";
                        }
                        if (!validProfess)
                        {
                            errors += "\n - You need to select your main activity";
                        }
                        if (!validCountry)
                        {
                            errors += "\n - You need to choose a country";
                        }
                        if (string.IsNullOrWhiteSpace(txtState.Text))
                        {
                            errors += "\n - State/Province must be filled";
                        }
                        if (string.IsNullOrWhiteSpace(txtCity.Text))
                        {
                            errors += "\n - City must be filled";
                        }
                        if (string.IsNullOrWhiteSpace(txtEmail.Text))
                        {
                            errors += "\n - E-mail must be filled";
                        }
                        if (!emailHasAt || !emailHasDot)
                        {
                            errors += "\n - Invalid E-mail";
                        }
                        if (string.IsNullOrWhiteSpace(txtPass.Password))
                        {
                            errors += "\n - Password must be filled";
                        }
                        if (txtEmail.Text != txtEmailConf.Text)
                        {
                            errors += "\n - E-mail and confirmation must be equal";
                        }
                        if (txtPass.Password != txtPassConf.Password)
                        {
                            errors += "\n - Password and confirmation must be equal";
                        }
                        if (checkAccept.IsChecked == false)
                        {
                            errors += "\n - You must agree with the terms to use this app";
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
                            case "1011":
                                customInfo = "User do not have permission";
                                break;
                            case "2002":
                                customInfo = "This app version is disabled, please contact support or update this version.";
                                break;
                            case "3009":
                                customInfo = "Register disabled for this app version";
                                break;
                            case "3010":
                                customInfo = "Register has an invalid field or disabled for this version ";
                                break;
                            case "3011":
                                customInfo = "Password error";
                                break;
                            case "3012":
                                customInfo = "A required field is empty";
                                break;
                            case "3013":
                                customInfo = "A main field is empty";
                                break;
                            case "3014":
                                customInfo = "Error on the external register";
                                //isJustWarn = false;
                                break;
                            case "3021":
                                customInfo = "General register error";
                                //isJustWarn = false;
                                break;
                            case "3033":
                                customInfo = "User already registered";
                                break;
                            case "3038":
                                customInfo = "Error occurred while trying to communicate with the server";
                                //isJustWarn = false;
                                break;
                            case "3039":
                                customInfo = "The field id can not be used as a field to register";
                                break;
                            case "3040":
                                customInfo = "Invalid E-mail";
                                break;
                            case "3041":
                                customInfo = "A required field is missing";
                                break;
                            case "3043":
                                customInfo = "Duplicated fields on register";
                                break;
                            case "8000":
                                customInfo = "Value exceeds the limit";
                                break;
                            default:
                                customInfo = "Server error";
                                break;
                        }

                        customInfo += " please, if you have any questions, contact onbim@onboxdesign.com.br for more information.";

                        MessageBox.Show(excep + "\n" + customInfo + "\n", "Register error.");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<CultureInfo> allCultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).ToList();
            IList<string> allCountries = new List<string>();
            foreach (CultureInfo currentCultureInfo in allCultures)
            {
                RegionInfo currentRegionInfo = null;
                try
                {
                    currentRegionInfo = new RegionInfo(currentCultureInfo.Name);
                }
                catch
                {
                    continue;
                }

                if (currentRegionInfo != null)
                {
                    string currentCountryName = currentRegionInfo.EnglishName;
                    if (!allCountries.Contains(currentCountryName))
                    {
                        allCountries.Add(currentCountryName);
                        comboCountry.Items.Add(currentCountryName);
                    }
                }
            }
        }

        private void lblAbout_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
