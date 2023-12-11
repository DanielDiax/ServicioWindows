using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServicioWindows
{
    partial class Archivos : ServiceBase
    {
        bool blBandera = false;
        public Archivos()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            stLapso.Start();
        }

        protected override void OnStop()
        {
            stLapso.Stop();
        }

        private void stLapso_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (blBandera == true) return;

            try
            {
                blBandera = true;

                EventLog.WriteEntry("Se inicio el evento de copiado", EventLogEntryType.Information);

                string stRutaOrigen = ConfigurationSettings.AppSettings["stRutaOrigen"].ToString();
                string stRutaDestino = ConfigurationSettings.AppSettings["stRutaDestino"].ToString();
                DirectoryInfo directoryInfo = new DirectoryInfo(stRutaOrigen);

                foreach (var archivo in directoryInfo.GetFiles())
                {
                    if (File.Exists(stRutaDestino + archivo.Name))
                    {
                        File.SetAttributes(stRutaDestino + archivo.Name, FileAttributes.Normal);
                        File.Delete(stRutaDestino + archivo.Name);
                    }

                    File.Copy(stRutaOrigen + archivo.Name, stRutaDestino + archivo.Name);
                    File.SetAttributes(stRutaDestino + archivo.Name, FileAttributes.Normal);

                    if (File.Exists(stRutaDestino + archivo.Name))
                    {
                        EventLog.WriteEntry("Se ha copiado el archivo con éxito", EventLogEntryType.Information);
                    }
                    else
                    {
                        EventLog.WriteEntry("No se copió el archivo", EventLogEntryType.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                blBandera = false;
            }
        }
    }
}
