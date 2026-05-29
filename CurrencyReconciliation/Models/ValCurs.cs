using System.Xml.Serialization;

namespace CurrencyReconciliation.Models;

[XmlRoot("ValCurs")]
public class ValCurs
{
    [XmlAttribute("Date")]
    public string Date { get; set; } = string.Empty;

    [XmlElement("Valute")]
    public List<Valute> Valutes { get; set; } = new();
}
