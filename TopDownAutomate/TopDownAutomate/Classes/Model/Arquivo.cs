namespace TopDownAutomate
{
    public class Arquivo
    {

        //Informações recuperadas do SAC
        private string tipo;
        private float versao;
        private string caminhoParcial;
        private string nomeProduto;

        public string Tipo
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
        public float Versao
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

        public string CaminhoParcial
        {
            get
            {
                return caminhoParcial;
            }

            set
            {
                caminhoParcial = value;
            }
        }

        public string NomeProduto
        {
            get
            {
                return nomeProduto;
            }

            set
            {
                nomeProduto = value;
            }
        }

        public Arquivo (string infos)
        {
            Tipo = infos.Split(':')[0];
            int i = infos.IndexOf(" - Versão"); //indica em que índice começa a parte que não faz parte do caminho, mas que explicita a versão
            Versao = float.Parse(infos.Substring(i+10)); //pega a partir do indice i + 10 (=" - Versão "), ou seja pega a versão 1.9, ex.

            caminhoParcial = infos.Split(':')[1]; //separa "BD: Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql" - Versão 1.41 e pega " Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql - Versão 1.41"
            caminhoParcial = caminhoParcial.Remove(0, 1); //Remove o espaço inicial que sobra do split
            int j = caminhoParcial.IndexOf(" - Versão"); //indica em que índice começa a parte que não faz parte do caminho, mas que explicita a versão
            caminhoParcial = caminhoParcial.Remove(j); //remove a partir do indice j
        }

        public Arquivo(string infos, string nomeProduto)
        {
            Tipo = infos.Split(':')[0];
            int i = infos.IndexOf(" - Versão"); //indica em que índice começa a parte que não faz parte do caminho, mas que explicita a versão
            Versao = float.Parse(infos.Substring(i + 10)); //pega a partir do indice i + 10 (=" - Versão "), ou seja pega a versão 1.9, ex.

            caminhoParcial = infos.Split(':')[1]; //separa "BD: Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql" - Versão 1.41 e pega " Packages\TS.AUT_REL_GUIA_TISS_3_02_00_ddl.sql - Versão 1.41"
            caminhoParcial = caminhoParcial.Remove(0, 1); //Remove o espaço inicial que sobra do split
            int j = caminhoParcial.IndexOf(" - Versão"); //indica em que índice começa a parte que não faz parte do caminho, mas que explicita a versão
            caminhoParcial = caminhoParcial.Remove(j); //remove a partir do indice j

            NomeProduto = nomeProduto;
        }
    }
}