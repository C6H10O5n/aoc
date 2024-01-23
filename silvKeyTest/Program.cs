using System.Data.Common;

namespace silvKeyTest
{
    internal class Program
    {
        internal enum emTreat { Grow, Thin, Weed, Harvest, Unknown }
        internal class SilvKey
        {
            public string Species { get; set; }
            public double Height { get; set; }
            public double Density { get; set; }
            public double Dbh { get; set; }
            public double GrowingStock => Density; //0.00007854 * Math.Pow(Dbh,2) * Density;


            public emTreat GetTreatment()
            {
                if (Height < 6) return emTreat.Grow;
                else if (Height > 9) return SubKeyHarvest();
                else
                {
                    if (GrowingStock < 2) return emTreat.Grow;
                    else if (GrowingStock <= 4) return emTreat.Weed;
                    else return emTreat.Thin;
                }
            }

            private emTreat SubKeyHarvest()
            {
                if (Dbh <= 12)
                    return emTreat.Grow;
                else
                {
                    if ((GrowingStock <= 16 && Species == "Oak") || (GrowingStock <= 10 && Species == "Maple"))
                        return emTreat.Grow;
                    else
                    {
                        if ((GrowingStock <= 30 && Species == "Oak") || (GrowingStock <= 20 && Species == "Maple"))
                            return emTreat.Thin;
                        else
                            return emTreat.Harvest;
                    }
                };
            }

        }

        static void Main(string[] args)
        {
            var idata =
                """
                Oak,12,21,33
                Maple,8,1,8
                Maple,7,5,1
                """.Split("\r\n");

            Console.WriteLine($"Processing Stand Treatments:");
            for (int i = 0; i < idata.Length; i++)
            {
                var d = idata[i].Split(',');
                var stand = new SilvKey
                {
                    Species = d[0],
                    Height = double.Parse(d[1]),
                    Density = double.Parse(d[2]),
                    Dbh = double.Parse(d[3])
                };

                Console.WriteLine($"  Stand {i + 1}: [{idata[i]}]:  ->Treatment = {stand.GetTreatment()}");
            }
        }
    }
}