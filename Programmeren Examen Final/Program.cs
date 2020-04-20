using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class Program
    {
        static void Main()
        {
            //setup
            string zipPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata.zip";
            string unzippedPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\WRdata-master";
            string reportPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\Rapporten";
            string serializingPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata";

            Unzipper.Unzip(zipPath);
            FileExtraction fe = new FileExtraction();
            Belgie belg = fe.ExtractFromFiles(unzippedPath);

            Console.WriteLine("Writing Report");
            ReportWriter rw = new ReportWriter(reportPath);
            rw.CompleteReport(belg);

            Serializer serializer = new Serializer(belg,serializingPath);
            serializer.Serialize();
            Console.WriteLine("finished");
        }
    }
}
