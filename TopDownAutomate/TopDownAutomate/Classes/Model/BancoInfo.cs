using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopDownAutomate
{
   public class BancoInfo
    {
        private int id;
        private string nome;
        private List<string> owners;
        private List<string> senhas;

        public BancoInfo()
        {
            owners = new List<string>();
            senhas = new List<string>();
        }

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

        public List<string> Owners
        {
            get
            {
                return owners;
            }

            set
            {
                owners = value;
            }
        }

        public List<string> Senhas
        {
            get
            {
                return senhas;
            }

            set
            {
                senhas = value;
            }
        }

        override
        public string ToString()
        {
            return Nome;
        }

    }
}
