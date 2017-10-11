using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopDownAutomate;
using System.Data.SQLite;
using System.Data;

namespace TDFU.classes.Control
{
   public class UsuarioController
    {
        public Usuario usuario;
        private static string conexao = "Data Source=Arquivos/Config/Banco.db";
        private static string nomebanco = "Arquivos/Config/Banco.db";


        //Lê as Informações do Arquivo e Joga na Tela
        public void CarregaInformacoesAoIniciar(TextBox usernameBox, TextBox passwordBox, TextBox pacotesFolder)
        {
            if (File.Exists("Arquivos/Config/config.bin"))
            {
                using (BinaryReader reader = new BinaryReader(File.Open("Arquivos/Config/config.bin", FileMode.Open)))
                {
                    usernameBox.Text = reader.ReadString();
                    passwordBox.Text = reader.ReadString();
                    pacotesFolder.Text = reader.ReadString();

                }
                if (usernameBox.Text != "" && passwordBox.Text != "")
                {
                    usuario = new Usuario(usernameBox.Text, passwordBox.Text);
                    if (pacotesFolder.Text != "")
                        usuario.PastaPacotes = pacotesFolder.Text;
                }
            }
        }

        public void CarregaInformacoesAoIniciar_doBanco(TextBox usernameBox, TextBox passwordBox, TextBox tnsnamesBox, TextBox pacotesFolder)
        {

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SQLiteCommand sql_cmd = new SQLiteCommand("SELECT * FROM USUARIO", conn);
            SQLiteDataReader sql_dataReader = sql_cmd.ExecuteReader();
            usuario = new Usuario();

            while (sql_dataReader.Read())
            {
                usuario.NomeCompleto = sql_dataReader["NomeCompletoSAC"].ToString();
                usuario.Username = sql_dataReader["UsuarioSAC"].ToString();
                usuario.Password = sql_dataReader["SenhaSAC"].ToString();
                usuario.TnsNames = sql_dataReader["Tnsnames"].ToString();
                usuario.PastaPacotes = sql_dataReader["PastaPacote"].ToString();
            }

            usernameBox.Text = usuario.Username;
            passwordBox.Text = usuario.Password;
            pacotesFolder.Text = usuario.PastaPacotes;
            tnsnamesBox.Text = usuario.TnsNames;

        }

        internal bool SalvarInformacoesDeConfiguracao_noBanco(TextBox usernameBox, TextBox passwordBox, TextBox tnsnamesBox, TextBox pacotesFolder)
        {

            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();


            //Dá o update
            SQLiteCommand sql_cmd = new SQLiteCommand("UPDATE USUARIO SET UsuarioSAC = @USUARIOSAC, SenhaSAC= @SENHASAC, Tnsnames= @TNS, PastaPacote = @PASTAPACOTE WHERE ID = 1", conn);
            sql_cmd.Parameters.AddWithValue("USUARIOSAC", usernameBox.Text.Trim());
            sql_cmd.Parameters.AddWithValue("SENHASAC", passwordBox.Text.Trim());
            sql_cmd.Parameters.AddWithValue("TNS", tnsnamesBox.Text.Trim());
            sql_cmd.Parameters.AddWithValue("PASTAPACOTE", pacotesFolder.Text.Trim());


            try
            {
                sql_cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        internal void SalvarInformacoesDeConfiguracao(TextBox usernameBox, TextBox passwordBox, TextBox pacotesFolderBox)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("Arquivos/Config/config.bin", FileMode.Create)))
            {
                writer.Write(usernameBox.Text);
                writer.Write(passwordBox.Text);
                writer.Write(pacotesFolderBox.Text);
            }
        }


    }
}
