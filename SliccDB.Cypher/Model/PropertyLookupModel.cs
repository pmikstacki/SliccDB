namespace SliccDB.Cypher.Model
{
    public class PropertyLookupModel
    {
        /// <summary>
        /// Name of a variable holding reference to
        /// node/relation with certain property
        /// </summary>
        public string ScopeVariableName { get; set; }
        /// <summary>
        /// Name of a property associated with node/relation
        /// (after ".")
        /// example: ".propertyName" -> "propertyName"
        /// </summary>
        public string ScopePropertyName { get; set; }
    }
}