﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class ManejoArchivos
{
    // Constructor...
    public ManejoArchivos(string RutaCarpeta )
    {
        rutaCarpeta = RutaCarpeta;
        ArchivosTxt = Directory.GetFiles(this.rutaCarpeta,"*.txt");
        ContenidoArchivos = new string[ArchivosTxt.Length];
        
    }
    public string rutaCarpeta{get;set;}
    public string[] ArchivosTxt{get;set;}
    public string[] ContenidoArchivos{get;set;}
    public Dictionary<string, string[]> PalabrasUnicas= new Dictionary<string, string[]>();
    public Dictionary<string, string[]> NombresvsPalabras = new Dictionary<string, string[]>();
    public Dictionary<string, Dictionary< string,double>> Textos_Palabras_TF = new Dictionary<string, Dictionary< string,double>>(); 
    public Dictionary<string, Dictionary< string,double>> Textos_Palabras_IDF = new Dictionary<string, Dictionary< string,double>>(); 
 public Dictionary<string, Dictionary< string,double>> DiccionarioTF_IDF = new Dictionary<string, Dictionary< string,double>>();
    

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
        char[] delimitadores = { ' ', ',', '.', ':', '¿', '?','!','*','/','"','#',')','(', };
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
      
      /// metodo contador para tf-idf
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
        foreach (var item in this.Textos_Palabras_TF)
        {
            Dictionary<string, double> DiccionarioTF_IDF = new Dictionary<string, double>();
            foreach (var item2 in item.Value)
            {
                DiccionarioTF_IDF.Add(item2.Key, item2.Value * this.Textos_Palabras_IDF[item.Key][item2.Key]);
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
    
   static void Main()
{
    ManejoArchivos Objeto1 = new ManejoArchivos("E:/Mi primer proyecto/Database");
    Objeto1.Motor();
    // System.Console.WriteLine(String.Join(", ", Objeto1.));
    // Console.ReadLine(); // Agrega esta línea.
}


}