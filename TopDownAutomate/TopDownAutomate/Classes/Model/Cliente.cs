using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownAutomate
{
   public class Cliente
    {
        String nome;
        String nomeBanco;
        String userBanco;
        String senhaBanco;
        String pastaTopSaude;
        String userRede;
        String senhaRede;
        bool possuiSAC;
        List<int> numeroSAC;
        bool temInfoCadastrada;
        private string text;

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

        public string NomeBanco
        {
            get
            {
                return nomeBanco;
            }

            set
            {
                nomeBanco = value;
            }
        }

        public string UserBanco
        {
            get
            {
                return userBanco;
            }

            set
            {
                userBanco = value;
            }
        }

        public string SenhaBanco
        {
            get
            {
                return senhaBanco;
            }

            set
            {
                senhaBanco = value;
            }
        }

        public string PastaTopSaude
        {
            get
            {
                return pastaTopSaude;
            }

            set
            {
                pastaTopSaude = value;
            }
        }

        public string UserRede
        {
            get
            {
                return userRede;
            }

            set
            {
                userRede = value;
            }
        }

        public string SenhaRede
        {
            get
            {
                return senhaRede;
            }

            set
            {
                senhaRede = value;
            }
        }

        public bool TemInfoCadastrada
        {
            get
            {
                return temInfoCadastrada;
            }

            set
            {
                temInfoCadastrada = value;
            }
        }

        public List<int> NumeroSAC
        {
            get
            {
                return numeroSAC;
            }

            set
            {
                numeroSAC = value;
            }
        }

        public Cliente(string nomeCliente)
        {
            Nome = nomeCliente;
            TemInfoCadastrada = procuraInfo();
        }

        public Cliente(string nomeCliente, string SAC)
        {
            Nome = nomeCliente;
            NumeroSAC = new List<int>();
            try
            {
               int sac =  Int32.Parse(SAC);
                NumeroSAC.Add(sac);
            }
            catch (Exception)
            {
                //Não Conseguiu converter para INT, então significa que não tem SAC
            }
        }

        public bool procuraInfo ()
        {
            string nomeTEMP;
            StreamReader arqReader = new StreamReader("Arquivos\\AplicaEmBanco\\info_bancos_webs.txt");
            bool found = false;
            do
            {
                arqReader.ReadLine(); //Lê /**INFO**/
                arqReader.ReadLine(); //Lê CLIENTE
                nomeTEMP = arqReader.ReadLine();
                arqReader.ReadLine(); //LÊ NOMEBANCO
                this.NomeBanco = arqReader.ReadLine();
                arqReader.ReadLine();
                this.UserBanco = arqReader.ReadLine();
                arqReader.ReadLine();
                this.SenhaBanco = arqReader.ReadLine();
                arqReader.ReadLine();
                this.PastaTopSaude = arqReader.ReadLine();
                arqReader.ReadLine();
                this.UserRede = arqReader.ReadLine();
                arqReader.ReadLine();
                this.SenhaRede = arqReader.ReadLine();
                if (nomeTEMP == nome)
                {
                    found = true;
                }
                arqReader.ReadLine(); //Lê /**END**/
            } while ((arqReader.EndOfStream == false) && (found == false));
            return found;
        }

    }




}
