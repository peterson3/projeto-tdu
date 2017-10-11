using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;

namespace TopDownAutomate
{
    /// <summary>
    /// Provê métodos estáticos para recuperação de informações de banco.
    /// </summary>
    public static class Banco
    {
        private static string conexao = "Data Source=Arquivos/Config/Banco.db";
        private static string nomebanco = "Arquivos/Config/Banco.db";

        //versao arquivo text
        static public List<BancoInfo> listarBancosCadastrados()
        {
            List<BancoInfo> parcial = new List<BancoInfo>();
            parcial.Clear();
            StreamReader arqReader = new StreamReader("Arquivos\\Config\\bancos.txt");


            while (!arqReader.EndOfStream)
            {
                string linha = arqReader.ReadLine();
                if (!String.IsNullOrWhiteSpace(linha))
                {
                    if (linha.Trim().ToUpper() == "--BANCO")
                    {
                        BancoInfo novoBanco = new BancoInfo();
                        parcial.Add(novoBanco);
                        string hostDoBanco = arqReader.ReadLine();
                        hostDoBanco = hostDoBanco.Trim().ToUpper().Split('=')[1];
                        novoBanco.Nome = hostDoBanco.Trim();
                        string userSenha;
                        do
                        {
                            userSenha = arqReader.ReadLine();
                            if (!String.IsNullOrWhiteSpace(userSenha))
                            {
                            //MessageBox.Show("LINHA: " + userSenha);
                            userSenha = userSenha.Trim().Split('=')[1];
                            //MessageBox.Show("DPOIS DO SPLIT com '=' : " + userSenha);
                            string user = userSenha.Split('/')[0];
                            user=user.Trim();
                            //MessageBox.Show(user);
                            string senha = userSenha.Split('/')[1];
                            senha = senha.Trim();
                            //MessageBox.Show(senha);
                            novoBanco.Owners.Add(user);
                            novoBanco.Senhas.Add(senha);
                            }
                        } while ((userSenha.Trim().ToUpper() != "--BANCO") && !(String.IsNullOrWhiteSpace(userSenha)) && !(arqReader.EndOfStream));
                        string bancogetInfo = novoBanco.Nome + Environment.NewLine;
                        for (int j = 0; j < novoBanco.Owners.Count; j++)
                            bancogetInfo += novoBanco.Owners[j] + "/" + novoBanco.Senhas[j] + Environment.NewLine;
                        //MessageBox.Show(bancogetInfo);
                    }
                }
            }
            return parcial;
        }
        
        //versao bancos cadastrados do banco
        static public List<BancoInfo> listarBancosCadastrador_versao_BD()
        {
            //conecta ao  banco
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM BANCO", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            
            //List<BancoInfo> bancos = new List<BancoInfo>();
            List<BancoInfo>  bancos = new List<BancoInfo>();
            bancos.Clear();
            while (sql_dataReader.Read())
            {

                SQLiteCommand sql_cmd2 = new SQLiteCommand("SELECT * FROM BDOWNER WHERE BancoID = '" + sql_dataReader["ID"].ToString() + "'", conn);
                SQLiteDataReader sql_dataRead2 = sql_cmd2.ExecuteReader();
                List<string> owners = new List<string>();
                List<string> senhas = new List<string>();

                while (sql_dataRead2.Read())
                {
                    owners.Add(sql_dataRead2["User"].ToString());
                    senhas.Add(sql_dataRead2["Senha"].ToString());
                }

                bancos.Add(new BancoInfo
                {
                    Id = Convert.ToInt32(sql_dataReader["ID"]),
                    Nome = sql_dataReader["NOME"].ToString(),
                    Owners = owners,
                    Senhas = senhas
                });
            }

            return bancos;

        }
        /// <summary>
        /// Abre o arquivo de configuração de bancos para edição.
        /// </summary>
        static public void abrirConfig()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "bancos.txt";
            processInfo.WorkingDirectory = "Arquivos\\Config\\";
            Process.Start(processInfo);
        }

        /// <summary>
        /// Retorna um vertor de inteiro com 2 informações -> [0]- ìndice do banco na lista passada, [1]- indice do owner na lista de owners do banco -- que é o mesmo índice da senha
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="bd"></param>
        /// <param name="todosOsBancosSelecionados"></param>
        /// <returns></returns>
        static public int[]  procurarOwnerBancoINDEX(string owner, string bd, List<BancoInfo> todosOsBancosSelecionados)
        {
          for (int i=0; i<todosOsBancosSelecionados.Count; i++)
            {
                if (todosOsBancosSelecionados[i].Nome == bd)
                {
                    //É esse o banco procurado, procurar o owner.
                    for (int k=0; k<todosOsBancosSelecionados[i].Owners.Count; k++)
                    {
                        if (todosOsBancosSelecionados[i].Owners[k] == owner)
                        {
                            int[] resp = { i, k };
                            return resp;
                        }
                    }
                }
            }
            return null;
        }
    }
}
