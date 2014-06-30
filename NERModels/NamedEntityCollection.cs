using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalUniProject.NERModels
{
    public static class NamedEntityCollection
    {
        public static List<NamedEntity> namedEntities = new List<NamedEntity>();

        public static void AddNamedEntity(NamedEntity entity) {
            namedEntities.Add(entity);
        }   
    }
}