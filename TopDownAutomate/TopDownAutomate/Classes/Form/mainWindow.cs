using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using TDFU.classes;
using TDFU.classes.Control;
using TDFU.classes.Model;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Data.SQLite;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace TopDownAutomate
{

    public partial class    mainWindow : Form
    {
        bool developerMode = false;
        Panel activePanel;
        Controle controle;
        Navegador nav;
        List<WebInfo> Webs;
        List<BancoInfo> bancos;
        List<BancoInfo> bdsAplica;
        List<BancoInfo> bdsAplicaCripto;
        List<ObjetoBanco> objetosDeBanco;
        List<ObjetoBanco> objetosDeBancoEmOrdemESelecionados;
        List<ObjetoBanco> obejtosDeBancoSelecionadosNaGridView;
        ToolTip toolTip = new ToolTip();
        private MainController mainControl;
        private static string conexao = "Data Source=Arquivos/Config/Banco.db";
        private static string nomebanco = "Arquivos/Config/Banco.db";

        public mainWindow()
        {
            InitializeComponent();
            carregaConfig();
            setInitPanel(pnl_home);
          
        }

        public mainWindow(MainController mainControl, string[] initArgs)
        {

            Logger.escrever("Sistema Iniciado.");
            if (!File.Exists(nomebanco))
            {
                Logger.escrever("Não Encontrado o Arquivo de Banco. Criando..");
                try
                {

                    SQLiteConnection.CreateFile(nomebanco);
                    SQLiteConnection conn = new SQLiteConnection(conexao);
                    Logger.escrever("Abrindo Conexão..");
                    conn.Open();

                    StringBuilder sql_query = new StringBuilder();
                    sql_query.AppendLine("CREATE TABLE IF NOT EXISTS BANCO ([ID] INTEGER PRIMARY KEY AUTOINCREMENT,");
                    sql_query.AppendLine("[NOME] VARCHAR(50))");

                    SQLiteCommand sql_cmd = new SQLiteCommand(sql_query.ToString(), conn);
                    sql_cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Logger.escrever("Erro: " + ex.Message);
                }
            }
            this.mainControl = mainControl;
            InitializeComponent();
            carregaConfig();
            setInitPanel(pnl_home);

            if (initArgs == null)
            {

            }
            else
            {
                for (int i = 0; i < initArgs.Length; i++)
                {
                    //MessageBox.Show("Argumento " + i + ": " + initArgs[i]);
                    if (initArgs[i] == "-developer")
                        DeveloperModeOn();
                }

            }
        }

        //Carrega Informações do Usuário Cadastrado e joga na tela
        public void carregaConfig() 
        {
            //mainControl.UsuarioController.CarregaInformacoesAoIniciar(usernameBox, passwordBox, pacotesFolderBox);
            mainControl.UsuarioController.CarregaInformacoesAoIniciar_doBanco(usernameBox, passwordBox, tnsnamesBox , pacotesFolderBox);
        }

        //Ao Clicar no Menu Configurações - Muda O Painel para Configurações de Usuário
        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_userConfig);   
        }

        //Salva informações inseridas pelo usuário na tela de configurações
        private void button1_Click(object sender, EventArgs e)
        {
            mainControl.UsuarioController.SalvarInformacoesDeConfiguracao_noBanco(usernameBox, passwordBox, tnsnamesBox,pacotesFolderBox);
            informativeMessage("Configurações de usuário salvas.");
            Logger.escrever("Salvo novas configurações de usuário");
        }

        //Erro ao Rejeitar o Input do Numero
        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            warningMessage("Entrada não aceita");
        }

        //Carregar Controle com Número Inputado.
        private void numeroControleOkBtn_Click(object sender, EventArgs e)
        {
            if (mainControl.UsuarioController.usuario == null)
                errorMessage("Usuário não configurado");
            else
            {
                nav = new Navegador(0);
                nav.TheAcessoDetalhesCA(Int32.Parse(numControleBox.Text), mainControl.UsuarioController.usuario, this);
                this.controle = nav.getInformacoesDaPagDetalhes(Int32.Parse(numControleBox.Text), mainControl.UsuarioController.usuario, this);
                this.label9.Text = controle.Numero.ToString();
                changePanel(pnl_controleActions);
            }    
        }

        //Abrir o Painel "Sobre"
        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_about);
        }

        //Abrir Folder Browser para setar a Pasta de Pacotes
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.pacotesFolderBox.Text = fbd.SelectedPath;
            }

        }

        //Abrir Painel de Procurar Controle
        private void procurarControleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_procurarControle);
        }

        //Abrir Painel de Realizar Ações de Controle
        private void realizarAçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);
        }

        //Opção Criar / Abrir Pasta
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                controle.criarPasta();
                //controle.criarPastaArquivosInternos();

            }
            catch (NullReferenceException ex)
            {
                errorMessage(ex, "Controle não carregado.");
            }
            catch (Exception ex)
            {
                errorMessage(ex, "Erro desconhecido");
            }
                
        }

        //Opção Mover Pasta
        //TODO: usar controllers
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                controle.moverArquivosDaPastaDeDownloadParaPastaDestino();
                Logger.escrever("Arquivos do CA " + controle.Numero + "movido para a pasta destino.");
            }
            catch (Exception ex)
            {
                errorMessage(ex, "Controle não carregado.");
            }

        }

        //Opção Quantidade de Objetos Inválidos Antes/Depois
        //TODO: usar Controllers
        private void button11_Click(object sender, EventArgs e) 
        {

            if (controle == null)
            {
                errorMessage("Controle não Carregado");
            }
            else
            {

                //if (bdsAplica.Count == 0)
                //{
                //    errorMessage("Nenhum Banco Registrado para Aplicação");
                //}
                //else
                //{
                //    controle.compararQuantidadeDeObjetosInvalidosAntesDepois(bdsAplica);
                //}
                //VERIFICAR QUANTOS .logs existem na pasta

                List<string> arqLogs = Directory.GetFiles(controle.enderecoPasta, "objetosInvalidosAntes*.log", SearchOption.AllDirectories).ToList();
                List<string> nomeDoBanco = new List<string>();
                foreach (string arq in arqLogs)
                {
                    string s;
                    s = arq.Remove(arq.Length - ".log".Length);
                    s = s.Remove(0, controle.enderecoPasta.Length+1+"objetosInvalidosAntes_".Length);
                    //informativeMessage(s); //s= nome do banco
                    nomeDoBanco.Add(s);
                }

                foreach (string nomeBanco in nomeDoBanco)
                {
                    listBox1.Items.Add(nomeBanco);
                }

                changePanel(pnl_selectLOGbd_invalid);
            }

        }

        //Opção texto de distribuição para o ClipBoard (pode discontinuar)
        //TODO: usar Controllers
        private void button15_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
                controle.criarTxtParaDistribuicao(bdsAplicaCripto);
                informativeMessage("Texto de distribuição copiado para o ClipBoard");
            }

        }

        //Opção de Aplicar na WEB
        //TODO: usar controllers
        private void button12_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Controle não carregado");
            }
            else
            {
                //Pegar Lista de Webs cadastradas
                 //mainControl.webInfoController.carregarWebs(label42, checkBox_WebsCadastradas);
                mainControl.webInfoController.carregarWebs_doBanco(label42, checkBox_WebsCadastradas);
                //Mudar o Painel
                changePanel(pnl_aplicarWeb);
            }

        }

        //Opção Extrair Pacote Gerado
        //TODO: usar controllers
        private void button7_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
                controle.extrairPacoteGerado();
                Logger.escrever("Arquivo do pacote do CA " + controle.Numero + " extraído.");

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (controle == null)
                errorMessage("Nenhum Controle Carregado");
            else
            {
                controle.conferirVersaoPacote();
                Logger.escrever("Log de versões do CA " + controle.Numero + " aberto.");
            }
        }

        //CRIAR APLICA.SQL
        private void button9_Click(object sender, EventArgs e)
        {
            if (controle == null)
              errorMessage("Controle não Carregado");
            else
            {
                //LISTA DE BANCOS CADASTRADOS NO ARQUIVO
                //bancos = Banco.listarBancosCadastrados();
                bancos = Banco.listarBancosCadastrador_versao_BD();
                //LIMPA A CHECKLIST
                chkListBox_bdsSelecionados.Items.Clear();
                //PEDE A SUGESTÃO DE APLICÃO
                List<string> bancosSugeridos = Sugest.sugerirBancos(bancos, controle.Produtos, controle.Clientes);

                //POPULA O CHECKLISTBOX
                foreach (BancoInfo bd in bancos)
                {
                    chkListBox_bdsSelecionados.Items.Add(bd);
                    foreach (string s in bancosSugeridos)
                    {
                        //SETA COMO CHECADO OS BANCOS SUGERIDOS
                        if (bd.Nome.ToUpper() == s.ToUpper())
                        {
                            chkListBox_bdsSelecionados.SetItemChecked(chkListBox_bdsSelecionados.Items.IndexOf(bd), true);
                        }
                    }

                }
                //VAI PARA A PROXIMA PAGINA
                changePanel(pnl_selecionarBancos);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
                try
                {
                    controle.aplicarNoBanco();
                    Logger.escrever("Executado o aplica no sqlplus.");
                }
                catch (Exception)
                {
                    Logger.escrever("Erro de de execução no sqlplus.");
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
             //   controle.abrirLogDeAplicacao(bdsAplica);
                Logger.escrever("Log de aplicão no banco do CA " + controle.Numero + " aberto.");

                List<string> arqLogs = Directory.GetFiles(controle.enderecoPasta, "objetos*.log", SearchOption.TopDirectoryOnly).ToList();
                List<string> arqLogs2 = Directory.GetFiles(controle.enderecoPasta, "*.log", SearchOption.TopDirectoryOnly).ToList();
                foreach(string arq in arqLogs)
                {
                    arqLogs2.Remove(arq);
                }

                foreach (string arq in arqLogs2)
                {
                    FileInfo f = new FileInfo(arq);
                    listBox2.Items.Add(f.Name);
                }

                changePanel(pnl_selectLOGbd);
            }

        }

        private void btn_compilarDlls_Click(object sender, EventArgs e)
        {
            informativeMessage("Não Implementado");
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            obejtosDeBancoSelecionadosNaGridView = new List<ObjetoBanco>();
           
            bdsAplica = new List<BancoInfo>();
            bdsAplica.Clear();

            foreach (object bancoSelecionado in chkListBox_bdsSelecionados.CheckedItems)
            {
                bdsAplica.Add((BancoInfo)bancoSelecionado);
                BancoInfo bd = (BancoInfo)bancoSelecionado;
                Logger.escrever("Banco Selecionado Para aplicar: " + bd.Nome);
            }

            objetosDeBanco = controle.recuperarArquivosSQLnaOrdemDeAtualizacao();

            dataGridView1.Rows.Clear();

            listView1.Items.Clear();
            listView1.Refresh();
            listView1.Groups.Clear();
            listView1.Clear();

            foreach (ObjetoBanco objbd in objetosDeBanco)
            {
                dataGridView1.Rows.Add(objbd.getTipo(), objbd.ArquivoBanco.Name);
            }



            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;

            //ADICIONA TODOS OS GRUPOS NA LISTVIEW

            for (int k=0; k<bdsAplica.Count; k++)
            {
                listView1.Groups.Add(new ListViewGroup(bdsAplica[k].Nome, bdsAplica[k].Nome));
            }

            //ADICIONA TODOS OS ITENS NA LISTVIEW
            for (int k = 0; k < bdsAplica.Count; k++)
            {
                for (int i = 0; i < bdsAplica[k].Owners.Count; i++)
                {
                    listView1.Items.Add(bdsAplica[k].Owners[i]).Group = listView1.Groups[k];
                }
            }

            changePanel(pnl_selecionaObjetosSql);

        }


        private void button5_Click_2(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);
        }


        private void abrirNavegadorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainControl.UsuarioController.usuario == null)
                MessageBox.Show("Usuário não configurado");
            else
            {
                if (nav != null)
                {
                    
                    nav.Recuperar();
                   // MessageBox.Show("Navegador Linkado com o programa já aberto.");
                }
                else
                {
                    nav = new Navegador(0);
                    nav.FazerLogin(mainControl.UsuarioController.usuario);
                    Logger.escrever("Abeto driver com Firefox.");
                }
            }

        }

        private void btn_okSelecionaObjsSql_Click(object sender, EventArgs e)
        {
           
            objetosDeBancoEmOrdemESelecionados = new List<ObjetoBanco>();
            objetosDeBancoEmOrdemESelecionados.Clear();

            for (int i=0; i<dataGridView1.Rows.Count; i++)
            {
                objetosDeBancoEmOrdemESelecionados.Add(ObjetoBanco.procurarObjetoDeBancoNaLista(objetosDeBanco, dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString()));
            }

            chkListBox_BancosParaCripto.Items.Clear();

            foreach (BancoInfo bd in bdsAplica)
            {
                chkListBox_BancosParaCripto.Items.Add(bd);
            }
            Logger.escrever("Sistema alterando o aplica para ordem certa de aplicação"); 
            changePanel(pnl_bancosParaCripto);

        }

        private void btn_cancelSelecionaObjsSql_Click(object sender, EventArgs e)
        {
            changePanel(pnl_selecionarBancos);
        }


        private void chkListBox_ordemSqls_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void chkListBox_ordemSqls_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(ObjetoBanco));
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:peterson.topdown@gmail.com");

        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            bdsAplicaCripto = new List<BancoInfo>();
            foreach (object bancoSelecionadosParaCripto in chkListBox_BancosParaCripto.CheckedItems)
            {
                bdsAplicaCripto.Add((BancoInfo)bancoSelecionadosParaCripto);
                bdsAplica.Remove((BancoInfo)bancoSelecionadosParaCripto);
            }

           string BDSsemCripto = "Selecionado P/ Não Criptografar em: ";

            foreach (BancoInfo temp in bdsAplica)
            {
                BDSsemCripto += temp.ToString() + " ";
            }
            Logger.escrever(BDSsemCripto);

            string BDScomCripto = "Selecionado P/ Criptografar em: ";

            foreach (BancoInfo temp in bdsAplicaCripto)
            {
                BDScomCripto += temp.ToString() + " ";
            }
            Logger.escrever(BDScomCripto);
            //MessageBox.Show(BDSsemCripto +Environment.NewLine+ BDScomCripto);

            if (checkBox1.Checked == true)
            {
                controle.criaAplicaTXTmodeloAntigo(bdsAplica, bdsAplicaCripto, objetosDeBancoEmOrdemESelecionados);
            }
            
            else
            {
                //controle.criaAplicaTXT(bdsAplica, bdsAplicaCripto, objetosDeBancoEmOrdemESelecionados);
                controle.criaAplicaSQL(bdsAplica, bdsAplicaCripto, objetosDeBancoEmOrdemESelecionados);
            }
            Logger.escrever("Arquivo \"APLICA.SQL\" criado com sucesso."); 
            informativeMessage("Arquivo \"APLICA.SQL\" criado com sucesso.");
            //ABRIR O ARQUIVO APLICA.SQL
            controle.abrirAplicaSql();
            changePanel(pnl_controleActions);


        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_selecionaObjetosSql);
        }

        public void changePanel(Panel painel)
        {
            if (activePanel != null)
                activePanel.Visible = false;
            painel.Visible = true;
            activePanel = painel;
            string windowName = "";

            if (painel == pnl_about)
                windowName = "Sobre";
            if (painel == pnl_aplicarWeb)
                windowName = "Aplicar na Web";
            if (painel == pnl_detalhesCA)
                windowName = "Detalhes do CA";
            if (painel == pnl_userConfig)
                windowName = "Configurações do Usuário";
            if (painel == pnl_procurarControle)
                windowName = "Procurar Controle";
            if (painel == pnl_controleActions)
                if (controle==null)
                windowName = "Realizar Ações - CA não carregado ";
                else
                windowName = "Realizar Ações do CA "+controle.Numero.ToString();
            if (painel == pnl_macros)
                windowName = "Macros";
            if (painel == pnl_selecionaObjetosSql)
                windowName = "Selecionar Objetos de Banco";
            if (painel == pnl_bancosParaCripto)
                windowName = "Bancos Para Criptografia";
            if (painel == pnl_selecionarBancos)
                windowName = "Selecionar Bancos";
            if (painel == pnl_objetosWeb)
                windowName = "Comparar/Substituir Objetos de Web";
            if (painel == pnl_openAnyCAFolder)
                windowName = "Abrir Pasta de CA Não Carregado";


            this.Text = windowName;
        }

        public void setInitPanel (Panel painel)
        {
            activePanel = painel;
        }

        private void bancosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Banco.abrirConfig();
        }

        private void websToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rede.abrirConfig();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
                //TODO
                label21.Text = controle.Numero.ToString();
                label33.Text = controle.ResponsavelTeste;
                label34.Text = controle.ResponsavelDistribuicao;
                label35.Text = controle.Situacao;
                label36.Text = controle.RegistradoEm;
                richTextBox2.Text = controle.Descricao;
                richTextBox3.Text = controle.Parametros;
                richTextBox4.Text = controle.Impacto;
                richTextBox5.Text = "";
                foreach (Produto p in controle.Produtos)
                {
                    richTextBox5.Text += p.Nome +" "+ p.Versao +" "+ p.Modulo +" "+ p.Funcao;
                    richTextBox5.Text += Environment.NewLine;
                }
                richTextBox6.Text = "";
                foreach (Cliente c in controle.Clientes)
                {
                    richTextBox6.Text += c.Nome + " ";
                    foreach (int s in c.NumeroSAC)
                    {
                        richTextBox6.Text += s.ToString();
                        richTextBox6.Text += " ";
                    }
                    richTextBox6.Text += Environment.NewLine;
                }
                richTextBox7.Text = "";
                foreach (string d in controle.ParteCaminhoArquivos)
                {
                    richTextBox7.Text += d;
                    richTextBox7.Text += Environment.NewLine;
                }

                changePanel(pnl_detalhesCA);
            }
        }

        private void btn_exibirDetalhes_Click(object sender, EventArgs e)
        {
            if (controle == null)
            {
                errorMessage("Nenhum Controle Carregado");
            }
            else
            {
                //TODO
                label21.Text = controle.Numero.ToString();
                label33.Text = controle.ResponsavelTeste;
                label34.Text = controle.ResponsavelDistribuicao;
                label35.Text = controle.Situacao;
                label36.Text = controle.RegistradoEm;
                richTextBox2.Text = controle.Descricao;
                richTextBox3.Text = controle.Parametros;
                richTextBox4.Text = controle.Impacto;
                richTextBox5.Text = "";
                foreach (Produto p in controle.Produtos)
                {
                    richTextBox5.Text += p.Nome + " " + p.Versao + " " + p.Modulo + " " + p.Funcao;
                    richTextBox5.Text += Environment.NewLine;
                }
                richTextBox6.Text = "";
                foreach (Cliente c in controle.Clientes)
                {
                    richTextBox6.Text += c.Nome + " ";
                    foreach (int s in c.NumeroSAC)
                    {
                        richTextBox6.Text += s.ToString();
                        richTextBox6.Text += " ";
                    }
                    richTextBox6.Text += Environment.NewLine;
                }
                richTextBox7.Text = "";
                foreach (Arquivo a in controle.Objetos)
                {
                    richTextBox7.Text += a.CaminhoParcial;
                    richTextBox7.Text += Environment.NewLine;
                }

                changePanel(pnl_detalhesCA);
            }
        }

        private void numControleBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                numeroControleOkBtn.Focus();
                numeroControleOkBtn.PerformClick();
            }
        }

        private void ativarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "LightscreenPortable.exe";
            processInfo.WorkingDirectory = Environment.CurrentDirectory + "\\Arquivos\\AutoPrint\\LightscreenPortable";
            Process.Start(processInfo);

        }


        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {

            obejtosDeBancoSelecionadosNaGridView.Clear();

            for (int i = 0; i < e.Row.DataGridView.SelectedRows.Count; i++)
            {
                //PROCURAR O Objeto de Banco correspondente a linha e adicionar na lista
                obejtosDeBancoSelecionadosNaGridView.Add(ObjetoBanco.procurarObjetoDeBancoNaLista(objetosDeBanco, e.Row.DataGridView.SelectedRows[i].Cells[0].Value.ToString(), e.Row.DataGridView.SelectedRows[i].Cells[1].Value.ToString()));
            }

            if (obejtosDeBancoSelecionadosNaGridView.Count == 1)
            {
                //Deseleciona Toda a lista
                for (int k=0; k<listView1.Items.Count; k++)
                {
                    listView1.Items[k].Checked = false;
                }

                //Seleciona Apenas com Owners/Banco ja setados no Objeto
                for (int k = 0; k < listView1.Items.Count; k++)
                {
                    if (obejtosDeBancoSelecionadosNaGridView[0].verificarOwnerBanco(listView1.Items[k]))
                    {
                        listView1.Items[k].Checked = true;
                    }
                }
            }
            else //Count >1
            {
                //Deseleciona Toda a lista
                for (int k = 0; k < listView1.Items.Count; k++)
                {
                    listView1.Items[k].Checked = false;
                }
            }
        }

        private void pegarBancosTESTEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bancos = Banco.listarBancosCadastrados();
        }

        private void button8_Click_3(object sender, EventArgs e)
        {
            string bla = "";
            for (int i = 0; i < obejtosDeBancoSelecionadosNaGridView.Count; i++)
            {
                bla += obejtosDeBancoSelecionadosNaGridView[i].ArquivoBanco.Name + Environment.NewLine;
            }
            MessageBox.Show(bla);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            foreach (ObjetoBanco obj in obejtosDeBancoSelecionadosNaGridView)
            {
                obj.limparOwnersBancos();
                for (int k = 0; k < listView1.CheckedItems.Count; k++)
                {
                    obj.aplicarNoOwnerBanco(listView1.CheckedItems[k].Text, listView1.CheckedItems[k].Group.Name);
                    Logger.escrever("Atribuído objeto do tipo " + obj.getTipo() + " - " + obj.ArquivoBanco.Name + " em " + listView1.CheckedItems[k].Text + "/" + listView1.CheckedItems[k].Group.Name);
                }
            }
            informativeMessage("Atribuição Salva!");
        }

        private void resetarInformacoes()
        {
            controle = null;
            bancos = null;
            bdsAplica = null;
            bdsAplicaCripto = null;
            objetosDeBanco = null;
            objetosDeBancoEmOrdemESelecionados = null;
            obejtosDeBancoSelecionadosNaGridView = null; 
            //objetosWeb = null;
        }

        private void carregarControleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                resetarInformacoes();
                if (mainControl.UsuarioController.usuario == null)
                    errorMessage("Usuário não configurado");
                else
                {
                    this.controle = nav.getInformacoesDaPagDetalhes(mainControl.UsuarioController.usuario, this);
                    if (controle == null)
                    {
                        errorMessage("Controle NULO");
                    }
                    else
                    {
                        Logger.escrever("Controle instanciado: " + controle.Numero);
                        label9.Text = controle.Numero.ToString();
                        changePanel(pnl_controleActions);
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage(ex, "Navegador não encontrado.");
            }
        }

        private void btn_exibirDetalhes_MouseHover(object sender, EventArgs e)
        {
            toolTip.Show("Exibe Detalhes do Controle", btn_exibirDetalhes);
        }


        private void checkBox_WebsCadastradas_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int ix=0; ix< checkBox_WebsCadastradas.Items.Count; ix++)
            {
                if (ix != e.Index)
                    checkBox_WebsCadastradas.SetItemChecked(ix, false);
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2) //Clicou em uma Comparacao do Winmerge
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.UseShellExecute = true;
                processInfo.FileName = "WinMergeU.exe";
                processInfo.Arguments = (string)dataGridView2.Rows[e.RowIndex].Cells[0].Value; //ARQUIVO DO PACOTE
                processInfo.Arguments += " " + (string)dataGridView2.Rows[e.RowIndex].Cells[1].Value; //ARQUIVO DA WEB
                Process.Start(processInfo);
            }

            if ((e.ColumnIndex== 0) ||(e.ColumnIndex==1)) //Clicou no Endereço do Arquivo do Pacote
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = (string)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value; //ARQUIVO DO PACOTE ou DA WEB
                Process.Start(processInfo);
            }

            if (e.ColumnIndex == 3) //Clicou em Substituir ou Criar
            {
                FileInfo arq = new FileInfo((string)dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                FileAttributes dirOrFile = File.GetAttributes((string)dataGridView2.Rows[e.RowIndex].Cells[1].Value);
                int failedToCopy = 0;
                if (dirOrFile.HasFlag(FileAttributes.Directory))
                {
                    //Eh um diretorio
                    DirectoryInfo dir = new DirectoryInfo((string)dataGridView2.Rows[e.RowIndex].Cells[1].Value);
                    try
                    {
                        arq.CopyTo(dir.FullName + "\\" + arq.Name, true);
                        Logger.escrever(arq.FullName + " copiado para " + dir.FullName);
                    }
                    catch (Exception ex)
                    {
                        //1o - tenta uma outra solucao - copiar via cmd
                        bool tentativa2= Rede.copiarParaAWebDir(arq.FullName, dir.FullName);
                        if (tentativa2 == true)
                        {
                            Logger.escrever(arq.FullName + " copiado para " + dir.FullName);
                            //OK CONSEGUIU COPIAR
                        }
                        else
                        {
                            Logger.escrever(arq.FullName + " falha ao copiar para " + dir.FullName);
                            MessageBox.Show(ex.Message);
                            failedToCopy++;
                        }
                    }
                    
                }
                else
                {
                    //eh um arquivo
                    FileInfo arqDest = new FileInfo((string)dataGridView2.Rows[e.RowIndex].Cells[1].Value);
                    try
                    {
                        if (arqDest.IsReadOnly == true)
                        {
                            arqDest.IsReadOnly = false;
                            Logger.escrever("Alterando atributo de arquivo de somente leitura.");
                        }
                        arq.CopyTo(arqDest.FullName, true);
                        Logger.escrever(arq.FullName + " copiado para " + arqDest);
                    }
                    catch (Exception ex)
                    {
                        //1o - tenta uma outra solucao - copiar via cmd
                        bool tentativa2 = Rede.copiarParaAWebArq(arq.FullName, arqDest.FullName);
                        //Logger.escrever(arq.FullName + " copiado para " + arqDest);


                        if (tentativa2 == true)
                        {
                            Logger.escrever(arq.FullName + " copiado para " + arqDest);
                            //OK CONSEGUIU COPIAR
                        }
                        else
                        {
                            Logger.escrever(arq.FullName + " falha ao copiar para " + arqDest.FullName);
                            MessageBox.Show(ex.Message);
                            failedToCopy++;
                        }
                    }

                }

                if (failedToCopy == 0)
                {
                    MessageBox.Show("Arquivo Copiado com Sucesso!");
                }
                else
                {
                    MessageBox.Show("Erro ao Copiar " + failedToCopy.ToString() +" Arquivo(s)");
                }
            }
        }

        private void googleChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainControl.UsuarioController.usuario == null)
                MessageBox.Show("Usuário não configurado");
            else
            {
                if (nav != null)
                {
                    //MessageBox.Show("Navegador Linkado com o programa já aberto.");
                    nav.Recuperar();
                }
                else
                {
                    nav = new Navegador(1);
                    nav.FazerLogin(mainControl.UsuarioController.usuario);
                    Logger.escrever("Aberto driver com Chrome.");
                }
                
            }
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);

        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_aplicarWeb);
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            changePanel(pnl_aplicarWeb);
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);
        }

        //BOTÃO: ATUALIZAR TODOS
        private void button13_Click(object sender, EventArgs e)
        {
            int failedToCopy = 0;
            for (int i=0; i<dataGridView2.RowCount; i++)
            {
                // Pegar o  valor da primeira coluna (é sempre um arquivo, do pacote)
                FileInfo arqPacote = new FileInfo(dataGridView2.Rows[i].Cells[0].Value.ToString());

                //Pegar o valor da segunda coluna (é um arquivo ou um diretorio)
                FileAttributes dirOrFile = File.GetAttributes((string)dataGridView2.Rows[i].Cells[1].Value);
                if (dirOrFile.HasFlag(FileAttributes.Directory))
                {
                    //Eh um diretorio
                    DirectoryInfo dir = new DirectoryInfo((string)dataGridView2.Rows[i].Cells[1].Value);
                    
                    //TODO: ESTE COPY NÃO ESTÁ FUNCIONANDO: MUDAR PARA COPY VIA CMD
                    try
                    {
                        arqPacote.CopyTo(dir.FullName + "\\" + arqPacote.Name, true);
                        Logger.escrever(arqPacote.FullName + " copiado para " + dir.FullName);
                    }
                    catch (Exception ex)
                    {



                        //TENTATIVA 2

                        new FileIOPermission(FileIOPermissionAccess.AllAccess, dir.FullName).Demand();
                        try
                        {
                            arqPacote.CopyTo(dir.FullName + "\\" + arqPacote.Name, true);
                            Logger.escrever("Tentativa 2: " + arqPacote.FullName + " copiado para " + dir.FullName);
                        }
                        catch (Exception ex2)
                        {
                            
                            //TENTATIVA 3
                            //    
                            Logger.escrever("Tentativa 2: " + ex2.Message);
                            //1o - tenta uma outra solucao - copiar via cmd
                            bool tentativa2 = Rede.copiarParaAWebDir(arqPacote.FullName, dir.FullName);
                            if (tentativa2 == true)
                            {
                                Logger.escrever(arqPacote.FullName + " copiado para " + dir.FullName);
                                //OK CONSEGUIU COPIAR
                            }
                            else
                            {
                                Logger.escrever(arqPacote.FullName + " falha ao copiar para " + dir.FullName);
                                MessageBox.Show(ex.Message);
                                failedToCopy++;
                            }
                        }

                    }

                }
                else
                {
                    //eh um arquivo
                    FileInfo arqDest = new FileInfo((string)dataGridView2.Rows[i].Cells[1].Value);
                    //TODO: ESTE COPY NÃO ESTÁ FUNCIONANDO: MUDAR PARA COPY VIA CMD
                    try
                    {
                        if (arqDest.IsReadOnly == true)
                        {
                            arqDest.IsReadOnly = false;
                            Logger.escrever("Alterando atributo de arquivo de somente leitura.");
                        }
                        arqPacote.CopyTo(arqDest.FullName, true);
                        Logger.escrever(arqPacote.FullName + " copiado para " + arqDest.FullName);
                    }
                    catch (Exception ex)
                    {


                        //TENTATIVA 2
                        new FileIOPermission(FileIOPermissionAccess.AllAccess, arqDest.FullName).Demand();
                        try
                        {
                            arqPacote.CopyTo(arqDest.FullName, true);
                            Logger.escrever("Tentativa 2: " + arqPacote.FullName + " copiado para " + arqDest.FullName);
                        }
                        catch (Exception ex2)
                        {
                            Logger.escrever("Tentativa 2: " + ex2.Message);
                            //TENTATIVA 3                        
                            //1o - tenta uma outra solucao - copiar via cmd
                            bool tentativa2 = Rede.copiarParaAWebArq(arqPacote.FullName, arqDest.FullName);
                            if (tentativa2 == true)
                            {
                                Logger.escrever(arqPacote.FullName + " copiado para " + arqDest.FullName);
                                //OK CONSEGUIU COPIAR
                            }
                            else
                            {
                                Logger.escrever(arqPacote.FullName + " falha ao copiar para " + arqDest);
                                MessageBox.Show(ex.Message);
                                failedToCopy++;
                            }

                        }

                    }
                   
                }
            }

            if (failedToCopy == 0)
            {
                MessageBox.Show("Arquivos Copiados com Sucesso!");
            }
            else
            {
                MessageBox.Show("Erro ao Copiar " + failedToCopy.ToString() + " Arquivo(s)");
            }

        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        

            //TRABALHO ASSÍNCRONO -- CARREGAR OBJETOS DA WEB
            button10.Enabled = false;
            dataGridView2.Rows.Clear();
            Image imagem = Image.FromFile("Arquivos/Icone/96px-WinMergeLogo.png");
            imagem = (Image)new Bitmap(imagem, new Size(20, 20));

            Image imagemSub = Image.FromFile("Arquivos/Icone/Swapster-Arrows.png");
            imagemSub = (Image)new Bitmap(imagemSub, new Size(20, 20));

            Image imagemCria = Image.FromFile("Arquivos/Icone/ic_menu_upload.png");
            imagemCria = (Image)new Bitmap(imagemCria, new Size(20, 20));

            // 0 - recuperar Web Selecionada
            WebInfo webAserAplicada = (WebInfo)checkBox_WebsCadastradas.CheckedItems[0];

    
            // 0.1 - Computador (recuperar Seu nome na Rede)
            string nomePc = Rede.recuperarNomePC();
            //nomePc = "TD-264"; -nao resolveu fera
            if (nomePc == null){
                //MessageBox.Show("Erro ao achar nome PC");

            }
            else
            {
                //MessageBox.Show("NOME PC: " + nomePc);

            }

            // 1 - logar na web selecionada
            List<string> possibleNetWorkFolders = new List<string>();

            string Ret = Rede.LogarNaMaquina(webAserAplicada, nomePc);
            if (Ret.ToUpper().Contains("COMANDO")) //FUNCIONOU
            {
                Ret = "Sucesso Ao logar.";
                informativeMessage(Ret);
            }
            else
            {
                //MessageBox.Show("ERRO AO LOGAR NA MAQ..TENTANDO RELOGAR");
                Rede.DeslogarDaMaquina(webAserAplicada);
                Ret = Rede.LogarNaMaquina(webAserAplicada, nomePc);
                if (Ret.ToUpper().Contains("COMANDO"))
                {
                    Ret = "Sucesso Ao logar.";
                    informativeMessage(Ret);
                }
                else
                {
                    Ret = "Erro ao Logar. Verifique o Login/Senha Cadastrado. Abortando Procura de Arquivos";
                    errorMessage(Ret);
                    return;
                }
            }
            Logger.escrever("Resultado do Login na WEB " + webAserAplicada +": " + Ret);

            //0.1 - Das webs recuperar possíveis pastas
            List<String> possibleFolders = Rede.retornarPastas("\\\\" + webAserAplicada.Endereco);
            string a = "";

            possibleNetWorkFolders.Clear();

            foreach (string p in possibleFolders)
            {
                a+=p+" ";
                possibleNetWorkFolders.Add(p);
            }

            if (possibleFolders.Count == 0)
            {
                warningMessage("Não encontrada pastas na rede!");
            }
            else
            {
               // informativeMessage("Pastas:\n" + a);
               Logger.escrever("Sistema procurará os arquivos na(s) pasta(s): " + a);
            }

            
            // 2- Recuperar Todos os Objetos que o pacote tem, que não sejam objetos de banco, 

            //pastas possíveis da rede

            //TESTANDO LISTA DE NETWORK FOLDER POSSÍVEIS PARA CADA 

            //possibleNetWorkFolders.AddRange(possibleFolders);

            List<FileInfo> arquivosDoPacote = new List<FileInfo>();
            List<string> arquivosDoPacoteSTRING = new List<string>();

            //MessageBox.Show("NUMERO PASTAS:" + possibleNetWorkFolders.Count.ToString());

            arquivosDoPacoteSTRING.AddRange(Directory.GetFiles(controle.enderecoPasta, "*", SearchOption.AllDirectories).ToList());





            // Procurar Possiveis Pasta da Rede
            //DIRETORIOS E SUBDIRETORIOS POSSIVEIS
            List<string> diretoriosESubs = new List<string>();

            Stopwatch cronometro = new Stopwatch();
            cronometro.Start();
      

            //TRANSFORMA A String DOS ARQUIVOS DO PACOTE EM ARQUIVO
            foreach (string arq in arquivosDoPacoteSTRING)
            {
                FileInfo arquivo = new FileInfo(arq);
                if ((arquivo.Extension.ToUpper() != ".SQL") && (arquivo.Extension.ToUpper() != ".SPC") && (arquivo.Extension.ToUpper() != ".PLB"))
                {
                    //Teste de diretório
                    DirectoryInfo dir1 = new DirectoryInfo(arquivo.DirectoryName);
                    DirectoryInfo dir2 = new DirectoryInfo(controle.enderecoPasta);
                    //Se o diretório do arquivo for diferente do diretório raiz
                    if (dir1.FullName.ToUpper() != dir2.FullName.ToUpper())
                    {
                        arquivosDoPacote.Add(new FileInfo(arq));
                    }
                }
            }


            Logger.escrever("Quantidade de Arquivos: " + arquivosDoPacote.Count.ToString() + " (arquivos que não estão na raíz).");
            //CONFIGURACAO DA BARRA DE PROGRESSO
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Step = 100 / arquivosDoPacote.Count;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            //


            //pode, pra tentar achar logo o arquivo //Já que 90% das vezes vai cair aqui
            //TODO: Verificar....
            //1 -FUNCIONA mas tem q cancelar a outra procura (EXAUSTIVA)
            // 1.1- Só continuar procurando se algum deles não for encontrado
            //2- Caso não ache o arquivo, tentar achar a pasta (nao feito ainda)
            //1- Extrair do arquivo seu endereco Parcial, ou seja, sem o caminho do pacote
            List<FileInfo> arquivosNaoEncontrados = new List<FileInfo>();
            foreach (FileInfo arq in arquivosDoPacote)
            {
                label42.Text = "Procurando "+arq.Name;
                label42.Refresh();
                bool encontrado = false;
                string dirAbsoluto = arq.FullName; //nome do diretório + nome do arquivo
                int tamanhoNomePastaControle = controle.enderecoPasta.Length;
                string dirRelativo = dirAbsoluto.Remove(0, tamanhoNomePastaControle + 1); //+1 pq tira tb o "\"
                //Logger.escrever("endereco relativo arq " + arq.Name + ": " + dirRelativo);
                
                if (dirRelativo.Contains("\\")) //quem contém isso NÃO É endereco raiz
                {
                    if (encontrado == false) //Continua sem achar
                    {
                        //Procura nas pastas da rede
                        foreach (String endPossiveldaRede in possibleNetWorkFolders)
                        {
                            //2- Verificar se o arquivo existe no endereco da rede
                            string arqProcurado = "\\\\" + webAserAplicada.Endereco + "\\" + endPossiveldaRede + "\\" + dirRelativo;
                            //TODO:
                            //na realidade o primeiro teste deve ser, pegando endereco relativo dos arquivos, tentar jogar direto no endereco da rede

                            if (File.Exists(arqProcurado))
                            {
                                Logger.escrever("Arquivo ENCONTRADO em " + arqProcurado);
                                dataGridView2.Rows.Add(arq.FullName, arqProcurado, imagem, imagemSub);
                                encontrado = true;
                            }
                            else
                            {
                                // Logger.escrever("Arquivo não encontrado em " + arqProcurado);
                                int tamanhoNomeArq = arq.Name.Length;
                                string pastaProcurada = arqProcurado.Substring(0, arqProcurado.Length - tamanhoNomeArq);
                                if (Directory.Exists(pastaProcurada))
                                {
                                    Logger.escrever("Pasta ENCONTRADA em " + pastaProcurada);
                                    dataGridView2.Rows.Add(arq.FullName, pastaProcurada, imagem, imagemCria);
                                    encontrado = true;
                                }
                                else
                                {
                                    //tratar quando possui algo do tipo \\10.1.1.159\web\web\rbm\asp\ -- ou n?
                                    //Não encontrou nem pasta nem arquivo
                                   // Logger.escrever("Não foi encontrada nem arquivo nem pasta correspondente em " + pastaProcurada);
                                }
                            }
                        }
                        if (encontrado == false)
                        {
                            //1 - verifica se existe no endereco de rede raiz 
                            //-> remove ambiguidades tipo \\10.1.1.159\web\web\rbm\asp\
                            string arqProcurandoRaiz = "\\\\" + webAserAplicada.Endereco + "\\" + dirRelativo;
                            if (File.Exists(arqProcurandoRaiz))
                            {
                                Logger.escrever("Arquivo ENCONTRADO em " + arqProcurandoRaiz);
                                dataGridView2.Rows.Add(arq.FullName, arqProcurandoRaiz, imagem, imagemSub);
                                encontrado = true;
                            }
                            else
                            {
                                int tamanhoNomeArq = arq.Name.Length;
                                string pastaProcurada = arqProcurandoRaiz.Substring(0, arqProcurandoRaiz.Length - tamanhoNomeArq);
                                if (Directory.Exists(pastaProcurada))
                                {
                                    Logger.escrever("Pasta ENCONTRADA em " + pastaProcurada);
                                    dataGridView2.Rows.Add(arq.FullName, pastaProcurada, imagem, imagemCria);
                                    encontrado = true;
                                }
                                else
                                {
                                    //tratar quando possui algo do tipo \\10.1.1.159\web\web\rbm\asp\ -- ou n?
                                    //Não encontrou nem pasta nem arquivo
                                    //Logger.escrever("Não foi encontrada nem arquivo nem pasta correspondente em " + pastaProcurada);
                                }
                            }
                        }
                        if (encontrado == false) //se ainda nao encontrou,,, sinto muito
                        {

                            arquivosNaoEncontrados.Add(arq);
                        }
                    }
                }
                progressBar1.PerformStep();
                progressBar1.Refresh();
            }
            if (arquivosNaoEncontrados.Count > 0) //NÃO ENCONTROU NEM ARQUIVO NEM PASTA DE ALGUM OBJETO
            {
               if(MessageBox.Show("Há objetos " + arquivosNaoEncontrados.Count.ToString() + " que não foram encontrados, nem sua pasta correspondente. Realizar busca aprofundada? (Processo pode demorar)", "Realizar Busca Aprofundada?", MessageBoxButtons.YesNo)==DialogResult.Yes)
               {
                   Logger.escrever("Realizando busca aprofundada para objetos não encontrados...");
                   //colocar a busca exaustiva aqui
                  List<string> ListaDiretorios = new List<string>();
                  foreach (string endPossiveldaRede in possibleNetWorkFolders)
                  {
                      try
                      {
                          List<String> aaa = Directory.GetDirectories("\\\\" + webAserAplicada.Endereco + "\\" + endPossiveldaRede, "*", SearchOption.AllDirectories).ToList();
                          ListaDiretorios.AddRange(aaa);
                      }
                      catch (Exception ex)
                      {
                          //Logger.escrever("ERRO: " + ex.Message);
                      }
                  }
                   
  

                  foreach (string currentDirectory in ListaDiretorios)
                  {
                      foreach (FileInfo fileNotFound in arquivosNaoEncontrados)
                      {
                          string arqProcurado = fileNotFound.Name;
                          string dirAbsoluto = fileNotFound.DirectoryName; //nome do diretório + nome do arquivo
                          int tamanhoNomePastaControle = controle.enderecoPasta.Length;
                          string dirRelativo = dirAbsoluto.Remove(0, tamanhoNomePastaControle + 1); //+1 pq tira tb o "\"   
                          //Logger.escrever("Pesquisando endereço " + dirRelativo + " em " + currentDirectory);
                          if (currentDirectory.ToUpper().Contains(dirRelativo.ToUpper()) == true)
                          {
                              //Chegamos na pasta que queríamos
                              //Procurar o arquivo
                             // Logger.escrever("ACHOU PASTA..." + currentDirectory + " PROCURANDO ARQUIVO " + arqProcurado);
                              if (File.Exists(currentDirectory + "\\" + arqProcurado))
                              {
                                  FileInfo file = new FileInfo(currentDirectory + "\\" + arqProcurado);
                                  dataGridView2.Rows.Add(fileNotFound.FullName, file.FullName, imagem, imagemSub);
                              }
                              else
                              {
                                  //adiciona a pasta na grid
                                  DirectoryInfo dir = new DirectoryInfo(currentDirectory);
                                  dataGridView2.Rows.Add(fileNotFound.FullName, dir.FullName, imagem, imagemCria);
                              }
                          }
                      }
                  }

                 }
               }
               else
               {
                   Logger.escrever("Busca Aprofundada necessária não realizada");
               }
            
            cronometro.Stop();
            Logger.escrever("Tempo que levou para achar todos os arquivos: " + cronometro.Elapsed.Minutes + " minuto(s), " + cronometro.Elapsed.Seconds + " segundo(s) e " + cronometro.Elapsed.Milliseconds + " milisegundo(s).");
 }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //PERMITIR A CONTINUIDADE DO PROCESSO
            changePanel(pnl_objetosWeb);
            button10.Enabled = true;
        }

        private void abrirPastaDeControlePorNúmeroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_openAnyCAFolder);
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            Controle.abrirPastaDeControleNaoCarregado(Int32.Parse(maskedTextBox1.Text), mainControl.UsuarioController.usuario);
        }

        //Mensagens de warning, erro e informativo
        private void warningMessage(Exception ex, String Mensagem)
        {
            MessageBox.Show(Mensagem + "\n\nDetalhes:\n" + ex.Message, "Ops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void warningMessage(String Mensagem)
        {
            MessageBox.Show(Mensagem, "Ops!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void errorMessage(Exception ex, String Mensagem)
        {
            MessageBox.Show(Mensagem + "\n\nDetalhes:\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void errorMessage(String Mensagem)
        {
            MessageBox.Show(Mensagem,"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void informativeMessage (String Mensagem)
        {
            MessageBox.Show(Mensagem,"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void macrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_macros);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            changePanel(pnl_macros);
        }

        private void userCANCELbtn_Click(object sender, EventArgs e)
        {
            changePanel(pnl_controleActions);
        }

        //TODO: FUNÇÃO QUE CANCELA TODAS AS ALTERAÇÕES REFERENTES A TELA CORRENTE (OU SEJA, LIMPA TODOS OS COMPONENTES) E RETORNA PRA TELA ANTERIOR;
        private void cancelar()  
        {

        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Logger.abrir() == false)
            {
                warningMessage("Log não encontrado para o dia de hoje.");
            }
        }

        private void mainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.escrever("Sistema finalizado."+Environment.NewLine);
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];

                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex == 0)  //É um arquivo do Pacote - Ou seja, sempre é um arquivo // nunca um caminho
                    {
                        c.ContextMenuStrip = new ContextMenuStrip();
                        ToolStripItem abrirArq = c.ContextMenuStrip.Items.Add("Abrir Arquivo");
                        abrirArq.Click += abrirArq_Click;
                        ToolStripItem abrirCaminho = c.ContextMenuStrip.Items.Add("Abrir Caminho");
                        abrirCaminho.Click += abrirCaminho_Click;
                        //c.ContextMenuStrip.Items.Add("Fazer Backup");
                    }
                    else if (e.ColumnIndex == 1) //É um arquivo ou um caminho
                    {
                        c.ContextMenuStrip = new ContextMenuStrip();
                        FileAttributes dirOrFile = File.GetAttributes((string)c.Value);
                        if (dirOrFile.HasFlag(FileAttributes.Directory))
                        {
                            //Se for Caminho
                            c.ContextMenuStrip = new ContextMenuStrip();
                            ToolStripItem abrirCaminho = c.ContextMenuStrip.Items.Add("Abrir Caminho");
                            abrirCaminho.Click += abrirArq_Click; //MESMA SITUACAO DE ABRIR ARQ
                        }
                        else
                        {
                            //Se for Arquivo
                            c.ContextMenuStrip = new ContextMenuStrip();
                            ToolStripItem abrirArq = c.ContextMenuStrip.Items.Add("Abrir Arquivo");
                            abrirArq.Click += abrirArq_Click;
                            ToolStripItem abrirCaminho = c.ContextMenuStrip.Items.Add("Abrir Caminho");
                            abrirCaminho.Click += abrirCaminho_Click;
                            //c.ContextMenuStrip.Items.Add("Fazer Backup");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Usuário está clicando nos HEADERS da datagridview
               // errorMessage(ex.Message);
            }

        }

        void abrirCaminho_Click(object sender, EventArgs e)
        {
            string arq = (string)dataGridView2.CurrentCell.Value;
            FileInfo file = new FileInfo(arq);
            Process.Start(file.DirectoryName);
        }

        void abrirArq_Click(object sender, EventArgs e)
        {
            string Arq = (string)dataGridView2.CurrentCell.Value;
            Process.Start(Arq);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (string nomeBanco in listBox1.SelectedItems)
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.UseShellExecute = true;
                processInfo.FileName = "WinMergeU.exe";
                processInfo.Arguments = controle.enderecoPasta + "\\" + "objetosInvalidosAntes_" + nomeBanco + ".log"; //ARQUIVO DO PACOTE
                processInfo.Arguments += " " + controle.enderecoPasta + "\\" + "objetosInvalidosDepois_" + nomeBanco + ".log";  //ARQUIVO DA WEB
                Process.Start(processInfo);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            changePanel(pnl_controleActions);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            foreach (string nomeArq in listBox2.SelectedItems)
            {
                Process.Start(controle.enderecoPasta+"\\"+nomeArq);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            changePanel(pnl_controleActions);
        }

        private void nOVOBancosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changePanel(pnl_bdConfig);
            Carregar();

        }

        private void Carregar()
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM BANCO", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            //List<BancoInfo> bancos = new List<BancoInfo>();
            bancos = new List<BancoInfo>();
            while (sql_dataReader.Read())
            {
                bancos.Add(new BancoInfo
                {
                    Id = Convert.ToInt32(sql_dataReader["ID"]),
                    Nome = sql_dataReader["NOME"].ToString()
                });
            }
            dataGridView3.DataSource = bancos;
            dataGridView4.Rows.Clear();
            dataGridView4.Refresh();
        }


        private void CarregarWeb()
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM WEB", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            Webs = new List<WebInfo>();
            while (sql_dataReader.Read())
            {
                Webs.Add(new WebInfo
                {
                    Id = Convert.ToInt32(sql_dataReader["ID"]),
                    Nome = sql_dataReader["NOME"].ToString(),
                    Endereco = sql_dataReader["IP"].ToString(),
                    User = sql_dataReader["USER"].ToString(),
                    Senha = sql_dataReader["SENHA"].ToString()
                });
            }
            dataGridView5.DataSource = Webs;
            dataGridView5.Refresh();
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
               
            }
            else
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //PEGAR O ID DO BANCO SELECIONADO NA VIEW3
                int linha = e.RowIndex;
                int coluna = e.ColumnIndex;
                string valor = dataGridView3.Rows[linha].Cells[0].Value.ToString();
                string nome = dataGridView3.Rows[linha].Cells[1].Value.ToString();
                int IdBanco = Int32.Parse(valor);
                
                
                //Quando um banco é selecionado -- Habilita a adição de Owners/senha
                groupBox3.Visible = true;
                textBox6.Text = IdBanco.ToString();
                textBox7.Text = nome;

                //RECUPERAR OWNERS DO BANCO SELECIONADO
                // MessageBox.Show("SELECT * FROM BDOWNER WHERE BancoID = " + valor + "");

                dataGridView4.Rows.Clear();

                SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM BDOWNER WHERE BancoID = '" + valor + "'", conn);
                SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
                BancoInfo bd = null;
                while (sql_dataReader.Read())
                {
                    //Encontrar o Banco com esse ID
                    if (bd == null)
                    {
                        int idbanco = Convert.ToInt32(sql_dataReader["BancoID"]);
                        // MessageBox.Show("ID do banco:" + idbanco.ToString());
                        bd = bancos.Find(x => x.Id == idbanco);
                        //MessageBox.Show(bd.Nome);
                    }

                    //Adicionar Nele esses Owners/Senhas
                    string id_bdowner = sql_dataReader["ID"].ToString();
                    string owner = sql_dataReader["User"].ToString();
                    string senha = sql_dataReader["Senha"].ToString();
                    //MessageBox.Show("1" + owner + senha);
                    bd.Owners.Add(owner);
                    bd.Senhas.Add(senha);
                    //MessageBoxs.Show("2" + owner + senha);
                    dataGridView4.Rows.Add(id_bdowner,owner, senha);
                    dataGridView4.Refresh();
                }
            }
        }


        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //SELECIONA O REGISTRO NO BANCO E MODIFICA PARA NOVO VALOR
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            //PEGAR O ID DO BANCO SELECIONADO NA VIEW3
            int linha = e.RowIndex;
            int coluna = e.ColumnIndex;
            string valor = dataGridView3.Rows[linha].Cells[0].Value.ToString();
            int IdBanco = Int32.Parse(valor);

            //PEGAR O NOVO NOME DO BANCO SELECIONADO NA VIEW3
            string novoNome = dataGridView3.Rows[linha].Cells[1].Value.ToString();


            SQLiteCommand sql_cmd = new SQLiteCommand("UPDATE BANCO SET Nome='"+novoNome+"' WHERE Id="+IdBanco+"", conn);
            sql_cmd.ExecuteNonQuery();
        }

        //ADICIONA BANCO
        private void button20_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                warningMessage("Não é possível adicionar com o campo vazio!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o insert
                SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO BANCO (Nome) Values (@Nome)", conn);
                sql_cmd.Parameters.AddWithValue("Nome", textBox2.Text.Trim());
                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Novo banco adicionado!");
                    textBox2.Clear();
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }
                Carregar();
            }
        }

        //ADICIONA OWNER
        private void button21_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(textBox3.Text)) 
            {
                warningMessage("Não é possível adicionar com campo vazio!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o insert
                SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO BDOWNER (BancoId, User, Senha)  Values (@BancoId, @User, @Senha)", conn);
               // sql_cmd.Parameters.AddWithValue("BancoId", textBox5.Text.Trim());
               // sql_cmd.Parameters.AddWithValue("User", textBox1.Text.Trim());
               // sql_cmd.Parameters.AddWithValue("Senha", textBox3.Text.Trim());

                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Novo Owner adicionado!");
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }
                Carregar();
            }
        }

        //BOTÃO DE EXCLUSÃO
        private void button22_Click(object sender, EventArgs e)
        {
            //RECUPERAR O BANCO SELECIONADO
            //TRATAR: NENHUM BANCO SELECIONADO
            if (String.IsNullOrEmpty(textBox6.Text) || String.IsNullOrEmpty(textBox6.Text))
            {
                warningMessage("Não é possível adicionar com campo vazio!");
            }
            else
            {
                int idBanco = Convert.ToInt32(textBox6.Text);
                string nomeBanco = textBox7.Text;

                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o delete
                //quando deletar um banco que possua owners/senha -> precisa deletar owners tb
                SQLiteCommand sql_cmd = new SQLiteCommand("DELETE FROM BANCO WHERE ID= @CODIGO", conn);
                sql_cmd.Parameters.AddWithValue("CODIGO", idBanco);
                SQLiteCommand sql_cmd2 = new SQLiteCommand("DELETE FROM BDOWNER WHERE BancoID= @BANCOID", conn);
                sql_cmd2.Parameters.AddWithValue("BANCOID", idBanco);

                //CONFIRMAR EXCLUSÃO
                if(MessageBox.Show("Certeza que deseja deletar o banco: "+nomeBanco, "Confirmar Exclusão", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {

                    //EXCLUIR
                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                        sql_cmd2.ExecuteNonQuery();
                        informativeMessage("Banco Excluído dos Registros!");
                    }
                    catch (Exception ex)
                    {
                        errorMessage("Erro ao excluir registro: " + ex.Message);
                    }
                    Carregar();
                }
                else
                {

                } 
 
            }
            
            
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //Habilitar Edição
            textBox7.Enabled = true;
            textBox7.ReadOnly = false;
            //Colocar o foco do mouse para o textbox
            textBox7.Focus();
            button23.Visible = true;
            //Dar update no registro

            //Finalizar Edição
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            //VALIDA a ALTERACAO
            if (String.IsNullOrEmpty(textBox7.Text))
            {
                warningMessage("Não é possível alterar com o campo vazio!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o update
                SQLiteCommand sql_cmd = new SQLiteCommand("UPDATE BANCO SET NOME = @NOME WHERE ID = @CODIGO", conn);
                sql_cmd.Parameters.AddWithValue("NOME", textBox7.Text.Trim());
                sql_cmd.Parameters.AddWithValue("CODIGO", textBox6.Text.Trim());
                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Banco Alterado!");
                    textBox2.Clear();
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }

                textBox7.Enabled = false;
                button23.Visible = false;
                Carregar();
            }

        }

        private void textBox7_Leave(object sender, EventArgs e)
        {
            //Botão ok é o focado
            if (button23.Focused == true)
            {

            }
            else
            {
                textBox7.Enabled = false;
                button23.Visible = false;
            }

        }

        private void button25_Click(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBox4.Text)) || (String.IsNullOrEmpty(textBox4.Text)))
            {
                warningMessage("Não é possível adicionar com o campo OWNER ou SENHA vazios!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o insert
                SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO BDOWNER (BancoID, User, Senha) Values (@BancoID, @User, @Senha)", conn);
                sql_cmd.Parameters.AddWithValue("BancoID", textBox6.Text.Trim());
                sql_cmd.Parameters.AddWithValue("User", textBox5.Text.Trim());
                sql_cmd.Parameters.AddWithValue("Senha", textBox4.Text.Trim());

                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Novo OWNER/SENHA adicionado!");
                    textBox4.Clear();
                    textBox5.Clear();
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }
                Carregar();
            }
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //PEGAR O ID DO BDOWNER SELECIONADO NA VIEW4
                int linha = e.RowIndex;
                int coluna = e.ColumnIndex;
                string idBdOwnerStr = dataGridView4.Rows[linha].Cells[0].Value.ToString();
                string nome = dataGridView4.Rows[linha].Cells[1].Value.ToString();
                string senha = dataGridView4.Rows[linha].Cells[2].Value.ToString();
                int idBdOwner = Int32.Parse(idBdOwnerStr);


                //Atualiza a exibicao do groupbox Owners/senha
                textBox1.Text = nome;
                textBox3.Text = senha;
                textBox8.Text = idBdOwnerStr; //ESTE EH INVISIVEL


                //RECUPERAR OWNERS DO BANCO SELECIONADO
                // MessageBox.Show("SELECT * FROM BDOWNER WHERE BancoID = " + valor + "");

                groupBox6.Refresh();
            }
        }

        //DELETAR O BDOWNER SELECIONADO
        private void button24_Click(object sender, EventArgs e)
        {
            String nome, senha, idBdOwnerStr;

            nome = textBox1.Text;
            senha = textBox3.Text;
            idBdOwnerStr = textBox8.Text; 

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            //Dá o delete
            //quando deletar um banco que possua owners/senha -> precisa deletar owners tb
            SQLiteCommand sql_cmd = new SQLiteCommand("DELETE FROM BDOWNER WHERE ID= @CODIGO", conn);
            sql_cmd.Parameters.AddWithValue("CODIGO", idBdOwnerStr);

            //CONFIRMAR EXCLUSÃO
            if (MessageBox.Show("Certeza que deseja deletar o registro: " + nome + "/" + senha, "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                //EXCLUIR
                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Registro de Owner Excluído!");
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao excluir registro: " + ex.Message);
                }
                Carregar();
            }
            else
            {

            }
            Carregar();
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            //Habilitar Edição
            textBox1.Enabled = true;
            textBox1.ReadOnly = false;
            textBox3.Enabled = true;
            textBox3.ReadOnly = false;
            //Colocar o foco do mouse para o textbox
            textBox1.Focus();
            button27.Visible = true;
            //Dar update no registro

            //Finalizar Edição
        }

        private void button27_Click(object sender, EventArgs e)
        {
            //VALIDA a ALTERACAO
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox3.Text))
            {
                warningMessage("Não é possível alterar com o campo vazio!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o update
                SQLiteCommand sql_cmd = new SQLiteCommand("UPDATE BDOWNER SET USER = @USER, SENHA = @SENHA WHERE ID = @CODIGO", conn);
                sql_cmd.Parameters.AddWithValue("USER", textBox1.Text.Trim());
                sql_cmd.Parameters.AddWithValue("SENHA", textBox3.Text.Trim());
                sql_cmd.Parameters.AddWithValue("CODIGO", textBox8.Text.Trim());
                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Owner/Senha Alterado!");
                    textBox1.Clear();
                    textBox3.Clear();
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }

                textBox1.Enabled = false;
                textBox3.Enabled = false;
                button27.Visible = false;
                Carregar();
            }

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            //Botão ok é o focado ou o textbutton3
            if ((button27.Focused == true)|| (textBox3.Focused==true))
            {

            }
            else
            {
                textBox1.Enabled = false;
                textBox3.Enabled = false;
                button27.Visible = false;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            //Botão ok é o focado ou o textbutton3
            if ((button27.Focused == true)|| (textBox1.Focused==true))
            {

            }
            else
            {
                textBox1.Enabled = false;
                textBox3.Enabled = false;
                button27.Visible = false;
            }
        }


        public List<string> LoadTNSNames(string OracleHomeRegistryKey)
        {
            List<string> DBNamesCollection = new List<string>();
            string RegExPattern = @"[\n][\s]*[^\(][a-zA-Z0-9_.]+[\s]*=[\s]*\(";
            string strTNSNAMESORAFilePath = OracleHomeRegistryKey;

            if (!strTNSNAMESORAFilePath.Equals(""))
            {
                //check out that file does physically exists
                System.IO.FileInfo fiTNS = new System.IO.FileInfo(strTNSNAMESORAFilePath);
                if (fiTNS.Exists)
                {
                    if (fiTNS.Length > 0)
                    {
                        //read tnsnames.ora file
                        int iCount;
                        for (iCount = 0; iCount < Regex.Matches(
                            System.IO.File.ReadAllText(fiTNS.FullName),
                            RegExPattern).Count; iCount++)
                        {
                            DBNamesCollection.Add(Regex.Matches(
                                System.IO.File.ReadAllText(fiTNS.FullName),
                                RegExPattern)[iCount].Value.Trim().Substring(0,
                                Regex.Matches(System.IO.File.ReadAllText(fiTNS.FullName),
                                RegExPattern)[iCount].Value.Trim().IndexOf(" ")));
                        }
                    }
                }
            }

            for (int i = 0; i < DBNamesCollection.Count; i++)
            {
                string s = DBNamesCollection[i];
                if (s.Contains("="))
                {
                    DBNamesCollection[i]=s.Trim();
                    s = DBNamesCollection[i];
                    DBNamesCollection[i] = s.Remove(s.Length - 1);
                    //MessageBox.Show(DBNamesCollection[i]+"!!!");
                }
            }  
            return DBNamesCollection;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            List<string> nome = LoadTNSNames(tnsnamesBox.Text);
            string text = "";
            
            foreach (string s in nome){
                text += s + ", ";
            }

            if (MessageBox.Show("Confirmar Adição de: \n" + text, "Confirmar Adição", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO BANCO (NOME) Values (@NOME)", conn);

                foreach (string s in nome)
                {
                    sql_cmd.Parameters.AddWithValue("NOME", s);
                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorMessage("Erro ao salvar registro: " + ex.Message);
                    }
                }
                Carregar();
            }
            else
            {

            }
            
        }

        private void websToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            changePanel(pnl_webConfig);
            CarregarWeb();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox11.Text) || String.IsNullOrEmpty(textBox12.Text) || String.IsNullOrEmpty(textBox13.Text) || String.IsNullOrEmpty(textBox14.Text))
            {
                warningMessage("Não é possível adicionar com o campo vazio!");
            }
            else
            {
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o insert
                SQLiteCommand sql_cmd = new SQLiteCommand("INSERT INTO WEB (IP, NOME, USER, SENHA) Values (@Ip, @Nome, @User, @Senha)", conn);
                sql_cmd.Parameters.AddWithValue("Ip", textBox11.Text.Trim());
                sql_cmd.Parameters.AddWithValue("Nome", textBox12.Text.Trim());
                sql_cmd.Parameters.AddWithValue("User", textBox13.Text.Trim());
                sql_cmd.Parameters.AddWithValue("Senha", textBox14.Text.Trim());

                try
                {
                    sql_cmd.ExecuteNonQuery();
                    informativeMessage("Nova WEB adicionada!");
                    textBox11.Clear();
                    textBox12.Clear();
                    textBox13.Clear();
                    textBox14.Clear();
                }
                catch (Exception ex)
                {
                    errorMessage("Erro ao salvar registro: " + ex.Message);
                }
                CarregarWeb();
            }
        }

        private void dataGridView5_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            textBox15.Text = dataGridView5.Rows[e.RowIndex].Cells[0].Value.ToString(); //ID
            textBox9.Text = dataGridView5.Rows[e.RowIndex].Cells[2].Value.ToString(); //IP
            textBox10.Text = dataGridView5.Rows[e.RowIndex].Cells[1].Value.ToString(); //NOME
        }

        private void button29_Click(object sender, EventArgs e)
        {
           //deletar
 
                //Abre conexão com o banco
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                //Dá o delete
                //quando deletar um banco que possua owners/senha -> precisa deletar owners tb
                SQLiteCommand sql_cmd = new SQLiteCommand("DELETE FROM WEB WHERE ID= @CODIGO", conn);
                sql_cmd.Parameters.AddWithValue("CODIGO", textBox15.Text);

                //CONFIRMAR EXCLUSÃO
                if (MessageBox.Show("Certeza que deseja deletar a web: " + textBox10.Text, "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    //EXCLUIR
                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                        informativeMessage("Web Excluída dos Registros!");
                    }
                    catch (Exception ex)
                    {
                        errorMessage("Erro ao excluir registro: " + ex.Message);
                    }
                    Carregar();
                }
                else
                {

                }


                CarregarWeb();
                
        }


        public void DeveloperModeOn()
        {
            label37.Text = "Developer Mode";
            nOVOBancosToolStripMenuItem.Visible = true;
            websToolStripMenuItem1.Visible = true;
            errosBancoNOVOToolStripMenuItem.Visible = true;
            flowLayoutPanel3.Visible = true;
            tableLayoutPanel47.Visible = true;
        }

        private void button35_Click(object sender, EventArgs e)
        {

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "Fluxo_Atualizacao_V11.pdf";
            processInfo.WorkingDirectory = Environment.CurrentDirectory + "\\Arquivos\\Fluxo\\";
            Process.Start(processInfo);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            Process.Start(tnsnamesBox.Text);
        }

        private void button37_Click(object sender, EventArgs e)
        {
          
        
           OpenFileDialog fbd = new OpenFileDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.tnsnamesBox.Text = fbd.FileName;
            }
        }


    }
}
