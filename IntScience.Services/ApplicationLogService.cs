using FluentValidation.Results;
using IntScience.Repository;
using IntScience.Repository.Models;
using IntScience.Services.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntScience.Services;

public class ApplicationLogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationLogService> _logger;
    private readonly ApplicationLogValidator _logValidator;

    public ApplicationLogService(ApplicationDbContext context, ILogger<ApplicationLogService> logger)
    {
        _context = context;
        _logger = logger;
        _logValidator = new ApplicationLogValidator();
    }

    public async Task<List<ApplicationLog>> GetAllRecordsAsync()
    {
        try
        {
            return await _context.ApplicationLogs.AsNoTracking().OrderByDescending(a => a.Created).ToListAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("An error occurred while fetching data: {message}", ex?.InnerException?.Message);
            throw ex;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {message}", ex.Message);
            throw;
        }
    }

    public async Task<ApplicationResult<ApplicationLog>> InsertNewRecordAsync(ApplicationLog logToInsert)
    {
        ValidationResult result = _logValidator.Validate(logToInsert);

        if (!result.IsValid)
        {
            return ApplicationResult<ApplicationLog>.Failure(result.Errors.Select(e => e.ErrorMessage).ToList());
        }

        try
        {
            _context.ApplicationLogs.Add(logToInsert);
            await _context.SaveChangesAsync();

            return ApplicationResult<ApplicationLog>.Success(logToInsert);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("An error occurred while inserting a record: {message}", ex?.InnerException?.Message);
            throw ex;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {message}", ex.Message);
            throw;
        }
    }
}
