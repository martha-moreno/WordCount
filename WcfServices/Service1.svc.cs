using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServices
{
    public static class Extensions
    {
        //List partition in N: Generic extension method returns a List 
        public static List<List<T>> partition<T>(this List<T> values, int chunkSize)
        {
            var partitions = new List<List<T>>();
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                partitions.Add(values.GetRange(i, Math.Min(chunkSize, values.Count - i)));
            }
            return partitions;
        }
    }

    public class Service1 : IService1
    {
        //Mapping Service-partitions the data
        

        public List<string> Map(int input_N, string[] words)
        {
            List<string> listPartitions = new List<string>();
            List<string> fullWordsList = new List<string>();

            foreach (string value in words)
            {
                fullWordsList.Add(value);
            }

            int mod = fullWordsList.Count % input_N;
            int numberOfLists;
            if (mod == 0)
            {
                numberOfLists = fullWordsList.Count / input_N; //define the number of partitions needed for the number of user threads "N" entered by the user 
            }
            else
            {
                numberOfLists = fullWordsList.Count / input_N + 1;
            }
            var partitions = fullWordsList.partition(numberOfLists);

            foreach (List<string> partition in partitions)
            {
                listPartitions.Add(String.Join(" ", partition));
            }

            return listPartitions;

        }

        //Reduce Service- creates key value pairs for each partition

        public List<string> Reduce(string partition)
        {
            var splitWordsInPartition = partition.Split(' '); //separate words in received partition
            Dictionary<string, int> keyValuePairsInPartition = new Dictionary<string, int>();//create Key Value pairs Dictionary


            for (int i = 0; i < splitWordsInPartition.Length; i++)
            {
                if (keyValuePairsInPartition.ContainsKey(splitWordsInPartition[i])) // Checking if word already exist in dictionary update the number of appearances --Dictionary does not allow for repeated keys                                             
                {
                    int keyValue = keyValuePairsInPartition[splitWordsInPartition[i]];
                    keyValuePairsInPartition[splitWordsInPartition[i]] = keyValue + 1;//add 1 per every repeated key
                }
                else
                {
                    keyValuePairsInPartition.Add(splitWordsInPartition[i], 1);//value will be 1 if key is not repeated
                }
            }

            List<string> mappedList = new List<string>();
            List<string> other = new List<string>();
            string res;

            foreach (KeyValuePair<string, int> kvp in keyValuePairsInPartition)
            {
                mappedList.Add(kvp.Key + ":" + kvp.Value);
                res = string.Join(" ", mappedList);
                other.Add(res);
            }

            string result = string.Join(" ", mappedList);
            return mappedList;

        }

        //Combiner Service - combine all reducers into final key value pairs list
        public List<KeyValuePair<string, int>> Combiner(string[] reducersArray)
        {
            //Creating key value pairs list from all reducers (combinedReducers)- which may contain duplicated keys
            // var keyValueList = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> keyValueList = new List<KeyValuePair<string, int>>();

            foreach (string item in reducersArray)
            {
                var sequence = item.Split(' ').ToList();
                foreach (string word in sequence)
                {
                    keyValueList.Add(new KeyValuePair<string, int>(word.Split(':')[0], int.Parse(word.Split(':')[1])));

                }

            }

            //Merging duplicated key-values into a new key value pair list
            List<KeyValuePair<string, int>> grouped = (from kvp in keyValueList
                                                       group kvp by kvp.Key
                                                     into g
                                                       select
                                                           new KeyValuePair<string, int>(g.Key,
                                                                                            g.Sum(a => a.Value))).
           ToList();
            
            return grouped;
        }
    }
}
