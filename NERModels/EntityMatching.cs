using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimMetricsUtilities;
using SimMetricsMetricUtilities;
using SimMetricsApi;

namespace FinalUniProject.NERModels
{
    public static class EntityMatching
    {
        //public static List<MasterEntityObject> matchedEntities = new List<MasterEntityObject>();
        //private static Levenstein metric = new Levenstein();
        //private static double maxDistance = 2;
        //private static TimeSpan maxAge = new TimeSpan(1, 0, 0);

        //private static bool hasMatch(NamedEntity entity)
        //{
        //    matchedEntities.ForEach(item =>
        //        {
                    
        //        });
        //    return true;
        //}
        //private static void addToMasterEntity(NamedEntity entity, MasterEntityObject match)
        //{
        //    match.matchingEntities.Add(entity);
        //}
        //private static void getLastMentionedTimeSpanForMasterEntity(NamedEntity entity)
        //{
        //    // iterate over matched entities collection
        //    matchedEntities.ForEach(item =>
        //    {
        //        DateTime lastUpdated = item.lastUpdated;
        //        DateTime now = DateTime.Now;

        //        // Calculate the TimeSpan between now and the last updated time
        //        TimeSpan difference = now.Subtract(lastUpdated);

        //        // If difference is more than max age allowed, then remove the master entity from the List 
        //        if (difference > maxAge)
        //        {
        //            matchedEntities.Remove(item);
        //        }
        //    });
        //}
        //private static bool hasValueMatch(NamedEntity entity)
        //{
        //    string entityValue = entity.Name;
        //    matchedEntities.ForEach(item =>
        //    {
        //        string bestGuessApproximation = item.matchedValueApproximation;
        //        bool isMatch = getSimilarityForTwoStrings(entityValue, bestGuessApproximation);
        //        if (isMatch)
        //        {
        //            //item.matchingEntities.Add(entity);
        //            //item.lastUpdated = DateTime.Now;
        //            return;
        //        }
        //    });
        //    return false;
        //}
        //private static bool getSimilarityForTwoStrings(string name1, string name2)
        //{
        //    double actualDistance = metric.GetSimilarity(name1, name2);
        //    if (actualDistance < maxDistance)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}