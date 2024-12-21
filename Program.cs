using System;
using System.IO;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        // input au format : projetmakersharp -dll1 -dll2 ... output MonBeauProjet
        // il va aller chercher les DLL du dossier de DLL
        // s'il en trouve pas une, il sort "Wallah, j'ai pas trouvé xxx"
        // s'il trouve tout il créer le dossier localement avec le Program.cs et le csproj avec toutes les DLL qui vont bieng
        // !! bien penser à copier correctement les DLL

        // simple first check to see if no argument at all
        if(args.Length == 0){
            Console.WriteLine("Pas d'argument, pas de boulot");
            return ;
        }

        // else there is content, so all the required variables to set up
        bool outputIsHere = false;
        string nameOutput ="";
        List<string> allArguments = new List<string>();

        foreach(string arg in args){
            if(arg == "output"){
                outputIsHere = true;
                if(args.Length == Array.LastIndexOf(args, arg)+1){
                    Console.WriteLine("Manque un argument de sortie mon cochon");
                    return;
                }
                nameOutput = args[Array.LastIndexOf(args, arg)+1];
                break;
            }
            if(arg[0]=='-'){
                string[] part2arg = arg.Split('-');
                allArguments.Add(part2arg[1]);
            }

        }
        if(!outputIsHere || nameOutput==""){
            Console.WriteLine("manque l'ouput");
            return;
        }
        if(allArguments.ToArray().Length == 0){
            Console.WriteLine("manque des arguments, ou argument pas au bon format");
            return;
        }

        // time to get down to business and build that motherfucking project
        string pathCurrentDirectory = Directory.GetCurrentDirectory();
        string newDirectory = pathCurrentDirectory + @"\" + nameOutput;
        Directory.CreateDirectory(newDirectory);
        Directory.CreateDirectory(newDirectory + @"\" + "Bibliotheques");

        // create the .cs
        using (FileStream fs = File.Create(newDirectory + @"\" + "Program.cs"))
        {
            char coma = '"';
            byte[] info = new UTF8Encoding(true).GetBytes(
                "internal class Program\n"
                +"{\n"
                +"private static void Main(string[] args)\n"
                +"{\n"
                +"Console.WriteLine("+coma+"Programme à foutre ici"+coma+");\n"
                +"}\n"
                +"}\n"
            );
            fs.Write(info, 0, info.Length);
        }

        // create the .csproj
        using (FileStream fs = File.Create(newDirectory + @"\" + nameOutput.ToLower() +".csproj"))
        {
            char coma = '"';
            string argument = 
                "<Project Sdk="+coma+"Microsoft.NET.Sdk"+coma+">\n"
                +"<PropertyGroup>\n"
                +"<OutputType>Exe</OutputType>\n"
                +"<TargetFramework>net8.0</TargetFramework>\n"
                +"<ImplicitUsings>enable</ImplicitUsings>\n"
                +"<Nullable>enable</Nullable>\n"
                +"</PropertyGroup>\n"
                +"<ItemGroup>\n";
            foreach(string arg in allArguments){
                argument = argument + "<Reference Include="+coma+arg+coma+">\n"
                +"<HintPath>"+@"Bibliotheques\"+arg+".DLL</HintPath>\n"
                +"</Reference>\n";
            }
            argument = argument +"</ItemGroup>\n</Project>\n";
            byte[] info = new UTF8Encoding(true).GetBytes(argument);
            fs.Write(info, 0, info.Length);
        }

        // copy all DLL in the right directory
        string pathDLLs = "";
        foreach(string arg in allArguments){
            string pathDLL = pathDLLs + @"\" + arg + ".DLL";
            string newpathDLL = newDirectory + @"\" + "Bibliotheques" + arg + ".DLL";
            File.Copy(pathDLL, newpathDLL, overwrite: true);
        }
    }
}