using System.Data;
using System.Text;
using System.Text.Json;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PipelineSteps.Definitions;
using PipelineSteps.Enums;
using PipelineSteps.Events;
using PipelineSteps.Interfaces.Internals;
using PipelineSteps.Models;

namespace PipelineSteps.Services;

public class EventPersistence : IEventPersistence
{
    private readonly string _connString;
    private readonly string _tableName;
    private readonly string _schemaName;
    private readonly ILogger<EventPersistence> _logger;

    public bool PersistStepEventsActive { get; set; }
    public Queue<StepEvent> Events { get; set; }

    public EventPersistence(PipelineOptions options, ILogger<EventPersistence> logger)
    {
        PersistStepEventsActive = options.PersistStepEvents;
        _logger = logger;

        if (!options.PersistStepEvents) 
            return;
        
        Events = new Queue<StepEvent>();
        _connString = options.DatabaseConnString;
        _tableName = options.PersistEventsTableName;
        _schemaName = options.PersistEventsSchemaName;
    }

    public void AddEvent<TData>(PipelineExecutionContext context, int stepId, string bodyName, string stepName, StepResultType stepResult, TData data) where TData : class
    {
        Events.Enqueue(new StepEvent()
        {
            StepId = stepId,
            Result = stepResult,
            EventTimeUtc = DateTime.UtcNow,
            BodyName = bodyName,
            StepName = stepName,
            DataJson = JsonSerializer.Serialize(data),
            WorkflowName = context.WorkflowName,
            ContextId = context.ContextId,
            IsBranch = context.IsBranch ? 1 : 0,
            BranchId = context.BranchId,
            BranchName = context.BranchName
        });
    }

    public async Task PersistEvents()
    {
        if (!PersistStepEventsActive || Events.Count == 0)
            return;

        if (string.IsNullOrEmpty(_connString))
        {
            _logger.LogError("Missing connection string for workflow {_tableName}.  Cannot persist events to database.", _tableName);
            return;
        }

        try
        {
            string statement = BuildStatement();
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            await using SqlConnection conn = new(_connString);
            await conn.OpenAsync();
            await using SqlCommand cmd = new(statement, conn);

            cmd.CommandType = CommandType.Text;
            await cmd.ExecuteNonQueryAsync();

            scope.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EventPersistence error: {Message}", ex.Message);
        }
    }

    public string BuildStatement()
    {
        int eventsCount = Events.Count;
        StringBuilder sql = new StringBuilder("INSERT INTO ")
            .AppendFormat("{0}.{1} (WorkflowName, ContextId, StepId, Result, EventTimeUtc, BodyName, StepName, IsBranch, BranchId, BranchName, DataJson) VALUES", _schemaName, _tableName);

        for (int i = 0; i < eventsCount; i++)
        {
            var item = Events.Dequeue();

            sql.AppendFormat("('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}', {7}, {8}, '{9}', '{10}')", 
                item.WorkflowName, item.ContextId, item.StepId, item.Result, item.EventTimeUtc, item.BodyName, item.StepName, item.IsBranch, item.BranchId, item.BranchName, item.DataJson);

            if (i + 1 < eventsCount)
                sql.AppendLine().Append(", ");
        }

        return sql.ToString();
    }
}
