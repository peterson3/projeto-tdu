using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDFU.classes.Control;

namespace TopDownAutomate
{
    
    class MainPgm
    {

        [STAThread]
        static void Main(string[] args) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        MainController mainControl = new MainController();
        mainWindow janela = new mainWindow(mainControl, args);

        Application.Run(janela);
        }
    }
}
