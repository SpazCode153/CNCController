export class signalRRetryPolicy implements signalR.IRetryPolicy {
    nextRetryDelayInMilliseconds(context: signalR.RetryContext) {
        if (context.retryReason.message === 'WebSocket is not in the OPEN state') {
            return 0;
        } else {
            return 15000;
        }
    }
}