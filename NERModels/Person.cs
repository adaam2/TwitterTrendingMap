namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Model Class for the Person tag returned by the Named Entity Recognition classifier
    /// </summary>
    public class Person : NamedEntity
    {
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string entityType = "Person";
    }
}