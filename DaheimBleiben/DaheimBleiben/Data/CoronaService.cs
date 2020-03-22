using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChartJs.Blazor.ChartJS.Common.Time;
using LiteDB;

namespace DaheimBleiben.Data
{
    public class CoronaService
    {
        private const string DATA = "MyData.db";

        public void Update()
        {
            using (var db = new LiteDatabase(DATA))
            {
                var col = db.GetCollection<CoronaCounter>("coronacounter");
                var entry = col.Query().Where(x => x.CoronaDay == DateTime.Today).SingleOrDefault();

                if (entry != null)
                {
                    entry.Count++;
                    col.Update(entry);
                }
                else
                {
                    var coronaDay = new CoronaCounter
                    {
                        CoronaDay = DateTime.Today,
                        Count = 1
                    };
                    col.Insert(coronaDay);
                }
            }
        }

        public long GetCurrentNumber()
        {
            using (var db = new LiteDatabase(DATA))
            {
                var col = db.GetCollection<CoronaCounter>("coronacounter");

                var entry = col.Query().Where(x => x.CoronaDay == DateTime.Today).SingleOrDefault();

                if (entry != null)
                {
                    return entry.Count;
                }
                else
                {
                    return (long)0;
                }
            }
        }

        public List<TimeTuple<long>> GetData()
        {
            List<TimeTuple<long>> ret = new List<TimeTuple<long>>();

            using (var db = new LiteDatabase(DATA))
            {
                var col = db.GetCollection<CoronaCounter>("coronacounter");
                var list = col.Query().Where(x => x.CoronaDay <= DateTime.Today && x.CoronaDay >= DateTime.Today.AddDays(-14)).ToList();

                foreach (var item in list)
                {
                    ret.Add(new TimeTuple<long>(new Moment(item.CoronaDay),item.Count));
                }
            }

            return ret;
        }
    }
}
