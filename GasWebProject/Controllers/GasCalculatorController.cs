using GasWebProject.Data.DTOs;
using GasWebProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace GasWebProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GasCalculatorController : ControllerBase
    {
        private readonly IGasCalculatorService _calculator;

        public GasCalculatorController(IGasCalculatorService calculator)
        {
            _calculator = calculator;
        }

        [HttpGet("components")]
        public IActionResult GetComponents()
        {
            try
            {
                var components = _calculator.GetAvailableComponents();
                return Ok(components);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("components/add")]
        public IActionResult AddComponent([FromBody] AddComponentDto dto)
        {
            try
            {
                _calculator.AddComponent(dto);
                return Ok(new { Message = "Компонент успешно добавлен!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("convert")]
        public IActionResult ConvertMixture([FromBody] ConversionRequest request)
        {
            try
            {
                var result = _calculator.Convert(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
