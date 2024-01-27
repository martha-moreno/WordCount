using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGrease.Css.Ast;
using static System.Net.WebRequestMethods;

// Assignment 8 
//CSE 598
//create a Web application platform that allows user
//to perform automated parallel computing that mimics the Hadoop process.
namespace A8_WordCount
{

    public partial class _Default : Page
    {
        public static string str;
        protected void Page_Load(object sender, EventArgs e)
        {
            txt_N.Text = "1";
            txt_WebServiceMapFunction.Text = "http://localhost:50472/Service1.svc";
            txt_WebServiceReduceFunction.Text = "http://localhost:50472/Service1.svc";
            txt_WebServiceCombinerFunction.Text = "http://localhost:50472/Service1.svc";
        }

        string GetFileContents(HttpPostedFile file)
        {
            str = new StreamReader(file.InputStream).ReadToEnd();
            return str;
        }
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            lbl_warning.Visible= false;
            if (FileUploadControl.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUploadControl.FileName);
                    string path = Server.MapPath("~/") + "Files/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileUploadControl.SaveAs(path + filename);
                    lbl_status.Text = "File uploaded successfully!";
                }
                catch (Exception ex)
                {
                    lbl_status.Text = "The following error occurred: " + ex.Message;
                }
                GetFileContents(FileUploadControl.PostedFile);
            }
        }

        protected void btn_PerformMapReduce_Click(object sender, EventArgs e)
        {
            if(str!=null)
            {
                /*********NameNode: START- It splits the uploaded Data File into N subsets and provides a subset to each TaskTracker*****************/
                List<string> mapResponseList = new List<string>();
                List<string> reduceResponseList = new List<string>();
                string[] words = Regex.Split(str, @"\W+");
                string[] lower_words = new string[words.Length];
                for (int i = 0; i < words.Length; i++)
                {
                    lower_words[i] = words[i].ToLower();
                }

                int input_N = Int32.Parse(txt_N.Text);
                //-----MAP----------------------------------------
                //Make the partitions requested by the user and convert to a list of strings
                //Create a proxy to the WCF service
                ServiceReference.Service1Client myProxy = new ServiceReference.Service1Client();

                var mapResponse = myProxy.Map(input_N, lower_words);
                if (mapResponse != null)
                {

                    // lbl_results.Text = mapResponse[0] +"-----------" + mapResponse[1];
                    /*********NameNode:END********************************************************/

                    /**********TaskTracker: START - execute individual map and reduce tasks on each cluster node************************************************/

                    //---------Map & Reduce each cluster
                    List<string> reducedResponseList = new List<string>();//saves the response of Reduce service for each partition in a list of string that contains key value pairs

                    for (int i = 0; i < mapResponse.Count(); i++)
                    {
                        //-----REDUCE----------------------------------------
                        var reducedResponse = myProxy.Reduce(mapResponse[i]);//call Reduce service
                        reducedResponseList.Add(String.Join(" ", reducedResponse)); //add the response of Reduce service to a list of strings
                    }

                    /**********TaskTracker: END*************************************************/
                    //lbl_results.Text = String.Join(" ", reducedResponseList[1]); 

                    //----------------COMBINER-----------------------------

                    //Merging reducers into a list
                    List<string> combinedReducers = new List<string>();
                    for (int i = 0; i < reducedResponseList.Count(); i++)
                    {
                        combinedReducers.Add(reducedResponseList[i]);
                    }

                    //reducers list into Array
                    string[] reducersArray = combinedReducers.ToArray();

                    //Call Combiner Service
                    var combinerResponse = myProxy.Combiner(reducersArray);
                    //Printing results in key value pairs
                    lbl_results.Text = String.Join(" ", combinerResponse);

                }


            }
            else
            {
                lbl_warning.Visible = true;
            }

        }
    }
}