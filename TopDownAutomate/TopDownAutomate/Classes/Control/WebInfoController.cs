using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopDownAutomate;

namespace TDFU.classes.Control
{
    public class WebInfoController
    {

        public WebInfoController()
        {

        }

        internal void carregarWebs(mainWindow mainWindow)
        {
         
        }

        internal void carregarWebs(Label label42, CheckedListBox checkBox_WebsCadastradas)
        {
            //LIMPAR ITEM DA CHECKBOX LIST
            checkBox_WebsCadastradas.Items.Clear();

            //REFRESHAR OS LABELS
            label42.Text = "";
            label42.Refresh();

            //CARREGAR TODAS AS WEBS
            List<WebInfo> webInfos = new List<WebInfo>();
            webInfos.Clear();
            webInfos = Web.listarWebsCadastradas();
            foreach (WebInfo w in webInfos)
            {
                //Adicionar na CheckBox
                checkBox_WebsCadastradas.Items.Add(w);
            }
        }

        internal void carregarWebs_doBanco(Label label42, CheckedListBox checkBox_WebsCadastradas)
        {
            //LIMPAR ITEM DA CHECKBOX LIST
            checkBox_WebsCadastradas.Items.Clear();

            //REFRESHAR OS LABELS
            label42.Text = "";
            label42.Refresh();

            //CARREGAR TODAS AS WEBS
            List<WebInfo> webInfos = new List<WebInfo>();
            webInfos.Clear();
            webInfos = Web.listarWebsCadastradas_doBanco();
            foreach (WebInfo w in webInfos)
            {
                //Adicionar na CheckBox
                checkBox_WebsCadastradas.Items.Add(w);
            }
        }
    }
}
