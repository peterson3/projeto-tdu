using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownAutomate;

namespace TDFU.classes
{
    public static class Sugest
    {


       public static List<string> sugerirBancos(List<BancoInfo> bancos, List<Produto> produtos, List<Cliente> clientes)
        {
            List<string> nomeDosBancosSugeridos = new List<string>();

            foreach (Produto p in produtos)
            {
                if (p.Versao == "11")
                {
                    nomeDosBancosSugeridos.Add("HOMO_MED");
                    nomeDosBancosSugeridos.Add("HOMO_ODO");
                    nomeDosBancosSugeridos.Add("INST_MED");
                    nomeDosBancosSugeridos.Add("INST_ODO");
                    nomeDosBancosSugeridos.Add("DEMO_MED");
                    nomeDosBancosSugeridos.Add("DEMO_ODO");
                }
            } 

            foreach (Cliente c in clientes)
            {
                if (c.Nome.ToUpper() == "METLIFE")
                {
                    nomeDosBancosSugeridos.Add("METLIFE");
                }
                if ((c.Nome.ToUpper() == "TOPDOWN") || (c.Nome.ToUpper() == "TOP DOWN") || (c.Nome.ToUpper() == "ADMIX"))
                {
                    nomeDosBancosSugeridos.Add("GRM");
                }
                if (c.Nome.ToUpper() == "SEPACO")
                {
                    nomeDosBancosSugeridos.Add("SEPACO-TESTE");
                    nomeDosBancosSugeridos.Add("SEP-VAZIo");
                }
                if ((c.Nome.ToUpper() == "UNIMED SEGUROS") || (c.Nome.ToUpper() == "SEGUROS UNIMED"))
                {
                    nomeDosBancosSugeridos.Add("SEG_DES8");
                    nomeDosBancosSugeridos.Add("SEG_VAZIO");
                }

            }
            return nomeDosBancosSugeridos;
        }
                
    }
}
