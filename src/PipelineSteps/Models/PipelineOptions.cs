using PipelineSteps.Enums;

namespace PipelineSteps.Models;

public class PipelineOptions
{
    /// <summary>
    /// Will apply condition to run post-workflow middleware.  <see cref="PostMiddleware.OnSuccess"/>  
    /// will execute the pipeline on successful runs only.  <see cref="PostMiddleware.Always"/> will execute
    /// the pipeline regardless of workflow errors or result.  Default is <see cref="PostMiddleware.OnSuccess"/>.
    /// </summary>
    public PostMiddleware RunPostWorkflowMiddlewareCondition { get; set; } = PostMiddleware.OnSuccess;
    /// <summary>
    /// Persist step events to database.  Default is false.
    /// </summary>
    public bool PersistStepEvents { get; set; }
    public string? DatabaseConnString { get; set; }
    /// <summary>
    /// Persistence table name.  The default is 'WorkflowEvents'.
    /// </summary>
    public string PersistEventsTableName { get; set; } = "WorkflowEvents";
    /// <summary>
    /// Persistence table schema name.  Default is 'ODS'.
    /// </summary>
    public string PersistEventsSchemaName { get; set; } = "ODS";
}