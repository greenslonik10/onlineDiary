using AutoMapper;
using Diary.Entity.Models;
using Diary.Repository;
using Diary.Services.Abstract;
using Diary.Services.Models;

namespace Diary.Services.Implementation;

public class ScheduleService : IScheduleService
{
    private readonly IRepository<Schedule> scheduleRepository;
    private readonly IMapper mapper;
    public ScheduleService(IRepository<Schedule> scheduleRepository, IMapper mapper)
    {
        this.scheduleRepository = scheduleRepository;
        this.mapper = mapper;
    }

    public void DeleteSchedule(Guid id)
    {
        var scheduleToDelete = scheduleRepository.GetById(id);
        if (scheduleToDelete == null)
        {
            throw new Exception("Schedule not found");
        }

        scheduleRepository.Delete(scheduleToDelete);
    }

    public ScheduleModel GetSchedule(Guid id)
    {
        var schedule = scheduleRepository.GetById(id);
        return mapper.Map<ScheduleModel>(schedule);
    }

    public PageModel<SchedulePreviewModel> GetSchedules(int limit = 20, int offset = 0)
    {
        var schedules = scheduleRepository.GetAll();
        int totalCount = schedules.Count();
        var chunk = schedules.OrderBy(x => x.DayOfWeek).Skip(offset).Take(limit);

        return new PageModel<SchedulePreviewModel>()
        {
            Items = mapper.Map<IEnumerable<SchedulePreviewModel>>(schedules),
            TotalCount = totalCount
        };
    }

    public ScheduleModel UpdateSchedule(Guid id, UpdateScheduleModel schedule)
    {
        var existingSchedule = scheduleRepository.GetById(id);
        if (existingSchedule == null)
        {
            throw new Exception("Schedule not found");
        }

        existingSchedule.HomeTask = schedule.HomeTask;
        existingSchedule.DateTime = schedule.DateTime;
        existingSchedule.DayOfWeek = schedule.DayOfWeek;

        existingSchedule = scheduleRepository.Save(existingSchedule);
        return mapper.Map<ScheduleModel>(existingSchedule);
    }
}