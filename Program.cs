using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


public class ManejoArchivos
{
    // Constructor...
    public ManejoArchivos(string RutaCarpeta)
    {
        rutaCarpeta = RutaCarpeta;
        ArchivosTxt = Directory.GetFiles(this.rutaCarpeta, "*.txt");
        ContenidoArchivos = new string[ArchivosTxt.Length];

    }
    public string rutaCarpeta { get; set; }
    public string[] ArchivosTxt { get; set; }
    public string[] ContenidoArchivos { get; set; }
    public Dictionary<string, string[]> PalabrasUnicas = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> NombresvsPalabras = new Dictionary<string, string[]>();
    public Dictionary<string, Dictionary<string, double>> Textos_Palabras_TF = new Dictionary<string, Dictionary<string, double>>();
    public Dictionary<string, Dictionary<string, double>> Textos_Palabras_IDF = new Dictionary<string, Dictionary<string, double>>();
    public Dictionary<string, Dictionary<string, double>> DiccionarioTF_IDF = new Dictionary<string, Dictionary<string, double>>();


    // Metodo para leer el contenido de cada array y almacenarlo en el array correspondinte
    public string[] ObtenerTextos()
    {
        for (int i = 0; i < this.ArchivosTxt.Length; i++)
        {
            if (this.ArchivosTxt[i] != null)
            {
                this.ContenidoArchivos[i] = File.ReadAllText(ArchivosTxt[i]);
                this.ContenidoArchivos[i] = this.ContenidoArchivos[i].ToLower();
            }
        }
        return this.ContenidoArchivos;
    }



    // Metodo para obtener palabras (Tokenizador)
    public void ObtenerPalabras()
    {
        // dividir el string en palabras 
        char[] delimitadores = { ' ', ',', '.', ':', '¿', '?', '!', '*', '/', '"', '#', ')', '(', };
        string StringTexto = "";
        for (int i = 0; i < this.ContenidoArchivos.Length; i++)
        {
            if (this.ContenidoArchivos[i] != null)
            {
                StringTexto += this.ContenidoArchivos[i];
                this.NombresvsPalabras.Add(this.ArchivosTxt[i], this.ContenidoArchivos[i].Split(delimitadores));
                this.PalabrasUnicas.Add(this.ArchivosTxt[i], NombresvsPalabras[this.ArchivosTxt[i]].Distinct().ToArray());

            }
        }


    }

    public double DondeEstaLaPalabra(string Palabra, Dictionary<string, string[]> Dic, string[] Texto)
    {
        double DocumentosDondeExiste = 0;
        for (int i = 0; i < Texto.Length; i++)
        {
            string[] Documento = Dic[Texto[i]];

            for (int o = 0; o < Documento.Length; o++)
            {

                if (Documento[o] == Palabra)
                {
                    DocumentosDondeExiste++;
                    break;
                }

            }

        }
        return DocumentosDondeExiste;


    }


    ///  tf-idf
    public void Tf()
    {
        foreach (var item in this.PalabrasUnicas)
        {
            Dictionary<string, double> DiccionarioTF = new Dictionary<string, double>();
            foreach (var item2 in item.Value)
            {
                double contador = 0;
                foreach (var item3 in this.NombresvsPalabras[item.Key])
                {
                    if (item2 == item3)
                    {
                        contador++;
                    }
                }
                DiccionarioTF.Add(item2, contador / this.NombresvsPalabras[item.Key].Length);
            }
            this.Textos_Palabras_TF.Add(item.Key, DiccionarioTF);
        }
    }

    public void Idf()
    {
        foreach (var item in this.PalabrasUnicas)
        {
            Dictionary<string, double> DiccionarioIDF = new Dictionary<string, double>();
            foreach (var item2 in item.Value)
            {
                double contador = 0;
                foreach (var item3 in this.PalabrasUnicas)
                {
                    foreach (var item4 in item3.Value)
                    {
                        if (item2 == item4)
                        {
                            contador++;
                            break;
                        }
                    }
                }
                DiccionarioIDF.Add(item2, Math.Log10(this.PalabrasUnicas.Count / contador));
            }
            this.Textos_Palabras_IDF.Add(item.Key, DiccionarioIDF);
        }
    }
    public void Tf_Idf()
    {
        foreach (var item in this.PalabrasUnicas)
        {
            Dictionary<string, double> DiccionarioTF_IDF = new Dictionary<string, double>();
            foreach (var item2 in item.Value)
            {
                DiccionarioTF_IDF.Add(item2, this.Textos_Palabras_TF[item.Key][item2] * this.Textos_Palabras_IDF[item.Key][item2]);
            }
            this.DiccionarioTF_IDF.Add(item.Key, DiccionarioTF_IDF);
        }

    }



