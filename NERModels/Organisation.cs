namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Model Class for the Organization tag returned by the Named Entity Recognition classifier
    /// </summary>
    public class Organisation : NamedEntity
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
        public override string EntityName
        {
            get
            {
                return _entityName;
            }
            set
            {
                _entityName = "Organisation";
            }
        }
    }
}