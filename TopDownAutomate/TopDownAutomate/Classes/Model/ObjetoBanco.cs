using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopDownAutomate
{
    public class ObjetoBanco
    {
        //ordem prioritária
        public static readonly int TIPO_SEQUENCE = 1;
        public static readonly int TIPO_TABLE = 2;
        public static readonly int TIPO_TRIGGER = 3;
        public static readonly int TIPO_VIEW = 4;
        public static readonly int TIPO_INDICE = 5;
        public static readonly int TIPO_FUNCTION = 6;
        public static readonly int TIPO_PROCEDURE = 7;
        public static readonly int TIPO_PACKAGE = 8;
        public static readonly int TIPO_SYNONYM = 9;
        public static readonly int TIPO_SCRIPT = 10;
        public static readonly int TIPO_DML_VIEW = 11;
        public static readonly int TIPO_JOB = 12;



        FileInfo arquivoBanco;
        int tipo;
        List<string> owners;
        List<string> bancos;
        public FileInfo ArquivoBanco
        {
            get
            {
                return arquivoBanco;
            }

            set
            {
                arquivoBanco = value;
            }
        }

        public int Tipo
        {
            get
            {
                return tipo;
            }

            set
            {
                tipo = value;
            }
        }


        public string getTipo()
        {
            string retorno;
            switch (this.Tipo)
            {
                case 1:
                    retorno = "Sequence";
                    break;
                case 2:
                    retorno = "Table";
                    break;
                case 3:
                    retorno = "Trigger";
                    break;
                case 4:
                    retorno = "View";
                    break;
                case 5:
                    retorno = "Index";
                    break;
                case 6:
                    retorno = "Function";
                    break;
                case 7:
                    retorno = "Procedure";
                    break;
                case 8:
                    retorno = "Package";
                    break;
                case 9:
                    retorno = "Sinonimo";
                    break;
                case 10:
                    retorno = "Script";
                    break;
                case 11:
                    retorno = "DML_View";
                    break;
                case 12:
                    retorno = "Job";
                    break;
                default:
                    retorno = null;
                    break;
            }
            return retorno;

        }

        public string getOwner()
        {
            if (this.Tipo == TIPO_DML_VIEW)
            {
                return "DML_VIEW (sem tipo)";
            }
            else
            {
                string nomeArq;
                nomeArq = arquivoBanco.Name;
                string[] separados = nomeArq.Split('.');
                string owner = separados[0];
                if (!(owner.ToUpper().Contains("TS.")) || !(owner.ToUpper().Contains("TS_")))
                {
                    owner = "TS"; // trata caso de Scripts que não contém TS. no nome
                }
                return owner;
            }

        }

        override
        public string ToString()
        {
            string parcial;
            parcial = getTipo().ToUpper();
            parcial += ": " + arquivoBanco.Name;
            parcial += "\tOwner: " + getOwner().ToUpper();
            return parcial;
        }

        public ObjetoBanco(FileInfo arqInfo, int type)
        {
            ArquivoBanco = arqInfo;
            Tipo = type;
            //IndecesBancoOwnerSenha = new List<int[]>();
            owners = new List<string>();
            bancos = new List<string>();
        }

        public void aplicarNoOwnerBanco(string owner, string banco)
        {
            this.owners.Add(owner);
            this.bancos.Add(banco);
        }

        public void limparOwnersBancos()
        {
            owners.Clear();
            bancos.Clear();
        }

        public static ObjetoBanco procurarObjetoDeBancoNaLista (List<ObjetoBanco> lista, string tipo, string nomeArquivo)
        {
            for (int i=0; i<lista.Count; i++)
            {
                if ((tipo.ToUpper() == lista[i].getTipo().ToUpper()) && (nomeArquivo.ToUpper() == lista[i].ArquivoBanco.Name.ToUpper()))
                {
                    return lista[i];
                }
            }
            return null;
        }

        public bool verificarOwnerBanco(ListViewItem listViewItem)
        {
            bool existe = false;
           for (int i=0; i<owners.Count; i++)
            {
                if ((owners[i].ToUpper()==listViewItem.Text.ToUpper()) && (bancos[i].ToUpper() == listViewItem.Group.Name.ToUpper()))
                {
                    return true;
                }
            }
            return existe;
        }

        public bool verificarOwnerBanco(string owner, string banco)
        {
            bool existe = false;
            for (int i = 0; i < owners.Count; i++)
            {
                if ((owners[i].ToUpper() == owner.ToUpper()) && (bancos[i].ToUpper() == banco.ToUpper()))
                {
                    return true;
                }
            }
            return existe;
        }
    }
}
