using System.Text;

namespace WebAPIAFA.Helpers
{
    public class ErrorLogs
    {
        public void SaveGeneralLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string message = BuildMessage(ex);

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "logException.txt");

                //Preparar el mensaje
                var strBuilder = new StringBuilder();
                strBuilder.Append("FECHA: ");
                strBuilder.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ", " + Environment.NewLine);
                strBuilder.Append("Excepcion completa: " + Environment.NewLine + message + Environment.NewLine);
                strBuilder.Append("-------------------------------------------------------------------- " + Environment.NewLine + Environment.NewLine);

                sw = new StreamWriter(filePath, true, Encoding.UTF8);
                sw.WriteLine(strBuilder.ToString());
                sw.Close();
            }
            catch (Exception)
            {
                //throw ex;
                throw new Exception("Ocurrió un error al generar el log");
            }
            finally
            {
                if (sw != null) sw.Dispose();
            }
        }

        private string BuildMessage(Exception ex)
        {
            StringBuilder message = new StringBuilder();

            int contMax = 5;

            int cont = 1;

            while (ex != null && cont <= contMax)
            {
                message.AppendLine(Environment.NewLine);
                message.AppendLine("Excepcion: " + ex.Message);
                message.AppendLine("StackTrace: " + ex.StackTrace);
                message.AppendLine(Environment.NewLine);
                ex = ex.InnerException;

                cont++;
            }

            return message.ToString();
        }
    }
}
