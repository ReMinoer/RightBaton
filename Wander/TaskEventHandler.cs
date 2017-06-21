using System;
using System.Linq;
using System.Threading.Tasks;

namespace Wander
{
    public delegate Task TaskEventHandler(object sender, EventArgs e);
    public delegate Task TaskEventHandler<in TEventArgs>(object sender, TEventArgs e);

    static public class TaskEventHandlerExtension
    {
        static public async Task InvokeAsync(this TaskEventHandler taskEventHandler, object sender, EventArgs e)
        {
            await Task.WhenAll(taskEventHandler.GetInvocationList().Select(x => ((TaskEventHandler)x)(sender, e)));
        }

        static public async Task InvokeAsync<TEventArgs>(this TaskEventHandler<TEventArgs> taskEventHandler, object sender, TEventArgs e)
        {
            await Task.WhenAll(taskEventHandler.GetInvocationList().Select(x => ((TaskEventHandler<TEventArgs>)x)(sender, e)));
        }
    }
}