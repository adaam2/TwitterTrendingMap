using System;
namespace FinalUniProject.NERModels
{
    public abstract class NamedEntity
    {
        /// <summary>
        /// Abstract modelling class for NER tagging - overridden by specific named entities. Used here so that all classes inherit from a single base class - polymorphic list
        /// </summary>
        protected string _name;
        public abstract string Name { get; set; }
        //public static NamedEntity Combine(NamedEntity previous, NamedEntity current)
        //{
        //    if (previous.GetType() == current.GetType())
        //    {
        //        string previousValue = previous.Name;
        //        string currentValue = current.Name;
        //        Type targetType = Type.GetType(previous.GetType().ToString());
        //        NamedEntity combined = (NamedEntity)Activator.CreateInstance(targetType);
        //        combined.Name = previousValue + " " + currentValue;
        //        return combined;
        //    }
        //    return null;
        //}
        public NamedEntity()
        {
            // default constructor
        }
        public NamedEntity(string previous, string current)
        {
            // best approximation formatting 
            _name = previous + " " + current;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}