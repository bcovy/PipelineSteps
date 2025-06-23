# Test helpers for PipelineSteps

Provides support writing tests for workflows built on PipelineTestHelper

## Usage

### With xUnit

* Create a class that inherits from PipelineTestHelper
* Call the `Setup()` method in the constructor
* Implement your tests using the helper method `StartPipeline()`

```C#
public class xUnitTest : PipelineTestHelper
{
    public xUnitTest()
    {
        Setup();
    }

    [Fact]
    public void MyWorkflow()
    {
        var result = await StartPipeline(new BasicModel());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, BasicModel.Step1Ticker);
        Assert.Equal(1, BasicModel.Step2Ticker);
    }
}
```

If additional items need to be registered, override the `ConfigureServices()` method.

```c#
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddStepMiddleware<MiddleStep>();
    }
```