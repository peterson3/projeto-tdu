using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownAutomate
{
    public class Produto
    {
        string nome;
        string versao;
        string modulo;
        string funcao;

        public Produto (string nome, string versao, string modulo, string funcao)
        {
            this.nome = nome;
            this.versao = versao;
            this.modulo = modulo;
            this.funcao = funcao;
        }
        
        public string Funcao
        {
            get
            {
                return funcao;
            }

            set
            {
                funcao = value;
            }
        }

        public string Modulo
        {
            get
            {
                return modulo;
            }

            set
            {
                modulo = value;
            }
        }

        public string Versao
        {
            get
            {
                return versao;
            }

            set
            {
                versao = value;
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
    }
}