    public string[] Motor()
    {
        ObtenerTextos();
        ObtenerPalabras();
        Tf();
        Idf();
        Tf_Idf();

        System.Console.WriteLine("Datos cargados");
        return this.ContenidoArchivos;

    }




}

/////////////////////////query//////////////////////////////
class Query
{
    ///constructor //////
    public Query(string QueryInput)
    {
        queryinput = QueryInput;
        queryPalabrasUnicas = new string[0];
        queryToken = new string[0];
    }

    public string queryinput { get; set; }
    public string[] queryPalabrasUnicas { get; set; }
    public string[] queryToken { get; set; }
    public Dictionary<string, double> Query_TF = new Dictionary<string, double>();
    public Dictionary<string, double> Query_IDF = new Dictionary<string, double>();
    public Dictionary<string, double> Query_TF_IDF = new Dictionary<string, double>();


    public void QueryToken()
    {
        char[] delimitadores = { ' ', ',', '.', ':', '¿', '?', '!', '*', '/', '"', '#', ')', '(', };
        string[] queryToken = this.queryinput.Split(delimitadores);

        
    }
    public void QueryPalabrasMinusculas(string[] queryToken)
    {
        for (int i = 0; i < this.queryToken.Length; i++)
        {
            if (queryToken != null)
            {
                this.queryToken[i] = this.queryToken[i].ToLower();
                this.queryPalabrasUnicas = this.queryToken.Distinct().ToArray();
            }
        }
    }


    //// QueryTF
    public void Query_TF_()
    {
        foreach (var item in this.queryPalabrasUnicas)
        {
            double contador = 0;
            foreach (var item2 in this.queryToken)
            {
                if (item == item2)
                {
                    contador++;
                }
            }
            this.Query_TF.Add(item, contador / this.queryToken.Length);
        }
    }
    ////QueryIDF

    public void Query_IDF_()
    {
        ManejoArchivos Objeto1 = new ManejoArchivos("E:/Mi primer proyecto/Database");
        double TotalDeDocumentos = Objeto1.ArchivosTxt.Length;
        foreach (var item in this.queryToken)
        {
            this.Query_IDF.Add(item, Math.Log10(TotalDeDocumentos + 1 / Objeto1.DondeEstaLaPalabra(item, Objeto1.NombresvsPalabras, Objeto1.ArchivosTxt) + 1));
        }
        // for (int i = 0; i < this.queryToken.Length; i++)
        // {

        //     this.Query_IDF.Add(this.queryToken[i], Math.Log10(TotalDeDocumentos + 1 / Objeto1.DondeEstaLaPalabra(this.queryToken[i], Objeto1.NombresvsPalabras, Objeto1.ArchivosTxt) + 1));

        // }

    }

    ////QueryTF-IDF
    public void Query_TF_IDF_()
    {
        foreach (var item in this.queryPalabrasUnicas)
        {
            this.Query_TF_IDF.Add(item, this.Query_TF[item] * this.Query_IDF[item]);
        }
    }

    public void Motor2()
    {
        QueryToken();
        QueryPalabrasMinusculas(queryToken);
        Query_TF_();
        Query_IDF_();
        System.Console.WriteLine("Datos cargados");
    }	

}




class Program
{
    static void Main()
    {
        ManejoArchivos Objeto1 = new ManejoArchivos("E:/Mi primer proyecto/Database");
        Objeto1.Motor();
        Query Objeto2 = new Query("The Project Gutenberg");
        Objeto2.Motor2();
        // System.Console.WriteLine(String.Join(", ", Objeto1.));
        // Console.ReadLine(); // Agrega esta línea.
    }
}



