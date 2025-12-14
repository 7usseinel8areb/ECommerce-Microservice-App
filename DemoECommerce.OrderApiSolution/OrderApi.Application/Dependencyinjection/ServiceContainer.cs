using eCommerce.SharedLibrary.DependencyInjection;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.Dependencyinjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add application services here
        // Register Http Client
        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"] ??
                throw new InvalidOperationException("OrderService URL is not configured."));

            options.Timeout = TimeSpan.FromSeconds(1);
        });

        // Create retry strategy
        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args => 
            {
                string message = $"On Retry, Attempt: {args.AttemptNumber} Outcome {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
 
                return ValueTask.CompletedTask;
            }
        };
        /*
         
         | الخاصية                                                                     | معناها                                                                                                                     |
| --------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| **`ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>()`** | معناها: “أعد المحاولة فقط لو الخطأ اللي حصل هو `TaskCanceledException`”، يعني مثلاً لو العملية تم إلغاؤها أو حصل تايم آوت. |
| **`BackoffType = DelayBackoffType.Constant`**                               | نوع التأخير بين المحاولات. هنا “ثابت” — يعني كل مرة ننتظر نفس المدة.                                                       |
| **`UseJitter = true`**                                                      | يضيف “عشوائية بسيطة” على التأخير لتجنب أن كل السيرفرات تحاول في نفس الوقت (يساعد على تجنب ضغط الشبكة).                     |
| **`MaxRetryAttempts = 3`**                                                  | عدد المحاولات القصوى لإعادة التنفيذ (3 مرات قبل أن يفشل نهائيًا).                                                          |
| **`Delay = TimeSpan.FromMilliseconds(500)`**                                | الزمن بين كل محاولة وأخرى (500 ملي ثانية).                                                                                 |
| **`OnRetry = args => { ... }`**                                             | كود يتم تشغيله في كل محاولة فاشلة قبل إعادة المحاولة التالية — هنا يتم تسجيل (Log) رقم المحاولة ونتيجتها.                  |

         
         */
        // Use Retry strategy
        services.AddResiliencePipeline("my-retry-pipeline", builder =>
        {
            builder.AddRetry(retryStrategy);
        });
        /*
         
         داخلها بتضيف سياسة إعادة المحاولة (Retry Policy) اللي أنشأتها فوق.

الهدف منها إنك تستخدمها لاحقًا لتغليف أي عملية ممكن تفشل مؤقتًا (زي HTTP أو قاعدة بيانات) علشان تعيد المحاولة تلقائيًا بدل ما تفشل فورًا.
         */

        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        // Use Shared Policies
        // Register Middlewares such as handle external errors, logging, etc.
        // Listen to only api gateway calls 
        SharedService.UseSharedPolicies(app);

        return app;
    }
}
