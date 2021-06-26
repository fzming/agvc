using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace AgvcService.System.Message.Senders.Mail
{
    /// <summary>
    ///     SmtpClient that support cancelable SendMailAsync
    /// </summary>
    public class SmtpClientWrapper : IDisposable
    {
        private AsyncRetryPolicy _policy;
        private SmtpClient _smtpClient;

        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public AsyncRetryPolicy RetryPolicy
        {
            get
            {
                if (_policy != null) return _policy;
                _policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt =>
                        {
                            var timeToWait = TimeSpan.FromSeconds(Math.Pow(5, retryAttempt));
                            Console.WriteLine(
                                $"[Retry Policy({GetType().Name})]Waiting {timeToWait.TotalSeconds} seconds");
                            return timeToWait;
                        }
                    );
                return _policy;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SendMailAsync(MailMessage message, CancellationToken ct)
        {
            await RetryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    var task = _smtpClient.SendMailAsync(message);
                    using (ct.Register(_smtpClient.SendAsyncCancel))
                    {
                        try
                        {
                            await task;
                        }
                        catch (OperationCanceledException exception)
                        {
                            if (exception.CancellationToken == ct)
                            {
                                Trace.TraceWarning("Operation has been canceled.");
                                return;
                            }

                            throw;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Unexpected exception occured; Exception details: {0}", exception);
                    throw;
                }
            });
        }

        protected virtual void Dispose(bool p)
        {
            if (_smtpClient != null)
            {
                _smtpClient.Dispose();
                _smtpClient = null;
            }
        }
    }
}