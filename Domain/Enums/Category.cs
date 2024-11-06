using System.ComponentModel;

namespace Domain.Enums
{
    public enum Category
    {
        [Description("Action")]
        Action,
        [Description("Comedy")]
        Comedy,
        [Description("Fantasy")]
        Fantasy,
        [Description("Thriller")]
        Thriller,
    }
}
