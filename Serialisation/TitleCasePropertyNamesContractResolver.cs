namespace Evolution.Serialisation
{
    using Newtonsoft.Json.Serialization;
    
    /// <summary>Resolve TitleNamedProperties in JSON.</summary>
    sealed class TitleCasePropertyNamesContractResolver :
        DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName) => propertyName;
    }
}