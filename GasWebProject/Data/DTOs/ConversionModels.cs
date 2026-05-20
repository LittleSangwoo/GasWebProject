namespace GasWebProject.Data.DTOs
{
    public enum UnitType { MolarFraction, VolumeFraction, MassFraction, MassConcentration }

    public class ComponentDto
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public bool IsRemainder { get; set; }
    }

    public class ConversionRequest
    {
        public UnitType SourceUnit { get; set; }
        public UnitType TargetUnit { get; set; } // Вернули целевую единицу
        public List<ComponentDto> Components { get; set; } = new();
    }

    // DTO для добавления нового газа
    public class AddComponentDto
    {
        public string Name { get; set; } = string.Empty;
        public double M { get; set; }
        public double Z { get; set; }
        public double? Nominal { get; set; }
    }

    public class GasInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public double? Nominal { get; set; }
    }
}
