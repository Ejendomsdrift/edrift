var StackWorker = (function () {
    function StackWorker() {
        this.pool = [];
    }
    StackWorker.prototype.add = function (eventType, executeCallback) {
        var stackItem = this.getByEventType(eventType);
        if (!stackItem) {
            stackItem = {
                eventType: eventType,
                isNew: true,
                isRunned: false,
                isCompleted: false,
                funcList: []
            };
        }
        stackItem.funcList.push(executeCallback);
        if (stackItem.isRunned) {
            this.execute(stackItem);
        }
        if (stackItem.isNew) {
            this.pool.push(stackItem);
        }
    };
    StackWorker.prototype.run = function (eventType) {
        var stackItem = this.getByEventType(eventType);
        if (stackItem) {
            this.execute(stackItem);
            return;
        }
        stackItem = {
            eventType: eventType,
            isNew: true,
            isRunned: true,
            isCompleted: false,
            funcList: []
        };
        this.pool.push(stackItem);
    };
    StackWorker.prototype.execute = function (stackItem) {
        if (!stackItem.isNew && !stackItem.isCompleted) {
            throw new Error("execution was runned more than ones, previous execution was not compeleted");
        }
        stackItem.isNew = false;
        stackItem.isRunned = true;
        stackItem.funcList.forEach(function (stackItemFunc) {
            stackItemFunc();
        });
        stackItem.funcList = [];
        stackItem.isCompleted = true;
    };
    StackWorker.prototype.getByEventType = function (eventType) {
        var result = _.find(this.pool, function (stackItem) {
            return stackItem.eventType === eventType;
        });
        return result;
    };
    return StackWorker;
}());
var StackWorkerFactory = (function () {
    function StackWorkerFactory() {
    }
    Object.defineProperty(StackWorkerFactory, "StackWorker", {
        get: function () {
            if (!this.stackWorker) {
                this.stackWorker = new StackWorker();
            }
            return this.stackWorker;
        },
        enumerable: true,
        configurable: true
    });
    return StackWorkerFactory;
}());
//# sourceMappingURL=stack-worker.js.map