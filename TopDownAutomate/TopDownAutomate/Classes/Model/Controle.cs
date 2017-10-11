using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace TopDownAutomate
{
    public class Controle
    {
        Usuario user;

        int numero;          //numero do controle
        String solicitante;    //nome do Analista responsável
        String responsavelTeste; //nome do responsável por Testes
        String responsavelDistribuicao; //nome do responsável por Distribuicao
        String descricao; //Descrição do CA
        string parametros;
        string impacto;
        string situacao;
        string registradoEm;

        List<Produto> produtos;
        List<Cliente> clientes;
        List<Arquivo> objetos;
        List<FileStream> evidencias;
        List<String> parteCaminhoArquivos; //Parte do caminho dos arquivos = Ex Procedures/seila/seila.qlqr
        List<String> CaminhoArquivos; //Caminho do Cliente na rede + parte da caminho dos arquivos 
        int quantidadeArquivos;

        //INFORMAÇÕES DEDUZIADAS PELAS INFO INICIAIS (pasta de controle)
        String enderecoPastaPacotes;    //endereco da pasta onde ficam os pacotes Ex.: "K:\Pacotes"
        String numeroPastaCatalogo; //ex.: "48701-48750" (a pasta "48731-Marcos" ficaria dentro desta)
        String nomePasta; //nome da pasta do controle (e.g. 49454-Edgard)
        public String enderecoPasta; //  x = enderecoPastaPacotes + numeroPastaCatalogo + numero + "-" + solicitante       
        DirectoryInfo pasta;

        public Controle(int numero, string nomeAnalista, string descricao, List<Cliente> Clts, List<Produto> Prdts, Usuario u)
        {
            user = u;
            enderecoPastaPacotes = user.PastaPacotes;
            this.Descricao = descricao;
            this.Numero = numero;
            this.solicitante = tirarCaracteresEspeciais(nomeAnalista);
            this.Clientes = Clts;
            this.Produtos = Prdts;
            numeroPastaCatalogo = geraNomePastaCatalogo();
            //Já pegou o numero da pasta catálogo 
            //-- procurar dentro da pasta catálogo se a pasta do controle ja existe
            if (Directory.Exists(enderecoPastaPacotes + "\\" + numeroPastaCatalogo) == false)
                Directory.CreateDirectory(enderecoPastaPacotes + "\\" + numeroPastaCatalogo);
            string[] diretorios = Directory.GetDirectories(enderecoPastaPacotes + "\\" + numeroPastaCatalogo);
            Boolean pastaExiste = false;
            List<DirectoryInfo> directoryList = new List<DirectoryInfo>();

            foreach (string diretorio in diretorios)
            {
                directoryList.Add(new DirectoryInfo(diretorio));
            }

            foreach (DirectoryInfo diretorio in directoryList)
            {
                if (diretorio.Name.Contains(Numero.ToString()))
                {
                    pastaExiste = true;
                    pasta = diretorio;
                    enderecoPasta = pasta.FullName;
                }
            }

            if (pastaExiste == false)
            {
                nomePasta = geraNomePasta();
                enderecoPasta = enderecoPastaPacotes;
                enderecoPasta += "\\";
                enderecoPasta += numeroPastaCatalogo;
                enderecoPasta += "\\";
                enderecoPasta += nomePasta;
            }

        }

        string tirarCaracteresEspeciais(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public Usuario Usuario
        {
            get { return user; }
            set { user = value; }
        }

        public int Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        public List<string> ParteCaminhoArquivos
        {
            get { return parteCaminhoArquivos; }
            set { parteCaminhoArquivos = value; }
        }

        public string ResponsavelTeste
        {
            get { return responsavelTeste; }
            set { responsavelTeste = value; }
        }

        public string ResponsavelDistribuicao
        {
            get
            {
                return responsavelDistribuicao;
            }

            set
            {
                responsavelDistribuicao = value;
            }
        }

        public string Situacao
        {
            get
            {
                return situacao;
            }

            set
            {
                situacao = value;
            }
        }

        public string RegistradoEm
        {
            get
            {
                return registradoEm;
            }

            set
            {
                registradoEm = value;
            }
        }

        public string Descricao
        {
            get
            {
                return descricao;
            }

            set
            {
                descricao = value;
            }
        }

        public string Parametros
        {
            get
            {
                return parametros;
            }

            set
            {
                parametros = value;
            }
        }

        public string Impacto
        {
            get
            {
                return impacto;
            }

            set
            {
                impacto = value;
            }
        }

        public List<Produto> Produtos
        {
            get
            {
                return produtos;
            }

            set
            {
                produtos = value;
            }
        }

        public List<Cliente> Clientes
        {
            get
            {
                return clientes;
            }

            set
            {
                clientes = value;
            }
        }

        public List<FileStream> Evidencias
        {
            get
            {
                return evidencias;
            }

            set
            {
                evidencias = value;
            }
        }

        public List<Arquivo> Objetos
        {
            get
            {
                return objetos;
            }

            set
            {
                objetos = value;
            }
        }

        public string geraNomePastaCatalogo()
        {
            String numeroCA = Numero.ToString();
            int num_catalog_dig1e2, num_catalog_dig3e4e5;
            String temp = Char.ToString(numeroCA[3]) + Char.ToString(numeroCA[4]);
            String temp2 = Char.ToString(numeroCA[0]) + Char.ToString(numeroCA[1]) + Char.ToString(numeroCA[2]);
            String nomeDaPasta_catalogo_temp;
            num_catalog_dig1e2 = Int32.Parse(temp);
            num_catalog_dig3e4e5 = Int32.Parse(temp2);


            if (num_catalog_dig1e2 >= 01 && num_catalog_dig1e2 <= 50)
            {
                nomeDaPasta_catalogo_temp = temp2 + "01" + "-" + temp2 + "50";
                Console.WriteLine("nome da pasta-catalogo: " + nomeDaPasta_catalogo_temp);
            }
            else
            {
                if (num_catalog_dig1e2 == 00) //final 00 ex: 51500 (pasta é 51451-51500)
                {
                    nomeDaPasta_catalogo_temp = (Int32.Parse(temp2) - 1).ToString() + "51" + "-" + numeroCA;
                }
                else //entre 51-99
                {
                    nomeDaPasta_catalogo_temp = temp2 + "51" + "-" + (Int32.Parse(temp2) + 1).ToString() + "00";
                }

            }

            return nomeDaPasta_catalogo_temp;
        }

        public string geraNomePasta()
        {
            String nomePasta = Numero.ToString();
            nomePasta += "-";
            nomePasta += solicitante.Split(' ')[0]; //Pega o primeiro nome do Analista
            return nomePasta;
        }

        public void criarPasta()
        {
            if (Directory.Exists(enderecoPasta) == false)
                pasta = Directory.CreateDirectory(enderecoPasta);
            System.Diagnostics.Process.Start(pasta.FullName);
        }

        public void criarPastaArquivosInternos()
        {
            DirectoryInfo pastaInterna;
            if (Directory.Exists(enderecoPasta) == true)
            {
                if (Directory.Exists(enderecoPasta + "\\_$TD$_files\\") == false)
                {
                    pastaInterna = Directory.CreateDirectory(enderecoPasta + "\\_$TD$_files\\");
                }
                else
                {
                    pastaInterna = new DirectoryInfo(enderecoPasta + "\\_$TD$_files\\");
                }

                pastaInterna.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        public void moverArquivosDaPastaDeDownloadParaPastaDestino()
        {
            //recuperar todos os arquivos da pasta de download
            String[] arquivos = Directory.GetFiles(user.PastaDownloads);
            //mover todos os arquivos recuperados pra pasta destinos
            foreach (string arq in arquivos)
            {
                FileInfo arqInfo = new FileInfo(arq);
                File.Move(user.PastaDownloads + "\\" + arqInfo.Name, enderecoPasta + "\\" + arqInfo.Name);
            }
        }

        public void extrairPacoteGerado()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "WinRAR.exe";
            processInfo.Arguments = "x ";
            processInfo.Arguments += enderecoPasta + "\\CA" + Numero + ".zip" + " " + enderecoPasta + "\\";
            Process.Start(processInfo);
        }

        public void conferirVersaoPacote()
        {
            Process.Start(enderecoPasta + "\\" + "LOG_CA" + Numero.ToString() + ".txt").WaitForExit();
        }

        //Criar o "aplica.txt" no modelo antigo --->A SER DESCONTINUADO
        public bool criaAplicaTXTmodeloAntigo(List<BancoInfo> bdSEMcripto, List<BancoInfo> bdCOMcripto, List<ObjetoBanco> objsBanco)
        {
            List<FileInfo> arquivosSQL = getAllSQLFIles(enderecoPasta);
            if (arquivosSQL.Count == 0)
            {
                return false;
            }
            else
            {
                if (bdCOMcripto.Count > 0)
                {
                    //this.criptografa(objsBanco);
                }
                //Criar o arquivo aplica.txt na pasta
                File.Create(enderecoPasta + "\\" + "aplica.sql").Close();
                StreamWriter arq = new StreamWriter(enderecoPasta + "\\" + "aplica.sql");
                foreach (BancoInfo banco in bdCOMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {

                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log
                            arq.WriteLine("set echo on");
                            arq.WriteLine("set define off");
                            arq.WriteLine("show user");
                            arq.WriteLine("select instance_name from v$instance;");

                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                if ((arqsql.Tipo == ObjetoBanco.TIPO_FUNCTION) || (arqsql.Tipo == ObjetoBanco.TIPO_PACKAGE) || (arqsql.Tipo == ObjetoBanco.TIPO_PROCEDURE))
                                {
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName + ".plb");
                                }
                                else
                                {
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                }

                            }

                            arq.WriteLine("spool off");
                            arq.WriteLine("");

                        }

                    }




                }

                foreach (BancoInfo banco in bdSEMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {
                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log
                            arq.WriteLine("set echo on");
                            arq.WriteLine("set define off");
                            arq.WriteLine("show user");
                            arq.WriteLine("select instance_name from v$instance;");


                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                            }

                            arq.WriteLine("spool off");
                            arq.WriteLine("");
                        }

                    }



                }
                arq.WriteLine("quit");
                arq.WriteLine("");
                arq.Close();
                File.Create(enderecoPasta + "\\" + "aplica.bat").Close();
                StreamWriter executaaplica = new StreamWriter(enderecoPasta + "\\" + "aplica.bat");
                executaaplica.WriteLine("sqlplus /nolog @aplica.sql");
                executaaplica.Close();
                return true;
            }
        }


        //Criar o "aplica.txt" no modelo novo
        //TODO: Ao invés de pegar informações do arquivo, inserir texto na mão (definir strings)
        public bool criaAplicaTXT(List<BancoInfo> bdSEMcripto, List<BancoInfo> bdCOMcripto, List<ObjetoBanco> objsBanco)
        {

            List<FileInfo> arquivosSQL = getAllSQLFIles(enderecoPasta);
            if (arquivosSQL.Count == 0)
            {
                return false;
            }
            else
            {
                if (bdCOMcripto.Count > 0)
                {
                    //this.criptografa(objsBanco); //CRIPTOGRAFIA DESCONTINUADA -- AUTOMATICO PELO SAC
                }
                //Criar o arquivo aplica.txt na pasta
                //File.Delete(enderecoPasta + "\\" + "aplica.sql"); //DELETE DESCONTINUADO
                File.Create(enderecoPasta + "\\" + "aplica.sql").Close();
                StreamWriter arq = new StreamWriter(enderecoPasta + "\\" + "aplica.sql");
                StreamReader leitorArqModelo = new StreamReader("Arquivos/Aplica/aplica.sql");
                foreach (BancoInfo banco in bdCOMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {
                            string linhaLida;
                            string linhaCopiada;

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--CONNECTION AND CONFIG");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "CONN"
                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS BEFORE ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosAntes_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_antes_nomebanco.log


                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG APPEND ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log APPEND"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE OBJECTS TO BE APPLIED");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "@objeto"

                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                if ((arqsql.Tipo == ObjetoBanco.TIPO_FUNCTION) || (arqsql.Tipo == ObjetoBanco.TIPO_PACKAGE) || (arqsql.Tipo == ObjetoBanco.TIPO_PROCEDURE))
                                {
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName + ".plb");
                                }
                                else
                                {
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                }

                            }

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS AFTER ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosDepois_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_depois_nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (!leitorArqModelo.EndOfStream);

                            leitorArqModelo.BaseStream.Position = 0;
                        }
                    }
                }

                foreach (BancoInfo banco in bdSEMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {
                            string linhaLida;
                            string linhaCopiada;

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--CONNECTION AND CONFIG");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "CONN"
                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS BEFORE ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosAntes_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_antes_nomebanco.log


                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG APPEND ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log APPEND"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE OBJECTS TO BE APPLIED");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "@objeto"

                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                            }

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS AFTER ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosDepois_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_depois_nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (!leitorArqModelo.EndOfStream);

                            leitorArqModelo.BaseStream.Position = 0;

                        }
                    }
                }
                arq.WriteLine("quit");
                arq.WriteLine("");
                arq.Close();
                File.Create(enderecoPasta + "\\" + "aplica.bat").Close();
                StreamWriter executaaplica = new StreamWriter(enderecoPasta + "\\" + "aplica.bat");
                executaaplica.WriteLine("sqlplus /nolog @aplica.sql");
                executaaplica.Close();
                return true;
            }
        }

        /// <summary>
        ///  Cria o Aplica SQL, do novo modelo, de maneira modular.
        /// </summary>
        /// <param name="bdSEMcripto">BANCOS em que não aplicam objetos criptografados</param>
        /// <param name="bdCOMcripto">BANCOS em que aplic objetos criptografados</param>
        /// <param name="objsBanco">Objetos de bancos Selecionados</param>
        /// <returns>Retorna 0, caso não tenha objetos de banco selecionados, 1 caso tenha gerado de maneira correta os, -1 caso não tenha Bancos nas listas.</returns>
        public int criaAplicaSQL(List<BancoInfo> bdSEMcripto, List<BancoInfo> bdCOMcripto, List<ObjetoBanco> objsBanco)
        {
            //List<FileInfo> arquivosSQL = getAllSQLFIles(enderecoPasta);
            //if (objsBanco.Count == 0)
            //{
            //    return 0;
            //}
            //if ((bdSEMcripto.Count == 0) && (bdCOMcripto.Count == 0))
            //    return -1;
            //else
            //{
            //    return 1;
            //}
            List<FileInfo> arquivosSQL = getAllSQLFIles(enderecoPasta);
            if (arquivosSQL.Count == 0)
            {
                return 0;
            }
            if ((bdSEMcripto.Count == 0) && (bdCOMcripto.Count == 0))
                return -1;
            else
            {
                if (bdCOMcripto.Count > 0)
                {
                    //this.criptografa(objsBanco); //CRIPTOGRAFIA DESCONTINUADA -- AUTOMATICO PELO SAC
                }
                //Criar o arquivo aplica.txt na pasta
                //File.Delete(enderecoPasta + "\\" + "aplica.sql"); //DELETE DESCONTINUADO
                File.Create(enderecoPasta + "\\" + "aplica.sql").Close();
                StreamWriter arq = new StreamWriter(enderecoPasta + "\\" + "aplica.sql");
                //ABRIR ARQUIVO MODELO PARA LER
                StreamReader leitorArqModelo = new StreamReader("Arquivos/Aplica/aplica.sql");
                //ARQUIVO CONFERE -> Objetivo: Facilitar a Visualização de o que tá sendo aplicado onde
                File.Create(enderecoPasta + "\\" + "confere.txt").Close();
                StreamWriter arqConfere = new StreamWriter(enderecoPasta + "\\" + "confere.txt");
                arqConfere.WriteLine("RESUMO DO APLICA");
                arqConfere.WriteLine();

                foreach (BancoInfo banco in bdCOMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {
                            string linhaLida;
                            string linhaCopiada;

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--CONNECTION AND CONFIG");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "CONN"
                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);
                            arqConfere.WriteLine("APLICANDO EM " + banco.Owners[i].ToUpper() + "/" + banco.Senhas[i].ToUpper() + "@" + banco.Nome.ToUpper() + " os objetos:");

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS BEFORE ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosAntes_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_antes_nomebanco.log


                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG APPEND ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log APPEND"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE OBJECTS TO BE APPLIED");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "@objeto"

                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                if ((arqsql.Tipo == ObjetoBanco.TIPO_FUNCTION) || (arqsql.Tipo == ObjetoBanco.TIPO_PACKAGE) || (arqsql.Tipo == ObjetoBanco.TIPO_PROCEDURE))
                                {
                                    arq.WriteLine("PROMPT aplicando " + arqsql.getTipo() + " - " + arqsql.ArquivoBanco.Name + ".plb");
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName + ".plb");
                                    arqConfere.WriteLine("@" + arqsql.ArquivoBanco.FullName + ".plb");
                                    arq.WriteLine("PROMPT");

                                }
                                else
                                {
                                    arq.WriteLine("PROMPT aplicando "+ arqsql.getTipo()+" - " + arqsql.ArquivoBanco.Name + "");
                                    arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                    arqConfere.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                    arq.WriteLine("PROMPT");
                                }

                            }
                            arqConfere.WriteLine();
                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS AFTER ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosDepois_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_depois_nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (!leitorArqModelo.EndOfStream);

                            leitorArqModelo.BaseStream.Position = 0;
                        }
                    }
                }

                foreach (BancoInfo banco in bdSEMcripto)
                {
                    for (int i = 0; i < banco.Owners.Count; i++)
                    {
                        List<ObjetoBanco> objsCadastradosParaEsseOwner = new List<ObjetoBanco>();
                        objsCadastradosParaEsseOwner.Clear();

                        foreach (ObjetoBanco obj in objsBanco)
                        {
                            if (obj.verificarOwnerBanco(banco.Owners[i], banco.Nome) == true)
                            {
                                objsCadastradosParaEsseOwner.Add(obj);
                            }
                        }

                        if (objsCadastradosParaEsseOwner.Count != 0)
                        {
                            string linhaLida;
                            string linhaCopiada;

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--CONNECTION AND CONFIG");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "CONN"
                            arq.WriteLine("conn " + banco.Owners[i] + "/" + banco.Senhas[i] + "@" + banco.Nome);
                            arqConfere.WriteLine("APLICANDO EM " + banco.Owners[i].ToUpper() + "/" + banco.Senhas[i].ToUpper() + "@" + banco.Nome.ToUpper() + " os objetos:");

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS BEFORE ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosAntes_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_antes_nomebanco.log


                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL LOG APPEND ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\" + banco.Owners[i] + "-" + banco.Nome + ".log APPEND"); //fazer mudancas endereco\nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE OBJECTS TO BE APPLIED");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "@objeto"

                            //escrever arquivos de de banco a serem aplicados
                            foreach (ObjetoBanco arqsql in objsCadastradosParaEsseOwner)
                            {
                                arq.WriteLine("PROMPT aplicando " + arqsql.getTipo() + " - " + arqsql.ArquivoBanco.Name + "");
                                arq.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                arqConfere.WriteLine("@" + arqsql.ArquivoBanco.FullName);
                                arq.WriteLine("PROMPT");
                            }
                            arqConfere.WriteLine();

                            
                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (linhaLida != "--VARIABLE SPOOL INVALID OBJECTS AFTER ADDRESS/FILENAME.EXT");

                            linhaLida = leitorArqModelo.ReadLine(); //CONSOME A LINHA do "SPOOL"
                            arq.WriteLine("spool " + enderecoPasta + "\\objetosInvalidosDepois_" + banco.Nome + ".log"); //fazer mudancas endereco\objetos_invalidos_depois_nomebanco.log

                            do
                            {
                                linhaLida = leitorArqModelo.ReadLine();
                                linhaCopiada = linhaLida;
                                arq.WriteLine(linhaCopiada);
                            } while (!leitorArqModelo.EndOfStream);

                            leitorArqModelo.BaseStream.Position = 0;

                        }
                    }
                }
                arq.WriteLine("quit");
                arq.WriteLine("");
                arq.Close();
                arqConfere.Close();
                File.Create(enderecoPasta + "\\" + "aplica.bat").Close();
                StreamWriter executaaplica = new StreamWriter(enderecoPasta + "\\" + "aplica.bat");
                executaaplica.WriteLine("sqlplus /nolog @aplica.sql");
                executaaplica.Close();
                return 1;
            }

        }

        //BUSCA ARQUIVOS DE BANCO SEM PRIORIDADE DE ORDEM
        public List<FileInfo> getAllSQLFIles(string path)
        {
            List<FileInfo> arquivosBanco = new List<FileInfo>();

            String[] arqs = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (string arq in arqs)
            {
                //POSSIVEIS EXTENSOES (sql, spc)
                FileInfo temp = new FileInfo(arq);
                if ((temp.Extension.ToUpper() == ".SQL") || (temp.Extension.ToUpper() == ".SPC"))
                {
                    arquivosBanco.Add(temp);
                }
            }
            return arquivosBanco;
        }

        //ORGANIZA OS ARQUIVOS DE BANCO POR PRIORIDADE DE ORDEM
        public List<ObjetoBanco> recuperarArquivosSQLnaOrdemDeAtualizacao()
        {
            List<FileInfo> arquivosSQL = getAllSQLFIles(enderecoPasta);
            List<ObjetoBanco> arquivosdebanco = new List<ObjetoBanco>();

            //Sequence
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("SEQUENCE"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_SEQUENCE));
                }
            }


            //Table
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("TABLE"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_TABLE));
                }
            }

            //Trigger
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("TRIGGER"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_TRIGGER));
                }
            }

            //view
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("VIEW"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_VIEW));
                }
            }

            //dml_view
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("DML_VIEW"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_DML_VIEW));
                }
            }


            //índices
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("INDEX"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_INDICE));
                }
            }

            //function
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("FUNCTION"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_FUNCTION));
                }
            }

            //procedure
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("PROCEDURE"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_PROCEDURE));
                }
            }


            //package
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("PACKAGE"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_PACKAGE));
                }
            }


            //sinônimo
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("SYNONYM"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_SYNONYM));
                }
            }

            //sinônimo
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("SINONIMO"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_SYNONYM));
                }
            }

            //script
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("SCRIPT"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_SCRIPT));
                }
            }

            //job
            foreach (FileInfo arq in arquivosSQL)
            {
                if (arq.DirectoryName.ToUpper().Contains("JOB"))
                {
                    arquivosdebanco.Add(new ObjetoBanco(arq, ObjetoBanco.TIPO_JOB));
                }
            }

            return arquivosdebanco;
        }

        public void aplicarNoBanco()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "aplica.bat";
            processInfo.WorkingDirectory = pasta.FullName;
            Process.Start(processInfo);
        }

        public void aplicarNaWeb()
        {
            //TODO: A TRANSFERIR PRA CÁ A RESPONSABILIDADE DA EXECUÇÃO (HOJE ESTÁ DIRETO NA JANELA)
        }

        public bool compararQuantidadeDeObjetosInvalidosAntesDepois(List<BancoInfo> bancos)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "WinMergeU.exe";
            foreach (BancoInfo banco in bancos)
            {
                processInfo.Arguments = enderecoPasta + "\\objetosInvalidosAntes_" + banco.Nome + ".log";
                processInfo.Arguments += " " + enderecoPasta + "\\objetosInvalidosDepois_" + banco.Nome + ".log";
                Process.Start(processInfo);
            }

            return true;
        }

        //TODO: A SER AJUSTADA POIS O NOME DO ARQUIVO DEVE SER OWNER-BANCO.LOG
        public void abrirLogDeAplicacao(List<BancoInfo> bancos)
        {
            foreach (BancoInfo banco in bancos)
            {
                if (File.Exists(enderecoPasta + "\\" + banco.Nome + ".log"))
                    Process.Start(enderecoPasta + "\\" + banco.Nome + ".log");
            }
        }


        //A SER DESCONTINUADA POIS O TEXTO É CRIADO AUTOMATICAMENTE NO SAC
        public void criarTxtParaDistribuicao(List<BancoInfo> bdsCripto)
        {

            String textoDistribuicao;
            textoDistribuicao = "Descrição: ";
            textoDistribuicao += this.Descricao;
            textoDistribuicao += Environment.NewLine;
            textoDistribuicao += Environment.NewLine;
            textoDistribuicao += "Descompactar o anexo e atualizar os seguintes objetos:";
            textoDistribuicao += Environment.NewLine;
            textoDistribuicao += Environment.NewLine;
            foreach (string path in ParteCaminhoArquivos)
            {
                textoDistribuicao += "- " + path;
                if (bdsCripto != null)
                {
                    if (bdsCripto.Count > 0)
                    {
                        if (path.ToUpper().Contains(".SQL"))
                        {
                            textoDistribuicao += ".plb";
                        }
                    }
                }

                textoDistribuicao += Environment.NewLine;
            }
            textoDistribuicao += Environment.NewLine;
            string[] arqs = Directory.GetFiles(enderecoPasta);
            List<FileInfo> arqsinfo = new List<FileInfo>();
            for (int i = 0; i < arqs.Length; i++)
            {
                arqsinfo.Add(new FileInfo(arqs[i]));
            }
            foreach (FileInfo arq in arqsinfo)
            {
                if ((arq.FullName.ToUpper().Contains("CTF_")) && (arq.FullName.ToUpper().Contains(".XLS")))
                {
                    textoDistribuicao += "\nSegue também o documento de teste ";
                    textoDistribuicao += arq.Name;
                    textoDistribuicao += Environment.NewLine;
                }
            }

            textoDistribuicao += Environment.NewLine;
            textoDistribuicao += "[TopDown CA" + numero + "]";

            Clipboard.SetText(textoDistribuicao);
        }

        //A SER DESCONTINUADA POIS O SAC CRIPTOGRAFA AUTOMATICAMENTE AGORA
        public void criptografa(List<ObjetoBanco> arqs)
        {
            if (arqs.Count == 0)
            {
                return;
            }
            foreach (ObjetoBanco arq in arqs)
            {
                if ((arq.Tipo == ObjetoBanco.TIPO_FUNCTION) || (arq.Tipo == ObjetoBanco.TIPO_PACKAGE) || (arq.Tipo == ObjetoBanco.TIPO_PROCEDURE))
                {
                    arq.ArquivoBanco.CopyTo("Arquivos/Cripto/input/" + arq.ArquivoBanco.Name, true);
                }
            }

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "CRIPTOGRAFAR.bat";
            processInfo.WorkingDirectory = Environment.CurrentDirectory + "\\Arquivos\\Cripto\\";
            Process.Start(processInfo).WaitForExit();

            foreach (ObjetoBanco arq in arqs)
            {
                if ((arq.Tipo == ObjetoBanco.TIPO_FUNCTION) || (arq.Tipo == ObjetoBanco.TIPO_PACKAGE) || (arq.Tipo == ObjetoBanco.TIPO_PROCEDURE))
                {
                    FileInfo arquivoDeMesmoNomeCriptografado = new FileInfo("Arquivos/Cripto/output/" + arq.ArquivoBanco.Name + ".plb");
                    arquivoDeMesmoNomeCriptografado.CopyTo(arq.ArquivoBanco.FullName + ".plb", true);
                }
            }


            string[] filesIn = Directory.GetFiles("Arquivos/Cripto/input/");
            for (int i = 0; i < filesIn.Length; i++)
            {
                File.Delete(filesIn[i]);
            }

            string[] filesOut = Directory.GetFiles("Arquivos/Cripto/output/");
            for (int i = 0; i < filesOut.Length; i++)
            {
                File.Delete(filesOut[i]);
            }

        }

        public static void abrirPastaDeControleNaoCarregado(int CANumber, Usuario u)
        {
            String numeroCA = CANumber.ToString();
            int num_catalog_dig1e2, num_catalog_dig3e4e5;
            String temp = Char.ToString(numeroCA[3]) + Char.ToString(numeroCA[4]);
            String temp2 = Char.ToString(numeroCA[0]) + Char.ToString(numeroCA[1]) + Char.ToString(numeroCA[2]);
            String nomeDaPasta_catalogo_temp;
            num_catalog_dig1e2 = Int32.Parse(temp);
            num_catalog_dig3e4e5 = Int32.Parse(temp2);


            if (num_catalog_dig1e2 >= 01 && num_catalog_dig1e2 <= 50)
            {
                nomeDaPasta_catalogo_temp = temp2 + "01" + "-" + temp2 + "50";
                Console.WriteLine("nome da pasta-catalogo: " + nomeDaPasta_catalogo_temp);
            }
            else
            {
                if (num_catalog_dig1e2 == 00) //final 00 ex: 51500 (pasta é 51451-51500)
                {
                    nomeDaPasta_catalogo_temp = (Int32.Parse(temp2) - 1).ToString() + "51" + "-" + numeroCA;
                }
                else //entre 51-99
                {
                    nomeDaPasta_catalogo_temp = temp2 + "51" + "-" + (Int32.Parse(temp2) + 1).ToString() + "00";
                }

            }



            string numeroPastaCatalogo = nomeDaPasta_catalogo_temp;

            //TODO code

            string enderecoPastaPacotes = u.PastaPacotes;
            //Já pegou o numero da pasta catálogo 

            //MessageBox.Show(enderecoPastaPacotes + "\\" + numeroPastaCatalogo);
            //-- procurar dentro da pasta catálogo se a pasta do controle ja existe
            if (Directory.Exists(enderecoPastaPacotes + "\\" + numeroPastaCatalogo) == false)
                MessageBox.Show("Não Encontrada a pasta catálogo");
            else
            {
                //SE ACHOU = PEGA A LISTA DE TODOS OS DIRETÓRIOS DENTRO DA PASTA  CATÁLOGO
                string[] diretorios = Directory.GetDirectories(enderecoPastaPacotes + "\\" + numeroPastaCatalogo);
                Boolean pastaExiste = false;
                DirectoryInfo pasta;
                String enderecoPasta = null;
                List<DirectoryInfo> directoryList = new List<DirectoryInfo>();

                foreach (string diretorio in diretorios)
                {
                    directoryList.Add(new DirectoryInfo(diretorio));
                }


                foreach (DirectoryInfo diretorio in directoryList)
                {
                    if (diretorio.Name.Contains(CANumber.ToString()))
                    {
                        pastaExiste = true;
                        pasta = diretorio;
                        enderecoPasta = pasta.FullName;
                    }
                }

                if (enderecoPasta == null)
                {
                    MessageBox.Show("Pasta não encontrada dentro do Catálogo");
                }
                else
                {
                    //SOMENTE ABRA
                    Process.Start(enderecoPasta);
                }


            }

        }

        public void abrirAplicaSql()
        {
            if (File.Exists(enderecoPasta + "\\" + "aplica.sql"))
                Process.Start(enderecoPasta + "\\" + "aplica.sql");
            if (File.Exists(enderecoPasta + "\\" + "confere.txt"))
                Process.Start(enderecoPasta + "\\" + "confere.txt");
        }
    }
}
