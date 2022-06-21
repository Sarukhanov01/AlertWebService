using AlertWebService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;

class Listener
{
    static List<Job> jobs;
    public static bool onStop = true;
    public static bool isStopped = false;


    public static void Listen()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/AlertWeb/");
        listener.Start();
        Console.WriteLine("Listener Started");
        while (onStop)
        {

            /*HttpListenerContext*/
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseStr = Process();
            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            if (isStopped)
            {
                if (listener != null)
                {
                    listener.Close();
                    onStop = false;
                    //return;
                }
            }

        }
        //listener.Stop();
        //Console.WriteLine("Listener Stopped");
        //Console.Read();
    }
    //public void Stop(bool isStopped)
    //{
    //    //if (!onStop)
    //    //{
    //    if (context == null)
    //    {

    //        if (listener != null)
    //        {
    //            listener.Close();
    //            onStop = false;
    //        }
    //    }


    //    //}
    //    //listener.Stop();
    //    //Console.WriteLine("Listener Stopped");

    //}
    public static string Process()
    {
        GetJobs();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(@"<!DOCTYPE html>
                                                   <html>
                                                    <head>
                                                    <meta charset=""UTF-8"">
                                                     <style>
                                                       table,th,td {
                                                       border:1px solid gray;
                                                       border-collapse: collapse;
                                                        }
                                                        caption{
                                                                margin-bottom: 10px;
                                                            }
                                                        tr:hover {
                                                                    background-color: yellow;}
                                                     </style>
                                                    </head>
                                                    <body>
                                                     <h1><center>Metak Web Alert</center></h1>
                                                        <table>
                                                    <caption><b>Dep. Jobs Sync</b></caption>
                                                         <thead>
                                                            <tr>
                                                                <th>Id</th> <th>Method</th> <th>Definition</th><th>Status</th> <th>Period</th> <th>Last_dt</th><th>Next_dt</th> <th>Log_path</th>
                                                            </tr>
                                                        </thead>
                                                             <tbody>");
        foreach (var item in jobs)
        {

            if (item.Next_dt.Subtract(item.Last_dt).TotalMinutes != item.Period)
            {
                stringBuilder.Append($@"<tr style=""background-color: red; "">
                                             <td> {item.Id} </td><td> {item.Method} </td><td> {item.Definition} </td><td> {item.Status}</td><td> {item.Period} </td><td> {item.Last_dt.ToUniversalTime()} </td><td> {item.Next_dt} </td><td><a href=""file:///{item.Log_Path}"">{item.Log_Path}</a></td>
                                        </ tr>");

            }
            else
            {
                stringBuilder.Append($@"<tr>
                                                <td> {item.Id} </td><td> {item.Method} </td><td> {item.Definition} </td><td> {item.Status}</td><td> {item.Period} </td><td> {item.Last_dt} </td><td> {item.Next_dt} </td><td><a href=""file:///{item.Log_Path}"">{item.Log_Path}</a></td>
                                            </tr>");

            }
        }
        stringBuilder.Append(@"</tbody>
                            </table>
                           </body>
                         </html>");
        return stringBuilder.ToString();
    }
    public static List<Job> GetJobs()
    {
        jobs = new List<Job>();
        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Mtk"].ToString()))
            {
                SqlCommand sqlCommand = new SqlCommand(" SELECT * FROM jobs", connection);
                connection.Open();
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            jobs.Add(new Job
                            {
                                Id = reader.GetByte(reader.GetOrdinal("id")),
                                Method = reader.GetString(reader.GetOrdinal("method")),
                                Definition = reader.GetString(reader.GetOrdinal("definition")),
                                Status = reader.GetByte(reader.GetOrdinal("status")),
                                Period = reader.GetInt32(reader.GetOrdinal("period")),
                                Last_dt = reader.GetDateTime(reader.GetOrdinal("last_dt")),
                                Next_dt = reader.GetDateTime(reader.GetOrdinal("next_dt")),
                                Log_Path = reader.GetString(reader.GetOrdinal("log_path"))
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

            File.AppendAllText(@"log.txt", DateTime.Now + " GetHrmErpSyncs " + ex.Message != null ? ex.Message.ToString() : ex.ToString() + "\r\n");
        }

        return jobs;
    }
}
