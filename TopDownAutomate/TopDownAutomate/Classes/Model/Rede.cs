using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System;
using TDFU.classes;


namespace TopDownAutomate
{
    public static class Rede
    {
       public static void abrirConfig()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = "webs.txt";
            processInfo.WorkingDirectory = "Arquivos\\Config\\";
            Process.Start(processInfo);
        }

       public static List<String> retornarPastas(string nomeLocal) //SUPOE JÁ LOGADO
       {

           List<String> Pastas = new List<String>();
           // Create a child process
           System.Diagnostics.Process process = new System.Diagnostics.Process();

           // Starts a new instance of the Windows command interpreter
           process.StartInfo.FileName = "cmd.exe";

           // Carries out the NET VIEW command and then terminates
           process.StartInfo.Arguments = "/c net view "+ nomeLocal;
           process.StartInfo.CreateNoWindow = true;

           // Redirect the output stream of the child process.
           process.StartInfo.UseShellExecute = false;
           process.StartInfo.RedirectStandardOutput = true;

           // Start!
           process.Start();


           for (int i = 0; i < 7; i++)
           {
               process.StandardOutput.ReadLine();
           }

            // Read the output stream
            while (!process.StandardOutput.EndOfStream)
            {
                // This is the current stream data

                //Passa as 7 primeiras Linhas
             
                string line = process.StandardOutput.ReadLine();
                if (line.Contains("Disco"))
                {
                    int x = line.IndexOf("Disco");
                    string nomeParcial = line.Substring(0, x);
                    string pasta = nomeParcial.Trim();
                    //PASTAS QUE CONHECIDAMENTE NAO SAO DE INTERESSE --REMOVER
                    if ((pasta.ToUpper() != "USERS")
                        && (pasta.ToUpper() != "TEMP")
                        && (pasta.ToUpper() != "E")
                        && (pasta.ToUpper() != "D")
                        && (pasta.ToUpper() != "C")
                        && (pasta.ToUpper() != "TRANSF"))
                    {
                        
                        Pastas.Add(pasta);
                    }
                }
              
            }

            // Wait indefinitely for the associated process to exit
            process.WaitForExit();

            // Frees all the resources
            process.Close();
            return Pastas;
       }

       public static bool copiarParaAWebDir(string arqFullName, string dirName)
       {
           System.Diagnostics.Process process = new System.Diagnostics.Process();
           System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
           startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
           // Redirect the output stream of the child process.
           startInfo.CreateNoWindow = true;
           startInfo.UseShellExecute = false;
           startInfo.RedirectStandardOutput = true;
           startInfo.FileName = "cmd.exe";
           startInfo.Arguments = "/C copy /y";
           startInfo.Arguments += arqFullName + " " + dirName;
           process.StartInfo = startInfo;
           process.Start();
           process.WaitForExit();
           string result="";
           while (!process.StandardOutput.EndOfStream)
           {
               string line = process.StandardOutput.ReadLine();
               result = line.Trim();
           }

           if (result == "1 arquivo(s) copiado(s).")
           {
               return true;
           }
           else
           {
               return false;
           }
       }
       public static bool copiarParaAWebArq(string arqSrcFullName, string arqDestFullName)
       {
           System.Diagnostics.Process process = new System.Diagnostics.Process();
           System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
           startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
           startInfo.CreateNoWindow = true;
           startInfo.UseShellExecute = false;
           startInfo.RedirectStandardOutput = true;
           startInfo.FileName = "cmd.exe";
           startInfo.Arguments = "/C copy /y";
           startInfo.Arguments += arqSrcFullName + " " + arqDestFullName;
           process.StartInfo = startInfo;
           process.Start();
           process.WaitForExit();
           string result = "";
           while (!process.StandardOutput.EndOfStream)
           {
               string line = process.StandardOutput.ReadLine();
               result = line.Trim();
           }

           if (result == "1 arquivo(s) copiado(s).")
           {
               return true;
           }
           else
           {
               return false;
           }
       }


       public static string recuperarNomePC()
       {
           string nomePasta="";
           System.Diagnostics.Process process = new System.Diagnostics.Process();
           process.StartInfo.FileName = "cmd.exe";
           process.StartInfo.Arguments = "/C echo %userdomain%";
           process.StartInfo.CreateNoWindow = true;
           process.StartInfo.UseShellExecute = false;
           process.StartInfo.RedirectStandardOutput = true;
           process.Start();
           while (!process.StandardOutput.EndOfStream)
           {
               string line = process.StandardOutput.ReadLine();
               nomePasta = line.Trim();
           }
           process.WaitForExit();
           process.Close();
           return nomePasta;
       }

       public static string LogarNaMaquina(WebInfo webAserAplicada, string nomePc)
       {
           string retorno = "";
           System.Diagnostics.Process process = new System.Diagnostics.Process();
           process.StartInfo.FileName = "cmd.exe";
           process.StartInfo.Arguments = "/C net use " + "\\\\" + webAserAplicada.Endereco + " /user:" + nomePc + "\\" + webAserAplicada.User + " " + webAserAplicada.Senha;
           process.StartInfo.CreateNoWindow = true;
           process.StartInfo.UseShellExecute = false;
           process.StartInfo.RedirectStandardOutput = true;
           process.Start();

           //while (!process.StandardOutput.EndOfStream)
           //{
           //    string line = process.StandardOutput.ReadLine();
           //    retorno += line;
           //    //retorno += Environment.NewLine;
           //}

           
           //ProcessStartInfo processInfo = new ProcessStartInfo();
           //processInfo.UseShellExecute = true;
           //processInfo.FileName = "cmd";
           //processInfo.Arguments = "/C net use " + "\\\\" + webAserAplicada.Endereco + " /user:" + nomePc + "\\" + webAserAplicada.User + " " + webAserAplicada.Senha;
           //Process.Start(processInfo);
           retorno = process.StandardOutput.ReadToEnd();
           process.WaitForExit();
           //return retorno;
           return retorno;
       }

       public static string DeslogarDaMaquina(WebInfo webAserAplicada)
       {
           string retorno = "";
           System.Diagnostics.Process process = new System.Diagnostics.Process();
           process.StartInfo.FileName = "cmd.exe";
           process.StartInfo.Arguments = "/C net use /delete /y " + "\\\\" + webAserAplicada.Endereco;
           process.StartInfo.CreateNoWindow = true;
           process.StartInfo.UseShellExecute = false;
           process.StartInfo.RedirectStandardOutput = true;
           process.Start();
           retorno = process.StandardOutput.ReadToEnd();
           process.WaitForExit();
           return retorno;
       }
    }


}