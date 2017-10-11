using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;

namespace TopDownAutomate
{
    public class AutoPrint 
    {

        public AutoPrint()
        {
            //k = new KeyEventHandler(KeyDown);
            //while (true)
            //{
            //}
        }
        public void listenPrt()
        {
            while (true)
            {

            }
        }

        public void chamaLightScreen()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "LightscreenPortable.exe";
            processInfo.WorkingDirectory = Environment.CurrentDirectory + "\\Arquivos\\AutoPrint\\LightscreenPortable";
            Process.Start(processInfo);
        }

        void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            MessageBox.Show("HEEEY");
            if (e.KeyCode == Keys.PrintScreen)
            {
                System.Drawing.Bitmap printScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                Graphics graphics = Graphics.FromImage(printScreen as Image);

                graphics.CopyFromScreen(0, 0, 0, 0, printScreen.Size);

                printScreen.Save("Arquivos/printScreens/", ImageFormat.Jpeg);
                MessageBox.Show("AAAA");
            }

        }
    }
}
