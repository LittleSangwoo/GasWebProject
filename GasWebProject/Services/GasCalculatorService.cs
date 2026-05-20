using GasWebProject.Data;
using GasWebProject.Data.DTOs;
using GasWebProject.Models;

namespace GasWebProject.Services
{
    public interface IGasCalculatorService
    {
        void AddComponent(AddComponentDto dto);
        List<ComponentDto> Convert(ConversionRequest request); // Возвращаем старый тип
        List<GasInfoDto> GetAvailableComponents();
    }

    public class GasCalculatorService : IGasCalculatorService
    {
        private readonly GasDbContext _db;
        private const double Vm = 24.04;

        public GasCalculatorService(GasDbContext db)
        {
            _db = db;
        }

        public List<GasInfoDto> GetAvailableComponents()
        {
            // Теперь мы достаем из БД не только имя, но и номинал
            return _db.Components.Select(c => new GasInfoDto
            {
                Name = c.Name,
                Nominal = c.Nominal
            }).ToList();
        }



        public void AddComponent(AddComponentDto dto)
        {
            if (_db.Components.Any(c => c.Name.ToLower() == dto.Name.ToLower()))
                throw new Exception("Компонент с таким именем уже существует!");

            _db.Components.Add(new GasComponent
            {
                Name = dto.Name,
                M = dto.M,
                Z = dto.Z,
                Nominal = dto.Nominal
            });
            _db.SaveChanges();
        }

        public List<ComponentDto> Convert(ConversionRequest request)
        {
            var componentNames = request.Components.Select(c => c.Name).ToList();
            var dbRefs = _db.Components.Where(c => componentNames.Contains(c.Name)).ToDictionary(c => c.Name, c => c);

            var remainderComp = request.Components.First(c => c.IsRemainder);
            string remainderName = remainderComp.Name;

            if (request.SourceUnit != UnitType.MassConcentration)
            {
                double currentSum = request.Components.Where(c => !c.IsRemainder).Sum(c => c.Value);
                remainderComp.Value = 1.0 - currentSum;
            }

            // 1. Всегда переводим во внутреннюю "шину" (Молярную долю)
            var molarFractions = ToMolar(request.Components, request.SourceUnit, dbRefs);

            // 2. Из молярной переводим в ту, которую запросил пользователь
            var resultFractions = FromMolar(molarFractions, request.TargetUnit, dbRefs);

            // 3. Балансируем остаток (Anti-drift)
            if (request.TargetUnit != UnitType.MassConcentration)
                ApplyAntiDrift(resultFractions, remainderName);
            else
                foreach (var mc in resultFractions) mc.Value = Math.Round(mc.Value, 0);

            return resultFractions;
        }

        private List<ComponentDto> ToMolar(List<ComponentDto> source, UnitType sourceType, Dictionary<string, GasComponent> dbRefs)
        {
            var result = new List<ComponentDto>();
            double sumDenominator = 0;

            if (sourceType == UnitType.VolumeFraction)
                sumDenominator = source.Sum(c => c.Value / dbRefs[c.Name].Z);
            else if (sourceType == UnitType.MassFraction)
                sumDenominator = source.Sum(c => c.Value / dbRefs[c.Name].M);

            foreach (var comp in source)
            {
                var dbRef = dbRefs[comp.Name];
                double molarValue = 0;

                switch (sourceType)
                {
                    case UnitType.VolumeFraction:
                        molarValue = (comp.Value / dbRef.Z) / sumDenominator; // Убрали * 100
                        break;
                    case UnitType.MassFraction:
                        molarValue = (comp.Value / dbRef.M) / sumDenominator; // Убрали * 100
                        break;
                    case UnitType.MassConcentration:
                        // Масс. концентрация требует умножения на 100 для перевода в доли (т.к. формула использует %)
                        molarValue = (comp.Value * Vm) / (dbRef.M * 1000.0 * 100.0);
                        break;
                    case UnitType.MolarFraction:
                        molarValue = comp.Value;
                        break;
                }
                result.Add(new ComponentDto { Name = comp.Name, Value = molarValue });
            }
            return result;
        }

        private List<ComponentDto> FromMolar(List<ComponentDto> molarFractions, UnitType targetType, Dictionary<string, GasComponent> dbRefs)
        {
            var result = new List<ComponentDto>();
            double sumDenominator = targetType == UnitType.VolumeFraction
                ? molarFractions.Sum(c => c.Value * dbRefs[c.Name].Z)
                : targetType == UnitType.MassFraction
                    ? molarFractions.Sum(c => c.Value * dbRefs[c.Name].M)
                    : 1;

            foreach (var comp in molarFractions)
            {
                var dbRef = dbRefs[comp.Name];
                double targetValue = 0;

                switch (targetType)
                {
                    case UnitType.VolumeFraction:
                        targetValue = (comp.Value * dbRef.Z) / sumDenominator;
                        break;
                    case UnitType.MassFraction:
                        targetValue = (comp.Value * dbRef.M) / sumDenominator;
                        break;
                    case UnitType.MassConcentration:
                        // Восстанавливаем % для формулы МК (умножаем на 100)
                        targetValue = (comp.Value * 100.0 * dbRef.M * 1000.0) / Vm;
                        break;
                    case UnitType.MolarFraction:
                        targetValue = comp.Value;
                        break;
                }
                result.Add(new ComponentDto { Name = comp.Name, Value = targetValue });
            }
            return result;
        }

        private void ApplyAntiDrift(List<ComponentDto> fractions, string remainderName)
        {
            foreach (var comp in fractions)
            {
                comp.Value = Math.Round(comp.Value, 6, MidpointRounding.AwayFromZero);
            }

            var remainderComp = fractions.First(c => c.Name == remainderName);
            double othersSum = fractions.Where(c => c.Name != remainderName).Sum(c => c.Value);
            remainderComp.Value = Math.Round(1.0 - othersSum, 6); // Погрешность до 1.0
        }
    }
}