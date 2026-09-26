using System.Xml.Serialization;

namespace Test2.Model
{
    public sealed record TaskModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
