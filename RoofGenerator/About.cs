using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Package
{
   
    static public class About
    {
        [CanUpdatePeriodically(false)]
        static public string AboutThisPackage()
        {

            Register.AboutUI aboutUI = new Register.AboutUI();
            aboutUI.ShowDialog();


            return "Success.";
        }
    }
}
