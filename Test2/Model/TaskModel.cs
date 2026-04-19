using System.Xml.Serialization;

namespace Test2.Model
{
    public class TaskModel
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlElement("MiddleName")]
        public string MiddleName { get; set; }

        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("Country")]
        public string Country { get; set; }
    }
}
