using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageClassifierApp.Annotations;

namespace ImageClassifierApp.Services.Async
{
    public class TaskWrapperEventArgs<TResult> : EventArgs
    {
        public TResult Result { get; set; }
    }

    public class TaskWrapper<TResult> : Task<TResult> where TResult : class
    {
        public event EventHandler<TaskWrapperEventArgs<TResult>> TaskFinished;
        private Func<TResult> _func;

        private TResult EventWrapper()
        {
            var result = _func?.Invoke();
            TaskFinished?.Invoke(this, new TaskWrapperEventArgs<TResult>() { Result = result });
            return result;
        }

        public TaskWrapper([NotNull] Func<TResult> function) : base(function)
        {
        }

        public TaskWrapper([NotNull] Func<TResult> function, CancellationToken cancellationToken) : base(function, cancellationToken)
        {
        }

        public TaskWrapper([NotNull] Func<TResult> function, TaskCreationOptions creationOptions) : base(function, creationOptions)
        {
        }

        public TaskWrapper([NotNull] Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, cancellationToken, creationOptions)
        {
        }

        public TaskWrapper(Func<object, TResult> function, object state) : base(function, state)
        {
        }

        public TaskWrapper([NotNull] Func<object, TResult> function, object state, CancellationToken cancellationToken) : base(function, state, cancellationToken)
        {
        }

        public TaskWrapper([NotNull] Func<object, TResult> function, object state, TaskCreationOptions creationOptions) : base(function, state, creationOptions)
        {
        }

        public TaskWrapper([NotNull] Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(function, state, cancellationToken, creationOptions)
        {
        }
    }
}
