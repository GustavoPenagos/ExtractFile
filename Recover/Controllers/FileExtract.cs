using Microsoft.AspNetCore.Mvc;
using System;
using ImageMagick;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;

namespace Recover.Controllers
{
    public class FileExtract : ControllerBase
    {
        [HttpGet]
        [Route("/api/v1/GetFile")]
        public async Task<dynamic> GetFiles(string pathO, string exten, string pathF, int count = 0, bool convert = false)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(pathO);
                FileInfo[] archivos = dirInfo.GetFiles();
                List<string> lista = new List<string>();

                foreach (FileInfo archivo in archivos)
                {
                    string rutaDestino = Path.Combine(pathF, archivo.Name);
                    if (rutaDestino.Contains(exten))
                    {
                        if (convert)
                        {
                            lista.Add(pathO + @"\" + archivo.Name);
                        }
                        else
                        {
                            archivo.CopyTo(rutaDestino, true);
                        }
                        count++;
                    }
                }
                if (convert)
                {
                    var convertThumb = ConvertThumb(lista, pathF);
                }
                return new
                {
                    succes = count,
                    messagge = "Exitoso",
                    result = new
                    {
                        Archivos = "Numero de archivos pasados: " + count
                    }
                };
            }
            catch
            {
                return new
                {
                    succes = count,
                    messagge = "Error copiando archivos",
                    result = new
                    {
                        Archivos = "Numero de archivos pasados: " + count
                    }
                };
            }
        }

        [HttpDelete]
        [Route("/api/v1/DeleteFile")]
        public async Task<dynamic> DeleteFiles(string pathO)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathO);
            FileInfo[] archivos = dirInfo.GetFiles();

            int count = 0;
            string messagge = archivos.Length > 0 ? "Borrado completo" : "No hay archivos en la ruta";
            bool completo = archivos.Length > 0 ? true : false;
            foreach (FileInfo archivo in archivos)
            {
                archivo.Delete();
                count++;
            }

            return new
            {
                succes = true,
                message = messagge,
                result = new
                {
                    Completado = completo,
                    Messagge = messagge + ": " + count + " arvivos eliminados"
                }
            };
        }

        [HttpPost]
        [Route("/api/v1/Convert")]
        public async Task<dynamic> ConvertThumb(List<string> lista, string pathF)
        {
            try
            {
                int count = 0;
                for(int i=0; i < lista.Count; i++)
                {
                    using (var image = new MagickImage(lista[i]))
                    {
                        image.Write(pathF + @"\video" + count + ".jpg");
                    }
                    count++;
                }           

                return new
                {
                    succes = true,
                    message = "Se convirtieron exitosamente",
                    Results = new
                    {
                        Lista = lista
                    } 
                };
            }
            catch
            {
                return new
                {
                    succes = false,
                    message = "No se puede convertir",
                    resul = new
                    {
                        Lista = lista
                    }

                };
            }
            
        }


    }
}