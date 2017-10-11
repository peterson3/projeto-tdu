using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TopDownAutomate
{
    public class Usuario
    {
        private string username;
        private string password;
        private string nomeCompleto;
        private string tnsnames;
        private string pastaPacotes;


        private string pastaDownloads;
        private string pastaWinrar;
        private string pastaWinMerge;

        public Usuario()
        {

        }
        public Usuario(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string NomeCompleto
        {
            get { return nomeCompleto; }
            set { nomeCompleto = value; }
        }
        public string TnsNames
        {
            get { return tnsnames; }
            set { tnsnames = value; }
        }
        public string PastaPacotes
        {
            get { return pastaPacotes; }
            set { pastaPacotes = value; }
        }

        public string PastaDownloads
        {
            get { return pastaDownloads; }
            set { pastaDownloads = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string PastaWinrar
        {
            get { return pastaWinrar; }
            set { pastaWinrar = value; }
        }

        public string PastaWinMerge
        {
            get { return pastaWinMerge; }
            set { pastaWinMerge = value; }
        }
    }
}
