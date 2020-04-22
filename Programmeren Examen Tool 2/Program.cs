using System;
using System.Collections.Generic;
using Programmeren_Examen_Tool_1;

namespace Programmeren_Examen_Tool_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting unserializing");
            string binPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\Belgie.bin";
            SerializationExtraction SE = new SerializationExtraction();

            Belgie belg = SE.Unserialize(binPath);
            DataProcessing dP = new DataProcessing(belg);

            Console.WriteLine("started filling database");
            //dP.CompletelyFillDataBase();
            //dP.FillDataBaseWithProvincie(belg.Provincies[0]);
            //dP.FillDataBaseWithProvincie(belg.Provincies[1]);
            //dP.FillDataBaseWithProvincie(belg.Provincies[2]);
            //dP.FillDataBaseWithProvincie(belg.Provincies[3]);
            dP.FillDataBaseWithProvincie(belg.Provincies[4]);
        }

            //Console.WriteLine("testen");
            //Belgie belg = TestData();
            ////Knoop knoop1 = new Knoop(30, new Punt(2.45, 3.45));
            ////dp.FillDataBase();
        //}

        static Belgie TestData()
        {
            Punt testPunt1 = new Punt(2.45, 3.45);
            Punt testPunt2 = new Punt(3.45, 4.45);
            Punt testPunt3 = new Punt(4.45, 5.45);
            Punt testPunt4 = new Punt(5.45, 6.45);
            Punt testPunt5 = new Punt(6.45, 7.45);
            Knoop knoop1 = new Knoop(30, testPunt1);
            Knoop knoop2 = new Knoop(40, testPunt2);
            Knoop knoop3 = knoop1;
            Knoop knoop4 = new Knoop(50, testPunt3);

            List<Punt> puntlijstSeg1 = new List<Punt> { testPunt4 };
            List<Punt> puntlijstSeg2 = new List<Punt> { testPunt5 };

            Segment testSegment1 = new Segment(112,knoop1,knoop2,puntlijstSeg1);
            Segment testSegment2 = new Segment(113, knoop3, knoop4, puntlijstSeg2);

            Dictionary<Knoop, List<Segment>> testMap1 = new Dictionary<Knoop, List<Segment>>();
            testMap1.Add(knoop1, new List<Segment> { testSegment1 });
            testMap1.Add(knoop2, new List<Segment> { testSegment1 });
            Dictionary<Knoop, List<Segment>> testMap2 = new Dictionary<Knoop, List<Segment>>();
            testMap2.Add(knoop1, new List<Segment> { testSegment2 });
            testMap2.Add(knoop2, new List<Segment> { testSegment2 });

            Graaf testGraaf1 = new Graaf(250,testMap1);
            Graaf testGraaf2 = new Graaf(260, testMap2);



            Gemeente testGemeente = new Gemeente(12,"testGemeente");
            Straat testStraat1 = new Straat(10, "TestStraat1", testGraaf1,testGemeente);
            Straat testStraat2 = new Straat(20, "TestStraat2", testGraaf2, testGemeente);
            Provincie testProvincie = new Provincie(1,"testProvincie");
            testProvincie.VoegGemeenteToe(testGemeente);
            Belgie belg = new Belgie(new List<Provincie>() { testProvincie });

            DataProcessing dp = new DataProcessing(belg);
            dp.CompletelyFillDataBase();

            return belg;
        }
    }
}
