using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDFU.classes
{
    public class WebInfo
    {
        int id;
        string nome;  //Unimed Seguros
        string endereco; //10.10.100.135
        string user; //TopTeste
        string senha; //sEnhsENha

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Nome
        {
            get
            {
                return nome;
            }

            set
            {
                nome = value;
            }
        }

        public string Endereco
        {
            get
            {
                return endereco;
            }

            set
            {
                endereco = value;
            }
        }

        public string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public string Senha
        {
            get
            {
                return senha;
            }

            set
            {
                senha = value;
            }
        }

        public WebInfo()
        {

        }
        
        override
        public string ToString()
        {
            return (this.Nome + "    (" + this.Endereco+")");
        }


    }




}
