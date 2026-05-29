using System.Xml.Serialization;

namespace CurrencyReconciliation.Models;

public class Valute
{
    [XmlElement("CharCode")]
    public string CharCode { get; set; } = string.Empty;

    [XmlElement("Nominal")]
    public int Nominal { get; set; }

    [XmlElement("Value")]
    public string ValueRaw { get; set; } = string.Empty;

    [XmlElement("VunitRate")]
    public string UnitRateRaw { get; set; } = string.Empty;

    public decimal Value =>
        decimal.Parse(ValueRaw.Replace(',', '.'),
            System.Globalization.CultureInfo.InvariantCulture);

    public decimal UnitRate =>
        decimal.Parse(UnitRateRaw.Replace(',', '.'),
            System.Globalization.CultureInfo.InvariantCulture);
}