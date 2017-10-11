using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TopDownAutomate
{
    public class Navegador
    {
        private IWebDriver driver;
        private Usuario user;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        private bool tryAgain;
        private int codNavegador;
        private string mainWindowHandle;
       
        public Navegador(int codNav)
        {
            codNavegador = codNav;
        }
        public void SetupTest()
        {
            //SE É FIREFOX
            if (codNavegador == 0)
            {
                driver = new FirefoxDriver();
            }
            else
            {
                if (codNavegador == 1)
                {
                    //SE É CHROME
                    ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(@"Arquivos\WebDriver\chromedriver_win32");
                    chromeDriverService.HideCommandPromptWindow = true;
                    driver = new ChromeDriver(chromeDriverService);
                }
            }


            baseURL = "http://sac.topdown.com.br/";
            verificationErrors = new StringBuilder();
           
        }


        public void FazerLogin (Usuario u)
        {
            this.SetupTest();
            driver.Navigate().GoToUrl(baseURL + "/ace/ace001a.asp");
            do
            {
                driver.FindElement(By.Name("Login")).Clear();
                driver.FindElement(By.Name("Login")).SendKeys(u.Username);
                driver.FindElement(By.Name("Senha")).Clear();
                driver.FindElement(By.Name("Senha")).SendKeys(u.Password);
                driver.FindElement(By.Name("Logar")).Click();
            } while (driver.Url == "http://sac.topdown.com.br//ace/ace001a.asp?pmsg=Ocorreu%20um%20erro%20ao%20entrar.%20Favor%20informar%20login/senha%20novamente.");

            mainWindowHandle = driver.CurrentWindowHandle;

        }

        public void fechar()
        {
            this.driver.Close();
            this.driver = null;
        }

        public void TheAcessoDetalhesCA(int numCA, Usuario u, mainWindow janela)
        {
            user = u;
            FazerLogin(u);

            //Configura Tempo de Espera de um item aparecer na Tela
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));

            //Chama o realizador de Ações
            Actions act = new Actions(driver);

            //Acessa o Menu de Controle (DropDown)
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(".//*[@id='menu_dropdown']/li/a")));
            IWebElement controle_dropdown = driver.FindElement(By.XPath(".//*[@id='menu_dropdown']/li/a"));
            act.MoveToElement(controle_dropdown).Build().Perform();

            //Clica em Pesquisar Controle
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(".//*[@id='menu_dropdown']/li/ul/li[2]/a")));
            IWebElement Pesquisarcontrole_dropdownItem = driver.FindElement(By.XPath(".//*[@id='menu_dropdown']/li/ul/li[2]/a"));
            act.MoveToElement(Pesquisarcontrole_dropdownItem).Build().Perform();
            act.Click().Perform();

            //Muda o foco para o Frame que possui as informaçoes (e não o frame do menu)
            driver.SwitchTo().Frame(0);

            //Escrever o numero do Controle no campo e aperta Enter
            wait.Until(ExpectedConditions.ElementExists(By.Name("num_controle")));
            IWebElement noCa_txt = driver.FindElement(By.Name("num_controle"));
            noCa_txt.SendKeys(numCA.ToString());
            act.SendKeys(OpenQA.Selenium.Keys.Enter).Perform();

            //Verfica se o numero do controle existe
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("html")));
            IWebElement situacaoControle = driver.FindElement(By.XPath(".//*[@id='unique_id']/tbody/tr/td[7]"));
            string detalhes_btn_xpath;
            if (situacaoControle.Text == "Encerrado")
                detalhes_btn_xpath = ".//*[@id='unique_id']/tbody/tr/td[1]/a[2]/img";

            else
                detalhes_btn_xpath = ".//*[@id='unique_id']/tbody/tr/td[1]/a[6]/img";

            IWebElement detalhes_btn = driver.FindElement(By.XPath(detalhes_btn_xpath));
            detalhes_btn.Click();

        }

        public Controle getInformacoesDaPagDetalhes(int numCA, Usuario u, mainWindow janela)
        {
            
            //Configura Tempo de Espera de um item aparecer na Tela
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));

            //Chama o realizador de Ações
            Actions act = new Actions(driver);

            //Acha a aba dso Objetos e Clica nela
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//*[@id='tab2']")));
            IWebElement obj_aba = driver.FindElement(By.XPath(".//*[@id='tab2']"));
            act.MoveToElement(obj_aba).Click().Perform();

            //Recupera a quantidade, tipo e versao de Arquivos - linkados aos produtos
            List<Arquivo> arqs = new List<Arquivo>();
            bool produtoArquivoFlag = true;
            for (int i = 2; produtoArquivoFlag; i++)
            {
                try
                {
                    IWebElement we_nomeProdutoArquivos = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[2]/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/table[1]/tbody/tr[" + i + "]/td[1]"));
                    bool arquivoFlag = true;
                    for (int k = 1; arquivoFlag; k++)
                    {
                        try
                        {
                            IWebElement we_arquivo = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[2]/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/table[1]/tbody/tr[" + i + "]/td[2]/span[" + k + "]"));
                            arqs.Add(new Arquivo(we_arquivo.Text, we_nomeProdutoArquivos.Text));
                        }
                        catch (NotFoundException)
                        {
                            //Não há mais arquivo na linha do produto
                            arquivoFlag = false;
                        }
                    }
                }
                catch (NotFoundException)
                {
                    //Não há mais produto na tabela
                    produtoArquivoFlag = false;
                }
            }


            //Recupera a quantidade de Evidências
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[2]/td/table/tbody")));
            int qtdEvidencias = this.getQuantidadeDeEvidencias();

            //Recupera o numero do Controle
            string numeroControle;
            IWebElement we_numeroControle = driver.FindElement(By.XPath("html/body/table/tbody/tr/td/table/tbody/tr/td[1]/font[2]"));
            numeroControle = we_numeroControle.Text;

            //Recupera o nome do Responsavel por Testes
            string responsavelTeste;
            IWebElement we_responsavelTeste = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[2]/td/font"));
            responsavelTeste = we_responsavelTeste.Text.Split(' ')[0];

            //Recupera o nome do Responsavel pela Distribuição
            string responsavelDistribuicao;
            IWebElement we_responsavelDistribuicao = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[3]/td/font"));
            responsavelDistribuicao = we_responsavelDistribuicao.Text.Split(' ')[0];

            //Recupera a Situação
            string situacao;
            IWebElement we_situacao = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/font"));
            situacao = we_situacao.Text;

            //Recupera 'Registrado Em'
            string registradoEm;
            IWebElement we_registradoEm = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[2]/table/tbody/tr[2]/td/font"));
            registradoEm = we_registradoEm.Text;

            //Recupera o Nome do Analista
            string nomeAnalistaResponsavel;
            IWebElement we_nomeAnalistaResponsavel = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[1]/td/font"));
            nomeAnalistaResponsavel = we_nomeAnalistaResponsavel.Text;

            //Recupera Descrição do Controle
            string descricaoControle;
            IWebElement we_descricaoControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[1]/td/font"));
            descricaoControle = we_descricaoControle.Text;

            //Recupera Parâmetros do Controle
            string parametrosControle;
            IWebElement we_parametrosControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[3]/td/font"));
            parametrosControle = we_parametrosControle.Text;

            //Recupera Impacto do Controle
            string impactoControle;
            IWebElement we_impactoControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[5]/td/font"));
            impactoControle = we_impactoControle.Text;

            //Recupera Produto(s)
            List<Produto> Produtos = new List<Produto>();
            bool produtoFlag = true;
            for (int i = 2; produtoFlag; i++)
            {
                try
                {
                    IWebElement we_nomeProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[1]"));
                    IWebElement we_versaoProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[2]"));
                    IWebElement we_moduloProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[3]"));
                    IWebElement we_funcaoProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[4]"));
                    Produtos.Add(new Produto(we_nomeProduto.Text, we_versaoProduto.Text, we_moduloProduto.Text, we_funcaoProduto.Text));
                }
                catch (NotFoundException)
                {
                    //Não há mais produtos
                    produtoFlag = false;
                }
            }

            //Recupera Cliente(s)
            List<Cliente> Clientes = new List<Cliente>();
            bool clienteFlag = true;
            for (int i = 2; clienteFlag; i++)
            {
                try
                {
                    IWebElement we_nomeCliente = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[2]/table/tbody/tr[" + i + "]/td[1]"));
                    IWebElement we_sacCliente = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[2]/table/tbody/tr[" + i + "]/td[2]"));
                    Clientes.Add(new Cliente(we_nomeCliente.Text, we_sacCliente.Text));
                }
                catch (NotFoundException)
                {
                    //Não há mais clientes
                    clienteFlag = false;
                }
            }



            Controle controle = new Controle(Int32.Parse(numeroControle), nomeAnalistaResponsavel, descricaoControle, Clientes, Produtos, u);
            controle.Objetos = arqs;
          //  controle.ParteCaminhoArquivos = DiretorioParcialarq;
            controle.ResponsavelTeste = responsavelTeste;
            controle.ResponsavelDistribuicao = responsavelDistribuicao;
            controle.RegistradoEm = registradoEm;
            controle.Situacao = situacao;
            controle.Impacto = impactoControle;
            controle.Parametros = parametrosControle;

            return controle;

        }

        public Controle getInformacoesDaPagDetalhes(Usuario u, mainWindow janela)
        {

            //Configura Tempo de Espera de um item aparecer na Tela
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));

            if ((driver == null))
            {
                return null;
            }


            driver.Manage().Window.Maximize();

            try
            {
                driver.SwitchTo().Frame(0);
            }
            catch (Exception)
            {
               // return null;
            }

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("html/body/table/tbody/tr/td/table/tbody/tr/td[1]/font[1]")));
                IWebElement we_labelCA = driver.FindElement(By.XPath("html/body/table/tbody/tr/td/table/tbody/tr/td[1]/font[1]"));
                if ((we_labelCA.Text == "Controle de Atualização: ") || (we_labelCA.Text == "Controle de Atualização:"))
                {
                    //bla
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }


            //Chama o realizador de Ações
            Actions act = new Actions(driver);

            //Acha a aba dso Objetos e Clica nela
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//*[@id='tab2']")));
            IWebElement obj_aba = driver.FindElement(By.XPath(".//*[@id='tab2']"));
            act.MoveToElement(obj_aba).Click().Perform();


            //Recupera a quantidade, tipo e versao de Arquivos - linkados aos produtos
            List<Arquivo> arqs = new List<Arquivo>();
            bool produtoArquivoFlag = true;
            for (int i = 2; produtoArquivoFlag; i++)
            {
                try
                {
                    IWebElement we_nomeProdutoArquivos = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[2]/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/table[1]/tbody/tr["+i+"]/td[1]"));
                    bool arquivoFlag = true;
                    for (int k = 1; arquivoFlag; k++)
                    {
                        try
                        {
                            IWebElement we_arquivo = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[2]/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/table[1]/tbody/tr[" + i + "]/td[2]/span["+k+"]"));
                            arqs.Add(new Arquivo(we_arquivo.Text, we_nomeProdutoArquivos.Text));
                        }
                        catch (NotFoundException)
                        {
                            //Não há mais arquivo na linha do produto
                            arquivoFlag = false;
                        }
                    }
                  }
                catch (NotFoundException)
                {
                    //Não há mais produto na tabela
                    produtoArquivoFlag = false;
                }
            }


            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[1]/td/table[1]/tbody/tr[2]/td[2]")));
            int qtdArqs = this.getQuantidadeDeArquivos();
            List<String> DiretorioParcialarq = getDiretorioParcialdosArquivos();
            
            //Recupera a quantidade de Evidências
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[2]/td/table/tbody")));
            int qtdEvidencias = this.getQuantidadeDeEvidencias();

            //Recupera o numero do Controle
            string numeroControle;
            IWebElement we_numeroControle = driver.FindElement(By.XPath("html/body/table/tbody/tr/td/table/tbody/tr/td[1]/font[2]"));
            numeroControle = we_numeroControle.Text;


            //Recupera o nome do Responsavel por Testes
            string responsavelTeste;
            IWebElement we_responsavelTeste = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[2]/td/font"));
            responsavelTeste = we_responsavelTeste.Text.Split(' ')[0];

            //Recupera o nome do Responsavel pela Distribuição
            string responsavelDistribuicao;
            IWebElement we_responsavelDistribuicao = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[3]/td/font"));
            responsavelDistribuicao = we_responsavelDistribuicao.Text.Split(' ')[0];

            //Recupera a Situação
            string situacao;
            IWebElement we_situacao = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[2]/table/tbody/tr[1]/td/font"));
            situacao = we_situacao.Text;

            //Recupera 'Registrado Em'
            string registradoEm;
            IWebElement we_registradoEm = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[2]/table/tbody/tr[2]/td/font"));
            registradoEm = we_registradoEm.Text;

            //Recupera o Nome do Analista
            string nomeAnalistaResponsavel;
            IWebElement we_nomeAnalistaResponsavel = driver.FindElement(By.XPath("html/body/form/table[1]/tbody/tr/td/table/tbody/tr[2]/td/div[1]/table/tbody/tr[1]/td/font"));
            nomeAnalistaResponsavel = we_nomeAnalistaResponsavel.Text;

            //Recupera Descrição do Controle
            string descricaoControle;
            IWebElement we_descricaoControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[1]/td/font"));
            descricaoControle = we_descricaoControle.Text;

            //Recupera Parâmetros do Controle
            string parametrosControle;
            IWebElement we_parametrosControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[3]/td/font"));
            parametrosControle = we_parametrosControle.Text;

            //Recupera Impacto do Controle
            string impactoControle;
            IWebElement we_impactoControle = driver.FindElement(By.XPath("html/body/form/table[2]/tbody/tr/td/table/tbody/tr[5]/td/font"));
            impactoControle = we_impactoControle.Text;

            //Recupera Produto(s)
            List<Produto> Produtos = new List<Produto>();
            bool produtoFlag = true;
            for (int i = 2; produtoFlag; i++)
            {
                try
                {
                    IWebElement we_nomeProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[1]"));
                    IWebElement we_versaoProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[2]"));
                    IWebElement we_moduloProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[3]"));
                    IWebElement we_funcaoProduto = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[1]/table/tbody/tr[" + i + "]/td[4]"));
                    Produtos.Add(new Produto(we_nomeProduto.Text, we_versaoProduto.Text, we_moduloProduto.Text, we_funcaoProduto.Text));
                }
                catch (NotFoundException)
                {
                    //Não há mais produtos
                    produtoFlag = false;
                }
            }

            //Recupera Cliente(s)
            List<Cliente> Clientes = new List<Cliente>();
            bool clienteFlag = true;
            for (int i = 2; clienteFlag; i++)
            {
                try
                {
                    IWebElement we_nomeCliente = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[2]/table/tbody/tr[" + i + "]/td[1]"));
                    IWebElement we_sacCliente = driver.FindElement(By.XPath("html/body/form/table[3]/tbody/tr/td/table[1]/tbody/tr/td/div[2]/table/tbody/tr[" + i + "]/td[2]"));
                    Clientes.Add(new Cliente(we_nomeCliente.Text, we_sacCliente.Text));
                }
                catch (NotFoundException)
                {
                    //Não há mais clientes
                    clienteFlag = false;
                }
            }



            Controle controle = new Controle(Int32.Parse(numeroControle), nomeAnalistaResponsavel, descricaoControle, Clientes, Produtos, u);
            controle.Objetos = arqs;
            //controle.ParteCaminhoArquivos = DiretorioParcialarq;
            controle.ResponsavelTeste = responsavelTeste;
            controle.ResponsavelDistribuicao = responsavelDistribuicao;
            controle.RegistradoEm = registradoEm;
            controle.Situacao = situacao;
            controle.Impacto = impactoControle;
            controle.Parametros = parametrosControle;
            return controle;
        }


        private int getQuantidadeDeArquivos()
        {
            int qtd = 0;

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> arqs = driver.FindElements(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[1]/td/table[1]/tbody/tr[2]/td[2]/span"));

            foreach (IWebElement arq in arqs)
            {
                qtd++;
            }
            return qtd;
        }

        private List<String> getDiretorioParcialdosArquivos()
        {
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> arqs = driver.FindElements(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[1]/td/table[1]/tbody/tr[2]/td[2]/span"));
            List<String> diretorioParcialArquivos = new List<String>();
            

            foreach (IWebElement arq in arqs)
            {
                String diretorioParcial = arq.Text.Split(':')[1]; //separa "BD: Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql" - Versão 1.41 e pega " Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql - Versão 1.41"
                diretorioParcial = diretorioParcial.Remove(0, 1); //Remove o espaço inicial que sobra do split
                int i = diretorioParcial.IndexOf(" - Versão"); //indica em que índice começa a parte que não faz parte do caminho, mas que explicita a versão
                diretorioParcial = diretorioParcial.Remove(i); //remove a partir do indice i
                diretorioParcialArquivos.Add(diretorioParcial);

            }

            return diretorioParcialArquivos;
        }

        private int getQuantidadeDeEvidencias()
        {
            int qtd = 0;

            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> arqs = driver.FindElements(By.XPath(".//*[@id='divObjetos']/table/tbody/tr[2]/td/table/tbody/tr"));

            foreach (IWebElement arq in arqs)
            {
                qtd++;
            }
            return qtd-1;
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }

        public void Recuperar()
        {
            driver.SwitchTo().ActiveElement();
            this.driver.Manage().Window.Maximize();
            //string test="";

            //verifica quantas abas estão abertas
            int abasAbertas = driver.WindowHandles.Count;
            //Verifica em qual aba está
            driver.SwitchTo().ActiveElement();
            //int abaNum = 0;
            //for (int i = 0; i < abasAbertas; i++)
            //{
            //    if (driver.WindowHandles[i] != mainWindowHandle)
            //    {
            //        driver.SwitchTo().Window(driver.WindowHandles[i]);
            //        driver.Close();
            //        abasAbertas--;
            //    }
            //}


            while (driver.WindowHandles.Count > 1)
            {
                if (driver.WindowHandles[driver.WindowHandles.Count-1] != mainWindowHandle)
                {
                    driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
                    driver.Close();
                    abasAbertas--;
                }
            }

            ////dá tabs até a aba principal
            //while (driver.CurrentWindowHandle != mainWindowHandle)
            //{
            //    controlTab();
            //    driver.SwitchTo().ActiveElement();
            //}


            //for (int i = 0; i < driver.WindowHandles.Count; i++)
            //{
            //    driver.SwitchTo().Window(driver.WindowHandles[i]);
            //    if (driver.WindowHandles[i] == mainWindowHandle)
            //    {
            //        test += "Está no corrente"+Environment.NewLine;
            //        //Já está na janela desejada
            //    }
            //    else
            //    {
            //        test += "não está na corrente"+Environment.NewLine;
            //        act.KeyDown(OpenQA.Selenium.Keys.Control);
            //        act.SendKeys(OpenQA.Selenium.Keys.Tab);
            //        act.Build();
            //        act.Perform();
            //    }
            //}

          
             //MessageBox.Show(test);
        }

        public void controlTab()
        {
            Actions act = new Actions(driver);
            act.KeyDown(OpenQA.Selenium.Keys.Control);
            act.SendKeys(OpenQA.Selenium.Keys.Tab);
            act.KeyUp(OpenQA.Selenium.Keys.Control);
            act.Perform();
        }

        public bool driverIsOpen()
        {
            try
            {
                string test = driver.CurrentWindowHandle;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
