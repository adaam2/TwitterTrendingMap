namespace FinalUniProject.NERModels
{
    public abstract class NamedEntity
    {
        /// <summary>
        /// Abstract modelling class for NER tagging - overridden by specific named entities. Used here so that all classes inherit from a single base class - polymorphic list
        /// </summary>
        protected string _name;
        protected string _entityName;
        public abstract string Name { get; set; }
        public abstract string EntityName { get; set; }
    }
}