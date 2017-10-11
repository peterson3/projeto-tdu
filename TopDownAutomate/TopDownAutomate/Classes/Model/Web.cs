using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace TDFU.classes
{
    /// <summary>
    /// Provê métodos estáticos para recuperação de Objetos do tipo WebInfo
    /// </summary>
    public static class Web
    {
        private static string conexao = "Data Source=Arquivos/Config/Banco.db";
        private static string nomebanco = "Arquivos/Config/Banco.db";

        static public List<WebInfo> listarWebsCadastradas()
        {
            List<WebInfo> parcial = new List<WebInfo>();
            parcial.Clear();
            StreamReader arqReader = new StreamReader("Arquivos\\Config\\webs.txt");
            do
            {
                WebInfo addWebInfo = new WebInfo();
                addWebInfo.Nome = arqReader.ReadLine();
                addWebInfo.Endereco = arqReader.ReadLine();
                addWebInfo.User = arqReader.ReadLine();
                addWebInfo.Senha = arqReader.ReadLine();
                parcial.Add(addWebInfo);
                if (arqReader.EndOfStream == false)
                {
                    arqReader.ReadLine();
                }
            } while (arqReader.EndOfStream == false);

            return parcial;
        }

        static public List<WebInfo> listarWebsCadastradas_doBanco()
        {
            List<WebInfo> webs;

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM WEB", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            
            webs = new List<WebInfo>();
            while (sql_dataReader.Read())
            {
                webs.Add(new WebInfo
                {
                    Id = Convert.ToInt32(sql_dataReader["ID"]),
                    Nome = sql_dataReader["NOME"].ToString(),
                    Endereco = sql_dataReader["IP"].ToString(),
                    User = sql_dataReader["USER"].ToString(),
                    Senha = sql_dataReader["SENHA"].ToString()
                });
            }

            return webs;
        }
    }
}
