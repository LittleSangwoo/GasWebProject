using System.ComponentModel.DataAnnotations;

namespace GasWebProject.Models
{
    public class GasComponent
    {
        [Key]
        public string Name { get; set; } = null!;

        // Молярная масса
        public double M { get; set; }

        // Коэффициент сжимаемости
        public double Z { get; set; }

        public double? Nominal { get; set; }
    }
}
