using System.Collections.Generic;
namespace Company.Scheduling.Api.Contracts;

public record RequiredSkillDto(string Name, int Count = 1);
public record CreateShiftRequest(string Date, string Start, string End, List<RequiredSkillDto> RequiredSkills);