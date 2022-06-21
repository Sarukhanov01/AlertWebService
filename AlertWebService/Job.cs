using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertWebService
{
    public class Job
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Definition { get; set; }
        public int Status { get; set; }
        public int Period { get; set; }
        public DateTime Last_dt { get; set; }
        public DateTime Next_dt { get; set; }
        public string Log_Path { get; set; }

        //    //public Jobs(int id, string method, string definition, int status, int period, DateTime last_dt, DateTime next_dt, string log_path)
        //    //{
        //    //    Id = id;
        //    //    Method = method;
        //    //    Definition = definition;
        //    //    Status = status;
        //    //    Period = period;
        //    //    Last_dt = last_dt;
        //    //    Next_dt = next_dt;
        //    //    Log_Path = log_path;
        //    //}
    }
}
